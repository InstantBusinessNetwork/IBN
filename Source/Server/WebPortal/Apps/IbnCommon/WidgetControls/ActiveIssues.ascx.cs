using System;
using System.Data;
using System.Globalization;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class ActiveIssues : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private const string prefix = "Issues";
		private const string keySort = prefix + "Issues_sort";

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			BindDataGrid(!IsPostBack);
			ctrlGrid.ChangingMCGridColumnHeader += new Mediachase.Ibn.Web.UI.ChangingMCGridColumnHeaderEventHandler(ctrlGrid_ChangingMCGridColumnHeader);
		}

		void ctrlGrid_ChangingMCGridColumnHeader(object sender, Mediachase.Ibn.Web.UI.ChangingMCGridColumnHeaderEventArgs e)
		{
			if (e.FieldName == "PriorityId")
			{
				e.ControlField.HeaderText = string.Format(CultureInfo.InvariantCulture, "<span title='{0}'>!!!</span>", GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
				//e.ControlField.HeaderText = String.Format("<img width='16' height='16' src='{0}' title='{1}'>",
				//    this.Page.ResolveClientUrl("~/layouts/images/PriorityHeader.gif"),
				//    GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
			}
		}

		#region BindDataGrid
		/// <summary>
		/// Binds the data grid.
		/// </summary>
		/// <param name="dataBind">if set to <c>true</c> [data bind].</param>
		void BindDataGrid(bool dataBind)
		{
			DataTable dt = Incident.GetListOpenIncidentsByUserOnlyDataTable();
			/*
						#region Variables
		int projId = 0; int iManId = 0; int iRespId = 0; int iCreatorId = 0; int iResId = 0;
		int priority_id = -1; int issbox_id = 0; int type_id = 0; int org_id = 0; int vcard_id = 0;
		int state_id = 0; int severity_id = 0; int genCategory_type = 0; int issCategory_type = 0;
		#endregion

		bool isUnansweredOnly = false;
 
			 * GetListIncidentsByFilterDataTable
					(projId, iManId, iCreatorId, iResId, iRespId, org_id, vcard_id,
					issbox_id, priority_id, type_id, state_id, severity_id, string.Empty,
					genCategory_type, issCategory_type, isUnansweredOnly, false);*/

			DataView dv = dt.DefaultView;

			if (pc[keySort] == null)
				pc[keySort] = "IncidentId DESC";

			dv.Sort = pc[keySort];
			ctrlGrid.DataSource = dv;
			
			divNoObjects.Visible = false;
			if (dv.Count == 0)
			{
				ctrlGrid.Visible = false;
				lblNoObjects.Text = String.Format("{0} <a href='{1}'>{2}</a>",
					GetGlobalResourceObject("IbnFramework.Global", "NoIssues").ToString(),
					this.Page.ResolveUrl("~/Incidents/IncidentEdit.aspx"),
						GetGlobalResourceObject("IbnFramework.Global", "CreateIssue").ToString());
				divNoObjects.Visible = true;
			}
		} 
		#endregion
	}
}