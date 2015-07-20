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
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.Apps.StateMachine.Modules.ManageControls
{
	public partial class StateEdit : System.Web.UI.UserControl
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

		#region StateId
		public int StateId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["StateId"] != null)
					retval = int.Parse(Request.QueryString["StateId"]);
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
			if (StateId > 0)
			{
				MetaObject mo = StateMachineManager.GetState(ClassName, StateId);
				txtName.Text = mo.Properties["Name"].Value.ToString();
				txtName.ReadOnly = true;
				txtFriendlyName.Text = mo.Properties["FriendlyName"].Value.ToString();
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!this.Page.IsValid)
				return;

			MetaObject mo = null;
			if (StateId > 0)
			{
				mo = StateMachineManager.GetState(ClassName, StateId);
				mo.Properties["FriendlyName"].Value = txtFriendlyName.Text;
				mo.Save();
			}
			else
			{
				StateMachineManager.AddState(ClassName, txtName.Text, txtFriendlyName.Text);
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