using System;
using System.Collections;
using System.Data;
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
	public partial class MetaLinkEdit : System.Web.UI.UserControl
	{
		MetaClass _mc = null;
		MetaField _mf = null;

		#region className
		private string className
		{
			get
			{
				if (Request.QueryString["class"] != null)
					return Request.QueryString["class"];
				else
					return "";
			}
		}
		#endregion

		#region fieldName
		private string fieldName
		{
			get
			{
				if (Request.QueryString["field"] != null)
					return Request.QueryString["field"];
				else
					return "";
			}
		}
		#endregion

		#region Page_Init
		protected void Page_Init(object sender, EventArgs e)
		{
			this.imbtnSave.ServerClick += new EventHandler(imbtnSave_ServerClick);
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			_mc = DataContext.Current.MetaModel.MetaClasses[className];
			if (!String.IsNullOrEmpty(fieldName))
				_mf = _mc.Fields[fieldName];

			//CHelper.AddToContext(NavigationBlock.KeyContextMenu, "MetaClassView");
			//CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CHelper.GetResFileString(_mc.FriendlyName));

			if (!IsPostBack)
			{
				lblOwnerMetaClass.Text = _mc.FriendlyName;
				BindMetaClasses();
				BindMetaFields();

				if (_mf != null)
					BindEditData();
				else
				{
					trEdit1.Visible = false;
					trEdit2.Visible = false;
				}
			}

			imbtnSave.Attributes.Add("onclick", "if(!SaveFunc()) return;");
			imbtnSave.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/saveitem.gif");
			imbtnCancel.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif");
			imbtnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}'; return false;", _mc.Name));
		}

		#region BindEditData
		private void BindEditData()
		{
			lblFieldName.Text = _mf.Name;
			txtFriendlyName.Text = _mf.FriendlyName;
			CHelper.SafeSelect(ddOwnerFields, _mf.LinkInformation.LinkedFieldList[0]);
			ddOwnerFields.Enabled = false;
			foreach (string asClass in _mf.LinkInformation.AssignedMetaClassList)
				CHelper.SafeMultipleSelect(lbLinkMetaClasses, asClass);
			BindGrid();
		} 
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindInfo();
		}
		#endregion

		#region BindMetaClasses
		private void BindMetaClasses()
		{
			DataView dv = GetTableClasses().DefaultView;
			dv.Sort = "FriendlyName";
			dv.RowFilter = "Name<>'" + _mc.Name + "'";
			lbLinkMetaClasses.DataSource = dv;
			lbLinkMetaClasses.DataTextField = "FriendlyName";
			lbLinkMetaClasses.DataValueField = "Name";
			lbLinkMetaClasses.DataBind();
		}
		#endregion

		#region BindMetaFields
		private void BindMetaFields()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));

			foreach (MetaField mf in _mc.Fields)
			{
				if (mf.IsReference || mf.IsBackReference || mf.IsLink)
					continue;
				DataRow row = dt.NewRow();
				row["Name"] = mf.Name;
				row["FriendlyName"] = CHelper.GetResFileString(mf.FriendlyName);
				dt.Rows.Add(row);
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "FriendlyName";

			ddOwnerFields.DataSource = dv;
			ddOwnerFields.DataTextField = "FriendlyName";
			ddOwnerFields.DataValueField = "Name";
			ddOwnerFields.DataBind();

			btnAdd.Attributes.Add("onclick", "return SelectFunc();");
		}
		#endregion

		#region BindInfo
		private void BindInfo()
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewLink").ToString();
			secHeader.AddLink(
				"<img src='" + ResolveClientUrl("~/images/IbnFramework/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToTableInfo").ToString(),
				String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, _mc.Name));
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			foreach (ListItem li in lbLinkMetaClasses.Items)
				if (li.Selected)
				{
					TemplateField tf = new TemplateField();
					tf.ItemStyle.CssClass = "ibn-vb";
					tf.ItemStyle.Width = Unit.Pixel(210);
					tf.HeaderStyle.CssClass = "ibn-vh";
					tf.HeaderText = li.Text;
					tf.ItemTemplate = new DropDownTemplateField(li.Value);
					grdMain.Columns.Add(tf);
				}

			DataView dv = GetTableFields().DefaultView;
			dv.Sort = "FriendlyName";
			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (GridViewRow gvr in grdMain.Rows)
			{
				foreach (ListItem li in lbLinkMetaClasses.Items)
					if (li.Selected)
					{
						DropDownList ddl = (DropDownList)gvr.FindControl(li.Value);
						if (ddl != null)
						{
							DataView dv1 = GetTableFieldsByClass(li.Value).DefaultView;
							dv1.Sort = "FriendlyName";
							ddl.DataSource = dv1;
							ddl.DataTextField = "FriendlyName";
							ddl.DataValueField = "Name";
							ddl.DataBind();
							if (!Page.IsPostBack && _mf != null)
							{
								MetaFieldMapping mfm = _mf.LinkInformation.FindMapping(li.Value, _mc.Name, _mf.LinkInformation.LinkedFieldList[0]);
								if (mfm != null)
									CHelper.SafeSelect(ddl, mfm.SrcMetaFieldName);
							}
						}
					}
			}
		}
		#endregion

		#region btnAdd_Click
		protected void btnAdd_Click(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "Delete" && _mc != null)
			{
				BindGrid();
			}
		}
		#endregion

		#region grdMain_RowDeleting
		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
		}
		#endregion

		#region imbtnSave_ServerClick
		protected void imbtnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			if (ddOwnerFields.SelectedItem == null)
				return;

			using (MetaClassManagerEditScope editScope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaField newLinkField = _mf;
				MetaField field = _mc.Fields[ddOwnerFields.SelectedValue];
				if (newLinkField != null)
				{
					newLinkField.LinkInformation.AssignedMetaClassList.Clear();
					newLinkField.LinkInformation.MappingList.Clear();
					newLinkField.FriendlyName = txtFriendlyName.Text;
				}

				string sValues = hidField.Value;
				string[] mas = sValues.Split(';');
				if (mas.Length > 0)
					for (int i = 0; i < mas.Length; i++)
					{
						string sValue = mas[i];
						if (sValue.Length > 0)
						{
							string[] mas_in = sValue.Split(':');
							string sClass = mas_in[0];
							string sField = mas_in[1];

							if (newLinkField == null)
							{
								newLinkField = _mc.CreateMetaField(field.Name + "Link", field.FriendlyName + " Link", "Link", new Mediachase.Ibn.Data.Meta.Management.AttributeCollection());
								newLinkField.LinkInformation.LinkedFieldReadonly = true;
								newLinkField.LinkInformation.TitleIndex = 0;
								newLinkField.LinkInformation.LinkedFieldList.Add(field.Name);
							}

							newLinkField.LinkInformation.AssignedMetaClassList.Add(sClass);
							newLinkField.LinkInformation.MappingList.Add(new MetaFieldMapping(
								DataContext.Current.MetaModel.MetaClasses[sClass].Fields[sField]
								, field));
						}
					}
				editScope.SaveChanges();
			}

			Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, _mc.Name), true);
		}
		#endregion

		#region imbtnCancel_ServerClick
		protected void imbtnCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, _mc.Name), true);
		} 
		#endregion

		#region GetTableFields
		private DataTable GetTableFields()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("IsSystem", typeof(bool));
			dt.Columns.Add("FieldTypeImageUrl", typeof(string));

			DataRow row = dt.NewRow();
			row["Name"] = ddOwnerFields.SelectedValue;
			row["FriendlyName"] = CHelper.GetResFileString(ddOwnerFields.SelectedItem.Text);
			MetaField field = _mc.Fields[ddOwnerFields.SelectedValue];
			row["IsSystem"] = field.Attributes.ContainsKey(MetaClassAttribute.IsSystem) || _mc.TitleFieldName == field.Name;
			string postfix = string.Empty;
			if ((bool)row["IsSystem"])
			{
				postfix = "_sys";
			}
			if (field.IsReferencedField)
				row["FieldTypeImageUrl"] = CHelper.GetAbsolutePath("/images/IbnFramework/metainfo/referencedfield" + postfix + ".gif");
			else if (field.IsReference)
				row["FieldTypeImageUrl"] = CHelper.GetAbsolutePath("/images/IbnFramework/metainfo/reference" + postfix + ".gif");
			else if (field.IsBackReference)
				row["FieldTypeImageUrl"] = CHelper.GetAbsolutePath("/images/IbnFramework/metainfo/backreference" + postfix + ".gif");
			else
				row["FieldTypeImageUrl"] = CHelper.GetAbsolutePath("/images/IbnFramework/metainfo/metafield" + postfix + ".gif");
			dt.Rows.Add(row);

			return dt;
		}
		#endregion

		#region GetTableFieldsByClass
		private DataTable GetTableFieldsByClass(string className)
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			MetaClass mc1 = DataContext.Current.MetaModel.MetaClasses[className];
			foreach (MetaField field in mc1.Fields)
			{
				DataRow row = dt.NewRow();
				row["Name"] = field.Name;
				row["FriendlyName"] = CHelper.GetResFileString(field.FriendlyName);
				dt.Rows.Add(row);
			}
			return dt;
		}
		#endregion

		#region GetTableClasses
		private DataTable GetTableClasses()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));

			foreach (MetaClass mc1 in DataContext.Current.MetaModel.MetaClasses)
			{
				if (mc1.IsCard || mc1.IsBridge)
					continue;
				DataRow row = dt.NewRow();
				row["Name"] = mc1.Name;
				row["FriendlyName"] = CHelper.GetResFileString(mc1.FriendlyName);
				dt.Rows.Add(row);
			}

			return dt;
		}
		#endregion
	}

	public class DropDownTemplateField : System.Web.UI.ITemplate
	{
		private string _className = "";
		public DropDownTemplateField(string className)
		{
			_className = className;
		}

		#region ITemplate Members

		public void InstantiateIn(System.Web.UI.Control container)
		{
			DropDownList ddl = new DropDownList();
			ddl.Width = Unit.Pixel(200);
			ddl.ID = _className;
			ddl.Attributes.Add("metaclass", _className);
			container.Controls.Add(ddl);
		}

		#endregion
	}
}