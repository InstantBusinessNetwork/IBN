using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaUI.Modules.SelectControls
{
	public partial class SelectItem : System.Web.UI.UserControl
	{
		protected MetaClass mc = null;

		#region ClassName
		private string _className = "";
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

		#region RefreshButton
		private string _refreshButton = String.Empty;
		public string RefreshButton
		{
			get { return _refreshButton; }
			set { _refreshButton = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region Events
		protected void Page_Init(object sender, EventArgs e)
		{
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.btnSearch.Click += new EventHandler(btnSearch_Click);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
		}

		protected void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			ViewState["SelectItem_CurrentPage"] = e.NewPageIndex;
			BindGrid();
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			BindGrid();
		}

		protected void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (ViewState["SelectItem_Sort"].ToString() == e.SortExpression)
				ViewState["SelectItem_Sort"] += " DESC";
			else
				ViewState["SelectItem_Sort"] = e.SortExpression;
			ViewState["SelectItem_CurrentPage"] = 0;
			BindGrid();
		}
		#endregion

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}

			if (Request.QueryString["btn"] != null)
			{
				RefreshButton = Request.QueryString["btn"];
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "SelectItem").ToString();

			if (RefreshButton != String.Empty)	// Dialog Mode
			{
				string url = "javascript:window.close();";
				secHeader.AddImageLink(
					ResolveClientUrl("~/images/IbnFramework/close.gif"),
					GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "Close").ToString(), 
					url);
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			lblClassName.Text = CHelper.GetResFileString(mc.PluralName);

			BindGrid();
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Name", typeof(string));

			string sSearch = tbSearchString.Text.Trim();

			FilterElementCollection fec = new FilterElementCollection();
			if (!String.IsNullOrEmpty(Request["filterName"]) && !String.IsNullOrEmpty(Request["filterValue"]))
			{
				FilterElement fe = FilterElement.EqualElement(Request["filterName"], Request["filterValue"]);
				FilterElement fe1 = FilterElement.IsNullElement(Request["filterName"]);
				FilterElement feOr = new OrBlockFilterElement();
				feOr.ChildElements.Add(fe);
				feOr.ChildElements.Add(fe1);
				fec.Add(feOr);
			}
			else if (!String.IsNullOrEmpty(Request["filterName"]) && String.IsNullOrEmpty(Request["filterValue"]))
			{
				FilterElement fe = FilterElement.IsNullElement(Request["filterName"]);
				fec.Add(fe);
			}

			MetaObject[] list = MetaObject.List(mc, fec.ToArray());

			foreach (MetaObject obj in list)
			{
				DataRow row = dt.NewRow();
				row["Id"] = obj.PrimaryKeyId.ToString();
				if (obj.Properties[mc.TitleFieldName].Value != null)
					row["Name"] = CHelper.GetResFileString(obj.Properties[mc.TitleFieldName].Value.ToString());
				dt.Rows.Add(row);
			}

			if (ViewState["SelectItem_Sort"] == null)
				ViewState["SelectItem_Sort"] = "Name";
			if (ViewState["SelectItem_CurrentPage"] == null)
				ViewState["SelectItem_CurrentPage"] = 0;
			if (dt.Rows != null && dt.Rows.Count < grdMain.PageSize)
				grdMain.AllowPaging = false;
			else
				grdMain.AllowPaging = true;
			DataView dv = dt.DefaultView;
			if (sSearch != String.Empty)
			{
				dv.RowFilter = String.Format(CultureInfo.InvariantCulture, "Name LIKE '%{0}%'", sSearch);
			}
			dv.Sort = ViewState["SelectItem_Sort"].ToString();
			if (ViewState["SelectItem_CurrentPage"] != null && grdMain.AllowPaging)
				grdMain.CurrentPageIndex = (int)ViewState["SelectItem_CurrentPage"];
			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibRelate");
				if (ib != null)
				{
					string sId = dgi.Cells[0].Text;
					string sAction = CHelper.GetCloseRefreshString(RefreshButton.Replace("xxx", sId));
					ib.Attributes.Add("onclick", sAction);
				}
			}
		}
		#endregion
	}
}