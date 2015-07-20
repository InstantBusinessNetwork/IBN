namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for QTResourcePercent.
	/// </summary>
	public partial  class QTracker : System.Web.UI.UserControl
	{

		protected System.Web.UI.HtmlControls.HtmlButton btnUpdate;
//		protected System.Web.UI.WebControls.TextBox tbTTPS;
//		protected System.Web.UI.WebControls.TextBox tbTTOS;
//		protected System.Web.UI.WebControls.TextBox tbTimesheetHours;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strQuickTracking", typeof(QTracker).Assembly);


		#region TaskID
		private int TaskID
		{
			get 
			{
				try
				{
					return int.Parse(Request["TaskID"]);
				}
				catch
				{
					throw new AccessDeniedException();
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get 
			{
				try
				{
					return int.Parse(Request["SharedId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBar();
			if (!IsPostBack)
			{
				BindDD();
			}

			DataBind();
		}

		#region BindVisibility
		private void BindVisibility()
		{
			try
			{
				Task.Tracking trk = Task.GetTrackingInfo(TaskID);
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

				if (!(trk.ShowPersonalStatus || trk.ShowPersonalStatusOnly || trk.ShowOverallStatus || trk.ShowOverallStatusOnly || trk.ShowActivate || trk.ShowComplete || trk.ShowSuspend || trk.ShowUncomplete || trk.ShowResume || trk.ShowAcceptDeny || trTimeTracker.Visible))
					this.Visible = false;
			}
			catch	(NotExistingIdException)
			{
				Response.Redirect("../Common/NotExistingId.aspx?TaskId=" + TaskID);
			}
		}
		#endregion

		#region BindDD
		private void BindDD()
		{
			ddPercentOS.Items.Clear();
			ddPercentOSOnly.Items.Clear();
			ddPercentPS.Items.Clear();
			ddPercentPSOnly.Items.Clear();

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
			using(IDataReader rdr = Task.GetResourceInfo(TaskID))
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

			string Phase = "";
			bool IsMileStone = false;
			using(IDataReader rdr = Task.GetTask(TaskID))
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

					IsMileStone = (bool)rdr["IsMileStone"];
					if (IsMileStone && rdr["PhaseName"] != DBNull.Value)
						Phase = rdr["PhaseName"].ToString();
				}
			}


			if (!IsMileStone)
			{
				lblComplete.Text = LocRM.GetString("Text3");
			}
			else
			{
				lblComplete.Text = LocRM.GetString("MileStone");
				btnCompleteTD.Text = LocRM.GetString("CompleteMilestone");
				btnCompleteTD.Style.Add(System.Web.UI.HtmlTextWriterStyle.Width, "150px");
			}

			if (Phase != "")
				lblPhase.Text = String.Format("{0} <b>{1}</b>", LocRM.GetString("ProjectPhase"), Phase);

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
			tbView.AddText(LocRM.GetString("QuickTracking"));
			btnUpdateOS.Attributes.Add("onclick","DisableButtons(this);");
			btnActivateTD.Attributes.Add("onclick","DisableButtons(this);");
			btnCompleteTD.Attributes.Add("onclick","DisableButtons(this);");
			btnSuspendTD.Attributes.Add("onclick","DisableButtons(this);");
			btnUncompleteTD.Attributes.Add("onclick","DisableButtons(this);");
			btnResumeTask.Attributes.Add("onclick","DisableButtons(this);");
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

				if (Minutes > 0 && !TimeTracking.CanUpdate(dtcPS.SelectedDate, Task.GetProject(TaskID)))
				{
					CustomValidator1.ErrorMessage = LocRM.GetString("tWrongDate");
					CustomValidator1.IsValid = false;
					return;
				}

				Task.UpdateResourcePercent(TaskID, percent, Minutes, dtcPS.SelectedDate);
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

				Task.UpdateResourcePercent(TaskID, percent);
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

				if (Minutes > 0 && !TimeTracking.CanUpdate(dtcOS.SelectedDate, Task.GetProject(TaskID)))
				{
					CustomValidator2.ErrorMessage = LocRM.GetString("tWrongDate");
					CustomValidator2.IsValid = false;
					return;
				}

				Task.UpdatePercent(TaskID, percent, Minutes, dtcOS.SelectedDate);
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

				Task.UpdatePercent(TaskID, percent);
			}
			CheckCompleted();
			Response.Redirect(GetLink());
		}
		#endregion

		#region CheckCompleted
		private void CheckCompleted()
		{
			if (!Task.CanRead(TaskID) && Security.CurrentUser.IsExternal)
				Response.Redirect("~/External/MissingObject.aspx");
		}
		#endregion

		#region btnActivateTD_ServerClick
		protected void btnActivateTD_ServerClick(object sender, System.EventArgs e)
		{
			Task.ActivateTask(TaskID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnCompleteTD_ServerClick
		protected void btnCompleteTD_ServerClick(object sender, System.EventArgs e)
		{
			Task.CompleteTask(TaskID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnSuspendTD_ServerClick
		protected void btnSuspendTD_ServerClick(object sender, System.EventArgs e)
		{
			Task.SuspendTask(TaskID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnUncompleteTD_ServerClick
		protected void btnUncompleteTD_ServerClick(object sender, System.EventArgs e)
		{
			Task.UncompleteTask(TaskID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnResumeTask_ServerClick
		protected void btnResumeTask_ServerClick(object sender, System.EventArgs e)
		{
			Task.ResumeTask(TaskID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnAccept_ServerClick
		protected void btnAccept_ServerClick(object sender, System.EventArgs e)
		{
			Task.AcceptResource(TaskID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnDecline_ServerClick
		protected void btnDecline_ServerClick(object sender, System.EventArgs e)
		{
			Task.DeclineResource(TaskID);
      if (Security.CurrentUser.IsExternal)
        Response.Redirect("~/External/MissingObject.aspx");
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Workspace/default.aspx?BTab=Workspace", Response);
		}
		#endregion

		#region btnUpdateTimeTracking_ServerClick
		protected void btnUpdateTimeTracking_ServerClick(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			//string Hours = tbTimesheetHours.Text;
			string Hours = String.Format("{0:H:mm}",dtcTimesheetHours.Value);
			if (Hours == "")
				Hours = "0:00";
			string[] parts = Hours.Split(':');
			int Minutes = 0;
			Minutes = int.Parse(parts[0]) * 60;
			if (parts.Length > 1)
				Minutes += int.Parse(parts[1]);

			if (Minutes > 0 && !TimeTracking.CanUpdate(dtc.SelectedDate, Task.GetProject(TaskID)))
			{
				CustomValidator3.ErrorMessage = LocRM.GetString("tWrongDate");
				CustomValidator3.IsValid = false;
				return;
			}

			Task.AddTimeSheet(TaskID, Minutes, dtc.SelectedDate);
			bindHours();

			Response.Redirect(GetLink());
		}
		#endregion

		#region GetLink
		private string GetLink()
		{
			String slink = String.Empty;
			if (SharedID>0)
				slink="&SharedId=" + SharedID;

      string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalTask.aspx" : "../Tasks/TaskView.aspx";
			return String.Format("{2}?TaskID={0}{1}", TaskID, slink, sPath);
		}
		#endregion
	}
}
