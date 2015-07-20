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

	/// <summary>
	///		Summary description for ProjectGroupEdit.
	/// </summary>
	public partial class ProjectGroupEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(ProjectGroupEdit).Assembly);
		private int PGID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Request["ProjectGroupId"] != null)
				PGID = int.Parse(Request["ProjectGroupId"]);
			if (PGID != 0)
				BindValues();
			BindToolBar();
		}

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = ProjectGroup.GetProjectGroups(PGID))
			{
				if (reader.Read())
				{
					txtTitle.Text = reader["Title"].ToString();
					txtDescr.Text = reader["Description"].ToString();
				}
			}
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			btnCancel.Text = LocRM.GetString("tbsave_cancel");

			if (PGID != 0)
				secHeader.Title = LocRM.GetString("tEditPortfolio");
			else
				secHeader.Title = LocRM.GetString("tNewPortfolio");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tBackPortfolio"), "../Projects/ProjectGroups.aspx");
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
		}
		#endregion

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			int PGID1 = PGID;
			if (PGID != 0)
				ProjectGroup.Update(PGID, txtTitle.Text, txtDescr.Text);
			else
				PGID1 = ProjectGroup.Create(txtTitle.Text, txtDescr.Text);
			Response.Redirect("../Projects/ProjectGroupView.aspx?ProjectGroupId=" + PGID1);
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (PGID > 0)
				Response.Redirect("../Projects/ProjectGroupView.aspx?ProjectGroupId=" + PGID);
			else
				Response.Redirect("../Projects/ProjectGroups.aspx");
		}
	}
}
