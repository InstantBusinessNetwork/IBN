using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class SharedCalendarLinkHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			using (IDataReader rdr = Mediachase.IBN.Business.CalendarView.GetListPeopleForCalendar())
			{
				if (rdr.Read())
				{
					retval = true;
				}
			}
			
			return retval;
		}

		#endregion
	}
}
