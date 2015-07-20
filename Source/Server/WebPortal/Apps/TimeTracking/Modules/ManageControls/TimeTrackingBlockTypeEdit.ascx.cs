using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.ManageControls
{
	public partial class TimeTrackingBlockTypeEdit : System.Web.UI.UserControl
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

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Mediachase.IBN.Business.Configuration.TimeTrackingCustomization)
				throw new LicenseRestrictionException();

			if (!IsPostBack)
			{
				BindData();
				txtName.Focus();
			}
		}

		#region BindData
		private void BindData()
		{
			ListItem li;

			// StateMachine
			Mediachase.Ibn.Data.Services.StateMachine[] smList = StateMachineManager.GetAvailableStateMachines(TimeTrackingManager.BlockMetaClassName);
			foreach (Mediachase.Ibn.Data.Services.StateMachine sm in smList)
			{
				li = new ListItem(CHelper.GetResFileString(sm.Name), sm.PrimaryKeyId.ToString());
				ddlStateMachine.Items.Add(li);
			}

			// BlockCard
			MetaClass mcBlock = TimeTrackingManager.GetBlockMetaClass();
			foreach (MetaClass card in mcBlock.GetCards())
			{
				li = new ListItem(CHelper.GetResFileString(card.FriendlyName), card.Name);
				ddlBlockCard.Items.Add(li);
			}

			// EntryCard
			MetaClass mcEntry = TimeTrackingManager.GetEntryMetaClass();
			foreach (MetaClass card in mcEntry.GetCards())
			{
				li = new ListItem(CHelper.GetResFileString(card.FriendlyName), card.Name);
				ddlEntryCard.Items.Add(li);
			}

			// SuperType
			li = new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ProjectType").ToString(), "1");
			ddlSuperType.Items.Add(li);
			li = new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "GlobalType").ToString(), "0");
			ddlSuperType.Items.Add(li);
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_Click(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			bool isProject = false;
			if (ddlSuperType.SelectedValue == "1")
				isProject = true;

			TimeTrackingManager.AddBlockTypeItem(txtName.Text, ddlBlockCard.SelectedValue, ddlEntryCard.SelectedValue, int.Parse(ddlStateMachine.SelectedValue), isProject);

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