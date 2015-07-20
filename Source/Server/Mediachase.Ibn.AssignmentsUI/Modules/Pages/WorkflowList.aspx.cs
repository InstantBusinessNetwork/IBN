using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.AssignmentsUI.Modules.Pages
{
	public partial class WorkflowList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindGrid();

			lblCreate.Click += new EventHandler(lblCreate_Click);
			ctrlList.RowCommand += new GridViewCommandEventHandler(ctrlList_RowCommand);
			ctrlList.RowDeleting += new GridViewDeleteEventHandler(ctrlList_RowDeleting);
		}

		void ctrlList_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
		}

		#region ctrlList_RowCommand
		/// <summary>
		/// Handles the RowCommand event of the ctrlList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
		void ctrlList_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				Response.Redirect(String.Format("~/TestWF.aspx?WfId={0}", e.CommandArgument));
			}

			if (e.CommandName == "Delete")
			{
				BusinessManager.Delete(WorkflowDefinitionEntity.ClassName, PrimaryKeyId.Parse(e.CommandArgument.ToString()));
			}

			BindGrid();
		}
		#endregion

		#region lblCreate_Click
		/// <summary>
		/// Handles the Click event of the lblCreate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblCreate_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Modules/Pages/WorkflowCreate.aspx");
		} 
		#endregion

		#region BindGrid
		/// <summary>
		/// Binds the grid.
		/// </summary>
		private void BindGrid()
		{
			EntityObject[] obj = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new Mediachase.Ibn.Data.FilterElement[] { });
			ctrlList.DataSource = obj;
			ctrlList.DataBind();
		}
		#endregion
	}
}
