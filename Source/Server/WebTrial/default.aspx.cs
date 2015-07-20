using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Resources;
using System.Threading;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class defaultpage : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebTrial.App_GlobalResources.Resources.Resources", typeof(defaultpage).Assembly);

		protected const string sHeaderFileName = "trial_header";

		private string ResellerGuid
		{
			get
			{
				//unknown Reseller GUID (see web.config)
				string retval = String.Empty;//Settings.UnknownResellerGuid;

				if (Request["Reseller"] != null && Request["Reseller"] != String.Empty)
				{
					try
					{
						retval = new Guid(Request["Reseller"]).ToString();
					}
					catch (FormatException)
					{
					}
				}

				//DV 2008-05-07
				if (retval == string.Empty)
				{
					switch (Thread.CurrentThread.CurrentUICulture.Name)
					{
						case "ru-RU":
							{
								retval = new Guid(Settings.RuResellerGuid).ToString();
								break;
							}
						case "en-US":
							{
								retval = new Guid(Settings.EnResellerGuid).ToString();
								break;
							}
						default:
							{
								retval = new Guid(Settings.UnknownResellerGuid).ToString();
								break;
							}
					}
				}

				return retval;
			}
		}

		private string CancelLink
		{
			get
			{
				return Server.UrlDecode(Request["Back"]);
			}
		}

		private string locale
		{
			get
			{
				return Request["locale"];
			}
		}

		#region Page_Load()
		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCreate.Visible = true;
			spanLanguage.Visible = true;
			string asppath = Settings.AspPath;
			lblDomain.Text = "." + TrialHelper.GetParentDomain();
			if (asppath != null)
				imgHeader.ImageUrl = asppath + "images/SignupHeader.aspx";
			lbRepeat.Visible = false;
			if (!IsPostBack)
			{
				if (locale != null && locale != String.Empty)
				{
					Thread.CurrentThread.CurrentCulture = new CultureInfo(locale);
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
				}
				else
					SetUserLocale();

				if (CancelLink == null || CancelLink == String.Empty) btnCancel.Visible = false;
				BindDefaultValues();
				tr4.Visible = false;
			}
			else if (ddLanguage.SelectedItem != null)
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(ddLanguage.SelectedItem.Value);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(ddLanguage.SelectedItem.Value);
			}

			cbConfirm.Attributes.Add("onclick", "EnableButtons(this);");

			btnCreate.Value = LocRM.GetString("strCreateMyPortal");
			btnCreate.Disabled = !cbConfirm.Checked;
			btnCreate.Attributes.Add("onclick", "CreateTrial();");

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			lbCreate.Attributes.Add("onclick", "DisableButtons(this);");
			lbRepeat.Attributes.Add("onclick", "DisableButtons(this);");
			cbConfirm.Text = LocRM.GetString("IAgree");
			lblTerms.Text = "<a href='javascript:OpenTerms()'>" + LocRM.GetString("IAgree2") + "</a>";
			cbSendMe.Text = LocRM.GetString("strSendMe");
			cbCallMe.Text = LocRM.GetString("strCallMe");
		}
		#endregion

		#region BindDefaultValues()
		private void BindDefaultValues()
		{
			string BrowserLocale = Thread.CurrentThread.CurrentCulture.Name;

			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

			ListItem li = ddLanguage.Items.FindByValue(BrowserLocale);
			if (li != null)
				li.Selected = true;
			else
			{
				ListItem li1 = ddLanguage.Items.FindByValue("en-US");
				if (li1 != null)
					li1.Selected = true;
			}

			ddTimeZone.DataValueField = "TimeZoneId";
			ddTimeZone.DataTextField = "DisplayName";
			DataView dv = TrialHelper.GetTimeZones(BrowserLocale).DefaultView;
			dv.Sort = "Bias DESC, DisplayName";
			ddTimeZone.DataSource = dv;
			ddTimeZone.DataBind();

			btnCancel.Value = LocRM.GetString("Cancel");
			lbRepeat.Text = LocRM.GetString("Repeat");
			ListItem liItem = null;
			switch (BrowserLocale.ToLower())
			{
				case "ru-ru":
					if (ddCountry.SelectedIndex > -1)
						ddCountry.Items[ddCountry.SelectedIndex].Selected = false;
					liItem = ddCountry.Items.FindByValue("46");
					if (liItem != null)
						liItem.Selected = true;

					if (ddTimeZone.SelectedIndex > -1)
						ddTimeZone.Items[ddTimeZone.SelectedIndex].Selected = false;
					liItem = ddTimeZone.Items.FindByValue("54");
					if (liItem != null)
						liItem.Selected = true;

					break;
				default:
					if (ddCountry.SelectedIndex > -1)
						ddCountry.Items[ddCountry.SelectedIndex].Selected = false;
					liItem = ddCountry.Items.FindByValue("57");
					if (liItem != null)
						liItem.Selected = true;

					if (ddTimeZone.SelectedIndex > -1)
						ddTimeZone.Items[ddTimeZone.SelectedIndex].Selected = false;
					liItem = ddTimeZone.Items.FindByValue("19");
					if (liItem != null)
						liItem.Selected = true;

					break;
			}
		}
		#endregion

		protected void Button2_ServerClick(object sender, System.EventArgs e)
		{
			btnCreate.Visible = true;
			lbRepeat.Visible = false;
			tr1.Visible = true;
			tr2.Visible = true;
			tr3.Visible = true;
			tr4.Visible = false;
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect(CancelLink);
		}

		protected void ddLanguage_change(object sender, System.EventArgs e)
		{
			BindDefaultValues();
		}

		#region SetUserLocale()
		public void SetUserLocale()
		{
			HttpRequest Request = HttpContext.Current.Request;
			if (Request.UserLanguages == null)
				return;
			string Lang = Request.UserLanguages[0];
			if (Lang != null)
			{
				if (Lang.Length < 3)
					Lang = Lang + "-" + Lang.ToUpper();
				try
				{
					System.Threading.Thread.CurrentThread.CurrentCulture =
						new System.Globalization.CultureInfo(Lang);
					System.Threading.Thread.CurrentThread.CurrentUICulture =
						new System.Globalization.CultureInfo(Lang);
				}
				catch { }
			}
		}
		#endregion

		#region lbCreate_Click
		protected void lbCreate_Click(object sender, EventArgs e)
		{
			//Page.Validate();
			if (!Page.IsValid || !cbConfirm.Checked)
				return;

			localhost.TrialResult tr = localhost.TrialResult.Failed;
			string message = String.Empty;

			string assist = LocRM.GetString("Assist");
			int requestId;
			string requestGuid;
			try
			{
				tr = TrialHelper.Request(
					tbPortalName.Text
					, tbDomain.Text
					, tbFirstName.Text
					, tbLatName.Text
					, tbEmail.Text
					, tbPhone.Text
					, ddCountry.Items[ddCountry.SelectedIndex].Text
					, tbLogin.Text
					, tbPassword.Text
					, ResellerGuid
					, Thread.CurrentThread.CurrentUICulture.Name
					, int.Parse(ddTimeZone.SelectedValue)
					, CManage.GetReferrer(Request)
					, out requestId
					, out requestGuid
					/*
					 , cbSendMe.Checked
					 , cbCallMe.Checked
					 */
				);

				switch (tr)
				{
					case localhost.TrialResult.AlreadyActivated:
						lblStep4Header.Text = LocRM.GetString("TrialRejected");
						lblStep4SubHeader.Text = LocRM.GetString("TrialRejectedReasons");
						message = LocRM.GetString("TrialActivated");
						lblStep4Text.Text = LocRM.GetString("TrialActivated") + assist;
						btnCreate.Visible = false;
						spanLanguage.Visible = false;
						break;
					case localhost.TrialResult.DomainExists:
						lblStep4Header.Text = LocRM.GetString("TrialRejected");
						lblStep4SubHeader.Text = LocRM.GetString("TrialRejectedReasons");
						message = LocRM.GetString("DomainExists");
						lblStep4Text.Text = LocRM.GetString("DomainExists") + assist;
						lbRepeat.Visible = true;
						btnCreate.Visible = false;
						spanLanguage.Visible = false;
						break;
					case localhost.TrialResult.Failed:
						lblStep4Header.Text = LocRM.GetString("TrialRejected");
						lblStep4SubHeader.Text = LocRM.GetString("TrialRejectedReasons");
						message = LocRM.GetString("UnknownReason");
						lblStep4Text.Text = LocRM.GetString("UnknownReason") + assist;
						break;
					case localhost.TrialResult.InvalidRequest:
						lblStep4Header.Text = LocRM.GetString("TrialRejected");
						lblStep4SubHeader.Text = LocRM.GetString("TrialRejectedReasons");
						message = LocRM.GetString("InvalidRequest");
						lblStep4Text.Text = LocRM.GetString("InvalidRequest") + assist;
						break;
					case localhost.TrialResult.RequestPending:
						lblStep4Header.Text = LocRM.GetString("RequestPending");
						lblStep4SubHeader.Text = LocRM.GetString("OneStep");
						message = String.Format("Request Pending.", tbDomain.Text);
						lblStep4Text.Text = String.Format("Request Pending.", tbDomain.Text);
						break;
					case localhost.TrialResult.Success:
						lblStep4Header.Text = LocRM.GetString("Created");
						lblStep4SubHeader.Text = LocRM.GetString("OneStep");
						message = String.Format(LocRM.GetString("Congratulations"), tbDomain.Text, lblDomain.Text);
						lblStep4Text.Text = String.Format(LocRM.GetString("Congratulations"), tbDomain.Text, lblDomain.Text);
						btnCreate.Visible = false;
						spanLanguage.Visible = false;
						break;
					case localhost.TrialResult.UserRegistered:
						lblStep4Header.Text = LocRM.GetString("TrialRejected");
						lblStep4SubHeader.Text = LocRM.GetString("TrialRejectedReasons");
						message = LocRM.GetString("UserRegistered");
						lblStep4Text.Text = LocRM.GetString("UserRegistered") + assist;
						lbRepeat.Visible = true;
						btnCreate.Visible = false;
						spanLanguage.Visible = false;
						break;
					case localhost.TrialResult.WaitingForActivation:
						Response.Redirect(String.Format("activate.aspx?rid={0}&guid={1}&locale={2}", requestId, requestGuid, Thread.CurrentThread.CurrentUICulture.Name), true);
						return;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				lblStep4Header.Text = LocRM.GetString("Failed");
				lblStep4SubHeader.Text = LocRM.GetString("TrialRejectedReasons");
				lblStep4Text.Text = ex.Message + @"<br>" + assist;
				message = ex.Message;
				cbConfirm.Checked = false;
				lbRepeat.Visible = true;
				btnCreate.Visible = false;
			}
			finally
			{
				try
				{
					string conStr = Settings.ConnectionString;
					if (conStr != null && conStr.Length > 0) //Save request in local database.
						CManage.CreateRequest(
							tbPortalName.Text
							, "1 - 20"
							, ""
							, tbDomain.Text
							, tbFirstName.Text
							, tbLatName.Text
							, tbEmail.Text
							, ddCountry.Items[ddCountry.SelectedIndex].Text
							, tbPhone.Text
							, ddTimeZone.Items[ddTimeZone.SelectedIndex].Text
							, tbLogin.Text
							, tbPassword.Text
							, new Guid(ResellerGuid)
							, (int)tr
							, message
						);
				}
				catch
				{
#if DEBUG
					//throw;
#endif
				}
			}
			tr1.Visible = false;
			tr2.Visible = false;
			tr3.Visible = false;
			tr4.Visible = true;
		}
		#endregion

		private void Page_PreRender(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>setTimeout(\"FocusElement('" + tbFirstName.ClientID + "')\", 0);</script>");

		}
	}
}
