using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents activity UI element.
	/// </summary>
	public class WebUIElement
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the create control.
		/// </summary>
		/// <value>The create control.</value>
		public string CreateControl{ get; set; }

		/// <summary>
		/// Gets or sets the edit control.
		/// </summary>
		/// <value>The edit control.</value>
		public string EditControl{ get; set; }

		/// <summary>
		/// Gets or sets the view control.
		/// </summary>
		/// <value>The view control.</value>
		public string ViewControl { get; set; }

		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityUIElement"/> class.
		/// </summary>
		public WebUIElement()
		{
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
