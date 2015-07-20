using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Mediachase.IBN.Business.EMail;
using Mediachase.Ibn.Forum;
using Mediachase.Forum;

namespace Mediachase.IBN.Business.ForumHandlers
{
  public class NodeActions : Mediachase.IBN.Business.ICommandHandler
  {
    #region ICommandHandler Members

    public void Init(object Sender)
    {
      ((BaseCommandManager)Sender).InvokeCommand += new CommandHandler(NodeActions_InvokeCommand);
      ((BaseCommandManager)Sender).Validate += new ValidateHandler(NodeActions_Validate);
		}


		#region NodeActions_Validate
		void NodeActions_Validate(object Sender, ValidateArgs e)
    {
			if (e.Node == null)
				return;

			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

      int iObjTypeId = -1;
      int iObjId = -1;
      if(HttpContext.Current.Items["ObjectTypeId"]!=null)
        iObjTypeId = (int)HttpContext.Current.Items["ObjectTypeId"];
      if(HttpContext.Current.Items["ObjectId"]!=null)
        iObjId = (int)HttpContext.Current.Items["ObjectId"];

			switch (e.CommandUid)
			{
				#region Reply
				case "Reply":
					if (e.Node.Type == "email" && e.Node.Attributes.ContainsKey("Incoming") && pc["IncForum_ReplyOutlook"] != "1" && pc["IncForum_ReplyEML"] != "1")
					{
						e.IsEnabled = true;
						e.IsVisible = true;
					}
					break;
				#endregion
				#region ReplyOutlook
				case "ReplyOutlook":
					if (e.Node.Type == "email" && e.Node.Attributes.ContainsKey("Incoming") && pc["IncForum_ReplyOutlook"] == "1")
					{
						e.IsEnabled = true;
						e.IsVisible = true;
					}
					break;
				#endregion
				#region ReplyEML
				case "ReplyEML":
					if (e.Node.Type == "email" && e.Node.Attributes.ContainsKey("Incoming") && pc["IncForum_ReplyEML"] != "1")
					{
						e.IsEnabled = true;
						e.IsVisible = true;
					}
					break;
				#endregion
				#region ReSend
				case "ReSend":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						if (e.Node.Type == "email")
						{
							if (e.Node.Attributes.ContainsKey("Incoming"))
							{
								IncidentBox incidentBox = IncidentBox.Load(Incident.GetIncidentBox(iObjId));
								EMailRouterIncidentBoxBlock settings = IncidentBoxDocument.Load(incidentBox.IncidentBoxId).EMailRouterBlock;
								if (settings.AllowEMailRouting)
								{
									e.IsEnabled = true;
									e.IsVisible = true;
								}
							}
						}
					}
					break;
				#endregion
				#region ReSendOut
				case "ReSendOut":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						if (e.Node.Type == "email")
						{
							if (e.Node.Attributes.ContainsKey("Outgoing"))
							{
								e.IsEnabled = true;
								e.IsVisible = true;
							}
						}
					}
					break;
				#endregion
				#region Resolution
				case "Resolution":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						if (!e.Node.Attributes.ContainsKey("Resolution")
								&& !e.Node.Attributes.ContainsKey("Question"))
						{
							e.IsEnabled = true;
							e.IsVisible = true;
						}
					}
					break;
				#endregion
				#region UnResolution
				case "UnResolution":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						if (e.Node.Attributes.ContainsKey("Resolution"))
						{
							e.IsEnabled = true;
							e.IsVisible = true;
						}
					}
					break;
				#endregion
				#region Workaround
				case "Workaround":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						if (!e.Node.Attributes.ContainsKey("Workaround")
								&& !e.Node.Attributes.ContainsKey("Question"))
						{
							e.IsEnabled = true;
							e.IsVisible = true;
						}
					}
					break;
				#endregion
				#region UnWorkaround
				case "UnWorkaround":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						if (e.Node.Attributes.ContainsKey("Workaround"))
						{
							e.IsEnabled = true;
							e.IsVisible = true;
						}
					}
					break;
				#endregion
				#region OnTop
				case "OnTop":
					if (!e.Node.Attributes.ContainsKey(Node.ONTOP_ATTRIBUTE))
					{
						e.IsEnabled = true;
						e.IsVisible = true;
					}
					break;
				#endregion
				#region UnOnTop
				case "UnOnTop":
					if (e.Node.Attributes.ContainsKey(Node.ONTOP_ATTRIBUTE))
					{
						e.IsEnabled = true;
						e.IsVisible = true;
					}
					break;
				#endregion
				#region Delete
				case "Delete":
					if (iObjTypeId == (int)ObjectTypes.Issue)
					{
						bool CanUpdate = Incident.CanUpdate(iObjId);
						e.IsEnabled = CanUpdate;
						e.IsVisible = CanUpdate;
						e.IsBreak = true;
					}
					break;
				#endregion
				default:
					break;
			}
		}
		#endregion

		#region NodeActions_InvokeCommand
		void NodeActions_InvokeCommand(object Sender, InvokeCommandArgs e)
    {
			SqlForumProvider fp = new Mediachase.Ibn.Forum.SqlForumProvider();
			HttpResponse Response = ((Control)Sender).Page.Response;

			int iObjTypeId = -1;
			int iObjId = -1;
			if (HttpContext.Current.Items["ObjectTypeId"] != null)
				iObjTypeId = (int)HttpContext.Current.Items["ObjectTypeId"];
			if (HttpContext.Current.Items["ObjectId"] != null)
				iObjId = (int)HttpContext.Current.Items["ObjectId"];

			// TODO: (OR) У Олега Ж. пока не реализовано. Должно вытаскиваться из атрибутов.
			int emailMessageId = -1;

			switch (e.CommandUid)
			{
				#region Reply
				case "Reply":
//					((Control)Sender).Page.Response.Write("Test reply event (TestCommandManager.dll : class TestReplyHandler )<br>");
					break;
				#endregion
				#region ReplyOutlook
				case "ReplyOutlook":
					Response.ClearContent();
					Response.ClearHeaders();
					Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", "mail.msg"));
					Response.ContentType = "application/msoutlook";

					MsgMessage.Open(iObjId, emailMessageId, Response.OutputStream);
					Response.End();
					break;
				#endregion
				#region ReplyEML
				case "ReplyEML":
					Response.ClearContent();
					Response.ClearHeaders();
					Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", "mail.eml"));
					Response.ContentType = "message/rfc822";
					MsgMessage.OpenEml(iObjId, emailMessageId, Response.OutputStream);
					Response.End();
					break;
				#endregion
				#region ReSend
				case "ReSend":
					EMailRouterOutputMessage.Send(iObjId, emailMessageId);
					break;
				#endregion
				#region ReSendOut
				case "ReSendOut":
					EMailRouterOutputMessage.Send(iObjId, emailMessageId);
					break;
					#endregion
				#region Resolution
				case "Resolution":
					e.Node.SetAttribute("Resolution", "1");
					fp.SaveNode(e.Node);
					break;
				#endregion
				#region UnResolution
				case "UnResolution":
					if (e.Node.Attributes.ContainsKey("Resolution"))
					{
						e.Node.Attributes.Remove("Resolution");
						fp.SaveNode(e.Node);
					}
					break;
				#endregion
				#region Workaround
				case "Workaround":
					e.Node.SetAttribute("Workaround", "1");
					fp.SaveNode(e.Node);
					break;
				#endregion
				#region UnWorkaround
				case "UnWorkaround":
					if (e.Node.Attributes.ContainsKey("Workaround"))
					{
						e.Node.Attributes.Remove("Workaround");
						fp.SaveNode(e.Node);
					}
					break;
				#endregion
				#region OnTop
				case "OnTop":
					e.Node.SetAttribute(Node.ONTOP_ATTRIBUTE, "1");
					fp.SaveNode(e.Node);
					break;
				#endregion
				#region UnOnTop
				case "UnOnTop":
					if (e.Node.Attributes.ContainsKey(Node.ONTOP_ATTRIBUTE))
					{
						e.Node.Attributes.Remove(Node.ONTOP_ATTRIBUTE);
						fp.SaveNode(e.Node);
					}
					break;
				#endregion
				#region Delete
				case "Delete":
					fp.DeleteNode(e.Node.Uid);
					break;
				#endregion
				default:
					break;
			}
		}
		#endregion

		#endregion
	}
}
