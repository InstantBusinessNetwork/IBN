namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Text;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;



	/// <summary>
	///		Summary description for ProjectReports.
	/// </summary>
	public partial  class ProjectReports : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell tdAssigned;
		//protected System.Web.UI.HtmlControls.HtmlAnchor A1;
		//protected System.Web.UI.HtmlControls.HtmlAnchor A2;
		//protected System.Web.UI.HtmlControls.HtmlAnchor A3;
		//protected System.Web.UI.HtmlControls.HtmlAnchor A4;
		//protected System.Web.UI.HtmlControls.HtmlAnchor A5;
		//protected System.Web.UI.HtmlControls.HtmlAnchor A6;
	//	protected System.Web.UI.HtmlControls.HtmlAnchor A7;
	//	protected System.Web.UI.HtmlControls.HtmlAnchor A8;
	//	protected System.Web.UI.HtmlControls.HtmlAnchor A9;
		protected System.Web.UI.WebControls.Label lblRepType;

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectReports", typeof(ProjectReports).Assembly);
		public int ProjId
		{
			get
			{
				try
				{
					return Request["ProjectId"] != null ? int.Parse(Request["ProjectId"]) : 0;
				}
				catch
				{
					return 0;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			trSnapshots.Visible = false;
			//if(!Page.IsPostBack)
			//{
				BindReportsLinks();
				BindTasksToDosGraph();
				BindResourcesGraph();
				BindToolBars();
			//}
			if(Configuration.HelpDeskEnabled)
				BindIssuesGraph();
			else
				tblIssues.Visible = false;
			// Put user code to initialize the page here
		}

		private void BindToolBars()
		{
			//Header1.Title = LocRM.GetString("tFinanceGraph");
			Header2.Title = LocRM.GetString("IssuesGraph");
			Header3.Title = LocRM.GetString("TasksToDosGraph");
			Header4.Title = LocRM.GetString("ResourceTasksGraph");
		}
		private void BindReportsLinks()
		{
			//A2.HRef = "~/Reports/OverallProjectSnapshot.aspx?ProjectId=" + ProjId.ToString();
			//A4.HRef = "~/Reports/ResourcesTracking.aspx?ProjectId=" + ProjId.ToString();

			//A5.HRef = "javascript:OpenWindow('../Reports/XMLReport.aspx?Type=Inc&ProjectID="+ProjId.ToString()+"',750,466,true)";
			//A6.HRef = "javascript:OpenWindow('../Reports/XMLReport.aspx?Type=Acts&ProjectID="+ProjId.ToString()+"',750,466,true)";
			//A9.HRef = "~/Reports/ProjectTimesheetTracking.aspx?ProjectId=" + ProjId.ToString();
			
			//imgFinanceGraph.Attributes.Add("border","0");
			//imgFinanceGraph.ImageUrl = "~/Modules/ChartImage.aspx?Type=DGist&PrjRep_Finance=1&ProjectId="+ProjId;//CommonHelper.ChartPath + gIssues.Draw();
		}

		private void BindIssuesGraph()
		{
			/*Gistogram gIssues = new Gistogram(Server.MapPath(CommonHelper.ChartPath),System.Drawing.Imaging.ImageFormat.Png);
			gIssues.Title		= LocRM.GetString("oIssuesGraphTitle");
			gIssues.Width		= 250;
			gIssues.Height		= 60;
			gIssues.Sort		= false;
			gIssues.BorderWidth = 0;
			

			int inew	= 0;
			int itotal	= 0;
			int iactive = 0;
			int iclosed = 0;
			int iopen	= 0;
			int ietc	= 0;
	

			using(IDataReader rdr = Incident.GetIncidentStatistic(ProjId))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll

				if (rdr.Read())
				{
					inew = (int)rdr["NewIncidentCount"];
					iactive = (int)rdr["ActiveIncidentCount"];
					itotal = (int)rdr["IncidentCount"];
					iopen = inew + iactive;
					ietc = itotal - iopen;
					iclosed = (int)rdr["ClosedIncidentCount"];
				}
			}

			if(itotal>0) 
			{
				string[] titles = new string[] {LocRM.GetString("oNew"),LocRM.GetString("oActive"),LocRM.GetString("oOpen"),LocRM.GetString("oClosed")};
				double[] values = new double[] {Convert.ToDouble(inew),Convert.ToDouble(iactive),Convert.ToDouble(iopen),Convert.ToDouble(iclosed)};
				gIssues.CollectDataPoints(titles, values);
			}
			else
				gIssues.CollectDataPoints(new string[0],new double[0]);*/
				
			imgIssuesGraph.Attributes.Add("border","0");
			imgIssuesGraph.ImageUrl = "~/Modules/ChartImage.aspx?Type=Gist&PrjRep_Issues=1&ProjectId="+ProjId;//CommonHelper.ChartPath + gIssues.Draw();
		}

		private void BindTasksToDosGraph()
		{
			/*PieChart pcTaskTodos = new PieChart(Server.MapPath(CommonHelper.ChartPath),System.Drawing.Imaging.ImageFormat.Png);
			pcTaskTodos.Title		=  LocRM.GetString("oTaksToDosGraphTitle");
			
			pcTaskTodos.Width		= 300;
			pcTaskTodos.Radius		= 110;
			pcTaskTodos.BorderWidth = 0;
			
			int itotal		= 0;
			int icompleted	= 0;
			int iactive		= 0;
			int iupcoming	= 0;
			int ipastdue	= 0;

			using(IDataReader rdr = Report.GetToDoAndTaskTrackingReport(ProjId))
			{
				/// Total, Completed, PastDue, Active, Upcoming
				if (rdr.Read())
				{
					itotal		= (int)rdr["Total"];
					icompleted	= (int)rdr["Completed"];
					iactive		= (int)rdr["Active"];
					iupcoming	= (int)rdr["Upcoming"];
					ipastdue	= (int)rdr["PastDue"];
				}
			}

			if(itotal>0) 
			{
				string[] titles = new string[] {LocRM.GetString("oUpcoming"),LocRM.GetString("oActiveTasks"),LocRM.GetString("oCompleted"),LocRM.GetString("oPastDue")};
				double[] values = new double[] {Convert.ToDouble(iupcoming),Convert.ToDouble(iactive-ipastdue),Convert.ToDouble(icompleted),Convert.ToDouble(ipastdue)};
				pcTaskTodos.CollectDataPoints(titles, values);
			}
			else
				pcTaskTodos.CollectDataPoints(new string[0],new double[0]);*/
			imgTasksToDosGraph.Attributes.Add("border","0");
			imgTasksToDosGraph.ImageUrl = "~/Modules/ChartImage.aspx?Type=Pie&PrjRep_TaskToDo=1&ProjectId="+ProjId;//CommonHelper.ChartPath + pcTaskTodos.Draw();
		}

		private void BindResourcesGraph()
		{
			

			DataTable dt = ToDo.GetListToDoAndTaskResourcesCountDataTable(ProjId);
		

			if (dt.Rows.Count >0)
				BuildTable(dt);
			else
			{
				lblTable.Text = "<tr><td colspan ='4'>" + LocRM.GetString("tNoRec") + "</td></tr>";
				trLegend.Visible=false;
			}
		}

		#region BuildTable
		private void BuildTable(DataTable dt)
		{
			StringBuilder sb = new StringBuilder();
			/// UserId, TaskTodoCount, FirstName, LastName

			int index = 1;
			int hvalue = 0;
			
			int maxvalue = 0;
			foreach(DataRow dr in dt.Rows)
				if ((int)dr["TaskTodoCount"] > maxvalue)
					maxvalue = (int)dr["TaskTodoCount"];

			if (maxvalue<=10)
				hvalue = 10;
			else
			{
				int div = (int)Math.Pow(10,(int)Math.Log10(maxvalue));
				hvalue = (maxvalue/div)*div + div;
			}

			foreach (DataRow dr in dt.Rows)
			{
				int val = (int)dr["TaskTodoCount"];

				sb.Append("<tr height=14><td width='30%'>");
				sb.Append(index.ToString() + ". " + dr["LastName"] + " " + dr["FirstName"]);
				sb.Append("</td><td colspan='3' ");

				if (index == 1 && dt.Rows.Count > 1)
					sb.Append("class='top' ");

				if (index == dt.Rows.Count && dt.Rows.Count>1)
					sb.Append("class='bottom' ");

				if (index == 1 && dt.Rows.Count == 1)
					sb.Append("class='all' ");

				else
					sb.Append("class='leftright' ");

				sb.Append("valign='center'>");
				sb.Append("<img alt='' src='../layouts/images/point.gif' height='10' width='" + ((val * 100) / (hvalue) - 4) + "%' border='0' align='absMiddle'/>");
				sb.Append("&nbsp;" + val.ToString());
				sb.Append("</td></tr>");

				index++;
			}
			lblTable.Text = sb.ToString();
			lblMaxValue.Text = hvalue.ToString();
			trLegend.Visible=true;
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
