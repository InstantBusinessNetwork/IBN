namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.IBN.Business.EMail;
	using ComponentArt.Web.UI;
    using Mediachase.IBN.Business.WebDAV.Common;
	using System.Globalization;

	/// <summary>
	///		Summary description for Forum.
	/// </summary>
	public partial class Forum : System.Web.UI.UserControl
	{
		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(Forum).Assembly);
		
		#region IncidentId
		private int IncidentId
		{
			get
			{
				int retval = -1;
				if (Request["IncidentId"] != null)
				{
					try
					{
						retval = int.Parse(Request["IncidentId"]);
					}
					catch { }
				}
				else if (Request["IssueId"] != null)
				{
					try
					{
						retval = int.Parse(Request["IssueId"]);
					}
					catch { }
				}
				return retval;
			}
		}
		#endregion

		private int WasFarwarded = 0;
		private int ForwardedEMail = -1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindSettings();
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindDataGrid();
			BindToolbar();
		}

		#region BindSettings
		private void BindSettings()
		{
			if (pc["IncForum_Sort458"] == null)
				pc["IncForum_Sort458"] = "CreationDate";
			if (pc["IncForum_Quests458"] == null)
				pc["IncForum_Quests458"] = "0";
			if (pc["IncForum_Resol458"] == null)
				pc["IncForum_Resol458"] = "0";
			if (pc["IncForum_Work458"] == null)
				pc["IncForum_Work458"] = "0";
			if (pc["IncForum_Info"] == null)
				pc["IncForum_Info"] = "1";
			if (pc["IncForum_ReplyOutlook"] == null)
				pc["IncForum_ReplyOutlook"] = "0";
			if (pc["IncForum_ReplyEML"] == null)
				pc["IncForum_ReplyEML"] = "0";
			if (pc["IncForum_ShowNums"] == null)
				pc["IncForum_ShowNums"] = "1";
			if (pc["IncForum_FullMess"] == null)
				pc["IncForum_FullMess"] = "0";
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("ForumMessages"));
			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM.GetString("tSettings");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			#region Sort By Date
			ComponentArt.Web.UI.MenuItem subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "date");
			subItem.Text = LocRM.GetString("tSort");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region --- Seperator ---
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "BreakItem";
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Questions on Top
			subItem = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_Quests458"] == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "quests");
			subItem.Text = LocRM.GetString("tQuests");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Resolutions on Top
			subItem = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_Resol458"] == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "resol");
			subItem.Text = LocRM.GetString("tResolutions");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Workarounds on Top
			subItem = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_Work458"] == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "work");
			subItem.Text = LocRM.GetString("tWorkarounds");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region --- Seperator ---
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "BreakItem";
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Show info messages
			subItem = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_Info"] == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "info");
			subItem.Text = LocRM.GetString("tShowInfo");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Show Numbers
			subItem = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_ShowNums"] == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "nums");
			subItem.Text = LocRM.GetString("tShowNums");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Full Message
			subItem = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_FullMess"] == "1")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "full");
			subItem.Text = LocRM.GetString("ShowFullMessage");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region --- Seperator ---
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.LookId = "BreakItem";
			topMenuItem.Items.Add(subItem);
			#endregion

			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Text = LocRM.GetString("tMenuReply");
			subItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
			subItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
			subItem.Look.RightIconWidth = Unit.Pixel(15);
			subItem.Look.RightIconHeight = Unit.Pixel(10);
			topMenuItem.Items.Add(subItem);

			#region Outlook Reply
			ComponentArt.Web.UI.MenuItem sub2Item = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_ReplyOutlook"] == "1")
			{
				sub2Item.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				sub2Item.Look.LeftIconWidth = Unit.Pixel(16);
				sub2Item.Look.LeftIconHeight = Unit.Pixel(16);
			}
			else
				sub2Item.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "reply");
			sub2Item.Text = LocRM.GetString("tOutlookReply");
			subItem.Items.Add(sub2Item);
			#endregion

			#region EML Reply
			sub2Item = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_ReplyEML"] == "1")
			{
				sub2Item.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				sub2Item.Look.LeftIconWidth = Unit.Pixel(16);
				sub2Item.Look.LeftIconHeight = Unit.Pixel(16);
			}
			else
				sub2Item.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "replyEML");
			sub2Item.Text = LocRM.GetString("tEMLReply");
			subItem.Items.Add(sub2Item);
			#endregion

			#region Web Reply
			sub2Item = new ComponentArt.Web.UI.MenuItem();
			if (pc["IncForum_ReplyEML"] == "0" && pc["IncForum_ReplyOutlook"] == "0")
			{
				sub2Item.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				sub2Item.Look.LeftIconWidth = Unit.Pixel(16);
				sub2Item.Look.LeftIconHeight = Unit.Pixel(16);
			}
			else
				sub2Item.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbSort, "replyWeb");
			sub2Item.Text = LocRM.GetString("tWebReply");
			subItem.Items.Add(sub2Item);
			#endregion

			secHeader.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			DataTable dtTop = new DataTable();
			DataRow dr;
			dtTop.Columns.Add(new DataColumn("Id", typeof(int)));
			dtTop.Columns.Add(new DataColumn("Index", typeof(string)));
			dtTop.Columns.Add(new DataColumn("EMailMessageId", typeof(int)));
			dtTop.Columns.Add(new DataColumn("Weight", typeof(int)));
			dtTop.Columns.Add(new DataColumn("Sender", typeof(string)));
			dtTop.Columns.Add(new DataColumn("Message", typeof(string)));
			dtTop.Columns.Add(new DataColumn("SystemMessage", typeof(string)));
			dtTop.Columns.Add(new DataColumn("Created", typeof(string)));
			dtTop.Columns.Add(new DataColumn("CreationDate", typeof(DateTime)));
			dtTop.Columns.Add(new DataColumn("Attachments", typeof(string)));
			dtTop.Columns.Add(new DataColumn("NodeType", typeof(string)));
			dtTop.Columns.Add(new DataColumn("CanMakeResolution", typeof(bool)));
			dtTop.Columns.Add(new DataColumn("CanMakeWorkaround", typeof(bool)));
			dtTop.Columns.Add(new DataColumn("CanUnMakeResolution", typeof(bool)));
			dtTop.Columns.Add(new DataColumn("CanUnMakeWorkaround", typeof(bool)));
			dtTop.Columns.Add(new DataColumn("CanReSend", typeof(bool)));
			dtTop.Columns.Add(new DataColumn("CanReSendOut", typeof(bool)));
			dtTop.Columns.Add(new DataColumn("CanReply", typeof(bool)));

			DataTable dt = dtTop.Clone();

			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IncidentId));
			EMailRouterIncidentBoxBlock settings = IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock;
			bool allowEMailRouting = settings.AllowEMailRouting;
			bool CanUpdate = Incident.CanUpdate(IncidentId);

			int _index = 0;
			foreach (ForumThreadNodeInfo node in Incident.GetForumThreadNodes(IncidentId))
			{
				bool fl_IsTop = false;
				int iWeight = 3;
				string typeName = "internal";
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(node.Id);
				if (coll[ForumThreadNodeSetting.Question] != null)
				{
					if (pc["IncForum_Quests458"] == "1")
					{
						fl_IsTop = true;
						iWeight = 0;
					}
					if (coll[ForumThreadNodeSetting.Incoming] != null)
						typeName = "Mail_incoming_quest";
					else if (coll[ForumThreadNodeSetting.Outgoing] != null)
						typeName = "Mail_outgoing_quest";
					else
						typeName = "internal_quest";
				}
				else if (coll[ForumThreadNodeSetting.Resolution] != null)
				{
					if (pc["IncForum_Resol458"] == "1")
					{
						fl_IsTop = true;
						iWeight = 1;
					}
					if (coll[ForumThreadNodeSetting.Incoming] != null)
						typeName = "Mail_incoming_resol";
					else if (coll[ForumThreadNodeSetting.Outgoing] != null)
						typeName = "Mail_outgoing_resol";
					else
						typeName = "internal_resol";
				}
				else if (coll[ForumThreadNodeSetting.Workaround] != null)
				{
					if (pc["IncForum_Work458"] == "1")
					{
						fl_IsTop = true;
						iWeight = 2;
					}
					if (coll[ForumThreadNodeSetting.Incoming] != null)
						typeName = "Mail_incoming_wa";
					else if (coll[ForumThreadNodeSetting.Outgoing] != null)
						typeName = "Mail_outgoing_wa";
					else
						typeName = "internal_wa";
				}
				else if (coll[ForumThreadNodeSetting.Incoming] != null)
					typeName = "Mail_incoming";
				else if (coll[ForumThreadNodeSetting.Outgoing] != null)
					typeName = "Mail_outgoing";

				if (fl_IsTop)
					dr = dtTop.NewRow();
				else
					dr = dt.NewRow();
				dr["Weight"] = iWeight;

				dr["Id"] = node.Id;
				dr["EMailMessageId"] = node.EMailMessageId;
				dr["CanMakeResolution"] = false;
				dr["CanMakeWorkaround"] = false;
				dr["CanUnMakeResolution"] = false;
				dr["CanUnMakeWorkaround"] = false;
				dr["CanReply"] = false;
				dr["CanReSend"] = false;
				dr["CanReSendOut"] = false;
				switch (typeName)
				{
					case "internal_resol":
					case "Mail_incoming_resol":
					case "Mail_outgoing_resol":
						dr["CanUnMakeResolution"] = true;
						break;
					case "internal_wa":
					case "Mail_incoming_wa":
					case "Mail_outgoing_wa":
						dr["CanMakeResolution"] = true;
						dr["CanUnMakeWorkaround"] = true;
						break;
					case "internal":
					case "Mail_incoming":
					case "Mail_outgoing":
						dr["CanMakeResolution"] = true;
						dr["CanMakeWorkaround"] = true;
						break;
					default:
						break;
				}
				if (coll[ForumThreadNodeSetting.Outgoing] != null && !Security.CurrentUser.IsExternal)
				{
					dr["CanReSendOut"] = true;
					///AK test
					//dr["CanReply"] = true;
					///
				}
				if (coll[ForumThreadNodeSetting.Incoming] != null && !Security.CurrentUser.IsExternal)
				{
					dr["CanReply"] = true;
					dr["CanReSend"] = allowEMailRouting;
				}

				if (node.EMailMessageId <= 0)
					dr["Sender"] = CommonHelper.GetUserStatus(node.CreatorId);

				dr["Created"] = node.Created.ToShortDateString() + " " + node.Created.ToShortTimeString();
				dr["CreationDate"] = node.Created;

				string sNodeType = typeName;
				try
				{
					sNodeType = LocRM.GetString(String.Format("NodeType_{0}", typeName));
				}
				catch
				{
				}

				dr["NodeType"] = String.Format("<img alt='{1}' src='../Layouts/Images/icons/{0}.gif'/>", typeName, sNodeType);
				dr["SystemMessage"] = "&nbsp;";

				string sMessage = "";

				if (coll[IncidentForum.IssueEvent.Declining.ToString()] != null)
				{
					sMessage = String.Format("<font color='red'><b>{0}</b></font>",
						LocRM.GetString("tUserDeclined"));
				}

				if (coll[IncidentForum.IssueEvent.State.ToString()] != null)
				{
					int stateId = int.Parse(coll[IncidentForum.IssueEvent.State.ToString()].ToString());
					if (sMessage == "")
						sMessage = String.Format("{2}: <font color='{0}'><b>{1}</b></font>",
							Util.CommonHelper.GetStateColorString(stateId),
							GetStatusName(stateId),
							LocRM.GetString("Status"));
					else
						sMessage += String.Format("{2}: <font color='{0}'><b>{1}</b></font>",
							Util.CommonHelper.GetStateColorString(stateId),
							GetStatusName(stateId),
							LocRM.GetString("Status"));
				}

				if (coll[IncidentForum.IssueEvent.Responsibility.ToString()] != null)
				{
					string sResp = "";

					int iResp = int.Parse(coll[IncidentForum.IssueEvent.Responsibility.ToString()].ToString());
					if (iResp == -2)
						sResp = String.Format("<img alt='' src='{0}'/> <span style='color:#808080;font-weight:bold'>{1}</span>",
							ResolveUrl("~/Layouts/Images/not_set.png"),
							LocRM.GetString("tRespNotSet"));
					else if (iResp == -1)
						sResp = String.Format("<img alt='' src='{0}'/> <span style='color:green;font-weight:bold'>{1}</span>",
							ResolveUrl("~/Layouts/Images/waiting.gif"),
							LocRM.GetString("tRespGroup"));
					else
						sResp = Util.CommonHelper.GetUserStatus(iResp);

					sResp = "<span class='IconAndText'><span>" + LocRM.GetString("tResponsible") + ":</span> " + sResp + "</span>";

					if (sMessage == "")
						sMessage = sResp;
					else
						sMessage += "<br/>" + sResp;
				}

				if (WasFarwarded == 1 && node.EMailMessageId == ForwardedEMail)
					sMessage = "<font color='green'><b>" + LocRM.GetString("tWasResended") + "</b></font><br/>" + sMessage;

				if (WasFarwarded == -1 && node.EMailMessageId == ForwardedEMail)
					sMessage = "<font color='red'><b>" + LocRM.GetString("tWasNotResended") + "</b></font><br/>" + sMessage;

				dr["SystemMessage"] = (sMessage.Length == 0) ? "&nbsp;" : sMessage;

				string sAttach = "";

				// EmailMessage
				if (node.EMailMessageId > 0)
				{
					EMailMessageInfo mi = null;
					try
					{
						mi = EMailMessageInfo.Load(node.EMailMessageId);
						int iUserId = User.GetUserByEmail(mi.SenderEmail);

						if (EMailClient.IsAlertSenderEmail(mi.SenderEmail))
							iUserId = node.CreatorId; //User.GetUserByEmail(mi.SenderEmail);            

						if (iUserId > 0)
							dr["Sender"] = Util.CommonHelper.GetUserStatus(iUserId);
						else
						{
							Client client = Mediachase.IBN.Business.Common.GetClient(mi.SenderEmail);
							if (client != null)
							{
								if (client.IsContact)
									dr["Sender"] = CommonHelper.GetContactLink(this.Page, client.Id, client.Name);
								else
									dr["Sender"] = CommonHelper.GetOrganizationLink(this.Page, client.Id, client.Name);
							}
							else if (mi.SenderName != "")
								dr["Sender"] = CommonHelper.GetEmailLink(mi.SenderEmail, mi.SenderName);
							else
								dr["Sender"] = CommonHelper.GetEmailLink(mi.SenderEmail, mi.SenderEmail);
						}

						string sBody = "";
						if (mi.HtmlBody != null)
						{
							int iMaxLen = 256;
							sBody = mi.HtmlBody;
							if (!Security.CurrentUser.IsExternal && pc["IncForum_FullMess"] != "1")
								sBody = EMailMessageInfo.CutHtmlBody(mi.HtmlBody, iMaxLen, "...");
						}

						if (pc["IncForum_FullMess"] != "1")
							dr["Message"] = String.Format("{0}<div class='text' style='text-align:right; font-weight:bold'><a href='javascript:OpenMessage({2},{3},false)'>{1}</a></div>", sBody, LocRM.GetString("More"), node.Id, node.EMailMessageId);
						else
							dr["Message"] = sBody;

						// Attachments
						for (int i = 0; i < mi.Attachments.Length; i++)
						{
							AttachmentInfo ai = mi.Attachments[i];
							int id = DSFile.GetContentTypeByFileName(ai.FileName);
							string sIcon = "";
							if (id > 0)
							{
								sIcon = String.Format("<img alt='' src='{0}'/>", ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + id));
							}
                            //sAttach += String.Format("<nobr><a href='{0}'{2}>{1}</a></nobr> &nbsp;&nbsp;",
                            //  ResolveUrl("~/Incidents/EmailAttachDownload.aspx") + "?EMailId=" + node.EMailMessageId.ToString() + "&AttachmentIndex=" + i.ToString(),
                            //  sIcon + "&nbsp;" + ai.FileName,
                            //  Mediachase.IBN.Business.Common.OpenInNewWindow(ai.ContentType) ? " target='_blank'" : "");
							sAttach += String.Format("<a href='{0}'{2} style='white-space:nowrap'>{1}</a> &nbsp;&nbsp;", WebDavUrlBuilder.GetEmailAtachWebDavUrl(node.EMailMessageId, i, true),
                                                    sIcon + "&nbsp;" + ai.FileName, Mediachase.IBN.Business.Common.OpenInNewWindow(ai.ContentType) ? " target='_blank'" : "");

						}
					}
					catch
					{
						dr["Sender"] = Util.CommonHelper.GetUserStatus(-1);
						dr["Message"] = String.Format("<font color='red'><b>{0}</b>&nbsp;(#{1})</font>", LocRM.GetString("tNotFound"), node.EMailMessageId);
					}
				}
				else
				{

					string sBody = CommonHelper.parsetext_br(node.Text, false);
					dr["Message"] = sBody;
					if (!Security.CurrentUser.IsExternal && pc["IncForum_FullMess"] != "1")
					{
						int iMaxLen = 300;
						string sBody1 = sBody;
						sBody = EMailMessageInfo.CutHtmlBody(sBody, iMaxLen, "...");

						if (!sBody.Equals(sBody1))
							dr["Message"] = String.Format("{0}<div class='text' style='text-align:right; font-weight:bold'><a href=\"javascript:OpenMessage({2},-1,true)\">{1}</a></div>", sBody, LocRM.GetString("More"), node.Id);
					}
					
					// Files
					if (node.ContentType == ForumStorage.NodeContentType.TextWithFiles)
					{
						FileInfo[] files = Incident.GetForumNodeFiles(node.Id);
						if (files.Length > 0)
						{
							foreach (FileInfo fl in files)
							{
								if (sAttach != "")
									sAttach += "&nbsp;&nbsp;&nbsp;";

								string sTarget = Mediachase.IBN.Business.Common.OpenInNewWindow(fl.FileBinaryContentType) ? " target='_blank'" : "";
								//string sLink = Util.CommonHelper.GetAbsolutePath(WebDavFileUserTicket.GetDownloadPath(fl.Id, fl.Name));
								string sLink = Util.CommonHelper.GetAbsoluteDownloadFilePath(fl.Id, fl.Name, "FileLibrary", "ForumNodeId_" + node.Id);
								string sNameLocked = CommonHelper.GetLockerText(sLink);
								sAttach += String.Format("<span style='white-space:nowrap'><a href=\"{1}\"{3}><img alt='' src='../Common/ContentIcon.aspx?IconID={2}'/> {0}</a> {4}</span>", fl.Name, sLink, fl.FileBinaryContentTypeId, sTarget, sNameLocked);
							}
						}
					}
				}

				dr["Attachments"] = sAttach;

				if (sMessage.Length > 0 && dr["Message"].ToString() == "" && sAttach.Length == 0)
				{
					if (pc["IncForum_Info"] == "0")
						continue;
					dr["NodeType"] = String.Format("<img src='../Layouts/Images/icons/info_message.gif' alt='{1}'/>", typeName, LocRM.GetString("NodeType_InfoMessage"));
					dr["CanMakeResolution"] = false;
					dr["CanMakeWorkaround"] = false;
					dr["CanUnMakeResolution"] = false;
					dr["CanUnMakeWorkaround"] = false;
				}

				if (dr["Message"].ToString().Trim() == "")
					dr["Message"] = "&nbsp;";
				if (pc["IncForum_ShowNums"] == "1")
					dr["Index"] = "<table cellspacing='0' cellpadding='4' border='0' width='100%'><tr><td class='text' align='center' style='background-position: center center; background-image: url(../Layouts/Images/atrisk1.gif); background-repeat: no-repeat'><b>" + (++_index).ToString() + "</b></td></tr></table>";
				else
					dr["Index"] = "&nbsp;";

				if (fl_IsTop)
					dtTop.Rows.Add(dr);
				else
					dt.Rows.Add(dr);
			}

			DataRow[] drtop = dtTop.Select("", "Weight, CreationDate");
			DataRow[] drother = dt.Select("", pc["IncForum_Sort458"]);

			DataTable result = dt.Clone();
			foreach (DataRow dr1 in drtop)
			{
				DataRow _dr = result.NewRow();
				_dr.ItemArray = (Object[])dr1.ItemArray.Clone();
				result.Rows.Add(_dr);
			}
			foreach (DataRow dr2 in drother)
			{
				DataRow _dr = result.NewRow();
				_dr.ItemArray = (Object[])dr2.ItemArray.Clone();
				result.Rows.Add(_dr);
			}

			DataView dv = result.DefaultView;
			dgForum.DataSource = dv;

			if (pc["IncidentForum_PageSize"] != null)
				dgForum.PageSize = int.Parse(pc["IncidentForum_PageSize"]);

			if (pc["IncidentForum_Page"] != null)
			{
				int iPageIndex = int.Parse(pc["IncidentForum_Page"]);
				int ppi = dv.Count / dgForum.PageSize;
				if (dv.Count % dgForum.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dgForum.CurrentPageIndex = iPageIndex;
				else
					dgForum.CurrentPageIndex = 0;

				pc["IncidentForum_Page"] = dgForum.CurrentPageIndex.ToString();
			}

			dgForum.DataBind();
			foreach (DataGridItem dgi in dgForum.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.ToolTip = LocRM.GetString("Delete");
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("WarningForumNode") + "')");
					ib.Visible = CanUpdate;
				}
				ImageButton ib1 = (ImageButton)dgi.FindControl("ibResolution");
				if (ib1 != null)
				{
					ib1.ToolTip = LocRM.GetString("tMarkAsResolution");
					ib1.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tMarkAsResolutionWarning") + "')");
				}
				ImageButton ib2 = (ImageButton)dgi.FindControl("ibWA");
				if (ib2 != null)
				{
					ib2.ToolTip = LocRM.GetString("tMarkAsWA");
					ib2.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tMarkAsWAWarning") + "')");
				}
				ImageButton ib3 = (ImageButton)dgi.FindControl("ibUnResolution");
				if (ib3 != null)
				{
					ib3.ToolTip = LocRM.GetString("tUnMarkAsResolution");
					ib3.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tUnMarkAsResolutionWarning") + "')");
				}
				ImageButton ib4 = (ImageButton)dgi.FindControl("ibUnWA");
				if (ib4 != null)
				{
					ib4.ToolTip = LocRM.GetString("tUnMarkAsWA");
					ib4.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tUnMarkAsWAWarning") + "')");
				}
				ImageButton ib5 = (ImageButton)dgi.FindControl("ibReSend");
				if (ib5 != null)
				{
					ib5.ToolTip = LocRM.GetString("tReSend");
					ib5.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tReSendWarning") + "')");
				}
				ImageButton ib6 = (ImageButton)dgi.FindControl("ibReSendOut");
				if (ib6 != null)
				{
					ib6.ToolTip = LocRM.GetString("tReSend");
					ib6.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tReSendOutWarning") + "')");
				}
				ImageButton ib7 = (ImageButton)dgi.FindControl("ibReply");
				if (ib7 != null)
				{
					ib7.ToolTip = LocRM.GetString("tReply");
					if (!allowEMailRouting || (pc["IncForum_ReplyOutlook"] == "0" && pc["IncForum_ReplyEML"] == "0"))
					{
						string command = string.Format(CultureInfo.InvariantCulture,
							"{{ShowResizableWizard('{0}?IncidentId={1}&NodeId={2}&send=1',800,600);return false;}}",
							Page.ResolveUrl("~/Incidents/AddEMailMessage.aspx"), IncidentId, ib7.CommandArgument);
						ib7.Attributes.Add("onclick", command);
					}
				}
			}

			if (dgForum.Items.Count == 0)
			{
				dgForum.Visible = false;
				divNoMess.Visible = true;

				divNoMess.InnerHtml = "<span style='color:red'>" + LocRM.GetString("NoMessages") + "</span>";
			}
			else
			{
				dgForum.Visible = true;
				divNoMess.Visible = false;
			}
		}
		#endregion

		protected string GetReplyUrl()
		{
			if (pc["IncForum_ReplyOutlook"] != null && pc["IncForum_ReplyOutlook"] == "1")
				return ResolveUrl("~/Layouts/Images/replymsg.gif");
			else if (pc["IncForum_ReplyEML"] != null && pc["IncForum_ReplyEML"] == "1")
				return ResolveUrl("~/Layouts/Images/replyeml.gif");
			else
				return ResolveUrl("~/Layouts/Images/reply.gif");
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
			dgForum.ItemCommand += new DataGridCommandEventHandler(dgForum_ItemCommand);
			dgForum.DeleteCommand += new DataGridCommandEventHandler(dgForum_DeleteCommand);
			dgForum.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgForum_PageSizeChanged);
			dgForum.PageIndexChanged += new DataGridPageChangedEventHandler(dgForum_PageIndexChanged);
			this.lbSort.Click += new EventHandler(lbSort_Click);
		}
		#endregion

		#region DataGrid events
		private void dgForum_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int nodeId = int.Parse(e.CommandArgument.ToString());
			Issue2.DeleteForumMessage(IncidentId, nodeId);
			Response.Redirect("../Incidents/IncidentView.aspx?IncidentId=" + IncidentId);
		}

		private void dgForum_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["IncidentForum_PageSize"] = e.NewPageSize.ToString();
		}

		private void dgForum_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["IncidentForum_Page"] = e.NewPageIndex.ToString();
		}

		private void dgForum_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Resolution")
			{
				int nodeId = int.Parse(e.CommandArgument.ToString());
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(nodeId);
				coll.Add(ForumThreadNodeSetting.Resolution, "1");

				if (coll[ForumThreadNodeSetting.Internal] == null &&
					coll[ForumThreadNodeSetting.Incoming] == null &&
					coll[ForumThreadNodeSetting.Outgoing] == null)
					coll.Add(ForumThreadNodeSetting.Internal, "1");
			}
			else if (e.CommandName == "Workaround")
			{
				int nodeId = int.Parse(e.CommandArgument.ToString());
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(nodeId);
				coll.Add(ForumThreadNodeSetting.Workaround, "1");

				if (coll[ForumThreadNodeSetting.Internal] == null &&
					coll[ForumThreadNodeSetting.Incoming] == null &&
					coll[ForumThreadNodeSetting.Outgoing] == null)
					coll.Add(ForumThreadNodeSetting.Internal, "1");
			}
			else if (e.CommandName == "UnResolution")
			{
				int nodeId = int.Parse(e.CommandArgument.ToString());
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(nodeId);
				if (coll[ForumThreadNodeSetting.Resolution] != null)
					coll.Remove(ForumThreadNodeSetting.Resolution);

				if (coll[ForumThreadNodeSetting.Internal] == null &&
					coll[ForumThreadNodeSetting.Incoming] == null &&
					coll[ForumThreadNodeSetting.Outgoing] == null)
					coll.Add(ForumThreadNodeSetting.Internal, "1");
			}
			else if (e.CommandName == "UnWorkaround")
			{
				int nodeId = int.Parse(e.CommandArgument.ToString());
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(nodeId);
				if (coll[ForumThreadNodeSetting.Workaround] != null)
					coll.Remove(ForumThreadNodeSetting.Workaround);

				if (coll[ForumThreadNodeSetting.Internal] == null &&
					coll[ForumThreadNodeSetting.Incoming] == null &&
					coll[ForumThreadNodeSetting.Outgoing] == null)
					coll.Add(ForumThreadNodeSetting.Internal, "1");
			}
			else if (e.CommandName == "ReSend")
			{
				int emailMessageId = int.Parse(e.CommandArgument.ToString());
				try
				{
					EMailRouterOutputMessage.Send(IncidentId, emailMessageId);
					WasFarwarded = 1;
					ForwardedEMail = emailMessageId;
				}
				catch
				{
					WasFarwarded = -1;
					ForwardedEMail = emailMessageId;
				}
			}
			else if (e.CommandName == "ReSendOut")
			{
				int emailMessageId = int.Parse(e.CommandArgument.ToString());
				try
				{
					EMailRouterOutputMessage.Send(IncidentId, emailMessageId);
					WasFarwarded = 1;
					ForwardedEMail = emailMessageId;
				}
				catch
				{
					WasFarwarded = -1;
					ForwardedEMail = emailMessageId;
				}
			}
			else if (e.CommandName == "Reply")
			{
				try
				{
					int emailMessageId = int.Parse(e.CommandArgument.ToString());

					if (pc["IncForum_ReplyOutlook"] == "1")
					{
						Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", "mail.msg"));
						HttpContext.Current.Response.ContentType = "application/msoutlook";

						MsgMessage.Open(IncidentId, emailMessageId,
							HttpContext.Current.Response.OutputStream);
					}
					else if (pc["IncForum_ReplyEML"] == "1")
					{
						Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", "mail.eml"));
						HttpContext.Current.Response.ContentType = "message/rfc822";
						MsgMessage.OpenEml(IncidentId, emailMessageId,
							HttpContext.Current.Response.OutputStream);
					}

					Response.End();
				}
				catch (Exception ex)
				{
					HttpContext.Current.Trace.Write(ex.Message);
				}
			}
		}
		#endregion

		#region lbSort_Click
		private void lbSort_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			if (arg == "date")
			{
				if (pc["IncForum_Sort458"] == "CreationDate DESC")
					pc["IncForum_Sort458"] = "CreationDate";
				else
					pc["IncForum_Sort458"] = "CreationDate DESC";
			}
			else if (arg == "quests")
			{
				if (pc["IncForum_Quests458"] == "1")
					pc["IncForum_Quests458"] = "0";
				else
				{
					pc["IncForum_Quests458"] = "1";
					//pc["IncForum_Resol458"] = "0";
					//pc["IncForum_Work458"] = "0";
				}
			}
			else if (arg == "resol")
			{
				if (pc["IncForum_Resol458"] == "1")
					pc["IncForum_Resol458"] = "0";
				else
				{
					//pc["IncForum_Quests458"] = "0";
					pc["IncForum_Resol458"] = "1";
					//pc["IncForum_Work458"] = "0";
				}
			}
			else if (arg == "work")
			{
				if (pc["IncForum_Work458"] == "1")
					pc["IncForum_Work458"] = "0";
				else
				{
					//pc["IncForum_Quests458"] = "0";
					//pc["IncForum_Resol458"] = "0";
					pc["IncForum_Work458"] = "1";
				}
			}
			else if (arg == "info")
			{
				if (pc["IncForum_Info"] == "1")
					pc["IncForum_Info"] = "0";
				else
					pc["IncForum_Info"] = "1";
			}
			else if (arg == "reply")
			{
				//if(pc["IncForum_ReplyOutlook"] == "1")
				//  pc["IncForum_ReplyOutlook"] = "0";
				//else
				//{
				pc["IncForum_ReplyOutlook"] = "1";
				pc["IncForum_ReplyEML"] = "0";
				//}
			}
			else if (arg == "replyEML")
			{
				//if(pc["IncForum_ReplyEML"] == "1")
				//  pc["IncForum_ReplyEML"] = "0";
				//else
				//{
				pc["IncForum_ReplyEML"] = "1";
				pc["IncForum_ReplyOutlook"] = "0";
				//}
			}
			else if (arg == "replyWeb")
			{
				pc["IncForum_ReplyEML"] = "0";
				pc["IncForum_ReplyOutlook"] = "0";
			}
			else if (arg == "nums")
			{
				if (pc["IncForum_ShowNums"] == "1")
					pc["IncForum_ShowNums"] = "0";
				else
					pc["IncForum_ShowNums"] = "1";
			}
			else if (arg == "full")
			{
				if (pc["IncForum_FullMess"] == "1")
					pc["IncForum_FullMess"] = "0";
				else
					pc["IncForum_FullMess"] = "1";
			}
		}
		#endregion

		#region CanVisible
		protected bool CanVisible(object message)
		{
			bool retVal = true;
			string sMessage = message.ToString();
			if (sMessage.Trim().Length == 0 || sMessage == "&nbsp;")
				retVal = false;
			return retVal;
		}
		#endregion

		#region GetStatusName
		private string GetStatusName(int StateId)
		{
			string retVal = "";
			using (IDataReader reader = Incident.GetIncidentState(StateId))
			{
				if (reader.Read())
					retVal = reader["StateName"].ToString();
			}
			return retVal;
		}
		#endregion
	}
}
