namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Resources;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Web;
	using Mediachase.IBN.Business.EMail;
	using System.Web.UI.WebControls;
	using System.Web.UI;

	/// <summary>
	///		Summary description for Responsibility.
	/// </summary>
	public partial class Responsibility : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(Responsibility).Assembly);

		#region IncidentID
		protected int IncidentID
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentID"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new Exception("IncidentID is Reqired");
				}
			}
		}
		#endregion

		#region Command
		protected string Command
		{
			get
			{
				string retval = String.Empty;
				if (Request["cmd"] != null)
					retval = Request["cmd"];

				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/objectDD.css");

			ApplyLocalization();
			if (!IsPostBack)
				BindValues();

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			if (!String.IsNullOrEmpty(Command))	// popup mode
			{
				secHeader.Visible = false;
				MainTable.Attributes["class"] = "ibn-propertysheet";
				if (cm != null)
				{
					cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_SelectResourceInFrame");
					cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_GroupResponsibilityInFrame");
				}
			}
			else
			{
				BindToolbar();
				if (cm != null)
				{
					cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_SelectResource");
					cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_GroupResponsibility");
				}
			}
			trPriority.Visible = PortalConfig.CommonIncidentAllowEditPriorityField;
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (!Incident.CanUpdate(IncidentID))
				trPriority.Visible = false;

			if (!Incident.CanChangeProject(IncidentID))
				trProject.Visible = false;

			if (!Incident.CanUpdate(IncidentID))
				trIssueBox.Visible = false;

			string str = "_" + ((int)Mediachase.IBN.Business.ObjectStates.Active).ToString() + "_" +
				"_" + ((int)Mediachase.IBN.Business.ObjectStates.ReOpen).ToString() + "_" +
				"_" + ((int)Mediachase.IBN.Business.ObjectStates.Upcoming).ToString() + "_";
			string str1 = "_" + ((int)Mediachase.IBN.Business.ObjectStates.OnCheck).ToString() + "_";
			ddStatus.Attributes.Add("onchange", String.Format("ChangeVisible('{0}', '{1}', this)", str, str1));

			ScriptManager.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
				String.Format("ChangeVisible('{0}', '{1}', document.getElementById('{2}'));", str, str1, ddStatus.ClientID),
				true);
			BindList();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			UseNewResponsible.Text = LocRM.GetString("UseNewResponsible");
			imbSend.Text = LocRM.GetString("tSend");
			imbSend.CustomImage = Page.ResolveUrl("~/Layouts/Images/saveitem.gif");
		}
		#endregion

		#region CreateDDCell
		private static HtmlTableCell CreateDDCell(string onclick)
		{
			HtmlTableCell tc = new HtmlTableCell();
			tc.Attributes.Add("class", "cellclass");
			tc.Attributes.Add("onmouseover", "TdOver(this)");
			tc.Attributes.Add("onmouseout", "TdOut(this)");
			tc.Attributes.Add("onclick", onclick);

			return tc;
		} 
		#endregion

		#region BindList
		private void BindList()
		{
			DataTable dt = null;

			if (ViewState["ResponsiblePool"] != null)
				dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();

			string sUsers = "";
			if (dt != null)
				foreach (DataRow dr in dt.Rows)
				{
					if ((bool)dr["ResponsePending"])
						sUsers += dr["PrincipalId"].ToString() + "*1_";
					else
						sUsers += dr["PrincipalId"].ToString() + "*0_";
				}

			CommandManager cm = CommandManager.GetCurrent(this.Page);

			Incident.Tracking trk = Incident.GetTrackingInfo(IncidentID);

			//			lblAccept.Visible = trk.ShowAccept || trk.ShowAcceptDecline;
			//			lblDecline.Visible = trk.ShowAcceptDecline || trk.ShowDecline;

			if (trk.ShowAcceptDecline)
			{
				lblAccept.Text = String.Format("<input type='checkbox' onclick=\"javascript:SelectMe('{0}','{1}','0')\" id='cbAccept' /><label for='cbAccept'>&nbsp;{2}</label>&nbsp;&nbsp;&nbsp;",
					HttpUtility.HtmlAttributeEncode(Util.CommonHelper.GetUserStatusUL(Security.CurrentUser.UserID)),
					Security.CurrentUser.UserID,
					LocRM.GetString("tAccept"));

				lblAcceptText.InnerHtml = String.Format("<fieldset style='height:50px;padding:7px;border:1px solid #D0D0BF;' id='fsAccept' class='rest-gray'>{0}</fieldset>",
					LocRM.GetString("tAcceptText"));

				lblDecline.Text = String.Format("<input type='checkbox' onclick=\"javascript:SelectMe('','','1');\" id='cbDecline' /><label for='cbDecline'>&nbsp;{0}</label>",
					LocRM.GetString("tDecline"));

				lblDeclineText.InnerHtml = String.Format("<fieldset style='height:50px;padding:7px;border:1px solid #D0D0BF;' id='fsDecline' class='rest-gray'>{0}</fieldset>",
					LocRM.GetString("tDeclineText"));
			}
			else if (trk.ShowAccept)
			{
				lblAccept.Text = String.Format("<input type='checkbox' onclick=\"javascript:SelectMe('{0}','{1}','0')\" id='cbAccept' /><label for='cbAccept'>&nbsp;{2}</label>&nbsp;&nbsp;&nbsp;",
					HttpUtility.HtmlAttributeEncode(Util.CommonHelper.GetUserStatusUL(Security.CurrentUser.UserID)),
					Security.CurrentUser.UserID,
					LocRM.GetString("tAcceptYourself"));

				lblAcceptText.InnerHtml = String.Format("<fieldset style='height:40px;padding:7px;border:1px solid #D0D0BF;' id='fsAccept' class='rest-gray'>{0}</fieldset>",
					LocRM.GetString("tAcceptYourselfText"));
			}
			else if (trk.ShowDecline)
			{
				lblDecline.Text = String.Format("<input type='checkbox' onclick=\"javascript:SelectMe('','','1');\" id='cbDecline' /><label for='cbDecline'>&nbsp;{0}</label>",
					LocRM.GetString("tDeclineYourself"));

				lblDeclineText.InnerHtml = String.Format("<fieldset style='height:40px;padding:7px;border:1px solid #D0D0BF;' id='fsDecline' class='rest-gray'>{0}</fieldset>",
					LocRM.GetString("tDeclineYourselfText"));

			}

			tableDD.Rows.Clear();
			HtmlTableRow tr;
			HtmlTableCell tc;

			if (trk.CanSetNoUser)
			{
				tr = new HtmlTableRow();
				tc = CreateDDCell("SelectThis(this, -2)");
				tc.InnerHtml = BuildIconAndText("not_set.png", "tRespNotSet");
				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}

			if (trk.CanSetGroup)
			{
				tr = new HtmlTableRow();
				tc = CreateDDCell("SelectThis(this, -1)");

				if (ViewState["DDStatus"].ToString() == "0" || IsAllDenied())
				{
					if (!Security.CurrentUser.IsExternal)
					{
						CommandParameters cp = null;
						if (!String.IsNullOrEmpty(Command))
							cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
						else
							cp = new CommandParameters("MC_HDM_GroupResponsibility");
						cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
						cp.AddCommandArgument("sUsersKey", sUsers);
						cp.AddCommandArgument("NotChangeKey", "0");
						string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
						tc.InnerHtml = BuildLinkWithIconAndText("red_denied.gif", "tRespGroup", cmd);
					}
					else
						tc.InnerHtml = BuildIconAndText("red_denied.gif", "tRespGroup");
				}
				else
				{
					if (!Security.CurrentUser.IsExternal)
					{
						CommandParameters cp = null;
						if (!String.IsNullOrEmpty(Command))
							cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
						else
							cp = new CommandParameters("MC_HDM_GroupResponsibility");
						cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
						cp.AddCommandArgument("sUsersKey", sUsers);
						cp.AddCommandArgument("NotChangeKey", "0");
						string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
						tc.InnerHtml = BuildLinkWithIconAndText("waiting.gif", "tRespGroup", cmd);
					}
					else
						tc.InnerHtml = BuildIconAndText("waiting.gif", "tRespGroup");
				}

				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}

			if (trk.CanSetUser)
			{
				ArrayList alUsers = Incident.GetResponsibleList(IncidentID);
				foreach (int iUserId in alUsers)
				{
					tr = new HtmlTableRow();
					tc = CreateDDCell("SelectThis(this, " + iUserId + ")");
					tc.InnerHtml = Util.CommonHelper.GetUserStatusUL(iUserId);
					tr.Cells.Add(tc);
					tableDD.Rows.Add(tr);
				}

				CommandParameters cp = null;
				if (!String.IsNullOrEmpty(Command))	// popup mode
					cp = new CommandParameters("MC_HDM_SelectResourceInFrame");
				else
					cp = new CommandParameters("MC_HDM_SelectResource");

				string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = "closeMenu();" + cmd;

				tr = new HtmlTableRow();
				tc = CreateDDCell(cmd);
				tc.InnerHtml = String.Format("<b>{0}</b>", LocRM.GetString("tRespMore"));
				tr.Cells.Add(tc);
				tableDD.Rows.Add(tr);
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			ddStatus.DataSource = Incident.GetListIncidentStates(IncidentID);
			ddStatus.DataTextField = "StateName";
			ddStatus.DataValueField = "StateId";
			ddStatus.DataBind();

			ddPriority.DataSource = Incident.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();

			// Issue box
			foreach (IncidentBox folder in IncidentBox.List())
				ddIssueBox.Items.Add(new ListItem(folder.Name, folder.IncidentBoxId.ToString()));

			int controllerId = -1;
			using (IDataReader reader = Incident.GetIncident(IncidentID))
			{
				if (reader.Read())
				{
					CommonHelper.SafeSelect(ddPriority, reader["PriorityId"].ToString());

					if (reader["ControllerId"] != DBNull.Value)
						controllerId = (int)reader["ControllerId"];

					if (reader["ProjectId"] != DBNull.Value)
					{
						ddProject.ObjectTypeId = (int)ObjectTypes.Project;
						ddProject.ObjectId = (int)reader["ProjectId"];
					}

					if (reader["IncidentBoxId"] != DBNull.Value)
						CommonHelper.SafeSelect(ddIssueBox, reader["IncidentBoxId"].ToString());
				}
			}
			if (controllerId > 0)
				lblController.Text = CommonHelper.GetUserStatus(controllerId);

			// Responsible
			DataTable dt = Incident.GetResponsibleGroupDataTable(IncidentID);
			dt.Columns.Add(new DataColumn("IsNew", typeof(bool)));

			string sUsers = "";
			if (dt != null)
				foreach (DataRow dr in dt.Rows)
				{
					dr["IsNew"] = false;
					if ((bool)dr["ResponsePending"])
						sUsers += dr["PrincipalId"].ToString() + "*1_";
					else
						sUsers += dr["PrincipalId"].ToString() + "*0_";
				}

			if (ViewState["ResponsiblePool"] == null)
				ViewState["ResponsiblePool"] = dt.Copy();

			using (IDataReader reader = Incident.GetIncidentTrackingState(IncidentID))
			{
				if (reader.Read())
				{
					if ((int)reader["ResponsibleId"] != -1)
						ViewState["DDStatus"] = reader["ResponsibleId"].ToString();//User
					else if (reader["IsResponsibleGroup"] != DBNull.Value && (bool)reader["IsResponsibleGroup"])
					{
						if ((int)reader["ResponsibleGroupState"] == 1)
							ViewState["DDStatus"] = "-1";//pending
						else
							ViewState["DDStatus"] = "0";//denied
					}
					else
						ViewState["DDStatus"] = "-2";//not set

					Util.CommonHelper.SafeSelect(ddStatus, reader["StateId"].ToString());
				}
			}
			string strddStatus = ViewState["DDStatus"].ToString();

			Incident.Tracking trk = Incident.GetTrackingInfo(IncidentID);
			CommandManager cm = CommandManager.GetCurrent(this.Page);

			if (strddStatus == "-2")
			{
				lblClient.Text = BuildIconAndText("not_set.png", "tRespNotSet");
				hidResp.Value = "-2";
			}
			else if (strddStatus == "-1")
			{
				if (!Security.CurrentUser.IsExternal)
				{
					CommandParameters cp = null;
					if (!String.IsNullOrEmpty(Command))
						cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					else
						cp = new CommandParameters("MC_HDM_GroupResponsibility");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", sUsers);
					cp.AddCommandArgument("NotChangeKey", (!trk.CanSetGroup) ? "1" : "0");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					lblClient.Text = BuildLinkWithIconAndText("waiting.gif", "tRespGroup", cmd);
				}
				else
					lblClient.Text = BuildIconAndText("waiting.gif", "tRespGroup");

				hidResp.Value = "-1";
			}
			else if (strddStatus == "0")
			{
				if (!Security.CurrentUser.IsExternal)
				{
					CommandParameters cp = null;
					if (!String.IsNullOrEmpty(Command))
						cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					else
						cp = new CommandParameters("MC_HDM_GroupResponsibility");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", sUsers);
					cp.AddCommandArgument("NotChangeKey", (!trk.CanSetGroup) ? "1" : "0");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					lblClient.Text = BuildLinkWithIconAndText("red_denied.gif", "tRespGroup", cmd);
				}
				else
					lblClient.Text = BuildIconAndText("red_denied.gif", "tRespGroup");

				hidResp.Value = "-1";
			}
			else
			{
				lblClient.Text = Util.CommonHelper.GetUserStatusUL(int.Parse(strddStatus, CultureInfo.InvariantCulture));
				hidResp.Value = strddStatus;
			}

			lblChange.Text = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' class='btndown2' border='0' src='{0}'/>",
				Page.ResolveUrl("~/Layouts/Images/downbtn.gif"));
			if (trk.CanSetNoUser || trk.CanSetGroup || trk.CanSetUser)
				tdChange.Attributes.Add("onclick", "javascript:ShowHideList(event, '" + tdChange.ClientID + "')");
			tdChange.Style.Add("cursor", "pointer");


			hidDecline.Value = "0";
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("tResponsibility"));

			if (!Security.CurrentUser.IsExternal)
			{
				//if (Mediachase.IBN.Business.EMail.EMailIssueExternalRecipient.HasEMailRecipient(IncidentID))
				secHeader.AddRightLink(
				  String.Format("<img alt='' src='{1}'/> {0}", LocRM.GetString("tbAddEMail"), Page.ResolveUrl("~/Layouts/Images/compose.gif")),
				  String.Format("javascript:ShowResizableWizard('{1}?IncidentId={0}',800,600);", IncidentID, Page.ResolveUrl("~/Incidents/AddEMailMessage.aspx")));

				secHeader.AddRightLink(
				  String.Format("<img alt='' src='{1}'/> {0}", LocRM.GetString("tbAddMess"), Page.ResolveUrl("~/Layouts/Images/icons/comments.gif")),
				  String.Format("javascript:ShowWizard('{1}?IncidentId={0}',570,460);", IncidentID, Page.ResolveUrl("~/Incidents/AddForumMessage.aspx")));
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.imbSend.ServerClick += new EventHandler(imbSend_ServerClick);
			this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
			this.btnRefreshMore.Click += new EventHandler(btnRefreshMore_Click);
		}
		#endregion

		#region Send
		private void imbSend_ServerClick(object sender, EventArgs e)
		{
			DataTable dt = null;
			if (ViewState["ResponsiblePool"] != null)
				dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();

			int iRespId = -1;
			bool isRespGroup = false;
			using (IDataReader reader = Incident.GetIncidentTrackingState(IncidentID))
			{
				if (reader.Read())
				{
					if ((int)reader["ResponsibleId"] != -1)
						iRespId = (int)reader["ResponsibleId"];
					if (reader["IsResponsibleGroup"] != DBNull.Value)
						isRespGroup = (bool)reader["IsResponsibleGroup"];
				}
			}

			string sMessage = txtDescription.Text;
			sMessage = Util.CommonHelper.parsetext_br(sMessage);

			int? issueBoxId = null;
			if (trIssueBox.Visible)
				issueBoxId = int.Parse(ddIssueBox.SelectedValue, CultureInfo.InvariantCulture);

			int? projectId = null;
			if (trProject.Visible)
				projectId = ddProject.ObjectId;

			if (hidDecline.Value == "1")
			{
				Issue2.UpdateQuickTracking(IncidentID, sMessage,
					int.Parse(ddStatus.SelectedValue, CultureInfo.InvariantCulture),
					int.Parse(ddPriority.SelectedValue, CultureInfo.InvariantCulture),
					issueBoxId,
					projectId,
					iRespId, isRespGroup, dt, true, UseNewResponsible.Checked);
			}
			else
			{
				switch (hidResp.Value)
				{
					case "-2":	//not set
						iRespId = -1;
						isRespGroup = false;
						break;
					case "-1":	//group
						iRespId = -1;
						isRespGroup = true;
						break;
					default:	//user
						try
						{
							iRespId = int.Parse(hidResp.Value, CultureInfo.InvariantCulture);
							isRespGroup = false;
						}
						catch { }
						break;
				}

				Issue2.UpdateQuickTracking(IncidentID, sMessage,
					int.Parse(ddStatus.SelectedValue, CultureInfo.InvariantCulture),
					int.Parse(ddPriority.SelectedValue, CultureInfo.InvariantCulture),
					issueBoxId,
					projectId,
					iRespId, isRespGroup, dt, false, UseNewResponsible.Checked);
			}

			if (!String.IsNullOrEmpty(Command))	// popup mode
			{
				CommandParameters cp = new CommandParameters(Command);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
			{
				string sLink = "../Incidents/IncidentView.aspx?IncidentId=" + IncidentID;
				if (Security.CurrentUser.IsExternal)
					sLink = "../External/ExternalIncident.aspx?IncidentId=" + IncidentID;
				Response.Redirect(sLink);
			}
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			string[] values = arg.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
			DataTable dt = null;
			ArrayList alUsers = new ArrayList();
			if (ViewState["ResponsiblePool"] != null)
				dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();
			try
			{
				for (int i = 0; i < values.Length; i++)
				{
					string sUser = values[i].Substring(0, values[i].IndexOf("*"));
					string temp = values[i].Substring(values[i].IndexOf("*") + 1);
					string sStatus = temp.Substring(0, temp.IndexOf("*"));
					string sIsNew = temp.Substring(temp.IndexOf("*") + 1);
					alUsers.Add(int.Parse(sUser, CultureInfo.InvariantCulture));
					DataRow[] dr_mas = dt.Select("PrincipalId=" + sUser);
					if (dr_mas.Length > 0)
					{
						dr_mas[0]["ResponsePending"] = (sStatus == "1");
						if (sIsNew == "1")
							dr_mas[0]["IsNew"] = true;
					}
					else
					{
						DataRow newRow = dt.NewRow();
						newRow["PrincipalId"] = int.Parse(sUser, CultureInfo.InvariantCulture);
						newRow["IsGroup"] = false;
						newRow["ResponsePending"] = (sStatus == "1");
						newRow["IsNew"] = true;
						dt.Rows.Add(newRow);
					}
				}
			}
			catch
			{
			}
			ArrayList alDeleted = new ArrayList();
			foreach (DataRow dr in dt.Rows)
				if (!alUsers.Contains((int)dr["PrincipalId"]))
					alDeleted.Add((int)dr["PrincipalId"]);
			foreach (int iDel in alDeleted)
			{
				DataRow[] dr_mas = dt.Select("PrincipalId = " + iDel);
				if (dr_mas.Length > 0)
					dt.Rows.Remove(dr_mas[0]);
			}

			if (IsAllDenied())
				ViewState["DDStatus"] = "0";
			else
				ViewState["DDStatus"] = "-1";

			ViewState["ResponsiblePool"] = dt.Copy();

			Incident.Tracking trk = Incident.GetTrackingInfo(IncidentID);

			CommandManager cm = CommandManager.GetCurrent(this.Page);

			if (ViewState["DDStatus"].ToString() == "0")
			{
				if (!Security.CurrentUser.IsExternal)
				{
					CommandParameters cp = null;
					if (!String.IsNullOrEmpty(Command))
						cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					else
						cp = new CommandParameters("MC_HDM_GroupResponsibility");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", arg);
					cp.AddCommandArgument("NotChangeKey", (!trk.CanSetGroup) ? "1" : "0");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					lblClient.Text = BuildLinkWithIconAndText("red_denied.gif", "tRespGroup", cmd);
				}
				else
					lblClient.Text = BuildIconAndText("red_denied.gif", "tRespGroup");
			}
			else
			{
				if (!Security.CurrentUser.IsExternal)
				{
					CommandParameters cp = null;
					if (!String.IsNullOrEmpty(Command))
						cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					else
						cp = new CommandParameters("MC_HDM_GroupResponsibility");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", arg);
					cp.AddCommandArgument("NotChangeKey", (!trk.CanSetGroup) ? "1" : "0");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					lblClient.Text = BuildLinkWithIconAndText("waiting.gif", "tRespGroup", cmd);
				}
				else
					lblClient.Text = BuildIconAndText("waiting.gif", "tRespGroup");
			}
		}
		#endregion

		#region btnRefreshMore_Click
		protected void btnRefreshMore_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			string[] values = arg.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length >= 2)
			{
				hidResp.Value = values[1];
				lblClient.Text = Util.CommonHelper.GetUserStatusUL(int.Parse(values[1], CultureInfo.InvariantCulture));
			}
		}
		#endregion

		#region IsAllDenied
		private bool IsAllDenied()
		{
			bool retVal = false;
			DataTable dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();
			foreach (DataRow dr in dt.Rows)
				if ((bool)dr["ResponsePending"])
				{
					retVal = true;
					break;
				}
			return !retVal;
		}
		#endregion

		#region BuildIconAndText
		private string BuildIconAndText(string iconFileName, string textResourceName)
		{
			return string.Format(CultureInfo.InvariantCulture,
				"<span><img alt=\"\" src=\"{0}\" /></span> <span>{1}</span>",
				Page.ResolveUrl("~/Layouts/Images/" + iconFileName),
				HttpUtility.HtmlEncode(LocRM.GetString(textResourceName)));
		} 
		#endregion

		#region BuildLinkWithIconAndText
		private string BuildLinkWithIconAndText(string iconFileName, string textResourceName, string javaScriptCommand)
		{
			return string.Format(CultureInfo.InvariantCulture,
				"<a href=\"{0}\"><img alt=\"\" src=\"{1}\" /> {2}</a>",
				HttpUtility.HtmlAttributeEncode("javascript:" + javaScriptCommand),
				Page.ResolveUrl("~/Layouts/Images/" + iconFileName),
				HttpUtility.HtmlEncode(LocRM.GetString(textResourceName)));
		} 
		#endregion

		#region ddIssueBox_SelectedIndexChanged
		protected void ddIssueBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UseNewResponsible.Visible = true;
			if (UseNewResponsible.Checked)
			{
				trMain.Visible = false;
				trAcceptDecline.Visible = false;
			}
			else
			{
				trMain.Visible = true;
				trAcceptDecline.Visible = true;
			}
		} 
		#endregion

		#region UseNewResponsible_CheckedChanged
		protected void UseNewResponsible_CheckedChanged(object sender, EventArgs e)
		{
			if (UseNewResponsible.Checked)
			{
				trMain.Visible = false;
				trAcceptDecline.Visible = false;
			}
			else
			{
				trMain.Visible = true;
				trAcceptDecline.Visible = true;
			}
		} 
		#endregion
	}
}
