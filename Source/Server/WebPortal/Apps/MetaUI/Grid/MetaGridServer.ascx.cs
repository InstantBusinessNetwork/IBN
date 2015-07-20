using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.XPath;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.MetaUI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.UI.Web.Apps.MetaUI.Grid
{
	#region class: MetaGridContextKey
	/// <summary>
	/// Обьект для передачи в ContextKey для WebService
	/// </summary>
	class MetaGridContextKey
	{
		#region prop: ViewName
		private string _viewName;

		/// <summary>
		/// Gets or sets the name of the view.
		/// </summary>
		/// <value>The name of the view.</value>
		public string ViewName
		{
			get { return _viewName; }
			set { _viewName = value; }
		}
		#endregion

		#region prop: CustomColumnsCount
		private int customColumnsCount;

		/// <summary>
		/// Gets or sets the custom columns count.
		/// </summary>
		/// <value>The custom columns count.</value>
		public int CustomColumnsCount
		{
			get { return customColumnsCount; }
			set { customColumnsCount = value; }
		}
		#endregion

		#region .ctor
		public MetaGridContextKey()
		{
		}

		public MetaGridContextKey(string viewName)
			: this()
		{
			this.ViewName = viewName;
		}

		public MetaGridContextKey(string viewName, int customColumnCount)
			: this(viewName)
		{
			this.CustomColumnsCount = customColumnCount;
		}
		#endregion
	}
	#endregion

	#region class: MetaGridCustomColumnInfo
	/// <summary>
	/// Description for custom columns (like ActionsColumn)
	/// </summary>
	class MetaGridCustomColumnInfo
	{
		#region prop: Column
		private DataControlField _column;

		/// <summary>
		/// Gets or sets the column.
		/// </summary>
		/// <value>The column.</value>
		public DataControlField Column
		{
			get { return _column; }
			set { _column = value; }
		}
		#endregion

		#region prop: Width
		private int _width = 50;

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get { return _width; }
			set { _width = value; }
		}
		#endregion

		#region prop: Title
		private string _title = string.Empty;

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}
		#endregion

		#region prop: Type
		private string _type;

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The title.</value>
		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}
		#endregion

		#region prop: Id
		private string _id = string.Empty;

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}
		#endregion

		#region .ctor
		public MetaGridCustomColumnInfo()
		{
		}

		public MetaGridCustomColumnInfo(string Type)
			: this()
		{
			this.Type = Type;
		}

		public MetaGridCustomColumnInfo(string Type, string Id)
			: this(Type)
		{
			this.Id = Id;
		}

		public MetaGridCustomColumnInfo(string Type, string Id, DataControlField Column)
			: this(Type, Id)
		{
			this.Column = Column;
		}
		#endregion
	}
	#endregion

	#region class: ChangingMetaGridColumnHeaderEventArgs
	public class ChangingMetaGridColumnHeaderEventArgs : EventArgs
	{
		#region ControlField
		private DataControlField _controlField;
		public DataControlField ControlField
		{
			get
			{
				return _controlField;
			}
		}
		#endregion

		#region prop: Field
		private MetaField _field;
		public MetaField Field
		{
			get
			{
				return _field;
			}
		}
		#endregion

		#region .ctor
		public ChangingMetaGridColumnHeaderEventArgs(DataControlField controlField, MetaField field)
		{
			_controlField = controlField;
			_field = field;
		}
		#endregion
	}

	public delegate void ChangingMetaGridColumnHeaderEventHandler(object sender, ChangingMetaGridColumnHeaderEventArgs e);
	#endregion

	public partial class MetaGridServer : System.Web.UI.UserControl
	{
		private readonly string placeNameKey = "PlaceName";

		#region styles: Default constants
		private readonly string defaultHeaderCssClass = "serverGridHeader";
		private readonly string defaultGridCssClass = "serverGridBody";
		private readonly string defaultFooterCssClass = "serverGridFooter";
		private readonly string defaultHeaderInnerCssClass = "serverGridHeaderInner";
		private readonly string defaultGridInnerCssClass = "serverGridInner";
		private readonly string defaultGridSelectedRowCssClass = "serverGridSelectedRow";
		#endregion

		#region prop: ViewName
		/// <summary>
		/// Gets or sets the name of the view.
		/// </summary>
		/// <value>The name of the view.</value>
		public string ViewName
		{
			get
			{
				if (ViewState[this.ID + "_viewName"] != null)
					return (string)ViewState[this.ID + "_viewName"];

				if (CHelper.GetFromContext("MetaViewName") != null)
					return (string)CHelper.GetFromContext("MetaViewName");

				return string.Empty;
			}
			set
			{
				ViewState[this.ID + "_viewName"] = value;
				CHelper.AddToContext("MetaViewName", value);
			}
		}
		#endregion

		#region prop: CurrentView
		private MetaView currentView;

		/// <summary>
		/// Gets the current view.
		/// </summary>
		/// <value>The current view.</value>
		public MetaView CurrentView
		{
			get
			{
				if (currentView == null)
				{
					if (DataContext.Current.MetaModel.MetaViews[ViewName] == null)
						throw new ArgumentException(String.Format("Cant find meta view: {0}", ViewName));

					currentView = DataContext.Current.MetaModel.MetaViews[ViewName];
				}

				return currentView;
			}
		}
		#endregion

		#region prop: PlaceName
		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		/// <value>The name of the place.</value>
		public string PlaceName
		{
			get
			{
				if (ViewState[placeNameKey] != null)
					return (string)ViewState[placeNameKey];
				else
					return String.Empty;
			}
			set
			{
				ViewState[placeNameKey] = value;
			}
		}
		#endregion

		#region prop: GridClientContainerId
		/// <summary>
		/// Gets the grid client container id.
		/// </summary>
		/// <value>The grid client container id.</value>
		public string GridClientContainerId
		{
			get
			{
				return this.MainGrid.ClientID;
			}
		}
		#endregion

		#region prop: ShowCheckboxes
		/// <summary>
		/// Gets or sets a value indicating whether [show checkboxes].
		/// </summary>
		/// <value><c>true</c> if [show checkboxes]; otherwise, <c>false</c>.</value>
		public bool ShowCheckboxes
		{
			get
			{
				if (ViewState["_ShowCheckboxes"] != null)
					return Convert.ToBoolean(ViewState["_ShowCheckboxes"].ToString());

				return true;
			}
			set
			{
				ViewState["_ShowCheckboxes"] = value;
			}
		}
		#endregion

		#region prop: CustomColumns
		private List<MetaGridCustomColumnInfo> _customColumns = null;
		/// <summary>
		/// Gets the custom columns.
		/// </summary>
		/// <value>The custom columns.</value>
		private List<MetaGridCustomColumnInfo> CustomColumns
		{
			get
			{
				if (_customColumns == null)
					_customColumns = new List<MetaGridCustomColumnInfo>();

				return _customColumns;
			}
		}
		#endregion

		#region prop: VisibleMetaFields
		private MetaField[] _visibleMetaFields = null;

		/// <summary>
		/// Gets or sets the visible meta fields.
		/// </summary>
		/// <value>The visible meta fields.</value>
		protected MetaField[] VisibleMetaFields
		{
			get
			{
				if (_visibleMetaFields == null)
					_visibleMetaFields = GetMetaViewPreference().GetVisibleMetaField();

				return _visibleMetaFields;
			}
			set
			{
				_visibleMetaFields = value;
			}
		}
		#endregion

		#region prop: SearchKeyword
		/// <summary>
		/// Gets or sets the search keyword.
		/// </summary>
		/// <value>The search keyword.</value>
		public string SearchKeyword
		{
			get
			{
				if (ViewState["_SearchKeyword"] != null)
					return ViewState["_SearchKeyword"].ToString();

				return string.Empty;
			}
			set { ViewState["_SearchKeyword"] = value; }
		}
		#endregion

		#region prop: Count
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				if (ViewState["_Count"] == null)
					return 0;
				return Convert.ToInt32(ViewState["_Count"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_Count"] = value;
			}
		}
		#endregion

		#region --- prop: Styles ---

		#region prop: HeaderCssClass
		/// <summary>
		/// Gets or sets the header CSS class.
		/// </summary>
		/// <value>The header CSS class.</value>
		public string HeaderCssClass
		{
			get
			{
				if (ViewState["_HeaderCssClass"] == null)
					return defaultHeaderCssClass;

				return ViewState["_HeaderCssClass"].ToString();
			}
			set { ViewState["_HeaderCssClass"] = value; }
		}
		#endregion

		#region prop: GridCssClass
		/// <summary>
		/// Gets or sets the grid CSS class.
		/// </summary>
		/// <value>The grid CSS class.</value>
		public string GridCssClass
		{
			get
			{
				if (ViewState["_GridCssClass"] == null)
					return defaultGridCssClass;

				return ViewState["_GridCssClass"].ToString();
			}
			set { ViewState["_GridCssClass"] = value; }
		}
		#endregion

		#region prop: FooterCssClass
		/// <summary>
		/// Gets or sets the footer CSS class.
		/// </summary>
		/// <value>The footer CSS class.</value>
		public string FooterCssClass
		{
			get
			{
				if (ViewState["_FooterCssClass"] == null)
					return defaultFooterCssClass;

				return ViewState["_FooterCssClass"].ToString();
			}
			set { ViewState["_FooterCssClass"] = value; }
		}
		#endregion

		#region prop: GridInnerCssClass
		/// <summary>
		/// Gets or sets the grid inner CSS class.
		/// </summary>
		/// <value>The grid inner CSS class.</value>
		public string GridInnerCssClass
		{
			get
			{
				if (ViewState["_GridInnerCssClass"] == null)
					return defaultGridInnerCssClass;

				return ViewState["_GridInnerCssClass"].ToString();
			}
			set { ViewState["_GridInnerCssClass"] = value; }
		}
		#endregion

		#region prop: HeaderInnerCssClass
		/// <summary>
		/// Gets or sets the header inner CSS class.
		/// </summary>
		/// <value>The header inner CSS class.</value>
		public string HeaderInnerCssClass
		{
			get
			{
				if (ViewState["_HeaderInnerCssClass"] == null)
					return defaultHeaderInnerCssClass;

				return ViewState["_HeaderInnerCssClass"].ToString();
			}
			set { ViewState["_HeaderInnerCssClass"] = value; }
		}
		#endregion

		#region prop: GridSelectedRowCssClass
		/// <summary>
		/// Gets or sets the grid selected row CSS class.
		/// </summary>
		/// <value>The grid selected row CSS class.</value>
		public string GridSelectedRowCssClass
		{
			get
			{
				if (ViewState["_GridSelectedRowCssClass"] == null)
					return defaultGridSelectedRowCssClass;

				return ViewState["_GridSelectedRowCssClass"].ToString();
			}
			set { ViewState["_GridSelectedRowCssClass"] = value; }
		}
		#endregion

		#endregion

		#region prop: PrimaryGroupType
		/// <summary>
		/// Gets or sets the type of the primary group.
		/// </summary>
		/// <value>The type of the primary group.</value>
		public string PrimaryGroupType
		{
			get
			{
				if (ViewState["_PrimaryGroupType"] == null)
					return MetaViewGroupUtil.keyValueNotDefined;

				if (ViewState["_PrimaryGroupType"].ToString() == string.Empty)
					return MetaViewGroupUtil.keyValueNotDefined;

				return ViewState["_PrimaryGroupType"].ToString();
			}
			set
			{
				ViewState["_PrimaryGroupType"] = value;
			}
		}
		#endregion

		#region prop: SecondaryGroupType
		/// <summary>
		/// Gets or sets the type of the secondary group.
		/// </summary>
		/// <value>The type of the secondary group.</value>
		public string SecondaryGroupType
		{
			get
			{
				if (ViewState["_SecondaryGroupType"] == null)
					return MetaViewGroupUtil.keyValueNotDefined;

				if (ViewState["_SecondaryGroupType"].ToString() == string.Empty)
					return MetaViewGroupUtil.keyValueNotDefined;

				return ViewState["_SecondaryGroupType"].ToString();
			}
			set
			{
				ViewState["_SecondaryGroupType"] = value;
			}
		}
		#endregion

		#region event: OnChangingMetaGridColumnHeader
		public event ChangingMetaGridColumnHeaderEventHandler ChangingMetaGridColumnHeader;
		protected virtual void OnChangingMetaGridColumnHeader(ChangingMetaGridColumnHeaderEventArgs e)
		{
			if (ChangingMetaGridColumnHeader != null)
				ChangingMetaGridColumnHeader(this, e);
		}
		#endregion

		#region GetMetaViewPreference
		/// <summary>
		/// Gets the meta view preference.
		/// </summary>
		/// <returns></returns>
		public McMetaViewPreference GetMetaViewPreference()
		{
			McMetaViewPreference pref = UserMetaViewPreference.Load(CurrentView, (int)DataContext.Current.CurrentUserId);

			if (pref == null || pref.Attributes.Count == 0)
			{
				McMetaViewPreference.CreateDefaultUserPreference(CurrentView);
				pref = UserMetaViewPreference.Load(CurrentView, (int)DataContext.Current.CurrentUserId);
			}

			return pref;
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, EventArgs e)
		{
			CHelper.AddToContext("MetaViewName", this.ViewName);

			MainGrid.RowDataBound += new GridViewRowEventHandler(MainGrid_RowDataBound);
			MainGrid.PageIndexChanging += new GridViewPageEventHandler(MainGrid_PageIndexChanging);
			MainGrid.Sorting += new GridViewSortEventHandler(MainGrid_Sorting);

			MainGrid.ShowCheckboxes = this.ShowCheckboxes;

			FillCustomColumnCollection();

			int customColumns = this.CustomColumns.Count;
			if (this.ShowCheckboxes)
				customColumns++;

			MainGridExt.ContextKey = UtilHelper.JsonSerialize(new MetaGridContextKey(this.ViewName, customColumns));
			MainGridExt.ServicePath = ResolveUrl("~/Apps/MetaUI/WebServices/MetaGridServerService.asmx");

			if (!IsPostBack)
				internalBind();
		}

		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			this.MainGridExt.StyleInfo = new Mediachase.Ibn.Web.UI.WebControls.GridStylesInfo(this.HeaderCssClass, this.GridCssClass, this.FooterCssClass, this.HeaderInnerCssClass, this.GridInnerCssClass, this.GridSelectedRowCssClass);
			this.MainGridExt.GetCssFromColumn = true;
			this.MainGridExt.PaddingWidth = 10;

			this.MainGrid.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");

			internalBindColumnsInfo();

			if (IsPostBack)
				this.internalBind();
		}
		#endregion

		#region --- Grid Events ---

		#region MainGrid_RowDataBound
		/// <summary>
		/// Handles the RowDataBound event of the MainGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
		void MainGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			McMetaViewPreference mvPref = GetMetaViewPreference();
			for (int i = 0; i < e.Row.Cells.Count; i++)
			{
				int index = i - this.CustomColumns.Count;
				if (this.ShowCheckboxes)
					index--;
				if (this.ShowCheckboxes && i == 0)
				{
					continue;
				}
				if (e.Row.RowType == DataControlRowType.DataRow) //if (e.Row.Cells[i].Controls[0] is Mediachase.Ibn.Web.UI.Modules.FieldControls.BaseType)
				{
					//e.Row.Cells[i].Attributes.Add("unselectable", "on");
					if (index <= this.VisibleMetaFields.Length)
						((BaseType)e.Row.Cells[i].Controls[0]).DataItem = (MetaObject)e.Row.DataItem;
					if (index < this.VisibleMetaFields.Length && index >= 0)
					{
						((BaseType)e.Row.Cells[i].Controls[0]).FieldName = this.VisibleMetaFields[index].Name;
					}
					if (e.Row.Cells[i].Controls[0] is CustomColumnBaseType)
					{
						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ViewName = this.ViewName;
						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).Place = this.PlaceName;
						if (this.ShowCheckboxes)
							index++;
						//if (index + this.CustomColumns.Count < this.CustomColumns.Count)
						if (this.ShowCheckboxes)
							((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ColumnId = this.CustomColumns[i - 1].Id;
						else
							((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ColumnId = this.CustomColumns[i].Id;
						//else if (index < this.CustomColumns.Count)
						//    ((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ColumnId = this.CustomColumns[index].Id;

					}
					if (i + 1 != e.Row.Cells.Count)
						e.Row.Cells[i].CssClass = this.GridInnerCssClass;
				}
			}

			GridViewRow row = e.Row;
			if (row.RowType == DataControlRowType.Header || row.RowType == DataControlRowType.DataRow)
				internalBindRowWidth(ref row);
			row.DataBind();
			if (row.RowType == DataControlRowType.DataRow)
			{
				//if (e.Row.DataItem != null && ((MetaObject)e.Row.DataItem).PrimaryKeyId.HasValue)
				//{
				//    if (this.ShowCheckboxes)
				//    {
				//        CheckBox cb = (CheckBox)row.Cells[0].Controls[0];
				//        cb.Attributes.Add(IbnGridView.primaryKeyIdAttr, ((MetaObject)e.Row.DataItem).PrimaryKeyId.Value.ToString());
				//    }

				//}
				if (e.Row.DataItem != null)
				{
					e.Row.Attributes.Add(IbnGridView.primaryKeyIdAttr, String.Format("{0}", MetaViewGroupUtil.CreateUniqueKey(CurrentView, (MetaObject)e.Row.DataItem, this.PrimaryGroupType, this.SecondaryGroupType)));
					if (this.ShowCheckboxes)
					{
						CheckBox cb = (CheckBox)row.Cells[0].Controls[0];
						cb.Attributes.Add(IbnGridView.primaryKeyIdAttr, String.Format("{0}", MetaViewGroupUtil.CreateUniqueKey(CurrentView, (MetaObject)e.Row.DataItem, this.PrimaryGroupType, this.SecondaryGroupType)));
					}
				}
			}

		}
		#endregion

		#region MainGrid_PageIndexChanging
		/// <summary>
		/// Handles the PageIndexChanging event of the MainGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewPageEventArgs"/> instance containing the event data.</param>
		void MainGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			MainGrid.PageIndex = e.NewPageIndex;
			internalBind();
		}
		#endregion

		#region ddPaging_SelectedIndexChanged
		/// <summary>
		/// Handles the SelectedIndexChanged event of the ddPaging control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void ddPaging_SelectedIndexChanged(object sender, EventArgs e)
		{
			McMetaViewPreference mvPref = GetMetaViewPreference();
			DropDownList ddPaging = (DropDownList)sender;

			if (Convert.ToInt32(ddPaging.SelectedValue) != -1)
			{
				//MainGrid.AllowPaging = true;
				MainGrid.PageSize = Convert.ToInt32(ddPaging.SelectedValue);
			}
			else
			{
				MainGrid.PageSize = 10000;
				//MainGrid.AllowPaging = false;
			}

			mvPref.Attributes.Set("PageSize", Convert.ToInt32(ddPaging.SelectedValue));
			Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.Ibn.Data.Services.Security.CurrentUserId, mvPref);

			//this.ForceUpdate = true;

			this.internalBind();
		}
		#endregion

		#region MainGrid_Sorting
		/// <summary>
		/// Handles the Sorting event of the MainGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
		void MainGrid_Sorting(object sender, GridViewSortEventArgs e)
		{
			#region Check for field being meta field
			bool _flag = false;
			foreach (MetaField field in CurrentView.AvailableFields)
			{
				if (field.Name == e.SortExpression)
				{
					_flag = true;
					break;
				}
			}
			if (!_flag)
				return;
			#endregion

			McMetaViewPreference mvPref = GetMetaViewPreference();
			//fix bug when user try to sort column which doesn't support sorting at MetaData level
			if (!MetaDataWrapper.IsSortable(e.SortExpression, mvPref))
				return;

			//fix issue with ajaxcontroltoolkit 3.5
			Thread.Sleep(750);

			if (mvPref.Sorting == null || mvPref.Sorting.Count == 0)
			{
				SortingElement new_sort = new SortingElement();
				mvPref.Sorting.Add(new_sort);
			}

			SortingElement sort = mvPref.Sorting[0];
			if (sort.Source.Equals(e.SortExpression))
			{
				if (sort.Type == SortingElementType.Asc)
					sort.Type = SortingElementType.Desc;
				else
					sort.Type = SortingElementType.Asc;
			}
			else
			{
				sort.Source = e.SortExpression;
				sort.Type = SortingElementType.Asc;
			}
		}
		#endregion

		#endregion

		#region --- CustomColumnMethods ---

		#region CustomColumnWidth
		/// <summary>
		/// Customs the width of the column.
		/// </summary>
		/// <param name="customColumn">The custom column.</param>
		/// <returns></returns>
		private int CustomColumnWidth(XPathNavigator customColumn)
		{
			int retVal = 0;

			foreach (XPathNavigator item in customColumn.SelectChildren("Item", string.Empty))
			{
				int width = 16; //default width

				string widthAttr = item.GetAttribute("width", string.Empty);

				string paddingLeft = item.GetAttribute("paddingLeft", string.Empty);
				string paddingRight = item.GetAttribute("paddingRight", string.Empty);

				if (widthAttr != string.Empty)
					width = Convert.ToInt32(widthAttr, CultureInfo.InvariantCulture);

				if (paddingLeft != string.Empty)
					width += Convert.ToInt32(paddingLeft, CultureInfo.InvariantCulture);

				if (paddingRight != string.Empty)
					width += Convert.ToInt32(paddingRight, CultureInfo.InvariantCulture);

				retVal += width;
			}

			return retVal;
		}
		#endregion

		#region FillCustomColumnCollection
		/// <summary>
		/// Fills the custom column collection.
		/// Read info about CustomColumns from XML
		/// </summary>
		private void FillCustomColumnCollection()
		{
			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, new Selector(CurrentView.MetaClassName, ViewName, PlaceName));
			XPathNavigator columns = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid/CustomColumns");

			if (columns != null)
			{
				foreach (XPathNavigator customColumn in columns.SelectChildren("Column", string.Empty))
				{
					string Type = customColumn.GetAttribute("type", string.Empty);
					string Width = customColumn.GetAttribute("width", string.Empty);
					string Title = customColumn.GetAttribute("title", string.Empty);
					string Id = customColumn.GetAttribute("id", string.Empty);

					if (Type != string.Empty)
					{
						int width = this.CustomColumnWidth(customColumn);

						if (Width != string.Empty)
							width = Convert.ToInt32(Width, CultureInfo.InvariantCulture);

						MetaGridCustomColumnInfo columnInfo = new MetaGridCustomColumnInfo(Type, Id, new ListColumnFactory(this.ViewName).GetCustomColumn(this.Page, CurrentView.MetaClass.Name, Type));
						columnInfo.Width = width;
						columnInfo.Title = CHelper.GetResFileString(Title);
						this.CustomColumns.Add(columnInfo);
					}
				}
			}
		}
		#endregion

		#endregion

		#region PagingJsOnKeyPressHandler
		/// <summary>
		/// return function for onkeypress handler in TextBox paging
		/// </summary>
		/// <param name="c">The c.</param>
		/// <returns></returns>
		private string PagingJsOnKeyPressHandler(WebControl c)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("if (Ext.EventObject.keyCode == 13) {");
			sb.AppendFormat("{0};return false;", this.Page.ClientScript.GetPostBackEventReference(c, string.Empty));
			sb.Append("}");

			return sb.ToString();
		}
		#endregion

		#region internalBindPaging
		/// <summary>
		/// Internals the bind paging.
		/// </summary>
		private void internalBindPaging()
		{
			if (MainGrid.BottomPagerRow != null)
			{
				if (MainGrid.BottomPagerRow.FindControl("ddPaging") != null)
				{
					DropDownList ddPaging = (DropDownList)MainGrid.BottomPagerRow.FindControl("ddPaging");

					if (MainGrid.PageSize < 10000)
						CHelper.SafeSelect(ddPaging, MainGrid.PageSize.ToString());
					else
						CHelper.SafeSelect(ddPaging, "-1");
				}

				if (MainGrid.BottomPagerRow.FindControl("tbCurrentPage") != null)
				{
					TextBox tb = (TextBox)MainGrid.BottomPagerRow.FindControl("tbCurrentPage");
					ImageButton btnRefresh = (ImageButton)MainGrid.BottomPagerRow.FindControl("btnRefresh");

					tb.Attributes.Add("onkeypress", this.PagingJsOnKeyPressHandler(btnRefresh));
					tb.Attributes.Add("autocomplete", "off");

					int pageIndex = MainGrid.PageIndex + 1;
					tb.Text = pageIndex.ToString();
				}

				if (MainGrid.PageIndex <= 0)
				{
					ImageButton imgPrev = (ImageButton)MainGrid.BottomPagerRow.FindControl("ImageButtonPrevious");
					imgPrev.CommandName = "";
					imgPrev.ImageUrl = "~/Images/IbnFramework/page-prev-disabled.gif";
					imgPrev.OnClientClick = "return false;";
					ImageButton imgFirst = (ImageButton)MainGrid.BottomPagerRow.FindControl("ImageButtonFirst");
					imgFirst.CommandName = "";
					imgFirst.ImageUrl = "~/Images/IbnFramework/page-first-disabled.gif";
					imgFirst.OnClientClick = "return false;";
				}

				if (MainGrid.PageIndex == MainGrid.PageCount - 1)
				{
					ImageButton imgNext = (ImageButton)MainGrid.BottomPagerRow.FindControl("ImageButtonNext");
					imgNext.CommandName = "";
					imgNext.ImageUrl = "~/Images/IbnFramework/page-next-disabled.gif";
					imgNext.OnClientClick = "return false;";
					ImageButton imgLast = (ImageButton)MainGrid.BottomPagerRow.FindControl("ImageButtonLast");
					imgLast.CommandName = "";
					imgLast.OnClientClick = "return false;";
					imgLast.ImageUrl = "~/Images/IbnFramework/page-last-disabled.gif";
				}

				if (MainGrid.PageCount == 1)
				{
					HtmlGenericControl divPaging = (HtmlGenericControl)MainGrid.BottomPagerRow.FindControl("pagingContainer");
					divPaging.Style.Add("display", "none");
				}
				else
				{
					HtmlGenericControl divPaging = (HtmlGenericControl)MainGrid.BottomPagerRow.FindControl("pagingContainer");
					divPaging.Style.Add("display", "block");
				}

				if (MainGrid.BottomPagerRow.FindControl("ltTotalElements") != null)
				{
					Literal lt = (Literal)MainGrid.BottomPagerRow.FindControl("ltTotalElements");
					lt.Text = String.Format("{0}.", this.Count.ToString()); //CHelper.GetResFileString("{IbnFramework.Common:
				}
				if (MainGrid.BottomPagerRow.FindControl("ltTotalPage") != null)
				{
					Literal ltPages = (Literal)MainGrid.BottomPagerRow.FindControl("ltTotalPage");
					ltPages.Text = String.Format("{0}", MainGrid.PageCount);
				}
			}
		}
		#endregion

		#region internalBindRowWidth
		/// <summary>
		/// Internals the width of the bind row.
		/// </summary>
		/// <param name="r">The r.</param>
		private void internalBindRowWidth(ref GridViewRow r)
		{
			//int counter = 0;
			//int notMetaFields = 0;

			if (this.ShowCheckboxes)
			{
				r.Cells[0].Width = 22;
				r.Cells[0].CssClass = this.GridInnerCssClass;
				//counter++;
				//notMetaFields++;
			}

			//for (int i = 0; i < this.CustomColumns.Count; i++)
			//{
			//    //r.Cells[counter + i].HeaderText = CHelper.GetResFileString(this.CustomColumns[i].Title);
			//    r.Cells[counter + i].Width = this.CustomColumns[i].Width;

			//    if (this.CustomColumns[i].Width == 0)
			//        r.Cells[counter + i].CssClass = "";

			//    notMetaFields++;
			//}

			//counter += this.CustomColumns.Count;

			//foreach (MetaField field in this.VisibleMetaFields)
			//{
			//    int _width = GetMetaViewPreference().GetMetaFieldWidth(counter - notMetaFields, 100);
			//    if (_width == 0)
			//        _width = 100;

			//    r.Cells[counter].Width = _width;

			//    if (r.Cells[counter].Width.Value == 0)
			//        r.Cells[counter].CssClass = "";

			//    counter++;
			//}
		}
		#endregion

		#region internalBindHeader
		/// <summary>
		/// Internals the bind header.
		/// </summary>
		private void internalBindHeader()
		{
			McMetaViewPreference mvPref = GetMetaViewPreference();
			int counter = 0;
			int notMetaFields = 0;

			for (int i = 0; i < this.CustomColumns.Count; i++)
			{
				MainGrid.Columns[i].HeaderText = CHelper.GetResFileString(this.CustomColumns[i].Title);
				MainGrid.Columns[i].ItemStyle.Width = this.CustomColumns[i].Width;
				MainGrid.Columns[i].HeaderStyle.Width = this.CustomColumns[i].Width;
				notMetaFields++;
			}

			counter += this.CustomColumns.Count;

			foreach (MetaField field in this.VisibleMetaFields)
			{
				int _width = GetMetaViewPreference().GetMetaFieldWidth(counter - notMetaFields, 100);
				if (_width == 0)
					_width = 100;
				MainGrid.Columns[counter].ItemStyle.Width = _width;
				MainGrid.Columns[counter].HeaderStyle.Width = _width;

				//add sorting only for metaFileds that supports sorting at MetaData level
				if (MetaDataWrapper.IsSortable(field))
				{
					MainGrid.Columns[counter].SortExpression = field.Name;
				}

				DataControlField controlField = MainGrid.Columns[counter];
				controlField.HeaderText = CHelper.GetResFileString(field.FriendlyName);

				// Raising event
				ChangingMetaGridColumnHeaderEventArgs e = new ChangingMetaGridColumnHeaderEventArgs(controlField, field);
				OnChangingMetaGridColumnHeader(e);

				#region Sorting header text (arrows up/down)
				if (mvPref.Sorting != null && mvPref.Sorting.Count > 0 && mvPref.Sorting[0].Source == field.Name)
				{
					if (mvPref.Sorting[0].Type == SortingElementType.Asc)
					{
						MainGrid.Columns[counter].HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_asc.gif"));
					}
					else
					{
						MainGrid.Columns[counter].HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_desc.gif"));
					}
				}
				#endregion

				counter++;
			}
		}
		#endregion

		#region internalBind
		/// <summary>
		/// Internals the bind.
		/// </summary>
		private void internalBind()
		{
			MainGrid.Columns.Clear();

			McMetaViewPreference mvPref = GetMetaViewPreference();
			MainGrid.AllowPaging = true;

			if (mvPref.Attributes["PageSize"] != null)
			{
				int pageSize = Convert.ToInt32(mvPref.Attributes.GetValue("PageSize").ToString());
				if (pageSize != -1)
				{
					MainGrid.PageSize = pageSize;

				}
				else
				{
					MainGrid.PageSize = 10000;
					//MainGrid.AllowPaging = false;
				}

				//CHelper.SafeSelect(ddPaging, mvPref.Attributes.Get("PageSize").ToString());
			}
			else
			{
				MainGrid.PageSize = 10;
				mvPref.Attributes.Set("PageSize", 10);
				Mediachase.Ibn.Core.UserMetaViewPreference.Save((int)DataContext.Current.CurrentUserId, mvPref);
			}

			int width = 0;

			if (this.ShowCheckboxes)
				width += 22 + 7;

			#region Check Additional columns from xml

			foreach (MetaGridCustomColumnInfo customColumn in this.CustomColumns)
			{
				MainGrid.Columns.Add(customColumn.Column);
				width += customColumn.Width + 7;
			}

			#endregion


			int counter = 0;
			foreach (MetaField field in this.VisibleMetaFields)
			{
				int cellWidth = 0;

				if (PlaceName == String.Empty)
					MainGrid.Columns.Add((new ListColumnFactory(this.ViewName)).GetColumn(this.Page, field));
				else
					MainGrid.Columns.Add((new ListColumnFactory(this.ViewName)).GetColumn(this.Page, field, PlaceName));


				cellWidth = mvPref.GetMetaFieldWidth(counter, 100);
				if (cellWidth == 0)
					cellWidth = 100;
				width += cellWidth;

				counter++;
			}

			width += this.VisibleMetaFields.Length * 7;


			MainGrid.Width = width;

			#region Adding PrimaryKeyColumn
			MainGrid.Columns.Add((new ListColumnFactory(this.ViewName)).GetCssColumn(this.Page, CurrentView.MetaClass.Name));
			#endregion

			internalBindHeader();

			FilterElement fe = null;

			if (this.SearchKeyword != string.Empty)
			{
				fe = ListManager.CreateFilterByKeyword(mvPref.MetaView.MetaClass, this.SearchKeyword);
				mvPref.Filters.Add(fe);
			}

			MetaObject[] list = CurrentView.List(mvPref);

			if (fe != null)
				mvPref.Filters.Remove(fe);

			if (CurrentView.PrimaryGroupBy == null && CurrentView.SecondaryGroupBy == null)
			{
				MainGridExt.IsEmpty = (list.Length == 0);

				if (list.Length == 0)
				{
					list = new MetaObject[] { new MetaObject(CurrentView.MetaClass) };
				}

				MainGrid.DataSource = list;
			}
			else
			{
				if (CurrentView.SecondaryGroupBy != null)
					list = MetaViewGroupUtil.ExcludeCollapsed(MetaViewGroupByType.Secondary, CurrentView.SecondaryGroupBy, CurrentView.PrimaryGroupBy, mvPref, list);

				list = MetaViewGroupUtil.ExcludeCollapsed(MetaViewGroupByType.Primary, CurrentView.PrimaryGroupBy, null, mvPref, list);

				MainGridExt.IsEmpty = (list.Length == 0);

				if (list.Length == 0)
				{
					list = new MetaObject[] { new MetaObject(CurrentView.MetaClass) };
				}

				MainGrid.DataSource = list;
			}

			this.Count = list.Length;
			if (MainGridExt.IsEmpty)
				this.Count = 0;
			MainGrid.DataBind();

			internalBindPaging();

			if (list.Length == 0)
				MainGrid.CssClass = "serverGridBodyEmpty";
		}
		#endregion

		#region internalBindColumnsInfo
		/// <summary>
		/// Internals the bind columns info.
		/// </summary>
		private void internalBindColumnsInfo()
		{
			List<GridViewColumnInfo> list = new List<GridViewColumnInfo>();
			MainGridExt.ColumnsInfo.Clear();
			int counter = 0;

			if (this.ShowCheckboxes)
			{
				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(22, false, false));
			}

			for (int i = 0; i < this.CustomColumns.Count; i++)
			{
				//ToDo: IsSystem
				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(this.CustomColumns[i].Width, false, false, true));
			}

			foreach (MetaField field in this.VisibleMetaFields)
			{
				int _width = GetMetaViewPreference().GetMetaFieldWidth(counter, 100);
				if (_width == 0)
					_width = 100;

				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(_width, true, true));

				counter++;
			}

			//MainGridExt.ColumnsInfo = list;
		}
		#endregion

		#region GetCheckedCollection
		/// <summary>
		/// Gets the checked collection.
		/// </summary>
		/// <returns></returns>
		protected string GetCheckedCollection()
		{
			StringBuilder sb = new StringBuilder();

			if (!this.ShowCheckboxes)
				return string.Empty;

			if (MainGrid.Rows.Count > 0)
			{
				for (int i = 0; i < MainGrid.Rows.Count; i++)
				{
					if (MainGrid.Rows[i].RowType == DataControlRowType.DataRow)
					{
						if (MainGrid.Rows[i].Cells.Count == 0)
							throw new ArgumentException("Grid.Cells.Count == 0");

						if (MainGrid.Rows[i].Cells[0].Controls.Count == 0)
							throw new ArgumentException("Grid.Cells[0].Controls.Count == 0");

						CheckBox cb = (CheckBox)MainGrid.Rows[i].Cells[0].Controls[0];

						if (cb != null && cb.Checked)
						{
							if (cb.Attributes[IbnGridView.primaryKeyIdAttr] != null)
							{
								sb.AppendFormat("{0};", cb.Attributes[IbnGridView.primaryKeyIdAttr]);
							}
							//if (cb.Checked)
						}
					}
				}
			}

			return sb.ToString();
		}
		#endregion

		#region --- static members ---
		/// <summary>
		/// Gets the checked collection.
		/// </summary>
		/// <param name="CurrentPage">The current page.</param>
		/// <param name="GridId">The grid id.</param>
		/// <returns></returns>
		public static string[] GetCheckedCollection(Page CurrentPage, string GridId)
		{
			MetaGridServer grid = null;
			grid = GetMetaGridFromCollection(CurrentPage.Controls, GridId);

			if (grid != null)
			{
				//return grid.ctrlJsGrid.hfGridCheckedValues.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				//return grid.hfCheckedCollection.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				return grid.GetCheckedCollection().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			}

			return null;
		}

		/// <summary>
		/// Gets the meta grid from collection.
		/// </summary>
		/// <param name="coll">The coll.</param>
		/// <param name="GridId">The grid id.</param>
		/// <returns></returns>
		public static MetaGridServer GetMetaGridFromCollection(ControlCollection coll, string GridId)
		{
			MetaGridServer retVal = null;
			foreach (Control c in coll)
			{
				if (c is MetaGridServer && (GridId != string.Empty && c.ID == GridId))
				{
					retVal = (MetaGridServer)c;
					break;

				}
				else
				{
					retVal = GetMetaGridFromCollection(c.Controls, GridId);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion

	}


}