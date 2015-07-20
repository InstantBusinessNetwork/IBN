using System;

namespace Mediachase.Web.UI.WebControls
{
	/// <summary>
	/// Summary description for CalendarDayOfWeek.
	/// </summary>
	[Flags]
	public enum CalendarDayOfWeek
	{
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesday = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64
	}
}
