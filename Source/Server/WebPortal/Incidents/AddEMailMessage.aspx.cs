using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.FileUploader.Web;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.UI.Web.Util;
using CS = Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	public partial class AddEMailMessage : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(AddEMailMessage).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strNotExistingId", typeof(AddEMailMessage).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(AddEMailMessage).Assembly);

		#region IncidentId
		protected int IncidentId
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentID"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region ErrorId
		protected string ErrorId
		{
			get
			{
				if (Request["ErrorId"] != null)
					return Request["ErrorId"];
				else
					return "";
			}
		}
		#endregion

		#region _guid
		protected string _guid
		{
			get
			{
				if (hidGuid.Value == "")
					hidGuid.Value = Guid.NewGuid().ToString();
				return hidGuid.Value;
			}
		}
		#endregion

		#region NodeId
		protected int NodeId
		{
			get
			{
				try
				{
					return int.Parse(Request["NodeId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region FileIds
		protected string FileIds
		{
			get
			{
				if (Request["FileIds"] != null)
					return Request["FileIds"];
				else
					return "";
			}
		}

		protected string FilesContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return "";
			}
		}

		protected string FilesContainerName
		{
			get
			{
				if (Request["ContainerName"] != null)
					return Request["ContainerName"];
				else
					return "";
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/outlook2003.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/emailsend.js");

			iconIBN.Attributes.Add("href", ResolveUrl("~/portal.ico"));
			imgAttachInner.Src = GlobalResourceManager.Strings["AttachFromProductUrl"];
			imgAttachInner.Attributes.Add("title", LocRM.GetString("tAddIBNfiles"));
			divMessage.Visible = false;
			Response.Cache.SetNoStore();
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");
			imbSave.Text = "<br />" + LocRM.GetString("tSend");
			if (!Page.IsPostBack)
				BindDefaultValues();
			imbSave.Disabled = false;
			imbSave.Attributes.Add("onclick", "CheckForUpload(\"" + Page.ClientScript.GetPostBackEventReference(lbSend, "") + "\", this);");
			imbSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/sendemail.gif");
			Ajax.Utility.RegisterTypeForAjax(typeof(AddEMailMessage));
		}

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			string fromEmail = Security.CurrentUser.Email;
			if(IncidentId > 0)
				fromEmail = EMailRouterOutputMessage.FindEMailRouterPublicEmail(IncidentId);
			txtFrom.Text = String.Format(CultureInfo.InvariantCulture,
				"{1}&nbsp;&lt;{0}&gt;", fromEmail, Security.CurrentUser.DisplayName);
			lblCCTitle.Text = "";
			lblToTitle.Text = LocRM.GetString("tTo") + ":";
			if (IncidentId > 0)
			{
				NameValueCollection _params = new NameValueCollection();
				_params["IssueId"] = IncidentId.ToString();
				string[] erList = EMailClient.GetDefaultRecipientList(EMailClient.IssueMode, _params);
				for (int i = 0; i < erList.Length; i++)
				{
					string sName = GetNameByEMail(erList[i]);
					if (sName != "")
						lblCC.Text += String.Format(CultureInfo.InvariantCulture, "{0} &lt;{1}&gt;; ", sName, erList[i]);
					else
						lblCC.Text += erList[i] + "; ";
				}

				string sValue = EMailMessage.GetOutgoingEmailFormatBodyPreview(IncidentId).Replace("[=Text=]", "");
				if (NodeId > 0)
				{
					EMailMessageInfo mi = EMailMessageInfo.Load(NodeId);
					sValue += "<br/>" + "<blockquote style='border-left: 2px solid rgb(0, 0, 0); padding-right: 0px; padding-left: 5px; margin-left: 5px; margin-right: 0px;' dir='ltr'>" + mi.HtmlBody + "</blockquote>";
				}
				fckEditor.Text = sValue;

				using (IDataReader reader = Incident.GetIncident(IncidentId))
				{
					if (reader.Read())
					{
						txtSubject.Text = string.Format(CultureInfo.InvariantCulture
							, "RE: [{0}] {1}"
							, (reader["Identifier"] != DBNull.Value) ?
								reader["Identifier"].ToString()
								: TicketUidUtil.Create(IncidentBox.Load((int)reader["IncidentBoxId"]).IdentifierMask, IncidentId)
							, HttpUtility.HtmlDecode(reader["Title"].ToString())
							);
					}
				}
			}
			if (FileIds != "")
			{
				string sFiles = FileIds.Trim();
				if (sFiles.EndsWith(","))
					sFiles = sFiles.Substring(0, sFiles.Length - 1);
				string[] masFiles = sFiles.Split(',');
				if (masFiles.Length > 0)
				{
					string _containerName = "FileLibrary";
					string _containerKey = "EMailAttach";
					CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create(_containerName, _containerKey);
					CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");

					CS.BaseIbnContainer bic2 = CS.BaseIbnContainer.Create(FilesContainerName, FilesContainerKey);
					CS.FileStorage fs2 = (CS.FileStorage)bic.LoadControl("FileStorage");

					CS.DirectoryInfo di = fs.GetDirectory(fs.Root.Id, _guid, true);
					for (int i = 0; i < masFiles.Length; i++)
					{
						fs2.CopyFile(int.Parse(masFiles[i]), di.Id);
					}
					Page.ClientScript.RegisterStartupScript(this.GetType(), "_getFiles",
					  "window.setTimeout('updateAttachments()', 500);", true);
				}
			}
			if (ErrorId != "")
			{
				string support_email = GlobalResourceManager.Strings["SupportEmail"];
				//if (Security.CurrentUser != null && Security.CurrentUser.Culture.ToLower().IndexOf("ru") >= 0)
				//    support_email = "support@mediachase.ru";
				txtTo.Text = support_email + "; ";

				txtSubject.Text = String.Format("{0} {1} {2} Error Report", IbnConst.CompanyName, IbnConst.ProductFamilyShort, IbnConst.VersionMajorDotMinor);

				string prefix = Request.Url.Host.Replace(".", "_");
				string FilePath = Server.MapPath("../Admin/Log/Error/" + prefix + "_" + ErrorId + ".html");
				string sTemp = String.Empty;
				using (StreamReader sr = File.OpenText(FilePath))
				{
					sTemp += sr.ReadToEnd();
				}

				Match match = Regex.Match(sTemp, @"<body[^>]*>(?<HtmlBody>[\s\S]*?)</body>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
				if (match.Success)
				{
					string body = match.Groups["HtmlBody"].Value;
					Match matchStyle = Regex.Match(sTemp, @"<style[^>]*>(?<HtmlStyle>[\s\S]*?)</style>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
					if (matchStyle.Success)
						body += matchStyle.Value;
					sTemp = body;
				}
				fckEditor.Text = sTemp;
			}
			if (lblCC.Text != "")
			{
				lblCCTitle.Text = LocRM.GetString("tTo") + ":";
				lblToTitle.Text = LocRM.GetString("tCc") + ":";
			}
		}
		#endregion

		#region Save-Send
		protected void lbSend_Click(object sender, System.EventArgs e)
		{
			string sTo = txtTo.Text.Trim();

			// all recipients string
			string allRecipients = String.Empty;
			if (sTo != String.Empty)
				allRecipients = sTo;
			if (allRecipients != String.Empty)
				allRecipients += "; ";
			allRecipients += lblCC.Text;
			CommonHelper.AddToContext(ForumThreadNodeSetting.AllRecipients, allRecipients);

			string regex = "([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@(([0-9a-zA-Z])+([-\\w]*[0-9a-zA-Z])*\\.)+[a-zA-Z]" +
			  "{2,9})";
			List<string> dic = new List<string>();

			System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
			  | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);


			foreach (Match item in reg.Matches(sTo))
			{
				if (!dic.Contains(item.Value))
					dic.Add(item.Value);
			}
			string[] masTo = dic.ToArray();

			if (masTo.Length == 0 && lblCC.Text == String.Empty)
			{
				lblException.Text = LocRM.GetString("NoRecipients");
				divMessage.Visible = true;
				Page.ClientScript.RegisterStartupScript(this.GetType(), "_getFiles",
					  "window.setTimeout('updateAttachments()', 500);", true);
				return;
			}


			string _containerName = "FileLibrary";
			string _containerKey = "EMailAttach";
			CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create(_containerName, _containerKey);
			CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");
			CS.DirectoryInfo di = fs.GetDirectory(fs.Root.Id, _guid, true);

			try
			{
				if (IncidentId > 0)
				{
					NameValueCollection _params = new NameValueCollection();
					_params["IssueId"] = IncidentId.ToString();
					EMailClient.SendMessage(masTo, txtSubject.Text, fckEditor.Text, di, EMailClient.IssueMode, _params);
					string sUrl = "../Incidents/IncidentView.aspx?IncidentId=" + IncidentId;
					if (Request["back"] != null)
						sUrl = HttpUtility.UrlDecode(Request["back"]);
					Page.ClientScript.RegisterStartupScript(this.GetType(), "_close", "try{window.opener.location.href='" + sUrl + "';}catch(ex){}window.close();", true);
				}
				else
				{
					EMailClient.SendMessage(masTo, txtSubject.Text, fckEditor.Text, di);
					Page.ClientScript.RegisterStartupScript(this.GetType(), "_close", "window.close();", true);
				}
			}
			catch (Exception ex)
			{
				string sException = "";
				switch (ex.GetType().ToString())
				{
					case "System.Net.Sockets.SocketException":
						sException = ex.Message;
						break;
					case "Mediachase.IBN.Business.EMail.SmtpClientException":
						sException = LocRM2.GetString("tServerSMTPError") + ":&nbsp;<span style='color:#444;font-style:italic;'>" + ex.Message + "</span>";
						break;
					default:
						// TODO: Save Exception to IBN Log And Show Friendly Link to Log
						sException = ex.Message;
						CHelper.GenerateErrorReport(ex);
						break;
				}
				lblException.Text = LocRM2.GetString("SMTPSettingsIncorrect") + "&nbsp;" +
					LocRM2.GetString("tWereErrorsSMTP") + "<br/>" + sException;
				if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
					lblSMTP.Text = "<a href='javascript:ToSMTPSettings();'><font style='color:Red;'><b><u>" + LocRM2.GetString("tSetupSMTPServer") + "</u></b></font></a>";
				else
					lblSMTP.Text = LocRM2.GetString("tContactSMTPError");
				divMessage.Visible = true;
				Page.ClientScript.RegisterStartupScript(this.GetType(), "_getFiles",
					"window.setTimeout('updateAttachments()', 500);", true);
			}
		}
		#endregion

		#region GetNameByEMail
		protected string GetNameByEMail(string eMail)
		{
			int iUserId = Mediachase.IBN.Business.User.GetUserByEmail(eMail);
			if (iUserId > 0)
				return UserLight.Load(iUserId).DisplayName;
			else
			{
				Mediachase.IBN.Business.Client client = Mediachase.IBN.Business.Common.GetClient(eMail);
				if (client != null)
				{
					return client.Name;
				}
				else
					return "";
			}
		}
		#endregion

		#region Ajax
		[Ajax.AjaxMethod()]
		public ArrayList GetProgressInfo(string FormId)
		{
			ArrayList values = new ArrayList();

			Guid progressUid = new Guid(FormId);
			UploadProgressInfo upi = UploadProgress.Provider.GetInfo(progressUid);

			if (upi == null)
			{
				values.Add("-1");
				values.Add(LocRM3.GetString("tWaitForUploading"));
			}
			else
			{
				if (upi.Result == UploadResult.Succeeded)
				{
					if (upi.BytesTotal != upi.BytesReceived)
					{
						values.Add("-2");
						values.Add(LocRM3.GetString("tUploadFailed"));
					}
					else
					{
						values.Add("-3");
						values.Add(LocRM3.GetString("tUploadSuccess"));
					}
				}
				else
				{
					// 0
					values.Add(CommonHelper.ByteSizeToStr(upi.BytesReceived));
					// 1
					values.Add(CommonHelper.ByteSizeToStr(upi.BytesTotal));
					// 2
					values.Add(upi.EstimatedTime.ToString().Substring(0, 8));
					// 3
					values.Add((upi.TimeRemaining.ToString().Substring(0, 8)).StartsWith("-") ? "00:00:00" : upi.TimeRemaining.ToString().Substring(0, 8));

					// 4
					int percents = (int)((float)upi.BytesReceived / (float)upi.BytesTotal * 100);
					values.Add(percents.ToString());

					// 5
					string sFName = upi.CurrentFileName;
					if (sFName.LastIndexOf("\\") >= 0)
						sFName = sFName.Substring(sFName.LastIndexOf("\\") + 1);
					values.Add(LocRM3.GetString("tInProgress") + " " + sFName);
				}
			}
			return values;
		}

		[Ajax.AjaxMethod()]
		public ArrayList GetAttachments(string _guid)
		{
			string sFiles = "";
			ArrayList values = new ArrayList();
			string _containerName = "FileLibrary";
			string _containerKey = "EMailAttach";
			CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create(_containerName, _containerKey);
			CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");
			CS.DirectoryInfo di = fs.GetDirectory(fs.Root.Id, _guid, false);
			if (di != null)
			{
				CS.FileInfo[] _fi = fs.GetFiles(di);
				foreach (CS.FileInfo fi in _fi)
				{
					sFiles += String.Format(CultureInfo.InvariantCulture,
						"<div style='padding-bottom:1px;'><img align='absmiddle' src='{0}' width='16' height='16'>&nbsp;{1}&nbsp;&nbsp;<img src='{2}' align='absmiddle' width='16' height='16' style='cursor:pointer;' onclick='_deleteFile({3})' title='{4}' /></div>",
						CHelper.GetAbsolutePath("/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId),
						Util.CommonHelper.GetShortFileName(fi.Name, 40),
						CHelper.GetAbsolutePath("/Layouts/Images/delete.gif"),
						fi.Id,
						LocRM.GetString("Delete"));
				}
			}
			values.Add(sFiles);
			return values;
		}

		[Ajax.AjaxMethod()]
		public ArrayList DeleteAttachment(string _guid, string _id)
		{
			string sFiles = "";
			ArrayList values = new ArrayList();
			string _containerName = "FileLibrary";
			string _containerKey = "EMailAttach";
			CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create(_containerName, _containerKey);
			CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");
			fs.DeleteFile(int.Parse(_id));
			CS.DirectoryInfo di = fs.GetDirectory(fs.Root.Id, _guid, false);
			if (di != null)
			{
				CS.FileInfo[] _fi = fs.GetFiles(di);
				foreach (CS.FileInfo fi in _fi)
				{
					sFiles += String.Format(CultureInfo.InvariantCulture,
						"<div style='padding-bottom:1px;'><img align='absmiddle' src='{0}' width='16' height='16'>&nbsp;{1}&nbsp;&nbsp;<img src='{2}' align='absmiddle' width='16' height='16' style='cursor:pointer;' onclick='_deleteFile({3})' title='{4}' /></div>",
						CHelper.GetAbsolutePath("/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId),
						Util.CommonHelper.GetShortFileName(fi.Name, 40),
						CHelper.GetAbsolutePath("/Layouts/Images/delete.gif"),
						fi.Id,
						LocRM.GetString("Delete"));
				}
			}
			values.Add(sFiles);
			return values;
		}
		#endregion

	}
}