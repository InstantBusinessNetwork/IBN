using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents supported activity element collection.
	/// </summary>
	public class ActivityMasterCollection: List<ActivityMaster>
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SupportedActivityMasterCollection"/> class.
		/// </summary>
		public ActivityMasterCollection()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the <see cref="Mediachase.Ibn.Assignments.Schemas.ActivityMaster"/> with the specified activity name.
		/// </summary>
		/// <value></value>
		public ActivityMaster this[string activityName]
		{
			get
			{
				foreach (ActivityMaster item in this)
				{
					if (item.Description.Name == activityName)
						return item;
				}

				return null;
			}
		}
		#endregion

		
	}
}
