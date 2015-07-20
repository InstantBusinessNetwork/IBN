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
	public partial class StateMachineEdit : System.Web.UI.UserControl
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

		#region SMId
		public int SMId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["SMId"] != null)
					retval = int.Parse(Request.QueryString["SMId"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			btnAddOne.Attributes.Add("onclick", "MoveOne(" + lstAvailableStates.ClientID + "," + lstSelectedStates.ClientID + "); return false;");
			btnAddAll.Attributes.Add("onclick", "MoveAll(" + lstAvailableStates.ClientID + "," + lstSelectedStates.ClientID + "); return false;");
			btnRemoveOne.Attributes.Add("onclick", "MoveOne(" + lstSelectedStates.ClientID + "," + lstAvailableStates.ClientID + "); return false;");
			btnRemoveAll.Attributes.Add("onclick", "MoveAll(" + lstSelectedStates.ClientID + "," + lstAvailableStates.ClientID + ");return false;");

			btnUp.Attributes.Add("onclick", "MoveUp(" + lstSelectedStates.ClientID + ");return false;");
			btnDn.Attributes.Add("onclick", "MoveDown(" + lstSelectedStates.ClientID + ");return false;");

			lstAvailableStates.Attributes.Add("ondblclick", "MoveOne(" + lstAvailableStates.ClientID + "," + lstSelectedStates.ClientID + "); return false;");
			lstSelectedStates.Attributes.Add("ondblclick", "MoveOne(" + lstSelectedStates.ClientID + "," + lstAvailableStates.ClientID + "); return false;");

			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region BindData
		private void BindData()
		{
			if (SMId > 0)
			{
				Mediachase.Ibn.Data.Services.StateMachine sm = StateMachineManager.GetStateMachine(ClassName, SMId);
				txtName.Text = sm.Name;

				foreach (State state in sm.States)
				{
					MetaObject mo = StateMachineManager.GetState(ClassName, state.Name);
					lstSelectedStates.Items.Add(new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), state.Name));
				}

				foreach (MetaObject mo in StateMachineManager.GetAvailableStates(ClassName))
				{
					string sName = mo.Properties["Name"].Value.ToString();
					ListItem item = lstSelectedStates.Items.FindByValue(sName);
					if (item == null)
						lstAvailableStates.Items.Add(new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), mo.Properties["Name"].Value.ToString()));
				}
			}
			else
			{
				foreach (MetaObject mo in StateMachineManager.GetAvailableStates(ClassName))
				{
					lstAvailableStates.Items.Add(new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), mo.Properties["Name"].Value.ToString()));
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

			Mediachase.Ibn.Data.Services.StateMachine sm = null;
			if (SMId > 0)
			{
				sm = StateMachineManager.GetStateMachine(ClassName, SMId);
				sm.States.Clear();
			}
			else
			{
				sm = new Mediachase.Ibn.Data.Services.StateMachine(MetaDataWrapper.GetMetaClassByName(ClassName));
			}

			sm.Name = txtName.Text;

			foreach (string s in hdnStates.Value.Split(','))
			{
				sm.States.Add(new State(s));
			}

			sm.Save();

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

		#region vldSelectedStates_ServerValidate
		protected void vldSelectedStates_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (hdnStates.Value == String.Empty)
				args.IsValid = false;
			else
				args.IsValid = true;
		}
		#endregion
	}
}