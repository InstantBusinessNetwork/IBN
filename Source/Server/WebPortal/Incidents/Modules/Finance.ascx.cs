namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.UI.Web.Modules;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for Finance.
	/// </summary>
	public partial class Finance : System.Web.UI.UserControl, IToolbarLight
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			int ProjectId = Util.CommonHelper.GetProjectIdByObjectIdObjectType(int.Parse(Request["IncidentId"]), (int)ObjectTypes.Issue);
			bool IsFinance41 = (Mediachase.IBN.Business.Finance.GetListActualFinancesByProject(ProjectId, false).Rows.Count > 0);
			if (!Mediachase.IBN.Business.SpreadSheet.ProjectSpreadSheet.IsActive(ProjectId) && IsFinance41)
			{
				
				TableCell.Controls.Add((Mediachase.UI.Web.Projects.Modules.FinanceActualList)this.Page.LoadControl("../Projects/Modules/FinanceActualList.ascx"));
			}
			else
			{
				TableCell.Controls.Add((Mediachase.UI.Web.Projects.Modules.FinanceActualList2)this.Page.LoadControl("../Projects/Modules/FinanceActualList2.ascx"));
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region IToolbarLight Members
		BlockHeaderLightWithMenu Mediachase.UI.Web.Modules.IToolbarLight.GetToolBar()
		{
			return secHeader;
		}
		#endregion
	}
}
