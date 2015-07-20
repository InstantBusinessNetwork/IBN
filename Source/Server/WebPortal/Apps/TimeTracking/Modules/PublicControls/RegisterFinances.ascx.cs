using System;
using System.Collections;
using System.Data;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class RegisterFinances : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTracking", typeof(RegisterFinances).Assembly);
		private TimeTrackingBlock block = null;

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

		#region BlockId
		public int BlockId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["BlockId"] != null)
					retval = int.Parse(Request.QueryString["BlockId"]);
				return retval;
			}
		}
		#endregion
	
		protected void Page_Load(object sender, EventArgs e)
		{
			if (BlockId <= 0)
				throw new ArgumentException("BlockId is wrong or not specified");

			block = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), BlockId);
//			block = new TimeTrackingBlock(BlockId);

			SecurityService ss = block.GetService<SecurityService>();
			if (ss == null || !ss.CheckUserRight(TimeTrackingManager.Right_RegFinances))
				throw new AccessDeniedException();

			btnApprove.Text = LocRM.GetString("tApprove");
			btnCancel.Text = LocRM.GetString("tCancel");
			if (!Page.IsPostBack)
			{
				BindAccounts();
				dtcDate.SelectedDate = Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow).Date;
			}

			btnApprove.CustomImage = CHelper.GetAbsolutePath("/layouts/images/accept.gif");
			btnCancel.CustomImage = CHelper.GetAbsolutePath("/layouts/images/cancel.gif");
		}

		#region BindAccounts
		private void BindAccounts()
		{
			ddAccounts.DataSource = Mediachase.IBN.Business.SpreadSheet.ProjectSpreadSheet.GetFactAvailableRows(block.ProjectId.Value);
			ddAccounts.DataTextField = "Name";
			ddAccounts.DataValueField = "Id";
			ddAccounts.DataBind();
		}
		#endregion

		#region btnApprove_click
		protected void btnApprove_click(object sender, System.EventArgs e)
		{
			Mediachase.IBN.Business.TimeTracking.RegisterFinances(block, ddAccounts.SelectedValue, dtcDate.SelectedDate);

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