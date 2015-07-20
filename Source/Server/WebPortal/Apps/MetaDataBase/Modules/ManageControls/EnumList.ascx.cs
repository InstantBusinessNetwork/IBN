using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class EnumList : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region grdMain_Sorting
		protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (this.Session["EnumList_Sort"].ToString() == e.SortExpression)
				this.Session["EnumList_Sort"] += " DESC";
			else
				this.Session["EnumList_Sort"] = e.SortExpression;
			BindData();
		}
		#endregion

		#region BindData
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("Type", typeof(string));
			dt.Columns.Add("IsUsed", typeof(bool));

			foreach (MetaFieldType mfType in MetaDataWrapper.GetEnumList())
			{
				DataRow row = dt.NewRow();
				row["Name"] = mfType.Name;
				row["FriendlyName"] = CHelper.GetResFileString(mfType.FriendlyName);
				if (mfType.Attributes.ContainsKey(McDataTypeAttribute.EnumMultivalue))
					row["Type"] = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MultiValue").ToString();
				else
					row["Type"] = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "SingleValue").ToString();
				row["IsUsed"] = MetaEnum.IsUsed(mfType);
				dt.Rows.Add(row);
			}
			DataView dv = dt.DefaultView;
			if (this.Session["EnumList_Sort"] == null)
				this.Session["EnumList_Sort"] = "FriendlyName";
			dv.Sort = this.Session["EnumList_Sort"].ToString();

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
			secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EnumList").ToString();
			secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Create").ToString(), "~/Apps/MetaDataBase/Pages/Admin/EnumEdit.aspx");
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				MetaFieldType type = MetaDataWrapper.GetEnumByName(e.CommandArgument.ToString());
				if (!MetaEnum.IsUsed(type))
					MetaEnum.Remove(type);
				BindData();
			}
			if (e.CommandName == "Edit")
			{
				MetaFieldType type = MetaDataWrapper.GetEnumByName(e.CommandArgument.ToString());
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/EnumEdit.aspx?type=" + type.Name);
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