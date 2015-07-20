namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for Workspaceview.
	/// </summary>
	public partial  class Workspaceview : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Control ctrl;

			TableRow tr = new TableRow();
			double RightWidth = 100;
			TableCell LeftColumn = new TableCell();
			LeftColumn.VerticalAlign = VerticalAlign.Top;
			LeftColumn.Attributes.Add("style","padding-right:7px");
			ctrl = LoadControl("~/Projects/modules/WorkspaceThisWeek.ascx");
			LeftColumn.Controls.Add(ctrl);

			tr.Cells.Add(LeftColumn);
			RightWidth = 70;

			TableCell RightColumn = new TableCell();
			RightColumn.VerticalAlign = VerticalAlign.Top;
			RightColumn.Width = Unit.Percentage(RightWidth);

			ctrl = LoadControl("~/Projects/modules/ResourceAssignments.ascx");
			RightColumn.Controls.Add(ctrl);

			ctrl = LoadControl("~/Projects/modules/WorkSpacePTTI.ascx");
			RightColumn.Controls.Add(ctrl);

			ctrl = LoadControl("~/Projects/modules/LatestMyUpdates.ascx");
			RightColumn.Controls.Add(ctrl);

			ctrl = LoadControl(@"~/Projects/modules/LatestAllUpdates.ascx");
			RightColumn.Controls.Add(ctrl);

			tr.Cells.Add(RightColumn);

			tblWorkspaceLayout.Rows.Add(tr);
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
