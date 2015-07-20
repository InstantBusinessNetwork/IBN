using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.IbnCommon.Modules
{
	public partial class ViewObjectNoBind : System.Web.UI.UserControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				if (ViewState["ClassName"] != null)
					return (string)ViewState["ClassName"];
				else
					return "";
			}
			set
			{
				ViewState["ClassName"] = value;
			}
		}
		#endregion

		#region PlaceName
		public string PlaceName
		{
			get
			{
				if (ViewState["PlaceName"] != null)
					return (string)ViewState["PlaceName"];
				else
					return "";
			}
			set
			{
				ViewState["PlaceName"] = value;
			}
		}
		#endregion

		#region MenuWidth
		public int MenuWidth
		{
			set
			{
				xmlStruct.MenuWidth = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

			if (!Page.IsPostBack)
			{
				if (!String.IsNullOrEmpty(ClassName))
					xmlStruct.ClassName = ClassName;
				if (!String.IsNullOrEmpty(PlaceName))
					xmlStruct.PlaceName = PlaceName;
				xmlStruct.LayoutType = LayoutType.ObjectView;
				xmlStruct.LayoutMode = LayoutMode.LeftMenu;

				xmlStruct.DataBind();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (CHelper.NeedToDataBind())
			{
				xmlStruct.DataBind();
			}
		}
		#endregion

		#region Page_PreRenderComplete
		void Page_PreRenderComplete(object sender, EventArgs e)
		{
			CHelper.RequireDataBind(false);
		}
		#endregion
	}
}