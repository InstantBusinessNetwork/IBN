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
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.UI.Web.Util;
using System.Reflection;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class HDMSettings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		private string constString = "420, 420, false";
		private string constStringNew = "420, 460, false";
		private string constString2 = "640, 570, false";
		private string constStringStat = "400, 250, false";

		#region InternalBoxId
		public int InternalBoxId
		{
			get
			{
				EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
				if (pop3Int != null)
					return pop3Int.EMailRouterPop3BoxId;
				else
					return -1;
			}
		} 
		#endregion

		#region InternalBoxName
		public string InternalBoxName
		{
			get
			{
				EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
				if (pop3Int != null)
					return pop3Int.Name;
				else
					return string.Empty;
			}
		} 
		#endregion

		#region InternalBoxProblem
		public string InternalBoxProblem
		{
			get
			{
				EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
				if (pop3Int != null)
				{
					if (pop3Int.HasProblem())
						return pop3Int.Activity.ErrorText;
					else
						return string.Empty;
				}
				else
					return string.Empty;
			}
		} 
		#endregion

		#region DisableAntiSpam
		public bool DisableAntiSpam
		{
			get
			{
				if (Request["DisableAntiSpam"] != null)
					return true;
				else
					return false;
			}
		} 
		#endregion

		#region ErrorId
		private int ErrorId
		{
			get
			{
				if (Request["errorsid"] != null)
					return int.Parse(Request["errorsid"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			BindTitles();
			if (DisableAntiSpam)
			{
				PortalConfig.UseAntiSpamFilter = false;
			}
			if (!Page.IsPostBack)
			{
				if (PortalConfig.HasExternalEMailBox)
				{
					tbNoBoxes.Visible = false;
					tbExternal.Visible = true;
					BindExternal();
					if (PortalConfig.UseAntiSpamFilter)
					{
						tbNoAntiSpam.Visible = false;
						tbAntiSpam.Visible = true;
						bhAntiSpam.AddLink("<img align='absmiddle' border='0' src='" + ResolveClientUrl("~/layouts/images/newitem.gif") + "' />&nbsp;" + LocRM.GetString("tAddRule"),
						"javascript:OpenWindow('" + ResolveUrl("~/Admin/EMailAntiSpamEdit.aspx") + "', " + constString + ")");
						bhAntiSpam.AddLink(LocRM.GetString("tDisableAntiSpam"), ResolveUrl("~/Admin/HDMSettings.aspx?DisableAntiSpam=1"));
						BindDG();
					}
					else
					{
						tbNoAntiSpam.Visible = true;
						lbAntiSpam.Text = LocRM.GetString("tEnableAntiSpam");
						tbAntiSpam.Visible = false;
					}
				}
				else
				{
					tbNoBoxes.Visible = true;
					hlCreateNewEmailBox.Text = LocRM.GetString("tPop3BoxNew");
					hlCreateNewEmailBox.NavigateUrl = "javascript:OpenWindow('" + ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "', " + constStringNew + ")";
					tbExternal.Visible = false;
					tbAntiSpam.Visible = tbNoAntiSpam.Visible = false;
				}
				BindIssBoxes();
			}

			trSmtpNotChecked.Visible = false;
		}

		#region BindTitles
		protected void BindTitles()
		{
			secHeader1.Title = LocRM.GetString("tMailBoxes");
			secHeader1.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tHDM"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin5"));
			bhExternal.Title = LocRM.GetString("tMailBoxes");
			bhExternal.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tHDM"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin5"));
			bhlExtOnly.AddText(LocRM.GetString("tExternalBoxes"));
			bhlExtOnly.AddRightLink("<img align='absmiddle' border='0' src='" + ResolveClientUrl("~/layouts/images/newitem.gif") + "' />&nbsp;" + LocRM.GetString("tAddBox"),
					"javascript:OpenWindow('" + ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "', " + constStringNew + ")");
			bhlExtInt.AddText(LocRM.GetString("tExternalBoxes"));
			bhlExtInt.AddRightLink("<img align='absmiddle' border='0' src='" + ResolveClientUrl("~/layouts/images/newitem.gif") + "' />&nbsp;" + LocRM.GetString("tAddBox"),
					"javascript:OpenWindow('" + ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "', " + constStringNew + ")");
			bhlIntExt.AddText(LocRM.GetString("tInternalBox"));
			bhlNoInternal.AddText(LocRM.GetString("tInternalBox"));
			bhlNoSmtp.AddText("SMTP");
			hlNoInternal.Text = LocRM.GetString("tNewIntBox");
			string sLink = String.Format("javascript:OpenWindow('{1}', {0});",
					constString,
					ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "?IsInternal=1");
			hlNoInternal.NavigateUrl = sLink;// ResolveUrl("~/Admin/EmailPop3BoxEdit.aspx") + "?IsInternal=1";
			hlNoSmtp.Text = LocRM.GetString("tCheckSMTP");
			hlNoSmtp.NavigateUrl = ResolveUrl("~/Admin/SMTPSettings.aspx");
			EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
			if (pop3Int != null)
			{
				lbIntName.Text = InternalBoxName;
				lbIntIsActive.Text = (pop3Int.Activity.IsActive) ? LocRM.GetString("tYes") : LocRM.GetString("tNo");
				lbIntMessageCount.Text = pop3Int.Activity.TotalMessageCount.ToString();
				lbIntLastReq.Text = pop3Int.Activity.LastRequest.ToString("g");
				lbIntLastSuccReq.Text = pop3Int.Activity.LastSuccessfulRequest.ToString("g");
			}

			bhNoAntiSpam.Title = LocRM.GetString("tAntiSpamList");
			bhAntiSpam.Title = LocRM.GetString("tAntiSpamList");

			bhIssueBoxes.Title = LocRM.GetString("tIssBoxes");
			bhIssueBoxes.AddLink("<img border='0' width='16px' src='" + ResolveClientUrl("~/layouts/images/folder.gif") + "' height='16px' align='absmiddle'/> " + LocRM.GetString("tIssueBoxNew"), "javascript:OpenWindow('" + ResolveUrl("~/Admin/EmailIssueBoxEdit.aspx") + "'," + constString + ")");
			lbDeleteInternalBox.Text = GetDeleteButton();
			lbDeleteInternalBox.Attributes.Add("onclick", "javascript:return confirm('" + LocRM.GetString("tDelete") + "'+'?')");
			lbIntEdit.Text = GetInternalEditButton(InternalBoxId, LocRM.GetString("tEdit"));
			lbIntStat.Text = GetInternalBoxStatisticsButton(); //GetBoxStatisticsButton(InternalBoxId);
			lbChangeStatusInternal.Text = GetChangeInternalStatusButton();
			lbIntStat.Attributes.Add("onclick", "javascript:ShowStatMenu('divIntEmailBox', event)");
			lbIntBoxName.Text = InternalBoxName;


		} 
		#endregion

		#region External Email Boxes
		protected void BindExternal()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("EMailRouterPop3BoxId", typeof(int)));
			dt.Columns.Add(new DataColumn("IsActive", typeof(bool)));
			dt.Columns.Add(new DataColumn("LastRequest", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("LastSuccessfulRequest", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("LastErrorText", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("TotalMessageCount", typeof(int)));
			EMailRouterPop3Box[] listExt = EMailRouterPop3Box.ListExternal();
			DataRow dr;
			foreach (EMailRouterPop3Box ex in listExt)
			{
				dr = dt.NewRow();
				dr["EMailRouterPop3BoxId"] = ex.EMailRouterPop3BoxId;
				dr["Name"] = ex.Name;
				EMailRouterPop3BoxActivity act = ex.Activity;
				dr["IsActive"] = act.IsActive;
				dr["LastRequest"] = act.LastRequest;
				dr["LastSuccessfulRequest"] = act.LastSuccessfulRequest;
				if (ex.HasProblem())
					dr["LastErrorText"] = act.ErrorText;
				else
					dr["LastErrorText"] = string.Empty;
				dr["TotalMessageCount"] = act.TotalMessageCount;
				dt.Rows.Add(dr);
			}
			rpExtOnly.DataSource = rpExtInt.DataSource = dt;
			rpExtOnly.DataBind(); rpExtInt.DataBind();
			foreach (RepeaterItem ri in rpExtOnly.Items)
			{
				LinkButton lb = (LinkButton)ri.FindControl("ibDelete");
				if (lb != null)
				{
					lb.Attributes.Add("onclick", "javascript:return confirm('" + LocRM.GetString("tDelete") + "'+'?')");
				}
			}
			foreach (RepeaterItem ri in rpExtInt.Items)
			{
				LinkButton lb = (LinkButton)ri.FindControl("ibDelete");
				if (lb != null)
				{
					lb.Attributes.Add("onclick", "javascript:return confirm('" + LocRM.GetString("tDelete") + "'+'?')");
				}
			}

			if (PortalConfig.HasExternalEMailBox)
			{
				//if (PortalConfig.HasInternalEMailBox && PortalConfig.SmtpSettings.IsChecked)
				//{
				//    trExternalOnly.Visible = trNoInternal.Visible = trSmtpNotChecked.Visible = false;
				//    trExternalInternal.Visible = true;

				//}
				//if (PortalConfig.HasInternalEMailBox && !PortalConfig.SmtpSettings.IsChecked)
				//{
				//    trExternalOnly.Visible = trNoInternal.Visible = false;
				//    trExternalInternal.Visible = trSmtpNotChecked.Visible = true;
				//}
				//if (!PortalConfig.HasInternalEMailBox && PortalConfig.SmtpSettings.IsChecked)
				//{
				//    trExternalOnly.Visible = trNoInternal.Visible = true;
				//    trExternalInternal.Visible = trSmtpNotChecked.Visible = false;
				//}
				//if (!PortalConfig.HasInternalEMailBox && !PortalConfig.SmtpSettings.IsChecked)
				//{
				//    trExternalOnly.Visible = trNoInternal.Visible = trSmtpNotChecked.Visible = true;
				//    trExternalInternal.Visible = false;
				//}

				if (PortalConfig.HasInternalEMailBox)
				{
				    trExternalOnly.Visible = trNoInternal.Visible = false;
				    trExternalInternal.Visible = true;
				}
				if (!PortalConfig.HasInternalEMailBox)
				{
					trExternalOnly.Visible = trNoInternal.Visible = true;
					trExternalInternal.Visible = false;
				}
			}


		}

		protected string GetMailBoxIcon(int BoxId)
		{
			if (BoxId > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(BoxId);
				if (box != null)
				{
					string IconName = "mailbox_";
					if (box.Activity.IsActive)
					{
						IconName += "started";
					}
					else
					{
						IconName += "stopped";
					}
					if (box.HasProblem())
					{

						IconName += "_problem";
					}
					IconName += ".gif";
					string error = string.Empty;
					if (box.HasProblem())
						error = box.Activity.ErrorText;
					return "<img border='0' src='" + ResolveUrl("~/layouts/images/" + IconName) + "' title='" + error + "' />";
				}
			}
			return string.Empty;
		}

		protected string GetStatus(object IsActive, object LastRequest, object LastSuccessfulRequest, object LastErrorText)
		{
			string retval = "";
			if ((bool)IsActive)
			{
				if ((DateTime)LastRequest != (DateTime)LastSuccessfulRequest)
					retval = "<img border='0' width='16px' src='" + ResolveUrl("~/layouts/images/icons/status_problem.gif") + "' height='16px' align='absmiddle' title='" + (string)LastErrorText + "'/>";
				else
					retval = "<img border='0' width='16px' src='" + ResolveUrl("~/layouts/images/icons/status_active.gif") + "' height='16px' align='absmiddle'/>";
			}
			else
			{
				retval = "<img border='0' width='16px' src='" + ResolveUrl("~/layouts/images/icons/status_stopped.gif") + "' height='16px' align='absmiddle'/>";
			}
			return retval;
		}

		protected string GetStatusDG(object IsActive)
		{
			return String.Format("<img border='0' align='absmiddle' src='{0}' />&nbsp;{1}",
				((bool)IsActive) ?
					ResolveUrl("~/layouts/images/icons/status_stopped.gif") :
					ResolveUrl("~/layouts/images/icons/status_active.gif"),
				((bool)IsActive) ?
					LocRM.GetString("tChangeStatusStop") :
					LocRM.GetString("tChangeStatusRun"));
		}

		protected string GetDeleteButton()
		{
			return String.Format("<img border='0' align='absmiddle' src='{0}' />&nbsp;{1}", ResolveUrl("~/layouts/images/delete.gif"), LocRM.GetString("tDelete"));
		}

		protected string GetEditButton(int id, string Tooltip)
		{
			if (id > 0)
			{
				return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{4}</a>",
					ResolveUrl("~/layouts/images/edit.gif"),
					Tooltip, constString,
					ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "?BoxId=" + id.ToString(),
					LocRM.GetString("tEdit"));
			}
			else
				return string.Empty;
		}

		protected string GetInternalEditButton(int id, string Tooltip)
		{
			if (id > 0)
			{
				return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{4}</a>",
					ResolveUrl("~/layouts/images/edit.gif"),
					Tooltip, constString,
					ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "?BoxId=" + id.ToString() + "&IsInternal=1",
					LocRM.GetString("tEdit"));
			}
			else
				return string.Empty;
		}

		protected string GetChangeInternalStatusButton()
		{
			if (InternalBoxId > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(InternalBoxId);
				if (box != null)
				{
					return String.Format("<img border='0' align='absmiddle' src='{0}' />&nbsp;{1}",
					(box.Activity.IsActive) ?
					ResolveUrl("~/layouts/images/icons/status_stopped.gif") :
					ResolveUrl("~/layouts/images/icons/status_active.gif"),
					(box.Activity.IsActive) ?
					LocRM.GetString("tChangeStatusStop") :
					LocRM.GetString("tChangeStatusRun"));
				}
				else
					return string.Empty;
			}
			else
				return string.Empty;
		}

		protected string GetBoxStatisticsButton(int id)
		{
			if (id > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(id);
				if (box != null)
				{
					return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{4}</a>",
					ResolveUrl("~/layouts/images/info.gif"),
					LocRM.GetString("tStatistics"), constStringStat,
					ResolveUrl("~/Admin/EMailPop3BoxStatistics.aspx") + "?BoxId=" + id.ToString(),
					LocRM.GetString("tStatistics"));
				}
				return string.Empty;
			}
			else
			{
				return string.Empty;
			}
		}

		protected string GetInternalBoxStatisticsButton()
		{
			if (InternalBoxId > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(InternalBoxId);
				if (box != null)
				{

					string err = string.Empty;
					if (box.HasProblem())
						err = box.Activity.ErrorText;

					return "<a class='text' href='#' onclick='javascript:ShowStatMenu(\"divEmailBox_" + InternalBoxId.ToString() + "\", event)'><img border='0' align='absmiddle' src='" + ResolveUrl("~/layouts/images/info.gif") + "' title='" + err + "'  />&nbsp;" + LocRM.GetString("tStatistics") + "</a>";
				}
			}
			return string.Empty;
		}

		protected string GetMappingButton(int id, string Tooltip)
		{
			if (id == ErrorId)
				return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img style='border:2px solid #FF746B;' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{4}</a>",
					ResolveUrl("~/layouts/images/rules.gif"),
					Tooltip, constString2,
					ResolveUrl("~/Admin/EMailDefaultMapping.aspx") + "?BoxId=" + id.ToString(),
					LocRM.GetString("tMapping"));
			else
			{
				if (id > 0)
				{
					return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{4}</a>",
						ResolveUrl("~/layouts/images/rules.gif"),
						Tooltip, constString2,
						ResolveUrl("~/Admin/EMailDefaultMapping.aspx") + "?BoxId=" + id.ToString(),
						LocRM.GetString("tMapping"));
				}
				else
					return string.Empty;
			}
		}
		#endregion

		#region Antispam rules
		private void BindDG()
		{
			int i = 1;
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tWeight");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tAction");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tKey");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tType");
			dgRules.Columns[i++].HeaderText = LocRM.GetString("tValue");

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Weight", typeof(int)));
			dt.Columns.Add(new DataColumn("IsAccept", typeof(bool)));
			dt.Columns.Add(new DataColumn("Key", typeof(string)));
			dt.Columns.Add(new DataColumn("Type", typeof(string)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));
			DataRow dr;
			EMailMessageAntiSpamRule[] mas = EMailMessageAntiSpamRule.List();
			foreach (EMailMessageAntiSpamRule asp in mas)
			{
				dr = dt.NewRow();
				dr["Id"] = asp.AntiSpamRuleId;
				dr["Weight"] = asp.Weight;
				dr["IsAccept"] = asp.Accept;
				dr["Key"] = GetKey(asp.Key);
				switch (asp.RuleType)
				{
					case EMailMessageAntiSpamRuleType.Contains:
						dr["Type"] = LocRM.GetString("tContains");
						break;
					case EMailMessageAntiSpamRuleType.IsEqual:
						dr["Type"] = LocRM.GetString("tIsEqual");
						break;
					case EMailMessageAntiSpamRuleType.RegexMatch:
						dr["Type"] = LocRM.GetString("tRegExMatch");
						break;
					case EMailMessageAntiSpamRuleType.Service:
						dr["Type"] = LocRM.GetString("tService");
						break;
					default:
						dr["Type"] = "";
						break;
				}
				dr["Value"] = asp.Value;
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "Weight";

			dgRules.DataSource = dv;
			dgRules.DataBind();

			foreach (DataGridItem dgi in dgRules.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("title", LocRM.GetString("tDelete"));
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning3") + "')");
				}
			}
		}
		private string GetKey(string _key)
		{
			switch (_key)
			{
				case "BlackList":
					_key = LocRM.GetString("tBlackList2");
					break;
				case "WhiteList":
					_key = LocRM.GetString("tWhiteList2");
					break;
				case "Ticket":
					_key = LocRM.GetString("tTicket");
					break;
				case "IncidentBoxRules":
					_key = LocRM.GetString("tIssBoxRulesService");
					break;
				case "From":
					_key = "[From]";
					break;
				case "To":
					_key = "[To]";
					break;
				case "Subject":
					_key = "[Subject]";
					break;
				case "Body":
					_key = "[Body]";
					break;
				case "SubjectOrBody":
					_key = "[Subject or Body]";
					break;
				default:
					break;
			}
			return _key;
		}

		protected string GetIcon(bool IsAccept)
		{
			string retVal = "";
			if (IsAccept)
				retVal = String.Format("<img align='absmiddle' border='0' src='{0}' />",
					ResolveUrl("~/layouts/images/accept_green.gif"));
			else
				retVal = String.Format("<img align='absmiddle' border='0' src='{0}' />",
					ResolveUrl("~/layouts/images/deny.gif"));
			return retVal;
		}
		protected string GetLink_1(int Id, string sKey)
		{
			string retVal = "";
			retVal = String.Format("<a href=\"javascript:OpenWindow('{0}', " + constString + ")\">{1}</a>",
				ResolveUrl("~/Admin/EMailAntiSpamEdit.aspx") + "?RuleId=" + Id.ToString(),
				sKey);
			return retVal;
		}


		protected string GetEditButton_1(int id, string Tooltip)
		{
			return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/></a>",
				ResolveUrl("~/layouts/images/edit.gif"),
				Tooltip, constString,
				ResolveUrl("~/Admin/EMailAntiSpamEdit.aspx") + "?RuleId=" + id.ToString());
		}
		#endregion

		#region Issue Boxes
		private void BindIssBoxes()
		{
			IncidentBox[] ibList = IncidentBox.List();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("IncidentBoxId", typeof(int)));
			dt.Columns.Add(new DataColumn("IsDefault", typeof(bool)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("IdentifierMask", typeof(string)));
			dt.Columns.Add(new DataColumn("Routing", typeof(string)));
			dt.Columns.Add(new DataColumn("Rules", typeof(string)));
			DataRow dr;
			foreach (IncidentBox ib in ibList)
			{
				dr = dt.NewRow();
				dr["IncidentBoxId"] = ib.IncidentBoxId;
				dr["Name"] = ib.Name;
				dr["IsDefault"] = ib.IsDefault;
				dr["IdentifierMask"] = ib.IdentifierMask;
				dr["Routing"] = "&nbsp;";
				dr["Rules"] = "&nbsp;";
				IncidentBoxDocument ibd = IncidentBoxDocument.Load(ib.IncidentBoxId);
				if (ibd != null)
				{
					if (ibd.EMailRouterBlock.AllowEMailRouting)
					{
						dr["Routing"] = "<img border='0' align='top' src='" + ResolveUrl("~/layouts/images/incidentbox_routing.gif") + "' title='" + LocRM.GetString("tEmailRoutingEnabled") + "'/>";
					}
				}
				IncidentBoxRule[] ibr = IncidentBoxRule.List(ib.IncidentBoxId);
				if (ibr != null && ibr.Length > 0)
				{
					dr["Rules"] = "<img border='0' align='top' src='" + ResolveUrl("~/layouts/images/incidentbox_rules.gif") + "' title='" + LocRM.GetString("tRulesExist") + "'/>";
				}
				dt.Rows.Add(dr);
			}

			rpIssBoxes.DataSource = dt;
			rpIssBoxes.DataBind();
			foreach (RepeaterItem ri in rpIssBoxes.Items)
			{
				LinkButton lb = (LinkButton)ri.FindControl("ibDelete");
				if (lb != null)
				{
					lb.Attributes.Add("onclick", "javascript:return confirm('" + LocRM.GetString("tDelete") + "'+'?')");
				}
			}

		}

		protected string GetEditButton(int id)
		{
			if (id > 0)
			{
				return String.Format("<a href=\"{3}\"><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{4}</a>",
						ResolveUrl("~/layouts/images/edit.gif"),
						LocRM.GetString("tEdit"), "",
						ResolveUrl("~/Admin/EMailIssueBoxView.aspx") + "?IssBoxId=" + id.ToString(),
						LocRM.GetString("tEdit"));
			}
			else
				return string.Empty;
		}

		protected string GetRuleButton(int id)
		{
			IncidentBoxRule[] ibList = IncidentBoxRule.List(id);
			string Tooltip = LocRM.GetString("tIssBoxRules");
			string s1 = LocRM.GetString("tRules");
			if (ibList.Length > 0)
				return String.Format("<a href=\"javascript:OpenWindow('{2}', 640,480,false)\"><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{3}</a>",
					ResolveUrl("~/layouts/images/rules.gif"),
					Tooltip,
					String.Format("{0}?IssBoxId={1}",
					ResolveUrl("~/Admin/EMailIssueBoxRulesView.aspx"), id), s1);
			else
				return String.Format("<a href='{2}'><img border='0' align='absmiddle' src='{0}' title='{1}'/>&nbsp;{3}</a>",
					ResolveUrl("~/layouts/images/rulesnew.gif"),
					Tooltip,
					String.Format("{0}?IssBoxId={1}",
						ResolveUrl("~/Admin/EMailIssueBoxRules.aspx"), id), s1);
		}

		protected string GetIssBoxDeleteButton(int Id)
		{
			if (Id > 0)
			{
				if (IncidentBox.CanDelete(Id))
					return String.Format("<img border='0' align='absmiddle' src='{0}' />&nbsp;{1}", ResolveUrl("~/layouts/images/delete.gif"), LocRM.GetString("tDelete"));
				else
					return string.Empty;
			}
			else
				return string.Empty;
		}
		#endregion

		#region DataGrid_Events
		private void dgRules_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int Id = int.Parse(e.CommandArgument.ToString());
			EMailMessageAntiSpamRule.Delete(Id);
			Response.Redirect("~/Admin/HDMSettings.aspx");
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
			this.lbAntiSpam.Click += new EventHandler(lbAntiSpam_Click);
			this.dgRules.DeleteCommand += new DataGridCommandEventHandler(dgRules_DeleteCommand);
			this.rpExtOnly.ItemCommand += new RepeaterCommandEventHandler(rpExtOnly_ItemCommand);
			this.rpExtInt.ItemCommand += new RepeaterCommandEventHandler(rpExtInt_ItemCommand);
			this.rpIssBoxes.ItemCommand += new RepeaterCommandEventHandler(rpIssBoxes_ItemCommand);
			this.lbDeleteInternalBox.Click += new EventHandler(lbDeleteInternalBox_Click);
			this.lbChangeStatusInternal.Click += new EventHandler(lbChangeStatusInternal_Click);
		}

		void lbChangeStatusInternal_Click(object sender, EventArgs e)
		{
			if (InternalBoxId > 0)
			{
				EMailRouterPop3Box box = EMailRouterPop3Box.Load(InternalBoxId);
				if (box != null)
				{
					if (EMailRouterPop3Box.CanActivate(InternalBoxId))
					{
						EMailRouterPop3Box.Activate(InternalBoxId, !box.Activity.IsActive);
					}
				}
			}
			Response.Redirect("~/Admin/HDMSettings.aspx");
		}

		void lbDeleteInternalBox_Click(object sender, EventArgs e)
		{
			EMailRouterPop3Box.Delete(InternalBoxId);
			Response.Redirect("~/Admin/HDMSettings.aspx");
		}

		void rpExtInt_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			int bid = -1;
			bid = int.Parse(e.CommandArgument.ToString());
			if (e.CommandName == "ChangeStatus")
			{
				bid = int.Parse(e.CommandArgument.ToString());
				EMailRouterPop3Box ebox = EMailRouterPop3Box.Load(bid);
				if (EMailRouterPop3Box.CanActivate(bid))
				{
					EMailRouterPop3Box.Activate(bid, !ebox.Activity.IsActive);
					Response.Redirect("~/Admin/HDMSettings.aspx");
				}
				else
					Response.Redirect("~/Admin/HDMSettings.aspx?errorsid=" + bid);
			}
			if (e.CommandName == "Delete")
			{
				try
				{
					EMailRouterPop3Box.Delete(bid);
					Response.Redirect("~/Admin/HDMSettings.aspx");
				}
				catch
				{
					this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), Guid.NewGuid().ToString(),
						String.Format("alert('{0}');", LocRM.GetString("tCantDeleteMailBox")), true);
				}
			}
		}

		void rpExtOnly_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			int bid = -1;
			bid = int.Parse(e.CommandArgument.ToString());
			if (e.CommandName == "ChangeStatus")
			{
				bid = int.Parse(e.CommandArgument.ToString());
				EMailRouterPop3Box ebox = EMailRouterPop3Box.Load(bid);
				if (EMailRouterPop3Box.CanActivate(bid))
				{
					EMailRouterPop3Box.Activate(bid, !ebox.Activity.IsActive);
					Response.Redirect("~/Admin/HDMSettings.aspx");
				}
				else
					Response.Redirect("~/Admin/HDMSettings.aspx?errorsid=" + bid);
			}
			if (e.CommandName == "Delete")
			{
				EMailRouterPop3Box.Delete(bid);
				Response.Redirect("~/Admin/HDMSettings.aspx");
			}
		}

		void rpIssBoxes_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			int bid = -1;
			bid = int.Parse(e.CommandArgument.ToString());
			if (e.CommandName == "Delete")
			{
				try
				{
					IncidentBox.Delete(bid);
				}
				catch
				{ }
				Response.Redirect("~/Admin/HDMSettings.aspx");
			}

		}

		void lbAntiSpam_Click(object sender, EventArgs e)
		{
			PortalConfig.UseAntiSpamFilter = true;
			Response.Redirect("~/Admin/HDMSettings.aspx", true);
		}
		#endregion
	}
}