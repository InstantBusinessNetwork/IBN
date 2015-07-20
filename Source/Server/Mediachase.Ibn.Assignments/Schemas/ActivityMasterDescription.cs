using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents activity master description.
	/// </summary>
	public class ActivityMasterDescription
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
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public string Icon { get; set; }

		/// <summary>
		/// Gets or sets the UI.
		/// </summary>
		/// <value>The UI.</value>
		public WebUIElement UI { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityMasterDescription"/> class.
		/// </summary>
		public ActivityMasterDescription()
		{
			this.UI = new WebUIElement();
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
