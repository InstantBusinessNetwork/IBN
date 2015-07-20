using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Gantt
{
	/// <summary>
	/// Specifies ISO 8601 week number
	/// </summary>
	public static class Iso8601WeekNumber
	{
		// OZ:
		//		  ¬ стандарте  ISO, год,  относ€щийс€ к  номеру недели  ISO, может
		//        отличатьс€  от  календарного  года.   Ќапример,  1  €нвар€  1988
		//        попадает  на  53-ю  неделю  ISO  дл€  1987  года.  Ќедел€ всегда
		//        начинаетс€ с понедельника и заканчиваетс€ воскресеньем.
		//
		//            *  ≈сли 1 €нвар€ падает на п€тницу, субботу или воскресенье,
		//               то  недел€,  включающа€  1  €нвар€,  считаетс€  последней
		//               неделей  предыдущего  года,  потому  что большинство дней
		//               этой недели принадлежат предыдущему году.
		//
		//
		//            *  ≈сли 1 €нвар€ падает  на понедельник, вторник, среду  или
		//               четверг, то  эта недел€  считаетс€ первой  неделей нового
		//               года, потому что большинство дней этой недели принадлежат
		//               новому году.
		//
		//        Ќапример,  1  €нвар€  1991  падает  на вторник, поэтому недел€ с
		//        понедельника,  31  декабр€  1990  по  воскресенье, 6 €нвар€ 1991
		//        считаетс€  неделей   1.  
		//
		// More Info:
		// http://www.boyet.com/Articles/PublishedArticles/CalculatingtheISOweeknumb.html

		/// <summary>
		/// Gets the first week.
		/// </summary>
		/// <param name="Year">The year.</param>
		/// <returns></returns>
		public static DateTime GetFirstWeekOfYear(int year)
		{
			// get the date for the 4-Jan for this year
			DateTime date = new DateTime(year, 1, 4);

			// get the ISO day number for this date 1==Monday, 7==Sunday
			int dayNumber = (int)date.DayOfWeek; // 0==Sunday, 6==Saturday
			if (dayNumber == 0)
			{
				dayNumber = 7;
			}

			// return the date of the Monday that is less than or equal
			// to this date
			return date.AddDays(1 - dayNumber);
		}

		/// <summary>
		/// Gets the week number.
		/// </summary>
		/// <param name="dt">The dt.</param>
		/// <returns></returns>
		public static int GetWeekNumber(DateTime date)
		{
			DateTime dateOfFirstWeekOfYear;
			int isoYear = date.Year;
			if (date >= new DateTime(isoYear, 12, 29))
			{
				dateOfFirstWeekOfYear = GetFirstWeekOfYear(isoYear + 1);
				if (date < dateOfFirstWeekOfYear)
				{
					dateOfFirstWeekOfYear = GetFirstWeekOfYear(isoYear);
				}
				else
				{
					isoYear++;
				}
			}
			else
			{
				dateOfFirstWeekOfYear = GetFirstWeekOfYear(isoYear);
				if (date < dateOfFirstWeekOfYear)
				{
					dateOfFirstWeekOfYear = GetFirstWeekOfYear(--isoYear);
				}
			}

			return ((date - dateOfFirstWeekOfYear).Days / 7 + 1);
		}

		/// <summary>
		/// Gets the year week number [Year] * 100 + [WeekNumber].
		/// </summary>
		/// <param name="dt">The dt.</param>
		/// <returns></returns>
		public static int GetYearWeekNumber(DateTime date)
		{
			DateTime dateOfFirstWeekOfYear;
			int isoYear = date.Year;

			if (date >= new DateTime(isoYear, 12, 29))
			{
				dateOfFirstWeekOfYear = GetFirstWeekOfYear(isoYear + 1);
				if (date < dateOfFirstWeekOfYear)
				{
					dateOfFirstWeekOfYear = GetFirstWeekOfYear(isoYear);
				}
				else
				{
					isoYear++;
				}
			}
			else
			{
				dateOfFirstWeekOfYear = GetFirstWeekOfYear(isoYear);
				if (date < dateOfFirstWeekOfYear)
				{
					dateOfFirstWeekOfYear = GetFirstWeekOfYear(--isoYear);
				}
			}

			return (isoYear * 100) + ((date - dateOfFirstWeekOfYear).Days / 7 + 1);
		}
	}
}
