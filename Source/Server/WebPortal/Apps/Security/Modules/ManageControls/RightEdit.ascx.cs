using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls
{
	public partial class RightEdit : System.Web.UI.UserControl
	{
		protected string labelColumnWidth = "120px";

		#region RefreshButton
		public string RefreshButton
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["btn"] != null)
					retval = Request.QueryString["btn"];
				return retval;
			}
		}
		#endregion

		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ClassName"] != null)
					retval = Request.QueryString["ClassName"];
				return retval;
			}
		}
		#endregion

		#region RightId
		public int RightId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["RightId"] != null)
					retval = int.Parse(Request.QueryString["RightId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindData();
			}

			txtName.Attributes.Add("onblur", "SetName('" + txtName.ClientID + "','" + txtFriendlyName.ClientID + "','" + rfvFriendlyName.ClientID + "')");
		}

		#region BindData
		private void BindData()
		{
			if (RightId > 0)
			{
				CustomTableRow row = Mediachase.Ibn.Data.Services.Security.GetRightItem(ClassName, RightId);
				if (row != null)
				{
					txtName.Text = row["RightName"].ToString();
					txtName.ReadOnly = true;
					txtFriendlyName.Text = row["FriendlyName"].ToString();
				}
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!this.Page.IsValid)
				return;

			if (RightId > 0)
			{
				CustomTableRow row = Mediachase.Ibn.Data.Services.Security.GetRightItem(ClassName, RightId);
				row["FriendlyName"] = txtFriendlyName.Text;
				row.Update();
			}
			else
			{
				Mediachase.Ibn.Data.Services.Security.RegisterRight(ClassName, txtName.Text, txtFriendlyName.Text);
			}


			// Closing window
			if (RefreshButton == String.Empty)
			{
				CHelper.CloseItAndRefresh(Response);
			}
			else  // Dialog Mode
			{
				CHelper.CloseItAndRefresh(Response, RefreshButton);
			}

		}
		#endregion
	}
}