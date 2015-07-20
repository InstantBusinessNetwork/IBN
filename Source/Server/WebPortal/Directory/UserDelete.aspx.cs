using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using System.Globalization;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Directory
{
	/// <summary>
	/// Summary description for UserDelete.
	/// </summary>
	public partial class UserDelete : System.Web.UI.Page
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strDeleteUser", typeof(UserDelete).Assembly);
	
		private int UserID
		{
			get
			{
				try
				{
					return int.Parse(Request["UserID"]);
				}
				catch
				{
					return 0;
				}
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			String controlname = String.Empty;
			if (UserID!=0)
			{
				if (Mediachase.IBN.Business.User.CheckForChangeableRoles(UserID))
					controlname = "ChangebleRoles.ascx";
				else 
					if (Mediachase.IBN.Business.User.CheckForUnchangeableRoles(UserID))
						controlname = "UnchangebleRoles.ascx";
					else
						controlname = "SimpleDelete.ascx";
			}
			Control ctrl = LoadControl(String.Concat("../Directory/Modules/",controlname));
			if (ctrl!=null)
				phDelete.Controls.Add(ctrl);
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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
