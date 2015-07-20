namespace Mediachase.UI.Web.UserReports.GlobalModules
{
	using System;
	using System.Data;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.UI.Web.UserReports.GlobalModules.PageTemplateExtension;
	using System.Resources;
	using Mediachase.IBN.DefaultUserReports;

	//using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for ReportHeader.
	/// </summary>
	public partial  class ReportHeader : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.GlobalModules.Resources.strTemplate", typeof(ReportHeader).Assembly);
		private string title = "";
	
		public string Title
		{
			set 
			{
				title = value;
				ReportName.Text = title;
			}
			get 
			{
				return title;
			}
		}
		public bool ForPrintOnly
		{
			set
			{
				ViewState["ForPrintOnly"] = value;
			}
		}

		private string filter = "";
		public string Filter
		{
			set 
			{
				if(value!="")
					filter = LocRM.GetString("tFilterparameters")+" :"+ "<br/>" + value;
				lblFilter.Text = filter;
			}
			get 
			{
				return filter;
			}
		}

		public bool BtnPrintVisible
		{
			set
			{
				ViewState["BtnPrintVisible"] = value;
			}
		}

		private string reportcreator = Mediachase.IBN.Business.Security.CurrentUser.DisplayName;
		public string ReportCreator
		{
			set 
			{
				reportcreator = value;
			}
			get 
			{
				return reportcreator;
			}
		}

		private DateTime reportcreated = UserDateTime.Now;
		public DateTime ReportCreated
		{
			set 
			{
				reportcreated = value;
			}
			get 
			{
				return reportcreated;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnPrint.Value = LocRM.GetString("tPrint");
			if((ViewState["ForPrintOnly"] != null) && (bool)ViewState["ForPrintOnly"])
			{
				MainTable.Style.Add("display","none");
				MainTable.Attributes.Add("Printable","1");
				btnPrint.Visible = false;
			}
			if((ViewState["BtnPrintVisible"] != null) && !(bool)ViewState["BtnPrintVisible"])
			{
				btnPrint.Visible = false;
			}
			lblDateReport.Text = LocRM.GetString("tDateRun")+": " + ReportCreated.ToLongDateString();
			lblUser.Text = LocRM.GetString("tProducedby")+": " + ReportCreator;
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

	}
}
