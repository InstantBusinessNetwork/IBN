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

namespace Mediachase.UI.Web.Tasks
{
	/// <summary>
	/// Summary description for TaskEdit.
	/// </summary>
	public partial class TaskEdit : System.Web.UI.Page
	{
		private int TID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(Request["TaskID"]!=null) TID = int.Parse(Request["TaskID"]);
			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strPageTitles", typeof(TaskEdit).Assembly);
			if(TID!=0)
				pT.Title = LocRM.GetString("tTaskEdit");
			else
				pT.Title = LocRM.GetString("tTaskAdd");
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
