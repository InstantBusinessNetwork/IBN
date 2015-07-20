using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Workflow.ComponentModel;
using System.Workflow.Activities;

namespace Mediachase.Ibn.AssignmentsUI.Code
{
	public static class ActivityFactory
	{
		#region GetActivityPrimitive
		/// <summary>
		/// Gets the activity primitive.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <param name="pageInstanse">The page instanse.</param>
		/// <returns></returns>
		public static Control GetActivityPrimitive(Activity activity, Page pageInstanse)
		{
			//ToDo: get additional info from activity
			if (activity is CompositeActivity && ((CompositeActivity)activity).Activities.Count > 0)
			{
				return pageInstanse.LoadControl("~/Modules/Primitives/CompositeActivity.ascx");
			}
			else
			{
				return pageInstanse.LoadControl("~/Modules/Primitives/SimpleActivity.ascx");
			}
		}
		#endregion

		#region GetAvailableActivities
		/// <summary>
		/// Gets the available activities.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetAvailableActivities()
		{
			List<string> retVal = new List<string>();

			retVal.Add("SimpleActivity");
			retVal.Add("CompositeActivity");

			return retVal;
		} 
		#endregion
	}
}
