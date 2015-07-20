using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Apps.Common.Layout.Modules
{
	public partial class AddFrameBuilder : System.Web.UI.UserControl
	{
		#region prop: IsAdmin
		/// <summary>
		/// Gets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				if (Request["IsAdmin"] == null)
					return false;

				return true;
				//return Convert.ToBoolean(Request["IsAdmin"], CultureInfo.InvariantCulture);
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				xmlStruct.ClassName = string.Empty;
				
				if (this.IsAdmin)
					xmlStruct.PlaceName = "WorkspaceAdmin";
				else
					xmlStruct.PlaceName = "Workspace";

				xmlStruct.LayoutType = LayoutType.Custom;

				xmlStruct.DataBind();
			}
		}
	}
}