using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web
{
	public partial class UserPhoto : System.Web.UI.Page
	{
		#region iUserID
		private int iUserID
		{
			get
			{
				try
				{
					return int.Parse(Request["id"]);
				}
				catch
				{
					return -1;
				}
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			string path = String.Format("/Common/GetUserPhoto.aspx?OriginalId={0}&t={1}", iUserID, DateTime.Now.Millisecond.ToString());
			Response.Redirect(path, true);
		}
	}
}
