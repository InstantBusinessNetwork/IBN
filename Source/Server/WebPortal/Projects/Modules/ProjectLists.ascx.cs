namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for ProjectLists.
	/// </summary>
	public partial class ProjectLists : System.Web.UI.UserControl, IToolbarLight
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ctrlLists.IsProject = true;
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

		#region IToolbarLight Members

		public BlockHeaderLightWithMenu GetToolBar()
		{
			return secHeader;
		}

		#endregion
	}
}
