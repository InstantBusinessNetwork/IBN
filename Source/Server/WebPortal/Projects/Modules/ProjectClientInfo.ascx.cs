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
	///		Summary description for ProjectClientInfo.
	/// </summary>
	public partial class ProjectClientInfo : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectClientInfo).Assembly);

		#region ProjectId
		private int ProjectId
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
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
			if (!PortalConfig.CommonProjectAllowViewClientField)
			{
				this.Visible = false;
				return;
			}

			if(!Page.IsPostBack)
			{
				BindValues();
			}

			BindToolbar();
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

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if(reader.Read())
					lblClient.Text = Util.CommonHelper.GetClientLink(this.Page, reader["OrgUid"], reader["ContactUid"], reader["ClientName"]);
			}
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("client_info"));
		}
		#endregion
	}
}
