using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
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
//using Mediachase.MetaDataPlus.Configurator;

namespace Mediachase.UI.Web.Admin
{
	/// <summary>
	/// Summary description for EMailIssueRuleEdit.
	/// </summary>
	public partial class EMailIssueRuleEdit : System.Web.UI.Page
	{
		#region HTML Vars
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected string sTitle = "";

		#region IssRuleId
		private int IssRuleId
		{
			get
			{
				if (Request["IssRuleId"] != null)
					return int.Parse(Request["IssRuleId"]);
				else
					return -1;
			}
		}
		#endregion

		#region IssBoxId
		private int IssBoxId
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

		#region IsNewIn
		private bool IsNewIn
		{
			get
			{
				if (Request["New"] != null && int.Parse(Request["New"]) == 1)
					return true;
				else
					return false;
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
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			sTitle = (IssRuleId > 0 && !IsNewIn) ? LocRM.GetString("tRuleEdit") : LocRM.GetString("tRuleNew");
			if (!Page.IsPostBack)
			{
				lblValueError.Text = "*";
				BindLists();
				BindTypes();
				BindKeys();
				BindValues();
			}

			imbSave.Text = LocRM.GetString("tSave");
			imbCancel.Text = LocRM.GetString("tCancel");
			lblValueError.Style.Add("display", "none");

			imbSave.CustomImage = this.ResolveUrl("~/Layouts/Images/saveitem.gif");
			imbCancel.CustomImage = this.ResolveUrl("~/Layouts/Images/cancel.gif");
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			txtValue.Style.Add("display", "");
			ddEmailBoxes.Style.Add("display", "none");
			ddCreator.Style.Add("display", "none");
			ddProject.Style.Add("display", "none");
			ddType.Style.Add("display", "none");
			ddPriority.Style.Add("display", "none");
			ddSeverity.Style.Add("display", "none");
			lbGenCats.Style.Add("display", "none");
			lbIssCats.Style.Add("display", "none");
			if (ddKey.SelectedValue == "TypeId")
			{
				txtValue.Style.Add("display", "none");
				ddType.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "EMailBox")
			{
				txtValue.Style.Add("display", "none");
				ddEmailBoxes.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "CreatorId")
			{
				txtValue.Style.Add("display", "none");
				ddCreator.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "ProjectId")
			{
				txtValue.Style.Add("display", "none");
				ddProject.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "PriorityId")
			{
				txtValue.Style.Add("display", "none");
				ddPriority.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "SeverityId")
			{
				txtValue.Style.Add("display", "none");
				ddSeverity.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "GeneralCategories")
			{
				txtValue.Style.Add("display", "none");
				lbGenCats.Style.Add("display", "");
			}
			else if (ddKey.SelectedValue == "IncidentCategories")
			{
				txtValue.Style.Add("display", "none");
				lbIssCats.Style.Add("display", "");
			}

			BindToolbar();
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			ddType.DataSource = Incident.GetListIncidentTypes();
			ddType.DataTextField = "TypeName";
			ddType.DataValueField = "TypeId";
			ddType.DataBind();

			ddPriority.DataSource = Incident.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();

			ddSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddSeverity.DataTextField = "SeverityName";
			ddSeverity.DataValueField = "SeverityId";
			ddSeverity.DataBind();

			lbGenCats.DataSource = Incident.GetListCategoriesAll();
			lbGenCats.DataTextField = "CategoryName";
			lbGenCats.DataValueField = "CategoryId";
			lbGenCats.DataBind();

			lbIssCats.DataSource = Incident.GetListIncidentCategories();
			lbIssCats.DataTextField = "CategoryName";
			lbIssCats.DataValueField = "CategoryId";
			lbIssCats.DataBind();

			ddEmailBoxes.DataSource = Mediachase.IBN.Business.EMail.EMailRouterPop3Box.ListExternal();
			ddEmailBoxes.DataTextField = "Name";
			ddEmailBoxes.DataValueField = "EMailRouterPop3BoxId";
			ddEmailBoxes.DataBind();

			ddCreator.DataSource = Mediachase.IBN.Business.User.GetListActive();
			ddCreator.DataValueField = "PrincipalId";
			ddCreator.DataTextField = "DisplayName";
			ddCreator.DataBind();

			ddProject.DataSource = Mediachase.IBN.Business.Project.GetListProjects();
			ddProject.DataValueField = "ProjectId";
			ddProject.DataTextField = "Title";
			ddProject.DataBind();
		}
		#endregion

		#region BindTypes
		private void BindTypes()
		{
			ddRuleType.Items.Clear();
			ddRuleType.Items.Add(new ListItem(LocRM.GetString("Operator"), "0"));	//0,1,2
			ddRuleType.Items.Add(new ListItem(LocRM.GetString("Block"), "4"));		//4,5
			ddRuleType.Items.Add(new ListItem(LocRM.GetString("Function"), "6"));	//6

			ddOperator.Items.Clear();
			ddOperator.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.Contains), ((int)IncidentBoxRuleType.Contains).ToString()));
			ddOperator.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.NotContains), ((int)IncidentBoxRuleType.NotContains).ToString()));
			ddOperator.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.RegexMatch), ((int)IncidentBoxRuleType.RegexMatch).ToString()));
			ddOperator.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.IsEqual), ((int)IncidentBoxRuleType.IsEqual).ToString()));
			ddOperator.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.NotIsEqual), ((int)IncidentBoxRuleType.NotIsEqual).ToString()));

			ddIndex.Items.Clear();
			if (!IsNewIn)
			{
				if (IssRuleId > 0)
				{
					IncidentBoxRule ibr = IncidentBoxRule.Load(IssRuleId);
					string sLevel = ibr.OutlineLevel;
					if (sLevel != ".")
					{
						if (sLevel.EndsWith("."))
							sLevel = sLevel.Substring(0, sLevel.Length - 1);
						sLevel = sLevel.Substring(sLevel.LastIndexOf(".") + 1);
						int iNum = int.Parse(sLevel);
						int[] intList = IncidentBoxRule.GetAvailableIndex(IssBoxId, iNum);
						for (int i = 0; i < intList.Length - 1; i++)
							ddIndex.Items.Add(new ListItem(intList[i].ToString(), intList[i].ToString()));
					}
					else
					{
						int[] intList = IncidentBoxRule.GetAvailableIndex(IssBoxId);
						for (int i = 0; i < intList.Length - 1; i++)
							ddIndex.Items.Add(new ListItem(intList[i].ToString(), intList[i].ToString()));
					}
				}
				else
				{
					int[] intList = IncidentBoxRule.GetAvailableIndex(IssBoxId);
					for (int i = 0; i < intList.Length; i++)
						ddIndex.Items.Add(new ListItem(intList[i].ToString(), intList[i].ToString()));
				}
			}
			else
			{
				int[] intList = IncidentBoxRule.GetAvailableIndex(IssBoxId, IssRuleId);
				for (int i = 0; i < intList.Length; i++)
					ddIndex.Items.Add(new ListItem(intList[i].ToString(), intList[i].ToString()));
			}
			if (IssRuleId < 0 || IsNewIn)
				ddIndex.SelectedIndex = ddIndex.Items.Count - 1;

			if (IssRuleId > 0 && !Page.IsPostBack && !IsNewIn)
			{
				IncidentBoxRule ibr = IncidentBoxRule.Load(IssRuleId);
				if ((int)ibr.RuleType < (int)IncidentBoxRuleType.Function ||
					(int)ibr.RuleType > (int)IncidentBoxRuleType.Function)
				{
					Util.CommonHelper.SafeSelect(ddRuleType, "0");
					Util.CommonHelper.SafeSelect(ddOperator, ((int)ibr.RuleType).ToString());
				}
				else if ((int)ibr.RuleType < (int)IncidentBoxRuleType.Function)
					Util.CommonHelper.SafeSelect(ddRuleType, "4");
				else
					Util.CommonHelper.SafeSelect(ddRuleType, "6");

				Util.CommonHelper.SafeSelect(ddIndex, ibr.OutlineIndex.ToString());
			}
		}
		#endregion

		#region BindKeys
		private void BindKeys()
		{
			ddKey.Items.Clear();
			txtValue.Text = "";
			dgParams.DataSource = null;
			dgParams.DataBind();
			lblNoFunction.Text = LocRM.GetString("tNoFunction");
			lblNoFunction.Visible = false;
			tblMain.Visible = true;

			switch (int.Parse(ddRuleType.SelectedValue))
			{
				case 0:		//Operator
					ddKey.Attributes.Add("onchange", "ChangeKey(this)");
					trValue.Visible = true;
					trOperator.Visible = true;

					ddKey.Items.Add(new ListItem("[MailSenderEmail]", "MailSenderEmail"));
					ddKey.Items.Add(new ListItem("[EMailBox]", "EMailBox"));
					ddKey.Items.Add(new ListItem("[EMailBody]", "EMailBody"));
					ddKey.Items.Add(new ListItem("[Title]", "Title"));
					ddKey.Items.Add(new ListItem("[Description]", "Description"));
					ddKey.Items.Add(new ListItem("[Title or Description or EMailBody]", "TitleOrDescriptionOrEMailBody"));
					ddKey.Items.Add(new ListItem("[Type]", "TypeId"));
					ddKey.Items.Add(new ListItem("[Priority]", "PriorityId"));
					ddKey.Items.Add(new ListItem("[Severity]", "SeverityId"));
					ddKey.Items.Add(new ListItem("[GeneralCategories]", "GeneralCategories"));
					ddKey.Items.Add(new ListItem("[IncidentCategories]", "IncidentCategories"));
					ddKey.Items.Add(new ListItem("[Creator]", "CreatorId"));
					if (Configuration.ProjectManagementEnabled)
						ddKey.Items.Add(new ListItem("[Project]", "ProjectId"));

					//					MetaClass mc = MetaClass.Load("IncidentsEx");
					//					foreach(MetaField mf in mc.UserMetaFields)
					//						ddKey.Items.Add(new ListItem("["+mf.FriendlyName+"]",mf.Name));

					//ddKey.Items.Add(new ListItem(LocRM.GetString("tTypeYour"),"0"));
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "ChangeKey2('" + ddKey.ClientID + "');", true);
					break;
				case 4:		//Block
					ddKey.Attributes.Remove("onchange");
					trValue.Visible = false;
					trOperator.Visible = false;
					ddKey.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.OrBlock), "4"));
					ddKey.Items.Add(new ListItem(GetRuleType(IncidentBoxRuleType.AndBlock), "5"));
					break;
				case 6:		//Function
					ddKey.Attributes.Remove("onchange");
					trValue.Visible = false;
					trOperator.Visible = false;

					IncidentBoxRuleFunction[] funcList = IncidentBoxRuleFunction.List();
					foreach (IncidentBoxRuleFunction fun in funcList)
						ddKey.Items.Add(new ListItem(fun.Name, fun.IncidentBoxRuleFunctionId.ToString()));

					if (ddKey.Items.Count == 0)
					{
						lblNoFunction.Visible = true;
						tblMain.Visible = false;
					}
					else
					{
						IncidentBoxRuleFunction brf = IncidentBoxRuleFunction.Load(int.Parse(ddKey.SelectedValue));
						System.Reflection.ParameterInfo[] paramsList = brf.GetParameters();
						DataTable dt = new DataTable();
						dt.Columns.Add(new DataColumn("Position", typeof(int)));
						dt.Columns.Add(new DataColumn("Name", typeof(string)));
						dt.Columns.Add(new DataColumn("DefaultValue", typeof(string)));
						DataRow dr;
						foreach (System.Reflection.ParameterInfo par in paramsList)
						{
							dr = dt.NewRow();
							dr["Position"] = par.Position;
							dr["Name"] = par.Name;
							dt.Rows.Add(dr);
						}
						dgParams.DataSource = dt.DefaultView;
						dgParams.DataBind();
					}
					break;
				default:
					break;
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if (IssRuleId > 0 && !IsNewIn)
			{
				IncidentBoxRule ibr = IncidentBoxRule.Load(IssRuleId);
				Util.CommonHelper.SafeSelect(ddKey, ibr.Key);
				if (ibr.Key == "TypeId")
					Util.CommonHelper.SafeSelect(ddType, ibr.Value);
				else if (ibr.Key == "EMailBox")
					Util.CommonHelper.SafeSelect(ddEmailBoxes, ibr.Value);
				else if (ibr.Key == "CreatorId")
					Util.CommonHelper.SafeSelect(ddCreator, ibr.Value);
				else if (ibr.Key == "ProjectId")
					Util.CommonHelper.SafeSelect(ddProject, ibr.Value);
				else if (ibr.Key == "PriorityId")
					Util.CommonHelper.SafeSelect(ddPriority, ibr.Value);
				else if (ibr.Key == "SeverityId")
					Util.CommonHelper.SafeSelect(ddSeverity, ibr.Value);
				else if (ibr.Key == "GeneralCategories")
				{
					string[] _values = ibr.Value.Split(';');
					foreach (string sId in _values)
						Util.CommonHelper.SafeMultipleSelect(lbGenCats, sId);
				}
				else if (ibr.Key == "IncidentCategories")
				{
					string[] _values = ibr.Value.Split(';');
					foreach (string sId in _values)
						Util.CommonHelper.SafeMultipleSelect(lbIssCats, sId);
				}
				else
					txtValue.Text = ibr.Value;

				if (ddRuleType.SelectedValue == "4")	//Block
					Util.CommonHelper.SafeSelect(ddKey, ((int)ibr.RuleType).ToString());
				if (ddRuleType.SelectedValue == "6")	//Function
				{
					Util.CommonHelper.SafeSelect(ddKey, ibr.Key);
					int funcId = int.Parse(ibr.Key);
					IncidentBoxRuleFunction brf = IncidentBoxRuleFunction.Load(funcId);

					ArrayList paramItems = new ArrayList();

					foreach (Match match in Regex.Matches(ibr.Value, "\"(?<Param>[^\"]+)\";?", RegexOptions.IgnoreCase | RegexOptions.Singleline))
					{
						string Value = match.Groups["Param"].Value;
						paramItems.Add(Value);
					}

					System.Reflection.ParameterInfo[] paramsList = brf.GetParameters();
					DataTable dt = new DataTable();
					dt.Columns.Add(new DataColumn("Position", typeof(int)));
					dt.Columns.Add(new DataColumn("Name", typeof(string)));
					dt.Columns.Add(new DataColumn("DefaultValue", typeof(string)));
					DataRow dr;
					foreach (System.Reflection.ParameterInfo par in paramsList)
					{
						dr = dt.NewRow();
						dr["Position"] = par.Position;
						dr["Name"] = par.Name;
						dr["DefaultValue"] = paramItems[par.Position];
						dt.Rows.Add(dr);
					}
					dgParams.DataSource = dt.DefaultView;
					dgParams.DataBind();
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = sTitle;
			secHeader.AddLink("<img alt='' src='" + ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tClose"), "javascript:window.close();");
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
			ddRuleType.SelectedIndexChanged += new EventHandler(ddRuleType_SelectedIndexChanged);
			this.imbSave.ServerClick += new EventHandler(imbSave_ServerClick);
		}
		#endregion

		private void ddRuleType_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindKeys();
		}

		#region Save
		private void imbSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			IncidentBoxRuleType _type = IncidentBoxRuleType.Contains;
			string _key = "";
			string _value = "";
			switch (ddRuleType.SelectedValue)
			{
				case "0":	//Operator
					_type = (IncidentBoxRuleType)(int.Parse(ddOperator.SelectedValue));
					_key = ddKey.SelectedValue;
					if (_key == "TypeId")
						_value = ddType.SelectedValue;
					else if (_key == "EMailBox")
						_value = ddEmailBoxes.SelectedValue;
					else if (_key == "CreatorId")
						_value = ddCreator.SelectedValue;
					else if (_key == "ProjectId")
						_value = ddProject.SelectedValue;
					else if (_key == "PriorityId")
						_value = ddPriority.SelectedValue;
					else if (_key == "SeverityId")
						_value = ddSeverity.SelectedValue;
					else if (_key == "GeneralCategories")
					{
						string sValue2 = "";
						foreach (ListItem li in lbGenCats.Items)
						{
							if (li.Selected)
								sValue2 += li.Value + ";";
						}
						if (sValue2.Length > 0)
							_value = sValue2.Substring(0, sValue2.Length - 1);
					}
					else if (_key == "IncidentCategories")
					{
						string sValue3 = "";
						foreach (ListItem li in lbIssCats.Items)
						{
							if (li.Selected)
								sValue3 += li.Value + ";";
						}
						if (sValue3.Length > 0)
							_value = sValue3.Substring(0, sValue3.Length - 1);
					}
					else
						_value = txtValue.Text;
					if (_value == "")
					{
						lblValueError.Style.Add("display", "");
						return;
					}
					break;
				case "4":	//Block
					_type = (IncidentBoxRuleType)(int.Parse(ddKey.SelectedValue));
					_key = "";
					_value = "";
					break;
				case "6":	//Function
					_type = IncidentBoxRuleType.Function;
					_key = ddKey.SelectedValue;
					string sValue1 = "";
					foreach (DataGridItem dgi in dgParams.Items)
					{
						TextBox tb = (TextBox)dgi.Cells[2].FindControl("tbValue");
						if (tb != null)
							sValue1 += "\"" + tb.Text + "\";";
					}
					if (sValue1.Length > 0)
						_value = sValue1.Substring(0, sValue1.Length - 1);
					break;
				default:
					break;
			}

			if (IssRuleId > 0 && !IsNewIn)
			{
				IncidentBoxRule ibr = IncidentBoxRule.Load(IssRuleId);
				ibr.RuleType = _type;
				ibr.Key = _key;
				ibr.Value = _value;
				IncidentBoxRule.Update(ibr, int.Parse(ddIndex.SelectedValue));
			}
			else if (!IsNewIn)
				IncidentBoxRule.Create(IssBoxId, int.Parse(ddIndex.SelectedValue), _type, _key, _value);
			else if (IsNewIn)
				IncidentBoxRule.CreateChild(IssBoxId, IssRuleId, int.Parse(ddIndex.SelectedValue),
					_type, _key, _value);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.opener.location.href=window.opener.location.href;}" +
					  "catch (e){} window.close();", true);
		}
		#endregion

		#region GetRuleType
		protected string GetRuleType(IncidentBoxRuleType _type)
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
				default:
					retVal = _type.ToString();
					break;
			}
			return retVal;
		}
		#endregion

	}
}
