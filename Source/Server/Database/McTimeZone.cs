using System;
using System.Collections;
using System.Globalization;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for McTimeZone.
	/// </summary>
	public class McTimeZone
	{
		private static Hashtable	_hash	=	new Hashtable(255);

		private	int			_TimeZoneId;
		private	int			_Bias;
		private	int			_StandardBias;
		private	int			_DaylightBias;
		private	int			_DaylightMonth;
		private	int			_DaylightDayOfWeek;
		private	int			_DaylightWeek;
		private	int			_DaylightHour;
		private	int			_StandardMonth;
		private	int			_StandardDayOfWeek;
		private	int			_StandardWeek;
		private	int			_StandardHour;
		private Hashtable	_cachedDaylightChanges	=	new Hashtable(16);

		static	McTimeZone()
		{
			_hash.Add(0,new McTimeZone(0,0,0,0,0,0,0,0,0,0,0,0));
			_hash.Add(1,new McTimeZone(1,-270,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(2,new McTimeZone(2,540,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(3,new McTimeZone(3,-180,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(4,new McTimeZone(4,-240,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(5,new McTimeZone(5,-180,0,-60,4,0,1,3,10,0,1,4));
			_hash.Add(6,new McTimeZone(6,240,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(7,new McTimeZone(7,-570,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(8,new McTimeZone(8,-600,0,-60,10,0,5,2,3,0,5,3));
			_hash.Add(9,new McTimeZone(9,60,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(10,new McTimeZone(10,360,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(11,new McTimeZone(11,60,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(12,new McTimeZone(12,-240,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(13,new McTimeZone(13,-570,0,-60,10,0,5,2,3,0,5,3));
			_hash.Add(14,new McTimeZone(14,360,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(15,new McTimeZone(15,-360,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(16,new McTimeZone(16,-60,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(17,new McTimeZone(17,-60,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(18,new McTimeZone(18,-660,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(19,new McTimeZone(19,360,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(20,new McTimeZone(20,-480,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(21,new McTimeZone(21,720,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(22,new McTimeZone(22,-180,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(23,new McTimeZone(23,-600,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(24,new McTimeZone(24,-120,0,-60,3,0,5,0,10,0,5,1));
			_hash.Add(25,new McTimeZone(25,180,0,-60,10,0,3,2,2,0,2,2));
			_hash.Add(26,new McTimeZone(26,300,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(27,new McTimeZone(27,-120,0,-60,5,5,1,2,9,3,5,2));
			_hash.Add(28,new McTimeZone(28,-300,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(29,new McTimeZone(29,-720,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(30,new McTimeZone(30,-120,0,-60,3,0,5,3,10,0,5,4));
			_hash.Add(31,new McTimeZone(31,0,0,-60,3,0,5,1,10,0,5,2));
			_hash.Add(32,new McTimeZone(32,180,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(33,new McTimeZone(33,0,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(34,new McTimeZone(34,-120,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(35,new McTimeZone(35,600,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(36,new McTimeZone(36,-330,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(37,new McTimeZone(37,-210,0,-60,3,0,1,2,9,2,4,2));
			_hash.Add(38,new McTimeZone(38,-120,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(39,new McTimeZone(39,-540,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(40,new McTimeZone(40,360,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(41,new McTimeZone(41,420,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(42,new McTimeZone(42,120,0,-60,3,0,5,2,9,0,5,2));
			_hash.Add(43,new McTimeZone(43,420,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(44,new McTimeZone(44,-390,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(45,new McTimeZone(45,-360,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(46,new McTimeZone(46,-345,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(47,new McTimeZone(47,-720,0,-60,10,0,1,2,3,0,3,2));
			_hash.Add(48,new McTimeZone(48,210,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(49,new McTimeZone(49,-480,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(50,new McTimeZone(50,-420,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(51,new McTimeZone(51,240,0,-60,10,6,2,0,3,6,2,0));
			_hash.Add(52,new McTimeZone(52,480,0,-60,4,0,1,2,10,0,5,2));
			_hash.Add(53,new McTimeZone(53,-60,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(54,new McTimeZone(54,-180,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(55,new McTimeZone(55,180,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(56,new McTimeZone(56,300,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(57,new McTimeZone(57,240,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(58,new McTimeZone(58,660,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(59,new McTimeZone(59,-420,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(60,new McTimeZone(60,-480,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(61,new McTimeZone(61,-120,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(62,new McTimeZone(62,-360,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(63,new McTimeZone(63,-480,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(64,new McTimeZone(64,-600,0,-60,10,0,1,2,3,0,5,3));
			_hash.Add(65,new McTimeZone(65,-540,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(66,new McTimeZone(66,-780,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(67,new McTimeZone(67,300,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(68,new McTimeZone(68,420,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(69,new McTimeZone(69,-600,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(70,new McTimeZone(70,-480,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(71,new McTimeZone(71,-60,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(72,new McTimeZone(72,-60,0,-60,3,0,5,2,10,0,5,3));
			_hash.Add(73,new McTimeZone(73,-300,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(74,new McTimeZone(74,-600,0,-60,0,0,0,0,0,0,0,0));
			_hash.Add(75,new McTimeZone(75,-540,0,-60,3,0,5,2,10,0,5,3));
		}

		public static	McTimeZone	Load(int TimeZoneId)
		{
			return (McTimeZone)_hash[TimeZoneId];
		}

		protected McTimeZone()
		{
		}

		protected McTimeZone(int	TimeZoneId,
			int	Bias,
			int	StandardBias,
			int DaylightBias,
			int DaylightMonth,
			int DaylightDayOfWeek,
			int DaylightWeek,
			int DaylightHour,
			int StandardMonth,
			int StandardDayOfWeek,
			int StandardWeek,
			int StandardHour)
		{
			_TimeZoneId=TimeZoneId;
			_Bias=Bias;
			_StandardBias=StandardBias;
			_DaylightBias=DaylightBias;
			_DaylightMonth=DaylightMonth;
			_DaylightDayOfWeek=DaylightDayOfWeek;
			_DaylightWeek=DaylightWeek;
			_DaylightHour=DaylightHour;
			_StandardMonth=StandardMonth;
			_StandardDayOfWeek=StandardDayOfWeek;
			_StandardWeek=StandardWeek;
			_StandardHour=StandardHour;
		}

		public	int	TimeZoneId {get{return TimeZoneId;}}

		public	int	Bias{get{return _Bias;}}
		public	int	StandardBias{get{return _StandardBias;}}
		public	int DaylightBias{get{return _DaylightBias;}}
		public	int DaylightMonth{get{return _DaylightMonth;}}
		public	int DaylightDayOfWeek{get{return _DaylightDayOfWeek;}}
		public	int DaylightWeek{get{return _DaylightWeek;}}
		public	int DaylightHour{get{return _DaylightHour;}}
		public	int StandardMonth{get{return _StandardMonth;}}
		public	int StandardDayOfWeek{get{return _StandardDayOfWeek;}}
		public	int StandardWeek{get{return _StandardWeek;}}
		public	int StandardHour{get{return _StandardHour;}}

		public DateTime	GetLocalDate(DateTime	time)
		{
			return new DateTime(time.Ticks + this.GetUtcOffsetFromUniversalTime(time));
		}

		public long	GetUtcOffset(DateTime	time)
		{
			return GetUtcOffsetFromUniversalTime(time)/600000000;
		}

		private long	GetUtcOffsetFromUniversalTime(DateTime	time)
		{
			object Year	=	time.Year;

			if(!_cachedDaylightChanges.ContainsKey(Year))
			{
				lock(this)
				{
					if(!_cachedDaylightChanges.ContainsKey(Year))
					{
						if(this.DaylightMonth==0 ||this.StandardMonth==0)
						{
							_cachedDaylightChanges.Add(Year, new DaylightTime(DateTime.MinValue,DateTime.MaxValue, new TimeSpan(0,0,-(this.Bias),0,0)));
						}
						else
						if(this.DaylightMonth<this.StandardMonth)
						{
							DateTime startDaylight = new DateTime(time.Year,this.DaylightMonth,1,this.DaylightHour/*-(this.StandardBias+this.DaylightBias)/60*/,0,0,0);
							DateTime endDaylight = new DateTime(time.Year,this.StandardMonth,1,this.StandardHour,0,0,0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight,this.DaylightWeek,(DayOfWeek)this.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight,this.StandardWeek,(DayOfWeek)this.StandardDayOfWeek);

							startDaylight = startDaylight.AddMinutes((this.Bias+this.StandardBias));
							endDaylight = endDaylight.AddMinutes((this.Bias+this.DaylightBias));

							_cachedDaylightChanges.Add(Year, new DaylightTime(startDaylight,endDaylight, new TimeSpan(0,0,-(this.Bias+this.DaylightBias),0,0)));
						}
						else
						{
							DateTime startStandard = new DateTime(time.Year,this.StandardMonth,1,this.StandardHour,0,0,0);
							DateTime endStandard = new DateTime(time.Year,this.DaylightMonth,1,this.DaylightHour/*-(this.StandardBias+this.DaylightBias)/60*/,0,0,0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard,this.StandardWeek,(DayOfWeek)this.StandardDayOfWeek);
							endStandard = TransformDate(endStandard,this.DaylightWeek,(DayOfWeek)this.DaylightDayOfWeek);

							startStandard = startStandard.AddMinutes((this.Bias+this.DaylightBias));
							endStandard = endStandard.AddMinutes((this.Bias+this.StandardBias));

							_cachedDaylightChanges.Add(Year, new DaylightTime(endStandard,startStandard, new TimeSpan(0,0,-(this.Bias+this.DaylightBias),0,0)));
						}
					}
				}
			}

			DaylightTime	daylightTime	 = (DaylightTime)_cachedDaylightChanges[Year];

			if(daylightTime.Start<daylightTime.End)
			{
				if(time<daylightTime.Start || time>daylightTime.End)
				{
					// Standard Time
					return (long)-1*(long)this.Bias*(long)600000000;
				}
				else
				{
					// Daylight Time
					return daylightTime.Delta.Ticks;
				}
			}
			else
			{
				if(time<daylightTime.End || time>daylightTime.Start)
				{
					// Standard Time
					return daylightTime.Delta.Ticks;
				}
				else
				{
					// Daylight Time
					return (long)-1*(long)this.Bias*(long)600000000;
				}
			}
		}

		internal static DateTime	TransformDate(DateTime date, int Week, DayOfWeek DayOfWeek)
		{
			DateTime retVal = date;
			int tmpMonth = date.Month;
			while(Week>0&&date.Month==tmpMonth)
			{
				if(date.DayOfWeek==DayOfWeek)
				{
					retVal = date;
					Week--;
				}
				date = date.AddDays(1);
			}
			return retVal;
		}


	}
}
