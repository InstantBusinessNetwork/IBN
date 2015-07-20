namespace Mediachase.UI.Web.Workspace.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using System.Globalization;
	using System.Resources;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for workspace.
	/// </summary>
	public partial class workspace : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(workspace).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strPageTitles", typeof(workspace).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		private string BTab
		{
			get
			{
				return Request["BTab"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{

			if (Request["AdminWizard"] != null && Request["AdminWizard"].ToString() == "1" && !IsPostBack && Configuration.CompanyType == 1)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  @"<script language=javascript>
					OpenAdminWizard();
					function OpenAdminWizard()
					{
						var w = 650;var h = 450;
						var l = (screen.width - w) / 2;
						var t = (screen.height - h) / 2;
						winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
						f = window.open('../Wizards/FirstTimeLoginAdminWizard.aspx', 'AdminWizard', winprops);
						if (f == null)
						{

							tW = document.getElementById('tblWarning');
							aW = document.getElementById('aWizardLink');
							if (tW != null && aW !=null)
							{
								tW.style.display = 'block';
								aW.href = 'javascript:OpenAdminWizard()';
								aW.innerHTML = '" + LocRM.GetString("AdminWizard") + @"';
							}
						}
					}
				</script>");

			else if (Request["AdminWizard"] != null && Request["AdminWizard"].ToString() == "1" && !IsPostBack && Configuration.CompanyType == 2)
			{
				if (Security.CurrentUser.Culture.IndexOf("ru") >= 0)
					Response.Redirect("../Workspace/FirstInviteAdmin.aspx");
				else
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  @"<script language=javascript>
            OpenAdminWizard();
            function OpenAdminWizard()
            {
              var w = 600;var h = 400;
              var l = (screen.width - w) / 2;
              var t = (screen.height - h) / 2;
              winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
              f = window.open('../Wizards/FirstInviteWizard.aspx', 'AdminWizard', winprops);
              if (f == null)
              {

                tW = document.getElementById('tblWarning');
                aW = document.getElementById('aWizardLink');
                if (tW != null && aW !=null)
                {
                  tW.style.display = 'block';
                  aW.href = 'javascript:OpenAdminWizard()';
                  aW.innerHTML = '" + LocRM.GetString("AdminWizard") + @"';
                }
              }
            }
          </script>");
			}
			else if (Request["wizard"] != null && Request["wizard"].ToString() == "1" && !Page.IsPostBack)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  @"<script language=javascript>
					OpenUserWizard();
					function OpenUserWizard()
					{
						var w = 600;var h = 550;
						var l = (screen.width - w) / 2;
						var t = (screen.height - h) / 2;
						winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
						f = window.open('../Wizards/FirstTimeLoginWizard.aspx', 'UserWizard', winprops);
						if (f == null)
						{
							tW = document.getElementById('tblWarning');
							aW = document.getElementById('aWizardLink');
							if (tW != null && aW !=null)
							{
								tW.style.display = 'block';
								aW.href = 'javascript:OpenUserWizard();';
								aW.innerHTML = '" + LocRM.GetString("UserWizard") + @"';
							}
						}
					}
				</script>");
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
