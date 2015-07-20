//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class 
// in file 'App_Code\Migrated\Common\Modules\Stub_LatestFiles_ascx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access 
// the code-behind page using the abstract base class.
// The associated content page 'Common\Modules\LatestFiles.ascx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
namespace Mediachase.UI.Web.Common.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
using Mediachase.UI.Web.Modules;

	/// <summary>
	///		Summary description for LatestFiles.
	/// </summary>
	public partial class LatestFiles : System.Web.UI.UserControl, IToolbarLight
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strLatestDocuments", typeof(LatestFiles).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			tbLatestDocuments.AddText(LocRM.GetString("tbLatestDocuments"));
		}

		#region IToolbarLight Members

		public BlockHeaderLightWithMenu GetToolBar()
		{
			return tbLatestDocuments;
		}

		#endregion
	}
}
