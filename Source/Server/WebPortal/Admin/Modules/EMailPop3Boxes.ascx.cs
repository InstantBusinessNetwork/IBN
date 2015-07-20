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
	///		Summary description for EMailPop3Boxes.
	/// </summary>
	public partial class EMailPop3Boxes : System.Web.UI.UserControl
	{
		#region HTML Vars
		#endregion

		private string constString = "400, 350, false";
		private string constStringNew = "400, 400, false";
		private string constString2 = "640, 570, false";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				divError.Visible = (ErrorId > 0);
				BindExtDG();
				BindInternal();
				BindLogSettings();
			}
			BindToolbars();
			btnUpdateLogSettings.CustomImage = this.Page.ResolveUrl("~/layouts/images/SAVEITEM.gif");
		}

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tEMailPop3");
			secInternal.AddText(LocRM.GetString("tInternalBox"));
			secExternal.AddText(LocRM.GetString("tExternalBoxes"));
			secExternal.AddRightLink("<img align='absmiddle' border='0' src='" + ResolveClientUrl("~/layouts/images/newitem.gif") + "' />&nbsp;" + LocRM.GetString("tAddBox"),
				"javascript:OpenWindow('" + this.Page.ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "', " + constStringNew + ")");
			secLog.AddText(LocRM.GetString("tEmailLogSettings"));
			btnUpdateLogSettings.Text = LocRM.GetString("tSave");
			cbLogActive.Text = "&nbsp;" + LocRM.GetString("tActive");
			EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
			if (pop3Int != null)
			{
				secInternal.AddRightLink("<img align='absmiddle' border='0' src='" + ResolveClientUrl("~/layouts/images/edit.gif") + "' />&nbsp;" + LocRM.GetString("tEdit"),
					"javascript:OpenWindow('" + this.Page.ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "?BoxId=" + pop3Int.EMailRouterPop3BoxId + "', " + constString + ")");
			}

		}
		#endregion

		#region BindExtDG
		private void BindExtDG()
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
				dr["LastErrorText"] = act.ErrorText;
				dr["TotalMessageCount"] = act.TotalMessageCount;
				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "Name";

			int i = 1;
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tName");
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tStatus");
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tMessageCount");
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tLastReq");
			dgBoxes.Columns[i++].HeaderText = LocRM.GetString("tLastSuccReq");

			dgBoxes.DataSource = dv;
			dgBoxes.DataBind();

			foreach (DataGridItem dgi in dgBoxes.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
					ib.Attributes.Add("title", LocRM.GetString("tDelete"));
				}
			}
		}
		#endregion

		#region BindInternal
		private void BindInternal()
		{
			EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
			if (pop3Int != null)
			{
				intBox.Visible = true;
				lblNoIntBox.Visible = false;
				lblIntName.Text = pop3Int.Name;
				lblIntServer.Text = pop3Int.Server;
				lblIntStatus.Text = GetStatus((object)pop3Int.Activity.IsActive,
					(object)pop3Int.Activity.LastRequest, (object)pop3Int.Activity.LastSuccessfulRequest,
					(object)pop3Int.Activity.ErrorText);
				lbIntChangeStatus.Text = String.Format("(&nbsp;<img border='0' align='absmiddle' src='{0}' />&nbsp;{1}&nbsp;)",
					(pop3Int.Activity.IsActive) ?
						this.Page.ResolveUrl("~/layouts/images/icons/status_stopped.gif") :
						this.Page.ResolveUrl("~/layouts/images/icons/status_active.gif"),
					(pop3Int.Activity.IsActive) ?
						LocRM.GetString("tChangeStatusStop") :
						LocRM.GetString("tChangeStatusRun"));
				lblIntPort.Text = pop3Int.Port.ToString();
				lblIntAddress.Text = pop3Int.EMailAddress;
				lblLastReq.Text = (pop3Int.Activity.LastRequest == DateTime.MinValue) ? "" : pop3Int.Activity.LastRequest.ToString("g");
				lblLastSuccReq.Text = (pop3Int.Activity.LastSuccessfulRequest == DateTime.MinValue) ? "" : pop3Int.Activity.LastSuccessfulRequest.ToString("g");
				
				lbMessageCount.Text = pop3Int.Activity.TotalMessageCount.ToString();

			}
			else
			{
				intBox.Visible = false;
				lblNoIntBox.Visible = true;
				lblNoIntBox.Text = LocRM.GetString("tNoIntBox");
				secInternal.AddRightLink(string.Format("<img align='absmiddle' border='0' src='{0}' />&nbsp;", this.Page.ResolveUrl("~/layouts/images/newitem.gif")) + LocRM.GetString("tAddBox"),
					"javascript:OpenWindow('" + this.Page.ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx?IsInternal=1") + "', " + constStringNew + ")");
			}
		}
		#endregion

		#region Protected DG Strings
		protected string GetBoxLink(int id, string server)
		{
			string retVal = "";
			retVal = String.Format("<a href=\"javascript:OpenWindow('{0}', " + constString + ")\">{1}</a>",
				this.Page.ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "?BoxId=" + id.ToString(),
				server);
			return retVal;
		}

		protected string GetEditButton(int id, string Tooltip)
		{
			return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/></a>",
				this.Page.ResolveUrl("~/layouts/images/edit.gif"),
				Tooltip, constString,
				this.Page.ResolveUrl("~/Admin/EMailPop3BoxEdit.aspx") + "?BoxId=" + id.ToString());
		}

		protected string GetMappingButton(int id, string Tooltip)
		{
			if (id == ErrorId)
				return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img style='border:2px solid #FF746B;' align='absmiddle' src='{0}' title='{1}'/></a>",
					this.Page.ResolveUrl("~/layouts/images/rules.gif"),
					Tooltip, constString2,
					this.Page.ResolveUrl("~/Admin/EMailDefaultMapping.aspx") + "?BoxId=" + id.ToString());
			else
				return String.Format("<a href=\"javascript:OpenWindow('{3}', {2})\"><img border='0' align='absmiddle' src='{0}' title='{1}'/></a>",
					this.Page.ResolveUrl("~/layouts/images/rules.gif"),
					Tooltip, constString2,
					this.Page.ResolveUrl("~/Admin/EMailDefaultMapping.aspx") + "?BoxId=" + id.ToString());
		}

		protected string GetStatus(object IsActive, object LastRequest, object LastSuccessfulRequest, object LastErrorText)
		{
			string retval = "";
			if ((bool)IsActive)
			{
				if ((DateTime)LastRequest != (DateTime)LastSuccessfulRequest)
					retval = "<img border='0' width='16px' src='" + this.Page.ResolveUrl("~/layouts/images/icons/status_problem.gif") + "' height='16px' align='absmiddle' title='" + (string)LastErrorText + "'/>";
				else
					retval = "<img border='0' width='16px' src='" + this.Page.ResolveUrl("~/layouts/images/icons/status_active.gif") + "' height='16px' align='absmiddle'/>";
			}
			else
			{
				retval = "<img border='0' width='16px' src='" + this.Page.ResolveUrl("~/layouts/images/icons/status_stopped.gif") + "' height='16px' align='absmiddle'/>";
			}
			return retval;
		}

		protected string GetStatusDG(object IsActive)
		{
			return String.Format("(&nbsp;<img border='0' align='absmiddle' src='{0}' />&nbsp;{1}&nbsp;)",
				((bool)IsActive) ?
					this.Page.ResolveUrl("~/layouts/images/icons/status_stopped.gif") :
					this.Page.ResolveUrl("~/layouts/images/icons/status_active.gif"),
				((bool)IsActive) ?
					LocRM.GetString("tChangeStatusStop") :
					LocRM.GetString("tChangeStatusRun"));
		}
		#endregion

		#region BindLogSettings
		protected void BindLogSettings()
		{
			EMailMessageLogSetting cur = EMailMessageLogSetting.Current;
			if (cur != null)
			{
				cbLogActive.Checked = cur.IsActive;
				if (!cur.IsActive)
				{
					tbLogPeriod.Text = cur.Period.ToString();
					tbLogPeriod.Enabled = false;
				}
				else
				{
					tbLogPeriod.Enabled = true;
					tbLogPeriod.Text = cur.Period.ToString();
				}
			}
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
			this.lbIntChangeStatus.Click += new EventHandler(lbIntChangeStatus_Click);
			this.dgBoxes.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_delete);
			this.dgBoxes.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_ItemCommand);
			this.btnUpdateLogSettings.ServerClick += new EventHandler(btnUpdateLogSettings_ServerClick);
		}
		#endregion

		#region DG Events
		private void dg_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			EMailRouterPop3Box.Delete(sid);
			Response.Redirect("~/Admin/EMailPop3Boxes.aspx");
		}

		private void dg_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "ChangeStatus")
			{
				int sid = int.Parse(e.Item.Cells[0].Text);
				EMailRouterPop3Box ebox = EMailRouterPop3Box.Load(sid);
				if (EMailRouterPop3Box.CanActivate(sid))
				{
					EMailRouterPop3Box.Activate(sid, !ebox.Activity.IsActive);
					Response.Redirect("~/Admin/EMailPop3Boxes.aspx");
				}
				else
					Response.Redirect("~/Admin/EMailPop3Boxes.aspx?errorsid=" + sid);
			}
		}
		#endregion

		private void lbIntChangeStatus_Click(object sender, EventArgs e)
		{
			EMailRouterPop3Box pop3Int = EMailRouterPop3Box.ListInternal();
			if (pop3Int != null && EMailRouterPop3Box.CanActivate(pop3Int.EMailRouterPop3BoxId))
				EMailRouterPop3Box.Activate(pop3Int.EMailRouterPop3BoxId, !pop3Int.Activity.IsActive);
			Response.Redirect(this.Page.ResolveUrl("~/Admin/EMailPop3Boxes.aspx"));
		}

		#region btnUpdateLogSettings_ServerClick
		private void btnUpdateLogSettings_ServerClick(object sender, EventArgs e)
		{
			EMailMessageLogSetting cur = EMailMessageLogSetting.Current;
			if (cbLogActive.Checked)
			{
				try
				{
					if (int.Parse(tbLogPeriod.Text) > 0)
						cur.Period = int.Parse(tbLogPeriod.Text);
				}
				catch
				{
					cur.Period = 7;
				}
			}
			cur.IsActive = cbLogActive.Checked;
			EMailMessageLogSetting.Update(cur);
			//BindLogSettings();
			Response.Redirect("~/Admin/EMailPop3Boxes.aspx");
		}
		#endregion
	}
}
