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
	///		Summary description for EditTemplate2.
	/// </summary>
	public partial class EditTemplate2 : System.Web.UI.UserControl
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
			/*using( IDataReader rdr = Alert.GetTemplate(EventTypeID, MessageTypeID, LanguageID, IsExternal))
			{
				if (rdr.Read())
				{
					title = (string)rdr["EventTypeName"];
					if (rdr["Subject"]!=DBNull.Value)
						tbSubject.Text = (string)rdr["Subject"];

					tbBody.Text = (string)rdr["Body"];

					ViewState["TemplateId"] = rdr["TemplateId"];

					if (MessageTypeID == 2)
						trSubject.Visible = false;
				}
			}

			dlSysVar.DataSource = Alert.GetVariablesByType(EventTypeID);
			dlSysVar.DataBind();
			*/

			string strSubject;
			string strBody;
			//btnReset.Disabled = !
			using (SkipApplyGlobalSubjectTemplate skip = new SkipApplyGlobalSubjectTemplate())
			{
				AlertTemplate.GetTemplate(AlertTemplateTypes.Notification, Language,
					Template,
					bCustom,
					out strSubject,
					out strBody);
				tbSubject.Text = strSubject;
				tbBody.Text = strBody;
			}

			title = AlertTemplate.GetDisplayName(AlertTemplateTypes.Notification, Template);

			DataTable dt = AlertTemplate.GetVariablesForTemplateEditor(AlertTemplateTypes.Notification, Template);
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";
			dlSysVar.DataSource = dv;
			dlSysVar.DataBind();
		}

		private void BindToolbar()
		{
			secHdr.Title = title;
			secHdr.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("MessageTemplates"), 
				ResolveUrl("~/Admin/MessageTemplates.aspx"));
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
			Response.Redirect("~/Admin/MessageTemplates.aspx");
		}

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			btnSave.Disabled = true;
			string body = "";
			body = tbBody.Text;
			AlertTemplate.UpdateTemplate(Language, Template, tbSubject.Text, body);
			Response.Redirect("~/Admin/MessageTemplates.aspx");
		}

		private void btnReset_ServerClick(object sender, EventArgs e)
		{
			//AlertTemplate.ResetTemplate(Language,Template);
			bCustom = false;
			BindValues();
		}
	}
}