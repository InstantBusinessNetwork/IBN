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
using System.Threading;
using System.Configuration;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
	/// <summary>
	/// Summary description for CheckDomain.
	/// </summary>
	public partial class CheckDomain : System.Web.UI.Page
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebTrial.App_GlobalResources.Resources.Resources", typeof(CheckDomain).Assembly);
		private string _name
		{
			get
			{
				return Request["name"];
			}
		}

		private string _locale
		{
			get
			{
				return Request["locale"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (_locale!=null && _locale!=String.Empty)
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(_locale);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(_locale);
			}
			Response.ContentType = "text/plain";
			string sDomain = string.Format("{0}.{1}", _name, TrialHelper.GetParentDomain());
			bool isExists = TrialHelper.DomainExists(sDomain);
			if(isExists)
				Response.Write("1"/*LocRM.GetString("strDomainExists")*/); 
			else
				Response.Write("0"/*LocRM.GetString("strDomainNotExists")*/); 
			
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
