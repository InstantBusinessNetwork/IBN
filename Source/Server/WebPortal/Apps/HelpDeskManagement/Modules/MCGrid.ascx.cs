using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.XPath;

using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;
using Mediachase.UI.Web.Apps.MetaUI.Grid;

namespace Mediachase.Ibn.Web.UI
{
	#region class: McGridCustomColumnInfo
	/// <summary>
	/// Description for custom columns (like default column)
	/// </summary>
	class McGridCustomColumnInfo
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

		#region prop: DataField
		private string _dataField = string.Empty;

		/// <summary>
		/// Gets or sets the dataField.
		/// </summary>
		/// <value>The dataField.</value>
		public string DataField
		{
			get { return _dataField; }
			set { _dataField = value; }
		}
		#endregion

		#region prop: IsSystem
		private bool _isSystem;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is system.
		/// </summary>
		/// <value><c>true</c> if this instance is system; otherwise, <c>false</c>.</value>
		public bool IsSystem
		{
			get { return _isSystem; }
			set { _isSystem = value; }
		}
		#endregion

		#region .ctor
		public McGridCustomColumnInfo()
		{
		}

		public McGridCustomColumnInfo(string Type)
			: this()
		{
			this.Type = Type;
		}

		public McGridCustomColumnInfo(string Type, string Id)
			: this(Type)
		{
			this.Id = Id;
		}

		public McGridCustomColumnInfo(string Type, string Id, DataControlField Column)
			: this(Type, Id)
		{
			this.Column = Column;
		}

		public McGridCustomColumnInfo(string Type, string Id, DataControlField Column, string dataField)
			: this(Type, Id, Column)
		{
			this.DataField = dataField;
		}

		public McGridCustomColumnInfo(string Type, string Id, bool IsSystem)
			: this(Type, Id)
		{
			this.IsSystem = IsSystem;
		}

