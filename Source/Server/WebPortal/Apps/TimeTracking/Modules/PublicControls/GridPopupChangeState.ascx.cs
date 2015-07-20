using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Apps.TimeTracking.Modules.PublicControls
{
	public partial class GridPopupChangeState : System.Web.UI.UserControl
	{
		#region BlockId
		public int BlockId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["BlockId"] != null)
				{
					int.TryParse(Request.QueryString["BlockId"], NumberStyles.Integer, CultureInfo.InvariantCulture, out retval);
				}
				return retval;
			}
		}
		#endregion
	
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindData();
			}
		}

		#region BindData
		private void BindData()
		{
			if (BlockId > 0)
			{
				TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), BlockId);
				StateMachineService sms = ttb.GetService<StateMachineService>();
				StateTransition[] nextTransitions = sms.GetNextAvailableTransitions();
				StateTransition[] prevTransitions = sms.GetPrevAvailableTransitions();

				ttbTitle.Text = ttb.Title;
				TTBlockComment.Value = string.Empty;

				BindTransitions(nextTransitions, prevTransitions);
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Request["closeFramePopup"] != null)
			{
				CancelButton.OnClientClick = String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
			}
		}
		#endregion

		#region BindTransitions
		/// <summary>
		/// Binds the transition buttons.
		/// </summary>
		/// <param name="nextTransitions">The next transitions.</param>
		/// <param name="prevTransitions">The prev transitions.</param>
		private void BindTransitions(StateTransition[] nextTransitions, StateTransition[] prevTransitions)
		{
			if (TransitionList.Items.Count > 0)
				TransitionList.Items.Clear();

			ListItem li;
			for (int i = 0; i < prevTransitions.Length; i++)
			{
				li = new ListItem(CHelper.GetResFileString(prevTransitions[i].Name), prevTransitions[i].Uid.ToString());
				TransitionList.Items.Add(li);
			}
			for (int i = 0; i < nextTransitions.Length; i++)
			{
				li = new ListItem(CHelper.GetResFileString(nextTransitions[i].Name), nextTransitions[i].Uid.ToString());
				TransitionList.Items.Add(li);
				if (i == 0)
					li.Selected = true;
			}
			if (TransitionList.Items.Count > 0 && TransitionList.SelectedItem == null)
				TransitionList.Items[0].Selected = true;

		}
		#endregion

		#region TransitionButton_Click
		/// <summary>
		/// Handles the Click event of the TransitionButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void TransitionButton_Click(object sender, EventArgs e)
		{
			if (BlockId > 0 && TransitionList.Items.Count > 0 && TransitionList.SelectedItem != null)
			{
				string selectedTransition = TransitionList.SelectedValue;
				TimeTrackingManager.MakeTransitionWithComment(BlockId, new Guid(selectedTransition), TTBlockComment.Value);

				string cmd = String.Empty;
				if (Request["cmd"] != null)
					cmd = Request["cmd"];
				CommandParameters cp = new CommandParameters(cmd);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion
	}
}