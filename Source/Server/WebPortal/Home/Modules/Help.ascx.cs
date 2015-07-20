using System;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Home.Modules
{
	/// <summary>
	/// Summary description for Help.
	/// </summary>
	public partial class Help : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblHelpText;
		protected string _quickHelp = "";
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Home.Resources.strWorkspace", typeof(Help).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			secHeader.Title = LocRM.GetString("tHelpFeedbackTitle");

			string botText = Mediachase.Ibn.Web.UI.CHelper.GetResFileString(GlobalResourceManager.Strings["HelpBottomTextKey"]);
			if (botText.IndexOf("{0}") >= 0)
				botText = String.Format(botText, GlobalResourceManager.Strings["SupportEmail"]);
			lblForMe.Text = botText;
			if (!String.IsNullOrEmpty(GlobalResourceManager.Strings["SupportPhone"]))
			{
				botText = Mediachase.Ibn.Web.UI.CHelper.GetResFileString(GlobalResourceManager.Strings["HelpBottomPhoneTextKey"]);
				if (botText.IndexOf("{0}") >= 0)
					botText = String.Format(botText, GlobalResourceManager.Strings["SupportPhone"]);
				lblForMe.Text += botText;
			}

			hlDocument.NavigateUrl = GlobalResourceManager.Strings["DocumentationLink"];
			if (GlobalResourceManager.Options["ShowQuickHelpLink"] && !String.IsNullOrEmpty(GlobalResourceManager.Strings["QuickHelpTextKey"]))
			{
				_quickHelp = "<img alt='' src='../layouts/images/quickhelp.gif' style='padding-right:10px;vertical-align:middle' />" + CommonHelper.LinkToBlank(GlobalResourceManager.Strings["QuickHelpLink"], string.Format("<span style='color:red'>{0}</span>", Mediachase.Ibn.Web.UI.CHelper.GetResFileString(GlobalResourceManager.Strings["QuickHelpTextKey"])));
			}
			else
				trQuickHelp1.Visible = false;
			if (GlobalResourceManager.Options["ShowTutorialLink"])
				hlTextBook.NavigateUrl = GlobalResourceManager.Strings["TutorialLink"];
			else
				trTutorial.Visible = false;
			hlForums.NavigateUrl = GlobalResourceManager.Strings["ForumsLink"];
			hlSupport.NavigateUrl = "mailto:" + GlobalResourceManager.Strings["SupportEmail"];

			trDocumentation.Visible = GlobalResourceManager.Options["ShowDocumentationLink"];
			trForum.Visible = GlobalResourceManager.Options["ShowForumLink"];
			trSupport.Visible = GlobalResourceManager.Options["ShowSupportLink"];

			//ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.Web.Resources.Admin.Resources.strTestRes", System.Reflection.Assembly.Load(new System.Reflection.AssemblyName("Mediachase.Ibn.Web")));
			//lblForMe.Text += LocRM2.GetString("strTest");
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
