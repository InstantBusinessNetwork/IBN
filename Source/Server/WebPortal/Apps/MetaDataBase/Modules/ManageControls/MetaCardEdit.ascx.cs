using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaCardEdit : System.Web.UI.UserControl
	{
		MetaClass mc = null;

		#region OwnerClassName
		private string _ownerClassName = String.Empty;
		public string OwnerClassName
		{
			get { return _ownerClassName; }
			set { _ownerClassName = value; }
		}
		#endregion

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

			if (Request.QueryString["owner"] != null)
			{
				OwnerClassName = Request.QueryString["owner"];
			}

			if (Request.QueryString["back"] != null)
				Back = Request.QueryString["back"].ToLower();
		}
		#endregion

		#region BindInfo
		private void BindInfo()
		{
			if (mc != null)
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CardEdit").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CardCreate").ToString();

			if (Back == "view" && mc != null)
			{
				secHeader.AddLink(
				  String.Format(CultureInfo.InvariantCulture, "<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BackToTableInfo").ToString()),
				  String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, mc.Name));

				imbtnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}'; return false;", mc.Name));
			}
			else if (Back == "owner" && OwnerClassName != String.Empty)
			{
				secHeader.AddLink(
				  String.Format(CultureInfo.InvariantCulture, "<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", CHelper.GetAbsolutePath("/images/IbnFramework/cancel.gif"), GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Back").ToString()),
				  String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, OwnerClassName));

				imbtnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}'; return false;", OwnerClassName));
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
		}
		#endregion

		#region BindData
		private void BindData()
		{
			if (mc == null)
			{
				if (OwnerClassName != String.Empty)
				{
					MetaClass ownerClass = MetaDataWrapper.GetMetaClassByName(OwnerClassName);
					ddlClass.Items.Add(new ListItem(CHelper.GetResFileString(ownerClass.FriendlyName), ownerClass.Name));
					ddlClass.Enabled = false;
				}
				else
				{
					foreach (MetaClass cls in MetaDataWrapper.GetMetaClassesSupportedCards())
					{
						ddlClass.Items.Add(new ListItem(cls.Name, cls.Name));
					}
				}

				txtClassName.Attributes.Add("onblur", "SetName('" + txtClassName.ClientID + "','" + txtClassFriendlyName.ClientID + "','" + vldClassFriendlyName_Required.ClientID + "')" + "; SetName('" + txtClassName.ClientID + "','" + txtClassPluralName.ClientID + "','" + vldClassPluralName_Required.ClientID + "')");
			}
			else
			{
				MetaClass ownerClass = MetaDataWrapper.GetOwnerClass(mc);
				ddlClass.Items.Add(new ListItem(ownerClass.Name, ownerClass.Name));
				ddlClass.Enabled = false;

				txtClassName.Text = mc.Name;
				txtClassName.Enabled = false;

				txtClassFriendlyName.Text = mc.FriendlyName;
				txtClassPluralName.Text = mc.PluralName;
			}
		}
		#endregion

		#region imbtnSave_ServerClick
		void imbtnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (mc == null) // Create
			{
				try
				{
					mc = MetaDataWrapper.CreateCard(ddlClass.SelectedValue,
					txtClassName.Text.Trim(), txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim());

					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, mc.Name), true);
				}
				catch (MetaClassAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CardExistsErrorMessage").ToString(), "'" + txtClassName.Text.Trim() + "'");
					lbError.Visible = true;
				}
			}
			else  // Update
			{
				MetaDataWrapper.UpdateCard(mc, txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim());

				if (Back == "list")
					Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx", true);
				else
					Response.Redirect(String.Format(CultureInfo.InvariantCulture, "{0}?class={1}", CHelper.MetaClassAdminPage, mc.Name), true);
			}
		}
		#endregion
	}
}