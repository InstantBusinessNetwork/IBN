using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mediachase.Gantt
{
	public class DrawingContext
	{
		private DateTime _startDate;
		private float _left;
		private float _top;
		private float _width;
		private float _height;

		private float _freeHeight;
		private TimeSpan _interval;
		private float _currentTop;
		private Graphics _graphics;
		private DateTime _endDate;
		private IFormatProvider _provider;
		private float _headerItemHeight;
		private float _itemHeight;
		private int _itemsPerPage;
		private int _pageNumber;

		public DrawingContext(DateTime startDate, float left, float top, float width, float height, int itemsPerPage, int pageNumber)
		{
			_startDate = RecreateDateTime(startDate);
			_left = left;
			_top = top;
			_width = width;
			_height = _freeHeight = height;
			_itemsPerPage = itemsPerPage;
			_pageNumber = pageNumber;
		}

		public DateTime StartDate
		{
			get { return _startDate; }
		}

		public float Left
		{
			get { return _left; }
		}

		public float Top
		{
			get { return _top; }
		}

		public float Width
		{
			get { return _width; }
		}

		public float Height
		{
			get { return _height; }
		}

		public float FreeHeight
		{
			get { return _freeHeight; }
			set { _freeHeight = value; }
		}

		public TimeSpan Interval
		{
			get { return _interval; }
		}

		public float CurrentTop
		{
			get { return _currentTop; }
			set { _currentTop = value; }
		}

		public Graphics Graphics
		{
			get { return _graphics; }
			set { _graphics = value; }
		}

		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = RecreateDateTime(value);
				_interval = _endDate - _startDate;
			}
		}

		public IFormatProvider Provider
		{
			get { return _provider; }
			set { _provider = value; }
		}

		public float HeaderItemHeight
		{
			get { return _headerItemHeight; }
			set { _headerItemHeight = value; }
		}

		public float ItemHeight
		{
			get { return _itemHeight; }
			set { _itemHeight = value; }
		}

		public float DateToPixel(DateTime date)
		{
			return DateToPixel(date, _startDate, _interval, _width);
		}

		public static float DateToPixel(DateTime date, DateTime startDate, TimeSpan interval, float width)
		{
			date = RecreateDateTime(date);
			startDate = RecreateDateTime(startDate);

			TimeSpan dateInterval = date - startDate;
			return Convert.ToSingle(Math.Floor(width * dateInterval.TotalMinutes / interval.TotalMinutes));
		}

		public DateTime PixelToDate(float pixel)
		{
			return PixelToDate(pixel, _startDate, _interval, _width);
		}

		public static DateTime PixelToDate(float pixel, DateTime startDate, TimeSpan interval, float width)
		{
			return startDate.AddMinutes(pixel * interval.TotalMinutes / width);
		}

		public float TimeSpanToPixel(TimeSpan timeSpan)
		{
			return TimeSpanToPixel(timeSpan, _interval, _width);
		}

		public static float TimeSpanToPixel(TimeSpan timeSpan, TimeSpan interval, float width)
		{
			return Convert.ToSingle(Math.Ceiling(width * timeSpan.TotalMinutes / interval.TotalMinutes));
		}

		public int ItemsPerPage
		{
			get { return _itemsPerPage; }
		}

		public int PageNumber
		{
			get { return _pageNumber; }
		}

		private static DateTime RecreateDateTime(DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
		}
	}
}
