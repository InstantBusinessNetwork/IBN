using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Gantt
{
	public class TimeCell
	{
		int _index;
		DateTime _startDate;
		TimeSpan _interval;
		float _left;
		float _width;
		AxisElement _owner;

		public int Index { get { return _index; } }

		public DateTime StartDate { get { return _startDate; } }

		public TimeSpan Interval { get { return _interval; } }

		public float Left { get { return _left; } }

		public float Width { get { return _width; } }

		public AxisElement Owner { get { return _owner; } }

		internal TimeCell(int index, DateTime startDate, TimeSpan interval, float left, float width, AxisElement owner)
		{
			_index = index;
			_startDate = startDate;
			_interval = interval;
			_left = left;
			_width = width;
			_owner = owner;
		}

		public TimeSpan GetTimeInterval(float pixelInterval)
		{
			return new TimeSpan(Convert.ToInt64(_interval.Ticks * pixelInterval / _width));
		}
	}
}
