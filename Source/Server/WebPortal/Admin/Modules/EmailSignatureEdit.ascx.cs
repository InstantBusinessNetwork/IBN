namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Resources;
	using System.Collections;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for EmailSignatureEdit.
	/// </summary>
	public partial class EmailSignatureEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		//protected System.Web.UI.WebControls.TextBox tbSubject;
		//protected System.Web.UI.HtmlControls.HtmlTableRow trSubject;
		string title = "";
		private bool bCustom = true;

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
			string strSubject;
			string strBody;

			AlertTemplate.GetTemplate(AlertTemplateTypes.Special, Language,
				Template,
				bCustom,
				out strSubject,
				out strBody);

			IncidentBoxDocument ibd = IncidentBoxDocument.Load(IncidentBoxId);
			if (ibd != null)
			{
				tbSubject.Text = ibd.GeneralBlock.OutgoingEmailFormatSubject;
				tbBody.Text = ibd.GeneralBlock.OutgoingEmailFormatBody;
			}
			IncidentBox ib = IncidentBox.Load(IncidentBoxId);
			string ibName = "";
			if (ib != null)
				ibName = ib.Name;
			title = LocRM.GetString("TemplateEdit") + "&nbsp;" + ibName;


			DataTable dt = AlertTemplate.GetVariablesForTemplateEditor(AlertTemplateTypes.Special, Template);
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			dlSysVar.DataSource = dv;
			dlSysVar.DataBind();
		}

		private void BindToolbar()
		{
			secHdr.Title = title;
			secHdr.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("IssueBox"), 
				ResolveUrl("~/Admin/EmailIssueBoxView.aspx?IssBoxId=" + IncidentBoxId.ToString()));
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
				ibd.GeneralBlock.OutgoingEmailFormatBody = body;
				ibd.GeneralBlock.OutgoingEmailFormatSubject = tbSubject.Text;
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
				ibd.GeneralBlock.ResetOutgoingEmailFormat();
				IncidentBoxDocument.Save(ibd);
			}
			bCustom = false;
			BindValues();
			secHdr.Title = title;
		}
	}
}
