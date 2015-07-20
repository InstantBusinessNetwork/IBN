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
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for MailIncidents.
	/// </summary>
	public partial class MailIncidents : System.Web.UI.UserControl
	{

		protected string Password = "";
		private int _Level = 0;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

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

		private Mediachase.IBN.Business.MailboxType mbType
		{
			get
			{
				try
				{
					return (Mediachase.IBN.Business.MailboxType)(int.Parse(Request["Type"].ToString()));
				}
				catch
				{
					return MailboxType.Issue;
				}
			}
		}

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.FileStorage fs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");

			if (MailBoxId > 0)
			{
				Pop3Manager.Current.SelectedPop3Box = Pop3Manager.Current.GetPop3Box(MailBoxId);
			}
			else
				Pop3Manager.Current.SelectedPop3Box = new Pop3Box();

			cbUseMailIncs.Attributes.Add("onclick", "EnableDisable();");
			BindToolbars();
			lblCheck.Visible = false;
			if (!IsPostBack)
			{
				ApplyLocalization();
				BindData();
			}
			if (!cbUseMailIncs.Checked && txtPort.Value == "")
				txtPort.Value = "110";
			if (ViewState["_Pass"] != null)
				Password = ViewState["_Pass"].ToString();
			else
				Password = "";
			if (mbType == MailboxType.Issue)
			{
				trFolder.Visible = false;
			}
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
			cbUseMailIncs.Text = LocRM.GetString("tUse");
			lblPeriod.Text = LocRM.GetString("tPeriod") + ":";
			lblServer.Text = LocRM.GetString("tServer") + ":";
			lblPort.Text = LocRM.GetString("tPort") + ":";
			lblUser.Text = LocRM.GetString("tUser") + ":";
			lblPass.Text = LocRM.GetString("tPassword") + ":";
			cbAutoApprove.Text = LocRM.GetString("tAutoApprove");
			cbAutoDelete.Text = LocRM.GetString("tAutoDelete");
			cbUseExternal.Text = LocRM.GetString("tUseExternal");
			lgdIncSets.InnerText = LocRM.GetString("tIncSets");
			lgdPopSets.InnerText = LocRM.GetString("tPopSets");
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t10min"), "10"));
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t30min"), "30"));
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t60min"), "60"));
			txtPeriod.Items.Add(new ListItem(LocRM.GetString("t180min"), "180"));
			lgdFolder.InnerText = LocRM.GetString("tSelectFolder");
			cvFolder.ErrorMessage = LocRM.GetString("tCantSelectThisFolder");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>" +
					  "ChColl=document.getElementsByTagName('input');" +
					  "for(j=0;j<ChColl.length;j++){" +
					  "obj = ChColl[j];" +
					  "_obj_id = obj.id;" +
					  "if(_obj_id.indexOf('cbAutoApprove')>=0)" +
					  "obj.disabled=true;" +
					  "if(_obj_id.indexOf('cbAutoDelete')>=0)" +
					  "obj.disabled=true;" +
					  "if(_obj_id.indexOf('cbUseExternal')>=0)" +
					  "obj.disabled=true;}" +
					  "</script>");

			ddFolder.Items.Clear();

			bic = BaseIbnContainer.Create("FileLibrary", "Workspace");
			fs = (FileStorage)bic.LoadControl("FileStorage");
			ProcessFolder(fs.Root.Id);
			/*using (IDataReader reader1 = Folder.GetFolderList(0))
			{
				while (reader1.Read())
				{
					ProcessFolder((int)reader1["FolderId"]);
				}
			}*/

			//using (IDataReader reader = Mailbox.Get(MailBoxId, false))
			//{
			//	if(reader.Read())
			if (MailBoxId > 0)
			{
				txtMBTitle.Text = Pop3Manager.Current.SelectedPop3Box.Name;
				if (Pop3Manager.Current.SelectedPop3Box.IsActive)
				{
					cbUseMailIncs.Checked = true;
					txtPeriod.Disabled = false;
					//int iPeriod = (int)reader["Period"];
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
					txtServer.Disabled = false;
					txtServer.Value = Pop3Manager.Current.SelectedPop3Box.Server;
					txtPort.Disabled = false;
					txtPort.Value = Pop3Manager.Current.SelectedPop3Box.Port.ToString();
					txtUser.Disabled = false;
					txtUser.Value = Pop3Manager.Current.SelectedPop3Box.Login;
					txtPass.Disabled = false;
					ViewState["_Pass"] = Pop3Manager.Current.SelectedPop3Box.Password;
					txtPass.Value = Pop3Manager.Current.SelectedPop3Box.Password;
					if (mbType == MailboxType.Folder)
					{
						ddFolder.Disabled = false;
						ListItem liItem = ddFolder.Items.FindByValue(Pop3Manager.Current.SelectedPop3Box.Parameters["FolderId"]);
						if (liItem != null)
							liItem.Selected = true;
					}
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>" +
									"ChColl=document.getElementsByTagName('input');" +
									"for(j=0;j<ChColl.length;j++){" +
									"obj = ChColl[j];" +
									"_obj_id = obj.id;" +
									"if(_obj_id.indexOf('cbAutoApprove')>=0)" +
									"obj.disabled=false;" +
									"if(_obj_id.indexOf('cbAutoDelete')>=0)" +
									"obj.disabled=false;" +
									"if(_obj_id.indexOf('cbUseExternal')>=0)" +
									"obj.disabled=false;}" +
									"</script>");

					cbAutoApprove.Checked = Pop3Manager.Current.SelectedPop3Box.Parameters["AutoApproveForKnown"] == "1";
					cbAutoDelete.Checked = Pop3Manager.Current.SelectedPop3Box.Parameters["AutoKillForUnknown"] == "1";
					cbUseExternal.Checked = Pop3Manager.Current.SelectedPop3Box.Parameters["OnlyExternalSenders"] == "1";

					visCheck.Disabled = false;
				}
				else
				{
					//						if(reader["Period"]!=DBNull.Value)
					//						{
					int iPeriod = Pop3Manager.Current.SelectedPop3Box.Interval;
					foreach (ListItem li in txtPeriod.Items)
					{
						if (li.Value == iPeriod.ToString())
						{
							li.Selected = true;
							break;
						}
					}
					//						}
					//if(reader["Server"]!=DBNull.Value)
					txtServer.Value = Pop3Manager.Current.SelectedPop3Box.Server;
					//if(reader["Port"]!=DBNull.Value)
					txtPort.Value = Pop3Manager.Current.SelectedPop3Box.Port.ToString();
					//if(reader["User"]!=DBNull.Value)
					txtUser.Value = Pop3Manager.Current.SelectedPop3Box.Login;
					//if(reader["Password"]!=DBNull.Value)
					//{
					ViewState["_Pass"] = Pop3Manager.Current.SelectedPop3Box.Password;
					txtPass.Value = Pop3Manager.Current.SelectedPop3Box.Password;
					//}
					if (mbType == MailboxType.Folder)
					{
						ddFolder.Disabled = false;
						ListItem liItem = ddFolder.Items.FindByValue(Pop3Manager.Current.SelectedPop3Box.Parameters["FolderId"]);
						if (liItem != null)
							liItem.Selected = true;
						ddFolder.Disabled = true;
					}
					//if(reader["AutoApproveForKnown"]!=DBNull.Value)
					//	cbAutoApprove.Checked=(bool)reader["AutoApproveForKnown"];
					//if(reader["AutoKillForUnknown"]!=DBNull.Value)
					//	cbAutoDelete.Checked=(bool)reader["AutoKillForUnknown"];
					//if(reader["OnlyExternalSenders"]!=DBNull.Value)
					//	cbUseExternal.Checked=(bool)reader["OnlyExternalSenders"];
					cbAutoApprove.Checked = Pop3Manager.Current.SelectedPop3Box.Parameters["AutoApproveForKnown"] == "1";
					cbAutoDelete.Checked = Pop3Manager.Current.SelectedPop3Box.Parameters["AutoKillForUnknown"] == "1";
					cbUseExternal.Checked = Pop3Manager.Current.SelectedPop3Box.Parameters["OnlyExternalSenders"] == "1";
				}
				//				}
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
			this.cvFolder.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvFolder_ServerValidate);
		}
		#endregion

		#region Save_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			bool IsEnable = cbUseMailIncs.Checked;

			if (!IsEnable && MailBoxId > 0)
			{
				Pop3Manager.Current.SelectedPop3Box.IsActive = false;
				Pop3Manager.Current.UpdatePop3Box(Pop3Manager.Current.SelectedPop3Box);

				//Mailbox.Disable(MailBoxId);
				Response.Redirect("~/Admin/EmailBoxes.aspx");
			}

			foreach (ListItem liItem in ddFolder.Items)
			{
				if (liItem.Value.IndexOf("*") >= 0)
					liItem.Attributes.Add("style", "COLOR: gray;");
			}

			Page.Validate();
			if (!Page.IsValid)
				return;

			int iFolderId = -1;
			if (mbType == MailboxType.Folder)
				iFolderId = int.Parse(ddFolder.Value);

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
				currentBox.Interval = iPeriod;
				currentBox.Server = sServer;
				currentBox.Port = iPort;
				currentBox.Login = sUser;
				currentBox.Password = sPass;

				currentBox.Parameters["AutoApproveForKnown"] = cbAutoApprove.Checked ? "1" : "0";
				currentBox.Parameters["AutoKillForUnknown"] = cbAutoDelete.Checked ? "1" : "0";
				currentBox.Parameters["OnlyExternalSenders"] = cbUseExternal.Checked ? "1" : "0";
				currentBox.Parameters["FolderId"] = iFolderId.ToString();

				//Pop3Manager.Current.
				//				Mailbox.Update(MailBoxId, txtMBTitle.Text, IsEnable, mbType, iFolderId, iPeriod, sServer, iPort, sUser, sPass,
				//					cbAutoApprove.Checked, cbAutoDelete.Checked, cbUseExternal.Checked);
			}
			else
			{
				int iPeriod = int.Parse(txtPeriod.Value);
				string sServer = txtServer.Value;
				int iPort = int.Parse(txtPort.Value);
				string sUser = txtUser.Value;
				string sPass = txtPass.Value;

				currentBox.Name = txtMBTitle.Text;
				currentBox.IsActive = IsEnable;
				currentBox.Interval = iPeriod;
				currentBox.Server = sServer;
				currentBox.Port = iPort;
				currentBox.Login = sUser;
				currentBox.Password = sPass;

				currentBox.Parameters["AutoApproveForKnown"] = cbAutoApprove.Checked ? "1" : "0";
				currentBox.Parameters["AutoKillForUnknown"] = cbAutoDelete.Checked ? "1" : "0";
				currentBox.Parameters["OnlyExternalSenders"] = cbUseExternal.Checked ? "1" : "0";
				currentBox.Parameters["FolderId"] = iFolderId.ToString();

				if (mbType == MailboxType.Folder)
					currentBox.Handlers.Add("File.Pop3MessageHandler");
				else
					currentBox.Handlers.Add("IssueRequest.Pop3MessageHandler");

				//Pop3Manager.Current.
				//				Mailbox.Create(txtMBTitle.Text, cbUseMailIncs.Checked, mbType, iFolderId, int.Parse(txtPeriod.Value), 
				//					txtServer.Value, int.Parse(txtPort.Value), txtUser.Value, txtPass.Value, 
				//					cbAutoApprove.Checked, cbAutoDelete.Checked, cbUseExternal.Checked);
			}

			Pop3Manager.Current.UpdatePop3Box(currentBox);

			Response.Redirect("~/Admin/EmailBoxes.aspx");
		}
		#endregion

		#region Check_Click
		protected void btnCheck_Click(object sender, System.EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>EnableDisable();</script>");
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
			foreach (ListItem liItem in ddFolder.Items)
			{
				if (liItem.Value.IndexOf("*") >= 0)
					liItem.Attributes.Add("style", "COLOR: gray;");
			}
		}
		#endregion

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/EmailBoxes.aspx");
		}

		private void ProcessFolder(int iFolder)
		{
			string name = "";
			string folder = "";
			for (int i = 0; i < _Level; i++)
				name += "&nbsp;&nbsp;&nbsp;";
			_Level++;
			bool fl = false;
			if (fs.CanUserWrite(iFolder))
			{
				if (iFolder == fs.Root.Id)
					name += LocRM.GetString("tRoot");
				else
					name += fs.GetDirectory(iFolder).Name;
				folder = iFolder.ToString();
				fl = true;
			}
			else
			{
				if (iFolder == fs.Root.Id)
					name += LocRM.GetString("tRoot");
				else
					name += fs.GetDirectory(iFolder).Name;
				folder = "*" + iFolder.ToString();
			}
			System.IO.StringWriter writer = new System.IO.StringWriter();
			Server.HtmlDecode(name, writer);
			string decoded = writer.ToString();

			ListItem item = new ListItem(decoded, folder);
			if (!fl)
				item.Attributes.Add("style", "COLOR: gray;");

			ddFolder.Items.Add(item);

			DirectoryInfo[] _di = fs.GetDirectories(iFolder);
			foreach (DirectoryInfo di in _di)
			{
				ProcessFolder(di.Id);
				_Level--;
			}
		}

		private void cvFolder_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>EnableDisable();</script>");
			if (ddFolder.Value.IndexOf("*") >= 0)
				args.IsValid = false;
			else
				args.IsValid = true;
		}
	}
}
