using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.TemplateControls
{
	public partial class ListTemplates : System.Web.UI.UserControl
	{
		UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindGrid();

			BindToolbar();

			grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			grdMain.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
		}

		void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			ListManager.DeleteList(int.Parse(e.CommandArgument.ToString()));
			pc["Lists_Temp_Page"] = "0";
			BindGrid();
		}

		void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["Lists_Temp_PageSize"] = e.NewPageSize.ToString();
			BindGrid();
		}

		void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["Lists_Temp_Page"] = e.NewPageIndex.ToString();
			BindGrid();
		}

		private void BindToolbar()
		{
			secHeader.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:tListTemplates}");
		}

		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ListId", typeof(int)));
			dt.Columns.Add(new DataColumn("ClassName", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			DataRow dr;

			foreach (ListInfo li in ListManager.GetTemplates())
			{
				dr = dt.NewRow();
				dr["ListId"] = li.PrimaryKeyId.Value;
				dr["ClassName"] = ListManager.GetListMetaClassName(li.PrimaryKeyId.Value);
				dr["Name"] = li.Title;
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "Name";

			if (pc["Lists_Temp_PageSize"] != null)
				grdMain.PageSize = int.Parse(pc["Lists_Temp_PageSize"]);

			int iPageIndex = 0;
			if (pc["Lists_Temp_Page"] != null)
			{
				iPageIndex = int.Parse(pc["Lists_Temp_Page"]);
			}

			int ppi = dv.Count / grdMain.PageSize;
			if (dv.Count % grdMain.PageSize == 0)
				ppi = ppi - 1;
			if (iPageIndex <= ppi)
				grdMain.CurrentPageIndex = iPageIndex;
			else grdMain.CurrentPageIndex = 0;

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("btnDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + CHelper.GetResFileString("{IbnFramework.ListInfo:WarningT}") + "?');");
			}
		}
	}
}