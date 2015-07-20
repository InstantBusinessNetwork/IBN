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
	///		Summary description for ResourceUtilProjects.
	/// </summary>
	public partial class ResourceUtilProjects : System.Web.UI.UserControl
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourceUtilProjects).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindDG();
			if (dgProjects.Items.Count==0)
			{
				lblNoItems.Text = LocRM.GetString("tTheAreNoItems");
				spanLbl.Visible = true;
				dgProjects.Visible = false;
			}
			else
			{
				spanLbl.Visible = false;
				dgProjects.Visible = true;
			}
		}

		private void BindDG()
		{
			dgProjects.Columns[0].HeaderText = LocRM.GetString("Title");
			dgProjects.Columns[2].HeaderText = LocRM.GetString("Priority");
			dgProjects.Columns[3].HeaderText = LocRM.GetString("Status");
			dgProjects.Columns[4].HeaderText = LocRM.GetString("tPrjPhase");
			dgProjects.Columns[1].HeaderText = LocRM.GetString("tRole");
			int GroupId = int.Parse(pc["ResUtil_Group"].ToString());
			
			DataTable dt = Project.GetListProjectsByUsersInGroup(GroupId);
			///		UserId, UserName, ProjectId, Title, PriorityId, PriorityName, 
			///		StatusId, StatusName, PhaseId, PhaseName, IsTeamMember,
			///		IsSponsor, IsStakeHolder, IsManager, IsExecutiveManager, IsGroup
			dgProjects.DataSource = dt.DefaultView;
			dgProjects.DataBind();
		}

		protected string GetTitle(bool IsHeader, int UserId, int ProjectId, int StatusId, string Title)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"<b>" + CommonHelper.GetUserStatus(UserId) + "</b>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:42px'>"+CommonHelper.GetProjectStatus(ProjectId, Title, StatusId)+"</span>";
			}
		}

		protected string GetPriority(bool IsHeader, string PriorityName)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+PriorityName+"</span>";
			}
		}

		protected string GetStatus(bool IsHeader, string StatusName)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+StatusName+"</span>";
			}
		}

		protected string GetPhase(bool IsHeader, string PhaseName)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+PhaseName+"</span>";
			}
		}

		protected string GetRole(bool IsHeader, bool IsTeamMember, bool IsSponsor, bool IsStakeHolder,
								bool IsManager, bool IsExecutiveManager)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				string retval = "";
				if(IsTeamMember)
					retval += "<span class='text' style='padding-left:5px'>"+LocRM.GetString("tTeamMember")+"</span><br>";
				if(IsSponsor)
					retval += "<span class='text' style='padding-left:5px'>"+LocRM.GetString("tSponsor")+"</span><br>";
				if(IsStakeHolder)
					retval += "<span class='text' style='padding-left:5px'>"+LocRM.GetString("tStakeholder")+"</span><br>";
				if(IsManager)
					retval += "<span class='text' style='padding-left:5px'>"+LocRM.GetString("tManager")+"</span><br>";
				if(IsExecutiveManager)
					retval += "<span class='text' style='padding-left:5px'>"+LocRM.GetString("tExeManager")+"</span><br>";
				if(retval.Length>0)
					retval = retval.Substring(0, retval.Length-4);
				return  retval;
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
	}
}
