using System;
using System.Data;
using System.Globalization;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class ActiveProjects : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private const string prefix = "Project";
		private const string keySort = prefix + "Project_sort";

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			ctrlGrid.ChangingMCGridColumnHeader += new Mediachase.Ibn.Web.UI.ChangingMCGridColumnHeaderEventHandler(ctrlGrid_ChangingMCGridColumnHeader);
			BindDataGrid(!IsPostBack);
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

		void BindDataGrid(bool dataBind)
		{
			DataTable dt = Project.GetListActiveProjectsByUserOnlyDataTable();
			DataView dv = dt.DefaultView;

			if (pc[keySort] == null)
				pc[keySort] = "Title ASC";

			ctrlGrid.DataSource = dv;

			divNoObjects.Visible = false;
			if (dv.Count == 0)
			{
				ctrlGrid.Visible = false;
				lblNoObjects.Text = GetGlobalResourceObject("IbnFramework.Global", "NoProjects").ToString();
				if (Project.CanCreate())
				{
					lblNoObjects.Text += String.Format(" <a href='{0}'>{1}</a>", 
						this.Page.ResolveUrl("~/Projects/ProjectEdit.aspx"),
						GetGlobalResourceObject("IbnFramework.Global", "CreateProject").ToString());
				}
				divNoObjects.Visible = true;
			}
		}
	}
}