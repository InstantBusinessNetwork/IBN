namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Data;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Resources;
	using System.Threading;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn;
	using Mediachase.Ibn.Web.Interfaces;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for PageTemplate.
	/// </summary>
	public partial class PageTemplateExternal : System.Web.UI.UserControl
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

		protected string Is24Hours = "0";
		protected string TimeOffset = "0";

		protected string sTitle = String.Format(" | {0} {1}", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			iconIBN.Href = Page.ResolveUrl("~/portal.ico");
			RegisterScripts();

			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(PageTemplateExternal).Assembly);
			tableContactUs.Visible = GlobalResourceManager.Options["TableContactUsVisible"];
			lblProductName.Text = IbnConst.ProductFamily;

			if (Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator == String.Empty)
				Is24Hours = "1";
			//Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			//  "var Is24Hours = true;", true);

			int offset = User.GetCurrentBias();
			TimeOffset = offset.ToString();
			//Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			//  "var TimeOffset = " + offset + ";", true);

			// Default Settings
			printScript.Visible = false;
			frmMain.Visible = true;

			body.Attributes.Add("onload", "javascript:ScrollIt()");
			body.Attributes.Add("onscroll", "javascript:setcoords()");

			frmMain.Enctype = this.enctype;
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
			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated != false)
			{
				lblUser.Text = "[ " + Security.CurrentUser.FirstName + " " + Security.CurrentUser.LastName + " ]";
				lblTime.Text = ", " + CommonHelper.GetStringTimeZoneOffset(Security.CurrentUser.TimeZoneId);
			}
		}
		#endregion

		#region RegisterScripts()
		private void RegisterScripts()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/IbnFramework/JsDebug.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/toptabsnew.js");

			CommonHelper.RegisterScrollScript(Page, PageX.ClientID, PageY.ClientID);
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
		}
		#endregion
	}
}