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
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.Pages
{
	public partial class ListInfoEdit : System.Web.UI.Page
	{
		#region ClassName
		/// <summary>
		/// Gets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		protected string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request["class"] != null)
					retval = Request["class"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			ListInfo li = ListManager.GetListInfoByMetaClassName(ClassName);
			if(li.IsTemplate)
				pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListTempInfoEdit}");
			else
				pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListInfoEdit}");
		}
	}
}
