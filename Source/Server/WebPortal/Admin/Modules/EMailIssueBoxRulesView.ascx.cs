namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EMailIssueBoxRulesView.
	/// </summary>
	public partial class EMailIssueBoxRulesView : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));




		#region IssBoxId
		protected int IssBoxId
		{
			get
			{
				if (Request["IssBoxId"] != null)
					return int.Parse(Request["IssBoxId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();
			BindToolbar();
		}

		#region BindValues
		private void BindValues()
		{
			lblRules.Text = "&nbsp;";
			IncidentBox issb = IncidentBox.Load(IssBoxId);
			lblIssBoxName.Text = issb.Name;
			IncidentBoxRule[] ibList = IncidentBoxRule.List(IssBoxId);
			string retVal = "";
			for (int i = 0; i < ibList.Length; i++)
			{
				retVal += "<tr><td width='30px'>";

				IncidentBoxRule ibr = ibList[i];

				if (ibr.RuleType == IncidentBoxRuleType.OrBlock || ibr.RuleType == IncidentBoxRuleType.AndBlock)
				{
					string thisLevel = ibr.OutlineLevel + ibr.IncidentBoxRuleId + ".";
					retVal += GetLabel(ibr, true) + "</td><td><b>(</b>";
					string ss = "";
					i = Reccurence(ibList, out ss, thisLevel, i);
					retVal += ss + "<b>)</b>";
				}
				else
				{
					retVal += GetLabel(ibr, true);
					retVal += "</td><td><b>(</b>&nbsp;" + GetInnerText(ibr) + "<b>)</b>";
				}

				retVal += "</td></tr>";
			}
			if (retVal != "")
				lblRules.Text = String.Format("<table cellspacing='0' cellpadding='5' border='0' class='text'>{0}</table>", retVal);
			else
				lblRules.Text = "<font color='red' class='text'>" + LocRM.GetString("tIssueBoxRules") + "</font>";
		}
		#endregion

		#region Reccurence
		private int Reccurence(IncidentBoxRule[] ibList, out string ss, string thisLevel, int i)
		{
			ss = "";
			while (++i < ibList.Length && ibList[i].OutlineLevel.StartsWith(thisLevel))
			{
				IncidentBoxRule ibr = ibList[i];
				if (ibr.RuleType == IncidentBoxRuleType.OrBlock || ibr.RuleType == IncidentBoxRuleType.AndBlock)
				{
					string thisLevel1 = ibr.OutlineLevel + ibr.IncidentBoxRuleId + ".";
					ss += GetLabel(ibr, false) + "&nbsp;<b>(</b>";
					string ss1 = "";
					i = Reccurence(ibList, out ss1, thisLevel1, i);
					ss += ss1 + "<b>)</b>";
				}
				else
					ss += GetLabel(ibr, false) + "&nbsp;<b>(</b>&nbsp;" + GetInnerText(ibr) + "<b>)&nbsp;</b>";
			}
			return --i;
		}
		#endregion

		#region GetLabel
		private string GetLabel(IncidentBoxRule ibr, bool IsFirst)
		{
			string retVal = "";
			if (IsFirst)
			{
				int iLevel = Util.CommonHelper.CountOf(ibr.OutlineLevel, ".") - 1;
				if (iLevel > 0)
				{
					iLevel = iLevel * 40;
					retVal += String.Format("<img alt='' src='{1}' width='{0}px' height='1px' border='0' />",
						iLevel, this.Page.ResolveUrl("~/layouts/images/blank.gif"));
				}
			}

			string sAnd_OR = "";
			if (ibr.OutlineLevel == ".")
			{
				if (ibr.OutlineIndex > 0)
					sAnd_OR = LocRM.GetString("tAND");
			}
			else
			{
				string sLevel = ibr.OutlineLevel;
				if (sLevel.EndsWith("."))
					sLevel = sLevel.Substring(0, sLevel.Length - 1);
				sLevel = sLevel.Substring(sLevel.LastIndexOf(".") + 1);
				int iNum = int.Parse(sLevel);
				IncidentBoxRule ibr1 = IncidentBoxRule.Load(iNum);
				if (ibr1.RuleType == IncidentBoxRuleType.OrBlock && ibr.OutlineIndex > ibr1.OutlineIndex + 1)
					sAnd_OR = LocRM.GetString("tOR");
				if (ibr1.RuleType == IncidentBoxRuleType.AndBlock && ibr.OutlineIndex > ibr1.OutlineIndex + 1)
					sAnd_OR = LocRM.GetString("tAND");
			}
			if (IsFirst || sAnd_OR != "")
				retVal += "<font color='green'><b>" + sAnd_OR + "</b></font>";
			return retVal;
		}
		#endregion

		#region GetInnerText
		private string GetInnerText(IncidentBoxRule ibr)
		{
			string retVal = "";
			switch (ibr.RuleType)
			{
				case IncidentBoxRuleType.Contains:
				case IncidentBoxRuleType.IsEqual:
				case IncidentBoxRuleType.RegexMatch:
				case IncidentBoxRuleType.NotContains:
				case IncidentBoxRuleType.NotIsEqual:
					string sKey = ibr.Key;
					sKey = (sKey == "TypeId") ? "Type" : sKey;
					sKey = (sKey == "PriorityId") ? "Priority" : sKey;
					sKey = (sKey == "SeverityId") ? "Severity" : sKey;
					sKey = (sKey == "TypeId") ? "Type" : sKey;
					sKey = (sKey == "CreatorId") ? "Creator" : sKey;
					sKey = (sKey == "ProjectId") ? "Project" : sKey;
					sKey = (sKey == "EMailBox") ? "E-mail Box" : sKey;
					sKey = (sKey == "TitleOrDescriptionOrEMailBody") ? "Title or Description or EMailBody" : sKey;
					retVal += "<font color='#0000ff'>[" + sKey + "]</font>&nbsp;&nbsp;";
					retVal += "<b>" + GetRuleType(ibr.RuleType) + "</b>";
					if (ibr.Value.Length > 0)
					{
						if (sKey == "Type")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.Common.GetIncidentType(int.Parse(ibr.Value)) + "'</font>";
						else if (sKey == "E-mail Box")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.EMail.EMailRouterPop3Box.Load(int.Parse(ibr.Value)).Name + "'</font>";
						else if (sKey == "Creator")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Util.CommonHelper.GetUserStatusPureName(int.Parse(ibr.Value)) + "'</font>";
						else if (sKey == "Project")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Task.GetProjectTitle(int.Parse(ibr.Value)) + "'</font>";
						else if (sKey == "Priority")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.Common.GetPriority(int.Parse(ibr.Value)) + "'</font>";
						else if (sKey == "Severity")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.Common.GetIncidentSeverity(int.Parse(ibr.Value)) + "'</font>";
						else if (sKey == "GeneralCategories" && ibr.Value.Length > 0)
						{
							retVal += "&nbsp;&nbsp;<font color='red'>";
							string[] _values = ibr.Value.Split(';');
							for (int i = 0; i < _values.Length; i++)
							{
								string sId = _values[i];
								if (i > 0)
								{
									if (ibr.RuleType == IncidentBoxRuleType.Contains ||
										ibr.RuleType == IncidentBoxRuleType.NotContains)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tAND") + "&nbsp;&nbsp;";
									if (ibr.RuleType == IncidentBoxRuleType.IsEqual ||
										ibr.RuleType == IncidentBoxRuleType.NotIsEqual)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tOR") + "&nbsp;&nbsp;";
								}
								retVal += "'" +
									Mediachase.IBN.Business.Common.GetGeneralCategory(int.Parse(sId))
									+ "'&nbsp;&nbsp;";
							}
							retVal += "</font>";
						}
						else if (sKey == "IncidentCategories" && ibr.Value.Length > 0)
						{
							retVal += "&nbsp;&nbsp;<font color='red'>";
							string[] _values = ibr.Value.Split(';');
							for (int i = 0; i < _values.Length; i++)
							{
								string sId = _values[i];
								if (i > 0)
								{
									if (ibr.RuleType == IncidentBoxRuleType.Contains ||
										ibr.RuleType == IncidentBoxRuleType.NotContains)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tAND") + "&nbsp;&nbsp;";
									if (ibr.RuleType == IncidentBoxRuleType.IsEqual ||
										ibr.RuleType == IncidentBoxRuleType.NotIsEqual)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tOR") + "&nbsp;&nbsp;";
								}
								retVal += "'" +
									Mediachase.IBN.Business.Common.GetIncidentCategory(int.Parse(sId))
									+ "'&nbsp;&nbsp;";
							}
							retVal += "</font>";
						}
						else
						{
							if (ibr.RuleType == IncidentBoxRuleType.Contains ||
								ibr.RuleType == IncidentBoxRuleType.IsEqual ||
								ibr.RuleType == IncidentBoxRuleType.NotContains ||
								ibr.RuleType == IncidentBoxRuleType.NotIsEqual)
							{
								retVal += "&nbsp;&nbsp;<font color='red'>";
								string[] _values = ibr.Value.Split(';');
								for (int i = 0; i < _values.Length; i++)
								{
									string sId = _values[i];
									if (i > 0)
									{
										if (ibr.RuleType == IncidentBoxRuleType.Contains ||
											ibr.RuleType == IncidentBoxRuleType.NotContains)
											retVal += "&nbsp;&nbsp;" + LocRM.GetString("tAND") + "&nbsp;&nbsp;";
										if (ibr.RuleType == IncidentBoxRuleType.IsEqual ||
											ibr.RuleType == IncidentBoxRuleType.NotIsEqual)
											retVal += "&nbsp;&nbsp;" + LocRM.GetString("tOR") + "&nbsp;&nbsp;";
									}
									retVal += "'" + sId + "'&nbsp;&nbsp;";
								}
								retVal += "</font>";
							}
							else
								retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" + ibr.Value + "'</font>";
						}
					}
					break;
				case IncidentBoxRuleType.Function:
					retVal += "<b>" + GetRuleType(ibr.RuleType) + "</b>";
					IncidentBoxRuleFunction brf = IncidentBoxRuleFunction.Load(int.Parse(ibr.Key));
					retVal += "&nbsp;&nbsp;<font color='#0000ff'>" + brf.Name + "(&nbsp;</font>";
					retVal += "<font color='#ff0000'>" + ibr.Value + "</font><font color='#0000ff'>&nbsp;)</font>";
					break;
				default:
					break;
			}
			return retVal;
		}

		private string GetRuleType(IncidentBoxRuleType _type)
		{
			string retVal = "";
			switch (_type)
			{
				case IncidentBoxRuleType.IsEqual:
					retVal = LocRM.GetString("tIsEqual");
					break;
				case IncidentBoxRuleType.Contains:
					retVal = LocRM.GetString("tContains");
					break;
				case IncidentBoxRuleType.NotContains:
					retVal = LocRM.GetString("tNotContains");
					break;
				case IncidentBoxRuleType.NotIsEqual:
					retVal = LocRM.GetString("tIsNotEqual");
					break;
				case IncidentBoxRuleType.RegexMatch:
					retVal = LocRM.GetString("tRegExMatch");
					break;
				case IncidentBoxRuleType.Function:
					retVal = LocRM.GetString("Function");
					break;
				default:
					retVal = _type.ToString();
					break;
			}
			return retVal;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tIssBoxRules");
			secHeader.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
					this.Page.ResolveUrl("~/Layouts/Images/edit.gif"), LocRM.GetString("tEdit")),
				String.Format("javascript:{{window.opener.location.href='{0}?IssBoxId={1}';window.close();}}", this.Page.ResolveUrl("~/Admin/EMailIssueBoxRules.aspx"), IssBoxId));
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tClose"), "javascript:window.close();");
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
