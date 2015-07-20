using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Database;

namespace Mediachase.Ibn.Business.Configuration
{
	internal class TimeZoneInfo
	{
		public int TimeZoneId;
		public int Bias;
		public int StandardBias;
		public int DaylightBias;
		public int DaylightMonth;
		public int DaylightDayOfWeek;
		public int DaylightWeek;
		public int DaylightHour;
		public int StandardMonth;
		public int StandardDayOfWeek;
		public int StandardWeek;
		public int StandardHour;

		private TimeZoneInfo()
		{
		}

		public static TimeZoneInfo[] GetZones(DBHelper dbh, int langId)
		{
			List<TimeZoneInfo> list = new List<TimeZoneInfo>();

			using (IDataReader reader = dbh.RunSPDataReader("TimeZonesGet", DBHelper.MP("@LanguageId", SqlDbType.Int, langId)))
			{
				while (reader.Read())
				{
					TimeZoneInfo item = new TimeZoneInfo();

					item.TimeZoneId = (int)reader["TimeZoneId"];
					item.Bias = (int)reader["Bias"];
					item.StandardBias = (int)reader["StandardBias"];
					item.DaylightBias = (int)reader["DaylightBias"];
					item.DaylightMonth = (int)reader["DaylightMonth"];
					item.DaylightDayOfWeek = (int)reader["DaylightDayOfWeek"];
					item.DaylightWeek = (int)reader["DaylightWeek"];
					item.DaylightHour = (int)reader["DaylightHour"];
					item.StandardMonth = (int)reader["StandardMonth"];
					item.StandardDayOfWeek = (int)reader["StandardDayOfWeek"];
					item.StandardWeek = (int)reader["StandardWeek"];
					item.StandardHour = (int)reader["StandardHour"];

					list.Add(item);
				}
			}

			return list.ToArray();
		}
	}
}
