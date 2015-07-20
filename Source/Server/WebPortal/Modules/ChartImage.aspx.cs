using System;
using System.Collections;
using System.Data;
using System.Resources;

using Mediachase.Ibn.Web.Drawing.Charting;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Modules
{
	/// <summary>
	/// Summary description for ChartImage.
	/// </summary>
	public partial class ChartImage : System.Web.UI.Page
	{
		protected ResourceManager LocRM;
		UserLightPropertyCollection prop_col = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "image/png";
			if (Request["Type"] != null && Request["Type"].ToString() == "Pie")
			{
				prop_col["Dashboard_GraphType"] = "0";
				BindPieChart();
			}
			else if (Request["Type"] != null && Request["Type"].ToString() == "Gist")
			{
				prop_col["Dashboard_GraphType"] = "1";
				BindBarChart();
			}
			else if (Request["Type"] != null && Request["Type"].ToString() == "DGist")
			{
				BindDGistogramm();
			}
			Response.End();
		}

		#region BindPieChart
		private void BindPieChart()
		{
			PieChart pc = new PieChart();

			if (Request["IncRep_Status"] != null)
				BindIncRep_Status(pc);
			else if (Request["IncRep_Type"] != null)
				BindIncRep_Type(pc);
			else if (Request["AllPrjRep_Status"] != null)
				AllPrjRep_Status(pc);
			else if (Request["AllPrjRep_PrjCat"] != null)
				AllPrjRep_PrjCat(pc);
			else if (Request["PrjRep_TaskToDo"] != null)
				PrjRep_TaskToDo(pc);
			else if (Request["IncStat_IncStatus"] != null)
				IncStat_IncStatus(pc);
			else if (Request["IncStat_IncType"] != null)
				IncStat_IncType(pc);
			else if (Request["IncStat_GenCat"] != null)
				IncStat_GenCat(pc);
			else if (Request["IncStat_IncCat"] != null)
				IncStat_IncCat(pc);
			else if (Request["IncStat_Man"] != null)
				IncStat_Man(pc);
			else if (Request["IncStat_Prj"] != null)
				IncStat_Prj(pc);
			else if (Request["IncAnalys_Status"] != null)
				IncAnalys_Status(pc);
			else if (Request["IncAnalys_Type"] != null)
				IncAnalys_Type(pc);
			else if (Request["IncAnalys_GenCat"] != null)
				IncAnalys_GenCat(pc);
			else if (Request["IncAnalysCat_Status"] != null)
				IncAnalysCat_Status(pc);
			else if (Request["IncAnalysCat_Type"] != null)
				IncAnalysCat_Type(pc);
			else if (Request["IncAnalysCat_GenCat"] != null)
				IncAnalysCat_GenCat(pc);
			else if (Request["IncAnalysMan_Status"] != null)
				IncAnalysMan_Status(pc);
			else if (Request["IncAnalysMan_Type"] != null)
				IncAnalysMan_Type(pc);
			else if (Request["IncAnalysMan_GenCat"] != null)
				IncAnalysMan_GenCat(pc);
			else if (Request["IncAnalysType_Status"] != null)
				IncAnalysType_Status(pc);
			else if (Request["IncAnalysType_Type"] != null)
				IncAnalysType_Type(pc);
			else if (Request["IncAnalysType_GenCat"] != null)
				IncAnalysType_GenCat(pc);
			else if (Request["IncClosed_Status"] != null)
				IncClosed_Status(pc);
			else if (Request["IncClosed_Type"] != null)
				IncClosed_Type(pc);
			else if (Request["IncClosed_GenCat"] != null)
				IncClosed_GenCat(pc);
			else if (Request["IncAnalysPrj_Status"] != null)
				IncAnalysPrj_Status(pc);
			else if (Request["IncAnalysPrj_Type"] != null)
				IncAnalysPrj_Type(pc);
			else if (Request["IncAnalysPrj_GenCat"] != null)
				IncAnalysPrj_GenCat(pc);
			else if (Request["OverHDIss_Status"] != null)
				OverHDIss_Status(pc);
			else if (Request["OverHDIss_Type"] != null)
				OverHDIss_Type(pc);
			else if (Request["OverHDIss_GenCat"] != null)
				OverHDIss_GenCat(pc);
			else if (Request["PrjStat_Status"] != null)
				PrjStat_Status(pc);
			else if (Request["PrjStat_Type"] != null)
				PrjStat_Type(pc);
			else if (Request["PrjStat_GenCat"] != null)
				PrjStat_GenCat(pc);
			else if (Request["PrjStat_PrjCat"] != null)
				PrjStat_PrjCat(pc);
			else if (Request["PrjStat_Man"] != null)
				PrjStat_Man(pc);
			//////////////////Dashboard - Projects////////////////////////////////////////////////////////
			else if (Request["Dash_Prj_Stat"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_Stat");
			else if (Request["Dash_Prj_Prior"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_Prior");
			else if (Request["Dash_Prj_Typ"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_Typ");
			else if (Request["Dash_Prj_Man"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_Man");
			else if (Request["Dash_Prj_GenCat"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_GenCat");
			else if (Request["Dash_Prj_PrjCat"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_PrjCat");
			else if (Request["Dash_Prj_PrjGrp"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_PrjGrp");
			else if (Request["Dash_Prj_Phas"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_Phas");
			else if (Request["Dash_Prj_Clnt"] != null)
				Dash_Prj_Item_Pie(pc, "Dash_Prj_Clnt");
			//////////////////Dashboard - Issues////////////////////////////////////////////////////////
			else if (Request["Dash_Iss_Stat"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_Stat");
			else if (Request["Dash_Iss_Prior"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_Prior");
			else if (Request["Dash_Iss_Typ"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_Typ");
			else if (Request["Dash_Iss_Prj"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_Prj");
			else if (Request["Dash_Iss_Man"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_Man");
			else if (Request["Dash_Iss_Sev"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_Sev");
			else if (Request["Dash_Iss_GenCat"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_GenCat");
			else if (Request["Dash_Iss_IssCat"] != null)
				Dash_Iss_Item_Pie(pc, "Dash_Iss_IssCat");

			System.IO.MemoryStream tmpStream = new System.IO.MemoryStream();
			pc.Draw(tmpStream, System.Drawing.Imaging.ImageFormat.Png);
			byte[] tmpBuffer = tmpStream.GetBuffer();
			this.Response.OutputStream.Write(tmpBuffer, 0, tmpBuffer.Length);
		}
		#endregion

		#region private void BindBarChart()
		private void BindBarChart()
		{
			BarChart barChart = new BarChart();

			if (Request["PrjRep_Issues"] != null)
				PrjRep_Issues(barChart);
			else if (Request["OverPrjSnap_Issues"] != null)
				OverPrjSnap_Issues(barChart);
			else if (Request["OverPrjSnap_TaskToDo"] != null)
				OverPrjSnap_TaskToDo(barChart);
			////////////Dashboard//////////////////////////////////////////////////////////////
			else if (Request["Dash_Prj_Stat"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_Stat");
			else if (Request["Dash_Prj_Prior"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_Prior");
			else if (Request["Dash_Prj_Typ"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_Typ");
			else if (Request["Dash_Prj_Man"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_Man");
			else if (Request["Dash_Prj_GenCat"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_GenCat");
			else if (Request["Dash_Prj_PrjCat"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_PrjCat");
			else if (Request["Dash_Prj_PrjGrp"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_PrjGrp");
			else if (Request["Dash_Prj_Phas"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_Phas");
			else if (Request["Dash_Prj_Clnt"] != null)
				Dash_Prj_Item_Gist(barChart, "Dash_Prj_Clnt");
			//////////////////Dashboard - Issues////////////////////////////////////////////////////////
			else if (Request["Dash_Iss_Stat"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_Stat");
			else if (Request["Dash_Iss_Prior"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_Prior");
			else if (Request["Dash_Iss_Typ"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_Typ");
			else if (Request["Dash_Iss_Prj"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_Prj");
			else if (Request["Dash_Iss_Man"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_Man");
			else if (Request["Dash_Iss_Sev"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_Sev");
			else if (Request["Dash_Iss_GenCat"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_GenCat");
			else if (Request["Dash_Iss_IssCat"] != null)
				Dash_Iss_Item_Gist(barChart, "Dash_Iss_IssCat");

			System.IO.MemoryStream tmpStream = new System.IO.MemoryStream();
			barChart.Draw(tmpStream, System.Drawing.Imaging.ImageFormat.Png);
			byte[] tmpBuffer = tmpStream.GetBuffer();
			this.Response.OutputStream.Write(tmpBuffer, 0, tmpBuffer.Length);
		}
		#endregion

		#region BindDGistogramm
		private void BindDGistogramm()
		{
			GraphicImage dgist = new GraphicImage();

			if (Request["PrjRep_Finance"] != null)
				PrjRep_Finance(dgist);

			System.IO.MemoryStream tmpStream = new System.IO.MemoryStream();
			dgist.Render(tmpStream, System.Drawing.Imaging.ImageFormat.Png);
			byte[] tmpBuffer = tmpStream.GetBuffer();
			this.Response.OutputStream.Write(tmpBuffer, 0, tmpBuffer.Length);
		}
		#endregion

		///////////////////////////////////////////////////////////////////////////////////////

		#region PrjRep_Finance
		private void PrjRep_Finance(GraphicImage dgist)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectReports", typeof(ChartImage).Assembly);
			int ProjId = int.Parse(Request["ProjectId"].ToString());
			DataTable dt = Finance.GetListTopLevelAccountsDataTable(ProjId);

			dgist.ImageWidth = 250;
			dgist.ImageHeight = 250;
			dgist.ChartItemsLName = LocRM.GetString("tTargetBudget");
			dgist.ChartItemsRName = LocRM.GetString("tActualBudget");

			string[] titles = new string[dt.Rows.Count + 1];
			double[] values1 = new double[dt.Rows.Count + 1];
			double[] values2 = new double[dt.Rows.Count + 1];
			titles[0] = "";
			values1[0] = 0;
			values2[0] = 0;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if ((int)dt.Rows[i]["OutlineLevel"] == 1)
					titles[i + 1] = LocRM.GetString("tOther");
				else
					titles[i + 1] = dt.Rows[i]["Title"].ToString();
				values1[i + 1] = Convert.ToDouble(dt.Rows[i]["Target"]);
				values2[i + 1] = Convert.ToDouble(dt.Rows[i]["Actual"]);
			}
			dgist.CollectDataPoints(titles, values1, values2);
		}
		#endregion

		#region Dash_Prj_Item_Pie
		private void Dash_Prj_Item_Pie(PieChart pc, string sItem)
		{
			prop_col["Dashboard_GraphView"] = sItem;
			pc.BorderWidth = 0;
			pc.DrawPercents = true;
			pc.MinPercentToDraw = 5;

			DataTable dt = new DataTable();

			switch (sItem)
			{
				case "Dash_Prj_Stat":
					dt = Project.GetProjectStatisticByStatusDataTable();
					break;
				case "Dash_Prj_Prior":
					dt = Project.GetProjectStatisticByPriorityDataTable();
					break;
				case "Dash_Prj_Typ":
					dt = Project.GetProjectStatisticByTypeDataTable();
					break;
				case "Dash_Prj_Man":
					dt = Project.GetProjectStatisticByManagerDataTable();
					break;
				case "Dash_Prj_GenCat":
					dt = Project.GetProjectStatisticByGeneralCategoryDataTable();
					break;
				case "Dash_Prj_PrjCat":
					dt = Project.GetProjectStatisticByProjectCategoryDataTable();
					break;
				case "Dash_Prj_PrjGrp":
					dt = Project.GetProjectStatisticByProjectGroupDataTable();
					break;
				case "Dash_Prj_Phas":
					dt = Project.GetProjectStatisticByPhaseDataTable();
					break;
				case "Dash_Prj_Clnt":
					dt = Project.GetProjectStatisticByClientDataTable();
					break;
			}

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString() + " (" + dt.Rows[i]["Count"].ToString() + ")";
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region Dash_Prj_Item_Gist
		private void Dash_Prj_Item_Gist(BarChart barChart, string sItem)
		{
			prop_col["Dashboard_GraphView"] = sItem;

			//gist.Width		= 250;
			//gist.Height		= 60;
			barChart.Sort = false;
			barChart.BorderWidth = 0;

			DataTable dt = new DataTable();

			switch (sItem)
			{
				case "Dash_Prj_Stat":
					dt = Project.GetProjectStatisticByStatusDataTable();
					break;
				case "Dash_Prj_Prior":
					dt = Project.GetProjectStatisticByPriorityDataTable();
					break;
				case "Dash_Prj_Typ":
					dt = Project.GetProjectStatisticByTypeDataTable();
					break;
				case "Dash_Prj_Man":
					dt = Project.GetProjectStatisticByManagerDataTable();
					break;
				case "Dash_Prj_GenCat":
					dt = Project.GetProjectStatisticByGeneralCategoryDataTable();
					break;
				case "Dash_Prj_PrjCat":
					dt = Project.GetProjectStatisticByProjectCategoryDataTable();
					break;
				case "Dash_Prj_PrjGrp":
					dt = Project.GetProjectStatisticByProjectGroupDataTable();
					break;
				case "Dash_Prj_Phas":
					dt = Project.GetProjectStatisticByPhaseDataTable();
					break;
				case "Dash_Prj_Clnt":
					dt = Project.GetProjectStatisticByClientDataTable();
					break;
			}

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			barChart.CollectDataPoints(titles, values);
		}
		#endregion

		#region Dash_Iss_Item_Pie
		private void Dash_Iss_Item_Pie(PieChart pc, string sItem)
		{
			prop_col["Dashboard_GraphView"] = sItem;

			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(ChartImage).Assembly);

			pc.BorderWidth = 0;
			pc.DrawPercents = true;
			pc.MinPercentToDraw = 5;

			DataTable dt = new DataTable();
			bool _IsPrj = false;
			switch (sItem)
			{
				case "Dash_Iss_Stat":
					dt = Incident.GetListIncidentStatisticByStatusDataTable();
					DataRow[] _dr = dt.Select("ItemId=" + ((int)ObjectStates.ReOpen).ToString());
					if (_dr.Length > 0)
					{
						int iCount = (int)_dr[0]["Count"];
						dt.Rows.Remove(_dr[0]);
						_dr = dt.Select("ItemId=" + ((int)ObjectStates.Active).ToString());
						if (_dr.Length > 0)
						{
							_dr[0]["Count"] = (int)_dr[0]["Count"] + iCount;
						}
					}
					break;
				case "Dash_Iss_Prior":
					dt = Incident.GetListIncidentStatisticByPriorityDataTable();
					break;
				case "Dash_Iss_Typ":
					dt = Incident.GetListIncidentStatisticByTypeDataTable();
					break;
				case "Dash_Iss_Prj":
					dt = Incident.GetListIncidentStatisticByProjectDataTable();
					_IsPrj = true;
					break;
				case "Dash_Iss_Man": //Issue Box
					dt = Incident.GetListIncidentStatisticByIncidentBoxDataTable();
					break;
				case "Dash_Iss_Sev":
					dt = Incident.GetListIncidentStatisticBySeverityDataTable();
					break;
				case "Dash_Iss_GenCat":
					dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();
					break;
				case "Dash_Iss_IssCat":
					dt = Incident.GetListIncidentStatisticByIncidentCategoryDataTable();
					break;
			}

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (_IsPrj && dt.Rows[i]["ItemName"] == DBNull.Value)
					titles[i] = LocRM.GetString("tNoProject") + " (" + dt.Rows[i]["Count"].ToString() + ")";
				else
					titles[i] = dt.Rows[i]["ItemName"].ToString() + " (" + dt.Rows[i]["Count"].ToString() + ")";
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region Dash_Iss_Item_Gist
		private void Dash_Iss_Item_Gist(BarChart barChart, string sItem)
		{
			prop_col["Dashboard_GraphView"] = sItem;

			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(ChartImage).Assembly);

			barChart.Sort = false;
			barChart.BorderWidth = 0;

			DataTable dt = new DataTable();
			bool _IsPrj = false;
			switch (sItem)
			{
				case "Dash_Iss_Stat":
					dt = Incident.GetListIncidentStatisticByStatusDataTable();
					DataRow[] _dr = dt.Select("ItemId=" + ((int)ObjectStates.ReOpen).ToString());
					if (_dr.Length > 0)
					{
						int iCount = (int)_dr[0]["Count"];
						dt.Rows.Remove(_dr[0]);
						_dr = dt.Select("ItemId=" + ((int)ObjectStates.Active).ToString());
						if (_dr.Length > 0)
						{
							_dr[0]["Count"] = (int)_dr[0]["Count"] + iCount;
						}
					}
					break;
				case "Dash_Iss_Prior":
					dt = Incident.GetListIncidentStatisticByPriorityDataTable();
					break;
				case "Dash_Iss_Typ":
					dt = Incident.GetListIncidentStatisticByTypeDataTable();
					break;
				case "Dash_Iss_Prj":
					dt = Incident.GetListIncidentStatisticByProjectDataTable();
					_IsPrj = true;
					break;
				case "Dash_Iss_Man": //IssueBox
					dt = Incident.GetListIncidentStatisticByIncidentBoxDataTable();
					break;
				case "Dash_Iss_Sev":
					dt = Incident.GetListIncidentStatisticBySeverityDataTable();
					break;
				case "Dash_Iss_GenCat":
					dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();
					break;
				case "Dash_Iss_IssCat":
					dt = Incident.GetListIncidentStatisticByIncidentCategoryDataTable();
					break;
			}

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (_IsPrj && dt.Rows[i]["ItemName"] == DBNull.Value)
					titles[i] = LocRM.GetString("tNoProject");
				else
					titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			barChart.CollectDataPoints(titles, values);
		}
		#endregion

		///////////////////////////////////////////////////////////////////////////////////////

		#region BindIncRep_Status
		private void BindIncRep_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(ChartImage).Assembly);

			pc.BorderWidth = 0;
			pc.Radius = 110;

			int iTotal = 0;
			int iNew = 0;
			int iActive = 0;
			int iClosed = 0;

			using (IDataReader reader = Incident.GetIncidentStatistic(0))
			{
				if (reader.Read())
				{
					iTotal = (int)reader["IncidentCount"];
					iNew = (int)reader["NewIncidentCount"];
					iActive = (int)reader["ActiveIncidentCount"];
					iClosed = (int)reader["ClosedIncidentCount"];
				}
			}

			if (iTotal > 0)
			{
				string[] titles = new string[] { LocRM.GetString("oClosed"), LocRM.GetString("oNew"), LocRM.GetString("oActive") };
				double[] values = new double[] { Convert.ToDouble(iClosed), Convert.ToDouble(iNew), Convert.ToDouble(iActive) };
				pc.CollectDataPoints(titles, values);
			}
			else
				pc.CollectDataPoints(new string[0], new double[0]);
		}
		#endregion

		#region BindIncRep_Type
		private void BindIncRep_Type(PieChart pc)
		{
			pc.BorderWidth = 0;
			pc.Radius = 110;

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region AllPrjRep_Status
		private void AllPrjRep_Status(PieChart pc)
		{
			pc.Width = 200;
			pc.Radius = 140;
			pc.BorderWidth = 0;

			DataTable dt = Project.GetProjectStatisticByStatusDataTable();

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region AllPrjRep_PrjCat
		private void AllPrjRep_PrjCat(PieChart pc)
		{
			DataTable dt = Project.GetProjectStatisticByProjectCategoryDataTable();

			pc.Width = 250;
			pc.Radius = 140;
			pc.BorderWidth = 0;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region PrjRep_TaskToDo
		private void PrjRep_TaskToDo(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectReports", typeof(ChartImage).Assembly);

			pc.Title = LocRM.GetString("oTaksToDosGraphTitle");

			pc.Width = 300;
			pc.Radius = 110;
			pc.BorderWidth = 0;

			int itotal = 0;
			int icompleted = 0;
			int iactive = 0;
			int iupcoming = 0;
			int ipastdue = 0;
			int ProjId = int.Parse(Request["ProjectId"].ToString());
			using (IDataReader rdr = Report.GetToDoAndTaskTrackingReport(ProjId))
			{
				/// Total, Completed, PastDue, Active, Upcoming
				if (rdr.Read())
				{
					itotal = (int)rdr["Total"];
					icompleted = (int)rdr["Completed"];
					iactive = (int)rdr["Active"];
					iupcoming = (int)rdr["Upcoming"];
					ipastdue = (int)rdr["PastDue"];
				}
			}

			if (itotal > 0)
			{
				string[] titles = new string[] { LocRM.GetString("oUpcoming"), LocRM.GetString("oActiveTasks"), LocRM.GetString("oCompleted"), LocRM.GetString("oPastDue") };
				double[] values = new double[] { Convert.ToDouble(iupcoming), Convert.ToDouble(iactive - ipastdue), Convert.ToDouble(icompleted), Convert.ToDouble(ipastdue) };
				pc.CollectDataPoints(titles, values);
			}
			else
				pc.CollectDataPoints(new string[0], new double[0]);
		}
		#endregion

		#region PrjRep_Issues
		private void PrjRep_Issues(BarChart barChart)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectReports", typeof(ChartImage).Assembly);

			barChart.Title = LocRM.GetString("oIssuesGraphTitle");
			barChart.Width = 250;
			barChart.Height = 60;
			barChart.Sort = false;
			barChart.BorderWidth = 0;


			int inew = 0;
			int iactive = 0;
			int isuspended = 0;
			int iclosed = 0;
			int ioncheck = 0;
			int itotal = 0;

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			using (IDataReader rdr = Incident.GetIncidentStatistic(ProjId))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll,
				///  OnCheckIncidentCount, ReOpenIncidentCount, SuspendedIncidentCount

				if (rdr.Read())
				{
					inew = (int)rdr["NewIncidentCount"];
					iactive = (int)rdr["ActiveIncidentCount"] + (int)rdr["ReOpenIncidentCount"];
					isuspended = (int)rdr["SuspendedIncidentCount"];
					iclosed = (int)rdr["ClosedIncidentCount"];
					ioncheck = (int)rdr["OnCheckIncidentCount"];
					itotal = inew + iactive + isuspended + iclosed + ioncheck;
				}
			}

			if (itotal > 0)
			{
				string[] titles = new string[] { LocRM.GetString("oNew"), LocRM.GetString("oOpen"), LocRM.GetString("oSuspended"), LocRM.GetString("oClosed"), LocRM.GetString("oOnCheck") };
				double[] values = new double[] { Convert.ToDouble(inew), Convert.ToDouble(iactive), Convert.ToDouble(isuspended), Convert.ToDouble(iclosed), Convert.ToDouble(ioncheck) };
				barChart.CollectDataPoints(titles, values);
			}
			else
				barChart.CollectDataPoints(new string[0], new double[0]);
		}
		#endregion

		#region IncStat_IncStatus
		private void IncStat_IncStatus(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int inew = 0;
			int iactive = 0;
			int iclosed = 0;

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			int ManId = int.Parse(Request["ManagerId"].ToString());
			using (IDataReader rdr = Incident.GetIncidentStatistic(ProjId, ManId))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll

				if (rdr.Read())
				{
					inew = (int)rdr["NewIncidentCount"];
					iactive = (int)rdr["ActiveIncidentCount"];
					iclosed = (int)rdr["ClosedIncidentCount"];
				}
			}
			pc.Title = LocRM.GetString("IncDistribution");
			pc.Width = 200;
			pc.Radius = 140;
			if (Convert.ToDouble(inew) != 0 || Convert.ToDouble(iactive) != 0 || Convert.ToDouble(iclosed) != 0)
			{
				string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
				double[] values = new double[] { Convert.ToDouble(inew), Convert.ToDouble(iactive), Convert.ToDouble(iclosed) };
				pc.CollectDataPoints(titles, values);
			}
			else
				pc.CollectDataPoints(new string[0], new double[0]);
		}
		#endregion

		#region IncStat_IncType
		private void IncStat_IncType(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			int ManId = int.Parse(Request["ManagerId"].ToString());

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable(ProjId, ManId);

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 200;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncStat_GenCat
		private void IncStat_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			int ManId = int.Parse(Request["ManagerId"].ToString());

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable(ProjId, ManId);

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 200;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncStat_IncCat
		private void IncStat_IncCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			int ManId = int.Parse(Request["ManagerId"].ToString());

			DataTable dt = Incident.GetListIncidentStatisticByIncidentCategoryDataTable(ProjId, ManId);

			pc.Title = LocRM.GetString("IncByIncCat");
			pc.Width = 250;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncStat_Man
		private void IncStat_Man(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByIncidentBoxDataTable();

			pc.Title = LocRM.GetString("IncByMan");
			pc.Width = 250;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncStat_Prj
		private void IncStat_Prj(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int ManId = int.Parse(Request["ManagerId"].ToString());
			DataTable dt = Incident.GetListIncidentStatisticByProjectDataTable(ManId);

			pc.Title = LocRM.GetString("IncByProj");
			pc.Width = 250;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (dt.Rows[i]["ItemName"] == DBNull.Value)
					dt.Rows[i]["ItemName"] = LocRM.GetString("WithoutProject");
				string sName = dt.Rows[i]["ItemName"].ToString();
				if (sName.Length < 33)
					titles[i] = sName;
				else
					titles[i] = sName.Substring(0, 30) + "...";
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalys_Status
		private void IncAnalys_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			using (IDataReader rdr = Incident.GetIncidentStatistic(0, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region IncAnalys_Type
		private void IncAnalys_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalys_GenCat
		private void IncAnalys_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysCat_Status
		private void IncAnalysCat_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			using (IDataReader rdr = Incident.GetIncidentStatistic(0, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region IncAnalysCat_Type
		private void IncAnalysCat_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysCat_GenCat
		private void IncAnalysCat_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysMan_Status
		private void IncAnalysMan_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			using (IDataReader rdr = Incident.GetIncidentStatistic(0, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region IncAnalysMan_Type
		private void IncAnalysMan_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysMan_GenCat
		private void IncAnalysMan_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysType_Status
		private void IncAnalysType_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			using (IDataReader rdr = Incident.GetIncidentStatistic(0, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region IncAnalysType_Type
		private void IncAnalysType_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysType_GenCat
		private void IncAnalysType_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncClosed_Status
		private void IncClosed_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			using (IDataReader rdr = Incident.GetIncidentStatistic(0, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region IncClosed_Type
		private void IncClosed_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncClosed_GenCat
		private void IncClosed_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysPrj_Status
		private void IncAnalysPrj_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);
			int ProjId = int.Parse(Request["ProjectId"].ToString());
			using (IDataReader rdr = Incident.GetIncidentStatistic(ProjId, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region IncAnalysPrj_Type
		private void IncAnalysPrj_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable(ProjId, 0);

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region IncAnalysPrj_GenCat
		private void IncAnalysPrj_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable(ProjId, 0);

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region OverHDIss_Status
		private void OverHDIss_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			using (IDataReader rdr = Incident.GetIncidentStatistic(0, 0))
			{
				///  IncidentCount, Pop3IncidentCount, NewIncidentCount, ActiveIncidentCount, 
				///  ClosedIncidentCount, AvgTimeInNewState, AvgTimeInActiveState, 
				///  AvgTimeForResolveClosed, AvgTimeForResolveAll
				rdr.Read();

				pc.Title = LocRM.GetString("IncDistribution");
				pc.Width = 160;
				pc.Radius = 110;
				if (Convert.ToDouble(rdr["NewIncidentCount"]) != 0 || Convert.ToDouble(rdr["ActiveIncidentCount"]) != 0 || Convert.ToDouble(rdr["ClosedIncidentCount"]) != 0)
				{
					string[] titles = new string[] { LocRM.GetString("New"), LocRM.GetString("Active"), LocRM.GetString("Closed") };
					double[] values = new double[] { Convert.ToDouble(rdr["NewIncidentCount"]), Convert.ToDouble(rdr["ActiveIncidentCount"]), Convert.ToDouble(rdr["ClosedIncidentCount"]) };
					pc.CollectDataPoints(titles, values);
				}
				else
					pc.CollectDataPoints(new string[0], new double[0]);
			}
		}
		#endregion

		#region OverHDIss_Type
		private void OverHDIss_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByTypeDataTable();

			pc.Title = LocRM.GetString("IncByType");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region OverHDIss_GenCat
		private void OverHDIss_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strIncidentStatistics", typeof(ChartImage).Assembly);

			DataTable dt = Incident.GetListIncidentStatisticByGeneralCategoryDataTable();

			pc.Title = LocRM.GetString("IncByGenCat");
			pc.Width = 160;
			pc.Radius = 110;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ItemName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region PrjStat_Status
		private void PrjStat_Status(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectStatistics", typeof(ChartImage).Assembly);

			ArrayList alPrjsID = new ArrayList();
			string sID = prop_col["ProjectStatistics_PrjId"].ToString();
			while (sID.Length > 0)
			{
				alPrjsID.Add(int.Parse(sID.Substring(0, sID.IndexOf(","))));
				sID = sID.Remove(0, sID.IndexOf(",") + 1);
			}

			ArrayList alPrjs = Project.GetListProjectsByListId(alPrjsID);
			DataTable dt = Project.GetProjectStatisticByStatusDataTable(alPrjs);

			pc.Title = LocRM.GetString("ProjDistribution");
			pc.Width = 200;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["StatusName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}

			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region PrjStat_Type
		private void PrjStat_Type(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectStatistics", typeof(ChartImage).Assembly);

			ArrayList alPrjsID = new ArrayList();
			string sID = prop_col["ProjectStatistics_PrjId"].ToString();
			while (sID.Length > 0)
			{
				alPrjsID.Add(int.Parse(sID.Substring(0, sID.IndexOf(","))));
				sID = sID.Remove(0, sID.IndexOf(",") + 1);
			}

			ArrayList alPrjs = Project.GetListProjectsByListId(alPrjsID);
			DataTable dt = Project.GetProjectStatisticByTypeDataTable(alPrjs);

			pc.Title = LocRM.GetString("ProjByType");
			pc.Width = 200;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["TypeName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region PrjStat_GenCat
		private void PrjStat_GenCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectStatistics", typeof(ChartImage).Assembly);

			ArrayList alPrjsID = new ArrayList();
			string sID = prop_col["ProjectStatistics_PrjId"].ToString();
			while (sID.Length > 0)
			{
				alPrjsID.Add(int.Parse(sID.Substring(0, sID.IndexOf(","))));
				sID = sID.Remove(0, sID.IndexOf(",") + 1);
			}

			ArrayList alPrjs = Project.GetListProjectsByListId(alPrjsID);
			DataTable dt = Project.GetProjectStatisticByGeneralCategoryDataTable(alPrjs);

			pc.Title = LocRM.GetString("ProjByGenCat");
			pc.Width = 200;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["CategoryName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region PrjStat_PrjCat
		private void PrjStat_PrjCat(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectStatistics", typeof(ChartImage).Assembly);

			ArrayList alPrjsID = new ArrayList();
			string sID = prop_col["ProjectStatistics_PrjId"].ToString();
			while (sID.Length > 0)
			{
				alPrjsID.Add(int.Parse(sID.Substring(0, sID.IndexOf(","))));
				sID = sID.Remove(0, sID.IndexOf(",") + 1);
			}

			ArrayList alPrjs = Project.GetListProjectsByListId(alPrjsID);
			DataTable dt = Project.GetProjectStatisticByProjectCategoryDataTable(alPrjs);

			pc.Title = LocRM.GetString("ProjByProjCat");
			pc.Width = 250;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["CategoryName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region PrjStat_Man
		private void PrjStat_Man(PieChart pc)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectStatistics", typeof(ChartImage).Assembly);

			ArrayList alPrjsID = new ArrayList();
			string sID = prop_col["ProjectStatistics_PrjId"].ToString();
			while (sID.Length > 0)
			{
				alPrjsID.Add(int.Parse(sID.Substring(0, sID.IndexOf(","))));
				sID = sID.Remove(0, sID.IndexOf(",") + 1);
			}

			ArrayList alPrjs = Project.GetListProjectsByListId(alPrjsID);
			DataTable dt = Project.GetProjectStatisticByManagerDataTable(alPrjs);

			pc.Title = LocRM.GetString("ProjByMan");
			pc.Width = 250;
			pc.Radius = 140;

			string[] titles = new string[dt.Rows.Count];
			double[] values = new double[dt.Rows.Count];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				titles[i] = dt.Rows[i]["ManagerName"].ToString();
				values[i] = Convert.ToDouble(dt.Rows[i]["Count"]);
			}
			pc.CollectDataPoints(titles, values);
		}
		#endregion

		#region OverPrjSnap_Issues
		private void OverPrjSnap_Issues(BarChart barChart)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectSnapshot", typeof(ChartImage).Assembly);

			barChart.Title = LocRM.GetString("oIssuesGraphTitle");
			barChart.Width = 320;
			barChart.Height = 100;
			barChart.Sort = false;


			int inew = 0;
			int itotal = 0;
			int iactive = 0;
			int iopen = 0;
			int ietc = 0;

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			using (IDataReader rdr = Incident.GetIncidentStatistic(ProjId))
			{
				if (rdr.Read())
				{
					inew = (int)rdr["NewIncidentCount"];
					iactive = (int)rdr["ActiveIncidentCount"];
					itotal = (int)rdr["IncidentCount"];
					iopen = inew + iactive;
					ietc = itotal - iopen;
				}
			}

			if (itotal > 0)
			{
				string[] titles = new string[] { LocRM.GetString("oNew"), LocRM.GetString("oOpen"), LocRM.GetString("oActive"), LocRM.GetString("oEtc") };
				double[] values = new double[] { Convert.ToDouble(inew), Convert.ToDouble(iopen), Convert.ToDouble(iactive), Convert.ToDouble(ietc) };
				barChart.CollectDataPoints(titles, values);
			}
			else
				barChart.CollectDataPoints(new string[0], new double[0]);
		}
		#endregion

		#region OverPrjSnap_TaskToDo
		private void OverPrjSnap_TaskToDo(BarChart barChart)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectSnapshot", typeof(ChartImage).Assembly);

			barChart.Title = LocRM.GetString("oTaskGraphTitle");
			barChart.Width = 320;
			barChart.Height = 100;
			barChart.Sort = false;

			int itotal = 0;
			int icompleted = 0;
			int iactive = 0;
			int ipastdue = 0;

			int ProjId = int.Parse(Request["ProjectId"].ToString());
			using (IDataReader rdr = Report.GetToDoAndTaskTrackingReport(ProjId))
			{
				if (rdr.Read())
				{
					itotal = (int)rdr["Total"];
					icompleted = (int)rdr["Completed"];
					iactive = (int)rdr["Active"];
					ipastdue = (int)rdr["PastDue"];
				}
			}

			if (itotal > 0)
			{
				string[] titles = new string[] { LocRM.GetString("oTotal"), LocRM.GetString("oCompleted"), LocRM.GetString("oActive"), LocRM.GetString("oPastDue") };
				double[] values = new double[] { Convert.ToDouble(itotal), Convert.ToDouble(icompleted), Convert.ToDouble(iactive), Convert.ToDouble(ipastdue) };
				barChart.CollectDataPoints(titles, values);
			}
			else
				barChart.CollectDataPoints(new string[0], new double[0]);
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
