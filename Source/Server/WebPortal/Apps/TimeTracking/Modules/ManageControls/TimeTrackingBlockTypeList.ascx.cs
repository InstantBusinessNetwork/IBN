using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;


namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.ManageControls
{
	public partial class TimeTrackingBlockTypeList : System.Web.UI.UserControl
	{
		protected readonly string dialogWidth = "400";
		protected readonly string dialogHeight = "250";
		protected readonly string deleteCommand = "Dlt";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Mediachase.IBN.Business.Configuration.TimeTrackingCustomization)
				throw new LicenseRestrictionException();

			if (!IsPostBack)
			{
				BindGrid();
			}

			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "BlockTypes").ToString();
			secHeader.AddLink(
				"<img src='" + CHelper.GetAbsolutePath("/images/IbnFramework/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" +
				GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewItem").ToString(),
				String.Format("javascript:ShowWizard(&quot;TimeTrackingBlockTypeEdit.aspx?btn={0}&quot;, {1}, {2})", Page.ClientScript.GetPostBackEventReference(btnRefresh, ""), dialogWidth, dialogHeight));
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Title", typeof(string));
			dt.Columns.Add("SuperType", typeof(string));
			dt.Columns.Add("StateMachine", typeof(string));
			dt.Columns.Add("BlockCard", typeof(string));
			dt.Columns.Add("EntryCard", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));

			foreach (MetaObject mo in TimeTrackingManager.GetBlockTypeList())
			{
				DataRow row = dt.NewRow();
				row["Id"] = mo.PrimaryKeyId;
				row["Title"] = mo.Properties["Title"].Value.ToString();
				if ((bool)mo.Properties["IsProject"].Value)
					row["SuperType"] = GetGlobalResourceObject("IbnFramework.TimeTracking", "ProjectType").ToString();
				else
					row["SuperType"] = GetGlobalResourceObject("IbnFramework.TimeTracking", "GlobalType").ToString();

				int stateMachineId = (Mediachase.Ibn.Data.PrimaryKeyId)mo.Properties["StateMachineId"].Value;
				Mediachase.Ibn.Data.Services.StateMachine sm = StateMachineManager.GetStateMachine(TimeTrackingManager.BlockMetaClassName, stateMachineId);
				row["StateMachine"] = CHelper.GetResFileString(sm.Name);

				if (mo.Properties["BlockCard"].Value != null)
				{
					string cardName = mo.Properties["BlockCard"].Value.ToString();
					MetaClass mcCard = MetaDataWrapper.GetMetaClassByName(cardName);
					if (mcCard != null)
						row["BlockCard"] = CHelper.GetResFileString(mcCard.FriendlyName);
				}
				if (mo.Properties["EntryCard"].Value != null)
				{
					string cardName = mo.Properties["EntryCard"].Value.ToString();
					MetaClass mcCard = MetaDataWrapper.GetMetaClassByName(cardName);
					if (mcCard != null)
						row["EntryCard"] = CHelper.GetResFileString(mcCard.FriendlyName);
				}

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
				int id = int.Parse(e.CommandArgument.ToString());
				try
				{
					TimeTrackingManager.DeleteBlockTypeItem(id);
				}
				catch (SqlException ex)
				{
					if (ex.ErrorCode == -2146232060)
						CommandManager.GetCurrent(this.Page).InfoMessage = GetGlobalResourceObject("IbnFramework.Common", "ReferencesExists").ToString();
					else
						throw ex;
				}
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
	}
}