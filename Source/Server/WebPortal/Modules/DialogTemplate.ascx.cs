namespace Mediachase.UI.Web.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for DialogTemplate.
	/// </summary>
	public partial class DialogTemplate : System.Web.UI.UserControl
	{
		private Hashtable controlProperties = new Hashtable();

		#region ControlName
		private string controlName = "";
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
		#endregion

		#region Title
		private string title = "";
		public string Title
		{
			set 
			{
				title = value;
			}
			get 
			{
				return title;
			}
		}
		#endregion

		#region Enctype
		private string enctype = "application/x-www-form-urlencoded"; 
		public string Enctype 
		{
			set {enctype = value;}
			get {return enctype;}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			iconIBN.Href = Page.ResolveUrl("~/portal.ico");
			RegisterScripts();

			Response.Cache.SetNoStore();
			mainForm.Enctype = this.Enctype;
			iconIBN.Href = ResolveUrl("~/portal.ico");
			
			if (controlName != "")
			{
				System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
				foreach (DictionaryEntry de in controlProperties)
				{
					control.GetType().BaseType.GetProperty(de.Key.ToString()).SetValue(control, de.Value, null);
				}
				phMain.Controls.Add(control);
			}
		}

		#region SetControlProperties
		public void SetControlProperties(string key, object value)
		{
			controlProperties.Add(key, value);
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
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if(phMain.Controls[0] is IPageTemplateTitle)
			{
				this.Title = ((IPageTemplateTitle)phMain.Controls[0]).Modify(this.Title);
			}
		}
		#endregion

		private void RegisterScripts()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/common.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
		}
	}
}
