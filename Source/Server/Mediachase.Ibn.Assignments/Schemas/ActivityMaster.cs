using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using System.Workflow.ComponentModel;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents supported activity element.
	/// </summary>
	public class ActivityMaster : Master
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public ActivityMasterDescription Description { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SupportedActivityElement"/> class.
		/// </summary>
		public ActivityMaster()
		{
			this.Description = new ActivityMasterDescription();
		}
		#endregion
	}
}
