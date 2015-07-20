using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls
{
	public partial class GlobalRoleAclStateEdit : System.Web.UI.UserControl
	{
		private string principalField = "PrincipalId";
		protected ArrayList checkControls = new ArrayList();
		protected ArrayList rights = new ArrayList();
		protected MetaClass mc = null;
		protected string labelColumnWidth = "135px";

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

			GenerateStructure();
			if (!IsPostBack)
			{
				BindData();
			}
		}

		#region GenerateStructure
		private void GenerateStructure()
		{
			CustomTableRow[] classRights = CustomTableRow.List(SqlContext.Current.Database.Tables[Mediachase.Ibn.Data.Services.Security.BaseRightsTableName],
				FilterElement.EqualElement("ClassOnly", 1));

			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(ClassName))
			{
				// Check for Class Right (ex. Create)
				bool isClassRight = false;
				string rightUid = right.BaseRightUid.ToString();
				foreach (CustomTableRow r in classRights)
				{
					if (r["BaseRightUid"].ToString() == rightUid)
					{
						isClassRight = true;
						break;
					}
				}
				if (isClassRight)
					continue;

				HtmlTableRow tr = new HtmlTableRow();
				HtmlTableCell td = new HtmlTableCell();
				CheckControl ctrl = (CheckControl)LoadControl("~/Apps/Common/Design/CheckControl.ascx");
				ctrl.Text = CHelper.GetResFileString(right.FriendlyName);

				td.Controls.Add(ctrl);
				tr.Cells.Add(td);
				tblRights.Rows.Add(tr);

				checkControls.Add(ctrl);
				rights.Add(right.RightName);
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

			if (moState == null)
				moState = mo;

			if (moState != null)
			{
				MetaObjectPropertyCollection properties = moState.Properties;

				for (int i = 0; i < rights.Count; i++)
				{
					string rightName = rights[i].ToString();
					((CheckControl)checkControls[i]).Value = (int)properties[rightName].Value;
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
			}

			MetaObjectPropertyCollection properties = moState.Properties;
			properties[StateMachineUtil.GlobalAclField].Value = mo.PrimaryKeyId.Value;
			properties[StateMachineUtil.StateMachineField].Value = StateMachineId;
			properties[StateMachineUtil.StateField].Value = StateId;

			for (int i = 0; i < rights.Count; i++)
			{
				string rightName = rights[i].ToString();
				properties[rightName].Value = ((CheckControl)checkControls[i]).Value;
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