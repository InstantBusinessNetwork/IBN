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
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls
{
	public partial class ObjectRoleStateEdit : System.Web.UI.UserControl
	{
		protected ArrayList checkControls = new ArrayList();
		protected ArrayList rights = new ArrayList();
		protected MetaClass mc = null;

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

		#region RoleName
		public string RoleName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["Role"] != null)
					retval = Request.QueryString["Role"];
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
			mc = Mediachase.Ibn.Data.Services.RoleManager.GetObjectRoleMetaClass(ClassName);

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

				// adding rows
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
			MetaObject mo = Mediachase.Ibn.Data.Services.RoleManager.GetObjectRole(mc, RoleName);
			MetaObject moState = StateMachineUtil.GetObjectRoleStateItem(ClassName, mo.PrimaryKeyId.Value, StateMachineId, StateId);

			if (mo != null)
			{
				string roleName = mo.Properties["RoleName"].Value.ToString();
				RoleNameLabel.Text = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());
				if (mo.Properties["ClassName"].Value != null)
				{
					string className = (string)mo.Properties["ClassName"].Value;
					if (className.Contains("::"))
					{
						string[] s = new string[] { "::" };
						className = className.Split(s, StringSplitOptions.None)[0];
						RoleNameLabel.Text = String.Format(CultureInfo.InvariantCulture,
							"{0} ({1})",
							RoleNameLabel.Text,
							CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(className).PluralName));
					}
				}
			}

			if (moState == null)
				moState = mo;

			MetaObjectPropertyCollection properties = moState.Properties;

			if (moState != null)
			{
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
			MetaObject mo = Mediachase.Ibn.Data.Services.RoleManager.GetObjectRole(mc, RoleName);
			MetaObject moState = StateMachineUtil.GetObjectRoleStateItem(ClassName, mo.PrimaryKeyId.Value, StateMachineId, StateId);

			if (moState == null)
			{
				MetaClass stateClass = StateMachineUtil.GetObjectRoleStateMetaClass(ClassName);
				moState = new MetaObject(stateClass);
			}

			MetaObjectPropertyCollection properties = moState.Properties;
			properties[StateMachineUtil.RoleField].Value = mo.PrimaryKeyId.Value;
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