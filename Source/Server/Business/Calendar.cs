using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

using Mediachase.IBN.Database;
using Mediachase.Ibn;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Calendar.
	/// </summary>
	public class Calendar
	{
		private const int dayDuration = 1440;	 // Minutes per day

		#region enum DaysOfWeek
		public enum DaysOfWeek
		{
			Monday = 1,
			Tuesday = 2,
			Wednesday = 3,
			Thursday = 4,
			Friday = 5,
			Saturday = 6,
			Sunday = 7
		}
		#endregion

		#region enum TimeType
		public enum TimeType
		{
			Off = 1,			// нерабочее время
			CalendarEntry = 2,	// календарное событие
			WorkUrgent = 3,		// работа с экстренным приоритетом
			WorkVeryHigh = 4,	// работа с очень высоким приоритетом
			WorkHigh = 5,		// работа с высоким приоритетом
			WorkNormal = 6,		// работа с нормальным приоритетом
			WorkLow = 7,		// работа с низким приоритетом
			Free = 8,			// свободное рабочее время
			Past = 9,			// время в прошлом
			WorkPinnedUp = 10	// закреплённая работа
		}
		#endregion

		#region Create
		public static int Create(int ProjectId, string CalendarName)
		{
			return Create(ProjectId, CalendarName, Security.CurrentUser.TimeZoneId);
		}

		public static int Create(int ProjectId, string CalendarName, int TimeZoneId)
		{
			int CalendarId = -1;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				CalendarId = DBCalendar.GetCalendarByProject(ProjectId);
				if (CalendarId > 0)
				{
					DBCalendar.EmptyCalendar(CalendarId);
					DBCalendar.Update(CalendarId, CalendarName, TimeZoneId);
				}
				else
				{
					CalendarId = DBCalendar.Create(CalendarName, TimeZoneId, ProjectId);
				}
				tran.Commit();
			}
			return CalendarId;
		}

		public static int Create(string CalendarName, int CopyFromCalendarId, int TimeZoneId)
		{
			int CalendarId = -1;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				CalendarId = DBCalendar.Create(CalendarName, TimeZoneId);
				DBCalendar.Copy(CopyFromCalendarId, CalendarId);

				tran.Commit();
			}

			return CalendarId;
		}
		#endregion

		#region Update
		public static void Update(int CalendarId, string CalendarName, int TimeZoneId)
		{
			// Check the changing of TimeOffset
			int OldTimeZoneId = User.DefaultTimeZoneId;
			int projectId = -1;
			using(IDataReader reader = DBCalendar.GetCalendar(CalendarId))
			{
				reader.Read();
				OldTimeZoneId = (int)reader["TimeZoneId"];
				projectId = (reader["ProjectId"]==DBNull.Value?-1:(int)reader["ProjectId"]);
			}

			// If TimeOffset changed then get the list of related projects
			ArrayList projects = new ArrayList();
			if (TimeZoneId != OldTimeZoneId)
			{
				if (projectId > 0 && Project.GetIsMSProject(projectId))
					throw new AccessDeniedException();

				using(IDataReader reader = DBProject.GetListProjectsByCalendar(CalendarId))
				{
					while (reader.Read())
						projects.Add((int)reader["ProjectId"]);
				}
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBCalendar.Update(CalendarId, CalendarName, TimeZoneId);
				RecalculateAll(projects);

				tran.Commit();
			}
		}
		#endregion

		#region Delete
		public static void Delete(int CalendarId)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();

			if (CalendarId == PortalConfig.DefaultCalendar)
				return;

			DBCalendar.Delete(CalendarId);
		}
		#endregion

		#region GetCalendar
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId
		/// </summary>
		public static IDataReader GetCalendar(int CalendarId)
		{
			if (CalendarId == 0)
				throw new Exception("Invalid CalendarId");
			return DBCalendar.GetCalendar(CalendarId);
		}
		#endregion

		#region GetListCalendar
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId, CanDelete
		/// </summary>
		public static IDataReader GetListCalendar()
		{
			return DBCalendar.GetListCalendars();
		}
		#endregion

		#region GetListCalendarsDataTable
		/// <summary>
		/// Reader returns fields:
		///		CalendarId, ProjectId, ProjectName, CalendarName, TimeZoneId, CanDelete
		/// </summary>
		public static DataTable GetListCalendarsDataTable()
		{
			return DBCalendar.GetListCalendarsDataTable();
		}
		#endregion

		#region UpdateWeekdayHours
		public static void UpdateWeekdayHours(
			int CalendarId, byte DayOfWeek, 
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			if (IsMSProject(CalendarId))
				throw new AccessDeniedException();

			To1 = (To1 == 0)?1440:To1;
			To2 = (To2 == 0)?1440:To2;
			To3 = (To3 == 0)?1440:To3;
			To4 = (To4 == 0)?1440:To4;
			To5 = (To5 == 0)?1440:To5;
			int MaxMin = 24*60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5 
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4)
				|| (From1 > 0 && From1 == To1) || (From2 > 0 && From2 == To2)
				|| (From3 > 0 && From3 == To3) || (From4 > 0 && From4 == To4)
				|| (From5 > 0 && From5 == To5))
				throw new InvalidIntervalException();

			ArrayList projects = new ArrayList();
			using(IDataReader reader = DBProject.GetListProjectsByCalendar(CalendarId))
			{
				while (reader.Read())
					projects.Add((int)reader["ProjectId"]);
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBCalendar.DeleteWeekday(CalendarId, DayOfWeek);

				if (From1 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From1, To1);

				if (From2 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From2, To2);

				if (From3 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From3, To3);

				if (From4 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From4, To4);

				if (From5 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From5, To5);

				using(IDataReader reader = DBCalendar.GetListWeekdayHours(CalendarId, 0))
				{
					if (!reader.Read())
						throw new EmptyCalendarException();
				}

				RecalculateAll(projects);

				tran.Commit();
			}
		}
		public static void UpdateWeekdayHoursInternal(
			int CalendarId, byte DayOfWeek, 
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			To1 = (To1 == 0)?1440:To1;
			To2 = (To2 == 0)?1440:To2;
			To3 = (To3 == 0)?1440:To3;
			To4 = (To4 == 0)?1440:To4;
			To5 = (To5 == 0)?1440:To5;
			int MaxMin = 24*60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5 
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4)
				|| (From1 > 0 && From1 == To1) || (From2 > 0 && From2 == To2)
				|| (From3 > 0 && From3 == To3) || (From4 > 0 && From4 == To4)
				|| (From5 > 0 && From5 == To5))
				throw new InvalidIntervalException();

				DBCalendar.DeleteWeekday(CalendarId, DayOfWeek);

				if (From1 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From1, To1);

				if (From2 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From2, To2);

				if (From3 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From3, To3);

				if (From4 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From4, To4);

				if (From5 >= 0)
					DBCalendar.AddWeekdayHours(CalendarId, DayOfWeek, From5, To5);

		}
		#endregion

		#region GetListWeekdayHours
		/// <summary>
		/// Reader return fields:
		///  CalendarId, DayOfWeek, FromTime, ToTime 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <param name="DayOfWeek"></param>
		/// <returns></returns>
		public static IDataReader GetListWeekdayHours(int CalendarId, byte DayOfWeek)
		{
			return DBCalendar.GetListWeekdayHours(CalendarId, DayOfWeek);
		}

		/// <summary>
		/// Reader return fields:
		///  CalendarId, DayOfWeek, FromTime, ToTime 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListWeekdayHours(int CalendarId)
		{
			return DBCalendar.GetListWeekdayHours(CalendarId, 0);
		}
		#endregion

		#region CreateException
		public static int CreateException(
			int CalendarId, 
			DateTime FromDate, DateTime ToDate,
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			if (IsMSProject(CalendarId))
				throw new AccessDeniedException();

			To1 = (To1 == 0)?1440:To1;
			To2 = (To2 == 0)?1440:To2;
			To3 = (To3 == 0)?1440:To3;
			To4 = (To4 == 0)?1440:To4;
			To5 = (To5 == 0)?1440:To5;

			int MaxMin = 24*60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5 
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4))
				throw new InvalidIntervalException();

			FromDate = FromDate.Date;
			ToDate = ToDate.Date.AddDays(1);

			ArrayList projects = new ArrayList();
			using(IDataReader reader = DBProject.GetListProjectsByCalendar(CalendarId))
			{
				while (reader.Read())
					projects.Add((int)reader["ProjectId"]);
			}

			int ExceptionId = -1;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				ExceptionId = DBCalendar.AddException(CalendarId, FromDate, ToDate);

				if (From1 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From1, To1);

				if (From2 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From2, To2);

				if (From3 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From3, To3);

				if (From4 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From4, To4);

				if (From5 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From5, To5);

				RecalculateIntervals(CalendarId, ExceptionId, FromDate, ToDate);

				RecalculateAll(projects);

				tran.Commit();
			}

			return ExceptionId;
		}

		public static int CreateExceptionInternal(
			int CalendarId, 
			DateTime FromDate, DateTime ToDate,
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			To1 = (To1 == 0)?1440:To1;
			To2 = (To2 == 0)?1440:To2;
			To3 = (To3 == 0)?1440:To3;
			To4 = (To4 == 0)?1440:To4;
			To5 = (To5 == 0)?1440:To5;

			int MaxMin = 24*60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5 
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4))
				throw new InvalidIntervalException();

			FromDate = FromDate.Date;
			ToDate = ToDate.Date.AddDays(1);

			int	ExceptionId = DBCalendar.AddException(CalendarId, FromDate, ToDate);

			if (From1 >= 0)
				DBCalendar.AddExceptionHours(ExceptionId, From1, To1);

			if (From2 >= 0)
				DBCalendar.AddExceptionHours(ExceptionId, From2, To2);

			if (From3 >= 0)
				DBCalendar.AddExceptionHours(ExceptionId, From3, To3);

			if (From4 >= 0)
				DBCalendar.AddExceptionHours(ExceptionId, From4, To4);

			if (From5 >= 0)
				DBCalendar.AddExceptionHours(ExceptionId, From5, To5);

			RecalculateIntervals(CalendarId, ExceptionId, FromDate, ToDate);

			return ExceptionId;
		}
		#endregion

		#region UpdateException
		public static void UpdateException(
			int ExceptionId, 
			DateTime FromDate, DateTime ToDate,
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			To1 = (To1 == 0)?1440:To1;
			To2 = (To2 == 0)?1440:To2;
			To3 = (To3 == 0)?1440:To3;
			To4 = (To4 == 0)?1440:To4;
			To5 = (To5 == 0)?1440:To5;

			int MaxMin = 24*60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5 
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4))
				throw new InvalidIntervalException();

			FromDate = FromDate.Date;
			ToDate = ToDate.Date.AddDays(1);

			int CalendarId = DBCalendar.GetCalendarByException(ExceptionId);

			if (IsMSProject(CalendarId))
				throw new AccessDeniedException();

			ArrayList projects = new ArrayList();
			using(IDataReader reader = DBProject.GetListProjectsByCalendar(DBCalendar.GetCalendarByException(ExceptionId)))
			{
				while (reader.Read())
					projects.Add((int)reader["ProjectId"]);
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBCalendar.UpdateException(ExceptionId, FromDate, ToDate);

				DBCalendar.DeleteExceptionHours(ExceptionId);

				if (From1 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From1, To1);

				if (From2 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From2, To2);

				if (From3 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From3, To3);

				if (From4 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From4, To4);

				if (From5 >= 0)
					DBCalendar.AddExceptionHours(ExceptionId, From5, To5);

				RecalculateIntervals(CalendarId, ExceptionId, FromDate, ToDate);

				RecalculateAll(projects);

				tran.Commit();
			}
		}
		#endregion

		#region DeleteException
		public static void DeleteException(int ExceptionId)
		{
			int calendarId = DBCalendar.GetCalendarByException(ExceptionId);
			if (IsMSProject(calendarId))
				throw new AccessDeniedException();

			ArrayList projects = new ArrayList();
			using(IDataReader reader = DBProject.GetListProjectsByCalendar(DBCalendar.GetCalendarByException(ExceptionId)))
			{
				while (reader.Read())
					projects.Add((int)reader["ProjectId"]);
			}

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBCalendar.DeleteException(ExceptionId);
				RecalculateAll(projects);

				tran.Commit();
			}
		}
		#endregion

		#region GetException
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, CalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetException(int ExceptionId)
		{
			return DBCalendar.GetException(ExceptionId);
		}
		#endregion

		#region GetListExceptions
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, CalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListExceptions(int CalendarId)
		{
			return DBCalendar.GetListExceptions(CalendarId);
		}
		#endregion

		#region GetListExceptionHours
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, FromTime, ToTime 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetListExceptionHours(int ExceptionId)
		{
			return DBCalendar.GetListExceptionHours(ExceptionId);
		}
		#endregion

		#region GetListExceptionHoursByCalendar
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, CalendarId, FromDate, ToDate, FromTime, ToTime
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListExceptionHoursByCalendar(int CalendarId)
		{
			return DBCalendar.GetListExceptionHoursByCalendar(CalendarId);
		}
		#endregion

		#region GetCalendarByException
		public static int GetCalendarByException(int ExceptionId)
		{
			return DBCalendar.GetCalendarByException(ExceptionId);
		}
		#endregion

		#region RecalculateAll
		public static void RecalculateAll(ArrayList projects)
		{
			foreach (int project in projects)
				Project.RecalculateAll(project);
		}
		#endregion

		#region RecalculateIntervals
		public static void RecalculateIntervals(int CalendarId, int ExceptionId, DateTime FromDate, DateTime ToDate)
		{
			DataRow row;
			DataTable table = new DataTable();
			table.Columns.Add("OverlapedExceptionId", typeof(int));
			table.Columns.Add("OverlapedFromDate", typeof(DateTime));
			table.Columns.Add("OverlapedToDate", typeof(DateTime));

			using(IDataReader reader = DBCalendar.GetOverlapedExceptions(ExceptionId, FromDate, ToDate))
			{
				while (reader.Read())
				{
					row = table.NewRow();
					row["OverlapedExceptionId"] = (int)reader["ExceptionId"];
					row["OverlapedFromDate"] = (DateTime)reader["FromDate"];
					row["OverlapedToDate"] = (DateTime)reader["ToDate"];
					table.Rows.Add(row);
				}
			}

			foreach (DataRow dr in table.Rows)
			{
				int OverlapedExceptionId = (int)dr["OverlapedExceptionId"];
				DateTime OverlapedFromDate = (DateTime)dr["OverlapedFromDate"];
				DateTime OverlapedToDate = (DateTime)dr["OverlapedToDate"];

				if (FromDate <= OverlapedFromDate)
				{
					if (ToDate < OverlapedToDate)
					{
						// Cut the Overlaped Exception from left
						DBCalendar.UpdateException(OverlapedExceptionId, ToDate, OverlapedToDate);
					}
					else	// ToDate >= OverlapedToDate
					{
						// Delete the Overlaped Exception
						DBCalendar.DeleteException(OverlapedExceptionId);
					}
				}
				else	// FromDate > OverlapedFromDate
				{
					if (ToDate < OverlapedToDate)
					{
						// Double cut the Overlaped Exception
						DBCalendar.UpdateException(OverlapedExceptionId, OverlapedFromDate, FromDate);
						int NewOverlapedExceptionId = DBCalendar.AddException(CalendarId, ToDate, OverlapedToDate);
						DBCalendar.CopyExceptionHours(OverlapedExceptionId, NewOverlapedExceptionId);
					}
					else	// ToDate >= OverlapedToDate
					{
						// Cut the Overlaped Exception from right
						DBCalendar.UpdateException(OverlapedExceptionId, OverlapedFromDate, FromDate);
					}
				}
			}
		}
		#endregion

		#region IsMSProject
		public static bool IsMSProject(int calendarId)
		{
			bool retval = false;
			int projectId = -1;
			using (IDataReader reader = DBCalendar.GetCalendar(calendarId))
			{
				if (reader.Read() && (reader["ProjectId"] != DBNull.Value))
					projectId = (int)reader["ProjectId"];
			}

			if (projectId > 0)
				retval = Project.GetIsMSProject(projectId);

			return retval;
		}
		#endregion

		//===== Resource Itilization and UserCalendars ======
		#region GetResourceUtilization
		/// <summary>
		/// UserId, Start, Finish, Type, ObjectId, ObjectTypeId, ObjectName, Duration, PercentCompleted,
		/// PriorityId, StateId, FinishDate, Highlight, IsNewMessage, IsOverdue, AssignmentId, AssignmentName
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="curDate"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <param name="objectTypes"></param>
		/// <param name="highlightedItems"></param>
		/// <param name="getPastTime"></param>
		/// <param name="getOffTime"></param>
		/// <param name="mergeBreaks"></param>
		/// <param name="strictBorder"></param>
		/// <returns></returns>
		public static DataTable GetResourceUtilization(
			int userId,
			DateTime curDate,
			DateTime fromDate, 
			DateTime toDate,
			ArrayList objectTypes,	/*ArrayList arr = new ArrayList();arr.Add(ObjectTypes.Task);arr.Add(ObjectTypes.ToDo);arr.Add(ObjectTypes.CalendarEntry);arr.Add(ObjectTypes.Document);arr.Add(ObjectTypes.Issue);*/
			List<KeyValuePair<int, int>> highlightedItems,	/* key:ObjectId, value:ObjectTypeId */
			bool getPastTime,
			bool getOffTime,
			bool mergeBreaks,
			bool strictBorder)
		{
			/* Методика расчёта:
			 * 1. Создаётся массив с размерностью, равной количеству минут между текущеим времени и концом диапазона
			 *    (для удобства лучше выровнять границы массив на начало текущего дня и на конец последнего дня)
			 * 2. Массив заполняется значениеми TimeType.Past (прошедшее время) и TimeType.Off.
			 *    Т.е. по умолчанию всё время - нерабочее. 
			 * 3. На основании настроек календаря выставляем в массиве рабочее время (TimeType.Free)
			 * 4. Добавляем информацию о нерабочем времени в результирующую таблицу.
			 * 5. Достаём все мероприятия за период, заполняем массив значениями TimeType.CalendarEntry
			 *    и одновременно добавляем соответствующую инфу в результирующую таблицу 
			 * 6. Достаём WorkPinnedUp, заполняем массив значениями TimeTypeю.Sticked 
			 *    и одновременно добавляем соответствующую инфу в результирующую таблицу
			 * 7. Достаём задачи, поручения, инциденты (отсортированные в нужном поорядке) и
			 *    распределяем инфу о них в массиве со значениями TimeType.WorkUrgent - 
			 *    TimeTime.WorkLow (в зависимости от приоритета) и одновременно добавляем 
			 *    соответствующую инфу в результирующую таблицу. 
			 * 
			 * На выходе будет таблица с нужными данными. 
			 * Потом при необходимости можно будет сгруппировать элементы по приоритетам
			 * 
			 * - параметр "mergeBreaks" определяет, надо ли объединять разорванную перерывом работу 
			 * - параметр "strictBorder" определяет, что будет добавлено в результирующую таблицу, если  
			 *   работа попадает на границу временного интервала. Если true, то временные рамки работы
			 *   будут обрезаны по границам fromDate и toDate. Если false, то будут использованы 
			 *   реальные временные рамки работы
			 */

			// round dates to minutes
			fromDate = fromDate.Date.AddHours(fromDate.Hour).AddMinutes(fromDate.Minute);
			toDate = toDate.Date.AddHours(toDate.Hour).AddMinutes(toDate.Minute);
			curDate = curDate.Date.AddHours(curDate.Hour).AddMinutes(curDate.Minute);

			if (fromDate >= toDate)
				throw new ArgumentException("fromDate, toDate");

			DataTable resultTable;	// Resulting table
			int calendarId = -1;	// Calendar Id
			int userCalendarId = -1;	// UserCalendar Id
			List<TimeType> timeList;	// Main array. It contains information about 
										// every minute allocation from the 00:00 of current date
										// to 24:00 of the toDate
			int freeTimeCounter;	// Counter of free time
			DateTime curCalDate;	// Current Date in Calendar timezone
			DateTime fromCalDate;	// FromDate in Calendar timezone
			DateTime toCalDate;		// ToDate in Calendar timezone
			int curMinute;			// Total minutes elapsed from 00:00 of current day
			int fromMinute;			// Total minutes elapsed from 00:00 of current day to fromDate (curDate < fromdate)
									// or total minutes elapsed from 00:00 of current day (fromdate > curDate)
			int toMinute;			// Total minutes elapsed from 00:00 of current day to toDate
			TimeSpan ts;			// Auxiliary variable
			CalendarHelper calendarHelper;	// Auxiliary class
			int intervalDuration;	// total minutes from 00:00 of current date to 24:00 of the toDate

			bool getTasks = objectTypes.Contains(ObjectTypes.Task);
			bool getTodo = objectTypes.Contains(ObjectTypes.ToDo);
			bool getIncidents = objectTypes.Contains(ObjectTypes.Issue);
			bool getDocuments = objectTypes.Contains(ObjectTypes.Document);
			DataTable objectTable;

			#region PreStep: initial actions
			// Resulting DataTable definition
			resultTable = new DataTable();
			resultTable.Locale = CultureInfo.InvariantCulture;

			resultTable.Columns.Add("UserId", typeof(int));
			resultTable.Columns.Add("Start", typeof(DateTime));
			resultTable.Columns.Add("Finish", typeof(DateTime));
			resultTable.Columns.Add("Type", typeof(TimeType));
			resultTable.Columns.Add("ObjectId", typeof(int));
			resultTable.Columns.Add("ObjectTypeId", typeof(int));
			resultTable.Columns.Add("ObjectName", typeof(string));
			resultTable.Columns.Add("Duration", typeof(int));
			resultTable.Columns.Add("PercentCompleted", typeof(int));
			resultTable.Columns.Add("PriorityId", typeof(int));
			resultTable.Columns.Add("StateId", typeof(int));
			resultTable.Columns.Add("FinishDate", typeof(DateTime));
			resultTable.Columns.Add("Highlight", typeof(bool));
			resultTable.Columns.Add("IsNewMessage", typeof(bool));	// for incidents
			resultTable.Columns.Add("IsOverdue", typeof(bool));
			resultTable.Columns.Add("AssignmentId", typeof(string));
			resultTable.Columns.Add("AssignmentName", typeof(string));
			resultTable.Columns.Add("ProjectTitle", typeof(string));

			DateTime fromUtcDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(fromDate);
			DateTime toUtcDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(toDate);
			DateTime curUtcDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(curDate);

			// if we have past time
			if (fromDate < curDate)
			{
				if (curDate < toDate)
				{
					if (getPastTime)
					{
						AddRow(resultTable, userId, fromDate, curDate, TimeType.Past);
					}
				}
				else // all time is past
				{
					if (getPastTime)
					{
						AddRow(resultTable, userId, fromDate, toDate, TimeType.Past);
					}
					return resultTable;
				}
			}

			// Get User Calendar info
			using (IDataReader reader = GetUserCalendarByUser(userId))
			{
				if (reader.Read())
				{
					calendarId = (int)reader["CalendarId"];
					userCalendarId = (int)reader["UserCalendarId"];
				}
			}

			// Check user calendar existance
			if (calendarId < 0)	// if calendar doesn't exist
			{
				if (getOffTime)
				{
					if (strictBorder)
					{
						AddRow(resultTable,
							userId,
							(fromDate > curDate) ? fromDate : curDate,
							toDate,
							TimeType.Off);
					}
					else
					{
						AddRow(resultTable,
							userId,
							curDate,
							toDate.AddDays(1),	// no border in graph
							TimeType.Off);
					}
					
				}

				return resultTable;
			}

			UserLight ul = UserLight.Load(userId);
			int timeZoneId = ul.TimeZoneId;

			// Calendar TimeZone
			McTimeZone timeZoneInfo = McTimeZone.Load(timeZoneId);
			if (timeZoneInfo == null)
				throw new ArgumentException("Unknown TimeZoneId = '" + timeZoneId.ToString() + "'.");
			TimeZone calendarTimeZone = new Mediachase.Ibn.Data.UserTimeZone("Calendar Time Zone",
				timeZoneInfo.Bias,
				timeZoneInfo.DaylightBias,
				timeZoneInfo.DaylightMonth,
				timeZoneInfo.DaylightDayOfWeek,
				timeZoneInfo.DaylightWeek,
				timeZoneInfo.DaylightHour,
				timeZoneInfo.StandardBias,
				timeZoneInfo.StandardMonth,
				timeZoneInfo.StandardDayOfWeek,
				timeZoneInfo.StandardWeek,
				timeZoneInfo.StandardHour);

			curCalDate = calendarTimeZone.ToLocalTime(curUtcDate);
			fromCalDate = calendarTimeZone.ToLocalTime(fromUtcDate);
			toCalDate = calendarTimeZone.ToLocalTime(toUtcDate);

			curMinute = curCalDate.Hour * 60 + curCalDate.Minute;
			if (fromDate < curDate)
			{
				fromMinute = curMinute;
			}
			else
			{
				ts = fromCalDate.Subtract(curCalDate.Date);
				fromMinute = (int)ts.TotalMinutes;
			}
			ts = toCalDate.Subtract(curCalDate.Date);
			toMinute = (int)ts.TotalMinutes;
			#endregion

			#region Step 1: Array creation
			// leveling to the start of date
			if (toCalDate == toCalDate.Date)
				ts = toCalDate.Date.Subtract(curCalDate.Date);
			else
				ts = toCalDate.Date.AddDays(1).Subtract(curCalDate.Date);
			intervalDuration = (int)ts.TotalMinutes;

			if (intervalDuration == 0)
			{
				return resultTable;
			}

			// Calendar Helper
			calendarHelper = new CalendarHelper(curCalDate.Date, calendarTimeZone);
			calendarHelper.FromCalDate = fromCalDate;
			calendarHelper.ToCalDate = toCalDate;
			calendarHelper.IntervalDuration = intervalDuration;
			calendarHelper.HighlightedItems = highlightedItems;

			// array creation
			timeList = new List<TimeType>(intervalDuration);
			#endregion

			#region Step 2: Default array allocation
			for (int i = 0; i < intervalDuration; i++)
			{
				if (i < curMinute)
					timeList.Add(TimeType.Past);
				else
					timeList.Add(TimeType.Off);
			}
			#endregion

			#region Step 3: Data from calendar
			ProcessCalendarInfo(timeList, curCalDate, toCalDate, calendarId, userCalendarId, toMinute);
			#endregion

			#region Step 4: Add information about time off to resulting table
			TimeType curType = TimeType.Past;
			int startMinute = 0;
			int endMinute;
			freeTimeCounter = 0;

			for (int i = 0; i < intervalDuration; i++)
			{
				if (timeList[i] == TimeType.Free)
					freeTimeCounter++;

				if (timeList[i] != curType)	// if type is changed
				{
					endMinute = i;
					if (getOffTime && curType == TimeType.Off && startMinute < toMinute && endMinute > fromMinute)
					{
						if (strictBorder)
						{
							if (startMinute < fromMinute)
								startMinute = fromMinute;
							if (endMinute > toMinute)
								endMinute = toMinute;
						}
						if (startMinute < endMinute)
						{
							AddRow(resultTable, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(endMinute), TimeType.Off);
						}
					}
					startMinute = i;
					curType = timeList[i];
					continue;
				}
			}

			// Last interval
			if (getOffTime && curType == TimeType.Off && startMinute < toMinute)
			{
				endMinute = intervalDuration + 24*60;	// hide border
				if (strictBorder)
				{
					if (startMinute < fromMinute)
						startMinute = fromMinute;
					if (endMinute > toMinute)
						endMinute = toMinute;
				}
				if (startMinute < endMinute)
				{
					AddRow(resultTable, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(endMinute), TimeType.Off);
				}
			}

			if (freeTimeCounter == 0)
				return resultTable;

			//----------------------
			#endregion

			#region Step 5: Process Events
			if (objectTypes.Contains(ObjectTypes.CalendarEntry))
			{
				int entryType = (int)CalendarView.CalendarFilter.Appointment | (int)CalendarView.CalendarFilter.Event | (int)CalendarView.CalendarFilter.Meeting;
				DataTable events = CalendarView.GetListCalendarEntriesByUser(curDate.Date, (toDate == toDate.Date) ? toDate : toDate.Date.AddDays(1), true, true, false, entryType, userId);

				// ID, Type, Title, Description, StartDate, FinishDate, ShowStartDate, ShowFinishDate, StateId
				foreach (DataRow row in events.Rows)
				{
					if ((int)row["StateId"] == (int)ObjectStates.Completed || (int)row["StateId"] == (int)ObjectStates.Suspended)
						continue;

					DateTime objectStartDate = calendarHelper.GetCalendarDate((DateTime)row["StartDate"]);
					DateTime objectFinishDate = calendarHelper.GetCalendarDate((DateTime)row["FinishDate"]);

					if (objectStartDate >= ((toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1)) || objectFinishDate <= curCalDate.Date)
						continue;	// we don't need objects which have dates outside our period

					// round dates to the diapason boundaries
					if (objectStartDate < curCalDate.Date)
						objectStartDate = curCalDate.Date;
					if (objectFinishDate > ((toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1)))
						objectFinishDate = (toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1);

					// Get number of minutes between objectStartDate/objectFinishDate and curCalDate.Date
					ts = objectStartDate.Subtract(curCalDate.Date);
					startMinute = (int)ts.TotalMinutes;
					ts = objectFinishDate.Subtract(curCalDate.Date);
					endMinute = (int)ts.TotalMinutes;

					// feel the array
					for (int i = startMinute; i < endMinute; i++)
					{
						if (timeList[i] != TimeType.Past) // we shouldn't change the past time
							timeList[i] = TimeType.CalendarEntry;
					}

					// Add info to result table
					if (startMinute < toMinute && endMinute > fromMinute)
					{
						int duration = endMinute - startMinute;
						if (strictBorder)
						{
							if (startMinute < fromMinute)
								startMinute = fromMinute;
							if (endMinute > toMinute)
								endMinute = toMinute;
						}
						else if (startMinute < curMinute)
						{
							startMinute = curMinute;
						}

						if (startMinute < endMinute)
						{
							AddRow(resultTable, highlightedItems, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(endMinute), TimeType.CalendarEntry, (int)row["ID"], (int)ObjectTypes.CalendarEntry, row["Title"].ToString(), duration, 0, (int)row["PriorityId"], (int)row["StateId"], (DateTime)row["FinishDate"], null, null, String.Empty, false, false, false);
						}
					}
				}

				freeTimeCounter = 0;
				for (int i = 0; i < intervalDuration; i++)
				{
					if (timeList[i] == TimeType.Free)
						freeTimeCounter++;
				}
				if (freeTimeCounter == 0)
					return resultTable;
			}
			#endregion

			#region Step 6: Process WorkPinnedUp
			objectTable = DBCalendar.GetListStickedObjectsUtc(userId, getTasks, getTodo, getIncidents, getDocuments);

			ProcessObjectTable(timeList, curCalDate, userId, objectTable, resultTable, freeTimeCounter, fromMinute, toMinute, calendarHelper, mergeBreaks, strictBorder, true);
			#endregion

			#region Step 7: Process Tasks, Todo, Incidents
			objectTable = DBCalendar.GetListObjectsInPriorityOrderUtc(userId, curUtcDate, toUtcDate, getTasks, getTodo, getIncidents, getDocuments);

			ProcessObjectTable(timeList, curCalDate, userId, objectTable, resultTable, freeTimeCounter, fromMinute, toMinute, calendarHelper, mergeBreaks, strictBorder, false);
			#endregion

			return resultTable;
		}
		#endregion

		#region ProcessCalendarInfo
		private static void ProcessCalendarInfo(List<TimeType> timeList, DateTime curCalDate, DateTime toCalDate, int calendarId, int userCalendarId, int toMinute)
		{
			#region loop by days
			int dayNum = 0;
			while (curCalDate.Date.AddDays(dayNum) < ((toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1)))
			{
				byte dayOfWeek = (byte)curCalDate.Date.AddDays(dayNum).DayOfWeek;
				if (dayOfWeek == 0)
					dayOfWeek = 7;

				using (IDataReader reader = GetListWeekdayHours(calendarId, dayOfWeek))
				{
					// FromTime, ToTime 
					while (reader.Read())
					{
						int fromTime = (int)reader["FromTime"];
						int toTime = (int)reader["ToTime"];
						if (toTime == 0)
							toTime = dayDuration;	// Minutes per day

						for (int i = dayNum * dayDuration + fromTime; i < dayNum * dayDuration + toTime; i++)
						{
							if (timeList[i] == TimeType.Off)
								timeList[i] = TimeType.Free;
						}
					}
				}

				dayNum++;
			}
			#endregion


			#region loop by Calendar Exceptions and collect info about fit exceptions
			List<ExceptionInfo> exceptionList = new List<ExceptionInfo>();
			using (IDataReader reader = GetListExceptions(calendarId))
			{
				// reader contains info: ExceptionId, FromDate, ToDate
				while (reader.Read())
				{
					DateTime fromExceptionDate = (DateTime)reader["FromDate"];
					DateTime toExceptionDate = ((DateTime)reader["ToDate"]).AddDays(1);
					if (fromExceptionDate < toCalDate && toExceptionDate > curCalDate)
					{
						// we need only interval between curCalDate.Date and toCalDate.Date.AddDays(1)
						if (fromExceptionDate < curCalDate.Date)
							fromExceptionDate = curCalDate.Date;
						if (toExceptionDate > ((toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1)))
							toExceptionDate = (toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1);

						// save exception info
						exceptionList.Add(new ExceptionInfo((int)reader["ExceptionId"], fromExceptionDate, toExceptionDate));
					}
				}
			}
			#endregion

			#region loop by fit Calendar Exceptions and process exception hours
			foreach (ExceptionInfo exceptionInfo in exceptionList)
			{
				// replace all time in exception period to TimeType.Off 
				TimeSpan ts;
				int start = 0;
				int finish = toMinute;
				if (exceptionInfo.fromDate > curCalDate)
				{
					ts = exceptionInfo.fromDate.Subtract(curCalDate.Date);
					start = (int)ts.TotalMinutes;
				}
				if (exceptionInfo.toDate < toCalDate)
				{
					ts = exceptionInfo.toDate.Subtract(curCalDate.Date);
					finish = (int)ts.TotalMinutes;
				}
				for (int i = start; i < finish; i++)
				{
					if (timeList[i] != TimeType.Past)
						timeList[i] = TimeType.Off;
				}

				// add information about working hours
				using (IDataReader reader = GetListExceptionHours(exceptionInfo.exceptionId))
				{
					// reader contains info: FromTime, ToTime
					while (reader.Read())
					{
						int fromTime = (int)reader["FromTime"];
						int toTime = (int)reader["ToTime"];
						if (toTime == 0)
							toTime = dayDuration;

						// loop by exception period and replace exception time to TimeType.Free 
						dayNum = 0;
						while (exceptionInfo.fromDate.AddDays(dayNum) < exceptionInfo.toDate)
						{
							ts = exceptionInfo.fromDate.AddDays(dayNum).AddMinutes(fromTime).Subtract(curCalDate.Date);
							start = (int)ts.TotalMinutes;

							ts = exceptionInfo.fromDate.AddDays(dayNum).AddMinutes(toTime).Subtract(curCalDate.Date);
							finish = (int)ts.TotalMinutes;

							for (int i = start; i < finish; i++)
							{
								if (timeList[i] == TimeType.Off)
									timeList[i] = TimeType.Free;
							}

							dayNum++;
						}
					}
				}
			}
			#endregion


			#region loop by UserCalendar Exceptions and collect info about fit exceptions
			exceptionList = new List<ExceptionInfo>();
			using (IDataReader reader = GetListUserCalendarExceptions(userCalendarId))
			{
				// reader contains info: ExceptionId, FromDate, ToDate
				while (reader.Read())
				{
					DateTime fromExceptionDate = (DateTime)reader["FromDate"];
					DateTime toExceptionDate = ((DateTime)reader["ToDate"]).AddDays(1);
					if (fromExceptionDate < toCalDate && toExceptionDate > curCalDate)
					{
						// we need only interval between curCalDate.Date and toCalDate.Date.AddDays(1)
						if (fromExceptionDate < curCalDate.Date)
							fromExceptionDate = curCalDate.Date;
						if (toExceptionDate > ((toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1)))
							toExceptionDate = (toCalDate.Date == toCalDate) ? toCalDate : toCalDate.Date.AddDays(1);

						// save exception info
						exceptionList.Add(new ExceptionInfo((int)reader["ExceptionId"], fromExceptionDate, toExceptionDate));
					}
				}
			}
			#endregion

			#region loop by fit UserCalendar Exceptions and process exception hours
			foreach (ExceptionInfo exceptionInfo in exceptionList)
			{
				// replace all time in exception period to TimeType.Off 
				TimeSpan ts;
				int start = 0;
				int finish = toMinute;
				if (exceptionInfo.fromDate > curCalDate)
				{
					ts = exceptionInfo.fromDate.Subtract(curCalDate.Date);
					start = (int)ts.TotalMinutes;
				}
				if (exceptionInfo.toDate < toCalDate)
				{
					ts = exceptionInfo.toDate.Subtract(curCalDate.Date);
					finish = (int)ts.TotalMinutes;
				}
				for (int i = start; i < finish; i++)
				{
					if (timeList[i] != TimeType.Past)
						timeList[i] = TimeType.Off;
				}

				// add information about working hours
				using (IDataReader reader = GetListUserCalendarExceptionHours(exceptionInfo.exceptionId))
				{
					// reader contains info: FromTime, ToTime
					while (reader.Read())
					{
						int fromTime = (int)reader["FromTime"];
						int toTime = (int)reader["ToTime"];
						if (toTime == 0)
							toTime = dayDuration;

						// loop by exception period and replace exception time to TimeType.Free 
						dayNum = 0;
						while (exceptionInfo.fromDate.AddDays(dayNum) < exceptionInfo.toDate)
						{
							ts = exceptionInfo.fromDate.AddDays(dayNum).AddMinutes(fromTime).Subtract(curCalDate.Date);
							start = (int)ts.TotalMinutes;

							ts = exceptionInfo.fromDate.AddDays(dayNum).AddMinutes(toTime).Subtract(curCalDate.Date);
							finish = (int)ts.TotalMinutes;

							for (int i = start; i < finish; i++)
							{
								if (timeList[i] == TimeType.Off)
									timeList[i] = TimeType.Free;
							}

							dayNum++;
						}
					}
				}
			}
			#endregion
		}
		#endregion

		#region ProcessObjectTable
		private static void ProcessObjectTable(
			List<TimeType> timeList, 
			DateTime curCalDate, 
			int userId, 
			DataTable objectTable, 
			DataTable resultTable, 
			int freeTimeCounter,
			int fromMinute,
			int toMinute,
			CalendarHelper calendarHelper,
			bool mergeBreaks,
			bool strictBorder,
			bool sticked)
		{
			// we should extend to left the first work item
			bool extendToLeft = (mergeBreaks && curCalDate.Date < calendarHelper.FromCalDate.Date) ? true : false;	

			foreach (DataRow row in objectTable.Rows)
			{
				// ObjectId, ObjectTypeId, ObjectName, PriorityId, StartDate (UTC), FinishDate (UTC), 
				// CreationDate (UTC), TaskTime, PercentCompleted, TaskTimeLeft, FinishDateLeft (UTC)

				int duration = (int)row["TaskTimeLeft"];
				int originalDuration = duration;
				int objectId = (int)row["ObjectId"];
				int objectTypeId = (int)row["ObjectTypeId"];
				int taskTimeLeft = (int)row["TaskTimeLeft"];
				string objectName = row["ObjectName"].ToString();
				int percentCompleted = (int)row["PercentCompleted"];
				int priorityId = (int)row["PriorityId"];
				int stateId = (int)row["StateId"];
				bool isNewMessage = (bool)row["IsNewMessage"];
				bool isOverdue = (bool)row["IsOverdue"];
				DateTime finishDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime((DateTime)row["FinishDate"]);
				string assignmentId = row["AssignmentId"].ToString();
				string assignmentName = row["AssignmentName"].ToString();
				string projectTitle = string.Empty;
				if (row["ProjectTitle"] != DBNull.Value)
					projectTitle = row["ProjectTitle"].ToString();

				int realStartMinute = -1;	// the position of the first minute allocation

				TimeType curType;
				if (!sticked)
				{
					switch (priorityId)
					{
						case (int)Priority.Urgent:
							curType = TimeType.WorkUrgent;
							break;
						case (int)Priority.VeryHigh:
							curType = TimeType.WorkVeryHigh;
							break;
						case (int)Priority.High:
							curType = TimeType.WorkHigh;
							break;
						case (int)Priority.Low:
							curType = TimeType.WorkLow;
							break;
						default:
							curType = TimeType.WorkNormal;
							break;
					}
				}
				else
				{
					curType = TimeType.WorkPinnedUp;
				}

				DateTime objectStartDate = calendarHelper.GetCalendarDateFromUtc((DateTime)row["StartDate"]);
				if (objectStartDate < curCalDate.Date)
					objectStartDate = curCalDate.Date;
				TimeSpan ts = objectStartDate.Subtract(curCalDate.Date);
				int minStart = (int)ts.TotalMinutes;

				bool continueFlag = false;

				int startMinute = -1;
				int endMinute;
				for (int i = minStart; i < calendarHelper.IntervalDuration; i++)
				{
					// we should find free time
					if (timeList[i] == TimeType.Free)
					{
						if (realStartMinute == 0)	// total start
							realStartMinute = i;

						if (startMinute < 0)	// start after some breaks
							startMinute = i;

						if (originalDuration <= 0)	// sticked objects can have 0 duration
						{
							AddRow(resultTable, calendarHelper.HighlightedItems, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(startMinute), curType, objectId, objectTypeId, objectName, taskTimeLeft, percentCompleted, priorityId, stateId, finishDate, assignmentId, assignmentName, projectTitle, mergeBreaks, isNewMessage, isOverdue);
							continueFlag = true;
							break;
						}


						timeList[i] = curType;
						duration--;
						freeTimeCounter--;

						if (duration == 0 || freeTimeCounter == 0)
						{
							endMinute = i + 1;

							if (startMinute < toMinute && endMinute > fromMinute)
							{
								startMinute = CalculateStartMinute(startMinute, realStartMinute, fromMinute, calendarHelper, timeList, ref extendToLeft, mergeBreaks, strictBorder);
								endMinute = CalculateEndMinute(endMinute, toMinute, calendarHelper, timeList, duration, mergeBreaks, strictBorder);

								if (startMinute < endMinute)
								{
									AddRow(resultTable, calendarHelper.HighlightedItems, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(endMinute), curType, objectId, objectTypeId, objectName, taskTimeLeft, percentCompleted, priorityId, stateId, finishDate, assignmentId, assignmentName, projectTitle, mergeBreaks, isNewMessage, isOverdue);
								}
							}
							startMinute = -1;
							break;
						}
					}
					else // free interval break
					{
						if (startMinute >= 0) 
						{
							endMinute = i;

							if (startMinute < toMinute && endMinute > fromMinute)
							{
								startMinute = CalculateStartMinute(startMinute, realStartMinute, fromMinute, calendarHelper, timeList, ref extendToLeft, mergeBreaks, strictBorder);
								endMinute = CalculateEndMinute(endMinute, toMinute, calendarHelper, timeList, duration, mergeBreaks, strictBorder);

								if (startMinute < endMinute)
								{
									AddRow(resultTable, calendarHelper.HighlightedItems, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(endMinute), curType, objectId, objectTypeId, objectName, taskTimeLeft, percentCompleted, priorityId, stateId, finishDate, assignmentId, assignmentName, projectTitle, mergeBreaks, isNewMessage, isOverdue);
								}
							}
							startMinute = -1;
						}
					}
				}

				if (continueFlag)
					continue;

				// last interval
				if (startMinute >= 0) 
				{
					endMinute = calendarHelper.IntervalDuration;

					if (startMinute < toMinute && endMinute > fromMinute)
					{
						startMinute = CalculateStartMinute(startMinute, realStartMinute, fromMinute, calendarHelper, timeList, ref extendToLeft, mergeBreaks, strictBorder);
						endMinute = CalculateEndMinute(endMinute, toMinute, calendarHelper, timeList, duration, mergeBreaks, strictBorder);
						if (startMinute < endMinute)
						{
							AddRow(resultTable, calendarHelper.HighlightedItems, userId, calendarHelper.GetUserDate(startMinute), calendarHelper.GetUserDate(endMinute), curType, objectId, objectTypeId, objectName, taskTimeLeft, percentCompleted, priorityId, stateId, finishDate, assignmentId, assignmentName, projectTitle, mergeBreaks, isNewMessage, isOverdue);
						}
					}
				}

				if (freeTimeCounter == 0)
				{
					break;
				}
			}
		}
		#endregion

		#region CalculateStartMinute
		private static int CalculateStartMinute(int startMinute, int realStartMinute, int fromMinute, CalendarHelper calendarHelper, List<TimeType> timeList, ref bool extendToLeft, bool mergeBreaks, bool strictBorder)
		{
			int savedStart = startMinute;
			if (strictBorder && startMinute < fromMinute)
				startMinute = fromMinute;

			if (extendToLeft)
			{
				// at the left we can have events or another work, so we should check this
				bool anotherWorkFound = false;
				bool freeTimeFound = false;
				for (int j = startMinute - 1; j >= 0; j--)
				{
					if (timeList[j] != TimeType.Off)
					{
						if (timeList[j] == TimeType.Free)	// free work time found. We can't extend to left
						{
							freeTimeFound = true;
							break;
						}

						if (timeList[j] != timeList[savedStart])	// types are different
							extendToLeft = false;
						else
							startMinute = j + 1;

						anotherWorkFound = true;
						break;
					}
				}

				if (!anotherWorkFound)
				{
					extendToLeft = false;

					if (strictBorder)
						startMinute = fromMinute;
					else if (!freeTimeFound)
						startMinute = (realStartMinute < 0) ? 0 : realStartMinute;
				}
			}

			return startMinute;
		}
		#endregion

		#region CalculateEndMinute
		private static int CalculateEndMinute(int endMinute, int toMinute, CalendarHelper calendarHelper, List<TimeType> timeList, int duration, bool mergeBreaks, bool strictBorder)
		{
			if (strictBorder && endMinute > toMinute)
				endMinute = toMinute;

			// if we are going to merge breaks we should extend the right edge in the case if 
			// interval is finished, but duration is still not 0
			if (duration > 0 && mergeBreaks)
			{
				// at the right we can have event or another work, so we should check this
				bool anotherWorkFound = false;
				for (int j = endMinute; j < calendarHelper.IntervalDuration; j++)
				{
					if (timeList[j] != TimeType.Off)
					{
						endMinute = j;
						anotherWorkFound = true;
						break;
					}
				}

				if (!anotherWorkFound)
				{
					if (strictBorder)
					{
						endMinute = calendarHelper.IntervalDuration;
					}
					else
					{
						// it's not a real endMinute, because we do not have information 
						// about time off of the next day. Just imitation for drawing purpose.
						endMinute = calendarHelper.IntervalDuration + duration;
					}
				}
			}
			return endMinute;
		}
		#endregion

		#region AddRow
		private static void AddRow(DataTable resultTable, int userId, DateTime start, DateTime finish, TimeType type)
		{
			DataRow row = resultTable.NewRow();
			row["UserId"] = userId;
			row["Start"] = start;
			row["Finish"] = finish;
			row["Type"] = type;
			row["ObjectId"] = -1;
			row["ObjectTypeId"] = -1;
			row["ObjectName"] = String.Empty;
			row["Highlight"] = false;
			row["IsNewMessage"] = false;
			row["IsOverdue"] = false;
			row["ProjectTitle"] = String.Empty;
			resultTable.Rows.Add(row);
		}

		private static void AddRow(DataTable resultTable, List<KeyValuePair<int, int>> highlightedItems,
			int userId, DateTime start, DateTime finish,
			TimeType type, int objectId, int objectTypeId, string objectName, int duration,
			int percentCompleted, int priorityId, int stateId, DateTime finishDate,
			string assignmentId, string assignmentName, string projectTitle,
			bool mergeBreaks, bool isNewMessage, bool isOverdue)
		{
			bool addNew = true;
			if (mergeBreaks && resultTable.Rows.Count > 0)
			{
				DataRow lastRow = resultTable.Rows[resultTable.Rows.Count - 1];
				if ((int)lastRow["UserId"] == userId
					&& (int)lastRow["ObjectId"] == objectId
					&& (int)lastRow["ObjectTypeId"] == objectTypeId
					&& lastRow["AssignmentId"].ToString() == assignmentId)
				{
					// we've found, that the last row in result table describes the same object
					addNew = false;

					// check that there are no events or other objects between two work pieces
					DateTime oldFinish = (DateTime)lastRow["Finish"];
					string expression = String.Format(CultureInfo.InvariantCulture, "Type<>{0}", (int)TimeType.Off);
					DataRow[] works = resultTable.Select(expression);
					foreach (DataRow workRow in works)
					{
						if ((DateTime)workRow["Start"] < start && (DateTime)workRow["Finish"] > oldFinish)
						{

							start = (DateTime)workRow["Finish"];

							addNew = true;
							break;
						}
					}
					//

					if (!addNew)
					{
						lastRow["Finish"] = finish;
					}
				}
			}

			if (addNew && start <= finish)
			{
				DataRow row = resultTable.NewRow();
				row["UserId"] = userId;
				row["Start"] = start;
				row["Finish"] = finish;
				row["Type"] = type;
				row["ObjectId"] = objectId;
				row["ObjectTypeId"] = objectTypeId;
				row["ObjectName"] = objectName;
				row["Duration"] = duration;
				if (objectTypeId == (int)ObjectTypes.Task || objectTypeId == (int)ObjectTypes.ToDo)
					row["PercentCompleted"] = percentCompleted;
				row["PriorityId"] = priorityId;
				row["StateId"] = stateId;
				row["FinishDate"] = finishDate;
				if (assignmentId != null)
					row["AssignmentId"] = assignmentId;
				if (assignmentName != null)
					row["AssignmentName"] = assignmentName;
				row["ProjectTitle"] = projectTitle;

				bool highlight = false;
				if (highlightedItems != null)
				{
					foreach (KeyValuePair<int, int> kvp in highlightedItems)
					{
						if (kvp.Key == objectId && kvp.Value == objectTypeId)
						{
							highlight = true;
							break;
						}
					}
				}
				row["Highlight"] = highlight;
				row["IsNewMessage"] = isNewMessage;
				row["IsOverdue"] = isOverdue;

				resultTable.Rows.Add(row);
			}
		}
		#endregion

		#region GetUserCalendarByUser
		/// <summary>
		/// Gets the user calendar info for current user.
		/// </summary>
		/// <returns>UserCalendarId, CalendarId, TimeZoneId</returns>
		public static IDataReader GetUserCalendar()
		{
			return GetUserCalendarByUser(Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Gets the user calendar info by user.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <returns>UserCalendarId, CalendarId, TimeZoneId</returns>
		public static IDataReader GetUserCalendarByUser(int userId)
		{
			return DBCalendar.GetUserCalendarByUser(userId);
		}
		#endregion

		#region GetListUserCalendarExceptions
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, UserCalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="CalendarId"></param>
		/// <returns></returns>
		public static IDataReader GetListUserCalendarExceptions(int userCalendarId)
		{
			return DBCalendar.GetListUserCalendarExceptions(userCalendarId);
		}

		public static IDataReader GetListUserCalendarExceptions()
		{
			int userCalendarId = -1;
			using (IDataReader reader = GetUserCalendar())
			{
				if (reader.Read())
					userCalendarId = (int)reader["UserCalendarId"];
			}
			return DBCalendar.GetListUserCalendarExceptions(userCalendarId);
		}
		#endregion

		#region GetListUserCalendarExceptionHours
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, FromTime, ToTime 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetListUserCalendarExceptionHours(int exceptionId)
		{
			return DBCalendar.GetListUserCalendarExceptionHours(exceptionId);
		}
		#endregion

		#region class CalendarHelper
		public class CalendarHelper
		{
			#region Fields
			private DateTime fromDate;
			private TimeZone calendarTimeZone;
			#endregion

			#region Properties
			private DateTime fromCalDate;
			public DateTime FromCalDate
			{
				set
				{
					fromCalDate = value;
				}
				get
				{
					return fromCalDate;
				}
			}

			private DateTime toCalDate;
			public DateTime ToCalDate
			{
				set
				{
					toCalDate = value;
				}
				get
				{
					return toCalDate;
				}
			}

			private int intervalDuration;
			public int IntervalDuration
			{
				set
				{
					intervalDuration = value;
				}
				get
				{
					return intervalDuration;
				}
			}

			private List<KeyValuePair<int, int>> highlightedItems;
			public List<KeyValuePair<int, int>> HighlightedItems
			{
				set
				{
					highlightedItems = value;
				}
				get
				{
					return highlightedItems;
				}
			}
			#endregion

			#region .Ctor
			public CalendarHelper(DateTime fromDate, TimeZone calendarTimeZone)
			{
				this.fromDate = fromDate;
				this.calendarTimeZone = calendarTimeZone;
			}
			#endregion

			#region Methods
			public DateTime GetUserDate(DateTime calendarDate)
			{
				DateTime utcDate = this.calendarTimeZone.ToUniversalTime(calendarDate);
				return Security.CurrentUser.CurrentTimeZone.ToLocalTime(utcDate);
			}

			public DateTime GetUserDate(int minutes)
			{
				DateTime utcDate = this.calendarTimeZone.ToUniversalTime(this.fromDate.AddMinutes(minutes));
				return Security.CurrentUser.CurrentTimeZone.ToLocalTime(utcDate);
			}

			public DateTime GetCalendarDate(DateTime userDate)
			{
				DateTime utcDate = Security.CurrentUser.CurrentTimeZone.ToUniversalTime(userDate);
				return this.calendarTimeZone.ToLocalTime(utcDate);
			}

			public DateTime GetCalendarDate(int minutes)
			{
				return this.fromDate.AddMinutes(minutes);
			}

			public DateTime GetCalendarDateFromUtc(DateTime utcDate)
			{
				return this.calendarTimeZone.ToLocalTime(utcDate);
			}
			#endregion
		}
		#endregion

		#region struct ExceptionInfo
		public struct ExceptionInfo
		{
			public int exceptionId;
			public DateTime fromDate, toDate;

			public ExceptionInfo(int exceptionId, DateTime fromDate, DateTime toDate)
			{
				this.exceptionId = exceptionId;
				this.fromDate = fromDate;
				this.toDate = toDate;
			}
		}
		#endregion

		#region AddUserCalendar
		public static void AddUserCalendar(int calendarId, int userId)
		{
			DBCalendar.AddUserCalendar(calendarId, userId);
		}
		#endregion

		#region UpdateUserCalendar
		public static void UpdateUserCalendar(
			int CalendarId)
		{
			DBCalendar.AddUserCalendar(CalendarId, Security.CurrentUser.UserID);
		}
		#endregion

		#region DeleteUserException
		public static void DeleteUserException(int exceptionId)
		{
			DBCalendar.DeleteUserException(exceptionId);
		}
		#endregion

		#region GetUserException
		/// <summary>
		/// Reader return fields:
		///  ExceptionId, UserCalendarId, FromDate, ToDate 
		/// </summary>
		/// <param name="ExceptionId"></param>
		/// <returns></returns>
		public static IDataReader GetUserException(int exceptionId)
		{
			return DBCalendar.GetUserException(exceptionId);
		}
		#endregion

		#region UpdateUserException
		public static void UpdateUserException(
			int ExceptionId,
			DateTime FromDate, DateTime ToDate,
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			To1 = (To1 == 0) ? 1440 : To1;
			To2 = (To2 == 0) ? 1440 : To2;
			To3 = (To3 == 0) ? 1440 : To3;
			To4 = (To4 == 0) ? 1440 : To4;
			To5 = (To5 == 0) ? 1440 : To5;

			int MaxMin = 24 * 60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4))
				throw new InvalidIntervalException();

			FromDate = FromDate.Date;
			ToDate = ToDate.Date.AddDays(1);

			using (DbTransaction tran = DbTransaction.Begin())
			{
				DBCalendar.UpdateUserException(ExceptionId, FromDate, ToDate);

				DBCalendar.DeleteUserExceptionHours(ExceptionId);

				if (From1 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From1, To1);

				if (From2 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From2, To2);

				if (From3 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From3, To3);

				if (From4 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From4, To4);

				if (From5 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From5, To5);

				int userCalendarId = DBCalendar.GetUserCalendarByException(ExceptionId);
				RecalculateUserCalendarIntervals(userCalendarId, ExceptionId, FromDate, ToDate);

				tran.Commit();
			}
		}
		#endregion

		#region RecalculateUserCalendarIntervals
		public static void RecalculateUserCalendarIntervals(int userCalendarId, int ExceptionId, DateTime FromDate, DateTime ToDate)
		{
			DataRow row;
			DataTable table = new DataTable();
			table.Columns.Add("OverlapedExceptionId", typeof(int));
			table.Columns.Add("OverlapedFromDate", typeof(DateTime));
			table.Columns.Add("OverlapedToDate", typeof(DateTime));

			using (IDataReader reader = DBCalendar.GetOverlapedUserExceptions(ExceptionId, FromDate, ToDate))
			{
				while (reader.Read())
				{
					row = table.NewRow();
					row["OverlapedExceptionId"] = (int)reader["ExceptionId"];
					row["OverlapedFromDate"] = (DateTime)reader["FromDate"];
					row["OverlapedToDate"] = (DateTime)reader["ToDate"];
					table.Rows.Add(row);
				}
			}

			foreach (DataRow dr in table.Rows)
			{
				int OverlapedExceptionId = (int)dr["OverlapedExceptionId"];
				DateTime OverlapedFromDate = (DateTime)dr["OverlapedFromDate"];
				DateTime OverlapedToDate = (DateTime)dr["OverlapedToDate"];

				if (FromDate <= OverlapedFromDate)
				{
					if (ToDate < OverlapedToDate)
					{
						// Cut the Overlaped Exception from left
						DBCalendar.UpdateUserException(OverlapedExceptionId, ToDate, OverlapedToDate);
					}
					else	// ToDate >= OverlapedToDate
					{
						// Delete the Overlaped Exception
						DBCalendar.DeleteUserException(OverlapedExceptionId);
					}
				}
				else	// FromDate > OverlapedFromDate
				{
					if (ToDate < OverlapedToDate)
					{
						// Double cut the Overlaped Exception
						DBCalendar.UpdateUserException(OverlapedExceptionId, OverlapedFromDate, FromDate);
						int NewOverlapedExceptionId = DBCalendar.AddUserException(userCalendarId, ToDate, OverlapedToDate);
						DBCalendar.CopyUserExceptionHours(OverlapedExceptionId, NewOverlapedExceptionId);
					}
					else	// ToDate >= OverlapedToDate
					{
						// Cut the Overlaped Exception from right
						DBCalendar.UpdateUserException(OverlapedExceptionId, OverlapedFromDate, FromDate);
					}
				}
			}
		}
		#endregion

		#region CreateUserException
		public static int CreateUserException(
			int userCalendarId,
			DateTime FromDate, DateTime ToDate,
			int From1, int To1,
			int From2, int To2,
			int From3, int To3,
			int From4, int To4,
			int From5, int To5)
		{
			To1 = (To1 == 0) ? 1440 : To1;
			To2 = (To2 == 0) ? 1440 : To2;
			To3 = (To3 == 0) ? 1440 : To3;
			To4 = (To4 == 0) ? 1440 : To4;
			To5 = (To5 == 0) ? 1440 : To5;

			int MaxMin = 24 * 60;
			if (From1 > To1 || From2 > To2 || From3 > To3 || From4 > To4 || From5 > To5
				|| To1 > MaxMin || To2 > MaxMin || To3 > MaxMin || To4 > MaxMin || To5 > MaxMin
				|| (From2 > 0 && From2 < To1) || (From3 > 0 && From3 < To2)
				|| (From4 > 0 && From4 < To3) || (From5 > 0 && From5 < To4))
				throw new InvalidIntervalException();

			FromDate = FromDate.Date;
			ToDate = ToDate.Date.AddDays(1);

			int ExceptionId = -1;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				ExceptionId = DBCalendar.AddUserException(userCalendarId, FromDate, ToDate);

				if (From1 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From1, To1);

				if (From2 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From2, To2);

				if (From3 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From3, To3);

				if (From4 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From4, To4);

				if (From5 >= 0)
					DBCalendar.AddUserExceptionHours(ExceptionId, From5, To5);

				RecalculateUserCalendarIntervals(userCalendarId, ExceptionId, FromDate, ToDate);

				tran.Commit();
			}

			return ExceptionId;
		}
		#endregion

		#region CheckUserCalendar
		public static bool CheckUserCalendar(int userId)
		{
			bool retval = false;
			using (IDataReader reader = DBCalendar.GetUserCalendarByUser(userId))
			{
				if (reader.Read())
					retval = true;
			}
			return retval;
		}
		#endregion

		#region GetStickedObjectsCount
		public static int GetStickedObjectsCount(int userId)
		{
			return DBCalendar.GetStickedObjectsCount(userId);
		}
		#endregion

		#region CheckStickedObject
		public static bool CheckStickedObject(int userId, int objectId, int objectTypeId, Guid? assignmentId)
		{
			return DBCalendar.CheckStickedObject(userId, objectId, objectTypeId, assignmentId);
		}
		#endregion

		#region AddStickedObject
		public static void AddStickedObject(int userId, int objectId, int objectTypeId, Guid? assignmentId, int position)
		{
			DBCalendar.AddStickedObject(userId, objectId, objectTypeId, assignmentId, position);
		}
		#endregion

		#region DeleteStickedObject
		public static void DeleteStickedObject(int userId, int objectId, int objectTypeId, Guid? assignmentId)
		{
			DBCalendar.DeleteStickedObject(objectTypeId, objectId, assignmentId, userId);
		}
		#endregion

		#region DeleteStickedObjectForAllUsers
		public static void DeleteStickedObjectForAllUsers(int objectId, int objectTypeId)
		{
			DBCalendar.DeleteStickedObjectForAllUsers(objectTypeId, objectId);
		}
		#endregion
		// =================================

		#region GetFinishDateByDuration
		public static DateTime GetFinishDateByDuration(int calendarId, DateTime startDate, int duration)
		{
			return DBCalendar.GetFinishDateByDuration(calendarId, startDate, duration);
		}
		#endregion
	}
}
