using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents allowed child activities element.
	/// </summary>
	public class ActivityRestriction
	{
		#region Const
		#endregion

		#region Properties
		public string Name { get; set; }

		public List<string> ChildActivities { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="AllowedChildActivitiesElement"/> class.
		/// </summary>
		public ActivityRestriction()
		{
		}
		#endregion

		#region Methods
		#endregion
	}
}
