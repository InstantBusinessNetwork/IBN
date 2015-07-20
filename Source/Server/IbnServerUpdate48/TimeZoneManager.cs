using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Sql;
using System.Data;
using System.Data.SqlClient;

namespace Update
{
	internal sealed class TimeZoneManager
	{
		private TimeZoneManager()
		{
		}

		#region public static void FillTimeConversionTables()
		public static void FillTimeConversionTables()
		{
			int lastYear = 0;
			using (IDataReader reader = SqlHelper.ExecuteReader(SqlContext.Current, CommandType.Text, "SELECT MAX([Start]) FROM [TIMEZONES_UTC_LOCAL]"))
			{
				if (reader.Read())
				{
					object o = reader[0];
					if (o != null && o != DBNull.Value)
						lastYear = ((DateTime)o).Year;
				}
			}

			if (lastYear == 2009)
			{
				LogFile.WriteLine("Filling time conversion tables...");
				try
				{
					FillTimeZonesLocalUtcTable(2010, 2020, 1);
					FillTimeZonesUtcLocalTable(2010, 2020, 1);

					Program.LogWriteOk();
				}
				catch
				{
					Program.LogWriteFailed();
					throw;
				}
			}
		}
		#endregion

		#region private static void FillTimeZonesLocalUtcTable(int firstYear, int lastYear, int languageId)
		private static void FillTimeZonesLocalUtcTable(int firstYear, int lastYear, int languageId)
		{
			TimeZoneInfo[] zones = TimeZoneInfo.GetZones(languageId);

			foreach (TimeZoneInfo zone in zones)
			{
				if (zone.DaylightMonth == 0 || zone.StandardMonth == 0)
				{
					// Add Standard interval [6/18/2004]
					AddTimeToLocalUtc(zone.TimeZoneId, new DateTime(firstYear, 1, 1, 0, 0, 0, 0), new DateTime(lastYear, 1, 1, 0, 0, 0, 0), zone.Bias, false);
				}
				else
				{
					DateTime lastEnd = new DateTime(firstYear, 1, 1, 0, 0, 0, 0);

					for (int year = firstYear; year < lastYear; year++)
					{
						if (zone.DaylightMonth < zone.StandardMonth)
						{
							DateTime startDaylight = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour - (zone.StandardBias + zone.DaylightBias) / 60, 0, 0, 0);
							DateTime endDaylight = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);

							// Add Standard interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, lastEnd, startDaylight, (zone.Bias + zone.StandardBias), false);

							// Add Daylight interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, startDaylight, endDaylight, (zone.Bias + zone.DaylightBias), true);

							lastEnd = endDaylight;
						}
						else
						{
							DateTime startStandard = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);
							DateTime endStandard = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour - (zone.StandardBias + zone.DaylightBias) / 60, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);
							endStandard = TransformDate(endStandard, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);

							// Add Standard interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, lastEnd, startStandard, (zone.Bias + zone.DaylightBias), true);

							// Add Daylight interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, startStandard, endStandard, (zone.Bias + zone.StandardBias), false);

							lastEnd = endStandard;
						}
					}
					// Add Final Interval [6/18/2004]
					// Add Standard interval [6/18/2004]
					AddTimeToLocalUtc(zone.TimeZoneId, lastEnd, new DateTime(lastYear, 1, 1, 0, 0, 0, 0), zone.Bias + (zone.DaylightMonth < zone.StandardMonth ? zone.StandardBias : zone.DaylightBias), false);
				}
			}
		}
		#endregion
		#region private static void FillTimeZonesUtcLocalTable(int firstYear, int lastYear, int languageId)
		private static void FillTimeZonesUtcLocalTable(int firstYear, int lastYear, int languageId)
		{
			TimeZoneInfo[] zones = TimeZoneInfo.GetZones(languageId);

			foreach (TimeZoneInfo zone in zones)
			{
				if (zone.DaylightMonth == 0 || zone.StandardMonth == 0)
				{
					// Add Standard interval [6/18/2004]
					AddTimeToUtcLocal(zone.TimeZoneId, new DateTime(firstYear, 1, 1, 0, 0, 0, 0), new DateTime(lastYear, 1, 1, 0, 0, 0, 0), -zone.Bias, false);
				}
				else
				{
					DateTime lastEnd = new DateTime(firstYear, 1, 1, 0, 0, 0, 0);

					for (int year = firstYear; year < lastYear; year++)
					{
						if (zone.DaylightMonth < zone.StandardMonth)
						{
							DateTime startDaylight = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour/*-(tmz.StandardBias+tmz.DaylightBias)/60*/, 0, 0, 0);
							DateTime endDaylight = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);

							startDaylight = startDaylight.AddMinutes((zone.Bias + zone.StandardBias));
							endDaylight = endDaylight.AddMinutes((zone.Bias + zone.StandardBias));

							// Add Standard interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, startDaylight, -(zone.Bias + zone.StandardBias), false);

							// Add Daylight interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, startDaylight, endDaylight, -(zone.Bias + zone.DaylightBias), true);

							lastEnd = endDaylight;
						}
						else
						{
							DateTime startStandard = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);
							DateTime endStandard = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour/*-(tmz.StandardBias+tmz.DaylightBias)/60*/, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);
							endStandard = TransformDate(endStandard, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);

							startStandard = startStandard.AddMinutes((zone.Bias + zone.StandardBias));
							endStandard = endStandard.AddMinutes((zone.Bias + zone.StandardBias));

							// Add Standard interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, startStandard, -(zone.Bias + zone.DaylightBias), true);

							// Add Daylight interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, startStandard, endStandard, -(zone.Bias + zone.StandardBias), false);

							lastEnd = endStandard;
						}
					}
					// Add Final Interval [6/18/2004]
					// Add Standard interval [6/18/2004]
					if (zone.DaylightMonth < zone.StandardMonth)
						AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, new DateTime(lastYear, 1, 1, 0, 0, 0, 0), -(zone.Bias + zone.StandardBias), false);
					else
						AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, new DateTime(lastYear, 1, 1, 0, 0, 0, 0), -(zone.Bias + zone.DaylightBias), false);
				}
			}
		}
		#endregion

		#region private static DateTime TransformDate(DateTime date, int week, DayOfWeek dayOfWeek)
		private static DateTime TransformDate(DateTime date, int week, DayOfWeek dayOfWeek)
		{
			DateTime retVal = date;
			int tmpMonth = date.Month;
			while (week > 0 && date.Month == tmpMonth)
			{
				if (date.DayOfWeek == dayOfWeek)
				{
					retVal = date;
					week--;
				}
				date = date.AddDays(1);
			}
			return retVal;
		}
		#endregion

		#region private static int AddTimeToLocalUtc(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		private static int AddTimeToLocalUtc(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		{
			return RunSPInteger("TimeZoneAddLocalUtc",
				MP("@TimeZoneId", SqlDbType.Int, zoneId),
				MP("@Start", SqlDbType.DateTime, start),
				MP("@End", SqlDbType.DateTime, end),
				MP("@TimeOffset", SqlDbType.Int, offset),
				MP("@IsDayLight", SqlDbType.Bit, isDayLight)
				);
		}
		#endregion
		#region private static int AddTimeToUtcLocal(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		private static int AddTimeToUtcLocal(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		{
			return RunSPInteger("TimeZoneAddUtcLocal",
				MP("@TimeZoneId", SqlDbType.Int, zoneId),
				MP("@Start", SqlDbType.DateTime, start),
				MP("@End", SqlDbType.DateTime, end),
				MP("@TimeOffset", SqlDbType.Int, offset),
				MP("@IsDayLight", SqlDbType.Bit, isDayLight)
				);
		}
		#endregion

		#region private static int RunSPInteger(string commandText, params SqlParameter[] parameters)
		private static int RunSPInteger(string commandText, params SqlParameter[] parameters)
		{
			int result = -1;

			object o = RunSPRetval(commandText, SqlDbType.Int, parameters);
			if (o != null)
				result = (int)o;

			return result;
		}
		#endregion
		#region private static object RunSPRetval(string commandText, SqlDbType resultType, params SqlParameter[] parameters)
		private static object RunSPRetval(string commandText, SqlDbType resultType, params SqlParameter[] parameters)
		{
			SqlParameter resultParameter = new SqlParameter("@retval", resultType);
			resultParameter.Direction = ParameterDirection.Output;

			SqlParameter[] newParameters = new SqlParameter[parameters.Length + 1];
			Array.Copy(parameters, newParameters, parameters.Length);
			newParameters[parameters.Length] = resultParameter;

			SqlHelper.ExecuteNonQuery(SqlContext.Current, CommandType.StoredProcedure, commandText, newParameters);

			object result = null;

			if (resultParameter.Value != DBNull.Value)
				result = resultParameter.Value;

			return result;
		}
		#endregion
		#region private static SqlParameter MP(string name, SqlDbType type, object value)
		private static SqlParameter MP(string name, SqlDbType type, object value)
		{
			if (type == SqlDbType.DateTime && value != DBNull.Value && (value == null || (DateTime)value == DateTime.MinValue))
				value = DBNull.Value;

			SqlParameter parameter = new SqlParameter(name, type);
			parameter.Direction = ParameterDirection.Input;
			parameter.Value = value;
			return parameter;
		}
		#endregion
	}
}
