namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for EditSpecialTemplate.
	/// </summary>
	public partial class EditSpecialTemplate : System.Web.UI.UserControl
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
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

			using (SkipApplyGlobalSubjectTemplate skip = new SkipApplyGlobalSubjectTemplate())
			{
				Mediachase.IBN.Business.AlertTemplate.GetTemplate(AlertTemplateTypes.Special, Language,
					Template,
					bCustom,
					out strSubject,
					out strBody);
				tbSubject.Text = strSubject;
				tbBody.Text = strBody;
			}

			title = Mediachase.IBN.Business.AlertTemplate.GetDisplayName(AlertTemplateTypes.Special, Template);
			if (title == string.Empty)
				title = " ";

			DataTable dt = Mediachase.IBN.Business.AlertTemplate.GetVariablesForTemplateEditor(AlertTemplateTypes.Special, Template);
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			dlSysVar.DataSource = dv;
			dlSysVar.DataBind();
		}

		private void BindToolbar()
		{
			secHdr.Title = title;
			secHdr.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("SpecialTemplates"), 
				ResolveUrl("~/Admin/SpecialTemplates.aspx"));
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
			this.btnReset.ServerClick += new EventHandler(btnReset_ServerClick);
		}
		#endregion

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/SpecialTemplates.aspx");
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			string body = "";
			body = tbBody.Text;
			Mediachase.IBN.Business.AlertTemplate.UpdateTemplate(Language, Template, tbSubject.Text, body);
			Response.Redirect("~/Admin/SpecialTemplates.aspx");
		}

		private void btnReset_ServerClick(object sender, EventArgs e)
		{
			//AlertTemplate.ResetTemplate(Language,Template);
			bCustom = false;
			BindValues();
		}

	}
}
