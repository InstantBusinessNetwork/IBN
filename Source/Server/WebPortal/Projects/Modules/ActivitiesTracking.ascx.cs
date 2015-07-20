namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ActivitiesTracking.
	/// </summary>
	public partial class ActivitiesTracking : System.Web.UI.UserControl
	{
		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ActivitiesTracking).Assembly);

		private string Tab
		{
			get 
			{
				return Request["Tab"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
			{
				BindDefaultValues();
				BindSavedValues();
			}
		}

    private void Page_PreRender(object sender, EventArgs e)
		{
			BindTabs();
		}

		private void BindTabs()
		{
			if (Tab!=null && (Tab == "PrjGrp" || Tab == "PrjPhase"  || Tab == "PrjMan"))
				pc["ActivitiesTracking_CurrentTab"] = Tab;
			else  if ( pc["ActivitiesTracking_CurrentTab"] == null )
				pc["ActivitiesTracking_CurrentTab"] = "PrjGrp";

			string controlName="";	
			if (pc["ActivitiesTracking_CurrentTab"] == "PrjGrp")
			{
				secHeader.Title = LocRM.GetString("tActTrackPrjByPrjGrp");
				controlName = "~/Projects/Modules/ActivitiesByPrjGroup.ascx";
			}
			else if (pc["ActivitiesTracking_CurrentTab"] == "PrjPhase")
			{
				secHeader.Title = LocRM.GetString("tActTrackPrjByPrjPhase");
				controlName = "~/Projects/Modules/ActivitiesByPhase.ascx";
			}
			else if (pc["ActivitiesTracking_CurrentTab"] == "PrjMan")
			{
				secHeader.Title = LocRM.GetString("tActTrackPrjByPrjMan");
				controlName = "~/Projects/Modules/ActivitiesByManager.ascx";
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}

		private void BindDefaultValues()
		{
			ddPrjGroup.DataSource = ProjectGroup.GetProjectGroups();
			ddPrjGroup.DataTextField = "Title";
			ddPrjGroup.DataValueField = "ProjectGroupId";
			ddPrjGroup.DataBind();
			ddPrjGroup.Items.Insert(0,new ListItem(LocRM.GetString("AllFem"),"0"));

			ddPrjPhase.DataSource = Project.GetListProjectPhases();
			ddPrjPhase.DataTextField = "PhaseName";
			ddPrjPhase.DataValueField = "PhaseId";
			ddPrjPhase.DataBind();
			ddPrjPhase.Items.Insert(0, new ListItem(LocRM.GetString("AllFem"),"0"));

			ddStatus.DataSource = Project.GetListProjectStatus();
			ddStatus.DataTextField = "StatusName";
			ddStatus.DataValueField = "StatusId";
			ddStatus.DataBind();
			ddStatus.Items.Insert(0, new ListItem(LocRM.GetString("All"),"0"));

			btnApplyFilter.Text = LocRM.GetString("Apply");
			btnResetFilter.Text = LocRM.GetString("Reset");
		}

		private void BindSavedValues()
		{
			if(pc["ActivitiesTracking_PrjGrp"]==null)
				pc["ActivitiesTracking_PrjGrp"] = "0";
			else
			{
				ddPrjGroup.ClearSelection();
				CommonHelper.SafeSelect(ddPrjGroup, pc["ActivitiesTracking_PrjGrp"].ToString());
			}
			if(pc["ActivitiesTracking_PrjPhase"]==null)
				pc["ActivitiesTracking_PrjPhase"] = "0";
			else
			{
				ddPrjPhase.ClearSelection();
				CommonHelper.SafeSelect(ddPrjPhase, pc["ActivitiesTracking_PrjPhase"].ToString());
			}
			if(pc["ActivitiesTracking_Status"]==null)
				pc["ActivitiesTracking_Status"] = "0";
			else
			{
				ddStatus.ClearSelection();
				CommonHelper.SafeSelect(ddStatus, pc["ActivitiesTracking_Status"].ToString());
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
			this.btnApplyFilter.Click += new EventHandler(btnApplyFilter_Click);
			this.btnResetFilter.Click += new EventHandler(btnResetFilter_Click);
		}
		#endregion

		private void btnApplyFilter_Click(object sender, EventArgs e)
		{
			pc["ActivitiesTracking_PrjGrp"] = ddPrjGroup.SelectedValue;
			pc["ActivitiesTracking_PrjPhase"] = ddPrjPhase.SelectedValue;
			pc["ActivitiesTracking_Status"] = ddStatus.SelectedValue;
		}

		private void btnResetFilter_Click(object sender, EventArgs e)
		{
			pc["ActivitiesTracking_PrjGrp"] = "0";
			pc["ActivitiesTracking_PrjPhase"] = "0";
			pc["ActivitiesTracking_Status"] = "0";
			BindDefaultValues();
		}
	}
}
