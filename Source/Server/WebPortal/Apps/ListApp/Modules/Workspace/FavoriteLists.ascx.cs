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
using Mediachase.IBN.Business;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.ListApp.Modules.Workspace
{
	public partial class FavoriteLists : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			BindTable();
		}

		#region BindTable
		/// <summary>
		/// Binds the table.
		/// </summary>
		void BindTable()
		{
			int i = 1;
			grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:tName}");
			//grdMain.Columns[i++].HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:CreatedBy}");

			DataTable dt = ListInfoBus.GetListFavoritesDT();
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));

			foreach (DataRow row in dt.Rows)
			{
				string metaClassName = String.Format("List_{0}", row["ObjectId"]);
				ListInfo li = ListManager.GetListInfoByMetaClassName(metaClassName);

				row["Name"] = String.Format("<a href='{2}?ClassName={0}'>{1}</a>",
					metaClassName, li.Title, ResolveUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx"));
				row["Icon"] = this.ResolveUrl("~/layouts/images/blank.gif");
				row["UserName"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(li.CreatorId);
			}

			grdMain.DataSource = dt;
			grdMain.DataBind();

			if (dt.Rows.Count == 0)
			{
				panelNoFavList.Visible = true;
				grdMain.Visible = false;
			}
			else
			{
				panelNoFavList.Visible = false;
				grdMain.Visible = true;
			}
		} 
		#endregion
	}
}