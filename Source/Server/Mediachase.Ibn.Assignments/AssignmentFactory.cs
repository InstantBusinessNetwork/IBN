using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using System.Web.UI;
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Assignments
{
	public static class AssignmentFactory
	{
		#region GetActivityPrimitive
		/// <summary>
		/// Gets the activity primitive.
		/// </summary>
		/// <param name="activity">The activity.</param>
		/// <param name="pageInstanse">The page instanse.</param>
		/// <returns></returns>
		public static Control GetActivityPrimitive(Activity activity, SchemaMaster shemaMaster, Page pageInstanse)
		{
			//ToDo: get additional info from activity
			ActivityMaster am = WorkflowActivityWrapper.GetActivityMaster(shemaMaster, activity, activity.Name);
			if (am != null)
				return pageInstanse.LoadControl(am.Description.UI.ViewControl);
			else
				return null;
			//if (activity is CompositeActivity && ((CompositeActivity)activity).Activities.Count > 0)
			//{
			//    return pageInstanse.LoadControl("~/Apps/BusinessProcess/Primitives/CompositeActivity.ascx");
			//}
			//else
			//{
			//    return pageInstanse.LoadControl("~/Apps/BusinessProcess/Primitives/SimpleActivity.ascx");
			//}
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
