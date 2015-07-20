using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using System.Reflection;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for EMailAntiSpamEdit.
	/// </summary>
	public partial class EMailAntiSpamEdit : System.Web.UI.Page
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string sTitle = "";

		#region RuleId
		private int RuleId
		{
			get
			{
				if (Request["RuleId"] != null)
					return int.Parse(Request["RuleId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dialog.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			sTitle = (RuleId > 0) ? LocRM.GetString("tRuleEdit") : LocRM.GetString("tRuleNew");
			imbSave.Text = LocRM.GetString("tSave");
			imbCancel.Text = LocRM.GetString("tCancel");
			cbFillList.Text = " " + LocRM.GetString("tAutoFillList");
			if (!Page.IsPostBack)
			{
				cbFillList.Checked = true;
				BindDD();
				if (RuleId > 0)
					BindValues();
			}

			imbSave.CustomImage = this.Page.ResolveUrl("~/Layouts/Images/saveitem.gif");
			imbCancel.CustomImage = this.Page.ResolveUrl("~/Layouts/Images/cancel.gif");
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			ddKey.AutoPostBack = (ddType.SelectedValue == "Service");
			trValue.Visible = (ddType.SelectedValue != "Service");
			trFillList.Visible = (ddType.SelectedValue == "Service" && (ddKey.SelectedValue == "WhiteList" || ddKey.SelectedValue == "BlackList"));

			BindToolbar();
		}

		#region BindDD
		private void BindDD()
		{
			ddType.Items.Clear();
			ddType.Items.Add(new ListItem(LocRM.GetString("tContains"), "Contains"));
			ddType.Items.Add(new ListItem(LocRM.GetString("tIsEqual"), "IsEqual"));
			ddType.Items.Add(new ListItem(LocRM.GetString("tRegExMatch"), "RegexMatch"));
			ddType.Items.Add(new ListItem(LocRM.GetString("tService"), "Service"));
			BindKey();

			ddWeight.Items.Clear();
			EMailMessageAntiSpamRule[] list = EMailMessageAntiSpamRule.List();
			for (int i = 0; i < list.Length; i++)
				ddWeight.Items.Add(new ListItem(i.ToString(), i.ToString()));

			rbList.Items.Clear();
			rbList.Items.Add(new ListItem(LocRM.GetString("tAccept"), "true"));
			rbList.Items.Add(new ListItem(LocRM.GetString("tDecline"), "false"));

			if (RuleId < 0)
			{
				ddWeight.Items.Add(new ListItem(list.Length.ToString(), list.Length.ToString()));
				Util.CommonHelper.SafeSelect(ddWeight, list.Length.ToString());
				Util.CommonHelper.SafeSelect(rbList, "true");
			}
		}
		#endregion

		#region BindKey
		private void BindKey()
		{
			ddKey.Items.Clear();
			if (ddType.SelectedValue == "Service")
			{
				ddKey.Items.Add(new ListItem(LocRM.GetString("tBlackList2"), "BlackList"));
				ddKey.Items.Add(new ListItem(LocRM.GetString("tWhiteList2"), "WhiteList"));
				ddKey.Items.Add(new ListItem(LocRM.GetString("tTicket"), "Ticket"));
				ddKey.Items.Add(new ListItem(LocRM.GetString("tIssBoxRulesService"), "IncidentBoxRules"));
			}
			else
			{
				ddKey.Items.Add(new ListItem("[From]", "From"));
				ddKey.Items.Add(new ListItem("[To]", "To"));
				ddKey.Items.Add(new ListItem("[Subject]", "Subject"));
				ddKey.Items.Add(new ListItem("[Body]", "Body"));
				ddKey.Items.Add(new ListItem("[Subject or Body]", "SubjectOrBody"));
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			ddType.ClearSelection();
			ddKey.ClearSelection();
			EMailMessageAntiSpamRule asr = EMailMessageAntiSpamRule.Load(RuleId);
			switch (asr.RuleType)
			{
				case EMailMessageAntiSpamRuleType.Contains:
					Util.CommonHelper.SafeSelect(ddType, "Contains");
					break;
				case EMailMessageAntiSpamRuleType.IsEqual:
					Util.CommonHelper.SafeSelect(ddType, "IsEqual");
					break;
				case EMailMessageAntiSpamRuleType.RegexMatch:
					Util.CommonHelper.SafeSelect(ddType, "RegexMatch");
					break;
				case EMailMessageAntiSpamRuleType.Service:
					Util.CommonHelper.SafeSelect(ddType, "Service");
					break;
				default:
					break;
			}
			BindKey();
			Util.CommonHelper.SafeSelect(ddKey, asr.Key);
			if (ddType.SelectedValue == "Service" && ddKey.SelectedValue == "BlackList")
				cbFillList.Checked = PortalConfig.AutoFillBlackList;
			if (ddType.SelectedValue == "Service" && ddKey.SelectedValue == "WhiteList")
				cbFillList.Checked = PortalConfig.AutoFillWhiteList;
			txtValue.Text = asr.Value;
			Util.CommonHelper.SafeSelect(ddWeight, asr.Weight.ToString());
			Util.CommonHelper.SafeSelect(rbList, asr.Accept.ToString().ToLower());
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = sTitle;
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tClose"), "javascript:window.close();");
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
			this.ddType.SelectedIndexChanged += new EventHandler(ddType_SelectedIndexChanged);
		}
		#endregion

		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			EMailMessageAntiSpamRuleType asmType = EMailMessageAntiSpamRuleType.Contains;
			switch (ddType.SelectedValue)
			{
				case "Contains":
					asmType = EMailMessageAntiSpamRuleType.Contains;
					break;
				case "IsEqual":
					asmType = EMailMessageAntiSpamRuleType.IsEqual;
					break;
				case "RegexMatch":
					asmType = EMailMessageAntiSpamRuleType.RegexMatch;
					break;
				case "Service":
					asmType = EMailMessageAntiSpamRuleType.Service;
					break;
				default:
					break;
			}
			if (RuleId > 0)
			{
				EMailMessageAntiSpamRule asr = EMailMessageAntiSpamRule.Load(RuleId);
				asr.Accept = (rbList.SelectedIndex == 0) ? true : false;
				asr.RuleType = asmType;
				asr.Key = ddKey.SelectedValue;
				asr.Value = txtValue.Text;
				asr.Weight = int.Parse(ddWeight.SelectedValue);
				EMailMessageAntiSpamRule.Update(asr);
			}
			else
				EMailMessageAntiSpamRule.Create((rbList.SelectedIndex == 0) ? true : false, asmType, ddKey.SelectedValue, txtValue.Text, int.Parse(ddWeight.SelectedValue));

			if (asmType == EMailMessageAntiSpamRuleType.Service)
			{
				if (ddKey.SelectedValue == "WhiteList")
					PortalConfig.AutoFillWhiteList = cbFillList.Checked;
				if (ddKey.SelectedValue == "BlackList")
					PortalConfig.AutoFillBlackList = cbFillList.Checked;
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script type='text/javascript'>" +
				"try {window.opener.location.href=window.opener.location.href;}" +
				"catch (e){} window.close();</script>");
		}

		private void ddType_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindKey();
		}

	}
}
