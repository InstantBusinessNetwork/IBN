namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;

	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.Ibn.Web.UI.WebControls;
	using System.Reflection;


	/// <summary>
	///		Summary description for EMailIssueBoxView.
	/// </summary>
	public partial class EMailIssueBoxView : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(EMailIssueBoxView).Assembly);



		//ddCreator

		#region IssBoxId
		protected int IssBoxId
		{
			get
			{
				if (Request["IssBoxId"] != null)
					return int.Parse(Request["IssBoxId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			hlAllowAutoReplyMessageLink.NavigateUrl = "~/Admin/EditTemplate.aspx?Language=" + CultureInfo.CurrentCulture.ToString() + "&Template=Issue_Created|Resource";
			hlAllowAutoReplyCloseMessageLink.NavigateUrl = "~/Admin/EmailIssueBoxTemplateEdit.aspx?Language=" + CultureInfo.CurrentCulture.ToString() + "&Template=Issue_Closed|Resource";

			ApplyLocalization();
			lblDuplicate.Visible = false;
			if (!Page.IsPostBack)
			{
				ViewState["IncidentBoxDocument"] = IncidentBoxDocument.Load(IssBoxId).GetDocumentString();
				BindLists();
				BindValues();
			}

			BindToolbars();

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_AdminResponsiblePool");
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			bool boxIsEnabled = (EMailRouterPop3Box.ListInternal() != null);
			divNoIntBox.Visible = !boxIsEnabled;
			cbAllowEMail.Enabled = cbAllowEMail.Checked || boxIsEnabled;

			tr2.Visible = cbAllowEMail.Checked;
			tr21.Visible = cbAllowEMail.Checked;
			tr3.Visible = cbAllowEMail.Checked;
			CheckEMR();

			trController1.Visible = cbAllowControl.Checked;
			trController.Visible = (cbAllowControl.Checked && (int)ControllerAssignType.CustomUser == int.Parse(ddContType.SelectedValue));
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			imbSave.Text = LocRM.GetString("tSave");
			imbSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			imbCancel.Text = LocRM.GetString("tCancel");
			imbCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			cbAllowAddRes.Text = "&nbsp;" + LocRM.GetString("tAllowAddResources");
			cbAllowAddToDo.Text = "&nbsp;" + LocRM.GetString("tAllowAddToDo");
			cbAllowControl.Text = "&nbsp;" + LocRM.GetString("tAllowControl");
			cbAllowToComeResp.Text = "&nbsp;" + LocRM.GetString("tAllowUserToComeResponsible");
			cbAllowToDeclineResp.Text = "&nbsp;" + LocRM.GetString("tAllowToDeclineResponsibility");
			cbAllowToReassignResp.Text = "&nbsp;" + LocRM.GetString("tAllowToReassignResponsibility");
			cbAllowEMail.Text = "&nbsp;" + LocRM.GetString("tAllowEMailRouter");
			cbReassignResp.Text = "&nbsp;" + LocRM.GetString("tReassignResponsible");
			cbAllowAutoReply.Text = "&nbsp;" + LocRM.GetString("tAllowAutoReply");
			cbAllowAutoReplyClose.Text = "&nbsp;" + LocRM.GetString("tAllowAutoReplyClose");
			hlAllowAutoReplyMessageLink.Text = LocRM.GetString("tAutoReplyLinkText") + "&nbsp;<img border='0' align='bottom' title='' width='16px' height='16px' src='" + this.Page.ResolveUrl("~/Layouts/Images/EDIT.GIF") + "'>";
			hlAllowAutoReplyMessageLink.NavigateUrl = string.Format("{0}?Language=", this.Page.ResolveUrl("~/Admin/EmailIssueBoxTemplateEdit.aspx")) + CultureInfo.CurrentCulture.ToString() + "&Template=Issue_Created|Resource" + "&IncidentBoxId=" + IssBoxId.ToString();

			hlAllowAutoReplyCloseMessageLink.Text = LocRM.GetString("tAutoReplyLinkText") + "&nbsp;<img border='0' align='bottom' title='' width='16px' height='16px' src='" + this.Page.ResolveUrl("~/Layouts/Images/EDIT.GIF") + "'>";
			hlAllowAutoReplyCloseMessageLink.NavigateUrl = string.Format("{0}?Language=", this.Page.ResolveUrl("~/Admin/EmailIssueBoxTemplateEdit.aspx")) + CultureInfo.CurrentCulture.ToString() + "&Template=Issue_Closed|Resource" + "&IncidentBoxId=" + IssBoxId.ToString();

			hlAllowAutoSigningLink.Text = LocRM.GetString("tAutoReplyLinkText") + "&nbsp;<img border='0' align='bottom' title='' width='16px' height='16px' src='" + this.Page.ResolveUrl("~/Layouts/Images/EDIT.GIF") + "'>";
			hlAllowAutoSigningLink.NavigateUrl = string.Format("{0}?Language=", this.Page.ResolveUrl("~/Admin/EmailSignatureEdit.aspx")) + CultureInfo.CurrentCulture.ToString() + "&Template=Issue_ResponseSignature" + "&IncidentBoxId=" + IssBoxId.ToString();
			cbIsDefault.Text = "&nbsp;" + LocRM.GetString("tDefaultBox");
			cbAllowAutoSigning.Text = "&nbsp;" + LocRM.GetString("tAllowMessageAutoSigning");

		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			ddRespType.Items.Clear();
			foreach (ResponsibleAssignType _type in Enum.GetValues(typeof(ResponsibleAssignType)))
				ddRespType.Items.Add(new ListItem(LocRM.GetString(_type.ToString()), ((int)_type).ToString()));

			ddContType.Items.Clear();
			foreach (ControllerAssignType _type in Enum.GetValues(typeof(ControllerAssignType)))
				ddContType.Items.Add(new ListItem(LocRM.GetString(_type.ToString()), ((int)_type).ToString()));

			ddExtActionType.Items.Clear();
			foreach (ExternalEMailActionType _type in Enum.GetValues(typeof(ExternalEMailActionType)))
				ddExtActionType.Items.Add(new ListItem(_type.ToString(), ((int)_type).ToString()));

			ddIntActionType.Items.Clear();
			foreach (InternalEMailActionType _type in Enum.GetValues(typeof(InternalEMailActionType)))
				ddIntActionType.Items.Add(new ListItem(_type.ToString(), ((int)_type).ToString()));

			//			ddCreator.Items.Clear();
			//			ddCreator.DataSource = User.GetListActive();
			//			ddCreator.DataValueField = "PrincipalId";
			//			ddCreator.DataTextField = "DisplayName";
			//			ddCreator.DataBind();

			ddManager.Items.Clear();
			ddManager.DataSource = User.GetListActive();
			ddManager.DataValueField = "PrincipalId";
			ddManager.DataTextField = "DisplayName";
			ddManager.DataBind();

			ddCalendar.Items.Clear();
			if (Configuration.ProjectManagementEnabled)
			{
				ddCalendar.DataSource = Project.GetListCalendars(0);
				ddCalendar.DataTextField = "CalendarName";
				ddCalendar.DataValueField = "CalendarId";
				ddCalendar.DataBind();
			}
			else
			{
				ddCalendar.Items.Add(new ListItem("no calendar", "0"));
				trCalendar.Visible = false;
			}

			ddController.Items.Clear();
			ddController.DataSource = User.GetListActive();
			ddController.DataValueField = "PrincipalId";
			ddController.DataTextField = "DisplayName";
			ddController.DataBind();

			ddResponsible.Items.Clear();
			ddResponsible.DataSource = User.GetListActive();
			ddResponsible.DataValueField = "PrincipalId";
			ddResponsible.DataTextField = "DisplayName";
			ddResponsible.DataBind();
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			IncidentBox ib = IncidentBox.Load(IssBoxId);
			lblIssBoxName.Text = ib.Name;
			tbMask.Text = ib.IdentifierMask;
			cbIsDefault.Checked = ib.IsDefault;

			IncidentBoxDocument ibd = null;
			if (ViewState["IncidentBoxDocument"] != null)
				ibd = IncidentBoxDocument.Load(IssBoxId, (string)ViewState["IncidentBoxDocument"]);
			else
				ibd = IncidentBoxDocument.Load(IssBoxId);

			//EMailRouterBlock
			EMailRouterIncidentBoxBlock eribb = ibd.EMailRouterBlock;

			cbAllowEMail.Checked = eribb.AllowEMailRouting;
			cbAllowAutoReply.Checked = eribb.SendAutoReply;
			cbAllowAutoReplyClose.Checked = eribb.SendAutoIncidentClosed;

			if (ibd != null)
			{
				cbAllowAutoSigning.Checked = ibd.GeneralBlock.AllowOutgoingEmailFormat;
			}

			Util.CommonHelper.SafeSelect(ddExtActionType, ((int)eribb.IncomingEMailAction).ToString());
			Util.CommonHelper.SafeSelect(ddIntActionType, ((int)eribb.OutgoingEMailAction).ToString());

			BindReipients(eribb);

			//GeneralBlock
			GeneralIncidentBoxBlock gibb = ibd.GeneralBlock;

			//Util.CommonHelper.SafeSelect(ddCreator, gibb.DefaultCreator.ToString());
			Util.CommonHelper.SafeSelect(ddManager, gibb.Manager.ToString());
			Util.CommonHelper.SafeSelect(ddCalendar, gibb.CalendarId.ToString());

			Util.CommonHelper.SafeSelect(ddContType, ((int)gibb.ControllerAssignType).ToString());
			if (gibb.ControllerAssignType == ControllerAssignType.CustomUser)
			{
				trController.Visible = true;
				Util.CommonHelper.SafeSelect(ddController, gibb.Controller.ToString());
			}
			else
				trController.Visible = false;

			Util.CommonHelper.SafeSelect(ddRespType, ((int)gibb.ResponsibleAssignType).ToString());
			if (gibb.ResponsibleAssignType == ResponsibleAssignType.CustomUser)
			{
				//				lblForResp.Text = LocRM.GetString("tResponsible") + ":";
				//				ddResponsible.Visible = true;
				//				lblResponsible.Visible = false;
				//				lblChangeButton.Visible = false;
				trCustomUser.Visible = true;
				Util.CommonHelper.SafeSelect(ddResponsible, gibb.Responsible.ToString());

				// [2008-09-09] O.R.: Sometimes we can have inactive or deleted user
				if (ddResponsible.SelectedValue != gibb.Responsible.ToString())
				{
					ddResponsible.Items.Insert(0, new ListItem("[ " + LocRM.GetString("tNotSet") + " ]", "-1"));
					Util.CommonHelper.SafeSelect(ddResponsible, "-1");
				}
			}
			else
			{
				trCustomUser.Visible = false;
			}

			BindResponsible(gibb);

			cbAllowAddRes.Checked = gibb.AllowAddResources;
			cbAllowAddToDo.Checked = gibb.AllowAddToDo;
			cbAllowControl.Checked = gibb.AllowControl;
			cbAllowToDeclineResp.Checked = gibb.AllowToDeclineResponsibility;
			cbAllowToReassignResp.Checked = gibb.AllowToReassignResponsibility;
			cbAllowToComeResp.Checked = gibb.AllowUserToComeResponsible;
			cbReassignResp.Checked = gibb.ReassignResponsibileOnReOpen;

			ucDuration.Value = DateTime.MinValue.AddMinutes(gibb.ExpectedDuration);
			ucResponseTime.Value = DateTime.MinValue.AddMinutes(gibb.ExpectedResponseTime);
			ucAssignTime.Value = DateTime.MinValue.AddMinutes(gibb.ExpectedAssignTime);
			ucTaskTime.Value = DateTime.MinValue.AddMinutes(gibb.TaskTime);
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tIssueboxSettings");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tIncComSet"), this.Page.ResolveUrl("~/Admin/HDMSettings.aspx"));

			bh1.AddText(LocRM.GetString("tIssManage"));

			bh2.AddText(LocRM.GetString("tFlags"));

			bh3.AddText(LocRM.GetString("tIssueControl"));

			bh4.AddText(LocRM.GetString("tEMailRouterSettings"));

			bh5.AddText(LocRM.GetString("tNewMessage"));
		}
		#endregion

		#region BindResponsible
		private void BindResponsible(GeneralIncidentBoxBlock gibb)
		{
			//			ddResponsible.Visible = false;
			//			lblResponsible.Visible = true;
			//			lblChangeButton.Visible = true;
			//			lblResponsible.Text = "";
			//			lblChangeButton.Text = "";
			lblForResp.Text = LocRM.GetString("tPool") + ":";
			string sResps = "";
			string sUsers = "";
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("PureName", typeof(string)));
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));
			DataRow dr;
			if (gibb.ResponsiblePool != null)
				foreach (int iUserId in gibb.ResponsiblePool)
				{
					sUsers += iUserId + "_";
					dr = dt.NewRow();
					dr["PureName"] = Util.CommonHelper.GetUserStatusPureName(iUserId);
					dr["UserName"] = Util.CommonHelper.GetUserStatus(iUserId);
					dt.Rows.Add(dr);
				}
			DataView dv = dt.DefaultView;
			dv.Sort = "PureName";
			foreach (DataRowView drv in dv)
				sResps += drv["UserName"].ToString() + "<br />";

			if (sResps.Length == 0)
				sResps = "&nbsp;";
			lblResponsible.Text = String.Format("<span class='text' style='width:180px;'>{0}</span>&nbsp;&nbsp;",
				sResps);

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_HDM_AdminResponsiblePool");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("sUsersKey", sUsers);
			string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
			cmd = cmd.Replace("\"", "&quot;");

			lblChangeButton.Text = String.Format("<a href=\"javascript:{{{2}}}\"><img align='absmiddle' border='0' src='{0}' />&nbsp;{1}</a>",
				ResolveClientUrl("~/layouts/images/icons/regular.gif"),
				LocRM.GetString("tChange"), cmd);
		}
		#endregion

		#region BindReipients
		private void BindReipients(EMailRouterIncidentBoxBlock eribb)
		{
			lblRecipients.Text = "";
			lblChangeButton2.Text = "";
			string sResps = "";
			string sUsers = "";

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("PureName", typeof(string)));
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));
			DataRow dr;
			if (eribb.InformationRecipientList != null)
				foreach (int iUserId in eribb.InformationRecipientList)
				{
					sUsers += iUserId + "_";
					dr = dt.NewRow();
					dr["PureName"] = Util.CommonHelper.GetUserStatusPureName(iUserId);
					dr["UserName"] = Util.CommonHelper.GetUserStatus(iUserId);
					dt.Rows.Add(dr);
				}
			DataView dv = dt.DefaultView;
			dv.Sort = "PureName";
			foreach (DataRowView drv in dv)
				sResps += drv["UserName"].ToString() + "<br />";

			if (sResps.Length == 0)
				sResps = "&nbsp;";
			lblRecipients.Text = String.Format("<span class='text' style='width:180px;'>{0}</span>&nbsp;&nbsp;",
				sResps);

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_HDM_AdminResponsiblePool2");
			cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
			cp.AddCommandArgument("sUsersKey", sUsers);
			string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
			cmd = cmd.Replace("\"", "&quot;");

			lblChangeButton2.Text = String.Format("<a href=\"javascript:{{{2}}}\"><img align='absmiddle' border='0' src='{0}' />&nbsp;{1}</a>",
				this.Page.ResolveUrl("~/layouts/images/icons/regular.gif"),
				LocRM.GetString("tChange"), cmd);
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
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
			this.imbCancel.ServerClick += new EventHandler(imbCancel_ServerClick);
			this.ddRespType.SelectedIndexChanged += new EventHandler(ddRespType_SelectedIndexChanged);
			this.ddContType.SelectedIndexChanged += new EventHandler(ddContType_SelectedIndexChanged);
			btnRefresh.Click += new EventHandler(btnRefresh_Click);
			btnRefresh2.Click += new EventHandler(btnRefresh2_Click);
			this.cbAllowEMail.CheckedChanged += new EventHandler(cbAllowEMail_CheckedChanged);
			this.cbAllowControl.CheckedChanged += new EventHandler(cbAllowControl_CheckedChanged);
		}
		#endregion

		#region Save-Cancel
		private void imbCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Admin/HDMSettings.aspx");
		}

		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			TimeSpan ts;
			if (ViewState["IncidentBoxDocument"] != null)
			{
				try
				{
					IncidentBox ib = IncidentBox.Load(IssBoxId);
					if (ib != null)
					{
						ib.Name = lblIssBoxName.Text.Trim();
						ib.IdentifierMask = tbMask.Text.Trim();
						ib.IsDefault = cbIsDefault.Checked;
					}

					IncidentBoxDocument ibd = IncidentBoxDocument.Load(IssBoxId, (string)ViewState["IncidentBoxDocument"]);

					//GeneralBlock
					GeneralIncidentBoxBlock gibb = ibd.GeneralBlock;

					//gibb.DefaultCreator = int.Parse(ddCreator.SelectedValue);
					gibb.Manager = int.Parse(ddManager.SelectedValue);
					gibb.CalendarId = int.Parse(ddCalendar.SelectedValue);
					gibb.ControllerAssignType = (ControllerAssignType)(int.Parse(ddContType.SelectedValue));
					if (gibb.ControllerAssignType == ControllerAssignType.CustomUser)
						gibb.Controller = int.Parse(ddController.SelectedValue);
					gibb.ResponsibleAssignType = (ResponsibleAssignType)(int.Parse(ddRespType.SelectedValue));
					if (gibb.ResponsibleAssignType == ResponsibleAssignType.CustomUser)
						gibb.Responsible = int.Parse(ddResponsible.SelectedValue);

					gibb.AllowAddResources = cbAllowAddRes.Checked;
					gibb.AllowAddToDo = cbAllowAddToDo.Checked;
					gibb.AllowControl = cbAllowControl.Checked;
					gibb.AllowToDeclineResponsibility = cbAllowToDeclineResp.Checked;
					gibb.AllowToReassignResponsibility = cbAllowToReassignResp.Checked;
					gibb.AllowUserToComeResponsible = cbAllowToComeResp.Checked;
					gibb.ReassignResponsibileOnReOpen = cbReassignResp.Checked;
					gibb.AllowOutgoingEmailFormat = cbAllowAutoSigning.Checked;

					ts = new TimeSpan(ucDuration.Value.Ticks);
					gibb.ExpectedDuration = (int)ts.TotalMinutes;

					ts = new TimeSpan(ucResponseTime.Value.Ticks);
					gibb.ExpectedResponseTime = (int)ts.TotalMinutes;

					ts = new TimeSpan(ucAssignTime.Value.Ticks);
					gibb.ExpectedAssignTime = (int)ts.TotalMinutes;

					ts = new TimeSpan(ucTaskTime.Value.Ticks);
					gibb.TaskTime = (int)ts.TotalMinutes;

					//EMailRouterBlock
					EMailRouterIncidentBoxBlock eribb = ibd.EMailRouterBlock;

					eribb.AllowEMailRouting = cbAllowEMail.Checked;
					eribb.IncomingEMailAction = (ExternalEMailActionType)(int.Parse(ddExtActionType.SelectedValue));
					eribb.OutgoingEMailAction = (InternalEMailActionType)(int.Parse(ddIntActionType.SelectedValue));
					eribb.SendAutoReply = cbAllowAutoReply.Checked;
					eribb.SendAutoIncidentClosed = cbAllowAutoReplyClose.Checked;

					//IncidentBoxDocument.Save(ibd);
					IncidentBox.Update(ib, ibd);
					Response.Redirect("~/Admin/HDMSettings.aspx");
				}
				catch (IncidentBoxDuplicateNameException)
				{
					lblDuplicate.Text = LocRM.GetString("tDuplicateName");
					lblDuplicate.Visible = true;
				}
				catch (IncidentBoxDuplicateIdentifierMaskException)
				{
					lblDuplicate.Text = LocRM.GetString("tDuplicateMask");
					lblDuplicate.Visible = true;
				}
			}
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			IncidentBoxDocument ibd = null;
			if (ViewState["IncidentBoxDocument"] != null)
				ibd = IncidentBoxDocument.Load(IssBoxId, (string)ViewState["IncidentBoxDocument"]);
			else
				ibd = IncidentBoxDocument.Load(IssBoxId);

			ibd.GeneralBlock.ResponsiblePool = GetUsers();

			BindResponsible(ibd.GeneralBlock);

			ViewState["IncidentBoxDocument"] = ibd.GetDocumentString();
		}
		#endregion

		#region btnRefresh2_Click
		protected void btnRefresh2_Click(object sender, EventArgs e)
		{
			IncidentBoxDocument ibd = null;
			if (ViewState["IncidentBoxDocument"] != null)
				ibd = IncidentBoxDocument.Load(IssBoxId, (string)ViewState["IncidentBoxDocument"]);
			else
				ibd = IncidentBoxDocument.Load(IssBoxId);


			ibd.EMailRouterBlock.InformationRecipientList = GetUsers();

			BindReipients(ibd.EMailRouterBlock);

			ViewState["IncidentBoxDocument"] = ibd.GetDocumentString();
		}
		#endregion

		#region ddRespType_SelectedIndexChanged
		private void ddRespType_SelectedIndexChanged(object sender, EventArgs e)
		{
			IncidentBoxDocument ibd = null;
			if (ViewState["IncidentBoxDocument"] != null)
				ibd = IncidentBoxDocument.Load(IssBoxId, (string)ViewState["IncidentBoxDocument"]);
			else
				ibd = IncidentBoxDocument.Load(IssBoxId);

			GeneralIncidentBoxBlock gibb = ibd.GeneralBlock;

			if ((int)ResponsibleAssignType.CustomUser == int.Parse(ddRespType.SelectedValue))
			{
				//				lblForResp.Text = LocRM.GetString("tResponsible") + ":";
				//				ddResponsible.Visible = true;
				//				lblResponsible.Visible = false;
				//				lblChangeButton.Visible = false;
				trCustomUser.Visible = true;
				Util.CommonHelper.SafeSelect(ddResponsible, gibb.Responsible.ToString());
			}
			else
				trCustomUser.Visible = false;

			BindResponsible(gibb);

			ViewState["IncidentBoxDocument"] = ibd.GetDocumentString();
		}
		#endregion

		#region ddContType_SelectedIndexChanged
		private void ddContType_SelectedIndexChanged(object sender, EventArgs e)
		{
			IncidentBoxDocument ibd = null;
			if (ViewState["IncidentBoxDocument"] != null)
				ibd = IncidentBoxDocument.Load(IssBoxId, (string)ViewState["IncidentBoxDocument"]);
			else
				ibd = IncidentBoxDocument.Load(IssBoxId);

			GeneralIncidentBoxBlock gibb = ibd.GeneralBlock;

			if ((int)ControllerAssignType.CustomUser == int.Parse(ddContType.SelectedValue))
				trController.Visible = true;
			else
				trController.Visible = false;

			ViewState["IncidentBoxDocument"] = ibd.GetDocumentString();
		}
		#endregion

		#region GetUsers
		private ArrayList GetUsers()
		{
			string arg = Request["__EVENTARGUMENT"];
			string[] values = arg.Split('_');
			ArrayList alUsers = new ArrayList();
			try
			{
				for (int i = 0; i < values.Length; i++)
					alUsers.Add(int.Parse(values[i]));
			}
			catch
			{
			}
			return alUsers;
		}
		#endregion

		#region CheckEMR
		private void CheckEMR()
		{
			if (cbAllowEMail.Checked)
			{
				ListItem liItem = ddRespType.Items.FindByValue(((int)ResponsibleAssignType.Manual).ToString());
				if (liItem != null)
				{
					if (liItem.Selected)
						Util.CommonHelper.SafeSelect(ddRespType, ((int)ResponsibleAssignType.Pool).ToString());
					ddRespType.Items.Remove(liItem);
				}
			}
			else
			{
				ListItem liItem = ddRespType.Items.FindByValue(((int)ResponsibleAssignType.Manual).ToString());
				if (liItem == null)
					ddRespType.Items.Add(new ListItem(LocRM.GetString(ResponsibleAssignType.Manual.ToString()), ((int)ResponsibleAssignType.Manual).ToString()));
			}
		}
		#endregion

		#region cbAllowEMail_CheckedChanged
		private void cbAllowEMail_CheckedChanged(object sender, EventArgs e)
		{
		}
		#endregion

		#region cbAllowControl_CheckedChanged
		private void cbAllowControl_CheckedChanged(object sender, EventArgs e)
		{
		}
		#endregion

		#region ResponsibleValidator_ServerValidate
		protected void ResponsibleValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (trCustomUser.Visible && ddResponsible.SelectedValue == "-1")
				args.IsValid = false;
		}
		#endregion
	}
}
