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

namespace Mediachase.UI.Web.Common.Modules
{
	public partial class SelectIncident : System.Web.UI.UserControl
	{

		#region RefreshButton
		public string RefreshButton
		{
			get
			{
				if (Request.QueryString["btn"] != null)
					return HttpUtility.UrlDecode(Request.QueryString["btn"]);
				return "";
			}
		}
		#endregion

		#region ExcludeIssIdsFilter
		public string ExcludeIssIdsFilter
		{
			get
			{
				if (Request.QueryString["exclude"] != null)
				{
					string retVal = String.Empty;
					string[] mas = Request["exclude"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
					if (mas.Length > 0)
						for (int i = 0; i < mas.Length; i++)
						{
							if (i > 0)
								retVal += " AND ";
							retVal += String.Format("(IncidentId <> {0})", mas[i]);
						}
					return retVal;
				}
				return "";
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", Assembly.GetExecutingAssembly());
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			BindControls();

			if (!IsPostBack)
			{
				if (pc["SelectIssue_onlyActive"] == null)
					pc["SelectIssue_onlyActive"] = false.ToString();
				cbShowActive.Checked = bool.Parse(pc["SelectIssue_onlyActive"]);
				BindGrid();
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();
		}

		#region BindControls
		private void BindControls()
		{
			grdMain.Columns[1].HeaderText = LocRM.GetString("tTitle");
			btnSearch.Text = LocRM.GetString("Search");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			cbShowActive.Text = LocRM.GetString("tShowOnlyActiveIss");
			secHeader.Title = LocRM.GetString("SelectIssue");
			secHeader.AddImageLink("../Layouts/images/cancel.gif", LocRM.GetString("Close"), "javascript:window.close();");
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			string sSearch = tbSearchString.Text.Trim();
			DataView dv = Incident.GetListIncidentsByKeywordDataTable(sSearch).DefaultView;
			string rowFilter = "";
			if (!String.IsNullOrEmpty(ExcludeIssIdsFilter))
				rowFilter += ExcludeIssIdsFilter;
			if (cbShowActive.Checked)
				rowFilter += String.Format("{3}(StateId={0} OR StateId={1} OR StateId={2})",
					((int)Mediachase.IBN.Business.ObjectStates.Active).ToString(),
					((int)Mediachase.IBN.Business.ObjectStates.ReOpen).ToString(),
					((int)Mediachase.IBN.Business.ObjectStates.Upcoming).ToString(),
					(rowFilter.Length > 0 ? " AND " : ""));

			dv.RowFilter = rowFilter;
			if (ViewState["SelectIssue_Sort"] == null)
				ViewState["SelectIssue_Sort"] = "Title";
			if (ViewState["SelectIssue_CurrentPage"] == null)
				ViewState["SelectIssue_CurrentPage"] = 0;
			dv.Sort = ViewState["SelectIssue_Sort"].ToString();

			grdMain.DataSource = dv;

			if (ViewState["SelectIssue_PageSize"] != null)
				grdMain.PageSize = (int)ViewState["SelectIssue_PageSize"];

			int iPageIndex = (int)ViewState["SelectIssue_CurrentPage"];
			if (iPageIndex <= dv.Count / grdMain.PageSize)
				grdMain.CurrentPageIndex = iPageIndex;
			else
				grdMain.CurrentPageIndex = 0;

			grdMain.DataBind();

			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibRelate");
				if (ib != null)
				{
					ib.ToolTip = LocRM.GetString("Select");

					string sId = dgi.Cells[0].Text;

					string sAction = "";
					if (RefreshButton != "")
					{
						string _func = RefreshButton.Replace("xxxtypeid", "7");
						_func = _func.Replace("xxxid", sId);
						string retval = Util.CommonHelper.GetObjectHTMLTitle(7, int.Parse(sId));
						retval = retval.Replace("\"", "\\\"");
						_func = _func.Replace("xxxhtml", retval);
						sAction = "try{window.opener." + _func + ";} catch (e){} window.close();";
					}
					ib.Attributes.Add("onclick", sAction);
				}
			}
		}
		#endregion

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibRelate");
				if (ib != null)
				{
					Page.ClientScript.RegisterForEventValidation(ib.UniqueID);
				}
			}
			base.Render(writer);
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
			this.btnSearch.Click += new EventHandler(btnSearch_Click);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.cbShowActive.CheckedChanged += new EventHandler(cbShowActive_CheckedChanged);
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region grdMain_PageIndexChanged
		protected void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			ViewState["SelectIssue_CurrentPage"] = e.NewPageIndex;
			BindGrid();
		}
		#endregion

		#region grdMain_PageSizeChanged
		protected void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			ViewState["SelectIssue_PageSize"] = e.NewPageSize;
			ViewState["SelectIssue_CurrentPage"] = 0;
			BindGrid();
		}
		#endregion

		#region grdMain_SortCommand
		protected void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (ViewState["SelectIssue_Sort"].ToString() == e.SortExpression)
				ViewState["SelectIssue_Sort"] += " DESC";
			else
				ViewState["SelectIssue_Sort"] = e.SortExpression;
			ViewState["SelectIssue_CurrentPage"] = 0;
			BindGrid();
		}
		#endregion

		private void cbShowActive_CheckedChanged(object sender, EventArgs e)
		{
			pc["SelectIssue_onlyActive"] = cbShowActive.Checked.ToString();
			BindGrid();
		}
	}
}