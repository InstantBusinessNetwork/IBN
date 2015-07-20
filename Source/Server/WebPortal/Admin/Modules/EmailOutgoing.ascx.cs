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
using System.Reflection;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class EmailOutgoing : System.Web.UI.UserControl
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

		protected void Page_Load(object sender, EventArgs e)
		{
			BindToolbar();
			if (!Page.IsPostBack)
			{
				BindGrid();
			}
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tEmailOutgoing");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("tHDM"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin5"));
		}

		private void BindGrid()
		{
			int i = 0;
			dgLog.Columns[i++].HeaderText = LocRM.GetString("tSubject");
			dgLog.Columns[i++].HeaderText = LocRM.GetString("tFrom");
			dgLog.Columns[i++].HeaderText = LocRM.GetString("tTo");
			dgLog.Columns[i++].HeaderText = LocRM.GetString("tCreatedReal");
			dgLog.Columns[i++].HeaderText = LocRM.GetString("tGeneration");
			dgLog.Columns[i++].HeaderText = LocRM.GetString("tErrorMsg");

			foreach (DataGridColumn dgc in dgLog.Columns)
			{
				if (dgc.SortExpression == Sorting)
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + this.Page.ResolveUrl("~/layouts/images/upbtnF.jpg") + "'/>";
				else if (dgc.SortExpression + " DESC" == Sorting)
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + this.Page.ResolveUrl("~/layouts/images/downbtnF.jpg") + "'/>";
			}

			//EmailMessageSmtpQueue[] list = EmailMessageSmtpQueue.List();
			DataTable dt = new DataTable();
			//dt.Columns.Add(new DataColumn("PrimaryKeyId", typeof(int)));
			//dt.Columns.Add(new DataColumn("Subject", typeof(string)));
			//dt.Columns.Add(new DataColumn("MailFrom", typeof(string)));
			//dt.Columns.Add(new DataColumn("RcptTo", typeof(string)));
			//dt.Columns.Add(new DataColumn("Created", typeof(DateTime)));
			//dt.Columns.Add(new DataColumn("Generation", typeof(int)));
			//dt.Columns.Add(new DataColumn("ErrorMsg", typeof(string)));
			//DataRow dr;
			//foreach (EmailMessageSmtpQueue mes in list)
			//{
			//    dr = dt.NewRow();
			//    dr["PrimaryKeyId"] = mes.PrimaryKeyId;
			//    dr["Subject"] = mes.Subject;
			//    dr["MailFrom"] = mes.MailFrom;
			//    dr["RcptTo"] = mes.RcptTo;
			//    dr["Created"] = User.GetLocalDate(mes.Created);
			//    dr["Generation"] = mes.Generation;
			//    dr["ErrorMsg"] = mes.ErrorMsg;
			//    dt.Rows.Add(dr);
			//}
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

			foreach (DataGridItem dgi in dgLog.Items)
			{
				ImageButton ibReset = (ImageButton)dgi.FindControl("ibReset");
				if (ibReset != null)
				{
					ibReset.ToolTip = LocRM.GetString("tResetGeneration");
				}
				ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
				if (ibDelete != null)
				{
					ibDelete.ToolTip = LocRM.GetString("tDelete");
					ibDelete.Attributes.Add("onclick", String.Format("if(!confirm('{0}')) return false;", LocRM.GetString("tWarningDelete")));
				}
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
			this.dgLog.ItemCommand += new DataGridCommandEventHandler(dgLog_ItemCommand);
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

		void dgLog_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Reset")
			{
				//EmailMessageSmtpQueue.ResetGeneration(int.Parse(e.CommandArgument.ToString()));
			}
			if (e.CommandName == "Delete")
			{
				//EmailMessageSmtpQueue.Delete(int.Parse(e.CommandArgument.ToString()));
			}
			BindGrid();
		}
	}
}