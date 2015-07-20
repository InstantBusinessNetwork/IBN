namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using System.Reflection;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for SearchSettings.
	/// </summary>
	public partial class SearchSettings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnSave.Click += new EventHandler(btnSave_Click);
			this.lbRunFTS.Click += new EventHandler(lbRunFTS_Click);
			this.lbEnableWD.Click += new EventHandler(lbEnableWD_Click);
			this.lbDisableFTS.Click += new EventHandler(lbDisableFTS_Click);
			this.lbDisableWD.Click += new EventHandler(lbDisableWD_Click);

			if (!this.IsPostBack)
			{
				BindList();
				BindSettings();
			}
			BindToolbal2();
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (trWDEnabled.Visible)
			{
				bhlWebDAV.AddRightLink(LocRM2.GetString("tWebDavSettings"), ResolveUrl("~/Admin/WebDAVSettings.aspx"));
			}
			base.OnPreRender(e);
		}

		#region BindList
		private void BindList()
		{
			ddSmtpBoxes.DataSource = SmtpBox.List();
			ddSmtpBoxes.DataTextField = "Name";
			ddSmtpBoxes.DataValueField = "SmtpBoxId";
			ddSmtpBoxes.DataBind();
			ddSmtpBoxes.Items.Insert(0, new ListItem(LocRM2.GetString("tDefaultValue"), "-1"));
		} 
		#endregion

		#region BindToolbar2
		private void BindToolbal2()
		{
			secHeader2.Title = LocRM.GetString("tAdditionalSettings");
			secHeader2.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tFilesForms"), ResolveClientUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin6"));
			bhlFTS.AddText(LocRM.GetString("tFTS"));
			bhlWebDAV.AddText(LocRM.GetString("tWebDAV"));
			lbEnableWD.Text = LocRM.GetString("tEnableWebDAV");
			lbRunFTS.Text = LocRM.GetString("tRunFTS");
			lbDisableFTS.Text = LocRM.GetString("tFTSDisable");
			lbDisableWD.Text = LocRM.GetString("tDisableWebDAV");

			btnSave.Text = LocRM.GetString("Save");
			bhSendFile.AddText(LocRM2.GetString("tTitleFileSMTP"));
		}
		#endregion

		#region BindSettings
		protected void BindSettings()
		{
			if (PortalConfig.UseFullTextSearch.HasValue)
			{
				trFTSNotInstalled.Visible = false;
				trFTSNotEnabled.Visible = false;
				trFTSEnabled.Visible = true;
				if ((bool)PortalConfig.UseFullTextSearch)
				{
					ftsStat.Visible = true;
					try
					{
						FullTextSearchInfo info = FullTextSearch.GetInformation();
						lbIndSize.Text = Util.CommonHelper.ByteSizeToStr(info.IndexSize * 1024 * 1024);
						lbStat.Text = info.PopulateStatus.ToString();
						ftsInfoOK.Visible = true;
						ftsInfoFailed.Visible = false;
					}
					catch (Exception ex)
					{
						ftsInfoOK.Visible = false;
						ftsInfoFailed.Visible = true;
						lbFTSErrorMessage.Text = ex.Message;
					}
				}
				else
				{
					trFTSEnabled.Visible = false;
					ftsStat.Visible = false;
					trFTSNotEnabled.Visible = true;
				}
			}
			else
			{
				if (!FullTextSearch.IsInstalled())
				{
					trFTSNotInstalled.Visible = true;
					trFTSNotEnabled.Visible = false;
					trFTSEnabled.Visible = false;
				}
				if (FullTextSearch.IsInstalled())
				{
					trFTSNotInstalled.Visible = false;
					trFTSNotEnabled.Visible = true;
					trFTSEnabled.Visible = false;
				}
			}

			if (PortalConfig.UseWebDav.HasValue)
			{
				if ((bool)PortalConfig.UseWebDav)
				{
					trWDDisabled.Visible = false;
					trWDEnabled.Visible = true;
				}
				else
				{
					trWDDisabled.Visible = true;
					trWDEnabled.Visible = false;
				}
			}
			else
			{
				trWDDisabled.Visible = true;
				trWDEnabled.Visible = false;
			}

			SmtpBox box = OutgoingEmailServiceConfig.FindSmtpBox(OutgoingEmailServiceType.SendFile, false);
			if (box != null)
				CommonHelper.SafeSelect(ddSmtpBoxes, box.SmtpBoxId.ToString());
		}
		#endregion

		#region Events
		void lbDisableWD_Click(object sender, EventArgs e)
		{
			PortalConfig.UseWebDav = false;
			Response.Redirect("~/Admin/SearchSettings.aspx", true);
		}

		void lbDisableFTS_Click(object sender, EventArgs e)
		{
			PortalConfig.UseFullTextSearch = false;
			if (FullTextSearch.IsInstalled() && FullTextSearch.IsActive())
				FullTextSearch.Deactivate();
			Response.Redirect("~/Admin/SearchSettings.aspx", true);
		}

		void lbEnableWD_Click(object sender, EventArgs e)
		{
			PortalConfig.UseWebDav = true;
			Response.Redirect("~/Admin/SearchSettings.aspx", true);
		}

		void lbRunFTS_Click(object sender, EventArgs e)
		{
			if (FullTextSearch.IsInstalled() && !FullTextSearch.IsActive())
				FullTextSearch.Activate();
			Response.Redirect("~/Admin/SearchSettings.aspx", true);
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			if (int.Parse(ddSmtpBoxes.SelectedValue) > 0)
				OutgoingEmailServiceConfig.AssignWithSmtpBox(OutgoingEmailServiceType.SendFile, int.Parse(ddSmtpBoxes.SelectedValue));
			else
				OutgoingEmailServiceConfig.AssignWithDefaultSmtpBox(OutgoingEmailServiceType.SendFile, null);
			Response.Redirect("~/Admin/SearchSettings.aspx", true);
		} 
		#endregion

	}
}
