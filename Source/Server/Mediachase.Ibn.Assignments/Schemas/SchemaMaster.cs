using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using System.Workflow.ComponentModel;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents worfklow schema.
	/// </summary>
	public class SchemaMaster : Master
	{
		#region Properties
		public SchemaMasterDescription Description { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SchemaMaster"/> class.
		/// </summary>
		public SchemaMaster()
		{
			this.Description = new SchemaMasterDescription();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <param name="activityMasterName">Name of the activity master.</param>
		/// <returns></returns>
		public ActivityMaster GetActivityMaster(string activityMasterName)
		{
			return this.Description.Activities[activityMasterName];
		}

		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <param name="activityType">Type of the activity.</param>
		/// <returns></returns>
		//public ActivityMaster GetActivityMaster(Activity activityType)
		//{
		//    if(activityType==null)
		//        throw new ArgumentNullException("activityType");

		//    return GetActivityMaster(activityType.GetType());
		//}

		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <param name="activityMasterName">Name of the activity master.</param>
		/// <returns></returns>
		public ActivityMaster GetActivityMaster(Type activityType)
		{
			string typeString = AssemblyUtil.GetTypeString(activityType);

			foreach (ActivityMaster item in this.Description.Activities)
			{
				if (item.TypeName == typeString)
					return item;
			}

			return null;
		}

		/// <summary>
		/// Gets the allowed activities.
		/// </summary>
		/// <param name="parentActivity">The parent activity.</param>
		/// <returns></returns>
		public ActivityMaster[] GetAllowedActivities(string parentActivityName)
		{
			foreach (ActivityRestriction item in this.Description.ActivityRestrictions)
			{
				if (item.Name == parentActivityName)
				{
					List<ActivityMaster> retVal = new List<ActivityMaster>();

					foreach (string activityName in item.ChildActivities)
					{
						ActivityMaster activity = GetActivityMaster(activityName);

						if (activity != null)
							retVal.Add(activity);
					}

					return retVal.ToArray();
				}
			}

			// Return All Activites
			return this.Description.Activities.ToArray();

		}
		#endregion

		
	}
}
