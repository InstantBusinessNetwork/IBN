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
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.WebDAV.Common;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for EMailView.
	/// </summary>
	public partial class EMailView : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strMailIncidentsList", typeof(EMailView).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(EMailView).Assembly);

		#region EMailId
		protected int EMailId
		{
			get
			{
				if (Request["EMailId"] != null)
					return int.Parse(Request["EMailId"]);
				else
					return -1;
			}
		}
		#endregion

		#region NodeId
		protected int NodeId
		{
			get
			{
				if (Request["NodeId"] != null)
					return int.Parse(Request["NodeId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			iconIBN.Attributes.Add("href", ResolveUrl("~/portal.ico"));

			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			if (EMailId > 0)
				BindEmail();
			else if (NodeId > 0)
				BindNode();
		}

		private void BindNode()
		{
			ForumThreadNodeInfo fi = ForumThreadNodeInfo.Load(NodeId);
			lblBody.CssClass = "text";
			if (fi != null)
				lblBody.Text = fi.Text;
			trEmail.Visible = false;
		}

		#region BindEmail
		private void BindEmail()
		{
			EMailMessageInfo mi = EMailMessageInfo.Load(EMailId);
			lblFrom.Text = mi.From;
			lblSubj.Text = mi.Subject;
			lblBody.Text = mi.HtmlBody;

			string sAttach = "";
			for (int i = 0; i < mi.Attachments.Length; i++)
			{
				AttachmentInfo ai = mi.Attachments[i];
				int id = DSFile.GetContentTypeByFileName(ai.FileName);
				string sIcon = "";
				if (id > 0)
				{
					sIcon = String.Format("<img align='absmiddle' border='0' src='{0}' />", ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + id));
				}
				//sAttach += String.Format("<nobr><a href='{0}'{2}>{1}</a></nobr> &nbsp;&nbsp;",
				//    ResolveUrl("~/Incidents/EmailAttachDownload.aspx")+"?EMailId="+EMailId.ToString()+"&AttachmentIndex="+i.ToString(),
				//    sIcon + "&nbsp;" + ai.FileName,
				//    Mediachase.IBN.Business.Common.OpenInNewWindow(ai.ContentType) ? " target='_blank'" : "");
				sAttach += String.Format("<nobr><a href='{0}'{2}>{1}</a></nobr> &nbsp;&nbsp;", WebDavUrlBuilder.GetEmailAtachWebDavUrl(EMailId, i, true),
													sIcon + "&nbsp;" + ai.FileName, Mediachase.IBN.Business.Common.OpenInNewWindow(ai.ContentType) ? " target='_blank'" : "");

			}
			lblAttach.Text = sAttach;
			lblBody.Text += "&nbsp;";

			if (NodeId > 0)
			{
				ForumThreadNodeSettingCollection coll = new ForumThreadNodeSettingCollection(NodeId);
				if (!String.IsNullOrEmpty(coll[ForumThreadNodeSetting.AllRecipients]))
					lblTo.Text = coll[ForumThreadNodeSetting.AllRecipients];
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (lblTo.Text == String.Empty)
				ToRow.Visible = false;
			if (lblAttach.Text == String.Empty)
				AttachRow.Visible = false;
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
