using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaUI.Modules.SelectControls
{
	public partial class SelectMultiReference : System.Web.UI.UserControl
	{
		protected MetaFieldType mft = null;

		#region TypeName
		private string _typeName = "";
		public string TypeName
		{
			get { return _typeName; }
			set { _typeName = value; }
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
			this.ddClasses.SelectedIndexChanged += new EventHandler(ddClasses_SelectedIndexChanged);

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

		protected void ddClasses_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request.QueryString["type"] != null)
			{
				TypeName = Request.QueryString["type"];
				mft = MetaDataWrapper.GetTypeByName(TypeName);
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
			secHeader.Title = "Select Item";

			if (RefreshButton != String.Empty)	// Dialog Mode
			{
				string url = "javascript:window.close();";
				secHeader.AddImageLink(CHelper.GetAbsolutePath("/images/IbnFramework/close.gif"), "Close", url);
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			lblTypeName.Text = CHelper.GetResFileString(mft.FriendlyName);

			ddClasses.Items.Clear();
			foreach (MetaClass mc in MultiReferenceType.GetAvailableReferences(mft))
			{
				ddClasses.Items.Add(new ListItem(CHelper.GetResFileString(mc.FriendlyName), mc.Name));
			}

			if (ddClasses.SelectedItem != null)
				BindGrid();
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Id", typeof(PrimaryKeyId));
			dt.Columns.Add("Name", typeof(string));

			string sSearch = tbSearchString.Text.Trim();

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(ddClasses.SelectedValue);
			if (mc == null)
				return;

			MetaObject[] list = MetaObject.List(mc);

			foreach (MetaObject obj in list)
			{
				DataRow row = dt.NewRow();
				row["Id"] = obj.PrimaryKeyId.Value;
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
					string sId = ddClasses.SelectedValue + "_" + dgi.Cells[0].Text;
					string sAction = CHelper.GetCloseRefreshString(RefreshButton.Replace("xxx", sId));
					ib.Attributes.Add("onclick", sAction);
				}
			}
		}
		#endregion
	}
}