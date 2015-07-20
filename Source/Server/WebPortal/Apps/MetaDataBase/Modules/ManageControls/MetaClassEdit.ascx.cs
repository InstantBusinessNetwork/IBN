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

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaClassEdit : System.Web.UI.UserControl
	{
		private MetaClass _mc = null;

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
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);

			LoadRequestVariables();

			//if (_mc != null)
			//{
			//    CHelper.AddToContext(NavigationBlock.KeyContextMenu, "MetaClassView");
			//    CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CHelper.GetResFileString(_mc.FriendlyName));
			//}

			BindInfo();

			if (!IsPostBack)
				BindData();
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				_mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}

			if (Request.QueryString["back"] != null)
				Back = Request.QueryString["back"].ToLower();
		}
		#endregion

		#region BindInfo
		private void BindInfo()
		{
			if (_mc != null)
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EditTable").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CreateTable").ToString();

			if (Back == "view" && _mc != null)
			{
				secHeader.AddLink(
				  String.Format(CultureInfo.InvariantCulture, "<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToTableInfo").ToString()),
				  String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, _mc.Name));

				btnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}';return false;", _mc.Name));
			}
			else  // Back to List by default
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToList").ToString()),
				  "~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx");

				btnCancel.Attributes.Add("onclick", "window.location.href='MetaClassList.aspx'; return false;");
			}

			btnSave.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/saveitem.gif");
			btnCancel.CustomImage = CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif");

			lgdClass.InnerText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "TableInfo").ToString(); ;
			lgdField.InnerText = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "FieldInfo").ToString(); ;
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (_mc == null)
			{
				ddlOwnerType.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Private").ToString(), OwnerTypes.Private.ToString()));
				ddlOwnerType.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Public").ToString(), OwnerTypes.Public.ToString()));

				txtFieldName.Text = "Title";
				txtFieldFriendlyName.Text = "Title";
				txtMaxLen.Text = "100";

				txtClassName.Attributes.Add("onblur", "SetName('" + txtClassName.ClientID + "','" + txtClassFriendlyName.ClientID + "','" + vldClassFriendlyName_Required.ClientID + "')" + "; SetName('" + txtClassName.ClientID + "','" + txtClassPluralName.ClientID + "','" + vldClassPluralName_Required.ClientID + "')");
				txtFieldName.Attributes.Add("onblur", "SetName('" + txtFieldName.ClientID + "','" + txtFieldFriendlyName.ClientID + "','" + vldFieldFriendlyName_Required.ClientID + "')");
			}
			else
			{
				// Class
				txtClassName.Text = _mc.Name;
				txtClassName.Enabled = false;

				txtClassFriendlyName.Text = _mc.FriendlyName;
				txtClassPluralName.Text = _mc.PluralName;

				string ownerType = _mc.Attributes.GetValue<OwnerTypes>(MetaDataWrapper.OwnerTypeAttr, OwnerTypes.Undefined).ToString();
				ddlOwnerType.Items.Add(new ListItem((string)GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", ownerType), ownerType));
				ddlOwnerType.Enabled = false;

				chkSupportsCards.Checked = _mc.SupportsCards;
				chkSupportsCards.Enabled = false;

				// Field
				MetaField mf = MetaDataWrapper.GetTitleField(_mc);

				txtFieldName.Text = mf.Name;
				txtFieldName.Enabled = false;

				txtFieldFriendlyName.Text = mf.FriendlyName;

				txtMaxLen.Text = string.Format(CultureInfo.CurrentUICulture, "{0}", mf.Attributes[McDataTypeAttribute.StringMaxLength]);
				txtMaxLen.Enabled = false;
			}
		}
		#endregion

		#region btnSave_ServerClick
		void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (_mc == null) // Create
			{

				OwnerTypes ownerType = (OwnerTypes)Enum.Parse(typeof(OwnerTypes), ddlOwnerType.SelectedValue);
				try
				{
					_mc = MetaDataWrapper.CreateMetaClass(txtClassName.Text.Trim(), txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim(), ownerType, chkSupportsCards.Checked, txtFieldName.Text.Trim(), txtFieldFriendlyName.Text.Trim(), int.Parse(txtMaxLen.Text));
					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, _mc.Name), true);
				}
				catch (MetaClassAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "TableExistsErrorMessage").ToString(), "'" + txtClassName.Text.Trim() + "'");
					lbError.Visible = true;
				}
				catch (MetaFieldAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "FieldExistsErrorMessage").ToString(), "'" + txtFieldName.Text.Trim() + "'");
					lbError.Visible = true;
				}
			}
			else // Update
			{
				MetaDataWrapper.UpdateMetaClass(_mc, txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim(), txtFieldFriendlyName.Text.Trim());

				if (Back == "list")
					Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx", true);
				else
					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, _mc.Name), true);
			}
		}
		#endregion
	}
}