namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;
	using Mediachase.Ibn;

	/// <summary>
	///		Summary description for QuickTracking.
	/// </summary>
	public partial class QuickTracking : System.Web.UI.UserControl
	{
		#region Html Vars
		#endregion

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(QuickTracking).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strQuickTracking", typeof(QuickTracking).Assembly);

		#region DocumentID
		private int DocumentID
		{
			get 
			{
				try
				{
					return int.Parse(Request["DocumentID"]);
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
				int retval = -1;
				if (Request["SharedId"] != null)
					retval = int.Parse(Request["SharedId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBarAndButtons();
			if (!IsPostBack)
			{
				BindDD();
			}

			DataBind();
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
			this.btnAccept.ServerClick +=new EventHandler(btnAccept_ServerClick);
			this.btnDecline.ServerClick +=new EventHandler(btnDecline_ServerClick);
			this.ibUpdate.ServerClick +=new EventHandler(ibUpdate_ServerClick);
			this.btnComplete.ServerClick +=new EventHandler(btnComplete_ServerClick);
			this.btnSuspend.ServerClick +=new EventHandler(btnSuspend_ServerClick);
			this.btnUncomplete.ServerClick +=new EventHandler(btnUncomplete_ServerClick);
			this.btnResume.ServerClick +=new EventHandler(btnResume_ServerClick);
			this.btnActivate.ServerClick +=new EventHandler(btnActivate_ServerClick);
			this.btnUpdateStatus.ServerClick +=new EventHandler(btnUpdateStatus_ServerClick);
			this.btnUpdateTTStatus.ServerClick +=new EventHandler(btnUpdateTTStatus_ServerClick);
		}
		#endregion

		#region BindToolBarAndButtons
		private void BindToolBarAndButtons()
		{
			tbView.AddText(LocRM.GetString("QuickTracking"));

			// O.R. [2009-05-25]: DisableButtons function doesn't work with CausesValidation property
			/*
			btnComplete.Attributes.Add("onclick","DisableButtons(this);");
			btnSuspend.Attributes.Add("onclick","DisableButtons(this);");
			btnUncomplete.Attributes.Add("onclick","DisableButtons(this);");
			btnResume.Attributes.Add("onclick","DisableButtons(this);");
			btnDecline.Attributes.Add("onclick","DisableButtons(this);");
			btnAccept.Attributes.Add("onclick","DisableButtons(this);");
			btnActivate.Attributes.Add("onclick","DisableButtons(this);");
			 */ 

			string ContainerKey = "DocumentVers_" + DocumentID.ToString();
			string ContainerName = "FileLibrary";
			BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
      string commandLink = (Security.CurrentUser.IsExternal) ? "~/External/FileUpload.aspx" : "~/FileStorage/FileUpload.aspx";
      string link = String.Format("javascript:ShowWizard('{0}?ParentFolderId={1}&ContainerKey={2}&ContainerName={3}{4}', 470, 270);return false;",
        ResolveUrl(commandLink), fs.Root.Id, ContainerKey, ContainerName,
        (Security.CurrentUser.IsExternal) ? ("&ExternalId=" + Security.CurrentUser.UserID) : "");
      btnNewVersion.Attributes.Add("onclick", link);
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			Document.Tracking trk = Document.GetTrackingInfo(DocumentID);
			trAT.Visible = trk.ShowActivate;
			trCD.Visible = trk.ShowComplete;
			trSD.Visible = trk.ShowSuspend;
			trUD.Visible = trk.ShowUncomplete;
			trRD.Visible = trk.ShowResume;
			trAD.Visible = trk.ShowAcceptDeny;
			trTimeTracker.Visible = trk.ShowTimeTracking;
			trStatus.Visible = trk.ShowStatus;
			trTTStatus.Visible = trk.ShowStatusAndTimeTracking;
      trNewVersion.Visible = trk.ShowAddNewVersion;

			if (!(trk.ShowActivate || trk.ShowComplete || trk.ShowSuspend || trk.ShowUncomplete || trk.ShowResume || trk.ShowAcceptDeny || trk.ShowTimeTracking || trk.ShowStatus || trk.ShowStatusAndTimeTracking || trk.ShowAddNewVersion))
				this.Visible = false;
		}
		#endregion

		#region BindDD
		private void BindDD()
		{
			dtc.SelectedDate = UserDateTime.UserToday;
			dtc2.SelectedDate = UserDateTime.UserToday;

			bindHours();

			using (IDataReader reader = Document.GetListDocumentStatus())
			{
				while (reader.Read())
				{
					string StatusName = reader["StatusName"].ToString();
					string StatusId = reader["StatusId"].ToString();

					ddStatus.Items.Add(new ListItem(StatusName, StatusId));
					ddStatus2.Items.Add(new ListItem(StatusName, StatusId));
				}
			}
		}
		#endregion

		#region bindHours
		private void bindHours()
		{
			//tbTimesheetHours.Text = "0:00";
			dtcTimesheetHours.Value = DateTime.MinValue;
			dtcTimesheetHours2.Value = DateTime.MinValue;
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
			ddStatus.ClearSelection();
			ddStatus2.ClearSelection();
			
			using(IDataReader rdr = Document.GetDocument(DocumentID))
			{
				if (rdr.Read())
				{
					string StatusId = rdr["StatusId"].ToString();
					ListItem li = ddStatus.Items.FindByValue(StatusId);
					if (li!=null) 
						li.Selected = true;
					li = ddStatus2.Items.FindByValue(StatusId);
					if (li!=null) 
						li.Selected = true;
				}
			}
		}
		#endregion

		#region ibUpdate_ServerClick
		private void ibUpdate_ServerClick(object sender, EventArgs e)
		{
			if (!Page.IsValid)
				return;

			string Hours = String.Format("{0:H:mm}",dtcTimesheetHours.Value);
			if (Hours == "")
				Hours = "0:00";
			string[] parts = Hours.Split(':');
			int Minutes = 0;
			Minutes = int.Parse(parts[0]) * 60;
			if (parts.Length > 1)
				Minutes += int.Parse(parts[1]);

			if( Minutes>0 && !TimeTracking.CanUpdate(dtc.SelectedDate, Document.GetProject(DocumentID)) )
			{
				cvHours.ErrorMessage = LocRM2.GetString("tWrongDate");
				cvHours.IsValid = false;
				return;
			}

			Document.AddTimeSheet(DocumentID, Minutes, dtc.SelectedDate);
			bindHours();

			Response.Redirect(GetLink());
		}
		#endregion

		#region btnAccept_ServerClick
		private void btnAccept_ServerClick(object sender, EventArgs e)
		{
			Document2.AcceptResource(DocumentID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnDecline_ServerClick
		private void btnDecline_ServerClick(object sender, EventArgs e)
		{
			Document2.DeclineResource(DocumentID);
      CheckCompleted();
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnComplete_ServerClick
		private void btnComplete_ServerClick(object sender, EventArgs e)
		{
			Document.CompleteDocument(DocumentID);
      CheckCompleted();
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnSuspend_ServerClick
		private void btnSuspend_ServerClick(object sender, EventArgs e)
		{
			Document.SuspendDocument(DocumentID);
      CheckCompleted();
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnUncomplete_ServerClick
		private void btnUncomplete_ServerClick(object sender, EventArgs e)
		{
			Document.UncompleteDocument(DocumentID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnResume_ServerClick
		private void btnResume_ServerClick(object sender, EventArgs e)
		{
			Document.ResumeDocument(DocumentID);
      Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnActivate_ServerClick
		private void btnActivate_ServerClick(object sender, EventArgs e)
		{
			Document.ActivateDocument(DocumentID);
			Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", GetLink(), Response);
		}
		#endregion

		#region btnUpdateStatus_ServerClick
		private void btnUpdateStatus_ServerClick(object sender, EventArgs e)
		{
			int StatusId = int.Parse(ddStatus.SelectedValue);
			Document2.UpdateStatus(DocumentID, StatusId);
			Response.Redirect(GetLink());
		}
		#endregion

		#region btnUpdateTTStatus_ServerClick
		private void btnUpdateTTStatus_ServerClick(object sender, EventArgs e)
		{
			if (!Page.IsValid)
				return;

			string Hours = String.Format("{0:H:mm}",dtcTimesheetHours2.Value);
			if (Hours == "")
				Hours = "0:00";
			string[] parts = Hours.Split(':');
			int Minutes = 0;
			Minutes = int.Parse(parts[0]) * 60;
			if (parts.Length > 1)
				Minutes += int.Parse(parts[1]);

			if (Minutes > 0 && !TimeTracking.CanUpdate(dtc2.SelectedDate, Document.GetProject(DocumentID)))
			{
				cvHours2.ErrorMessage = LocRM2.GetString("tWrongDate");
				cvHours2.IsValid = false;
				return;
			}
			Document.AddTimeSheet(DocumentID, Minutes, dtc2.SelectedDate);

			int StatusId = int.Parse(ddStatus2.SelectedValue);
			Document2.UpdateStatus(DocumentID, StatusId);

			Response.Redirect(GetLink());
		}
		#endregion

    #region CheckCompleted
    private void CheckCompleted()
    {
      if (!Document.CanRead(DocumentID) && Security.CurrentUser.IsExternal)
        Response.Redirect("~/External/MissingObject.aspx");
    }
    #endregion

		#region GetLink
		private string GetLink()
		{
			String slink = String.Empty;
			if (SharedID>0)
				slink="&SharedId=" + SharedID;

      string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalDocument.aspx" : "../Documents/DocumentView.aspx";
      return String.Format("{2}?DocumentID={0}{1}", DocumentID, slink, sPath);
		}
		#endregion
	}
}
