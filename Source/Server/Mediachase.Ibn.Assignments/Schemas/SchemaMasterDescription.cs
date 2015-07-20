using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents schema master description.
	/// </summary>
	public class SchemaMasterDescription
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets the creator.
		/// </summary>
		/// <value>The creator.</value>
		public string Creator { get; set; }

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public string Icon { get; set; }

		/// <summary>
		/// Gets or sets the activities.
		/// </summary>
		/// <value>The activities.</value>
		public ActivityMasterCollection Activities { get; set; }

		/// <summary>
		/// Gets or sets the activity restrictions.
		/// </summary>
		/// <value>The activity restrictions.</value>
		public ActivityRestrictionCollection ActivityRestrictions { get; set; }

		/// <summary>
		/// Gets or sets the UI.
		/// </summary>
		/// <value>The UI.</value>
		public WebUIElement UI { get; set; }

		/// <summary>
		/// Gets or sets the supported ibn object types.
		/// </summary>
		/// <value>The supported ibn object types.</value>
		public List<int> SupportedIbnObjectTypes { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SchemaMasterMetaInformation"/> class.
		/// </summary>
		public SchemaMasterDescription()
		{
			this.Activities = new ActivityMasterCollection();
			this.ActivityRestrictions = new ActivityRestrictionCollection();
			this.UI = new WebUIElement();
			this.SupportedIbnObjectTypes = new List<int>();
		}
		#endregion

		#region Methods
		#endregion
		
	}
}
