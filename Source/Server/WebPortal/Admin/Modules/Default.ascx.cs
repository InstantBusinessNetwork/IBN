namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;

	using System.Threading;
	using System.Globalization;
	using System.Resources;
	using System.Reflection;


	/// <summary>
	///		Summary description for _Default.
	/// </summary>
	public partial class _Default : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strPageTitles", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UserLightPropertyCollection pc;
			pc = Security.CurrentUser.Properties;

			String tab = "";
			try
			{
				tab = pc["Admin_CurrentTab"];
			}
			catch
			{
				tab = "PortalSetup";
			}

			bool portalsetup = tab == "PortalSetup";
			trPortalSetup1.Visible = portalsetup;
			trPortalSetup9.Visible = portalsetup;
			trPortalSetup3.Visible = false;
			trPortalSetup4.Visible = portalsetup;
			trPortalSetupHeader.Visible = portalsetup;

			bool dictionaries = tab == "Dictionaries";
			trDictionaries1.Visible = dictionaries;
			trDictionaries2.Visible = false;
			trDictionaries4.Visible = dictionaries;
			trDictionaries3.Visible = dictionaries && Configuration.ProjectManagementEnabled;
			trDictionariesHeader.Visible = dictionaries;

			bool customization = tab == "Customization";
			trCustomization1.Visible = customization;
			trCustomization2.Visible = customization;
			trCustomization4.Visible = customization;
			trCustomizationHeader.Visible = customization;

			bool routingwf = tab == "RoutingWorkflow";
			trRouting1.Visible = routingwf;
			trRouting4.Visible = routingwf;
			trRouting5.Visible = routingwf;
			//trRouting2.Visible = routingwf;
			//if (!IssueRequest.MailIssuesEnabled())
			//	trRouting2.Visible = false;
			trRouting3.Visible = routingwf;
			trRouting6.Visible = routingwf;
			trRouting7.Visible = routingwf;
			trRoutingHeader.Visible = routingwf;

			bool data = tab == "BusinessData";
			trDataHeader.Visible = data;
			trData1.Visible = data;
			trData2.Visible = data;
			trData3.Visible = data;
			trData4.Visible = data;
			trData5.Visible = data;
			trData6.Visible = data;

			bool comset = tab == "CommonSettings";
			trComSetH.Visible = comset;
			trComSet1.Visible = comset;
			trComSet2.Visible = comset;
			trComSet3.Visible = comset;
			trComSet4.Visible = comset;
			trComSet5.Visible = comset;
			trComSet6.Visible = comset;

			bool helpdesk = tab == "HelpDesk";
			trHelpDeskH.Visible = helpdesk;
			trHelpDesk1.Visible = helpdesk;
			trHelpDesk2.Visible = helpdesk;
			trHelpDesk3.Visible = helpdesk;
			trHelpDesk4.Visible = helpdesk;
			trHelpDesk5.Visible = helpdesk;

			bool filesforms = tab == "FilesForms";
			trFilesFormsH.Visible = filesforms;
			trFilesForms1.Visible = filesforms;
			trFilesForms2.Visible = filesforms;

			bool addtools = tab == "AddTools";
			trAddToolsH.Visible = addtools;
			trAddTools1.Visible = addtools;
			trAddTools2.Visible = addtools;
			trAddTools3.Visible = addtools;
			trAddTools4.Visible = addtools;
			trAddTools5.Visible = addtools && PortalConfig.UseIM;
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
