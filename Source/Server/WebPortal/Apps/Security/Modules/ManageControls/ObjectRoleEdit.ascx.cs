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
	public partial class ObjectRoleEdit : System.Web.UI.UserControl
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
			Role role = Mediachase.Ibn.Data.Services.RoleManager.GetObjectRole(mc, RoleName);

			if (role != null)
			{
				RoleNameLabel.Text = CHelper.GetResFileString(role.Properties["FriendlyName"].Value.ToString());
				if (role.Properties["ClassName"].Value != null)
				{
					string className = (string)role.Properties["ClassName"].Value;
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

				MetaObjectPropertyCollection properties = role.Properties;
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
			// Saving
			Role role = Mediachase.Ibn.Data.Services.RoleManager.GetObjectRole(mc, RoleName);
			for (int i = 0; i < rights.Count; i++)
			{
				role.Properties[rights[i].ToString()].Value = ((CheckControl)checkControls[i]).Value;
			}
			role.Save();

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