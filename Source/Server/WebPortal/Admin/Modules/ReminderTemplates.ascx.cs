namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using System.Collections.Specialized;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for ReminderTemplates.
	/// </summary>
	public partial  class ReminderTemplates : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private bool bCustom = true;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataBind();
			if (!IsPostBack)
			{
				BindDefaultValues();
				BindMessage();
			}		
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolBar();
		}

		private void BindDefaultValues()
		{
			DataTable dt = Mediachase.IBN.Business.AlertTemplate.GetNames(AlertTemplateTypes.Reminder);
			DataView dv = dt.DefaultView;

			if (!Configuration.WorkflowModule)
				dv.RowFilter = "Key <> 'AssignmentFinishDate'";

			dv.Sort = "Name";
			ddTemplateFor.DataSource = dv;
			ddTemplateFor.DataTextField = "Name";
			ddTemplateFor.DataValueField = "Key";
			ddTemplateFor.DataBind();

			dt = Mediachase.IBN.Business.AlertTemplate.GetLanguages();
			dv = dt.DefaultView;
			dv.Sort = "Name";
			ddLanguage.DataSource = dv;
			ddLanguage.DataTextField = "Name";
			ddLanguage.DataValueField = "Key";
			ddLanguage.DataBind();

			if (pc["ReminderTemplates_Language"]!=null)
				CommonHelper.SafeSelect(ddLanguage,pc["ReminderTemplates_Language"]);
			if (pc["ReminderTemplates_SendedBy"]!=null)
			{
				ddTemplateFor.ClearSelection();
				CommonHelper.SafeSelect(ddTemplateFor,pc["ReminderTemplates_SendedBy"]);
			}
		}

		private void BindMessage()
		{
			/*MessageType MessType = MessageType.Email;
			bool IsExternal = false;
			
			if (int.Parse(ddTemplateFor.SelectedItem.Value) == (int)MessageType.IBN)
				MessType = MessageType.IBN;
			else
				if (int.Parse(ddTemplateFor.SelectedItem.Value) == 3)
				IsExternal = true;

			using (IDataReader rdr = Alert.GetTemplate(
					   int.Parse(ddMessages.SelectedItem.Value),
					   (int)MessType,
					   int.Parse(ddLanguage.SelectedItem.Value),
					   IsExternal))
			{
				if (rdr.Read())
				{
					ViewState["TemplateId"] = rdr["TemplateId"];
					if (rdr["Subject"]!=DBNull.Value)
						lblSubject.Text = (string)rdr["Subject"];
					lblBody.Text = (string)rdr["Body"];
				}
			}

			if (ddTemplateFor.SelectedItem.Value != "2")
				trSubject.Visible = true;
			else
				trSubject.Visible = false;
		*/
			string strSubject;
			string strBody;
			//btnReset.Disabled = !
			bCustom = Mediachase.IBN.Business.AlertTemplate.GetTemplate(AlertTemplateTypes.Reminder, ddLanguage.SelectedItem.Value,
				ddTemplateFor.SelectedItem.Value,
				true,
				out strSubject,
				out strBody);
			lblSubject.Text = strSubject;
			lblBody.Text = strBody;
		}

		private void SaveState()
		{
			if (ddLanguage.SelectedItem !=null)
				pc["ReminderTemplates_Language"] = ddLanguage.SelectedItem.Value;
			if (ddTemplateFor.SelectedItem !=null)
				pc["ReminderTemplates_SendedBy"] = ddTemplateFor.SelectedItem.Value;
		}

		private void BindToolBar()
		{
			secH.Title = LocRM.GetString("ReminderTemplates");

			if(bCustom)
				secH.AddLink("<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/default.gif") + "' height='16px' align='absmiddle'/> "+LocRM.GetString("tReset"), Page.ClientScript.GetPostBackClientHyperlink(btnReset,""));

			secH.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/edit.gif") + "'/> " + LocRM.GetString("Edit"),
				String.Format("{2}?Language={0}&Template={1}",
				ddLanguage.SelectedItem.Value,ddTemplateFor.SelectedItem.Value,
				this.Page.ResolveUrl("~/Admin/EditReminderTemplate.aspx")));
			
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM2.GetString("tRoutingWorkflow"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4"));
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
			ddTemplateFor.SelectedIndexChanged +=new EventHandler(ddTemplateFor_SelectedIndexChanged);
			ddLanguage.SelectedIndexChanged +=new EventHandler(ddLanguage_SelectedIndexChanged);
			btnReset.Click +=new EventHandler(btnReset_Click);
		}
		#endregion

		private void ddTemplateFor_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveState();
			BindMessage();

      Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>"+
				"try {var focusTemplEl=document.getElementById('" + ddTemplateFor.ClientID + "');"+
				"focusTemplEl.focus();}" +
				"catch (e){}</script>");
		}

		private void ddLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveState();
			BindMessage();

      Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>"+
				"try {var focusLangEl=document.getElementById('" + ddLanguage.ClientID + "');"+
				"focusLangEl.focus();}" +
				"catch (e){}</script>");
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			Mediachase.IBN.Business.AlertTemplate.ResetTemplate(ddLanguage.SelectedItem.Value,ddTemplateFor.SelectedItem.Value);
			Response.Redirect("~/Admin/ReminderTemplates.aspx");
		}
	}
}
