namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Collections;
	using System.Resources;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for EmailIssueBoxTemplateEdit.
	/// </summary>
	public partial class EmailIssueBoxTemplateEdit : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		string title = "";
		//private bool bCustom = true;

		protected string Template
		{
			get
			{
				return Request["Template"];
			}
		}

		protected string Language
		{
			get
			{
				return Request["Language"];
			}
		}

		protected int IncidentBoxId
		{
			get
			{
				if (Request["IncidentBoxId"] != null)
					return int.Parse(Request["IncidentBoxId"].ToString());
				return -1;
			}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{

			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Text = LocRM.GetString("Save");
			btnReset.Text = LocRM.GetString("tReset");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			if (!IsPostBack)
				BindValues();
			BindToolbar();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnReset.CustomImage = this.Page.ResolveUrl("~/layouts/images/default.gif");
		}

		private void BindValues()
		{
			IncidentBoxDocument ibd = IncidentBoxDocument.Load(IncidentBoxId);
			if (ibd != null)
			{
				if (Template.IndexOf("Issue_Created") >= 0)
				{
					tbSubject.Text = ibd.GeneralBlock.AutoReplyEMailSubjectTemplate;
					tbBody.Text = ibd.GeneralBlock.AutoReplyEMailBodyTemplate;
				}
				else if (Template.IndexOf("Issue_Closed") >= 0)
				{
					tbSubject.Text = ibd.GeneralBlock.OnCloseAutoReplyEMailSubjectTemplate;
					tbBody.Text = ibd.GeneralBlock.OnCloseAutoReplyEMailBodyTemplate;
				}
			}
			IncidentBox ib = IncidentBox.Load(IncidentBoxId);
			string ibName = "";
			if (ib != null)
				ibName = ib.Name;
			if (Template.IndexOf("Issue_Created") >= 0)
				title = LocRM.GetString("NewIssueMessage") + "&nbsp;" + ibName;
			else if (Template.IndexOf("Issue_Closed") >= 0)
				title = LocRM.GetString("CloseIssueMessage") + "&nbsp;" + ibName;

			DataTable dt = AlertTemplate.GetVariablesForTemplateEditor(AlertTemplateTypes.Notification, Template);
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			dlSysVar.DataSource = dv;
			dlSysVar.DataBind();
		}

		private void BindToolbar()
		{
			secHdr.Title = title;
			secHdr.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("IssueBox"), 
				ResolveUrl("~/Admin/EmailIssueBoxView.aspx") + "?IssBoxId=" + IncidentBoxId.ToString());
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			btnReset.ServerClick += new EventHandler(btnReset_ServerClick);

		}
		#endregion

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			btnCancel.Disabled = true;
			Response.Redirect("~/Admin/EmailIssueBoxView.aspx?IssBoxId=" + IncidentBoxId.ToString());
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			btnSave.Disabled = true;
			string body = "";
			body = tbBody.Text;
			IncidentBoxDocument ibd = IncidentBoxDocument.Load(IncidentBoxId);
			if (ibd != null)
			{
				if (Template.IndexOf("Issue_Created") >= 0)
				{
					ibd.GeneralBlock.AutoReplyEMailBodyTemplate = body;
					ibd.GeneralBlock.AutoReplyEMailSubjectTemplate = tbSubject.Text;
				}
				else if (Template.IndexOf("Issue_Closed") >= 0)
				{
					ibd.GeneralBlock.OnCloseAutoReplyEMailBodyTemplate = body;
					ibd.GeneralBlock.OnCloseAutoReplyEMailSubjectTemplate = tbSubject.Text;
				}
				IncidentBoxDocument.Save(ibd);
			}

			//AlertTemplate.UpdateTemplate(Language,Template,tbSubject.Text,body);
			Response.Redirect("~/Admin/EmailIssueBoxView.aspx?IssBoxId=" + IncidentBoxId.ToString());
		}

		private void btnReset_ServerClick(object sender, EventArgs e)
		{
			//AlertTemplate.ResetTemplate(Language,Template);
			IncidentBoxDocument ibd = IncidentBoxDocument.Load(IncidentBoxId);
			if (ibd != null)
			{
				if (Template.IndexOf("Issue_Created") >= 0)
					ibd.GeneralBlock.ResetAutoReplySettings();
				else if (Template.IndexOf("Issue_Closed") >= 0)
					ibd.GeneralBlock.ResetOnCloseAutoReplySettings();

				IncidentBoxDocument.Save(ibd);
			}
			//bCustom = false;
			BindValues();
			secHdr.Title = title;
		}
	}
}
