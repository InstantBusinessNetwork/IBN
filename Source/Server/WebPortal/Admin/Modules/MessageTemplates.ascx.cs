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
	using Mediachase.Ibn;
	using System.Reflection;

	/// <summary>
	///		Summary description for MessageTemplates.
	/// </summary>
	public partial class MessageTemplates : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

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
			ddTemplateFor.Items.Add(new ListItem(LocRM.GetString("Email"), ((int)DeliveryType.Email).ToString()));
			ddTemplateFor.Items.Add(new ListItem(IbnConst.ProductFamilyShort, ((int)DeliveryType.IBN).ToString()));
			ddTemplateFor.Items.Add(new ListItem(LocRM.GetString("EMailExternal"), "3"));

			BindddMessages();

			ddLanguage.DataTextField = "FriendlyName";
			ddLanguage.DataValueField = "LanguageId";
			//ddLanguage.DataSource = Alert.GetListLanguages();
			ddLanguage.DataBind();

			if (pc["MessageTemplate_Message"] != null)
				CommonHelper.SafeSelect(ddMessages, pc["MessageTemplate_Message"]);
			if (pc["MessageTemplate_Language"] != null)
				CommonHelper.SafeSelect(ddLanguage, pc["MessageTemplate_Language"]);
			if (pc["MessageTemplate_SendedBy"] != null)
			{
				ddTemplateFor.ClearSelection();
				CommonHelper.SafeSelect(ddTemplateFor, pc["MessageTemplate_SendedBy"]);
			}
		}

		private void BindddMessages()
		{
			/*MessageType MessType = MessageType.Email;
			bool IsExternal = false;
			
			if (int.Parse(ddTemplateFor.SelectedItem.Value) == (int)MessageType.IBN)
				MessType = MessageType.IBN;
			else
				if (int.Parse(ddTemplateFor.SelectedItem.Value) == 3)
					IsExternal = true;

			ddMessages.Items.Clear();
			ddMessages.DataTextField = "FriendlyName";
			ddMessages.DataValueField = "TypeId";
			ddMessages.DataSource = Alert.GetEventTypes(MessType,IsExternal);
			ddMessages.DataBind();*/
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
		}

		private void SaveState()
		{
			if (ddMessages.SelectedItem != null)
				pc["MessageTemplate_Message"] = ddMessages.SelectedItem.Value;
			if (ddLanguage.SelectedItem != null)
				pc["MessageTemplate_Language"] = ddLanguage.SelectedItem.Value;
			if (ddTemplateFor.SelectedItem != null)
				pc["MessageTemplate_SendedBy"] = ddTemplateFor.SelectedItem.Value;
		}

		private void BindToolBar()
		{
			secH.Title = LocRM.GetString("MessageTemplates");

			int messtype = (int)DeliveryType.Email;
			string IsExternal = String.Empty;

			switch (ddTemplateFor.SelectedItem.Value)
			{
				case "2":
					messtype = (int)DeliveryType.IBN;
					break;
				case "3":
					IsExternal = "&IsExternal=1";
					break;
			}

			secH.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/edit.gif") + "'/> " + LocRM.GetString("Edit"), 
				String.Format("{4}?EventTypeID={0}&LanguageID={1}&MessageTypeID={2}{3}", 
				ddMessages.SelectedItem.Value, ddLanguage.SelectedItem.Value, messtype, IsExternal,
				ResolveUrl("~/Admin/EditTemplate.aspx")));

			secH.AddSeparator();
			secH.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("Back"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin4"));
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

		protected void btnApply_Click(object sender, System.EventArgs e)
		{
			SaveState();
			BindMessage();
		}

		protected void ddT_PostBack(object sender, System.EventArgs e)
		{
			BindddMessages();
		}
	}
}
