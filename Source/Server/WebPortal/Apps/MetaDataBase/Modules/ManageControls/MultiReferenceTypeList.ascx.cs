using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MultiReferenceTypeList : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region BindData
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("IsUsed", typeof(bool));

			foreach (MetaFieldType mfType in DataContext.Current.MetaModel.GetRegisteredTypes(McDataType.MultiReference))
			{
				DataRow row = dt.NewRow();
				row["Name"] = mfType.Name;
				row["FriendlyName"] = CHelper.GetResFileString(mfType.FriendlyName);
				row["IsUsed"] = MetaFieldType.IsUsed(mfType);
				dt.Rows.Add(row);
			}
			DataView dv = dt.DefaultView;
			if (this.Session["MRTList_Sort"] == null)
				this.Session["MRTList_Sort"] = "FriendlyName";
			dv.Sort = this.Session["MRTList_Sort"].ToString();

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (GridViewRow row in grdMain.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");

				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MReferenceTypeList").ToString();
			secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Create").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeEdit.aspx");
		}
		#endregion

		#region grdMain_Sorting
		protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (this.Session["MRTList_Sort"].ToString() == e.SortExpression)
				this.Session["MRTList_Sort"] += " DESC";
			else
				this.Session["MRTList_Sort"] = e.SortExpression;
			BindData();
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				MetaFieldType type = MetaDataWrapper.GetTypeByName(e.CommandArgument.ToString());
				if (!MetaFieldType.IsUsed(type))
					MultiReferenceType.Remove(type);
				BindData();
			}
			if (e.CommandName == "Edit")
			{
				MetaFieldType type = MetaDataWrapper.GetTypeByName(e.CommandArgument.ToString());
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeEdit.aspx?type=" + type.Name);
			}
		}
		#endregion

		#region grdMain_RowDeleting
		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
		}
		#endregion
	}
}