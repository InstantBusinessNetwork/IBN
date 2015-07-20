namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for PrefsEdit.
	/// </summary>
	public partial class PrefsEdit2 : System.Web.UI.UserControl
	{
		public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strPrefsEdit2", typeof(PrefsEdit2).Assembly);

		#region UserID
		private int UserID
		{
			get
			{
				int ret = 0;
				try
				{
					string s = Request["UserID"];
					if (s != null)
						ret = int.Parse(s);
				}
				catch { }
				if (ret == 0)
					ret = Security.CurrentUser.UserID;
				return ret;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblTimeZoneTitle.Text = LocRM.GetString("TimeZone");
			lblLangTitle.Text = LocRM.GetString("Lang");
			cbMenuInAlerts.Text = "&nbsp;" + LocRM.GetString("tMenuInAlerts");

			cbSystemEvents.Text = LocRM.GetString("EnableNotify");
			rbInstantly.Text = LocRM.GetString("Instantly");
			rbByBatch.Text = LocRM.GetString("ByBatch");
			cbReminder.Text = LocRM.GetString("EnableRemindNotify");
			cbEmail.Text = LocRM.GetString("Email");
			cbIBN.Text = IbnConst.ProductFamilyShort;
			cbRemEmail.Text = LocRM.GetString("Email");
			cbRemIBN.Text = IbnConst.ProductFamilyShort;
			lblEvery.Text = LocRM.GetString("Every");
			lblFrom.Text = LocRM.GetString("From");
			lblTo.Text = LocRM.GetString("To");

			ddEvery.Items.Add(new ListItem(LocRM.GetString("30m"), "30"));
			ddEvery.Items.Add(new ListItem(LocRM.GetString("60m"), "60"));
			ddEvery.Items.Add(new ListItem(LocRM.GetString("240m"), "240"));

			for (int i = 0; i <= 23; i++)
			{
				DateTime dt = new DateTime(1, 1, 1, i, 0, 0);
				ddFrom.Items.Add(new ListItem(dt.ToShortTimeString(), i.ToString()));
				ddTo.Items.Add(new ListItem(dt.ToShortTimeString(), i.ToString()));
			}
		}
		#endregion

		#region LoadPrefs
		public void LoadPrefs()
		{
			ApplyLocalization();

			lstLang.DataSource = Common.GetListLanguages();
			lstLang.DataTextField = "FriendlyName";
			lstLang.DataValueField = "LanguageId";
			lstLang.DataBind();

			lstTimeZone.DataSource = User.GetListTimeZone();
			lstTimeZone.DataTextField = "DisplayName";
			lstTimeZone.DataValueField = "TimeZoneId";
			lstTimeZone.DataBind();
			using (IDataReader rdr = User.GetUserInfo(UserID))
			{
				if (rdr.Read())
				{
					if ((bool)rdr["IsExternal"])
					{
						cbIBN.Visible = false;
						cbRemIBN.Visible = false;
					}
				}
			}

			using (IDataReader rdr = User.GetUserPreferencesTO(Security.CurrentUser.TimeZoneId, UserID))
			{
				while (rdr.Read())
				{
					ListItem lItemZone = lstTimeZone.Items.FindByValue(rdr["TimeZoneId"].ToString());
					if (lItemZone != null)
						lItemZone.Selected = true;

					ListItem lItemLang = lstLang.Items.FindByValue(rdr["LanguageId"].ToString());
					if (lItemLang != null)
						lItemLang.Selected = true;

					oldLang.Value = rdr["LanguageId"].ToString();
					oldTime.Value = rdr["TimeZoneId"].ToString();

					int iRemindType = (int)rdr["ReminderType"];
					if (iRemindType > 0)
					{
						cbReminder.Checked = true;
						cbRemEmail.Checked = true;
						cbRemIBN.Checked = true;
						if (iRemindType == 1)
							cbRemIBN.Checked = false;
						if (iRemindType == 2)
							cbRemEmail.Checked = false;
					}
					else
					{
						cbReminder.Checked = false;
					}

					cbSystemEvents.Checked = (bool)rdr["IsNotified"];
					cbEmail.Checked = (bool)rdr["IsNotifiedByEmail"];
					cbIBN.Checked = (bool)rdr["IsNotifiedByIBN"];

					if ((bool)rdr["IsBatchNotifications"])
					{
						rbInstantly.Checked = false;
						rbByBatch.Checked = true;
						hdnBatch.Value = "1";
					}
					else
					{
						rbInstantly.Checked = true;
						rbByBatch.Checked = false;
						hdnBatch.Value = "0";
					}

					ListItem li = ddEvery.Items.FindByValue(((int)rdr["Period"]).ToString());
					if (li != null)
						li.Selected = true;

					li = ddFrom.Items.FindByValue(((int)rdr["From"]).ToString());
					if (li != null)
						li.Selected = true;

					li = ddTo.Items.FindByValue(((int)rdr["Till"]).ToString());
					if (li != null)
						li.Selected = true;
				}
			}

			if (!PortalConfig.EnableAlerts)
			{
				cbReminder.Enabled = false;
				cbRemEmail.Enabled = false;
				cbRemIBN.Enabled = false;

				cbSystemEvents.Enabled = false;
				cbEmail.Enabled = false;
				cbIBN.Enabled = false;
				rbInstantly.Enabled = false;
				rbByBatch.Enabled = false;
				ddEvery.Enabled = false;
				ddFrom.Enabled = false;
				ddTo.Enabled = false;
			}
			else if (!PortalConfig.UseIM)
			{
				cbRemIBN.Enabled = false;
				cbIBN.Enabled = false;
			}

			cbMenuInAlerts.Checked = User.GetMenuInAlerts(UserID);

			if (PortalConfig.EnableAlerts)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "EnableDisable();", true);
		}
		#endregion

		#region SavePrefs
		public bool SavePrefs()
		{
			try
			{
				int Period = int.Parse(ddEvery.SelectedValue);
				int From = int.Parse(ddFrom.SelectedValue);
				int To = int.Parse(ddTo.SelectedValue);
				int iRemindId = 0;
				if (cbReminder.Checked)
				{
					if (cbRemEmail.Checked && cbRemIBN.Checked)
						iRemindId = 3;
					else if (cbRemEmail.Checked)
						iRemindId = 1;
					else if (cbRemIBN.Checked)
						iRemindId = 2;
				}
				bool bBatch = false;
				if (hdnBatch.Value == "1")
					bBatch = true;
				User.UpdatePreferences(UserID, cbSystemEvents.Checked, cbEmail.Checked, cbIBN.Checked,
					bBatch, Period, From, To, int.Parse(lstTimeZone.SelectedValue),
					int.Parse(lstLang.SelectedValue), cbMenuInAlerts.Checked, iRemindId);
			}
			catch (Exception ex)
			{
				string s = ex.ToString();
			}
			if (oldLang.Value == lstLang.SelectedValue && oldTime.Value == lstTimeZone.SelectedValue)
				return false;
			else
				return true;
		}
		#endregion
	}
}
