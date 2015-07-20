using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.ManageControls
{
	public partial class EnumViewControl : System.Web.UI.UserControl
	{
		protected MetaFieldType mft = null;

		#region TypeName
		private string _type = "";
		public string TypeName
		{
			get { return _type; }
			set { _type = value; }
		}
		#endregion

		#region RefreshButton
		private string _refreshButton = String.Empty;
		public string RefreshButton
		{
			get { return _refreshButton; }
			set { _refreshButton = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["type"] != null)
			{
				TypeName = Request.QueryString["type"];
				mft = DataContext.Current.MetaModel.RegisteredTypes[TypeName];
			}

			if (Request.QueryString["btn"] != null)
			{
				RefreshButton = Request.QueryString["btn"];
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EnumInfo").ToString();
			secHeader.AddLink(
				String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle'> {1}", CHelper.GetAbsolutePath("/Images/IbnFramework/newitem.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewItem").ToString()),
				Page.ClientScript.GetPostBackClientHyperlink(btnAddNewItem, ""));
			if (RefreshButton == String.Empty)
			{

				secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/edit.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EnumEdit").ToString(), "~/Apps/MetaDatabase/Pages/Admin/EnumEdit.aspx?type=" + TypeName + "&back=view");
				secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDatabase/Pages/Admin/EnumList.aspx");
			}
			else  // Dialog Mode
			{
				string url = String.Format("javascript:try{{window.opener.{0}}} catch (e){{}} window.close();", RefreshButton);
				secHeader.AddImageLink(CHelper.GetAbsolutePath("/images/IbnFramework/close.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Close").ToString(), url);
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			lblFriendlyName.Text = CHelper.GetResFileString(mft.FriendlyName);
			MetaFieldType mft1 = MetaDataWrapper.GetEnumByName(TypeName);
			if (mft1 != null)
			{
				if (mft1.Attributes.ContainsKey(McDataTypeAttribute.EnumMultivalue))
					lbType.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MultiValue").ToString();
				else
					lbType.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "SingleValue").ToString();
			}
			BindGrid(GetDataTable());
		}
		#endregion

		#region BindGrid
		private void BindGrid(DataTable dt)
		{
			grdMain.DataSource = dt.DefaultView;
			grdMain.DataBind();

			foreach (DataGridItem row in grdMain.Items)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
				TextBox tb = (TextBox)row.FindControl("txtName");
				HtmlImage im = (HtmlImage)row.FindControl("imResourceTemplate");
				RequiredFieldValidator rfv = (RequiredFieldValidator)row.FindControl("rfName");
				if (im != null)
				{
					im.Src = CHelper.GetAbsolutePath("/images/IbnFramework/resource.gif");
					im.Attributes.Add("title", GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "ResourceTooltip").ToString());
					if (tb != null && rfv != null)
						im.Attributes.Add("onclick", "SetText('" + tb.ClientID + "','{ResourceName:ResourceKey}','" + rfv.ClientID + "')");
				}
			}

			if (grdMain.EditItemIndex >= 0)
			{
				DropDownList ddl = (DropDownList)grdMain.Items[grdMain.EditItemIndex].FindControl("ddlOrder");
				if (ddl != null)
				{
					for (int i = 1; i <= grdMain.Items.Count; i++)
					{
						ddl.Items.Add(i.ToString());
					}
					ddl.SelectedIndex = grdMain.EditItemIndex;
				}
			}
		}
		#endregion

		#region GetDataTable
		private DataTable GetDataTable()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("OrderId", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("DisplayName", typeof(string));

			foreach (MetaEnumItem item in MetaEnum.GetItems(mft))
			{
				DataRow row = dt.NewRow();
				row["Id"] = item.Handle;
				row["OrderId"] = item.OrderId;
				row["Name"] = item.Name;
				row["DisplayName"] = CHelper.GetResFileString(item.Name);
				dt.Rows.Add(row);
			}
			return dt;
		}
		#endregion

		#region grdMain_CancelCommand
		protected void grdMain_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			grdMain.EditItemIndex = -1;
			BindGrid(GetDataTable());
		}
		#endregion

		#region grdMain_DeleteCommand
		protected void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			if (mft != null)
				MetaEnum.RemoveItem(mft, int.Parse(e.CommandArgument.ToString()));
			BindGrid(GetDataTable());
		}
		#endregion

		#region grdMain_EditCommand
		protected void grdMain_EditCommand(object source, DataGridCommandEventArgs e)
		{
			grdMain.EditItemIndex = e.Item.ItemIndex;

			BindGrid(GetDataTable());
		}
		#endregion

		#region grdMain_UpdateCommand
		protected void grdMain_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			if (mft != null)
			{
				int ItemId = int.Parse(e.CommandArgument.ToString());

				TextBox tb = (TextBox)e.Item.FindControl("txtName");
				DropDownList ddl = (DropDownList)e.Item.FindControl("ddlOrder");
				int OrderId = int.Parse(ddl.SelectedValue);

				if (tb != null && tb.Text.Trim() != String.Empty)
				{
					if (ItemId > 0)
						MetaEnum.UpdateItem(mft, ItemId, tb.Text.Trim(), OrderId);
					else
						MetaEnum.AddItem(mft, tb.Text.Trim(), OrderId);
				}
			}
			grdMain.EditItemIndex = -1;
			BindGrid(GetDataTable());
		}
		#endregion

		#region btnAddNewItem_Click
		protected void btnAddNewItem_Click(object sender, EventArgs e)
		{
			DataTable dt = GetDataTable();

			DataRow dr = dt.NewRow();
			dr["Id"] = -1;
			dr["OrderId"] = dt.Rows.Count + 1;
			dr["Name"] = "";
			dt.Rows.Add(dr);

			grdMain.EditItemIndex = dt.Rows.Count - 1;
			BindGrid(dt);
		}
		#endregion
	}
}