namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Globalization;
	using System.Resources;
	using Mediachase.IBN.Business;


	/// <summary>
	///		Summary description for DirectoryBrowser.
	/// </summary>
	public partial class DirectoryBrowser : System.Web.UI.UserControl
	{


		private string Tab
		{
			get
			{
				return Request["Tab"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindTabs();
			ctrlTopTab.Visible = false;
		}

		private void BindTabs()
		{
			bool ShowReports = Security.IsUserInGroup(InternalSecureGroups.Administrator);
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strDirectory", typeof(DirectoryBrowser).Assembly);
			ctrlTopTab.AddTab(LocRM.GetString("tabSecGroups"), "0");
			if (PortalConfig.UseIM)
				ctrlTopTab.AddTab(LocRM.GetString("tabIMGroups"), "1");
			ctrlTopTab.AddTab(LocRM.GetString("tabList"), "3");
			ctrlTopTab.AddTab(LocRM.GetString("tabSearch"), "2");

			if (ShowReports)
				ctrlTopTab.AddTab(LocRM.GetString("tReports"), "4");

			UserLightPropertyCollection pc;
			pc = Security.CurrentUser.Properties;

			if (Tab != null && (Tab == "0" || Tab == "1" || Tab == "3" || Tab == "2" || Tab == "4"))
				pc["Directory_CurrentTab"] = Tab;
			else if (pc["Directory_CurrentTab"] == null)
				pc["Directory_CurrentTab"] = "0";

			if (!PortalConfig.UseIM && pc["Directory_CurrentTab"] == "1")
				pc["Directory_CurrentTab"] = "0";

			if (!ShowReports && pc["Directory_CurrentTab"] == "4")
				pc["Directory_CurrentTab"] = "0";

			ctrlTopTab.SelectItem(pc["Directory_CurrentTab"]);

			string controlName = "";
			switch (pc["Directory_CurrentTab"])
			{
				case "0":
					if (Security.IsUserInGroup(InternalSecureGroups.Partner))
						controlName = "SecureGroupsPartners.ascx";
					else
						controlName = "SecureGroups.ascx";
					((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM.GetString("tabSecGroups");
					break;
				case "1":
					controlName = "IMGroups.ascx";
					((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM.GetString("tabIMGroups");
					break;
				case "2":
					controlName = "Search.ascx";
					break;
				case "3":
					controlName = "ListView.ascx";
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
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
	}
}
