using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Security.Modules.MetaClassViewControls
{
	public partial class Rights : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string deleteCommand = "Dlt";
		protected readonly string dialogWidth = "400";
		protected readonly string dialogHeight = "150";

		#region DataItem
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				if (value is MetaClass)
					mc = (MetaClass)value;

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		private MetaClass _mc = null;
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[className] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[className].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[className] = value.Name;
				_mc = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
		public override void DataBind()
		{
			if (mc != null)
			{
				if (!Mediachase.Ibn.Data.Services.Security.IsInstalled(mc))
				{
					this.Visible = false;
					return;
				}

				lnkNew.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewItem").ToString();
				lnkNew.NavigateUrl = String.Format("javascript:ShowWizard(\"{4}?ClassName={0}&btn={1}\", {2}, {3});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), 
					dialogWidth, dialogHeight,
					ResolveUrl("~/Apps/Security/Pages/Admin/RightEdit.aspx"));

				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				// O.R.: Ibn 4.7 add-on
				Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				bool adminMode = true;
				if (pc["TimeTrackingMode"] != null && pc["TimeTrackingMode"] == "dev")
					adminMode = false;

				return !adminMode && Mediachase.Ibn.Data.Services.Security.IsInstalled((MetaClass)dataItem);
			}
			else
			{
				return base.CheckVisibility(dataItem);
			}
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
//			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("CanDelete", typeof(bool));
			dt.Columns.Add("EditLink", typeof(string));

			foreach (SecurityRight right in Mediachase.Ibn.Data.Services.Security.GetMetaClassRights(mc.Name))
			{
				DataRow row = dt.NewRow();
				//					row["Id"] = (int)reader["RightId"];
				string rightName = right.RightName;
				row["Name"] = rightName;
				row["FriendlyName"] = CHelper.GetResFileString(right.FriendlyName);
				if (!right.BaseRightUid.HasValue
					&& rightName != TimeTrackingManager.Right_AddMyTTBlock
					&& rightName != TimeTrackingManager.Right_AddAnyTTBlock
					&& rightName != TimeTrackingManager.Right_RegFinances
					&& rightName != TimeTrackingManager.Right_UnRegFinances
					&& rightName != TimeTrackingManager.Right_ViewFinances)
				{
					row["CanDelete"] = true;
				}
				else
				{
					row["CanDelete"] = false;
				}
				row["EditLink"] = String.Format("javascript:ShowWizard(\"{5}?ClassName={0}&btn={1}&RightId={2}\", {3}, {4});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), 
					right.RightId.ToString(), dialogWidth, dialogHeight,
					ResolveUrl("~/Apps/Security/Pages/Admin/RightEdit.aspx"));

				dt.Rows.Add(row);
			}

			grdMain.DataSource = dt;
			grdMain.DataBind();

			foreach (GridViewRow row in grdMain.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Delete").ToString() + "?')");
			}
		}
		#endregion

		#region grdMain_RowCommand
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == deleteCommand)
			{
				string name = e.CommandArgument.ToString();
				Mediachase.Ibn.Data.Services.Security.UnregisterRight(mc, name);
			}
			BindGrid();
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region grdMain_RowDeleting
		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{

		}
		#endregion
	}
}