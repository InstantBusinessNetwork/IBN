using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public partial class EntityExportSettings : System.Web.UI.UserControl
	{
		#region _type
		private string _type
		{
			get
			{
				if (Request["Type"] != null)
					return Request["Type"];
				else
					return ExportMode.Excel.ToString();
			}
		}
		#endregion

		#region _refreshCommand
		private string _refreshCommand
		{
			get
			{
				if (Request["refreshCommand"] != null)
					return Request["refreshCommand"];
				else
					return String.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindValues();
				CHelper.SafeSelect(rbList, "0");
			}
		}

		private void BindValues()
		{
			rbList.Items.Clear();
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Common", "AllEntitiesExport").ToString(), "0"));
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Common", "ViewEntitiesExport").ToString(), "1"));
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Common", "VisibleEntitiesExport").ToString(), "2"));
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Common", "SelectedEntitiesExport").ToString(), "3"));

			btnExport.Text = GetGlobalResourceObject("IbnFramework.Common", "Export").ToString();
		}

		protected void btnExport_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(_refreshCommand);
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("Type", _type);
			cp.AddCommandArgument("Variant", rbList.SelectedValue);
			cp.AddCommandArgument("ClassName", Request["ClassName"]);
			cp.AddCommandArgument("GridId", Request["GridId"]);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}