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
	///		Summary description for ResourcesByRoles.
	/// </summary>
	public partial class ResourcesByRoles : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourcesByRoles).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindDG();
			if (dgUsers.Items.Count==0)
			{
				lblNoItems.Text = LocRM.GetString("tTheAreNoItems");
				spanLbl.Visible = true;
				dgUsers.Visible = false;
			}
			else
			{
				spanLbl.Visible = false;
				dgUsers.Visible = true;
			}
		}

		private void BindDG()
		{
			dgUsers.Columns[0].HeaderText = LocRM.GetString("tUserName");
			dgUsers.Columns[1].HeaderText = LocRM.GetString("tOpenTasks");
			dgUsers.Columns[2].HeaderText = LocRM.GetString("tCompletedTasks");
			dgUsers.Columns[3].HeaderText = LocRM.GetString("tIssues");
			
			DataTable dt = User.GetListUsersGroupedByRole();
			///  RoleId, RoleName, UserId, UserName, OpenTasks, CompletedTasks, Issues, IsHeader
			dgUsers.DataSource = dt.DefaultView;
			dgUsers.DataBind();
			if(!Configuration.HelpDeskEnabled)
				dgUsers.Columns[3].Visible = false;
		}

		protected string GetTitle(bool IsHeader, int RoleId, int UserId)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"<b>" + CommonHelper.GetGroupLink(RoleId) + "</b>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:15px'>"+CommonHelper.GetUserStatus(UserId)+"</span>";
			}
		}

		protected string GetOpenTasks(bool IsHeader, int OpenTasksCount)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+OpenTasksCount+"</span>";
			}
		}

		protected string GetCompletedTasks(bool IsHeader, int TCount)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+TCount+"</span>";
			}
		}

		protected string GetIssues(bool IsHeader, int IssCount)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='boldtext'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+IssCount+"</span>";
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
