using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.UI.Web.Modules;

namespace Mediachase.UI.Web.Modules
{
	public partial class BlockControlSingle : System.Web.UI.UserControl, IToolbarLight
	{
		#region InnerControl
		/// <summary>
		/// Gets or sets the inner control.
		/// </summary>
		/// <value>The inner control.</value>
		public string InnerControl
		{
			set
			{
				ViewState["InnerControl"] = value;
			}
			get
			{
				return (string)ViewState["InnerControl"];
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(InnerControl))
			{
				Control ctrl = Page.LoadControl(InnerControl);
				MainPlaceHolder.Controls.Add(ctrl);
			}
		}

		#region IToolbarLight Members

		public BlockHeaderLightWithMenu GetToolBar()
		{
			return secHeader;
		}

		#endregion
	}
}