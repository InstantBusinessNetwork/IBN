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
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class IssuePrintPreview : System.Web.UI.UserControl
	{
		UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private const string prefix = "IncidentView_Print_";

		protected void Page_Load(object sender, EventArgs e)
		{
			BindTable();

			reportHeader.Title = GetGlobalResourceObject("IbnFramework.Incident", "IssueViewReport").ToString();
			reportHeader.ReportCreated = UserDateTime.UserNow;
			reportHeader.ReportCreator = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(Mediachase.IBN.Business.Security.CurrentUser.UserID);
			if (Request.Browser.Browser.IndexOf("IE") < 0)
				reportHeader.BtnPrintVisible = false;
		}

		private void BindTable()
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell tc1 = new HtmlTableCell();
			HtmlTableCell tc2 = new HtmlTableCell();
			#region Variables
			string title = String.Empty;
			string description = String.Empty;
			string code = String.Empty;
			string issue_box = String.Empty;
			string priority = String.Empty;
			string state = String.Empty;
			string responsible = String.Empty;
			string manager = String.Empty;
			string client = String.Empty;
			string opendate = String.Empty;
			string created = String.Empty;
			string modified = String.Empty;
			string expecteddate = String.Empty;
			string resolvedate = String.Empty;
			string categories = String.Empty;
			string isscategories = String.Empty; 
			#endregion

			#region BindValues
			using (IDataReader reader = Incident.GetIncident(int.Parse(Request["IncidentId"])))
			{
				if (reader.Read())
				{
					title = reader["Title"].ToString();
					description = reader["Description"].ToString();

					code = "#" + reader["IncidentId"].ToString() + "&nbsp;";
					string sIdentifier = "";
					if (reader["Identifier"] != DBNull.Value)
						sIdentifier = reader["Identifier"].ToString();
					int ManagerId = 0;
					if (reader["IncidentBoxId"] != DBNull.Value)
					{
						IncidentBox box = IncidentBox.Load((int)reader["IncidentBoxId"]);
						code += ((sIdentifier == "") ? TicketUidUtil.Create(box.IdentifierMask, (int)reader["IncidentId"]) : sIdentifier);
						issue_box = box.Name;
						ManagerId = box.Document.GeneralBlock.Manager;
						manager = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(ManagerId);
					}
					priority = reader["PriorityName"].ToString();
					state = reader["StateName"].ToString();

					#region Responsible
					int StateId = (int)reader["StateId"];
					if (StateId == (int)ObjectStates.Upcoming ||
						StateId == (int)ObjectStates.Suspended ||
						StateId == (int)ObjectStates.Completed)
					{
						if (ManagerId > 0)
							responsible = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(ManagerId);
					}
					if (StateId == (int)ObjectStates.OnCheck)
					{
						if (reader["ControllerId"] != DBNull.Value)
							responsible = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName((int)reader["ControllerId"]);
					}
					if (reader["ResponsibleId"] != DBNull.Value && (int)reader["ResponsibleId"] > 0)
						responsible = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName((int)reader["ResponsibleId"]);
					else if (reader["IsResponsibleGroup"] != DBNull.Value && (bool)reader["IsResponsibleGroup"])
						responsible = CHelper.GetResFileString("{IbnFramework.Incident:tRespGroup}");
					else
						responsible = CHelper.GetResFileString("{IbnFramework.Incident:tRespNotSet}");
					#endregion

					client = reader["ClientName"].ToString();
					if (reader["ActualOpenDate"] != DBNull.Value)
						opendate = ((DateTime)reader["ActualOpenDate"]).ToShortDateString() + " " + ((DateTime)reader["ActualOpenDate"]).ToShortTimeString();
					created = string.Format("{0} {1} {2}",
						Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName((int)reader["CreatorId"]),
						((DateTime)reader["CreationDate"]).ToShortDateString(),
						((DateTime)reader["CreationDate"]).ToShortTimeString());
					modified = string.Format("{0} {1}",
						((DateTime)reader["ModifiedDate"]).ToShortDateString(),
						((DateTime)reader["ModifiedDate"]).ToShortTimeString());
					if (reader["ExpectedResponseDate"] != DBNull.Value)
						expecteddate = ((DateTime)reader["ExpectedResponseDate"]).ToShortDateString() + " " + ((DateTime)reader["ExpectedResponseDate"]).ToShortTimeString();
					if (reader["ExpectedResolveDate"] != DBNull.Value)
						resolvedate = ((DateTime)reader["ExpectedResolveDate"]).ToShortDateString() + " " + ((DateTime)reader["ExpectedResolveDate"]).ToShortTimeString();
				}
			}

			#region categories
			using (IDataReader reader = Incident.GetListCategories(int.Parse(Request["IncidentId"])))
			{
				while (reader.Read())
				{
					if (categories.Length > 0)
						categories += ", ";
					categories += reader["CategoryName"].ToString();
				}
			}
			#endregion

			#region isscategories
			using (IDataReader reader = Incident.GetListIncidentCategoriesByIncident(int.Parse(Request["IncidentId"])))
			{
				while (reader.Read())
				{
					if (isscategories.Length > 0)
						isscategories += ", ";
					isscategories += reader["CategoryName"].ToString();
				}
			}
			#endregion 
			#endregion


			#region showTitle
			if (_pc[prefix + "showTitle"] == null || (_pc[prefix + "showTitle"] != null && bool.Parse(_pc[prefix + "showTitle"])))
			{
				tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.VAlign = "top";
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showTitle").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.VAlign = "top";
				tc2.ColSpan = 3;
				tc2.InnerHtml = String.Format("<b>{0}</b>", title);
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				mainTable.Rows.Add(tr);
			} 
			#endregion
			#region showDescription
			if (_pc[prefix + "showDescription"] != null && bool.Parse(_pc[prefix + "showDescription"]))
			{
				tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.VAlign = "top";
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showDescription").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.VAlign = "top";
				tc2.ColSpan = 3;
				tc2.InnerHtml = String.Format("<i>{0}</i>", description);
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				mainTable.Rows.Add(tr);
			} 
			#endregion

			int countFields = 0;
			#region showCode
			if (_pc[prefix + "showCode"] != null && bool.Parse(_pc[prefix + "showCode"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showCode").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = code;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showIssBox
			if (_pc[prefix + "showIssBox"] != null && bool.Parse(_pc[prefix + "showIssBox"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showIssBox").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = issue_box;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showPriority
			if (_pc[prefix + "showPriority"] == null || (_pc[prefix + "showPriority"] != null && bool.Parse(_pc[prefix + "showPriority"])))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showPriority").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = priority;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showStatus
			if (_pc[prefix + "showStatus"] == null || (_pc[prefix + "showStatus"] != null && bool.Parse(_pc[prefix + "showStatus"])))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showStatus").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = state;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showResponsible
			if (_pc[prefix + "showResponsible"] == null || (_pc[prefix + "showResponsible"] != null && bool.Parse(_pc[prefix + "showResponsible"])))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showResponsible").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = responsible;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showManager
			if (_pc[prefix + "showManager"] != null && bool.Parse(_pc[prefix + "showManager"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showManager").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = manager;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showGenCats
			if (_pc[prefix + "showGenCats"] != null && bool.Parse(_pc[prefix + "showGenCats"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.VAlign = "top";
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showGenCats").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.VAlign = "top";
				tc2.InnerHtml = categories;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showIssCats
			if (_pc[prefix + "showIssCats"] != null && bool.Parse(_pc[prefix + "showIssCats"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.VAlign = "top";
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showIssCats").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.VAlign = "top";
				tc2.InnerHtml = isscategories;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showClient
			if (_pc[prefix + "showClient"] == null || (_pc[prefix + "showClient"] != null && bool.Parse(_pc[prefix + "showClient"])))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showClient").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = client;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showOpenDate
			if (_pc[prefix + "showOpenDate"] != null && bool.Parse(_pc[prefix + "showOpenDate"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showOpenDate").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = opendate;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showCreationInfo
			if (_pc[prefix + "showCreationInfo"] != null && bool.Parse(_pc[prefix + "showCreationInfo"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showCreationInfo").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = created;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showLastModifiedInfo
			if (_pc[prefix + "showLastModifiedInfo"] != null && bool.Parse(_pc[prefix + "showLastModifiedInfo"]))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showLastModifiedInfo").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = modified;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showExpectedDate
			if (_pc[prefix + "showExpectedDate"] == null || (_pc[prefix + "showExpectedDate"] != null && bool.Parse(_pc[prefix + "showExpectedDate"])))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showExpectedDate").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = expecteddate;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion
			#region showResolveDate
			if (_pc[prefix + "showResolveDate"] == null || (_pc[prefix + "showResolveDate"] != null && bool.Parse(_pc[prefix + "showResolveDate"])))
			{
				if (countFields == 0)
					tr = new HtmlTableRow();
				tc1 = new HtmlTableCell();
				tc1.InnerHtml = GetGlobalResourceObject("IbnFramework.Incident", "showResolveDate").ToString() + ":";
				tc2 = new HtmlTableCell();
				tc2.InnerHtml = resolvedate;
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				countFields++;
				if (countFields == 2)
				{
					mainTable.Rows.Add(tr);
					countFields = 0;
				}
			} 
			#endregion

			#region showForum
			if (_pc[prefix + "showForum"] == null || (_pc[prefix + "showForum"] != null && _pc[prefix + "showForum"] != "-1"))
			{
				DataTable dt = new DataTable();
				DataRow dr;
				dt.Columns.Add(new DataColumn("Index", typeof(string)));
				dt.Columns.Add(new DataColumn("Sender", typeof(string)));
				dt.Columns.Add(new DataColumn("Message", typeof(string)));
				dt.Columns.Add(new DataColumn("Created", typeof(string)));
				dt.Columns.Add(new DataColumn("CreationDate", typeof(DateTime)));
				int index_mess = 0;
				#region DataTable
				foreach (ForumThreadNodeInfo node in Incident.GetForumThreadNodes(int.Parse(Request["IncidentId"])))
				{
					dr = dt.NewRow();
					dr["Created"] = node.Created.ToShortDateString() + " " + node.Created.ToShortTimeString();
					dr["CreationDate"] = node.Created;

					// EmailMessage
					if (node.EMailMessageId > 0)
					{
						EMailMessageInfo mi = null;
						try
						{
							mi = EMailMessageInfo.Load(node.EMailMessageId);
							int iUserId = User.GetUserByEmail(mi.SenderEmail);

							if (EMailClient.IsAlertSenderEmail(mi.SenderEmail))
								iUserId = node.CreatorId;

							if (iUserId > 0)
								dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(iUserId);
							else
							{
								Client clientObj = Mediachase.IBN.Business.Common.GetClient(mi.SenderEmail);
								if (clientObj != null)
								{
									if (clientObj.IsContact)
										dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetContactLink(this.Page, clientObj.Id, clientObj.Name);
									else
										dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetOrganizationLink(this.Page, clientObj.Id, clientObj.Name);
								}
								else if (mi.SenderName != "")
									dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetEmailLink(mi.SenderEmail, mi.SenderName);
								else
									dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetEmailLink(mi.SenderEmail, mi.SenderEmail);
							}

							string sBody = "";
							if (mi.HtmlBody != null)
								sBody = mi.HtmlBody;

							dr["Message"] = sBody;
						}
						catch
						{
							dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(-1);
							dr["Message"] = String.Format("<font color='red'><b>{0}</b>&nbsp;(#{1})</font>", "Not Found", node.EMailMessageId);
						}
					}
					else
					{
						string sBody = Mediachase.UI.Web.Util.CommonHelper.parsetext_br(node.Text, false);
						dr["Message"] = sBody;
						dr["Sender"] = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(node.CreatorId);
					}

					dr["Index"] = "<table cellspacing='0' cellpadding='4' border='0' width='100%'><tr><td class='text' align='center' style='BACKGROUND-POSITION: center center; BACKGROUND-IMAGE: url(" + ResolveClientUrl("~/layouts/images/atrisk1.gif") + "); BACKGROUND-REPEAT: no-repeat;padding:4px;'><b>" + (++index_mess).ToString() + "</b></td></tr></table>";

					if (!String.IsNullOrEmpty(dr["Message"].ToString().Trim()))
						dt.Rows.Add(dr);
				} 
				#endregion
				int showForum = 1;
				if (_pc[prefix + "showForum"] != null)
					showForum = int.Parse(_pc[prefix + "showForum"]);

				DataView dv = dt.DefaultView;
				DataTable dtClone = dt.Clone();
				if (showForum > 0)
					dv.Sort = "CreationDate DESC";
				else
					dv.Sort = "CreationDate";

				int index = 0;
				foreach (DataRowView drv in dv)
				{
					dr = dtClone.NewRow();
					dr.ItemArray = drv.Row.ItemArray;
					dtClone.Rows.Add(dr);
					index++;
					if (showForum > 0 && index >= showForum)
						break;
				}
				dgForum.DataSource = dtClone.DefaultView;
				dgForum.DataBind();
			} 
			#endregion
		}
	}
}