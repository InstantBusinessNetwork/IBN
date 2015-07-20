namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Resources;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	//using MetaDataPlus.Configurator;

	/// <summary>
	///		Summary description for ProjectGroupView.
	/// </summary>
	public partial class ProjectGroupView : System.Web.UI.UserControl
	{

		#region HTML Vars
		#endregion

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(ProjectGroupView).Assembly);
		//protected Mediachase.UI.Web.Modules.MetaDataViewControl mdView;
		private bool bCanUpdate = ProjectGroup.CanUpdate();

		protected int PGID
		{
			get
			{
				try{
					return int.Parse(Request["ProjectGroupId"].ToString());
				}
				catch{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbars();
		}

		#region BindValues
		private void BindValues()
		{
			using(IDataReader reader = ProjectGroup.GetProjectGroups(PGID))
			{
				if(reader.Read())
				{
					lblTitle.Text = reader["Title"].ToString();
					lblDescr.Text = reader["Description"].ToString();
					lblCreationDate.Text = ((DateTime)reader["CreationDate"]).ToShortDateString();
					lblCreator.Text = CommonHelper.GetUserStatus((int)reader["CreatorId"]);
				}
			}
			dgProjects.Columns[1].HeaderText = LocRM.GetString("title");
			dgProjects.Columns[2].HeaderText = LocRM.GetString("manager");
			
			dgProjects.Columns[3].Visible = bCanUpdate;

			dgProjects.DataSource = Project.GetListProjectsByFilterDataTable("", 0, 0, 0, -1, PrimaryKeyId.Empty, PrimaryKeyId.Empty, 0, DateTime.Now, 0, DateTime.Now, 0, 0, 0, 0, -PGID, 0, false).DefaultView;
			dgProjects.DataBind();

			foreach (DataGridItem dgi in dgProjects.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
				{
					ib.ToolTip = LocRM.GetString("Delete");
					ib.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("tWarning") +"')");
				}
			}
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			/*if(mdView.tblCustomFields.Rows.Count==0)
			{
				bool HasMetaFields = false;
				MetaClass mtPort = MetaClass.Load("PortfolioEx");
				if (mtPort.UserMetaFields.Count > 0)
					HasMetaFields = true;
				mdView.Visible = false;
				if (bCanUpdate && HasMetaFields)
					secHeader.AddLink(
						String.Format("<img alt='' src='../Layouts/Images/Edit.gif'/> {0}", LocRM.GetString("tEditMetaFields")),
						String.Format("../Common/MetaDataEdit.aspx?id={0}&class={1}", PGID.ToString(), "PortfolioEx"));
			}*/
			if (dgProjects.Items.Count==0)
			{
				tcControl.Visible = false;
				if (bCanUpdate)
					secHeader.AddLink(
						String.Format("<img alt='' src='../Layouts/Images/newitem.gif'/> {0}", LocRM.GetString("tAddPrj")),
						String.Format("../Projects/AddRelatedProject.aspx?PrjGroupId={0}", PGID.ToString()));
			}
			else
			{
				secHeaderPrj.Title = LocRM.GetString("tPrjList");
				if (bCanUpdate)
					secHeaderPrj.AddLink(
						String.Format("<img alt='' src='../Layouts/Images/newitem.gif'/> {0}", LocRM.GetString("tAddPrj")),
						String.Format("../Projects/AddRelatedProject.aspx?PrjGroupId={0}", PGID.ToString()));
			}
			secHeader.Title = LocRM.GetString("tPortfolioInfo");
			if (bCanUpdate)
				secHeader.AddLink("<img alt='' src='../Layouts/Images/edit.gif'/> " + LocRM.GetString("tEditPortfolio"),"../Projects/ProjectGroupEdit.aspx?ProjectGroupId="+PGID);
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tBackPortfolio"),"../Projects/ProjectGroups.aspx");
		}
		#endregion

		#region GetBool
		protected bool GetBool(int val)
		{
			if (val == 1) 
				return true;
			else
				return false;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgProjects.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
		}
		#endregion

		#region DataGrid Events
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int RelProjectId = int.Parse(e.Item.Cells[0].Text);
			ProjectGroup.RemoveProjectFromProjectGroup(RelProjectId, PGID);
			Response.Redirect("../Projects/ProjectGroupView.aspx?ProjectGroupId="+PGID);
		}
		#endregion
	}
}
