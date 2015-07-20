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
using Mediachase.Ibn.Web.UI.WebControls;
using System.Text;

namespace Mediachase.UI.Web.Modules
{
	/// <summary>
	/// Summary description for ControlLoader.
	/// </summary>
	public partial class ControlLoader : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			RegisterScripts();
			Response.Cache.SetNoStore();
			BindControl();
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


		#region BindControl()
		private void BindControl()
		{
			string controlName = Request.QueryString["ctrl"];
			if (controlName != "")
			{
				System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
				phItems.Controls.Add(control);
			}
		}
		#endregion

		private void RegisterScripts()
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			UtilHelper.RegisterScriptBlock(Page, GenerateScript());
		}

		private string GenerateScript()
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine("	//<![CDATA[");
			builder.AppendLine("	function fnStartInit() {");
			builder.AppendLine("		if (document.readyState == 'complete') {");
			builder.AppendLine("			fnStartInitProcess();");
			builder.AppendLine("		}");
			builder.AppendLine("	}");
			builder.AppendLine("	function fnStartInitProcess() {");
			builder.AppendLine("		var obj = document.getElementById('" + tbImg.ClientID + "');");
			builder.AppendLine("		var obj1 = document.getElementById('" + tbMain.ClientID + "');");
			builder.AppendLine("		if (obj && obj.style && obj.style.display != 'none') {");
			builder.AppendLine("			obj.style.display = 'none';");
			builder.AppendLine("			obj1.style.display = '';");
			builder.AppendLine("		}");
			builder.AppendLine("	}");
			builder.AppendLine("	function HideMainMenu(e) {");
			builder.AppendLine("		if (parent && parent.HideMenu)");
			builder.AppendLine("			parent.HideMenu();");
			builder.AppendLine("	}");
			builder.AppendLine("	document.onclick = HideMainMenu;");
			builder.AppendLine("	//]]>");

			return builder.ToString();
		}
	}
}
