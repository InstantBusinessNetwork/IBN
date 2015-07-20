namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for ResourceWorks.
	/// </summary>
	public partial class ResourceWorks : System.Web.UI.UserControl, IPageViewMenu
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourceWorks).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Request["Shared"] != null)
				secHeader.Title = LocRM.GetString("tSharedActivities");
			else
				secHeader.Title = LocRM.GetString("tMyActivities");
		}

		#region IPageViewMenu Members

		public PageViewMenu GetToolBar()
		{
			return secHeader;
		}

		#endregion
	}
}
