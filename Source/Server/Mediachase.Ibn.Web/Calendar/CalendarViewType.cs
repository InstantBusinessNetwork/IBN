//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
using System;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Specifies the view that control is rendered in.
	/// </summary>
	public enum CalendarViewType
	{
		/// <summary>
		/// Indicates year view
		/// </summary>
		YearView, 
		
		/// <summary>
		/// Indicates month view
		/// </summary>
		MonthView, 

		/// <summary>
		/// Indicates week view, vertical format
		/// </summary>
		WeekView, 
		
		/// <summary>
		/// Indicates work week view, vertical format
		/// </summary>
		WorkWeekView, 

		/// <summary>
		/// Indicates week view, two columns format
		/// </summary>
		WeekView2, 

		/// <summary>
		/// Indicates day view
		/// </summary>
		DayView,

		/// <summary>
		/// Indicates Task view
		/// </summary>
		TaskView
	}

}
