namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Reflection;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for AlertUser.
	/// </summary>
	public partial class AlertUser : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));


		protected void Page_Load(object sender, System.EventArgs e)
		{
			secHeader.Title = LocRM.GetString("AlertUser");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tRoutingWorkflow"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4"));
			cbUseAS.Attributes.Add("onclick", "javascript:EnableDisable();");
			ApplyLocalization();
			if(!Page.IsPostBack)
				BindList();
			BindSavedValues();
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "EnableDisable();", true);
		}

		private void ApplyLocalization()
		{
			btnMove.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			cbUseAS.Text = LocRM.GetString("tUseAlerts");
			cbUseIMServer.Text = " " + LocRM.GetString("UseIMServer");
		}

		private void BindList()
		{
			ddSmtpBoxes.DataSource = SmtpBox.List();
			ddSmtpBoxes.DataTextField = "Name";
			ddSmtpBoxes.DataValueField = "SmtpBoxId";
			ddSmtpBoxes.DataBind();
			ddSmtpBoxes.Items.Insert(0, new ListItem(LocRM2.GetString("tDefaultValue"), "-1"));
		}

		private void BindSavedValues()
		{
			using (IDataReader rdr = Mediachase.IBN.Business.User.GetUserInfoByLogin("Alert"))
			{
				if (rdr.Read())
				{
					ViewState["UserId"] = rdr["UserId"];
					tbFirstname.Text = rdr["FirstName"].ToString();
					tbLastName.Text = rdr["LastName"].ToString();
					tbEmail.Text = rdr["Email"].ToString();
				}
			}
			SmtpBox box = OutgoingEmailServiceConfig.FindSmtpBox(OutgoingEmailServiceType.AlertService, false);
			if (box != null)
				CommonHelper.SafeSelect(ddSmtpBoxes, box.SmtpBoxId.ToString());
			cbUseAS.Checked = PortalConfig.EnableAlerts;
			txtSubjectTemp.Text = PortalConfig.AlertSubjectFormat;
			cbUseIMServer.Checked = PortalConfig.UseIM;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("AlertUser");
		}
		#endregion

		protected void btn_Save(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			int UserId = (int)ViewState["UserId"];
			try
			{
				// TODO: зачем сохранять два раза?
				Company.UpdateAlertInfo(cbUseAS.Checked, tbFirstname.Text, tbLastName.Text, tbEmail.Text);
				Company.UpdateAlertNotificationInfo(UserId, tbFirstname.Text, tbLastName.Text, tbEmail.Text);

				if (int.Parse(ddSmtpBoxes.SelectedValue) > 0)
					OutgoingEmailServiceConfig.AssignWithSmtpBox(OutgoingEmailServiceType.AlertService, int.Parse(ddSmtpBoxes.SelectedValue));
				else
					OutgoingEmailServiceConfig.AssignWithDefaultSmtpBox(OutgoingEmailServiceType.AlertService, null);

				PortalConfig.AlertSubjectFormat = txtSubjectTemp.Text;
				PortalConfig.UseIM = cbUseIMServer.Checked;

				Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4");
			}
			catch (EmailDuplicationException)
			{
				int iUserId = User.GetUserByEmail(tbEmail.Text);
				string sUserName = String.Empty;

				if (iUserId > 0)
					sUserName = CommonHelper.GetUserStatusPureName(iUserId);

				cv1.IsValid = false;
				cv1.ErrorMessage = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";
			}
		}

		protected void btn_Cancel(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4");
		}
	}
}
