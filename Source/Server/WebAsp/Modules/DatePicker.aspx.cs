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

using Mediachase.Web.UI.WebControls;


namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	/// Summary description for DatePicker.
	/// </summary>
	public partial class DatePicker : System.Web.UI.Page
	{
		//private BrowserLevelChecker _BrowserLevelChecker;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			//_BrowserLevelChecker = new BrowserLevelChecker("ie", 5, 0.5, true); 
			// Put user code to initialize the page here
			if(!Page.IsPostBack)
			{
				//Calendar1.SelectedDate = 
			}
			
		}

		protected void Calendar1_SelectionChanged(Object sender, System.EventArgs e)
		{
	
			//System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
			string strjscript = "<script language='javascript'>try{";
			if(new BrowserLevelChecker("ie", 5, 0.5, true).IsUpLevelBrowser(this.Context))
			{

				strjscript += "window.opener.document.getElementById('" + HttpContext.Current.Request.QueryString["formname"] + "')";
				strjscript += ".value = '" + Calendar1.SelectedDate.ToShortDateString() + "';";
			}
			else if(new BrowserLevelChecker("netscape", 7, 0, false).IsUpLevelBrowser(this.Context))
			{
				strjscript += "window.opener.document.getElementById(\"" + HttpContext.Current.Request.QueryString["formname"] + "\")";
				strjscript += ".value = \"" + Calendar1.SelectedDate.ToShortDateString() + "\";";
			}
			else
			{
				strjscript += "window.opener.document.getElementById(\"" + HttpContext.Current.Request.QueryString["formname"] + "\")";
				strjscript += ".value = \"" + Calendar1.SelectedDate.ToShortDateString() + "\";";
			}

			strjscript += "self.close();";
			strjscript += "} catch(e) {}</script" + ">";
			
			string DateickerIncludeScriptKey = "DatePickerIncludeScript";
			if(!Page.ClientScript.IsClientScriptBlockRegistered(DateickerIncludeScriptKey))
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), DateickerIncludeScriptKey, strjscript);
			//Response.Write(strjscript);
			//Literal1.Text = strjscript;
		}
    
    
		protected void Calendar1_DayRender(Object source, DayRenderEventArgs e)
		{
			if (e.Day.Date.ToString("d") == DateTime.Now.ToString("d"))
			{
				e.Cell.BackColor = System.Drawing.Color.LightGray;
			}
    
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
