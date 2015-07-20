namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;

	/// <summary> 
	///		Summary description for QTResourcePercent.
	/// </summary>
	public partial  class QTracker : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM;

		#region ToDoID
		private int ToDoID
		{
			get 
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strQuickTracking", typeof(QTracker).Assembly);
			BindToolBar();
			if (!IsPostBack)
			{
				BindDD();

			}

			DataBind();
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			Mediachase.IBN.Business.ToDo.Tracking trk = Mediachase.IBN.Business.ToDo.GetTrackingInfo(ToDoID);
			trPS.Visible = trk.ShowPersonalStatus;
			trPSOnly.Visible = trk.ShowPersonalStatusOnly;
			trOS.Visible = trk.ShowOverallStatus;
			trOSOnly.Visible = trk.ShowOverallStatusOnly;
			trAT.Visible = trk.ShowActivate;
			trCT.Visible = trk.ShowComplete;
			trST.Visible = trk.ShowSuspend;
			trUT.Visible = trk.ShowUncomplete;
			trRT.Visible = trk.ShowResume;
			trAD.Visible = trk.ShowAcceptDeny;
			trTimeTracker.Visible = trk.ShowTimeTracking;

			if (!(trk.ShowPersonalStatus || trk.ShowPersonalStatusOnly || trk.ShowOverallStatus || trk.ShowOverallStatusOnly || trk.ShowComplete || trk.ShowSuspend || trk.ShowUncomplete || trk.ShowResume || trk.ShowAcceptDeny || trTimeTracker.Visible))
				this.Visible = false;
		}
		#endregion

		#region BindDD
		private void BindDD()
		{
			for (int i=0;i<=100;i+=1)
			{
				ddPercentPS.Items.Add(new ListItem(i.ToString()+" %",i.ToString()));
				ddPercentPSOnly.Items.Add(new ListItem(i.ToString() + " %", i.ToString()));
				ddPercentOS.Items.Add(new ListItem(i.ToString()+" %",i.ToString()));
				ddPercentOSOnly.Items.Add(new ListItem(i.ToString() + " %", i.ToString()));
			}

			dtc.SelectedDate = UserDateTime.UserToday;
			dtcPS.SelectedDate = UserDateTime.UserToday;
			dtcOS.SelectedDate = UserDateTime.UserToday;

			bindHours();
		}
		#endregion

		#region bindHours
		private void bindHours()
		{
			dtcTimesheetHours.Value = DateTime.MinValue;
			dtcTimesheetHoursPS.Value = DateTime.MinValue;
			dtcTimesheetHoursOS.Value = DateTime.MinValue;
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindSavedValues();
			BindVisibility();
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			ddPercentPS.ClearSelection();
			ddPercentPSOnly.ClearSelection();
			ddPercentOS.ClearSelection();
			ddPercentOSOnly.ClearSelection();

			ListItem li;
			using(IDataReader rdr = ToDo.GetResourceInfo(ToDoID))
			{
				if (rdr.Read())
				{
					int PercentComleted = (int)rdr["PercentCompleted"];

					li = ddPercentPS.Items.FindByValue(PercentComleted.ToString());
					if (li!=null) 
						li.Selected = true;

					li = ddPercentPSOnly.Items.FindByValue(PercentComleted.ToString());
					if (li != null)
						li.Selected = true;			
				}
			}

			using(IDataReader rdr = ToDo.GetToDo(ToDoID))
			{
				if (rdr.Read())
				{
					int PercentComleted = (int)rdr["PercentCompleted"];

					li = ddPercentOS.Items.FindByValue(PercentComleted.ToString());
					if (li!=null) 
						li.Selected = true;

					li = ddPercentOSOnly.Items.FindByValue(PercentComleted.ToString());
					if (li != null)
						li.Selected = true;
				}
			}
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

		#region BindToolBar
		private void BindToolBar()
		{
			//tbView.Title = LocRM.GetString("QuickTracking");
			tbView.AddText(LocRM.GetString("QuickTracking"));
			btnUpdatePS.Attributes.Add("onclick","DisableButtons(this);");
			btnUpdatePSOnly.Attributes.Add("onclick", "DisableButtons(this);");
			btnUpdateOS.Attributes.Add("onclick","DisableButtons(this);");
			btnUpdateOSOnly.Attributes.Add("onclick", "DisableButtons(this);");
			btnActivateTD.Attributes.Add("onclick","DisableButtons(this);");
			btnCompleteTD.Attributes.Add("onclick","DisableButtons(this);");
			btnSuspendTD.Attributes.Add("onclick","DisableButtons(this);");
			btnUncompleteTD.Attributes.Add("onclick","DisableButtons(this);");
			btnResumeToDo.Attributes.Add("onclick","DisableButtons(this);");
			btnDecline.Attributes.Add("onclick","DisableButtons(this);");
			btnAccept.Attributes.Add("onclick","DisableButtons(this);");
		}
		#endregion

		#region btnUpdatePS_ServerClick
		protected void btnUpdatePS_ServerClick(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			ListItem li = ddPercentPS.SelectedItem;
			if (li!=null) 
			{
				int percent = int.Parse(li.Value);
				
				//string Hours = tbTTPS.Text;
				string Hours = String.Format("{0:H:mm}",dtcTimesheetHoursPS.Value);
				if (Hours == "")
					Hours = "0:00";
				string[] parts = Hours.Split(':');
				int Minutes = 0;
				Minutes = int.Parse(parts[0]) * 60;
				if (parts.Length > 1)
					Minutes += int.Parse(parts[1]);

				if( Minutes>0 )
				{
					using(IDataReader rdr = ToDo.GetToDo(ToDoID))
					{
						if (rdr.Read())
						{
							if (rdr["ProjectId"] != DBNull.Value)
							{
								int DRProjectId = (int)rdr["ProjectId"];
								if( !TimeTracking.CanUpdate(dtcPS.SelectedDate,DRProjectId) )
								{
									CustomValidator1.ErrorMessage = LocRM.GetString("tWrongDate");
									CustomValidator1.IsValid = false;
									return;
								}
							}
						}
					}
				}

				ToDo.UpdateResourcePercent(ToDoID, percent, Minutes, dtcPS.SelectedDate);
			}
			CheckCompleted();
			//bindHours();
			Response.Redirect(GetLink());
		}
		#endregion

		#region btnUpdatePSOnly_ServerClick
		protected void btnUpdatePSOnly_ServerClick(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			ListItem li = ddPercentPSOnly.SelectedItem;
			if (li != null)
			{
				int percent = int.Parse(li.Value);

				ToDo.UpdateResourcePercent(ToDoID, percent);
			}
			CheckCompleted();
			Response.Redirect(GetLink());
		}
		#endregion

		#region btnUpdateOS_ServerClick
		protected void btnUpdateOS_ServerClick(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			ListItem li = ddPercentOS.SelectedItem;
			if (li!=null)
			{
				int percent = int.Parse(li.Value);

				//string Hours = tbTTOS.Text;
				string Hours = String.Format("{0:H:mm}",dtcTimesheetHoursOS.Value);
				if (Hours == "")
					Hours = "0:00";
				string[] parts = Hours.Split(':');
				int Minutes = 0;
				Minutes = int.Parse(parts[0]) * 60;
				if (parts.Length > 1)
					Minutes += int.Parse(parts[1]);

				if( Minutes>0 )
				{
					using(IDataReader rdr = ToDo.GetToDo(ToDoID))
					{
						if (rdr.Read())
						{
							if (rdr["ProjectId"] != DBNull.Value)
							{
								int DRProjectId = (int)rdr["ProjectId"];
								if (!TimeTracking.CanUpdate(dtcOS.SelectedDate, DRProjectId))
								{
									CustomValidator2.ErrorMessage = LocRM.GetString("tWrongDate");
									CustomValidator2.IsValid = false;
									return;
								}
							}
						}
					}
				}

				ToDo.UpdatePercent(ToDoID, percent, Minutes, dtcOS.SelectedDate);
			}
			CheckCompleted();
			//bindHours();
      Response.Redirect(GetLink());
		}
		#endregion

		#region btnUpdateOSOnly_ServerClick
		protected void btnUpdateOSOnly_ServerClick(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			ListItem li = ddPercentOSOnly.SelectedItem;
			if (li != null)
			{
				int percent = int.Parse(li.Value);

				ToDo.UpdatePercent(ToDoID, percent);
			}
			CheckCompleted();
			Response.Redirect(GetLink());
		}
		#endregion

		#region btnUpdateTT_Click
		protected void btnUpdateTT_Click(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			//string Hours = tbTimesheetHours.Text;
			string Hours = String.Format("{0:H:mm}", dtcTimesheetHours.Value);
			if (Hours == "")
				Hours = "0:00";
			string[] parts = Hours.Split(':');
			int Minutes = 0;
			Minutes = int.Parse(parts[0]) * 60;
			if (parts.Length > 1)
				Minutes += int.Parse(parts[1]);

			if( Minutes>0 )
			{
				using(IDataReader rdr = ToDo.GetToDo(ToDoID))
				{
					if (rdr.Read())
					{
						if (rdr["ProjectId"] != DBNull.Value)
						{
							int DRProjectId = (int)rdr["ProjectId"];
							if (!TimeTracking.CanUpdate(dtc.SelectedDate, DRProjectId))
							{
								CustomValidator3.ErrorMessage = LocRM.GetString("tWrongDate");
								CustomValidator3.IsValid = false;
								return;
							}
						}
					}
				}
			}

			ToDo.AddTimeSheet(ToDoID,Minutes,dtc.SelectedDate);
			//bindHours();
			Response.Redirect(GetLink());
		}
		#endregion

		#region CheckCompleted
		private void CheckCompleted()
		{
			if (!ToDo.CanRead(ToDoID) && Security.CurrentUser.IsExternal)
				Response.Redirect("~/External/MissingObject.aspx");
		}
		#endregion

		#region btnActivateTD_ServerClick
		protected void btnActivateTD_ServerClick(object sender, System.EventArgs e)
		{
			Mediachase.IBN.Business.ToDo.ActivateTodo(ToDoID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnCompleteTD_ServerClick
		protected void btnCompleteTD_ServerClick(object sender, System.EventArgs e)
		{
			ToDo.CompleteToDo(ToDoID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnSuspendTD_ServerClick
		protected void btnSuspendTD_ServerClick(object sender, System.EventArgs e)
		{
			ToDo.SuspendToDo(ToDoID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnUncompleteTD_ServerClick
		protected void btnUncompleteTD_ServerClick(object sender, System.EventArgs e)
		{
			ToDo.UncompleteToDo(ToDoID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnResumeToDo_ServerClick
		protected void btnResumeToDo_ServerClick(object sender, System.EventArgs e)
		{
			ToDo.ResumeToDo(ToDoID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnAccept_ServerClick
		protected void btnAccept_ServerClick(object sender, System.EventArgs e)
		{
			ToDo2.AcceptResource(ToDoID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnDecline_ServerClick
		protected void btnDecline_ServerClick(object sender, System.EventArgs e)
		{
			ToDo2.DeclineResource(ToDoID);
			CheckCompleted();
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Workspace/default.aspx?BTab=Workspace", Response);
		}
		#endregion

		#region GetLink
		private string GetLink()
		{
			string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalToDo.aspx" : "../ToDo/ToDoView.aspx";
			return String.Format("{1}?ToDoID={0}", ToDoID, sPath);
		}
		#endregion

	}
}
