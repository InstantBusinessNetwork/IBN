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
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.TemplateControls
{
	public partial class ListTemplateCreate : System.Web.UI.UserControl
	{
		protected string HeaderText = "";
		protected string SubHeaderText = "";
		protected string StepText = "";
		private const int _maxStepCount = 2;

		protected void Page_Load(object sender, EventArgs e)
		{	
			ucWizard.DisplaySideBar = false;
			ucWizard.DisplayCancelButton = true;

			ucWizard.CancelButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tClose}");
			ucWizard.FinishCompleteButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tSave}");
			ucWizard.StartNextButtonText = CHelper.GetResFileString("{IbnFramework.ListInfo:tNext}") + " >";
			ucWizard.FinishPreviousButtonText = "< " + CHelper.GetResFileString("{IbnFramework.ListInfo:tPrev}");

			ucWizard.CancelButtonClick += new EventHandler(ucWizard_CancelButtonClick);
			ucWizard.FinishButtonClick += new WizardNavigationEventHandler(ucWizard_FinishButtonClick);
			ucWizard.ActiveStepChanged += new EventHandler(ucWizard_ActiveStepChanged);

			ApplyStepsLocalization();
		}

		protected override void OnPreRender(EventArgs e)
		{
			DefineHeaderTexts();
			base.OnPreRender(e);
		}

		#region ApplyStepsLocalization
		private void ApplyStepsLocalization()
		{
			lblTitle.Text = CHelper.GetResFileString("{IbnFramework.ListInfo:tListTemplateTitle}") + ":";
			cbWithData.Text = " " + CHelper.GetResFileString("{IbnFramework.ListInfo:tSaveWithData}");
		} 
		#endregion

		#region DefineHeaderTexts
		private void DefineHeaderTexts()
		{
			HeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:tListTemplateCreateHeader}");
			SubHeaderText = CHelper.GetResFileString("{IbnFramework.ListInfo:tListTemplateCreateSubHeader}");
			if (ucWizard.ActiveStep.ID == "step1")
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"1", _maxStepCount.ToString());
			else
				StepText = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tStepByStep}"),
					"2", _maxStepCount.ToString());
		} 
		#endregion

		#region ucWizard_ActiveStepChanged
		void ucWizard_ActiveStepChanged(object sender, EventArgs e)
		{
			if (ucWizard.ActiveStep.ID == "step2")
			{
				lblText.Text = String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:tFinishWizardText1}"),
					tbName.Text,
					ListManager.GetListInfoByMetaClassName(Request["class"]).Title);
				if (cbWithData.Checked)
					lblText.Text += "<br /><br />" + CHelper.GetResFileString("{IbnFramework.ListInfo:tFinishWizardText2}");
			}
		} 
		#endregion

		#region ucWizard_FinishButtonClick
		void ucWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
		{
			ListManager.CreateTemplate(tbName.Text, ListManager.GetListInfoByMetaClassName(Request["class"]), cbWithData.Checked);
			this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString("N"),
				String.Format("window.opener.location.href='{0}';window.close();", ResolveClientUrl("~/Apps/ListApp/Pages/ListTemplates.aspx")), true);
		} 
		#endregion

		#region ucWizard_CancelButtonClick
		void ucWizard_CancelButtonClick(object sender, EventArgs e)
		{
			this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString("N"),
				"window.close();", true);
		} 
		#endregion
	}
}