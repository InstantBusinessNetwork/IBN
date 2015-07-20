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

namespace Mediachase.Ibn.Web.UI.Apps.Security.Modules.ManageControls
{
	public partial class GlobalRoleAclEdit : System.Web.UI.UserControl
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
			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(ClassName))
			{
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
			principal.Label = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Principal").ToString() + ":";
			if (PrincipalId != -1)
			{
				principal.Value = PrincipalId;
				principal.ReadOnly = true;

				MetaObject mo = Mediachase.Ibn.Data.Services.Security.GetGlobalAceByPrincipal(mc, PrincipalId);

				if (mo != null)
				{
					MetaObjectPropertyCollection properties = mo.Properties;

					for (int i = 0; i < rights.Count; i++)
					{
						string rightName = rights[i].ToString();
						((CheckControl)checkControls[i]).Value = (int)properties[rightName].Value;
					}
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

			bool isNew = (PrincipalId == -1);

			if (isNew)	// New
			{
				// Check for unicity
				if (Mediachase.Ibn.Data.Services.Security.GetGlobalAceByPrincipal(mc, (int)(PrimaryKeyId)principal.Value) != null)
				{
					// already exists
					if (Page.Validators.Count > 0)
						Page.Validators[0].IsValid = false;

					return;
				}
			}


			// Saving
			MetaObject mo = null;
			if (!isNew)
				mo = Mediachase.Ibn.Data.Services.Security.GetGlobalAceByPrincipal(mc, PrincipalId);

			if (mo == null)
				mo = new MetaObject(mc);

			MetaObjectPropertyCollection properties = mo.Properties;
			properties[principalField].Value = principal.Value;
			for (int i = 0; i < rights.Count; i++)
			{
				string rightName = rights[i].ToString();
				properties[rightName].Value = ((CheckControl)checkControls[i]).Value;
			}

			mo.Save();

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