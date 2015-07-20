namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Resources;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using System.Reflection;

	/// <summary>
	///		Summary description for EmailLog.
	/// </summary>
	public partial class EmailLog : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region Sorting
		public string Sorting
		{
			get
			{
				if (pc["EmailLog_Sort"] != null)
					return pc["EmailLog_Sort"].ToString();
				else return "Created DESC";
			}
			set
			{
				pc["EmailLog_Sort"] = value;
			}
		}
		#endregion

		#region PageSize
		public string PageSize
		{
			get
			{
				if (pc["EmailLog_PageSize"] != null)
					return pc["EmailLog_PageSize"].ToString();
				else return "10";
			}
			set
			{
				pc["EmailLog_PageSize"] = value;
			}
		}
		#endregion

		#region PageIndex
		public string PageIndex
		{
			get
			{
				if (pc["EmailLog_PageIndex"] != null)
					return pc["EmailLog_PageIndex"].ToString();
				else return "0";
			}
			set
			{
				pc["EmailLog_PageIndex"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindGrid();
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tEmailLog");
			EMailMessageLogSetting cur = EMailMessageLogSetting.Current;
			if (cur.IsActive)
			{
				secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
					LocRM.GetString("tEmailLogSettings"),
					ResolveClientUrl("~/Layouts/Images/customize.gif")),
					String.Format("javascript:ShowWizard('{0}', 280, 100)", ResolveUrl("~/Admin/EmailLogSettings.aspx")));
			}

			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				!cur.IsActive ? LocRM.GetString("tEnableLog") : LocRM.GetString("tDisableLog"),
				!cur.IsActive ? ResolveClientUrl("~/layouts/images/icons/status_active.gif") : ResolveClientUrl("~/layouts/images/icons/status_stopped.gif")),
				"javascript:ChangeLogState()");
			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tClearLog"),
				ResolveClientUrl("~/Layouts/Images/delete.gif")),
				"javascript:ClearLog()");
			secHeader.AddLink("<img alt='' src='" + ResolveClientUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tHDM"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin5"));

		}

		private void BindGrid()
		{
			dgLog.Columns[0].HeaderText = LocRM.GetString("tDirection");
			dgLog.Columns[1].HeaderText = LocRM.GetString("tSubject");
			dgLog.Columns[2].HeaderText = LocRM.GetString("tFrom");
			dgLog.Columns[3].HeaderText = LocRM.GetString("tTo");
			dgLog.Columns[4].HeaderText = LocRM.GetString("tCreated");
			dgLog.Columns[5].HeaderText = LocRM.GetString("tEmailBox");
			dgLog.Columns[6].HeaderText = LocRM.GetString("tAntiSpamResult");


			foreach (DataGridColumn dgc in dgLog.Columns)
			{
				if (dgc.SortExpression == Sorting)
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + this.Page.ResolveUrl("~/layouts/images/upbtnF.jpg") + "'/>";
				else if (dgc.SortExpression + " DESC" == Sorting)
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + this.Page.ResolveUrl("~/layouts/images/downbtnF.jpg") + "'/>";
			}

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Direction", typeof(string)));
			dt.Columns.Add(new DataColumn("Subject", typeof(string)));
			dt.Columns.Add(new DataColumn("From", typeof(string)));
			dt.Columns.Add(new DataColumn("To", typeof(string)));
			dt.Columns.Add(new DataColumn("Created", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("EmailBoxId", typeof(int)));
			dt.Columns.Add(new DataColumn("AntiSpamResult", typeof(string)));


			EMailMessageLog[] emml = EMailMessageLog.List();
			foreach (EMailMessageLog item in emml)
			{
				DataRow dr = dt.NewRow();
				if (item.Incoming)
					dr["Direction"] = LocRM.GetString("tExtType");
				else
					dr["Direction"] = LocRM.GetString("tIntType");
				dr["Subject"] = item.Subject;
				dr["From"] = item.From;
				dr["To"] = item.To;
				dr["EmailBoxId"] = item.EMailBoxId;
				switch (item.AntiSpamResult)
				{
					case EMailMessageAntiSpamRuleRusult.Accept:
						dr["AntiSpamResult"] = LocRM.GetString("tAccepted");
						break;
					case EMailMessageAntiSpamRuleRusult.Deny:
						dr["AntiSpamResult"] = LocRM.GetString("tDenied");
						break;
					case EMailMessageAntiSpamRuleRusult.Pending:
						dr["AntiSpamResult"] = LocRM.GetString("tPending");
						break;
				}
				dr["Created"] = item.Created;
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;

			dv.Sort = Sorting;
			dgLog.DataSource = dv;

			dgLog.PageSize = int.Parse(PageSize);

			int iPageIndex = int.Parse(PageIndex);
			int ppi = dv.Count / dgLog.PageSize;
			if (dv.Count % dgLog.PageSize == 0)
				ppi = ppi - 1;
			if (iPageIndex <= ppi)
				dgLog.CurrentPageIndex = iPageIndex;
			else dgLog.CurrentPageIndex = 0;

			dgLog.DataBind();
		}

		protected string GetEmailBoxLink(int EmailBoxId)
		{
			if (EmailBoxId <= 0)
				return string.Empty;
			try
			{
				EMailRouterPop3Box emp3b = EMailRouterPop3Box.Load(EmailBoxId);
				string Link = "<a href='" + this.Page.ResolveUrl("~/Admin/HDMSettings.aspx") + "'>" + emp3b.Name + "</a>";
				return Link;

			}
			catch
			{
				return string.Empty;
			}

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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgLog.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgLog_PageSizeChanged);
			this.dgLog.PageIndexChanged += new DataGridPageChangedEventHandler(dgLog_PageIndexChanged);
			this.dgLog.SortCommand += new DataGridSortCommandEventHandler(dgLog_SortCommand);
			this.lbClearLog.Click += new EventHandler(lbClearLog_Click);
			this.lbChangeLogState.Click += new EventHandler(lbChangeLogState_Click);
		}

		void lbChangeLogState_Click(object sender, EventArgs e)
		{
			EMailMessageLogSetting cur = EMailMessageLogSetting.Current;
			cur.IsActive = !cur.IsActive;
			EMailMessageLogSetting.Update(cur);
			Response.Redirect("~/Incidents/EmailLog.aspx");
		}
		#endregion

		private void dgLog_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			PageSize = e.NewPageSize.ToString();
			PageIndex = "0";
			BindGrid();
		}

		private void dgLog_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			PageIndex = e.NewPageIndex.ToString();
			BindGrid();
		}

		private void dgLog_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (Sorting == e.SortExpression)
				Sorting += " DESC";
			else
				Sorting = e.SortExpression;
			BindGrid();
		}

		private void lbClearLog_Click(object sender, EventArgs e)
		{
			PageIndex = "0";
			EMailMessageLog.Clear();
			Response.Redirect("~/Incidents/EmailLog.aspx");
		}
	}
}
