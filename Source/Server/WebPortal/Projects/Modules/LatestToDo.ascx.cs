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

	/// <summary>
	///		Summary description for LatestToDo.
	/// </summary>
	public partial  class LatestToDo : System.Web.UI.UserControl
	{
		private int ProjID
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					throw new Exception("Invalid Project ID");
				}
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbars();
			if(!Page.IsPostBack)
			{
				ApplyLocalization();
				BindValues();
			}

		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectSummary", typeof(LatestToDo).Assembly);
			dgToDo.Columns[0].HeaderText = LocRM.GetString("title");
			dgToDo.Columns[1].HeaderText = LocRM.GetString("Completed");
			dgToDo.Columns[2].HeaderText = LocRM.GetString("Priority");
			dgToDo.Columns[3].HeaderText = LocRM.GetString("FinishDate");
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectSummary", typeof(LatestToDo).Assembly);
			tbToDo.Title = LocRM.GetString("tbToDo");
			tbToDo.AddLink("<img alt='' src='../Layouts/Images/icon-search.gif'/> " + LocRM.GetString("tbView"),Page.ClientScript.GetPostBackClientHyperlink(lbViewAll,""));
			if (Project.CanUpdate(ProjID))
			tbToDo.AddLink("<img alt='' src='../Layouts/Images/newitem.gif'/> " + LocRM.GetString("tbAdd"),"../ToDo/ToDoEdit.aspx?ProjectId=" + ProjID);
		}
		#endregion

		#region BindValues

		private void BindValues()
		{
			if(ProjID != 0)
			{
				DataTable dt = ToDo.GetListToDoForUserByProject(ProjID);
				DataView dv = dt.DefaultView;
				dgToDo.DataSource = dv;
				dgToDo.DataBind();
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

		protected void lbViewAll_Click(object sender, System.EventArgs e)
		{
			UserLightPropertyCollection pc= Security.CurrentUser.Properties;
			int mask = (int)CalendarView.CalendarFilter.ToDo;
			pc["ListView_Type"] = mask.ToString();
			Response.Redirect(String.Format("../Projects/ProjectView.aspx?ProjectID={0}&Tab=6&ABTab=ListView",ProjID.ToString()));			
		}

		public string FormatDate(Object oDt)
		{
			string retval = "";
			if (oDt != DBNull.Value)
			{
				DateTime dt = (DateTime)oDt;
				retval = dt.ToShortDateString() + " " + dt.ToShortTimeString(); 
			}
			return retval;
		}

		protected string GetToDoStatus(object FinishDate,bool IsCompleted,int ToDoId, string Title)
		{
			if (FinishDate!=DBNull.Value && (DateTime)FinishDate<UserDateTime.UserNow && !IsCompleted)
			{
				return 	@"<a href='../ToDo/ToDoView.aspx?ToDoId="+ ToDoId +"' style='color:red'>"+Title+"</a>";
			}
			else
				return @"<a href='../ToDo/ToDoView.aspx?ToDoId="+ ToDoId +"'>"+Title+"</a>";
		}
	}
}
