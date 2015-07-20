using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments.Workflow
{
	/// <summary>
	/// Represents activity element.
	/// </summary>
	[Serializable]
	public abstract class ActivityElement
	{
		#region Const	
		#endregion 

		#region Properties
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		[XmlIgnore]
		public ActivityElement Parent { get; set; }

		/// <summary>
		/// Gets or sets the activities.
		/// </summary>
		/// <value>The activities.</value>
		public ActivityElementCollection Activities { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityElement"/> class.
		/// </summary>
		public ActivityElement()
		{
			if (this.CanHaveChildren)
			{
				this.Activities = new ActivityElementCollection(this);
			}
		}
		#endregion
	
		#region Methods
		/// <summary>
		/// Gets a value indicating whether this instance can have children.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can have children; otherwise, <c>false</c>.
		/// </value>
		public abstract bool CanHaveChildren { get; }
		#endregion 
	} 
}
