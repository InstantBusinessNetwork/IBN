using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Modules
{
	/// <summary>
	///		Summary description for BottomCopyright.
	/// </summary>
	public partial class BottomCopyright : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(BottomCopyright).Assembly);

		bool _Russian = false;

		public bool IsRussian
		{
			get
			{
				return _Russian;
			}

			set
			{
				_Russian = value;
			}
		}

		bool _ShowTopLine = false;
		public bool ShowTopLine
		{
			get
			{
				return _ShowTopLine;
			}

			set
			{
				_ShowTopLine = value;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			loggedincopyright.Visible = ShowTopLine;
			lblVersion.Text = LocRM.GetString("tVersion") + " " + IbnConst.FullVersion;

			link1.HRef = GlobalResourceManager.Strings["CompanyWebUrl"];
			lblCopyright.Text = GlobalResourceManager.Strings["CopyrightText"];

			if (GlobalResourceManager.Options["PrivacyLinkVisible"])
			{
				link3.HRef = String.Format("javascript:OpenPopUpWindow('{0}',650,400)",
					ResolveClientUrl(GlobalResourceManager.Strings["PrivacyLink"]));
				lblPrivacy.Text = CHelper.GetResFileString(GlobalResourceManager.Strings["PrivacyStatementTextKey"]);
			}
			else
				lblPrivacy.Visible = false;

			if (GlobalResourceManager.Options["TermsOfUseVisible"])
			{
				link2.HRef = String.Format("javascript:OpenPopUpWindow('{0}',650,400)",
					ResolveClientUrl(GlobalResourceManager.Strings["TermsOfUseLink"]));
				lblTermsOfUse.Text = CHelper.GetResFileString(GlobalResourceManager.Strings["TermsOfUseTextKey"]);
			}
			else
				link2.Visible = false;

			if (!GlobalResourceManager.Options["LoggedInCopyrightVisible"])
				loggedincopyright.Visible = false;

			lblVersion.Text += GlobalResourceManager.Strings["VersionPostfix"];
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
