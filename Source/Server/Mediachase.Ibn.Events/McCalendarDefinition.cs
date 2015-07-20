using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Events
{
	public enum eRecurrenceState
	{
		Exception = 1,
		Master = 2,
		NotRecurring = 3,
		DeletedException = 4
	}

	public enum eResourceStatus
	{
		Accepted = 1, 
		Declined = 2,
		NotResponded = 3,
		Organized = 4,
		Tentative = 5
	}

	[Flags]
	public enum eBitDayOfWeek
	{
		Unknown		= 0x00,
		Monday		= 0x01,
		Tuesday		= 0x02,
		Wednesday	= 0x04,
		Thursday	= 0x08,
		Friday		= 0x10,
		Saturday	= 0x20,
		Sunday		= 0x40,
		Alldays		= Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,
		Weekdays	= Monday | Tuesday | Wednesday | Thursday | Friday
	}

	public enum eRecurrenceType
	{
		RecursDaily		= 1,
		RecursWeekly	= 2,
		RecursMonthly	= 3,
		RecursMonthNth	= 4,
		RecursYearly	= 5,
		RecursYearNth	= 6
	}

	public enum eInstanceType
	{
		InstanceFirst	= 1,
		InstanceSecond	= 2,
		InstanceThird	= 3,
		InstanceFour	= 4,
		InstanceLast	= 5
	}
	public enum eSesitivity
	{
		Confidential	= 1,
		Normal			= 2,
		Personal		= 3,
		Private			= 4
	}
	public enum eImportance
	{
		Low				= 0,
		Normal			= 1,
		High			= 2
	}

}
