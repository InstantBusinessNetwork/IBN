using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Workflow.ComponentModel;

namespace Mediachase.Ibn.AssignmentsUI.Modules.Primitives
{
	public partial class CompositeActivity : System.Web.UI.UserControl, IWorkflowPrimitive
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

		#region prop: HasChildNodes
		/// <summary>
		/// Gets a value indicating whether this instance has child nodes.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has child nodes; otherwise, <c>false</c>.
		/// </value>
		public bool HasChildNodes
		{
			get
			{
				if (this.DataItem != null && this.DataItem is System.Workflow.ComponentModel.CompositeActivity)
					return ((System.Workflow.ComponentModel.CompositeActivity)this.DataItem).Activities.Count > 0;

				return false;
			}
		}
		#endregion

		

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IWorkflowPrimitive Members

		public Activity DataItem
		{
			get
			{
				if (ViewState["_DataItem"] != null)
					return (Activity)ViewState["_DataItem"];

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