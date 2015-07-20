using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Assignments.UI;

namespace Mediachase.Ibn.AssignmentsUI.Modules.Primitives
{
	public partial class SimpleActivity : System.Web.UI.UserControl, IWorkflowPrimitive
	{
		#region prop: Description
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get
			{
				if (this.DataItem != null)
					return this.DataItem.Description;

				return string.Empty;
			}
		}
		#endregion

		#region prop: Name
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Name
		{
			get
			{
				if (this.DataItem != null)
					return this.DataItem.Name;

				return string.Empty;
			}
		}
		#endregion


		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IWorkflowPrimitive Members

		public IbnActivity DataItem
		{
			get
			{
				if (ViewState["_DataItem"] != null)
					return (IbnActivity)ViewState["_DataItem"];

				return null;
			}
			set
			{
				ViewState["_DataItem"] = value;
			}
		}

		#endregion
	}
}