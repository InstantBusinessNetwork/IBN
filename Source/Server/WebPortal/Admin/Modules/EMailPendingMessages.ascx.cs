namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
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
	///		Summary description for EMailPendingMessages.
	/// </summary>
	public partial class EMailPendingMessages : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (pc["emr_PendMess_Sort"] == null)
				pc["emr_PendMess_Sort"] = "PendingMessageId DESC";
			if (!Page.IsPostBack)
				BindDataGrid();
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbars();
		}

		#region BindToolbars
		private void BindToolbars()
		{
			secHeader.Title = LocRM.GetString("tPendingMess");
			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tApproveChecked"), this.Page.ResolveUrl("~/Layouts/Images/icon-key.gif")),
				"javascript:ActionChecked('approve')");

			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tAddToExistIss"), this.Page.ResolveUrl("~/Layouts/Images/icons/incident_plus.gif")),
				String.Format("javascript:AddChecked(&quot;{0}&quot;)", Page.ClientScript.GetPostBackEventReference(btnAddToExIss, "xxxsel;xxxtypeid;xxxid")));

			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tDeleteChecked"),
				this.Page.ResolveUrl("~/Layouts/Images/delete.gif")),
				"javascript:ActionChecked('delete')");
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			int i = 1;
			dgPendMess.Columns[i++].HeaderText = "ID";
			dgPendMess.Columns[i++].HeaderText = LocRM.GetString("tSubject");
			dgPendMess.Columns[i++].HeaderText = LocRM.GetString("tFrom");
			dgPendMess.Columns[i++].HeaderText = LocRM.GetString("tTo");
			dgPendMess.Columns[i++].HeaderText = LocRM.GetString("tCreated");
			foreach (DataGridColumn dgc in dgPendMess.Columns)
			{
				if (dgc.SortExpression == pc["emr_PendMess_Sort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + this.Page.ResolveUrl("~/layouts/images/upbtnF.jpg") + "'/>";
				else if (dgc.SortExpression + " DESC" == pc["emr_PendMess_Sort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + this.Page.ResolveUrl("~/layouts/images/downbtnF.jpg") + "'/>";
			}

			/*			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("PendingMessageId", typeof(int)));
			dt.Columns.Add(new DataColumn("From", typeof(string)));
			dt.Columns.Add(new DataColumn("To", typeof(string)));
			dt.Columns.Add(new DataColumn("Subject", typeof(string)));
			dt.Columns.Add(new DataColumn("Created", typeof(DateTime)));
			DataRow dr;
			int[] pendList = EMailMessage.ListPendigEMailMessageIds();
			foreach(int id in pendList)
			{
				dr = dt.NewRow();
				dr["PendingMessageId"] = id;
//				EMailMessageInfo emi = EMailMessageInfo.Load(id);
//				dr["From"] = GetAddress(emi.From);
//				dr["To"] = GetAddress(emi.To);
//				dr["Subject"] = emi.Subject;
//				dr["Created"] = emi.Created;
 				dt.Rows.Add(dr);
			}
			DataView dv = dt.DefaultView;
 */
			DataView dv = EMailMessage.GetPendingMessages().DefaultView;
			dv.Sort = pc["emr_PendMess_Sort"].ToString();

			dgPendMess.DataSource = dv;

			if (pc["emr_PendMess_PageSize"] != null)
				dgPendMess.PageSize = int.Parse(pc["emr_PendMess_PageSize"].ToString());

			if (pc["emr_PendMess_Page"] != null)
			{
				int iPageIndex = int.Parse(pc["emr_PendMess_Page"].ToString());
				int ppi = dv.Count / dgPendMess.PageSize;
				if (dv.Count % dgPendMess.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dgPendMess.CurrentPageIndex = iPageIndex;
				else dgPendMess.CurrentPageIndex = 0;
			}
			dgPendMess.DataBind();

			foreach (DataGridItem dgi in dgPendMess.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarningDelete") + "')");
					ib.Attributes.Add("title", LocRM.GetString("tDelete"));
				}
				ImageButton ib1 = (ImageButton)dgi.FindControl("ibApprove");
				if (ib1 != null)
				{
					ib1.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarningApprove") + "')");
					ib1.Attributes.Add("title", LocRM.GetString("tApprove"));
				}
			}
		}
		#endregion

		#region GetSubjectLink
		protected string GetSubjectLink(string sId, string sSubject)
		{
			string retVal = "";
			if (sSubject == "" || sSubject == "&nbsp;")
				sSubject = LocRM.GetString("tNosubject");
			retVal = String.Format("<a href=\"javascript:OpenMessage({0})\">{1}</a>&nbsp;", sId, sSubject);
			return retVal;
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
			this.dgPendMess.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.dgPendMess.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);
			this.dgPendMess.SortCommand += new DataGridSortCommandEventHandler(dg_SortCommand);
			this.dgPendMess.ItemCommand += new DataGridCommandEventHandler(dgPendMess_ItemCommand);
			this.lbDeleteChecked.Click += new EventHandler(lbDeleteChecked_Click);
			this.lbApproveChecked.Click += new EventHandler(lbApproveChecked_Click);
			this.btnAddToExIss.Click += new EventHandler(btnAddToExIss_Click);
		}
		#endregion

		#region DataGrid_Events
		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["emr_PendMess_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["emr_PendMess_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dg_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if ((pc["emr_PendMess_Sort"] != null) && (pc["emr_PendMess_Sort"].ToString() == (string)e.SortExpression))
				pc["emr_PendMess_Sort"] = (string)e.SortExpression + " DESC";
			else
				pc["emr_PendMess_Sort"] = (string)e.SortExpression;

			pc["emr_PendMess_Page"] = "0";
			BindDataGrid();
		}

		private void dgPendMess_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				int Id = int.Parse(e.CommandArgument.ToString());
				EMailMessage.Delete(Id);
				Response.Redirect("~/Incidents/EMailPendingMessages.aspx");
			}
			else if (e.CommandName == "Approve")
			{
				int Id = int.Parse(e.CommandArgument.ToString());
				EMailMessage.ApprovePending(Id);
				Response.Redirect("~/Incidents/EMailPendingMessages.aspx");
			}
		}
		#endregion

		#region Delete_Approve_Checked
		private void lbDeleteChecked_Click(object sender, EventArgs e)
		{
			string sIds = hidForDelete.Value;
			ArrayList alIds = new ArrayList();
			while (sIds.Length > 0)
			{
				alIds.Add(int.Parse(sIds.Substring(0, sIds.IndexOf(","))));
				sIds = sIds.Remove(0, sIds.IndexOf(",") + 1);
			}
			EMailMessage.Delete(alIds);
			Response.Redirect("~/Incidents/EMailPendingMessages.aspx");
		}

		private void lbApproveChecked_Click(object sender, EventArgs e)
		{
			string sIds = hidForDelete.Value;
			ArrayList alIds = new ArrayList();
			while (sIds.Length > 0)
			{
				alIds.Add(int.Parse(sIds.Substring(0, sIds.IndexOf(","))));
				sIds = sIds.Remove(0, sIds.IndexOf(",") + 1);
			}
			EMailMessage.ApprovePending(alIds);
			Response.Redirect("~/Incidents/EMailPendingMessages.aspx");
		}
		#endregion

		#region GetAddress
		private string GetAddress(string sAddress)
		{
			if (sAddress.IndexOf("<") >= 0)
			{
				sAddress = sAddress.Substring(sAddress.IndexOf("<") + 1);
				if (sAddress.IndexOf(">") >= 0)
					sAddress = sAddress.Substring(0, sAddress.IndexOf(">"));
			}
			return sAddress;
		}
		#endregion

		protected void btnAddToExIss_Click(object sender, EventArgs e)
		{
			string param = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(param))
			{
				string[] mas = param.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length < 3 || !mas[1].Equals("7"))
					return;

				string[] selIds = mas[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				ArrayList aList = new ArrayList();
				for (int i = 0; i < selIds.Length; i++)
				{
					aList.Add(int.Parse(selIds[i]));
				}
				int iToincId = int.Parse(mas[2]);
				EMailMessage.CopyToIncident(aList, iToincId);
				Response.Redirect("~/Incidents/EMailPendingMessages.aspx");
			}
		}

		/*		#region dgPendMess_ItemCreated
				protected void dgPendMess_ItemCreated(object sender, DataGridItemEventArgs e)
				{
					if ((e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) && e.Item.DataItem != null)
					{
						DataRow row = ((DataRowView)(e.Item.DataItem)).Row;
						int id = (int)row["PendingMessageId"];

						EMailMessageInfo emi = EMailMessageInfo.Load(id);
						row["From"] = GetAddress(emi.From);
						row["To"] = GetAddress(emi.To);
						row["Subject"] = emi.Subject;
						row["Created"] = emi.Created;
					}
				}
				#endregion
				 */
	}
}
