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

namespace Mediachase.UI.Web.Common.Modules
{
	public partial class LegendControl : System.Web.UI.UserControl
	{
		#region ShowLeftBorder
		/// <summary>
		/// Gets or sets a value indicating whether [show left border].
		/// </summary>
		/// <value><c>true</c> if [show left border]; otherwise, <c>false</c>.</value>
		public bool ShowLeftBorder
		{
			get
			{
				bool retval = true;
				if (ViewState["ShowLeftBorder"] != null)
					retval = (bool)ViewState["ShowLeftBorder"];
				return retval;
			}
			set
			{
				ViewState["ShowLeftBorder"] = value;
			}
		}
		#endregion

		#region ShowRightBorder
		/// <summary>
		/// Gets or sets a value indicating whether [show right border].
		/// </summary>
		/// <value><c>true</c> if [show right border]; otherwise, <c>false</c>.</value>
		public bool ShowRightBorder
		{
			get
			{
				bool retval = true;
				if (ViewState["ShowRightBorder"] != null)
					retval = (bool)ViewState["ShowRightBorder"];
				return retval;
			}
			set
			{
				ViewState["ShowRightBorder"] = value;
			}
		}
		#endregion

		#region ShowTopBorder
		/// <summary>
		/// Gets or sets a value indicating whether [show top border].
		/// </summary>
		/// <value><c>true</c> if [show top border]; otherwise, <c>false</c>.</value>
		public bool ShowTopBorder
		{
			get
			{
				bool retval = true;
				if (ViewState["ShowTopBorder"] != null)
					retval = (bool)ViewState["ShowTopBorder"];
				return retval;
			}
			set
			{
				ViewState["ShowTopBorder"] = value;
			}
		}
		#endregion

		#region ShowBottomBorder
		/// <summary>
		/// Gets or sets a value indicating whether [show bottom border].
		/// </summary>
		/// <value><c>true</c> if [show bottom border]; otherwise, <c>false</c>.</value>
		public bool ShowBottomBorder
		{
			get
			{
				bool retval = true;
				if (ViewState["ShowBottomBorder"] != null)
					retval = (bool)ViewState["ShowBottomBorder"];
				return retval;
			}
			set
			{
				ViewState["ShowBottomBorder"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}