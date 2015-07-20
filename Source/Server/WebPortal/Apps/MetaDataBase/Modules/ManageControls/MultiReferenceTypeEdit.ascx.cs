using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MultiReferenceTypeEdit : System.Web.UI.UserControl
	{
		#region TypeName
		public string TypeName
		{
			get
			{
				if (Request.QueryString["type"] != null)
					return Request.QueryString["type"].ToString();
				else
					return string.Empty;
			}
		}
		#endregion

		#region BackMode
		public string BackMode
		{
			get
			{
				if (Request.QueryString["back"] != null)
					return Request.QueryString["back"].ToString();
				else
					return string.Empty;

			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			this.imbtnSave.ServerClick += new EventHandler(imbtnSave_ServerClick);
			this.imbtnCancel.ServerClick += new EventHandler(imbtnCancel_ServerClick);
			this.dgClasses.DeleteCommand += new DataGridCommandEventHandler(dgClasses_delete);
			this.dgClasses.EditCommand += new DataGridCommandEventHandler(dgClasses_edit);
			this.dgClasses.CancelCommand += new DataGridCommandEventHandler(dgClasses_cancel);
			this.dgClasses.UpdateCommand += new DataGridCommandEventHandler(dgClasses_update);
			BindToolbar();

			if (!IsPostBack)
				BindData();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			if (TypeName == string.Empty)
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MReferenceTypeCreate").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "MReferenceTypeEdit").ToString();

			if (BackMode == "view" && TypeName != string.Empty)
				secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToEnumInfo").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type=" + TypeName);
			else
				secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");

			imbtnSave.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/saveitem.gif");
			imbtnCancel.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif");

			txtMRTName.Attributes.Add("onblur", "SetName('" + txtMRTName.ClientID + "','" + txtFriendlyName.ClientID + "','" + vldFriendlyName_Required.ClientID + "')");
		}
		#endregion

		#region Cancel
		void imbtnCancel_ServerClick(object sender, EventArgs e)
		{
			if (BackMode == "view" && TypeName != string.Empty)
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type=" + TypeName);
			else
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");
		}
		#endregion

		#region Save
		void imbtnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			if (TypeName == string.Empty)
			{
				string typeName = String.Empty;
				using (MetaClassManagerEditScope editScope = DataContext.Current.MetaModel.BeginEdit())
				{
					DataTable dt = (DataTable)ViewState["DT_Source"];
					List<MultiReferenceItem> mas = new List<MultiReferenceItem>();
					foreach (DataRow dr in dt.Rows)
						mas.Add(new MultiReferenceItem(dr["Name"].ToString(), dr["Name"].ToString(), dr["FriendlyName"].ToString()));
					MetaFieldType type = MultiReferenceType.Create(txtMRTName.Text, txtFriendlyName.Text, mas.ToArray());
					typeName = type.Name;
					editScope.SaveChanges();
				}
				//if (typeName != String.Empty)
				//    Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type={0}", typeName));
				//else
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");
			}
			else
			{
				MetaFieldType type = MetaDataWrapper.GetTypeByName(TypeName);
				type.FriendlyName = txtFriendlyName.Text;
				Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type={0}", TypeName));
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("FriendlyName", typeof(string)));

			if (TypeName != string.Empty)
			{
				MetaFieldType mft = MetaDataWrapper.GetTypeByName(TypeName);
				if (mft != null)
				{
					txtMRTName.Text = mft.Name;
					txtMRTName.ReadOnly = true;
					txtMRTName.CssClass = "text-readonly";
					txtFriendlyName.Text = mft.FriendlyName;
				}

				DataRow dr;
				foreach (MultiReferenceItem mri in MultiReferenceType.GetMultiReferenceItems(mft))
				{
					dr = dt.NewRow();
					dr["Name"] = mri.MetaClassName;
					dr["FriendlyName"] = mri.MetaClassName;
					dt.Rows.Add(dr);
				}
				dgClasses.Columns[2].Visible = false;
			}
			ViewState["DT_Source"] = dt;
			BindDG();
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			dgClasses.Columns[1].HeaderText = "MetaClass Name";
			if (TypeName == String.Empty)
				dgClasses.Columns[2].HeaderText = String.Format("<a href='#' onclick=\"{0}\"><img alt='' align='absmiddle' border='0' src='{1}' />&nbsp;New</a>",
					Page.ClientScript.GetPostBackClientHyperlink(lbNewClass, ""),
					CHelper.GetAbsolutePath("/images/IbnFramework/newitem.gif"));
			DataTable dt = (DataTable)ViewState["DT_Source"];
			dgClasses.DataSource = dt.DefaultView;
			dgClasses.DataBind();
		}
		#endregion

		#region dgClasses Events
		protected void lbNewClass_Click(object sender, EventArgs e)
		{
			DataTable dt = ((DataTable)ViewState["DT_Source"]).Copy();

			DataRow dr = dt.NewRow();
			dr["Name"] = "";
			dr["FriendlyName"] = "";
			dt.Rows.Add(dr);

			dgClasses.EditItemIndex = dt.Rows.Count - 1;
			dgClasses.DataKeyField = "Name";
			dgClasses.DataSource = dt.DefaultView;
			dgClasses.DataBind();

			foreach (DataGridItem dgi in dgClasses.Items)
			{
				DropDownList ddl = (DropDownList)dgi.Cells[1].FindControl("ddClasses");
				if (ddl != null)
				{
					//Dictionary<int, string> dataSource = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
					//List<string> list = new List<string>(dataSource.Values);
					List<string> list = new List<string>();
					foreach (MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
						list.Add(mc.Name);
					list.Sort();
					ddl.DataSource = list;
					ddl.DataBind();
				}
			}
		}

		private void dgClasses_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sName = dgClasses.DataKeys[e.Item.ItemIndex].ToString();
			DataTable dt = ((DataTable)ViewState["DT_Source"]).Copy();
			DataRow[] dr = dt.Select("Name = '" + sName + "'");
			if (dr.Length > 0)
			{
				dt.Rows.Remove(dr[0]);
			}
			ViewState["DT_Source"] = dt;
			dgClasses.EditItemIndex = -1;
			BindDG();
		}

		private void dgClasses_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgClasses.EditItemIndex = e.Item.ItemIndex;
			dgClasses.DataKeyField = "Name";
			BindDG();

			foreach (DataGridItem dgi in dgClasses.Items)
			{
				DropDownList ddl = (DropDownList)dgi.Cells[1].FindControl("ddClasses");
				if (ddl != null)
				{
					//Dictionary<int, string> dataSource = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
					//List<string> list = new List<string>(dataSource.Values);
					List<string> list = new List<string>();
					foreach (MetaClass mc in DataContext.Current.MetaModel.MetaClasses)
						list.Add(mc.Name);
					list.Sort();
					ddl.DataSource = list;
					ddl.DataBind();
					CHelper.SafeSelect(ddl, e.Item.Cells[0].Text);
				}
			}
		}

		private void dgClasses_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgClasses.EditItemIndex = -1;
			BindDG();
		}

		private void dgClasses_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sName = dgClasses.DataKeys[e.Item.ItemIndex].ToString();
			DataTable dt = ((DataTable)ViewState["DT_Source"]).Copy();
			DataRow[] dr = dt.Select("Name = '" + sName + "'");
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddClasses");
			if (ddl != null)
			{
				DataRow[] drTest = dt.Select("Name = '" + ddl.SelectedValue + "'");
				if (drTest.Length == 0)
				{
					if (dr.Length > 0)
					{
						dr[0]["Name"] = ddl.SelectedValue;
						dr[0]["FriendlyName"] = ddl.SelectedValue;
					}
					else
					{
						DataRow drNew = dt.NewRow();
						drNew["Name"] = ddl.SelectedValue;
						drNew["FriendlyName"] = ddl.SelectedValue;
						dt.Rows.Add(drNew);
					}
				}
			}
			ViewState["DT_Source"] = dt;
			dgClasses.EditItemIndex = -1;
			BindDG();
		}
		#endregion
	}
}
