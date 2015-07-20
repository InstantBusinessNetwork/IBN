namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
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
	///		Summary description for AddBroadcastAlert.
	/// </summary>
	public partial class AddBroadcastAlert : System.Web.UI.UserControl
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");

			ApplyLocalization();

			if (!Page.IsPostBack)
				BindValues();

			BindToolBar();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnSave.Text = LocRM.GetString("tSend");
			cvGroup.ErrorMessage = LocRM.GetString("tWarningGroups");
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tAlertAdd");
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle'>&nbsp;{1}",
				ResolveClientUrl("~/Layouts/Images/cancel.gif"), LocRM.GetString("tBroadcastAlerts")), 
				ResolveUrl("~/Admin/BroadcastAlerts.aspx"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(false, false, false, false, false, false, false, false, false, false, false))
			{
				while (reader.Read())
				{
					lbGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			dtcExDate.SelectedDate = User.GetLocalDate(DateTime.UtcNow.AddHours(1));
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cvGroup.ServerValidate += new ServerValidateEventHandler(cvGroup_ServerValidate);
		}
		#endregion

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Admin/BroadcastAlerts.aspx");
		}

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			ArrayList recipients = new ArrayList();
			foreach (ListItem liItem in lbGroups.Items)
				if (liItem.Selected)
					recipients.Add(int.Parse(liItem.Value));

			Mediachase.IBN.Business.Common.AddBroadCastMessage(fckEditor.Text, dtcExDate.SelectedDate, recipients);

			Response.Redirect("~/Admin/BroadcastAlerts.aspx", true);
		}

		private void cvGroup_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (lbGroups.SelectedItem == null)
				args.IsValid = false;
			else
				args.IsValid = true;
		}
	}
}
