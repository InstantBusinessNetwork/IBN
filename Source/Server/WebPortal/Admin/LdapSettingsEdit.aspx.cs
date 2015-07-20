using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.IBN.Business;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for LdapSettingsEdit.
	/// </summary>
	public partial class LdapSettingsEdit : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string sTitle = "";

		#region SetId
		private int SetId
		{
			get
			{
				if(Request["SetId"]!=null)
					return int.Parse(Request["SetId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dialog.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			sTitle = (SetId > 0) ? LocRM.GetString("tLDAPSettingsEdit") : LocRM.GetString("tLDAPSettingsAdd");
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				BindValues();
				dtcStart.Enabled = cbAutosync.Checked;
				txtInterval.Enabled = cbAutosync.Checked;
			}
			imbSave.Text = LocRM.GetString("Save");
			imbCancel.Text = LocRM.GetString("Cancel");
			imbSave.CustomImage = this.ResolveUrl("~/layouts/images/saveitem.gif");
			imbCancel.CustomImage = this.ResolveUrl("~/layouts/images/cancel.gif");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			cbActivate.Text = LocRM.GetString("tActivate");
			cbDeactivate.Text = LocRM.GetString("tDeactivate");
			cbAutosync.Text = LocRM.GetString("tAutosync");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			ddIBNKey.DataSource = UserInfo.PropertyNamesIbnAll;
			ddIBNKey.DataBind();
			
			ddLdap.DataSource = UserInfo.PropertyNamesAdAll;
			ddLdap.DataBind();
			ddLdap.Attributes.Add("onchange","ChangeLdap(this)");

			dtcStart.SelectedDate = Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow);
			Mediachase.IBN.Business.LdapSettings lsets = null;
			if(SetId>0)
			{
				lsets = Mediachase.IBN.Business.LdapSettings.Load(SetId);
				rfPass.Enabled = false;
			}
			else
			{
				lsets = new Mediachase.IBN.Business.LdapSettings();
				lsets.AutosyncStart = DateTime.UtcNow;
			}

			txtTitle.Text = lsets.Title;
			txtDomain.Text = lsets.Domain;
			txtUser.Text = lsets.Username;
			txtPass.Text = lsets.Password;
			ViewState["Pass"] = lsets.Password;
			txtFilter.Text = lsets.Filter;
			Util.CommonHelper.SafeSelect(ddIBNKey, lsets.IbnKey);
			Util.CommonHelper.SafeSelect(ddLdap, lsets.LdapKey);
			txtLdap.Text = lsets.LdapKey;
			txtInterval.Text = lsets.AutosyncInterval.ToString();
			cbActivate.Checked = lsets.Activate;
			cbDeactivate.Checked = lsets.Deactivate;
			cbAutosync.Checked = lsets.Autosync;
			dtcStart.SelectedDate = Mediachase.IBN.Business.User.GetLocalDate(lsets.AutosyncStart);
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.imbSave.ServerClick+=new EventHandler(imbSave_ServerClick);
			this.cbAutosync.CheckedChanged += new EventHandler(cbAutosync_CheckedChanged);
		}
		#endregion

		#region Save
		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if(!Page.IsValid)
				return;
				
			int newSetId = SetId;
			newSetId = Mediachase.IBN.Business.LdapSettings.CreateUpdate(newSetId, txtTitle.Text, txtDomain.Text, 
				txtUser.Text, (txtPass.Text!="")?txtPass.Text:ViewState["Pass"].ToString(), 
				txtFilter.Text, ddIBNKey.SelectedValue, txtLdap.Text, cbActivate.Checked,
				cbDeactivate.Checked, cbAutosync.Checked, 
				(dtcStart.SelectedDate.Year>1)?Mediachase.IBN.Business.User.GetUTCDate(dtcStart.SelectedDate):dtcStart.SelectedDate,
				(txtInterval.Text!="")?int.Parse(txtInterval.Text):0);
			
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"try {window.opener.location.href='LdapSettingsView.aspx?SetId="+newSetId+"';}" +
				"catch (e){} window.close();", true);
		}
		#endregion

		private void cbAutosync_CheckedChanged(object sender, EventArgs e)
		{
			dtcStart.Enabled = cbAutosync.Checked;
			txtInterval.Enabled = cbAutosync.Checked;
		}
	}
}
