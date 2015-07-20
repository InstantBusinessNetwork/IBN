using System.Collections.Generic;
using System.Data;

using Mediachase.Ibn.Data.Sql;

namespace Update
{
	internal sealed class TimeZoneInfo
	{
		public int TimeZoneId { get; private set; }
		public int Bias { get; private set; }
		public int StandardBias { get; private set; }
		public int DaylightBias { get; private set; }
		public int DaylightMonth { get; private set; }
		public int DaylightDayOfWeek { get; private set; }
		public int DaylightWeek { get; private set; }
		public int DaylightHour { get; private set; }
		public int StandardMonth { get; private set; }
		public int StandardDayOfWeek { get; private set; }
		public int StandardWeek { get; private set; }
		public int StandardHour { get; private set; }

		private TimeZoneInfo()
		{
		}

		public static TimeZoneInfo[] GetZones(int languageId)
		{
			List<TimeZoneInfo> list = new List<TimeZoneInfo>();

			using (IDataReader reader = SqlHelper.ExecuteReader(SqlContext.Current, CommandType.StoredProcedure, "TimeZonesGet", SqlHelper.SqlParameter("@LanguageId", SqlDbType.Int, languageId)))
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
