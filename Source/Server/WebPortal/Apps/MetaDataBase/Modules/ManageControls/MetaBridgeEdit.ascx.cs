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

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaBridgeEdit : System.Web.UI.UserControl
	{
		MetaClass mc = null;

		#region ClassName
		private string _className = "";
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

		#region Back
		private string _back = "";
		public string Back
		{
			get { return _back; }
			set { _back = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();
			BindInfo();

			if (!IsPostBack)
				BindData();
		}

		#region Page_Init
		protected void Page_Init(object sender, EventArgs e)
		{
			this.imbtnSave.ServerClick += new EventHandler(imbtnSave_ServerClick);
		}
		#endregion

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}

			if (Request.QueryString["back"] != null)
				Back = Request.QueryString["back"].ToLower();
		}
		#endregion

		#region BindInfo
		private void BindInfo()
		{
			if (mc != null)
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BridgeEdit").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BridgeCreate").ToString();

			if (Back == "view" && mc != null)
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToTableInfo").ToString()),
				  String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, mc.Name));

				imbtnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}'; return false;", mc.Name));
			}
			else  // Back to List by default
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToList").ToString()),
				  "~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx");

				imbtnCancel.Attributes.Add("onclick", "window.location.href='MetaClassList.aspx'; return false;");
			}

			imbtnSave.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/saveitem.gif");
			imbtnCancel.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif");

			lgdClass.InnerText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "TableInfo").ToString();
			lgdField1.InnerText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Field1Info").ToString();
			lgdField2.InnerText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Field2Info").ToString();
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (mc == null)
			{
				txtClassName.Attributes.Add("onblur", "SetName('" + txtClassName.ClientID + "','" + txtClassFriendlyName.ClientID + "','" + vldClassFriendlyName_Required.ClientID + "')" + "; SetName('" + txtClassName.ClientID + "','" + txtClassPluralName.ClientID + "','" + vldClassPluralName_Required.ClientID + "')");
				txtField1Name.Attributes.Add("onblur", "SetName('" + txtField1Name.ClientID + "','" + txtField1FriendlyName.ClientID + "','" + vldField1FriendlyName_Required.ClientID + "')");
				txtField2Name.Attributes.Add("onblur", "SetName('" + txtField2Name.ClientID + "','" + txtField2FriendlyName.ClientID + "','" + vldField2FriendlyName_Required.ClientID + "')");

				foreach (MetaClass cls in DataContext.Current.MetaModel.MetaClasses)
				{
					if (!cls.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
					{
						ddlClass1.Items.Add(new ListItem(cls.Name, cls.Name));
						ddlClass2.Items.Add(new ListItem(cls.Name, cls.Name));
					}
				}
			}
			else
			{
				txtClassName.Text = mc.Name;
				txtClassName.Enabled = false;

				txtClassFriendlyName.Text = mc.FriendlyName;
				txtClassPluralName.Text = mc.PluralName;

				MetaField[] refFields = mc.GetBridgeFields();
				ddlClass1.Items.Add(new ListItem(refFields[0].ReferenceToMetaClassName, refFields[0].ReferenceToMetaClassName));
				ddlClass1.Enabled = false;
				ddlClass2.Items.Add(new ListItem(refFields[1].ReferenceToMetaClassName, refFields[1].ReferenceToMetaClassName));
				ddlClass2.Enabled = false;

				txtField1Name.Text = refFields[0].Name;
				txtField1Name.Enabled = false;

				txtField1FriendlyName.Text = refFields[0].FriendlyName;

				txtField2Name.Text = refFields[1].Name;
				txtField2Name.Enabled = false;

				txtField2FriendlyName.Text = refFields[1].FriendlyName;
			}
		}
		#endregion

		#region imbtnSave_ServerClick
		void imbtnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (mc == null)
			{
				try
				{
					mc = MetaDataWrapper.CreateBridgeClass(
					txtClassName.Text.Trim(), txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim(),
					ddlClass1.SelectedValue, txtField1Name.Text.Trim(), txtField1FriendlyName.Text.Trim(),
					ddlClass2.SelectedValue, txtField2Name.Text.Trim(), txtField2FriendlyName.Text.Trim());

					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, mc.Name), true);
				}
				catch (MetaClassAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BridgeExistsErrorMessage").ToString(), "'" + txtClassName.Text.Trim() + "'");
					lbError.Visible = true;
				}
				catch (MetaFieldAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "FieldExistsErrorMessage").ToString(), "'" + txtField1Name.Text.Trim() + "' or '" + txtField2Name.Text.Trim() + "'");
					lbError.Visible = true;
				}
			}
			else
			{
				MetaDataWrapper.UpdateBridge(mc, txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim(),
				  txtField1FriendlyName.Text.Trim(), txtField2FriendlyName.Text.Trim());

				if (Back == "list")
					Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx", true);
				else
					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, mc.Name), true);
			}
		}
		#endregion
	}
}