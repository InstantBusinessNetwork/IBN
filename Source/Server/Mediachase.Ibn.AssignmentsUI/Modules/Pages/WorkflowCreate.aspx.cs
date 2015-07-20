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
	public partial class WorkflowCreate : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblCreate.Click += new EventHandler(lblCreate_Click);
		}

		#region lblCreate_Click
		/// <summary>
		/// Handles the Click event of the lblCreate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void lblCreate_Click(object sender, EventArgs e)
		{
			WorkflowDefinitionEntity en = BusinessManager.InitializeEntity<WorkflowDefinitionEntity>(WorkflowDefinitionEntity.ClassName);
			en.Name = tbName.Text;
			PrimaryKeyId newId = BusinessManager.Create(en);
			Response.Redirect(String.Format("~/TestWF.aspx?WfId={0}", newId.ToString()));
		}
		#endregion
	}
}
