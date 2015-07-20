using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Resources;
using System.Reflection;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.WebDAV.Common;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class IssueForumPostView : System.Web.UI.UserControl
	{
		protected int IncidentId
		{
			get
			{
				if (Request["IncidentID"] != null)
					return int.Parse(Request["IncidentID"]);
				else
					return -1;
			}
		}

		protected UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", Assembly.GetExecutingAssembly());
		private int WasFarwarded = 0;
		private int ForwardedEMail = -1;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IncidentId < 0)
				throw new Exception("IncidentId is not define!");

			if (!Page.IsPostBack)
			{
				BindSettings();
				dgForum.PagerStyle.NextPageText = String.Format("{1}&nbsp;<img width='16px' height='16px' border='0' align='absmiddle' src='{0}' />",
					ResolveClientUrl("~/Images/IbnFramework/right.gif"),
					GetGlobalResourceObject("IbnFramework.Incident", "NextPost").ToString());
				dgForum.PagerStyle.PrevPageText = String.Format("<img width='16px' height='16px' border='0' align='absmiddle' src='{0}' />&nbsp;{1}",
					ResolveClientUrl("~/Images/IbnFramework/left.gif"),
					GetGlobalResourceObject("IbnFramework.Incident", "PrevPost").ToString());
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			BindDataGrid();
			base.OnPreRender(e);
		}

		#region BindSettings
		private void BindSettings()
		{
			if (pc["IncForum_ReplyOutlook"] == null)
				pc["IncForum_ReplyOutlook"] = "0";
			if (pc["IncForum_ReplyEML"] == null)
				pc["IncForum_ReplyEML"] = "0";
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			DataTable dt = new DataTable();
			DataRow dr;
		
			#region DataTable
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Index", typeof(string)));
			dt.Columns.Add(new DataColumn("EMailMessageId", typeof(int)));
			dt.Columns.Add(new DataColumn("Sender", typeof(string)));
			dt.Columns.Add(new DataColumn("Message", typeof(string)));
			dt.Columns.Add(new DataColumn("SystemMessage", typeof(string)));
			dt.Columns.Add(new DataColumn("Created", typeof(string)));
			dt.Columns.Add(new DataColumn("CreationDate", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("Attachments", typeof(string)));
			dt.Columns.Add(new DataColumn("NodeType", typeof(string)));
			dt.Columns.Add(new DataColumn("CanMakeResolution", typeof(bool)));
			dt.Columns.Add(new DataColumn("CanMakeWorkaround", typeof(bool)));
			dt.Columns.Add(new DataColumn("CanUnMakeResolution", typeof(bool)));
			dt.Columns.Add(new DataColumn("CanUnMakeWorkaround", typeof(bool)));
			dt.Columns.Add(new DataColumn("CanReSend", typeof(bool)));
			dt.Columns.Add(new DataColumn("CanReSendOut", typeof(bool)));
			dt.Columns.Add(new DataColumn("CanReply", typeof(bool))); 
			#endregion

			IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(IncidentId));
			EMailRouterIncidentBoxBlock settings = IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock;
			bool allowEMailRouting = settings.AllowEMailRouting;
			bool CanUpdate = Incident.CanUpdate(IncidentId);

			int _index = 0;
			foreach (ForumThreadNodeInfo node in Incident.GetForumThreadNodes(IncidentId))
			{
				#region Define Node Type
				string typeName = "internal";
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(node.Id);
				if (coll[ForumThreadNodeSetting.Question] != null)
				{
					if (coll[ForumThreadNodeSetting.Incoming] != null)
						typeName = "Mail_incoming_quest";
					else if (coll[ForumThreadNodeSetting.Outgoing] != null)
						typeName = "Mail_outgoing_quest";
					else
						typeName = "internal_quest";
				}
				else if (coll[ForumThreadNodeSetting.Resolution] != null)
				{
					if (coll[ForumThreadNodeSetting.Incoming] != null)
						typeName = "Mail_incoming_resol";
					else if (coll[ForumThreadNodeSetting.Outgoing] != null)
						typeName = "Mail_outgoing_resol";
					else
						typeName = "internal_resol";
				}
				else if (coll[ForumThreadNodeSetting.Workaround] != null)
				{
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
				#endregion

				dr = dt.NewRow();
				
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
				if (coll[ForumThreadNodeSetting.Outgoing] != null && !Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
					dr["CanReSendOut"] = true;
				if (coll[ForumThreadNodeSetting.Incoming] != null && !Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				{
					dr["CanReply"] = true;
					dr["CanReSend"] = allowEMailRouting;
				}

				if (node.EMailMessageId <= 0)
					dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(node.CreatorId);

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
				dr["NodeType"] = String.Format("<img src='{2}/{0}.gif' width=24 height=16 align=absmiddle alt='{1}'>", 
					typeName, sNodeType, ResolveClientUrl("~/layouts/images/icons"));
				dr["SystemMessage"] = String.Empty;

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
							Mediachase.UI.Web.Util.CommonHelper.GetStateColorString(stateId),
							GetStatusName(stateId),
							LocRM.GetString("Status"));
					else
						sMessage += String.Format("{2}: <font color='{0}'><b>{1}</b></font>",
							Mediachase.UI.Web.Util.CommonHelper.GetStateColorString(stateId),
							GetStatusName(stateId),
							LocRM.GetString("Status"));
				}
				if (coll[IncidentForum.IssueEvent.Responsibility.ToString()] != null)
				{
					int iResp = int.Parse(coll[IncidentForum.IssueEvent.Responsibility.ToString()].ToString());
					string sResp = "";
					if (iResp == -2)
						sResp = String.Format("<img align='absmiddle' border='0' src='{0}' />&nbsp;<font color='#808080'><b>{1}</b></font>",
							ResolveClientUrl("~/layouts/images/not_set.png"),
							LocRM.GetString("tRespNotSet"));
					else if (iResp == -1)
						sResp = String.Format("<img align='absmiddle' border='0' src='{0}' />&nbsp;<font color='green'><b>{1}</b></font>",
							ResolveClientUrl("~/layouts/images/waiting.gif"),
							LocRM.GetString("tRespGroup"));
					else
						sResp = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(iResp);
					if (sMessage == "")
						sMessage = LocRM.GetString("tResponsible") + ": " + sResp;
					else
						sMessage += "<br />" + LocRM.GetString("tResponsible") + ": " + sResp;
				}
				if (WasFarwarded == 1 && node.EMailMessageId == ForwardedEMail)
					sMessage = "<font color='green'><b>" + LocRM.GetString("tWasResended") + "</b></font><br/>" + sMessage;
				if (WasFarwarded == -1 && node.EMailMessageId == ForwardedEMail)
					sMessage = "<font color='red'><b>" + LocRM.GetString("tWasNotResended") + "</b></font><br/>" + sMessage;

				dr["SystemMessage"] = (sMessage.Length == 0) ? String.Empty : sMessage;

				string sAttach = "";


				#region EmailMessage - Attachments
				if (node.EMailMessageId > 0)
				{
					EMailMessageInfo mi = null;
					try
					{
						mi = EMailMessageInfo.Load(node.EMailMessageId);
						int iUserId = User.GetUserByEmail(mi.SenderEmail);

						if (EMailClient.IsAlertSenderEmail(mi.SenderEmail))
							iUserId = node.CreatorId;

						if (iUserId > 0)
							dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(iUserId);
						else
						{
							Client client = Mediachase.IBN.Business.Common.GetClient(mi.SenderEmail);
							if (client != null)
							{
								if (client.IsContact)
									dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetContactLink(this.Page, client.Id, client.Name);
								else
									dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetOrganizationLink(this.Page, client.Id, client.Name);
							}
							else if (mi.SenderName != "")
								dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetEmailLink(mi.SenderEmail, mi.SenderName);
							else
								dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetEmailLink(mi.SenderEmail, mi.SenderEmail);
						}

						string sBody = String.Empty;
						if (mi.HtmlBody != null)
							sBody = mi.HtmlBody;

						dr["Message"] = sBody;

						// Attachments
						for (int i = 0; i < mi.Attachments.Length; i++)
						{
							AttachmentInfo ai = mi.Attachments[i];
							int id = DSFile.GetContentTypeByFileName(ai.FileName);
							string sIcon = "";
							if (id > 0)
							{
								sIcon = String.Format("<img align='absmiddle' border='0' src='{0}' />", ResolveClientUrl("~/Common/ContentIcon.aspx?IconID=" + id));
							}
                            //sAttach += String.Format("<nobr><a href='{0}'{2}>{1}</a></nobr> &nbsp;&nbsp;",
                            //  ResolveClientUrl("~/Incidents/EmailAttachDownload.aspx") + "?EMailId=" + node.EMailMessageId.ToString() + "&AttachmentIndex=" + i.ToString(),
                            //  sIcon + "&nbsp;" + ai.FileName,
                            //  Mediachase.IBN.Business.Common.OpenInNewWindow(ai.ContentType) ? " target='_blank'" : "");
                            sAttach += String.Format("<nobr><a href='{0}'{2}>{1}</a></nobr> &nbsp;&nbsp;", WebDavUrlBuilder.GetEmailAtachWebDavUrl(node.EMailMessageId, i, true),
                                                      sIcon + "&nbsp;" + ai.FileName, Mediachase.IBN.Business.Common.OpenInNewWindow(ai.ContentType) ? " target='_blank'" : "");
						}
					}
					catch
					{
						dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(-1);
						dr["Message"] = String.Format("<font color='red'><b>{0}</b>&nbsp;(#{1})</font>", LocRM.GetString("tNotFound"), node.EMailMessageId);
					}
				}
				else
				{

					string sBody = Mediachase.UI.Web.Util.CommonHelper.parsetext_br(node.Text, false);
					dr["Message"] = sBody;

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
								string sLink = Mediachase.UI.Web.Util.CommonHelper.GetAbsoluteDownloadFilePath(fl.Id, fl.Name, "FileLibrary", "ForumNodeId_" + node.Id);
								string sNameLocked = Mediachase.UI.Web.Util.CommonHelper.GetLockerText(sLink);

								sAttach += String.Format("<nobr><a href=\"{1}\"{3}><img src='{4}?IconID={2}' width='16' height='16' border=0 align=absmiddle> {0}</a> {5}</nobr>",
									fl.Name, 
									sLink, 
									fl.FileBinaryContentTypeId, 
									sTarget,
									ResolveClientUrl("~/Common/ContentIcon.aspx"),
									sNameLocked
								);
							}
						}
					}
				}

				dr["Attachments"] = sAttach; 
				#endregion

				if (sMessage.Length > 0 && dr["Message"].ToString() == "" && sAttach.Length == 0)
				{
					dr["NodeType"] = String.Format("<img src='{0}' width='16' height='16' align=absmiddle alt='{1}'>", 
						ResolveClientUrl("~/layouts/images/icons/info_message.gif"), 
						LocRM.GetString("NodeType_InfoMessage"));
					dr["CanMakeResolution"] = false;
					dr["CanMakeWorkaround"] = false;
					dr["CanUnMakeResolution"] = false;
					dr["CanUnMakeWorkaround"] = false;
				}

				if (dr["Message"].ToString().Trim() == "")
					dr["Message"] = "&nbsp;";
				dr["Index"] = "<table cellspacing='0' border='0' width='100%'><tr><td class='text' align='center' style='BACKGROUND-POSITION: center center; BACKGROUND-IMAGE: url(../../../layouts/images/atrisk1.gif); BACKGROUND-REPEAT: no-repeat;padding:4px;'><b>" + (++_index).ToString() + "</b></td></tr></table>";
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "CreationDate";

			dgForum.DataSource = dv;

			if (dv.Count == 0)
			{
				dgForum.Visible = false;
				dgForum.CurrentPageIndex = 0;
				divNoMess.Visible = true;

				divNoMess.InnerHtml = "<font color='red'>" + LocRM.GetString("NoMessages") + "</font>";
			}
			else
			{
				dgForum.Visible = true;
				divNoMess.Visible = false;

				int ppi = dv.Count / dgForum.PageSize;
				if (dv.Count % dgForum.PageSize == 0)
					ppi = ppi - 1;

				if (ViewState["IncidentForum_Page"] != null)
				{
					int iPageIndex = int.Parse(ViewState["IncidentForum_Page"].ToString());

					if (iPageIndex <= ppi)
						dgForum.CurrentPageIndex = iPageIndex;
					else
						dgForum.CurrentPageIndex = ppi;

					ViewState["IncidentForum_Page"] = dgForum.CurrentPageIndex.ToString();
				}
				else
					dgForum.CurrentPageIndex = ppi;
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
						string scommand = String.Format("{{ShowResizableWizard('{0}?IncidentId={1}&NodeId={2}&send=1&back={3}', 800, 600);return false;}}",
							ResolveClientUrl("~/Incidents/AddEMailMessage.aspx"),
							IncidentId,
							ib7.CommandArgument,
							HttpUtility.UrlEncode(this.Page.Request.RawUrl));
						ib7.Attributes.Add("onclick", scommand);
					}
				}
			}
		}
		#endregion

		#region GetReplyUrl
		protected string GetReplyUrl()
		{
			if (pc["IncForum_ReplyOutlook"] != null && pc["IncForum_ReplyOutlook"] == "1")
				return ResolveClientUrl("~/layouts/images/replymsg.gif");
			else if (pc["IncForum_ReplyEML"] != null && pc["IncForum_ReplyEML"] == "1")
				return ResolveClientUrl("~/layouts/images/replyeml.gif");
			else
				return ResolveClientUrl("~/layouts/images/reply.gif");
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
			dgForum.PageIndexChanged += new DataGridPageChangedEventHandler(dgForum_PageIndexChanged);
		}
		#endregion

		#region DataGrid events
		private void dgForum_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int nodeId = int.Parse(e.CommandArgument.ToString());
			Issue2.DeleteForumMessage(IncidentId, nodeId);
			//Response.Redirect(this.Page.Request.RawUrl, true);
		}

		private void dgForum_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			ViewState["IncidentForum_Page"] = e.NewPageIndex.ToString();
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
	}
}