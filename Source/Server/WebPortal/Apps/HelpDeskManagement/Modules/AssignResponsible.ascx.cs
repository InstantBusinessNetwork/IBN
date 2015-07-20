using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class AssignResponsible : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", Assembly.GetExecutingAssembly());

		#region Command
		protected string Command
		{
			get
			{
				string retval = String.Empty;
				if (Request["commandName"] != null)
					retval = Request["commandName"];

				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/objectDD.css");

			divErrors.Visible = false;
			ApplyLocalization();
			if (!IsPostBack)
			{
				BindValues();
				ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
					"GetIds();", true);
			}

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_SelectResourceInFrame");
			cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_GroupResponsibilityInFrame");
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindList();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			btnSave.CustomImage = Page.ResolveUrl("~/Layouts/Images/saveitem.gif");
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
			btnCancel.CustomImage = Page.ResolveUrl("~/Layouts/Images/cancel.gif");

			btnClose.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Close").ToString();
			btnClose.CustomImage = Page.ResolveUrl("~/Layouts/Images/cancel.gif");
			btnClose.ServerClick += new EventHandler(btnClose_ServerClick);

			if (Request["closeFramePopup"] != null)
				btnCancel.Attributes.Add("onclick", String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{;}}return false;", Request["closeFramePopup"]));

			this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
			this.btnRefreshMore.Click += new EventHandler(btnRefreshMore_Click);
			this.btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ResourceId", typeof(int)));
			dt.Columns.Add(new DataColumn("IncidentId", typeof(int)));
			dt.Columns.Add(new DataColumn("PrincipalId", typeof(int)));
			dt.Columns.Add(new DataColumn("IsGroup", typeof(bool)));
			dt.Columns.Add(new DataColumn("MustBeConfirmed", typeof(bool)));
			dt.Columns.Add(new DataColumn("ResponsePending", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsConfirmed", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsExternal", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsNew", typeof(bool)));

			ViewState["ResponsiblePool"] = dt.Copy();

			CommandManager cm = CommandManager.GetCurrent(this.Page);

			lblClient.Text = "&nbsp;";
			hidResp.Value = "-3";

			lblChange.Text = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' class='btndown2' src='{0}'/>",
				Page.ResolveUrl("~/Layouts/Images/downbtn.gif"));

			tdChange.Attributes.Add("onclick", "javascript:ShowHideList(event, '" + tdChange.ClientID + "')");
			tdChange.Style.Add("cursor", "pointer");
		}
		#endregion

		private static HtmlTableCell CreateDDCell(string onclick)
		{
			HtmlTableCell tc = new HtmlTableCell();
			tc.Attributes.Add("class", "cellclass");
			tc.Attributes.Add("onmouseover", "TdOver(this)");
			tc.Attributes.Add("onmouseout", "TdOut(this)");
			tc.Attributes.Add("onclick", onclick);

			return tc;
		}

		#region BindList
		private void BindList()
		{
			DataTable dt = null;
			if (ViewState["ResponsiblePool"] != null)
				dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();
			string sUsers = "";
			if (dt != null)
				foreach (DataRow dr in dt.Rows)
				{
					if ((bool)dr["ResponsePending"])
						sUsers += dr["PrincipalId"].ToString() + "*1_";
					else
						sUsers += dr["PrincipalId"].ToString() + "*0_";
				}
			tableDD.Rows.Clear();
			HtmlTableRow tr = null;
			HtmlTableCell tc = null;

			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = null;
			string cmd = String.Empty;

			//NotChange
			tr = new HtmlTableRow();
			tc = CreateDDCell("SelectThis(this, -3)");
			tc.InnerHtml = "&nbsp;";
			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);

			//NotSet
			tr = new HtmlTableRow();
			tc = CreateDDCell("SelectThis(this, -2)");
			tc.InnerHtml = BuildIconAndText("not_set.png", "tRespNotSet");
			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);

			//Group Resp
			tr = new HtmlTableRow();
			tc = CreateDDCell("SelectThis(this, -1)");

			if (IsAllDenied())
			{
				if (!Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				{
					cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", sUsers);
					cp.AddCommandArgument("NotChangeKey", "0");
					cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					tc.InnerHtml = BuildLinkWithIconAndText("red_denied.gif", "tRespGroup", cmd);
				}
				else
					tc.InnerHtml = BuildIconAndText("red_denied.gif", "tRespGroup");
			}
			else
			{
				if (!Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				{
					cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", sUsers);
					cp.AddCommandArgument("NotChangeKey", "0");
					cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					tc.InnerHtml = BuildLinkWithIconAndText("waiting.gif", "tRespGroup", cmd);
				}
				else
					tc.InnerHtml = BuildIconAndText("waiting.gif", "tRespGroup");
			}

			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);

			//User
			tr = new HtmlTableRow();
			tc = CreateDDCell("SelectThis(this, " + Mediachase.IBN.Business.Security.CurrentUser.UserID + ")");
			tc.InnerHtml = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL(Mediachase.IBN.Business.Security.CurrentUser.UserID);
			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);

			//MORE
			cp = new CommandParameters("MC_HDM_SelectResourceInFrame");
			cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
			cmd = "closeMenu();" + cmd;

			tr = new HtmlTableRow();
			tc = CreateDDCell(cmd);
			tc.InnerHtml = String.Format("<b>{0}</b>", LocRM.GetString("tRespMore"));
			tr.Cells.Add(tc);
			tableDD.Rows.Add(tr);
		}
		#endregion

		#region btnRefreshMore_Click
		void btnRefreshMore_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			string[] values = arg.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length >= 2)
			{
				hidResp.Value = values[1];
				lblClient.Text = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL(int.Parse(values[1], CultureInfo.InvariantCulture));
			}
		}
		#endregion

		#region btnRefresh_Click
		void btnRefresh_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			string[] values = arg.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
			DataTable dt = null;
			ArrayList alUsers = new ArrayList();
			if (ViewState["ResponsiblePool"] != null)
				dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();
			try
			{
				for (int i = 0; i < values.Length; i++)
				{
					string sUser = values[i].Substring(0, values[i].IndexOf("*"));
					string temp = values[i].Substring(values[i].IndexOf("*") + 1);
					string sStatus = temp.Substring(0, temp.IndexOf("*"));
					string sIsNew = temp.Substring(temp.IndexOf("*") + 1);
					alUsers.Add(int.Parse(sUser, CultureInfo.InvariantCulture));
					DataRow[] dr_mas = dt.Select("PrincipalId=" + sUser);
					if (dr_mas.Length > 0)
					{
						dr_mas[0]["ResponsePending"] = (sStatus == "1");
						if (sIsNew == "1")
							dr_mas[0]["IsNew"] = true;
					}
					else
					{
						DataRow newRow = dt.NewRow();
						newRow["PrincipalId"] = int.Parse(sUser, CultureInfo.InvariantCulture);
						newRow["IsGroup"] = false;
						newRow["ResponsePending"] = (sStatus == "1");
						newRow["IsNew"] = true;
						dt.Rows.Add(newRow);
					}
				}
			}
			catch
			{
			}
			ArrayList alDeleted = new ArrayList();
			foreach (DataRow dr in dt.Rows)
				if (!alUsers.Contains((int)dr["PrincipalId"]))
					alDeleted.Add((int)dr["PrincipalId"]);
			foreach (int iDel in alDeleted)
			{
				DataRow[] dr_mas = dt.Select("PrincipalId = " + iDel);
				if (dr_mas.Length > 0)
					dt.Rows.Remove(dr_mas[0]);
			}

			ViewState["ResponsiblePool"] = dt.Copy();

			CommandManager cm = CommandManager.GetCurrent(this.Page);

			if (IsAllDenied())
			{
				if (!Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				{
					CommandParameters cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", arg);
					cp.AddCommandArgument("NotChangeKey", "0");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					lblClient.Text = BuildLinkWithIconAndText("red_denied.gif", "tRespGroup", cmd);
				}
				else
					lblClient.Text = BuildIconAndText("red_denied.gif", "tRespGroup");
			}
			else
			{
				if (!Mediachase.IBN.Business.Security.CurrentUser.IsExternal)
				{
					CommandParameters cp = new CommandParameters("MC_HDM_GroupResponsibilityInFrame");
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
					cp.AddCommandArgument("sUsersKey", arg);
					cp.AddCommandArgument("NotChangeKey", "0");
					string cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
					lblClient.Text = BuildLinkWithIconAndText("waiting.gif", "tRespGroup", cmd);
				}
				else
					lblClient.Text = BuildIconAndText("waiting.gif", "tRespGroup");
			}
		}
		#endregion

		#region btnSave_ServerClick
		void btnSave_ServerClick(object sender, EventArgs e)
		{
			string values = hfValues.Value;
			if (!String.IsNullOrEmpty(values))
			{
				DataTable dt = null;
				if (ViewState["ResponsiblePool"] != null)
					dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();

				int iRespId = -1;
				bool isRespGroup = false;

				switch (hidResp.Value)
				{
					case "-3":	//not change
						iRespId = -3;
						isRespGroup = false;
						break;
					case "-2":	//not set
						iRespId = -1;
						isRespGroup = false;
						break;
					case "-1":	//group
						iRespId = -1;
						isRespGroup = true;
						break;
					default:	//user
						try
						{
							iRespId = int.Parse(hidResp.Value, CultureInfo.InvariantCulture);
							isRespGroup = false;
						}
						catch { }
						break;
				}

				if (iRespId > -3)
				{
					string sMessage = txtComment.Text;
					sMessage = Mediachase.UI.Web.Util.CommonHelper.parsetext_br(sMessage);
					ArrayList errors = new ArrayList();
					string[] elems = values.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string s in elems)
					{
						int id = Convert.ToInt32(s, CultureInfo.InvariantCulture);
						try
						{
							Issue2.UpdateQuickTracking(id, sMessage, iRespId, isRespGroup, dt);
						}
						catch
						{
							errors.Add(id);
						}
					}

					if (errors.Count > 0)
					{
						divErrors.Visible = true;
						tblMain.Visible = false;
						ShowErrors(errors);
					}
					else
						CloseThis();
				}
				else
					CloseThis();
			}
			else
				CloseThis();
		}
		#endregion

		#region ShowErrors
		private void ShowErrors(ArrayList list)
		{
			string sIncs = String.Empty;
			foreach (int id in list)
			{
				sIncs += "<div style='padding-left:5px;'>-&nbsp;&nbsp;" + Incident.GetTitle(id) + "</div>";
			}
			lblResult.Text = String.Format(GetGlobalResourceObject("IbnFramework.Incident", "ResponsibleNotChanged").ToString(), sIncs);
		}
		#endregion

		#region btnClose_ServerClick
		void btnClose_ServerClick(object sender, EventArgs e)
		{
			CloseThis();
		}
		#endregion

		#region CloseThis
		private void CloseThis()
		{
			CHelper.RequireBindGrid();
			if (!String.IsNullOrEmpty(Request["commandName"]))
			{
				string commandName = Request["commandName"];
				CommandParameters cp = new CommandParameters(commandName);
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}
		#endregion

		#region IsAllDenied
		private bool IsAllDenied()
		{
			bool retVal = false;
			DataTable dt = ((DataTable)ViewState["ResponsiblePool"]).Copy();
			foreach (DataRow dr in dt.Rows)
				if ((bool)dr["ResponsePending"])
				{
					retVal = true;
					break;
				}
			return !retVal;
		}
		#endregion

		private string BuildIconAndText(string iconFileName, string textResourceName)
		{
			return string.Format(CultureInfo.InvariantCulture,
				"<span><img alt=\"\" src=\"{0}\" /></span> <span>{1}</span>",
				Page.ResolveUrl("~/Layouts/Images/" + iconFileName),
				HttpUtility.HtmlEncode(LocRM.GetString(textResourceName)));
		}

		private string BuildLinkWithIconAndText(string iconFileName, string textResourceName, string javaScriptCommand)
		{
			return string.Format(CultureInfo.InvariantCulture,
				"<a href=\"{0}\"><img alt=\"\" src=\"{1}\" /> {2}</a>",
				HttpUtility.HtmlAttributeEncode("javascript:" + javaScriptCommand),
				Page.ResolveUrl("~/Layouts/Images/" + iconFileName),
				HttpUtility.HtmlEncode(LocRM.GetString(textResourceName)));
		}
	}
}
