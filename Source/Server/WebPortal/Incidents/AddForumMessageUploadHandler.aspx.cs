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
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for AddForumMessageUploadHandler.
	/// </summary>
	public partial class AddForumMessageUploadHandler : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentEdit", typeof(AddForumMessageUploadHandler).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(AddForumMessageUploadHandler).Assembly);

		#region IncidentId
		protected int IncidentId
		{
			get
			{
				if (Request["IncidentId"] != null)
					return int.Parse(Request["IncidentId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");

			if (!IsPostBack)
				BindData();
			else
				Process();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindData
		private void BindData()
		{
			cbList.Items.Clear();
			cbList.Items.Add(new ListItem(LocRM2.GetString("NodeType_Question"), ForumThreadNodeSetting.Question));
			cbList.Items.Add(new ListItem(LocRM2.GetString("NodeType_PublicResolution"), ForumThreadNodeSetting.Resolution));
			cbList.Items.Add(new ListItem(LocRM2.GetString("NodeType_PublicWorkaround"), ForumThreadNodeSetting.Workaround));

			// Status
			int stateId = 1;
			string stateName = "";
			using (IDataReader reader = Incident.GetIncident(IncidentId))
			{
				if (reader.Read())
				{
					stateId = (int)reader["StateId"];
					stateName = reader["StateName"].ToString();
				}
			}

			ddlStatus.DataSource = Incident.GetListIncidentStates(IncidentId);
			ddlStatus.DataTextField = "StateName";
			ddlStatus.DataValueField = "StateId";
			ddlStatus.DataBind();
			Util.CommonHelper.SafeSelect(ddlStatus, stateId.ToString());
		}
		#endregion

		#region Process
		private void Process()
		{
			string FileName1 = (McFileUp1.PostedFile == null) ? "" : McFileUp1.PostedFile.FileName;
			string FileName2 = (McFileUp2.PostedFile == null) ? "" : McFileUp2.PostedFile.FileName;
			string FileName3 = (McFileUp3.PostedFile == null) ? "" : McFileUp3.PostedFile.FileName;

			System.IO.Stream Stream1 = (McFileUp1.PostedFile == null) ? null : McFileUp1.PostedFile.InputStream;
			System.IO.Stream Stream2 = (McFileUp2.PostedFile == null) ? null : McFileUp2.PostedFile.InputStream;
			System.IO.Stream Stream3 = (McFileUp3.PostedFile == null) ? null : McFileUp3.PostedFile.InputStream;

			int nodeType = ((FileName1 != "" && Stream1 != null) ||
							(FileName2 != "" && Stream2 != null) ||
							(FileName3 != "" && Stream3 != null)) ?
				(int)ForumStorage.NodeContentType.TextWithFiles :
				(int)ForumStorage.NodeContentType.Text;

			ArrayList alAttrs = new ArrayList();
			foreach (ListItem liItem in cbList.Items)
				if (liItem.Selected)
					alAttrs.Add(liItem.Value);

			string sMessage = txtMessage.Text;
			sMessage = Util.CommonHelper.parsetext_br(sMessage);
			Issue2.AddForumMessage2(IncidentId, sMessage, nodeType, alAttrs, false, int.Parse(ddlStatus.SelectedValue), FileName1, Stream1, FileName2, Stream2, FileName3, Stream3);
		}
		#endregion

		#region TempReplace
		private string TempReplace(string sMessage)
		{
			string sTemplate = EMailMessage.GetOutgoingEmailFormatBodyPreview(IncidentId);
			sTemplate = sTemplate.Replace("<BR>", "\r\n");
			sTemplate = sTemplate.Replace("<BR/>", "\r\n");
			sTemplate = sTemplate.Replace("<BR />", "\r\n");
			sTemplate = sTemplate.Replace("<br>", "\r\n");
			sTemplate = sTemplate.Replace("<br/>", "\r\n");
			sTemplate = sTemplate.Replace("<br />", "\r\n");
			return sTemplate.Replace("[=Text=]", sMessage);
		}
		#endregion
	}
}