		public McGridCustomColumnInfo(string Type, string Id, DataControlField Column, string dataField, bool IsSystem)
			: this(Type, Id, Column, dataField)
		{
			this.IsSystem = IsSystem;
		}
		#endregion
	}
	#endregion

	#region class: BoundColumnTemplate
	/// <summary>
	/// Description for custom columns (like default column)
	/// </summary>
	class BoundColumnTemplate : ITemplate
	{
		#region prop: FieldName
		private string _fieldName = string.Empty;

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string FieldName
		{
			get { return _fieldName; }
			set { _fieldName = value; }
		}
		#endregion

		#region .ctor
		public BoundColumnTemplate(string fieldName)
		{
			_fieldName = fieldName;
		}
		#endregion

		#region ITemplate Members

		public void InstantiateIn(Control container)
		{
			Label lbl = new Label();
			lbl.DataBinding += new EventHandler(lbl_DataBinding);
			container.Controls.Add(lbl);
		}

		void lbl_DataBinding(object sender, EventArgs e)
		{
			Label lbl = (Label)sender;
			if (lbl.NamingContainer is IDataItemContainer)
			{
				IDataItemContainer container = (IDataItemContainer)lbl.NamingContainer;
				lbl.Text = DataBinder.Eval(container.DataItem, FieldName).ToString();
			}
		}

		#endregion
	}
	#endregion

	#region class: ChangingMCGridColumnHeaderEventArgs
	public class ChangingMCGridColumnHeaderEventArgs : EventArgs
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

		#region prop: FieldName
		private string _fieldName;
		public string FieldName
		{
			get
			{
				return _fieldName;
			}
		}
		#endregion

		#region .ctor
		public ChangingMCGridColumnHeaderEventArgs(DataControlField controlField, string fieldName)
		{
			_controlField = controlField;
			_fieldName = fieldName;
		}
		#endregion
	}

	public delegate void ChangingMCGridColumnHeaderEventHandler(object sender, ChangingMCGridColumnHeaderEventArgs e);
	#endregion

	public partial class MCGrid : System.Web.UI.UserControl
	{
		private readonly string classNameKey = "ClassName";
		private readonly string viewNameKey = "ViewName";
		private readonly string placeNameKey = "PlaceName";
		private readonly string primaryKeyIdKey = "PrimaryKeyId";
		private readonly string weightSortKey = "WeightSort";

		public const string PageSizePropertyKey = "PageSize";
		public const string SortingPropertyKey = "Sorting";

		private Mediachase.IBN.Business.UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		#region styles: Default constants
		private readonly string defaultHeaderCssClass = "serverGridHeader";
		private readonly string defaultGridCssClass = "serverGridBody";
		private readonly string defaultFooterCssClass = "serverGridFooter";
		private readonly string defaultHeaderInnerCssClass = "serverGridHeaderInner";
		private readonly string defaultGridInnerCssClass = "serverGridInner";
		private readonly string defaultGridInnerCssClassMCGrid = "serverGridInnerMCGrid";
		private readonly string defaultGridSelectedRowCssClass = "serverGridSelectedRow";
		private readonly string defaultCssTemplateUrlKey = "serverGridCssTemplateUrl";
		#endregion

		#region prop: GetCssFromColumn

		/// <summary>
		/// Gets or sets a value indicating whether [get CSS from column].
		/// </summary>
		/// <value><c>true</c> if [get CSS from column]; otherwise, <c>false</c>.</value>
		public bool GetCssFromColumn
		{
			get
			{
				if (ViewState["__GetCssFromColumn"] != null)
					return Convert.ToBoolean(ViewState["__GetCssFromColumn"].ToString());

				return false;
			}
			set
			{
				ViewState["__GetCssFromColumn"] = value;
			}
		}
		#endregion			

		#region prop: DataSource
		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		private object _dataSource;
		public object DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
			}
		}
		#endregion

		#region prop: ClassName
		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public string ClassName
		{
			get
			{
				if (ViewState[classNameKey] != null)
					return (string)ViewState[classNameKey];
				else
					return String.Empty;
			}
			set
			{
				ViewState[classNameKey] = value;
			}
		}
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
				if (ViewState[viewNameKey] != null)
					return (string)ViewState[viewNameKey];
				else
					return String.Empty;
			}
			set
			{
				ViewState[viewNameKey] = value;
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

		#region prop: WeightSort
		/// <summary>
		/// Gets or sets a value indicating whether [weight sort].
		/// </summary>
		/// <value><c>true</c> if [weight sort]; otherwise, <c>false</c>.</value>
		public bool WeightSort
		{
			get
			{
				if (ViewState[weightSortKey] != null)
					return (bool)ViewState[weightSortKey];
				else
					return false;
			}
			set
			{
				ViewState[weightSortKey] = value;
			}
		}
		#endregion

		#region prop: PrimaryKeyIdField
		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public string PrimaryKeyIdField
		{
			get
			{
				if (ViewState[primaryKeyIdKey] != null)
					return (string)ViewState[primaryKeyIdKey];
				else
					return String.Empty;
			}
			set
			{
				ViewState[primaryKeyIdKey] = value;
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

		#region prop: DashboardMode
		/// <summary>
		/// Gets or sets a value indicating whether [dashboard mode].
		/// </summary>
		/// <value><c>true</c> if [dashboard mode]; otherwise, <c>false</c>.</value>
		public bool DashboardMode
		{
			get
			{
				if (ViewState["_DashboardMode"] != null)
					return Convert.ToBoolean(ViewState["_DashboardMode"].ToString());

				return false;
			}
			set
			{
				ViewState["_DashboardMode"] = value;
			}
		}
		#endregion

		#region prop: PageIndex
		public int PageIndex
		{
			get
			{
				return MainGrid.PageIndex;
			}
		}
		#endregion

		#region prop: PageSize
		public int PageSize
		{
			get
			{
				if (ViewState["_PageSize"] != null)
					return Convert.ToInt32(ViewState["_PageSize"].ToString());

				return 0;
			}
			set
			{
				ViewState["_PageSize"] = value;
			}
		}
		#endregion

		#region prop: ShowPaging
		public bool ShowPaging
		{
			get
			{
				if (ViewState["_ShowPaging"] != null)
					return Convert.ToBoolean(ViewState["_ShowPaging"].ToString());

				return true;
			}
			set
			{
				ViewState["_ShowPaging"] = value;
			}
		}
		#endregion

		#region prop: PostBackRender
		public bool PostBackRender
		{
			get
			{
				if (ViewState["_PostBackRender"] != null)
					return Convert.ToBoolean(ViewState["_PostBackRender"].ToString());

				return false;
			}
			set
			{
				ViewState["_PostBackRender"] = value;
			}
		}
		#endregion

		#region prop: InnerGrid
		public IbnGridView InnerGrid
		{
			get
			{
				return this.MainGrid;
			}
		}
		#endregion

		#region prop: CustomColumns
		private List<McGridCustomColumnInfo> _customColumns = null;
		/// <summary>
		/// Gets the custom columns.
		/// </summary>
		/// <value>The custom columns.</value>
		private List<McGridCustomColumnInfo> CustomColumns
		{
			get
			{
				if (_customColumns == null)
					_customColumns = new List<McGridCustomColumnInfo>();

				return _customColumns;
			}
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

		#region prop: CssTemplateUrl
		public string CssTemplateUrl
		{
			get
			{
				if (ViewState[defaultCssTemplateUrlKey] != null)
					return (string)ViewState[defaultCssTemplateUrlKey];
				else
					return "~/Apps/MetaUI/Primitives/RowCssClass.Grid.ascx";
			}
			set
			{
				ViewState[defaultCssTemplateUrlKey] = value;
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
				{
					if (this.ShowCheckboxes)
						return defaultGridInnerCssClass;
					else
						return defaultGridInnerCssClassMCGrid;
				}

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
				return Mediachase.Ibn.Core.MetaViewGroupUtil.keyValueNotDefined;
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
				return Mediachase.Ibn.Core.MetaViewGroupUtil.keyValueNotDefined;
			}
		}
		#endregion

		#region event: OnChangingMCGridColumnHeader
		public event ChangingMCGridColumnHeaderEventHandler ChangingMCGridColumnHeader;
		protected virtual void OnChangingMCGridColumnHeader(ChangingMCGridColumnHeaderEventArgs e)
		{
			if (ChangingMCGridColumnHeader != null)
				ChangingMCGridColumnHeader(this, e);
		}
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (this.DashboardMode)
				this.MainGrid.ResetTemplate = true;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			//MainGrid.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
			MainGrid.RowDataBound += new GridViewRowEventHandler(MainGrid_RowDataBound);
			MainGrid.PageIndexChanging += new GridViewPageEventHandler(MainGrid_PageIndexChanging);
			MainGrid.Sorting += new GridViewSortEventHandler(MainGrid_Sorting);

			MainGrid.ShowCheckboxes = this.ShowCheckboxes;

			if (this.PageSize != 0)
				SetPageSize(this.PageSize);

			if (!IsPostBack || PostBackRender)
			{
				FillCustomColumnCollection();
				internalBind();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			this.MainGridExt.StyleInfo = new Mediachase.Ibn.Web.UI.WebControls.GridStylesInfo(this.HeaderCssClass, this.GridCssClass, this.FooterCssClass, this.HeaderInnerCssClass, this.GridInnerCssClass, this.GridSelectedRowCssClass);
			this.MainGridExt.DashboardMode = this.DashboardMode;
			this.MainGridExt.GetCssFromColumn = this.GetCssFromColumn;

			if (IsPostBack)
				FillCustomColumnCollection();

			internalBindColumnsInfo();

			if (IsPostBack)
				this.internalBind();
		}
		#endregion

		#region PageSize Get/Set

		#region SetPageSize
		private void SetPageSize(int value)
		{
			if (value <= 0)
				MainGrid.PageSize = 10000;
			else
				MainGrid.PageSize = value;
			if (!this.DashboardMode && this.ShowPaging)
				_pc[GetPropertyKey(PageSizePropertyKey)] = value.ToString();
		}
		#endregion

		#region GetPageSize
		private int GetPageSize()
		{
			if (!this.DashboardMode && this.ShowPaging)
			{
				if (_pc[GetPropertyKey(PageSizePropertyKey)] != null)
				{
					int pageSize = Convert.ToInt32(_pc[GetPropertyKey(PageSizePropertyKey)].ToString());
					if (pageSize != -1)
						return pageSize;
					else
						return 10000;
				}
				else
				{
					_pc[GetPropertyKey(PageSizePropertyKey)] = "10";
					return 10;
				}
			}
			return MainGrid.PageSize;
		}
		#endregion 

		#endregion

		#region FillCustomColumnCollection
		/// <summary>
		/// Fills the custom column collection.
		/// Read info about CustomColumns from XML
		/// </summary>
		private void FillCustomColumnCollection()
		{
			this.CustomColumns.Clear();
			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, new Selector(ClassName, ViewName, PlaceName));
			XPathNavigator columns = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid/CustomColumns");

			if (columns == null)
				return;

			string pKeyId = columns.GetAttribute("primaryKey", string.Empty);
			if (!String.IsNullOrEmpty(pKeyId))
				PrimaryKeyIdField = pKeyId;

			string cssTemplateUrl = columns.GetAttribute("cssTemplateUrl", string.Empty);
			if (!String.IsNullOrEmpty(cssTemplateUrl))
				CssTemplateUrl = cssTemplateUrl;

			foreach (XPathNavigator customColumn in columns.SelectChildren("Column", string.Empty))
			{
				string type = customColumn.GetAttribute("type", string.Empty);
				string width = customColumn.GetAttribute("width", string.Empty);
				string title = customColumn.GetAttribute("title", string.Empty);
				string id = customColumn.GetAttribute("id", string.Empty);
				string dataField = customColumn.GetAttribute("dataField", string.Empty);
				string templateUrl = customColumn.GetAttribute("templateUrl", string.Empty);
				string sortExpression = customColumn.GetAttribute("sortExpression", string.Empty);
				string isSystem = customColumn.GetAttribute("isSystem", string.Empty);
				string cssClass = customColumn.GetAttribute("cssClass", string.Empty);
				string align = customColumn.GetAttribute("align", string.Empty);

				if (type != string.Empty)
				{
					if (type == "ColumnsActions")
					{
						XPathNodeIterator iterator = customColumn.SelectChildren("Item", string.Empty);
						foreach (XPathNavigator actionItem in iterator)
						{
							string commandName = actionItem.GetAttribute("commandName", string.Empty);
							if (!String.IsNullOrEmpty(commandName))
								CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.PlaceName, commandName);
						}
					}

					int widthReal = this.CustomColumnWidth(customColumn);
					bool isSystemValue = false;

					if (!String.IsNullOrEmpty(isSystem))
					{
						isSystemValue = Convert.ToBoolean(isSystem, CultureInfo.InvariantCulture);
					}

					if (!String.IsNullOrEmpty(width))
					{
						Unit tempWidthUnit = Unit.Parse(width, CultureInfo.InvariantCulture);

						if (tempWidthUnit.Type == UnitType.Percentage)
						{
							MainGridExt.PercentWidth = true;
							if (isSystemValue)
								throw new ArgumentException("Column marked as System and UnitType.Percentage");
						}
						else if (tempWidthUnit.Type == UnitType.Pixel)
						{
							isSystemValue = true;
						}

						int tempWidth = Convert.ToInt32(tempWidthUnit.Value, CultureInfo.InvariantCulture); //Convert.ToInt32(width, CultureInfo.InvariantCulture);
						if (tempWidth > widthReal)
							widthReal = tempWidth;

					}

					TemplateField field = new TemplateField();
					if (!String.IsNullOrEmpty(templateUrl))
						field.ItemTemplate = this.Page.LoadTemplate(templateUrl);
					else
						field.ItemTemplate = new BoundColumnTemplate(dataField);

					if (!String.IsNullOrEmpty(cssClass))
					{
						field.ItemStyle.CssClass = cssClass;
					}

					if (!String.IsNullOrEmpty(align))
					{
						try
						{
							HorizontalAlign alignValue = (HorizontalAlign)Enum.Parse(typeof(HorizontalAlign), align);
							field.ItemStyle.HorizontalAlign = alignValue;
						}
						catch (Exception ex)
						{
							Trace.Write(String.Format("Exception during parsing xml, attribute align:'{1}', exception: '{0}'", ex.Message, align));
						}
					}

					//field.ItemStyle.

					field.HeaderText = title;
					if (!String.IsNullOrEmpty(sortExpression))
						field.SortExpression = sortExpression;

					McGridCustomColumnInfo columnInfo = new McGridCustomColumnInfo(type, id, field, dataField, isSystemValue);
					columnInfo.Width = widthReal;
					columnInfo.Title = CHelper.GetResFileString(title);

					this.CustomColumns.Add(columnInfo);
				}
			}

		}
		#endregion

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
				{
					Unit widthAttrValue = Unit.Parse(widthAttr, CultureInfo.InvariantCulture);
					width = Convert.ToInt32(widthAttrValue.Value, CultureInfo.InvariantCulture);
					if (widthAttrValue.Type == UnitType.Percentage)
						MainGridExt.PercentWidth = true;
				}

				if (paddingLeft != string.Empty)
					width += Convert.ToInt32(paddingLeft, CultureInfo.InvariantCulture);

				if (paddingRight != string.Empty)
					width += Convert.ToInt32(paddingRight, CultureInfo.InvariantCulture);

				retVal += width;
			}

			return retVal;
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

			if (this.ShowCheckboxes)
			{
				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(22, false, false, true));
			}

			for (int i = 0; i < this.CustomColumns.Count; i++)
			{
				int width = this.CustomColumns[i].Width;
				if (width == 0)
					width = 100;
				bool isSortable = !String.IsNullOrEmpty(this.CustomColumns[i].Column.SortExpression);

				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(width, true, isSortable, this.CustomColumns[i].IsSystem));
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

			if (!this.DashboardMode)
				MainGrid.AllowPaging = true;

			if (!ShowPaging)
			{
				MainGridExt.BottomHeight = 1;
				SetPageSize(-1);
			}
			else
			{
				MainGridExt.BottomHeight = 30;
				MainGrid.PageSize = GetPageSize();
			}

			int width = 0;

			if (this.ShowCheckboxes)
				width += 22 + 7;

			foreach (McGridCustomColumnInfo customColumn in this.CustomColumns)
			{
				MainGrid.Columns.Add(customColumn.Column);
				width += customColumn.Width + 7;
			}

			if (!this.DashboardMode)
				MainGrid.Width = width;

			#region Adding CssColumn
			TemplateField retVal = new TemplateField();
			retVal.ItemTemplate = this.Page.LoadTemplate(CssTemplateUrl);
			MainGrid.Columns.Add(retVal);
			#endregion



			internalBindHeader();

			//TODO - отвязать код от DataView
			if (DataSource is DataView)
			{
				MainGridExt.IsEmpty = (((DataView)DataSource).Count == 0);
				this.Count = ((DataView)DataSource).Count;
				if (MainGridExt.IsEmpty)
				{
					DataTable dt = ((DataView)DataSource).Table;
					DataRow dr = dt.NewRow();
					dt.Rows.Add(dr);
					DataView dv = dt.DefaultView;
					dv.RowFilter = "";
					DataSource = dv;
				}
			}

			if (DataSource is DataView)
			{
				string sortExpression = String.Empty;
				if (WeightSort)
					sortExpression += "Weight";
				if (_pc[GetPropertyKey(SortingPropertyKey)] != null)
				{
					sortExpression += (String.IsNullOrEmpty(sortExpression)) ? _pc[GetPropertyKey(SortingPropertyKey)].ToString() : (", " + _pc[GetPropertyKey(SortingPropertyKey)].ToString());
				}

				((DataView)DataSource).Sort = sortExpression;
			}
			if (this.DashboardMode && this.Count <= 10)
			{
				MainGridExt.BottomHeight = 1;
				SetPageSize(-1);
				MainGrid.AllowPaging = false;
			}

			MainGrid.DataSource = DataSource;

			MainGrid.DataBind();

			#region Appply paging for dashboard
			if (this.DashboardMode)
			{
				MainGridExt.BottomHeight = 16;

				if (MainGrid.PageCount == 1)
					MainGridExt.BottomHeight = 1;
				//HtmlControl c = (HtmlControl)MainGrid.BottomPagerRow.FindControl("pagingContainer");
				//c.Style.Add(HtmlTextWriterStyle.Position, "static");
				//c.Style.Add("float", "right");

				//MainGridExt.ServicePath = "debug";
			}
			#endregion

			internalBindPaging();

			//if (MainGridExt.IsEmpty)
			//    MainGrid.CssClass = "serverGridBodyEmpty";
		}
		#endregion

		#region internalBindHeader
		/// <summary>
		/// Internals the bind header.
		/// </summary>
		private void internalBindHeader()
		{
			for (int i = 0; i < this.CustomColumns.Count; i++)
			{
				DataControlField controlField = MainGrid.Columns[i];

				controlField.HeaderText = CHelper.GetResFileString(this.CustomColumns[i].Title);
				//if (this.CustomColumns[i].Type == "ColumnsActions")
				//{
				//    controlField.ItemStyle.Width = this.CustomColumns[i].Width + 7;
				//    controlField.HeaderStyle.Width = this.CustomColumns[i].Width + 7;
				//}
				//else
				//{
				if (this.DashboardMode)
				{
					if (!this.CustomColumns[i].IsSystem)
					{
						controlField.ItemStyle.Width = Unit.Percentage(Convert.ToDouble(this.CustomColumns[i].Width.ToString()));
						controlField.HeaderStyle.Width = Unit.Percentage(Convert.ToDouble(this.CustomColumns[i].Width.ToString()));
					}
					else
					{
						controlField.ItemStyle.Width = Unit.Pixel(Convert.ToInt32(this.CustomColumns[i].Width.ToString()));
						controlField.HeaderStyle.Width = Unit.Pixel(Convert.ToInt32(this.CustomColumns[i].Width.ToString()));
					}
				}
				else
				{
					controlField.ItemStyle.Width = this.CustomColumns[i].Width;
					controlField.HeaderStyle.Width = this.CustomColumns[i].Width;
				}
				//}
				controlField.SortExpression = this.CustomColumns[i].Column.SortExpression;

				// Raising event
				ChangingMCGridColumnHeaderEventArgs e = new ChangingMCGridColumnHeaderEventArgs(controlField, this.CustomColumns[i].DataField);
				OnChangingMCGridColumnHeader(e);

				#region Sorting header text (arrows up/down)
				if (_pc[GetPropertyKey(SortingPropertyKey)] != null && _pc[GetPropertyKey(SortingPropertyKey)].ToString() == MainGrid.Columns[i].SortExpression)
				{
					MainGrid.Columns[i].HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_asc.gif"));
				}
				else if (_pc[GetPropertyKey(SortingPropertyKey)] != null && _pc[GetPropertyKey(SortingPropertyKey)].ToString() == MainGrid.Columns[i].SortExpression + " DESC")
				{
					MainGrid.Columns[i].HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_desc.gif"));
				}
				#endregion
			}
		}
		#endregion

		#region internalBindPaging
		/// <summary>
		/// Internals the bind paging.
		/// </summary>
		private void internalBindPaging()
		{
			if (MainGrid.BottomPagerRow != null && !MainGrid.ResetTemplate)
			{
				if (MainGrid.BottomPagerRow.FindControl("DivPaging") != null)
				{
					HtmlGenericControl div = (HtmlGenericControl)MainGrid.BottomPagerRow.FindControl("DivPaging");
					if (MainGridExt.BottomHeight == 1)
						div.Style.Add("display", "none");
					else
						div.Style.Add("display", "block");

					if (this.DashboardMode)
					{
						div.Attributes.Add("class", "serverGridFooter");
						div.Style.Add(HtmlTextWriterStyle.BorderWidth, "0px");
					}
				}
				if (MainGrid.BottomPagerRow.FindControl("ddPaging") != null)
				{
					DropDownList ddPaging = (DropDownList)MainGrid.BottomPagerRow.FindControl("ddPaging");

					if (MainGrid.PageSize < 10000)
						CHelper.SafeSelect(ddPaging, MainGrid.PageSize.ToString());
					else
						CHelper.SafeSelect(ddPaging, "100");
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

				if (MainGrid.BottomPagerRow.FindControl("ltTotalElements2") != null)
				{
					Literal lt = (Literal)MainGrid.BottomPagerRow.FindControl("ltTotalElements2");
					lt.Text = String.Format(lt.Text, this.Count.ToString());//String.Format("{0}.", this.Count.ToString()); //CHelper.GetResFileString("{IbnFramework.Common:
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

		#region --- Grid Events ---

		#region MainGrid_RowDataBound
		/// <summary>
		/// Handles the RowDataBound event of the MainGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
		void MainGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			for (int i = 0; i < e.Row.Cells.Count; i++)
			{
				if (this.ShowCheckboxes && i == 0)
				{
					continue;
				}

				int index = i;
				if (this.ShowCheckboxes)
					index--;

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					e.Row.Cells[i].Attributes.Add("unselectable", "on");

					if (e.Row.Cells[i].Controls.Count > 0 && e.Row.Cells[i].Controls[0] is CustomColumnBaseType)
					{
						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ClassName = this.ClassName;
						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ViewName = this.ViewName;
						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).Place = this.PlaceName;
						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).PrimaryKeyIdField = this.PrimaryKeyIdField;

						((CustomColumnBaseType)e.Row.Cells[i].Controls[0]).ColumnId = this.CustomColumns[index].Id;
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
				if (e.Row.DataItem != null && !String.IsNullOrEmpty(PrimaryKeyIdField))
				{
					string keyValue = DataBinder.Eval(e.Row.DataItem, PrimaryKeyIdField).ToString();

					if (this.ShowCheckboxes)
					{
						CheckBox cb = (CheckBox)row.Cells[0].Controls[0];
						cb.Attributes.Add(IbnGridView.primaryKeyIdAttr, keyValue);
					}

					e.Row.Attributes.Add(IbnGridView.primaryKeyIdAttr, keyValue);
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

			CHelper.RequireBindGrid();
			//internalBind();
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
			DropDownList ddPaging = (DropDownList)sender;

			SetPageSize(Convert.ToInt32(ddPaging.SelectedValue));
			
			CHelper.RequireBindGrid();
			//this.internalBind();
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
			if (_pc[GetPropertyKey(SortingPropertyKey)] != null && _pc[GetPropertyKey(SortingPropertyKey)].ToString() == e.SortExpression)
				_pc[GetPropertyKey(SortingPropertyKey)] = e.SortExpression + " DESC";
			else
				_pc[GetPropertyKey(SortingPropertyKey)] = e.SortExpression;

			CHelper.RequireBindGrid();
		}
		#endregion

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
						}
					}
				}
			}

			return sb.ToString();
		}
		#endregion

		#region GetPropertyKey
		public string GetPropertyKey(string key)
		{
			return String.Format("{0}_{1}_{2}_{3}", this.ClassName, this.ViewName, this.PlaceName, key);
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
			MCGrid grid = null;
			grid = GetMetaGridFromCollection(CurrentPage.Controls, GridId);

			if (grid != null)
			{
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
		public static MCGrid GetMetaGridFromCollection(ControlCollection coll, string GridId)
		{
			MCGrid retVal = null;
			foreach (Control c in coll)
			{
				if (c is MCGrid && (GridId != string.Empty && c.ID == GridId))
				{
					retVal = (MCGrid)c;
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