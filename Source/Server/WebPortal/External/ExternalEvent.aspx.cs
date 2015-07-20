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
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.External
{
	/// <summary>
	/// Summary description for EventView.
	/// </summary>
	public partial class ExternalEvent : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			ApplyLocalization();
			if (!Security.CurrentUser.IsExternal)
				Response.Redirect("../logoff.aspx");
		}

		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strPageTitles", typeof(ExternalEvent).Assembly);
			pT.Title = LocRM.GetString("tEventView");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			if (!Mediachase.IBN.Business.CalendarEntry.CanRead(int.Parse(Request["EventId"])))
				Response.Redirect("~/External/MissingObject.aspx");

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
