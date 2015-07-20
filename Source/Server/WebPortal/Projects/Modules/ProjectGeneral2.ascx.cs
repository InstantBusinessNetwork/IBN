namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ProjectGeneral2.
	/// </summary>
	public partial class ProjectGeneral2 : System.Web.UI.UserControl
	{
		#region ProjectId
		private int ProjectId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "ProjectID", -1);
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectGeneral2).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
			{
				ibnTimeTracking.Visible = Project.CanViewFinances(ProjectId);

				if (Request["TemplateId"]!=null && this.Visible)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						@"<script language=javascript>
					OpenAssignWizard(" + ProjectId + "," + int.Parse(Request["TemplateId"].ToString()) + @");
					function OpenAssignWizard(ProjectId, TemplateId)
					{
						var w = 550;
						var h = 300;
						var l = (screen.width - w) / 2;
						var t = (screen.height - h) / 2;
						winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
						var f = window.open('GenerateProjectByTemplate.aspx?ProjectId=' + ProjectId+'&TemplateId='+TemplateId, 'GenerateProject', winprops);
						if (f == null)
						{

							tW = document.getElementById('tblWarning');
							aW = document.getElementById('aAssignLink');
							if (tW != null && aW !=null)
							{
								tW.style.display = 'block';
								aW.href = 'javascript:OpenAssignWizard(" + ProjectId + "," + int.Parse(Request["TemplateId"].ToString()) + @");';
								aW.innerHTML = '" + LocRM.GetString("AssignWizard")+@"';
							}
						}
					}
				</script>");

				ucProjectclientinfo.Visible = PortalConfig.GeneralAllowClientField;
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
