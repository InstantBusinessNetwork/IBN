using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
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
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.StateMachine.Modules.MetaClassViewControls
{
	public partial class States : MCDataBoundControl
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
				if (!BusinessObjectServiceManager.IsServiceInstalled(mc, StateMachineService.ServiceName))
				{
					this.Visible = false;
					return;
				}

				lnkNew.Text = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewItem").ToString();
				lnkNew.NavigateUrl = String.Format("javascript:ShowWizard(\"{4}?ClassName={0}&btn={1}\", {2}, {3});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), 
					dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/StateMachine/Pages/Admin/StateEdit.aspx"));

				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem is MetaClass)
			{
				return BusinessObjectServiceManager.IsServiceInstalled((MetaClass)dataItem, StateMachineService.ServiceName);
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
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));

			foreach (MetaObject mo in StateMachineManager.GetAvailableStates(mc))
			{
				DataRow row = dt.NewRow();
				row["Id"] = mo.PrimaryKeyId;
				row["Name"] = mo.Properties["Name"].Value.ToString();
				row["FriendlyName"] = CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString());
				row["EditLink"] = String.Format("javascript:ShowWizard(\"{5}?ClassName={0}&btn={1}&StateId={2}\", {3}, {4});", 
					mc.Name, Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), 
					mo.PrimaryKeyId.ToString(), dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/StateMachine/Pages/Admin/StateEdit.aspx"));

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
				int sId = int.Parse(e.CommandArgument.ToString());
				try
				{
					StateMachineManager.RemoveState(mc, sId);
				}
				catch (SqlException ex)
				{
					if (ex.ErrorCode == -2146232060)
						CommandManager.GetCurrent(this.Page).InfoMessage = GetGlobalResourceObject("IbnFramework.Common", "ReferencesExists").ToString();
					else
						throw ex;
				}
			}
			//BindGrid();
			CHelper.AddToContext("RebindPage", "true");
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			//BindGrid();
			CHelper.AddToContext("RebindPage", "true");
		}
		#endregion

		#region grdMain_RowDeleting
		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{

		}
		#endregion
	}
}