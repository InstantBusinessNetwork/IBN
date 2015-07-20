using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business
{
	public enum WeekItemStatus
	{
		/// <summary>
		/// No TT blocks
		/// </summary>
		NotCompleted = 0,

		/// <summary>
		/// At least one block rejected
		/// </summary>
		Rejected = 1,

		/// <summary>
		/// At least one block not sent to approve
		/// </summary>
		InProcess = 2,

		/// <summary>
		/// At least one not approved yet
		/// </summary>
		Submitted  = 3,

		/// <summary>
		/// All blocks approved
		/// </summary>
		Approved  = 4
	}

	/// <summary>
	/// Represents Time Tracking WeekItem information.
	/// </summary>
	public class WeekItemInfo
	{
		#region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WeekItemInfo"/> class.
		/// </summary>
		public WeekItemInfo()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WeekItemInfo"/> class.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="weekNumber">The week number.</param>
		/// <param name="status">The status.</param>
		public WeekItemInfo(DateTime startDate, int weekNumber, WeekItemStatus status)
		{
			this.StartDate = startDate;
			this.WeekNumber = weekNumber;
			this.Status = status;
		}

		#endregion

		#region Properties
		private DateTime _startDate;

		/// <summary>
		/// Gets or sets the start date.
		/// </summary>
		/// <value>The start date.</value>
		public DateTime StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}

		private int _weekNumber;

		/// <summary>
		/// Gets or sets the week number in year (1 - 52).
		/// </summary>
		/// <value>The week number.</value>
		public int WeekNumber
		{
			get { return _weekNumber; }
			set { _weekNumber = value; }
		}

		private WeekItemStatus _status;

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>The status.</value>
		public WeekItemStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		private double _day1;

		/// <summary>
		/// Gets or sets the day1.
		/// </summary>
		/// <value>The day1.</value>
		public double Day1
		{
			get { return _day1; }
			set { _day1 = value; }
		}

		private double _day2;

		/// <summary>
		/// Gets or sets the day2.
		/// </summary>
		/// <value>The day2.</value>
		public double Day2
		{
			get { return _day2; }
			set { _day2 = value; }
		}

		private double _day3;

		/// <summary>
		/// Gets or sets the day3.
		/// </summary>
		/// <value>The day3.</value>
		public double Day3
		{
			get { return _day3; }
			set { _day3 = value; }
		}

		private double _day4;

		/// <summary>
		/// Gets or sets the day4.
		/// </summary>
		/// <value>The day4.</value>
		public double Day4
		{
			get { return _day4; }
			set { _day4 = value; }
		}

		private double _day5;

		/// <summary>
		/// Gets or sets the day5.
		/// </summary>
		/// <value>The day5.</value>
		public double Day5
		{
			get { return _day5; }
			set { _day5 = value; }
		}

		private double _day6;

		/// <summary>
		/// Gets or sets the day6.
		/// </summary>
		/// <value>The day6.</value>
		public double Day6
		{
			get { return _day6; }
			set { _day6 = value; }
		}

		private double _day7;

		/// <summary>
		/// Gets or sets the day7.
		/// </summary>
		/// <value>The day7.</value>
		public double Day7
		{
			get { return _day7; }
			set { _day7 = value; }
		}

		/// <summary>
		/// Gets the day T.
		/// </summary>
		/// <value>The day T.</value>
		public double DayT
		{
			get { return this.Day1 + this.Day2 + this.Day3 + this.Day4 + this.Day5 + this.Day6 + this.Day7; }
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
