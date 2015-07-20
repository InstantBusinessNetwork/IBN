namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Text;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.Pop3;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for MailIncidents2.
	/// </summary>
	public partial class MailIncidents2 : System.Web.UI.UserControl
	{
		protected string Password = "";
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private System.Web.UI.UserControl propertyPageControl = null;
		private bool _CallPropertyPageLoad = false;

		private int MailBoxId
		{
			get
			{
				try
				{
					return int.Parse(Request["MailBoxId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");

			if (!IsPostBack)
			{
				_CallPropertyPageLoad = true;

				if (MailBoxId > 0)
					Pop3Manager.Current.SelectedPop3Box = Pop3Manager.Current.GetPop3Box(MailBoxId);
				else
					Pop3Manager.Current.SelectedPop3Box = new Pop3Box();
				ViewState["Pop3Box"] = Pop3Manager.Current.SelectedPop3Box;
			}
			else
				Pop3Manager.Current.SelectedPop3Box = (Pop3Box)ViewState["Pop3Box"];

			BindPropertyPage();

			BindToolbars();
			lblCheck.Visible = false;
			if (!IsPostBack)
			{
				ApplyLocalization();
				BindData();
			}
			if (txtPort.Value == "")
				txtPort.Value = "110";
			if (ViewState["_Pass"] != null)
				Password = ViewState["_Pass"].ToString();
			else
				Password = "";
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		private void BindToolbars()
		{
			tbMailIncs.Title = LocRM.GetString("tbTitle");
			tbMailIncs.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tMailBoxes"), 
				ResolveUrl("~/Admin/EmailBoxes.aspx"));

			btnSave.Text = LocRM.GetString("tSave");
			btnCancel.Text = LocRM.GetString("tCancel");
			visCheck.Value = LocRM.GetString("tCheck");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblPeriod.Text = LocRM.GetString("tPeriod") + ":";
			lblServer.Text = LocRM.GetString("tServer") + ":";
			lblPort.Text = LocRM.GetString("tPort") + ":";
			lblUser.Text = LocRM.GetString("tUser") + ":";
			lblPass.Text = LocRM.GetString("tPassword") + ":";
			lgdPopSets.InnerText = LocRM.GetString("tPopSets");
			lgdActivity.InnerText = LocRM.GetString("tActivitySets");
			cbUseMailIncs.Text = LocRM.GetString("tActive");
			cbDeleteFromServer.Text = LocRM.GetString("tDelFromServer");
			lgdInfo.InnerText = LocRM.GetString("tInfoBlock");
			lblLastRequestTitle.Text = LocRM.GetString("tLastReq") + ":";
			lblLastSuccRequestTitle.Text = LocRM.GetString("tLastSuccReq") + ":";
			lblErrorMessageTitle.Text = LocRM.GetString("tErrorMessage") + ":";
			lgdHandler.InnerText = LocRM.GetString("tHandlerBlock");
			lblHandlerTitle.Text = LocRM.GetString("tHandler") + ":";
			lblHandlerDescTitle.Text = LocRM.GetString("tDescription") + ":";
			lgdHandlerSets.InnerText = LocRM.GetString("tHandlerSets");
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t10min"), "10"));
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t30min"), "30"));
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t60min"), "60"));
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t180min"), "180"));
		}
		#endregion

		#region BindData
		private void BindData()
		{
			ddHandler.Items.Clear();
			foreach (Pop3MessageHandlerInfo pop3mhi in Pop3Manager.Current.Config.Handlers)
			{
				ListItem li = new ListItem(pop3mhi.Name, pop3mhi.Name);
				if (Pop3Manager.Current.SelectedPop3Box.Handlers.Count > 0)
				{
					if (li.Value == Pop3Manager.Current.SelectedPop3Box.Handlers[0].Name)
						li.Selected = true;
				}
				ddHandler.Items.Add(li);
			}
			ddHandler.Items.Insert(0, new ListItem(LocRM.GetString("tNone"), ""));

			cbDeleteFromServer.Checked = true;

			trInfo.Visible = false;

			if (MailBoxId > 0)
			{
				trInfo.Visible = true;

				txtMBTitle.Text = Pop3Manager.Current.SelectedPop3Box.Name;

				cbUseMailIncs.Checked = Pop3Manager.Current.SelectedPop3Box.IsActive;
				cbDeleteFromServer.Checked = Pop3Manager.Current.SelectedPop3Box.AutoKillForRead;

				int iPeriod = Pop3Manager.Current.SelectedPop3Box.Interval;
				foreach (ListItem li in txtPeriod.Items)
				{
					if (li.Value == iPeriod.ToString())
					{
						li.Selected = true;
						break;
					}
					else
						li.Selected = false;
				}

				txtServer.Value = Pop3Manager.Current.SelectedPop3Box.Server;
				txtPort.Value = Pop3Manager.Current.SelectedPop3Box.Port.ToString();
				txtUser.Value = Pop3Manager.Current.SelectedPop3Box.Login;
				ViewState["_Pass"] = Pop3Manager.Current.SelectedPop3Box.Password;
				txtPass.Value = Pop3Manager.Current.SelectedPop3Box.Password;


				if (Pop3Manager.Current.SelectedPop3Box.LastRequest != DateTime.MinValue)
					lblLastRequest.Text = Pop3Manager.Current.SelectedPop3Box.LastRequest.ToString();
				else
					lblLastRequest.Text = "";
				if (Pop3Manager.Current.SelectedPop3Box.LastSuccessfulRequest != DateTime.MinValue)
					lblLastSuccRequest.Text = Pop3Manager.Current.SelectedPop3Box.LastSuccessfulRequest.ToString();
				else
					lblLastSuccRequest.Text = "";
				lblErrorMessage.Text = Pop3Manager.Current.SelectedPop3Box.LastErrorText;
			}
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
		}
		#endregion

		protected void ddHandler_Change(object sender, System.EventArgs e)
		{
			if (Pop3Manager.Current.SelectedPop3Box.Handlers.Count > 0)
			{
				if (propertyPageControl is IPersistPop3MessageHandlerStorage)
					((IPersistPop3MessageHandlerStorage)propertyPageControl).Save(Pop3Manager.Current.SelectedPop3Box);
			}

			Pop3Manager.Current.SelectedPop3Box.Handlers.Clear();
			if (ddHandler.SelectedIndex > 0)
				Pop3Manager.Current.SelectedPop3Box.Handlers.Add(ddHandler.SelectedItem.Value);

			_CallPropertyPageLoad = true;

			BindPropertyPage();
		}

		private void BindPropertyPage()
		{
			tdHandlerSets.Controls.Clear();
			trHandlerSets.Visible = false;
			propertyPageControl = null;
			lblHandlerDesc.Text = "";

			if (Pop3Manager.Current.SelectedPop3Box.Handlers.Count > 0)
			{
				lblHandlerDesc.Text = Pop3Manager.Current.SelectedPop3Box.Handlers[0].Handler.Description;
				if (Pop3Manager.Current.SelectedPop3Box.Handlers[0].PropertyControlPath != string.Empty)
				{
					try
					{
						propertyPageControl = (System.Web.UI.UserControl)Page.LoadControl(Pop3Manager.Current.SelectedPop3Box.Handlers[0].PropertyControlPath);
						if (propertyPageControl is IPersistPop3MessageHandlerStorage && _CallPropertyPageLoad)
						{
							((IPersistPop3MessageHandlerStorage)propertyPageControl).Load(Pop3Manager.Current.SelectedPop3Box);
						}
					}
					catch (Exception ex)
					{
						//System.Diagnostics.Trace.WriteLine(ex);
						Label lblErrLoadMessage = new Label();
						lblErrLoadMessage.CssClass = "text";
						lblErrLoadMessage.Style.Add("color", "red");
						lblErrLoadMessage.Text = ex.ToString();
						tdHandlerSets.Controls.Add(lblErrLoadMessage);
						trHandlerSets.Visible = true;
						return;
					}
					tdHandlerSets.Controls.Add(propertyPageControl);
					trHandlerSets.Visible = true;
				}
			}
		}

		#region Save_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			bool IsEnable = cbUseMailIncs.Checked;

			Page.Validate();
			if (!Page.IsValid)
				return;

			Pop3Box currentBox = Pop3Manager.Current.SelectedPop3Box;

			if (MailBoxId > 0)
			{
				int iPeriod = int.Parse(txtPeriod.Value);
				string sServer = txtServer.Value;
				int iPort = int.Parse(txtPort.Value);
				string sUser = txtUser.Value;
				string sPass = txtPass.Value;
				if (sPass.Length <= 0)
					sPass = Password;

				currentBox.Name = txtMBTitle.Text;
				currentBox.IsActive = IsEnable;
				currentBox.AutoKillForRead = cbDeleteFromServer.Checked;
				currentBox.Interval = iPeriod;
				currentBox.Server = sServer;
				currentBox.Port = iPort;
				currentBox.Login = sUser;
				currentBox.Password = sPass;

			}
			else
			{
				int iPeriod = int.Parse(txtPeriod.Value);
				string sServer = txtServer.Value;
				int iPort = int.Parse(txtPort.Value);
				string sUser = txtUser.Value;
				string sPass = txtPass.Value;
				if (sPass.Length <= 0)
					sPass = Password;

				currentBox.Name = txtMBTitle.Text;
				currentBox.IsActive = IsEnable;
				currentBox.AutoKillForRead = cbDeleteFromServer.Checked;
				currentBox.Interval = iPeriod;
				currentBox.Server = sServer;
				currentBox.Port = iPort;
				currentBox.Login = sUser;
				currentBox.Password = sPass;

			}

			// Save Property Page Settings
			if (propertyPageControl is IPersistPop3MessageHandlerStorage)
			{
				((IPersistPop3MessageHandlerStorage)propertyPageControl).Save(currentBox);
			}
			//

			Pop3Manager.Current.UpdatePop3Box(currentBox);

			Response.Redirect("~/Admin/EmailBoxes.aspx");
		}
		#endregion

		#region Check_Click
		protected void btnCheck_Click(object sender, System.EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "try{EnableDisable();}catch(e){}", true);
			string sServer = txtServer.Value;
			int iPort = 0;
			try
			{
				iPort = int.Parse(txtPort.Value);
			}
			catch
			{
				lblCheck.Visible = true;
				lblCheck.ForeColor = Color.Red;
				lblCheck.Text = LocRM.GetString("tError") + "&nbsp;";
				return;
			}
			string sUser = txtUser.Value;
			string sPass = txtPass.Value;
			if (sPass.Length <= 0)
				sPass = Password;
			else
				ViewState["_Pass"] = txtPass.Value;

			//TEST Addon (OlegO): 
			//Incident.CreateFromEMail();

			Pop3SettingsResult _sok = EMailRouterPop3Box.CheckSettings(sServer, iPort, sUser, sPass, Pop3SecureConnectionType.None);
			if (_sok == Pop3SettingsResult.AllOk)
			{
				lblCheck.Visible = true;
				lblCheck.ForeColor = Color.Blue;
				lblCheck.Text = LocRM.GetString("tOK") + "&nbsp;";
			}
			else if (_sok == Pop3SettingsResult.None)
			{
				lblCheck.Visible = true;
				lblCheck.ForeColor = Color.Red;
				lblCheck.Text = LocRM.GetString("tError") + "&nbsp;";
				lblServerError.Text = "*";
				lblPortError.Text = "*";
			}
			else if (_sok == Pop3SettingsResult.ServerName)
			{
				lblCheck.Visible = true;
				lblCheck.ForeColor = Color.Red;
				lblCheck.Text = LocRM.GetString("tError") + "&nbsp;";
				lblUserError.Text = "*";
			}
			else if (_sok == (Pop3SettingsResult.ServerName | Pop3SettingsResult.Pop3User))
			{
				lblCheck.Visible = true;
				lblCheck.ForeColor = Color.Red;
				lblCheck.Text = LocRM.GetString("tError") + "&nbsp;";
				lblUserError.Text = "*";
				lblPassError.Text = "*";
			}
		}
		#endregion

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/EmailBoxes.aspx");
		}

	}
}
