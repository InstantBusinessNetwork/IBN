using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

using Mediachase.Ibn.Configuration;
using System.Text;


namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class PageTemplatePublic : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;

		private String _SelectedMenu = "0";

		private string ControlProperty;
		private string ControlPropertyValue;

		private string title = "";

		public string Title
		{
			set
			{
				title = value;
				lblTitle.Text = title;
			}
			get
			{
				return title;
			}
		}

		private string controlName = "";

		public void SetControlProperties(string cp, string cv)
		{
			ControlProperty = cp;
			ControlPropertyValue = cv;
		}

		public string ControlName
		{
			set
			{
				controlName = value;
			}
			get
			{
				return controlName;
			}
		}

		private string enctype = "application/x-www-form-urlencoded";
		public string Enctype
		{
			set { enctype = value; }
			get { return enctype; }
		}

		public String SelectedMenu
		{
			set
			{
				_SelectedMenu = value;
			}
			get
			{
				return _SelectedMenu;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Layouts/en-US/styles/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Themes/XP/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/Asp.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Layouts/en-US/browser.js");
			RegisterScrollScript(Page, PageX.ClientID, PageY.ClientID);

			Response.Cache.SetNoStore();

			LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Template", typeof(PageTemplatePublic).Assembly);

			// Default Settings
			printScript.Visible = false;
			frmMain.Visible = true;
			lblSiteName.Text = LocRM.GetString("ISPAdmin");

			body.Attributes.Add("onload", "javascript:ScrollIt()");

			//frmMain.Enctype = this.enctype;
			if (controlName != "")
			{
				System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
				if (ControlProperty != null && ControlPropertyValue != null)
				{
					control.GetType().BaseType.GetProperty(ControlProperty).SetValue(control, ControlPropertyValue, null);
				}

				phMain.Controls.Add(control);

				if (control is IPageTemplateTitle)
				{
					this.Title = ((IPageTemplateTitle)control).Modify(this.Title);
				}
			}

			BindData();
		}

		#region BindData()
		private void BindData()
		{
		}
		#endregion

		private void RegisterPrintScripts()
		{
			if (new Mediachase.Web.UI.WebControls.BrowserLevelChecker("netscape", 6, 0, false).IsUpLevelBrowser(this.Context))
				phPrintScripts.Visible = false;
		}

		#region public static void RegisterScrollScript(Page page, string pageXClientId, string pageYClientId)
		public static void RegisterScrollScript(Page page, string pageXClientId, string pageYClientId)
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine("	//<![CDATA[");
			builder.AppendLine("	function ScrollIt() {");
			builder.AppendLine("		window.scrollTo(document.forms[0]." + pageXClientId + ".value, document.forms[0]." + pageYClientId + ".value);");
			builder.AppendLine("	}");
			builder.AppendLine("	function setcoords() {");
			builder.AppendLine("		var myPageX;");
			builder.AppendLine("		var myPageY;");
			builder.AppendLine("		if (document.all) {");
			builder.AppendLine("			myPageX = document.body.scrollLeft;");
			builder.AppendLine("			myPageY = document.body.scrollTop;");
			builder.AppendLine("		}");
			builder.AppendLine("		else {");
			builder.AppendLine("			myPageX = window.pageXOffset;");
			builder.AppendLine("			myPageY = window.pageYOffset;");
			builder.AppendLine("		}");
			builder.AppendLine("		document.forms[0]." + pageXClientId + ".value = myPageX;");
			builder.AppendLine("		document.forms[0]." + pageYClientId + ".value = myPageY;");
			builder.AppendLine("	}");
			builder.AppendLine("	//]]>");

			UtilHelper.RegisterScriptBlock(page, builder.ToString(), true);
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSearch.Click += new System.Web.UI.ImageClickEventHandler(this.btnSearch_Click);

		}
		#endregion

		private void btnSearch_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			string keyword = tbSearchstr.Text;
			if (keyword != string.Empty)
				Response.Redirect("~/Pages/Search.aspx?keyword=" + HttpUtility.UrlEncode(keyword));
		}
	}
}
