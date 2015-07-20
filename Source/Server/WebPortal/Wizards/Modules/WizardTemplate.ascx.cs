namespace Mediachase.UI.Web.Wizards.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;
	using System.Globalization;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for WizardTemplate.
	/// </summary>
	public partial class WizardTemplate : System.Web.UI.UserControl
	{
		private IWizardControl _wizard;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strWdTemp", typeof(WizardTemplate).Assembly);
		protected System.Web.UI.HtmlControls.HtmlButton btnwewe;

		string _controlname = String.Empty;
		public string ControlName
		{
			get
			{
				return _controlname;
			}
			set
			{
				_controlname = value;
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				BindCurrentData();
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			iconIBN.Href = Page.ResolveUrl("~/portal.ico");
			RegisterScripts();

			btnNext.Attributes.Add("onclick", "DisableButtons(this);");
			if (ControlName != null || ControlName != String.Empty)
			{
				_wizard = (IWizardControl)LoadControl(ControlName);
				phControl.Controls.Add((Control)_wizard);
			}

			if (!IsPostBack)
			{
				BindGloabalValues();
				//	BindCurrentData();
			}
		}

		private void BindGloabalValues()
		{
			ViewState["step"] = 1;
			ViewState["steps"] = _wizard.StepCount;
			topHeader.Text = _wizard.TopTitle;
			if (!_wizard.ShowSteps)
				lblSteps.Visible = false;
		}

		private void BindCurrentData()
		{
			btnBack.Visible = true;
			btnNext.Visible = true;

			btnBack.InnerText = "< " + LocRM.GetString("Back");
			btnNext.InnerText = LocRM.GetString("Next") + " >";
			btnCancel.InnerHtml = LocRM.GetString("Cancel");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			/*			if(btnNext.Attributes["onclick"] == null)
							btnNext.Attributes.Add("onclick","");*/
			btnNext.Attributes["onclick"] = "";

			int stepNumber = (int)ViewState["step"];
			int stepCount = (int)ViewState["steps"];

			if (stepNumber == 1)
				btnBack.Visible = false;

			if (stepNumber == stepCount)
			{
				btnNext.InnerText = LocRM.GetString("Create");
				btnNext.Attributes.Add("onclick", "DisableButtons(this);");
			}

			_wizard.SetStep(stepNumber);

			if (_wizard.MiddleButtonText != null)
				btnNext.InnerText = _wizard.MiddleButtonText;

			if (_wizard.CancelText != null)
				btnCancel.InnerText = _wizard.CancelText;

			if (stepNumber == stepCount + 1)
			{
				btnBack.Visible = false;
				btnNext.Visible = false;
				lblSteps.Visible = false;
				btnCancel.InnerText = LocRM.GetString("Close");
			}

			lblSteps.Text = string.Format(CultureInfo.CurrentUICulture, "{0} {1} {2} {3}", LocRM.GetString("Step"), stepNumber, LocRM.GetString("of"), stepCount);

			topSubHeader.Text = _wizard.Subtitle;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected void btnBack_Click(object sender, System.EventArgs e)
		{
			ViewState["step"] = (int)ViewState["step"] - 1;
			BindCurrentData();
		}

		protected void btnNext_Click(object sender, System.EventArgs e)
		{
			if (btnNext.CausesValidation)
			{
				Page.Validate();
				if (!Page.IsValid)
					return;
			}

			ViewState["step"] = (int)ViewState["step"] + 1;
			BindCurrentData();
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			_wizard.CancelAction();
			if ((int)ViewState["step"] == (int)ViewState["steps"] + 1)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), _wizard.GenerateFinalStepScript(), true);
			else
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "window.close();", true);
		}

		#region Implementation of IWizard
		public System.Web.UI.HtmlControls.HtmlButton GetbtnBack()
		{
			return btnBack;
		}

		public System.Web.UI.HtmlControls.HtmlButton GetbtnNext()
		{
			return btnNext;
		}

		public System.Web.UI.HtmlControls.HtmlButton GetbtnCancel()
		{
			return btnCancel;
		}

		public System.Web.UI.WebControls.Label GettopHeader()
		{
			return topHeader;
		}

		public System.Web.UI.WebControls.Label GettopSubHeader()
		{
			return topSubHeader;
		}

		public System.Web.UI.WebControls.Label GetlblSteps()
		{
			return lblSteps;
		}
		#endregion

		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable btnNext.
		//===========================================================================
		public System.Web.UI.HtmlControls.HtmlButton btnNext
		{
			get { return Migrated_btnNext; }
			//set { Migrated_btnNext = value; }
		}
		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable btnBack.
		//===========================================================================
		public System.Web.UI.HtmlControls.HtmlButton btnBack
		{
			get { return Migrated_btnBack; }
			//set { Migrated_btnBack = value; }
		}

		private void RegisterScripts()
		{
			// Styles
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dialog.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");

			// Scripts
			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");
		}
	}
}
