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
	/// Summary description for _default.
	/// </summary>
	public partial class ExternalToDo : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		//	if (!Security.IsUserInGroup(InternalSecureGroups.External))
		//		Response.Redirect("../logoff.aspx");

			

			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strPageTitles", typeof(ExternalToDo).Assembly);
			pT.Title = LocRM.GetString("tToDoView");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			
			if (!Mediachase.IBN.Business.ToDo.CanRead(int.Parse(Request["ToDoID"])))
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
