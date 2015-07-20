namespace Mediachase.UI.Web.Wizards.Modules
{
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
	using System.Globalization;
	using System.Resources;
	using System.Threading;
	using System.IO;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for FirstTimeLoginWizard.
	/// </summary>
	public partial class FirstTimeLoginWizard : System.Web.UI.UserControl, IWizardControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strFirstLogWd", typeof(FirstTimeLoginWizard).Assembly);

		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();

		//private int maxsteps = 5;
		private int _stepCount = 4;

		#region HTML Vars
		protected System.Web.UI.WebControls.DropDownList lstTimeZone;
		protected System.Web.UI.WebControls.DropDownList lstLang;
		protected System.Web.UI.WebControls.Label lblEnableNotify;
		protected System.Web.UI.WebControls.Label lblEmail;
		protected System.Web.UI.WebControls.Label lblIBN;
		protected System.Web.UI.WebControls.Label lblInstantly;
		protected System.Web.UI.WebControls.Label lblByBatch;
		protected System.Web.UI.WebControls.Label lblEvery;
		protected System.Web.UI.WebControls.DropDownList ddEvery;
		protected System.Web.UI.WebControls.DropDownList ddFrom;
		protected System.Web.UI.WebControls.DropDownList ddTo;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox cbEnableNotify;
		protected System.Web.UI.HtmlControls.HtmlGenericControl fsNotification;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdNotifyBy;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox cbEmail;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox cbIBN;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdNotifyDelay;
		protected System.Web.UI.HtmlControls.HtmlInputRadioButton rbInstantly;
		protected System.Web.UI.HtmlControls.HtmlInputRadioButton rbByBatch;
		protected System.Web.UI.WebControls.Button btnSave;

		#endregion

		private int UserID = Security.CurrentUser.UserID;
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindStep1();
		}

		#region Step1
		private void BindStep1()
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), "GetDate", "<script>var date = new Date();" +
				"document.forms[0]." + hdnOffset.ClientID + ".value = date.getTimezoneOffset();</script>");

			pastStep.Value = "1";

			lgdContactInf.InnerText = LocRM.GetString("tContactInformation");
			lgdDetails.InnerText = LocRM.GetString("tDetails");

			cbAllow.Text = LocRM.GetString("tAllow");
			cbAllow.Checked = true;

			using (IDataReader reader = User.GetUserInfo(UserID))
			{
				if (reader.Read())
				{
					if (reader["FirstName"] != DBNull.Value)
						txtFirstName.Text = HttpUtility.HtmlDecode(reader["FirstName"].ToString());
					if (reader["LastName"] != DBNull.Value)
						txtLastName.Text = HttpUtility.HtmlDecode(reader["LastName"].ToString());
					if (reader["Email"] != DBNull.Value)
						txtEMail.Text = HttpUtility.HtmlDecode(reader["Email"].ToString());
				}
			}

			using (IDataReader reader = User.GetUserProfile(UserID))
			{
				if (reader.Read())
				{
					if (reader["phone"] != DBNull.Value)
						txtWorkPhone.Text = HttpUtility.HtmlDecode(reader["phone"].ToString());
					if (reader["mobile"] != DBNull.Value)
						txtMobilePhone.Text = HttpUtility.HtmlDecode(reader["mobile"].ToString());
				}
			}

			lbUsers.Attributes.Add("ondblclick", Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			cbCanManage.Text = HttpUtility.HtmlDecode(LocRM.GetString("CanManage"));
			ViewState["SearchMode"] = false;
			btnAdd.Text = LocRM.GetString("Add");
			btnSearch.Text = LocRM.GetString("FindNow");
		}
		#endregion

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			#region Step2 - Details
			if (step == 2)
			{
				if (pastStep.Value == "1")
					User.UpdateUserInfo(txtFirstName.Text, txtLastName.Text, txtEMail.Text, txtWorkPhone.Text, txtMobilePhone.Text);
				pastStep.Value = "2";
				using (IDataReader reader = User.GetUserProfile(UserID))
				{
					if (reader.Read())
					{
						if (reader["company"] != DBNull.Value)
							txtCompany.Text = HttpUtility.HtmlDecode(reader["company"].ToString());
						if (reader["position"] != DBNull.Value)
							txtPosition.Text = HttpUtility.HtmlDecode(reader["position"].ToString());
						if (reader["department"] != DBNull.Value)
							txtDepartment.Text = HttpUtility.HtmlDecode(reader["department"].ToString());
						if (reader["location"] != DBNull.Value)
							txtLocation.Text = HttpUtility.HtmlDecode(reader["location"].ToString());
						//if ( reader["PictureUrl"] == DBNull.Value || (string)reader["PictureUrl"]=="" )
						//{
						//    Picture.Visible=false;
						//}
						//else
						//{
						imgPhoto.Src = "~/Common/GetUserPhoto.aspx?UserID=" + UserID.ToString() + "&t=" + DateTime.Now.Millisecond.ToString();
						//    Picture.Visible=true;
						//}
					}
				}
			}
			#endregion

			#region Step3 - Preferences
			if (step == 3)
			{
				if (pastStep.Value == "2")
				{
					string filename = "";
					System.IO.Stream strres = new System.IO.MemoryStream();

					if (fPhoto.PostedFile != null && fPhoto.PostedFile.ContentLength > 0)
					{
						System.Drawing.Image img;
						string extension = "";
						img = Mediachase.Ibn.Web.UI.Images.ProcessImage(fPhoto.PostedFile, out extension);
						string photoid = Guid.NewGuid().ToString().Substring(0, 6);
						filename = photoid + extension;
						img.Save(strres, img.RawFormat);
						strres.Position = 0;
					}
					User.UpdateUserInfo(txtPosition.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, filename, strres);
				}
				ctlEditPrefs.LoadPrefs();
				pastStep.Value = "3";
			}
			#endregion

			#region Step4 - TeamStep
			if (step == 4)
			{
				if (pastStep.Value == "3")
				{
					ctlEditPrefs.SavePrefs();
				}
				pastStep.Value = "4";

				DataTable _dt = Mediachase.IBN.Business.User.GetListPeopleForSharingDataTable(UserID);
				ViewState["Participants"] = _dt;
				BindGroups();
				BinddgMemebers();
			}
			#endregion

			#region Step5
			if (step == 5)
			{
				if (pastStep.Value == "4")
				{
					DataTable dt = (DataTable)ViewState["Participants"];
					Mediachase.IBN.Business.User.UpdateSharing(UserID, dt);
				}
				pastStep.Value = "5";

				//BindStep5
			}
			#endregion

			#region Step6 - Congratulations
			if (step == 6)
			{
				//BindStep6
			}
			#endregion

		}

		#region TeamStep
		private void BindGroups()
		{
			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 1; i < Level; i++)
						GroupName = "  " + GroupName;
					ListItem item = new ListItem(GroupName, GroupId);

					ddGroups.Items.Add(item);
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}

		private void BindGroupUsers(int GroupID)
		{
			lbUsers.Items.Clear();
			DataTable dt = (DataTable)ViewState["Participants"];
			using (IDataReader rdr = SecureGroup.GetListActiveUsersInGroup(GroupID))
			{
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() + " " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));

				}
			}
		}

		private void BinddgMemebers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("Name");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("Status");

			DataTable dt = (DataTable)ViewState["Participants"];
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
				ib.ToolTip = LocRM.GetString("Delete");
			}
		}

		protected string GetLevel(int level)
		{
			if (level == 1)
				return LocRM.GetString("Manage");
			else
				return LocRM.GetString("Read");
		}

		protected string GetLink(int PID, bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLinkUL(PID);
			else
				return CommonHelper.GetUserStatusUL(PID);
		}

		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{

			int UserId = int.Parse(e.Item.Cells[0].Text);
			DataTable dt = (DataTable)ViewState["Participants"];
			DataRow[] dr = dt.Select("UserId = " + UserId);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Participants"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		protected void ddGroups_ChangeGroup(object sender, System.EventArgs e)
		{
			tbSearch.Text = "";
			ViewState["SearchString"] = null;

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}

		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			if (tbSearch.Text != "")
			{
				ViewState["SearchString"] = tbSearch.Text;
				BindSearchedUsers(tbSearch.Text);
			}
		}

		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["UserId"] = int.Parse(li.Value);
					if (cbCanManage.Checked)
						dr["Level"] = 1;
					else
						dr["Level"] = 0;
					dt.Rows.Add(dr);
				}

			ViewState["Participants"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		private void BindUsers()
		{
			if (ViewState["SearchString"] == null)
			{
				ListItem _li = ddGroups.SelectedItem;
				if (_li != null)
					BindGroupUsers(int.Parse(_li.Value));
			}
			else
				BindSearchedUsers(ViewState["SearchString"].ToString());
		}

		private void BindSearchedUsers(string searchstr)
		{
			DataTable dt = (DataTable)ViewState["Participants"];
			lbUsers.Items.Clear();
			using (IDataReader rdr = Mediachase.IBN.Business.User.GetListUsersBySubstring(searchstr))
			{
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() + " " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));
				}
			}
		}
		#endregion

		private void UpdateProperties()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			if (!cbAllow.Checked)
				pc["USetup_ShowStartupWizard"] = "False";
			else
				pc["USetup_ShowStartupWizard"] = "True";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			subtitles.Add(LocRM.GetString("s1SubTitle"));
			subtitles.Add(LocRM.GetString("s2SubTitle"));
			subtitles.Add(LocRM.GetString("s3SubTitle"));
			subtitles.Add(LocRM.GetString("s4SubTitle"));
			//subtitles.Add(LocRM.GetString("s5SubTitle"));
			subtitles.Add(LocRM.GetString("s6SubTitle"));
			steps.Add(step1);
			steps.Add(step2);
			steps.Add(step3);
			steps.Add(step4);
			//steps.Add(step5);
			steps.Add(step6);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgMembers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMembers_Delete);

		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("tTopTitle"); } }
		public bool ShowSteps { get { return true; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
			if (stepNumber == _stepCount)
				MiddleButtonText = LocRM.GetString("tFinish");
			else
				MiddleButtonText = null;
			CancelText = LocRM.GetString("tClose");
		}

		public string GenerateFinalStepScript()
		{
			return "try{window.opener.top.location.href='" + ResolveClientUrl("~/Apps/Shell/Pages/default.aspx") + "';} catch (e) {} window.close();";
		}

		public void CancelAction()
		{
			UpdateProperties();
		}
		#endregion

	}
}
