namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for UserView.
	/// </summary>
	public partial class UserView : System.Web.UI.UserControl
	{
		#region HTML Vars


		protected System.Web.UI.HtmlControls.HtmlTable tblEMails;

		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserView", typeof(UserView).Assembly);

		#region UserID
		private int userID = -1;
		private int UserID
		{
			get
			{
				if (userID < 0)
				{
					try
					{
						if (Request["UserID"] != null)
							userID = int.Parse(Request["UserID"]);
						else
						{
							if (Request["AccountID"] != null)
							{
								int iID = 0;
								using (IDataReader reader = User.GetUserInfoByOriginalId(int.Parse(Request["AccountID"])))
								{
									if (reader.Read())
										iID = (int)reader["UserId"];
								}
								if (iID > 0)
									userID = iID;
								else
									userID = Security.CurrentUser.UserID;
							}
							else
								userID = Security.CurrentUser.UserID;
						}
					}
					catch
					{
						throw new Exception("Invalid User ID");
					}
				}
				return userID;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblEmailDuplication.Visible = false;
			if (!IsPostBack)
			{
				ApplyLocalization();
				BindData();
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbars();
		}

		#region BindData()
		private void BindData()
		{
			bool isExternal = false;
			using (IDataReader rdr = User.GetUserInfo(UserID))
			{
				if (rdr.Read())
					isExternal = (bool)rdr["IsExternal"];
			}

			DataView dv = new DataView(User.GetListEmailsDataTable(UserID));
			dv.Sort = "EMail";
			dgEmails.DataSource = dv;
			dgEmails.DataBind();

			foreach (DataGridItem dgi in dgEmails.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");

				RequiredFieldValidator rf = (RequiredFieldValidator)dgi.FindControl("rfEmail");
				if (rf != null)
					rf.ErrorMessage = "*";

				RegularExpressionValidator reV = (RegularExpressionValidator)dgi.FindControl("reEmail");
				if (reV != null)
					reV.ErrorMessage = LocRM.GetString("WrongEmail");
			}
			if (!User.CanUpdateUserInfo(UserID))
				dgEmails.Columns[2].Visible = false;
			//if(dgEmails.Items.Count==0)
			//dgEmails.Visible = false;


			if (User.CanReadPreferences(UserID))
			{
				using (IDataReader rdr = User.GetUserPreferencesTO(Security.CurrentUser.TimeZoneId, UserID))
				{
					lblNotifyDelay.Text = "";
					lblNotifyBy.Text = "";
					while (rdr.Read())
					{
						/// UserId, IsNotified, IsNotifiedByEmail, IsNotifiedByIBN, IsBatchNotifications, 
						/// Period, From, Till, TimeZoneId, TimeOffsetLatest, LanguageId, Locale, 
						/// LanguageName, ReminderType, BatchLastSent, BatchNextSend


						lblTimeZone.Text = User.GetTimeZoneName((int)rdr["TimeZoneId"]);
						lblLang.Text = rdr["LanguageName"].ToString();

						if ((bool)rdr["IsNotified"])
						{
							lblNotifyEnabled.Text = LocRM.GetString("NotifyEnabled");

							if ((bool)rdr["IsBatchNotifications"])
							{
								lblNotifyDelay.Text = LocRM.GetString("NotifyTypeBatch") + ": ";
								String ps = "";
								switch ((int)rdr["Period"])
								{
									case 30:
										ps = LocRM.GetString("30m");
										break;
									case 60:
										ps = LocRM.GetString("60m");
										break;
									case 240:
										ps = LocRM.GetString("240m");
										break;
								}
								lblNotifyDelay.Text += ps
									+ ", "
									+ LocRM.GetString("NotifyFrom") + " ";

								DateTime From = new DateTime(1, 1, 1, (int)rdr["From"], 0, 0);
								DateTime To = new DateTime(1, 1, 1, (int)rdr["Till"], 0, 0);
								lblNotifyDelay.Text += From.ToShortTimeString() + " " + LocRM.GetString("NotifyTo") + " ";
								lblNotifyDelay.Text += To.ToShortTimeString();
								//lblNotifyBy.Text = LocRM.GetString("NotifyByEmail");
								if ((bool)rdr["IsNotifiedByEmail"])
								{
									lblNotifyBy.Text = LocRM.GetString("NotifyByEmail");
									if ((bool)rdr["IsNotifiedByIBN"] && PortalConfig.UseIM && !isExternal) lblNotifyBy.Text += ", ";
								}
								if ((bool)rdr["IsNotifiedByIBN"] && PortalConfig.UseIM && !isExternal)
								{
									lblNotifyBy.Text += IbnConst.ProductFamilyShort;
								}
								lblLastBatch.Text = (rdr["BatchLastSent"] != DBNull.Value) ? ((DateTime)rdr["BatchLastSent"]).ToString("g") : "";
								lblNextBatch.Text = (rdr["BatchNextSend"] != DBNull.Value) ? ((DateTime)rdr["BatchNextSend"]).ToString("g") : "";
							}
							else
							{
								lblNotifyDelay.Text = LocRM.GetString("NotifyTypeInst");

								if ((bool)rdr["IsNotifiedByEmail"])
								{
									lblNotifyBy.Text = LocRM.GetString("NotifyByEmail");
									if ((bool)rdr["IsNotifiedByIBN"] && PortalConfig.UseIM && !isExternal) lblNotifyBy.Text += ", ";
								}
								if ((bool)rdr["IsNotifiedByIBN"] && PortalConfig.UseIM && !isExternal)
								{
									lblNotifyBy.Text += IbnConst.ProductFamilyShort;
								}

								trBatchLast.Visible = false;
								trBatchNext.Visible = false;
							}
						}
						else
						{
							lblNotifyEnabled.Text = LocRM.GetString("NotifyDisabled");
							trNotifyBy.Visible = false;
							trNotifyDelay.Visible = false;
							trBatchLast.Visible = false;
							trBatchNext.Visible = false;
						}
					}
				}
				lblMenuInAlerts.Text = User.GetMenuInAlerts(UserID) ? LocRM.GetString("BooleanYes") :
					LocRM.GetString("BooleanNo");
			}
			else
			{
				tblPreferences.Visible = false;
				tbPreferences.Visible = false;
				tdPrefs.Visible = false;
			}

		}
		#endregion

		#region BindToolbars()
		private void BindToolbars()
		{
			tbEMails.AddText(LocRM.GetString("Emails"));
			if (User.CanUpdateUserInfo(UserID))
				tbEMails.AddRightLink("<img alt='' src='../Layouts/images/newitem.gif'/> " + LocRM.GetString("tAddEMail"), Page.ClientScript.GetPostBackClientHyperlink(btnAddNewItem, ""));

			tbPreferences.AddText(LocRM.GetString("tbPrefsTitle"));
			if (User.CanUpdateUserInfo(UserID))
				tbPreferences.AddRightLink("<img alt='' src='../Layouts/Images/edit.gif'/> " + LocRM.GetString("tbPrefsEdit"), "../Directory/PrefsEdit.aspx?Back=View&amp;UserID=" + UserID.ToString());
		}
		#endregion

		#region ApplyLocalization()
		private void ApplyLocalization()
		{
			lblTimeZoneTitle.Text = LocRM.GetString("TimeZoneTitle");
			lblLangTitle.Text = LocRM.GetString("LangTitle");

			lblNotifyEnabledTitle.Text = LocRM.GetString("Notify");
			lblNotifyByTitle.Text = LocRM.GetString("NotifyBy");
			lblNotifyDelayTitle.Text = LocRM.GetString("NotifyType");
			dgEmails.Columns[1].HeaderText = "e-Mail";

			lblEmailDuplication.Text = LocRM.GetString("EmailDuplication");
			lblMIATitle.Text = LocRM.GetString("tMenuInAlerts");

			lblLastBatchTitle.Text = LocRM.GetString("tLastBatch");
			lblNextBatchTitle.Text = LocRM.GetString("tNextBatch");

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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgEmails.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_delete);
			this.dgEmails.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_cancel);
			this.dgEmails.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_edit);
			this.dgEmails.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgEmails_update);
			this.dgEmails.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_itemCommand);
		}
		#endregion

		#region dgEmails
		protected void btnAddNewItem_Click(object sender, System.EventArgs e)
		{

			DataTable dt = User.GetListEmailsDataTable(UserID);

			DataRow dr = dt.NewRow();
			dr["EmailId"] = -1;
			dr["Email"] = "";
			dt.Rows.Add(dr);

			dgEmails.EditItemIndex = dt.Rows.Count - 1;
			dgEmails.DataKeyField = "EmailId";
			dgEmails.DataSource = dt.DefaultView;
			dgEmails.DataBind();
			dgEmails.Visible = true;
		}

		private void dgEmails_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			User.DeleteEmail(sid);
			dgEmails.EditItemIndex = -1;
			//BindData();
			Response.Redirect("../Directory/UserView.aspx?UserID=" + UserID.ToString());
		}

		private void dgEmails_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgEmails.EditItemIndex = e.Item.ItemIndex;
			dgEmails.DataKeyField = "EmailId";
			BindData();
		}

		private void dgEmails_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgEmails.EditItemIndex = -1;
			BindData();
		}

		private void dgEmails_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int ItemID = (int)dgEmails.DataKeys[e.Item.ItemIndex];
			TextBox tbEmail = (TextBox)e.Item.FindControl("tbEmail");
			if (tbEmail != null)
			{
				try
				{
					if (ItemID > 0)
						User.UpdateEmail(ItemID, tbEmail.Text);
					else
						User.AddEmail(UserID, tbEmail.Text);
				}
				catch (EmailDuplicationException)
				{
					lblEmailDuplication.Visible = true;
					return;
				}
			}

			dgEmails.EditItemIndex = -1;
			//BindData();	
			Response.Redirect("../Directory/UserView.aspx?UserID=" + UserID.ToString());
		}

		private void dg_itemCommand(Object sender, DataGridCommandEventArgs e)
		{
			if (((ImageButton)e.CommandSource).CommandName == "SetPrimary")
			{
				int sid = int.Parse(e.Item.Cells[0].Text);
				User.EmailSetPrimary(sid);
				Response.Redirect("../Directory/UserView.aspx?UserID=" + UserID.ToString());
			}
		}
		#endregion
	}
}
