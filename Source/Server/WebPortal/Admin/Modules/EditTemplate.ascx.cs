namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for EditTemplate.
	/// </summary>
	public partial class EditTemplate : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		string title = "";

		protected int EventTypeID
		{
			get
			{
				return int.Parse(Request["EventTypeID"]);
			}
		}

		protected int LanguageID
		{
			get
			{
				return int.Parse(Request["LanguageID"]);
			}
		}

		protected int MessageTypeID
		{
			get
			{
				return int.Parse(Request["MessageTypeID"]);
			}
		}

		protected bool IsExternal
		{
			get
			{
				if (Request["IsExternal"] != null)
					return true;
				else
					return false;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.ToolbarBackgroundImage = false;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = this.Page.ResolveUrl("~/Common/Empty.html");

			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			if (!IsPostBack)
				BindValues();
			BindToolbar();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
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
					if (!(bool)rdr["IsXSLT"])
					{
						fckEditor.Text = (string)rdr["Body"];
						tbBody.Visible = false;
					}
					else
					{
						tbBody.Text = (string)rdr["Body"];
						fckEditor.Visible = false;
					}

					ViewState["IsXSLT"] = rdr["IsXSLT"];
					ViewState["TemplateId"] = rdr["TemplateId"];

					if (MessageTypeID == 2)
						trSubject.Visible = false;
				}
			}

			dlSysVar.DataSource = Alert.GetVariablesByType(EventTypeID);
			dlSysVar.DataBind();
			*/
		}

		private void BindToolbar()
		{
			secHdr.Title = title;
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
			int TemplateId = (int)ViewState["TemplateId"];
			string body = "";
			if ((bool)ViewState["IsXSLT"])
				body = tbBody.Text;
			else
				body = fckEditor.Text;
			//Alert.UpdateAlertTemplate(TemplateId,LanguageID,tbSubject.Text,body);
			Response.Redirect("~/Admin/MessageTemplates.aspx");
		}
	}
}