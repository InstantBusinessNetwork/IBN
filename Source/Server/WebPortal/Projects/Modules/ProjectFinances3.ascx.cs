namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Text;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Resources;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.SpreadSheet;

	/// <summary>
	///		Summary description for ProjectFinances3.
	/// </summary>
	public partial class ProjectFinances3 : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(ProjectFinances3).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectFinances3).Assembly);
		protected Mediachase.IBN.Business.UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		private string pcKey = "PrjFin_CurTab";

		#region ProjectId
		private int ProjectId
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region SubTab
		private string subTab;
		protected string SubTab
		{
			get 
			{
				if (subTab == null)
				{
					subTab = "";
					if (Request["SubTab"] != null)
						subTab = Request["SubTab"];
				}
				return subTab;
			}
		}
		#endregion



		protected void Page_Load(object sender, System.EventArgs e)
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			
			// Add Tabs
			string link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=4&SubTab=0", ProjectId);
			bool IsFinance41 = (Finance.GetListActualFinancesByProject(int.Parse(this.Request["ProjectId"]), false).Rows.Count > 0);
			blockControl.AddTab("0", LocRM.GetString("tSpreadSheet"), link, "../Projects/Modules/FinanceSpreadSheet.ascx");			

			if ((SubTab == "0" || SubTab == "1" || SubTab == "2"))
				pc[pcKey] = SubTab;
			else if (pc[pcKey] == null)
				pc[pcKey] = "0";

			bool projectSpreadSheetIsActive = ProjectSpreadSheet.IsActive(int.Parse(this.Request["ProjectId"]));
			if (!projectSpreadSheetIsActive)
			{
				link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=4&SubTab=1", ProjectId);
				if (IsFinance41)
					blockControl.AddTab("1", LocRM.GetString("tActFinances"), link, "../Projects/Modules/FinanceActualList.ascx");

				if (pc[pcKey] != "2" && !IsFinance41)
					pc[pcKey] = "0";
			}
			else
			{
				//blockControl.AddTab("0", LocRM.GetString("tSpreadSheet"), link, "../Projects/Modules/FinanceSpreadSheet.ascx");

				link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=4&SubTab=1", ProjectId);
				blockControl.AddTab("1", LocRM.GetString("tActFinances"), link, "../Projects/Modules/FinanceActualList2.ascx");
			}

			//IBN 4.1 Finances
			if (IsFinance41 && !projectSpreadSheetIsActive)
			{
				link = String.Format("ProjectView.aspx?ProjectId={0}&Tab=4&SubTab=2", ProjectId);
				blockControl.AddTab("2", LocRM.GetString("tAccounts"), link, "../Projects/Modules/FinanceAccountsList.ascx");
			}

			// Select Tab
			blockControl.SelectTab(pc[pcKey]);
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion


	}
}
