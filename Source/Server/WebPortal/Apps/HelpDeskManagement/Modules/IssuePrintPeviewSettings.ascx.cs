using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class IssuePrintPeviewSettings : System.Web.UI.UserControl
	{
		UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private const string prefix = "IncidentView_Print_";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				ApplyLocalization();
				BindSavedValues();
			}

			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Print").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/print.gif");

			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			
			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}return false;", Request["closeFramePopup"]));
			
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			showHeader.Title = GetGlobalResourceObject("IbnFramework.Incident", "IssueGenInfoPrinting").ToString();

			string prefix = " ";
			showClient.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showClient").ToString();
			showCode.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showCode").ToString();
			showCreationInfo.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showCreationInfo").ToString();
			showDescription.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showDescription").ToString();
			showExpectedDate.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showExpectedDate").ToString();
			showGenCats.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showGenCats").ToString();
			showIssBox.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showIssBox").ToString();
			showIssCats.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showIssCats").ToString();
			showLastModifiedInfo.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showLastModifiedInfo").ToString();
			showManager.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showManager").ToString();
			showOpenDate.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showOpenDate").ToString();
			showPriority.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showPriority").ToString();
			showResolveDate.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showResolveDate").ToString();
			showResponsible.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showResponsible").ToString();
			showStatus.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showStatus").ToString();
			showTitle.Text = prefix + GetGlobalResourceObject("IbnFramework.Incident", "showTitle").ToString();

			showForum.Title = GetGlobalResourceObject("IbnFramework.Incident", "ForumInfoPrinting").ToString();

			rbList.Items.Clear();
			rbList.Items.Add(new ListItem(prefix + GetGlobalResourceObject("IbnFramework.Incident", "NotPrintForum").ToString(), "-1"));
			rbList.Items.Add(new ListItem(prefix + GetGlobalResourceObject("IbnFramework.Incident", "LastPostPrintForum").ToString(), "1"));
			rbList.Items.Add(new ListItem(prefix + GetGlobalResourceObject("IbnFramework.Incident", "Last3PostsPrintForum").ToString(), "3"));
			rbList.Items.Add(new ListItem(prefix + GetGlobalResourceObject("IbnFramework.Incident", "AllPostsPrintForum").ToString(), "0"));
			rbList.SelectedIndex = 1;
		} 
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (_pc[prefix + "showClient"] != null)
				showClient.Checked = bool.Parse(_pc[prefix + "showClient"]);
			if (_pc[prefix + "showCode"] != null)
				showCode.Checked = bool.Parse(_pc[prefix + "showCode"]);
			if (_pc[prefix + "showCreationInfo"] != null)
				showCreationInfo.Checked = bool.Parse(_pc[prefix + "showCreationInfo"]);
			if (_pc[prefix + "showDescription"] != null)
				showDescription.Checked = bool.Parse(_pc[prefix + "showDescription"]);
			if (_pc[prefix + "showExpectedDate"] != null)
				showExpectedDate.Checked = bool.Parse(_pc[prefix + "showExpectedDate"]);
			if (_pc[prefix + "showGenCats"] != null)
				showGenCats.Checked = bool.Parse(_pc[prefix + "showGenCats"]);
			if (_pc[prefix + "showIssBox"] != null)
				showIssBox.Checked = bool.Parse(_pc[prefix + "showIssBox"]);
			if (_pc[prefix + "showIssCats"] != null)
				showIssCats.Checked = bool.Parse(_pc[prefix + "showIssCats"]);
			if (_pc[prefix + "showLastModifiedInfo"] != null)
				showLastModifiedInfo.Checked = bool.Parse(_pc[prefix + "showLastModifiedInfo"]);
			if (_pc[prefix + "showManager"] != null)
				showManager.Checked = bool.Parse(_pc[prefix + "showManager"]);
			if (_pc[prefix + "showOpenDate"] != null)
				showOpenDate.Checked = bool.Parse(_pc[prefix + "showOpenDate"]);
			if (_pc[prefix + "showPriority"] != null)
				showPriority.Checked = bool.Parse(_pc[prefix + "showPriority"]);
			if (_pc[prefix + "showResolveDate"] != null)
				showResolveDate.Checked = bool.Parse(_pc[prefix + "showResolveDate"]);
			if (_pc[prefix + "showResponsible"] != null)
				showResponsible.Checked = bool.Parse(_pc[prefix + "showResponsible"]);
			if (_pc[prefix + "showStatus"] != null)
				showStatus.Checked = bool.Parse(_pc[prefix + "showStatus"]);
			if (_pc[prefix + "showTitle"] != null)
				showTitle.Checked = bool.Parse(_pc[prefix + "showTitle"]);
			if (_pc[prefix + "showForum"] != null)
				CHelper.SafeSelect(rbList, _pc[prefix + "showForum"]);
		} 
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			_pc[prefix + "showClient"] = showClient.Checked.ToString();
			_pc[prefix + "showCode"] = showCode.Checked.ToString();
			_pc[prefix + "showCreationInfo"] = showCreationInfo.Checked.ToString();
			_pc[prefix + "showDescription"] = showDescription.Checked.ToString();
			_pc[prefix + "showExpectedDate"] = showExpectedDate.Checked.ToString();
			_pc[prefix + "showGenCats"] = showGenCats.Checked.ToString();
			_pc[prefix + "showIssBox"] = showIssBox.Checked.ToString();
			_pc[prefix + "showIssCats"] = showIssCats.Checked.ToString();
			_pc[prefix + "showLastModifiedInfo"] = showLastModifiedInfo.Checked.ToString();
			_pc[prefix + "showManager"] = showManager.Checked.ToString();
			_pc[prefix + "showOpenDate"] = showOpenDate.Checked.ToString();
			_pc[prefix + "showPriority"] = showPriority.Checked.ToString();
			_pc[prefix + "showResolveDate"] = showResolveDate.Checked.ToString();
			_pc[prefix + "showResponsible"] = showResponsible.Checked.ToString();
			_pc[prefix + "showStatus"] = showStatus.Checked.ToString();
			_pc[prefix + "showTitle"] = showTitle.Checked.ToString();
			_pc[prefix + "showForum"] = rbList.SelectedValue;
		} 
		#endregion

		void btnSave_ServerClick(object sender, EventArgs e)
		{
			SaveValues();
			if (Request["IncidentId"] != null)
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_HDM_PrintIssue");
				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("IncidentId", Request["IncidentId"]);
				string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString("N"),
					String.Format(@"function closeWin()
									{{ 
										try{{
											window.parent.{1}();
										}}
										catch(ex){{;}} 
									}} 
									function openWin()
									{{
										try{{
											{0}; 
											setTimeout('closeWin()', 500);
										}}
										catch(ex){{
											setTimeout('openWin()', 500);
										}}
									}} 
									setTimeout('openWin()', 1000);", 
							
							cmd, Request["closeFramePopup"]), true);
			}
			else
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}
	}
}