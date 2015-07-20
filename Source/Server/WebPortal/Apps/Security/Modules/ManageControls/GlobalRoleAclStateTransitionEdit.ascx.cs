using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls
{
	public partial class GlobalRoleAclStateTransitionEdit : System.Web.UI.UserControl
	{
		private string principalField = "PrincipalId";
		protected ArrayList checkControls = new ArrayList();
		protected ArrayList states = new ArrayList();
		protected MetaClass mc = null;
		protected string labelColumnWidth = "135px";
		protected Mediachase.Ibn.Data.Services.StateMachine sm = null;

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

		#region PrincipalId
		public int PrincipalId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["PrincipalId"] != null)
					retval = int.Parse(Request.QueryString["PrincipalId"]);
				return retval;
			}
		}
		#endregion

		#region StateMachineId
		public int StateMachineId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["SmId"] != null)
					retval = int.Parse(Request.QueryString["SmId"]);
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
			mc = Mediachase.Ibn.Data.Services.Security.GetGlobalAclMetaClass(ClassName);
			sm = new Mediachase.Ibn.Data.Services.StateMachine(MetaDataWrapper.GetMetaClassByName(ClassName), StateMachineId);

			GenerateStructure();
			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region GenerateStructure
		private void GenerateStructure()
		{
			MetaObject mo = StateMachineManager.GetState(ClassName, StateId);
			string fromState = mo.Properties["Name"].Value.ToString();

			foreach (State state in sm.States)
			{
				StateTransition st = sm.FindTransition(fromState, state.Name);
				if (st != null)
				{
					HtmlTableRow tr = new HtmlTableRow();
					HtmlTableCell td = new HtmlTableCell();
					CheckControl ctrl = (CheckControl)LoadControl("~/Apps/Common/Design/CheckControl.ascx");
					ctrl.Text = CHelper.GetResFileString(st.Name);

					td.Controls.Add(ctrl);
					tr.Cells.Add(td);
					tblRights.Rows.Add(tr);

					checkControls.Add(ctrl);
					states.Add(state.Name);
				}
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			principal.BindData(mc.Fields[principalField]);
			principal.Value = PrincipalId;
			principal.ReadOnly = true;

			MetaObject mo = Mediachase.Ibn.Data.Services.Security.GetGlobalAceByPrincipal(mc, PrincipalId);
			MetaObject moState = StateMachineUtil.GetGlobalAclStateItem(ClassName, mo.PrimaryKeyId.Value, StateMachineId, StateId);

			if (moState != null)
			{
				MetaObjectPropertyCollection properties = moState.Properties;

				for (int i = 0; i < states.Count; i++)
				{
					string stateName = states[i].ToString();
					((CheckControl)checkControls[i]).Value = (int)properties[stateName].Value;
				}
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			MetaObject mo = Mediachase.Ibn.Data.Services.Security.GetGlobalAceByPrincipal(mc, PrincipalId);
			MetaObject moState = StateMachineUtil.GetGlobalAclStateItem(ClassName, mo.PrimaryKeyId.Value, StateMachineId, StateId);

			if (moState == null)
			{
				MetaClass stateClass = StateMachineUtil.GetGlobalAclStateMetaClass(ClassName);
				moState = new MetaObject(stateClass);

				moState.Properties[StateMachineUtil.GlobalAclField].Value = mo.PrimaryKeyId.Value;
				moState.Properties[StateMachineUtil.StateMachineField].Value = StateMachineId;
				moState.Properties[StateMachineUtil.StateField].Value = StateId;

				// Rights
				foreach (string rightName in Mediachase.Ibn.Data.Services.Security.GetRegisteredRights(ClassName))
				{
					moState.Properties[rightName].Value = mo.Properties[rightName].Value;
				}
			}

			for (int i = 0; i < states.Count; i++)
			{
				string stateName = states[i].ToString();
				moState.Properties[stateName].Value = ((CheckControl)checkControls[i]).Value;
			}
			moState.Save();

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