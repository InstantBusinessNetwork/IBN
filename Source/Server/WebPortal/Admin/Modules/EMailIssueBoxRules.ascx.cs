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
	///		Summary description for EMailIssueBoxRules.
	/// </summary>
	public partial class EMailIssueBoxRules : System.Web.UI.UserControl
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
			{
				BindList();
				BindValues();
			}
			BindToolbars();
		}

		#region BindList
		private void BindList()
		{
			ddIssBox.DataSource = IncidentBox.List();
			ddIssBox.DataTextField = "Name";
			ddIssBox.DataValueField = "IncidentBoxId";
			ddIssBox.DataBind();
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			IncidentBox issb = IncidentBox.Load(IssBoxId);
			Util.CommonHelper.SafeSelect(ddIssBox, issb.IncidentBoxId.ToString());

			IncidentBoxRule[] ibList = IncidentBoxRule.List(IssBoxId);

			dgRules.Columns[2].HeaderText = LocRM.GetString("tIssRuleInfo");

			dgRules.DataSource = ibList;
			dgRules.DataBind();

			foreach (DataGridItem dgi in dgRules.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning3") + "')");
					ib.Attributes.Add("title", LocRM.GetString("tDelete"));
				}

				ImageButton ib1 = (ImageButton)dgi.FindControl("ibNew");
				if (ib1 != null)
				{
					ib1.Attributes.Add("onclick", "javascript:{OpenWindow(\"EMailIssueRuleEdit.aspx?New=1&IssRuleId=" + ib1.CommandArgument + "&IssBoxId=" + IssBoxId + "\", 400, 300, false);return false;}");
					ib1.Attributes.Add("title", LocRM.GetString("tAddRule"));
				}
			}
		}
		#endregion

		#region GetLabel
		protected string GetLabel(object _type, object _key, object _value, object _index, object _level)
		{
			string retVal = "";
			int iLevel = Util.CommonHelper.CountOf(_level.ToString(), ".") - 1;
			if (iLevel > 0)
			{
				iLevel = iLevel * 40;
				retVal += String.Format("<img alt='' src='{1}' width='{0}px' height='1px' border='0' />",
					iLevel, this.Page.ResolveUrl("~/layouts/images/blank.gif"));
			}

			string sAnd_OR = "&nbsp;";
			if (_level.ToString() == ".")
			{
				if ((int)_index > 0)
					sAnd_OR = LocRM.GetString("tAND");
			}
			else
			{
				string sLevel = _level.ToString();
				if (sLevel.EndsWith("."))
					sLevel = sLevel.Substring(0, sLevel.Length - 1);
				sLevel = sLevel.Substring(sLevel.LastIndexOf(".") + 1);
				int iNum = int.Parse(sLevel);
				IncidentBoxRule ibr = IncidentBoxRule.Load(iNum);
				if (ibr.RuleType == IncidentBoxRuleType.OrBlock && (int)_index > ibr.OutlineIndex + 1)
					sAnd_OR = LocRM.GetString("tOR");
				if (ibr.RuleType == IncidentBoxRuleType.AndBlock && (int)_index > ibr.OutlineIndex + 1)
					sAnd_OR = LocRM.GetString("tAND");
			}
			retVal += "<span style='width:40px;'><font color='green'><b>" + sAnd_OR + "</b></font>&nbsp;</span>";
			switch ((IncidentBoxRuleType)_type)
			{
				case IncidentBoxRuleType.Contains:
				case IncidentBoxRuleType.IsEqual:
				case IncidentBoxRuleType.RegexMatch:
				case IncidentBoxRuleType.NotContains:
				case IncidentBoxRuleType.NotIsEqual:
					string sKey = _key.ToString();
					sKey = (sKey == "TypeId") ? "Type" : sKey;
					sKey = (sKey == "PriorityId") ? "Priority" : sKey;
					sKey = (sKey == "SeverityId") ? "Severity" : sKey;
					sKey = (sKey == "TypeId") ? "Type" : sKey;
					sKey = (sKey == "CreatorId") ? "Creator" : sKey;
					sKey = (sKey == "ProjectId") ? "Project" : sKey;
					sKey = (sKey == "EMailBox") ? "E-mail Box" : sKey;
					sKey = (sKey == "TitleOrDescriptionOrEMailBody") ? "Title or Description or EMailBody" : sKey;
					retVal += "<font color='#0000ff'>[" + sKey + "]</font>&nbsp;&nbsp;";
					retVal += "<b>" + GetRuleType((IncidentBoxRuleType)_type) + "</b>";
					if (_value.ToString().Length > 0)
					{
						if (sKey == "Type")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.Common.GetIncidentType(int.Parse(_value.ToString())) + "'</font>";
						else if (sKey == "E-mail Box")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.EMail.EMailRouterPop3Box.Load(int.Parse(_value.ToString())).Name + "'</font>";
						else if (sKey == "Creator")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Util.CommonHelper.GetUserStatusPureName(int.Parse(_value.ToString())) + "'</font>";
						else if (sKey == "Project")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Task.GetProjectTitle(int.Parse(_value.ToString())) + "'</font>";
						else if (sKey == "Priority")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.Common.GetPriority(int.Parse(_value.ToString())) + "'</font>";
						else if (sKey == "Severity")
							retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" +
								Mediachase.IBN.Business.Common.GetIncidentSeverity(int.Parse(_value.ToString())) + "'</font>";
						else if (sKey == "GeneralCategories" && _value.ToString().Length > 0)
						{
							retVal += "&nbsp;&nbsp;<font color='red'>";
							string[] _values = _value.ToString().Split(';');
							for (int i = 0; i < _values.Length; i++)
							{
								string sId = _values[i];
								if (i > 0)
								{
									if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.Contains ||
										(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotContains)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tAND") + "&nbsp;&nbsp;";
									if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.IsEqual ||
										(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotIsEqual)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tOR") + "&nbsp;&nbsp;";
								}
								retVal += "'" +
									Mediachase.IBN.Business.Common.GetGeneralCategory(int.Parse(sId))
									+ "'&nbsp;&nbsp;";
							}
							retVal += "</font>";
						}
						else if (sKey == "IncidentCategories" && _value.ToString().Length > 0)
						{
							retVal += "&nbsp;&nbsp;<font color='red'>";
							string[] _values = _value.ToString().Split(';');
							for (int i = 0; i < _values.Length; i++)
							{
								string sId = _values[i];
								if (i > 0)
								{
									if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.Contains ||
										(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotContains)
										retVal += "&nbsp;&nbsp;" + LocRM.GetString("tAND") + "&nbsp;&nbsp;";
									if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.IsEqual ||
										(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotIsEqual)
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
							if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.Contains ||
								(IncidentBoxRuleType)_type == IncidentBoxRuleType.IsEqual ||
								(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotContains ||
								(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotIsEqual)
							{
								retVal += "&nbsp;&nbsp;<font color='red'>";
								string[] _values = _value.ToString().Split(';');
								for (int i = 0; i < _values.Length; i++)
								{
									string sId = _values[i];
									if (i > 0)
									{
										if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.Contains ||
											(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotContains)
											retVal += "&nbsp;&nbsp;" + LocRM.GetString("tAND") + "&nbsp;&nbsp;";
										if ((IncidentBoxRuleType)_type == IncidentBoxRuleType.IsEqual ||
											(IncidentBoxRuleType)_type == IncidentBoxRuleType.NotIsEqual)
											retVal += "&nbsp;&nbsp;" + LocRM.GetString("tOR") + "&nbsp;&nbsp;";
									}
									retVal += "'" + sId + "'&nbsp;&nbsp;";
								}
								retVal += "</font>";
							}
							else
								retVal += "&nbsp;&nbsp;<font color='#ff0000'>'" + _value.ToString() + "'</font>";
						}
					}
					break;
				case IncidentBoxRuleType.AndBlock:
				case IncidentBoxRuleType.OrBlock:
					retVal += "<b>" + GetRuleType((IncidentBoxRuleType)_type) + "</b>";
					break;
				case IncidentBoxRuleType.Function:
					retVal += "<b>" + GetRuleType((IncidentBoxRuleType)_type) + "</b>";
					IncidentBoxRuleFunction brf = IncidentBoxRuleFunction.Load(int.Parse(_key.ToString()));
					retVal += "&nbsp;&nbsp;<font color='#0000ff'>" + brf.Name + "(&nbsp;</font>";
					retVal += "<font color='#ff0000'>" + _value.ToString() + "</font><font color='#0000ff'>&nbsp;)</font>";
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
				case IncidentBoxRuleType.OrBlock:
					retVal = LocRM.GetString("tORBlock");
					break;
				case IncidentBoxRuleType.AndBlock:
					retVal = LocRM.GetString("tANDBlock");
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

		#region GetNumber
		protected string GetNumber(object _index, object _level)
		{
			string retVal = "";
			string sLevel = _level.ToString();
			if (sLevel == ".")
				sLevel = "";
			if (sLevel.StartsWith("."))
				sLevel = sLevel.Substring(1);
			sLevel += _index.ToString();
			retVal = sLevel;
			return retVal;
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tIssBoxRules");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/newitem.gif") + "'/> " + LocRM.GetString("tNewRule"), "javascript:OpenWindow('EMailIssueRuleEdit.aspx?IssBoxId=" + IssBoxId.ToString() + "', 400, 300, false)");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tIssRules"), this.Page.ResolveUrl("~/Admin/EMailRules.aspx"));
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
			this.dgRules.DeleteCommand += new DataGridCommandEventHandler(dg_DeleteCommand);
		}
		#endregion

		#region Delete
		private void dg_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			IncidentBoxRule.Delete(sid);
			Response.Redirect("~/Admin/EMailIssueBoxRules.aspx?IssBoxId=" + IssBoxId);
		}
		#endregion

	}
}
