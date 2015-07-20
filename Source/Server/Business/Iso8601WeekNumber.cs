using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business
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
		public static DateTime GetFirstWeekOfYear(int Year)
		{
			// get the date for the 4-Jan for this year
			DateTime dt = new DateTime(Year, 1, 4);

			// get the ISO day number for this date 1==Monday, 7==Sunday
			int dayNumber = (int)dt.DayOfWeek; // 0==Sunday, 6==Saturday
			if (dayNumber == 0)
			{
				dayNumber = 7;
			}

			// return the date of the Monday that is less than or equal
			// to this date
			return dt.AddDays(1 - dayNumber);
		}

		/// <summary>
		/// Gets the week number.
		/// </summary>
		/// <param name="dt">The dt.</param>
		/// <returns></returns>
		public static int GetWeekNumber(DateTime dt)
		{
			DateTime week1;
			int IsoYear = dt.Year;
			if (dt >= new DateTime(IsoYear, 12, 29))
			{
				week1 = GetFirstWeekOfYear(IsoYear + 1);
				if (dt < week1)
				{
					week1 = GetFirstWeekOfYear(IsoYear);
				}
				else
				{
					IsoYear++;
				}
			}
			else
			{
				week1 = GetFirstWeekOfYear(IsoYear);
				if (dt < week1)
				{
					week1 = GetFirstWeekOfYear(--IsoYear);
				}
			}

			return ((dt - week1).Days / 7 + 1);
		}

		/// <summary>
		/// Gets the year week number [Year] * 100 + [WeekNumber].
		/// </summary>
		/// <param name="dt">The dt.</param>
		/// <returns></returns>
		public static int GetYearWeekNumber(DateTime dt)
		{
			DateTime week1;
			int IsoYear = dt.Year;

			if (dt >= new DateTime(IsoYear, 12, 29))
			{
				week1 = GetFirstWeekOfYear(IsoYear + 1);
				if (dt < week1)
				{
					week1 = GetFirstWeekOfYear(IsoYear);
				}
				else
				{
					IsoYear++;
				}
			}
			else
			{
				week1 = GetFirstWeekOfYear(IsoYear);
				if (dt < week1)
				{
					week1 = GetFirstWeekOfYear(--IsoYear);
				}
			}

			return (IsoYear * 100) + ((dt - week1).Days / 7 + 1);
		}
	}
}
