using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.XPath;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.MetaUIEntity;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;
using Mediachase.UI.Web.Apps.MetaUI.Grid;

namespace Mediachase.Ibn.Web.UI
{
	#region class: EntityGridCustomColumnInfo
	/// <summary>
	/// Description for custom columns (like default column)
	/// </summary>
	class EntityGridCustomColumnInfo
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

		#region .ctor
		public EntityGridCustomColumnInfo()
		{
		}

		public EntityGridCustomColumnInfo(string Type)
			: this()
		{
			this.Type = Type;
		}

		public EntityGridCustomColumnInfo(string Type, string Id)
			: this(Type)
		{
			this.Id = Id;
		}

		public EntityGridCustomColumnInfo(string Type, string Id, DataControlField Column)
			: this(Type, Id)
		{
			this.Column = Column;
		}

		public EntityGridCustomColumnInfo(string Type, string Id, DataControlField Column, string dataField)
			: this(Type, Id, Column)
		{
			this.DataField = dataField;
		}
		#endregion
	}
	#endregion

	#region class: ChangingEntityGridColumnHeaderEventArgs
	public class ChangingEntityGridColumnHeaderEventArgs : EventArgs
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
		public ChangingEntityGridColumnHeaderEventArgs(DataControlField controlField, string fieldName)
		{
			_controlField = controlField;
			_fieldName = fieldName;
		}
		#endregion
	}

	public delegate void ChangingEntityGridColumnHeaderEventHandler(object sender, ChangingEntityGridColumnHeaderEventArgs e);
	#endregion

	#region class: EntityFieldInfo
	/// <summary>
	/// Description for Entity Field
	/// </summary>
	class EntityFieldInfo
	{
		#region prop: Field
		private MetaField _field;

		/// <summary>
		/// Gets or sets the MetaField.
		/// </summary>
		/// <value>The column.</value>
		public MetaField Field
		{
			get { return _field; }
			set { _field = value; }
		}
		#endregion

		#region prop: OwnFieldName
		private string _ownFieldName = string.Empty;

		/// <summary>
		/// Gets or sets the OwnFieldName.
		/// </summary>
		/// <value>The OwnFieldName.</value>
		public string OwnFieldName
		{
			get { return _ownFieldName; }
			set
			{
				if (value.Contains("."))
				{
					IsAggregation = true;
					_ownFieldName = value.Substring(0, value.IndexOf("."));
				}
				else
				{
					IsAggregation = false;
					_ownFieldName = value;
				}
			}
		}
		#endregion

		#region prop: IsAggregation
		private bool _isAggregation = false;

		/// <summary>
		/// Gets or sets the IsAggregation.
		/// </summary>
		/// <value>The IsAggregation.</value>
		public bool IsAggregation
		{
			get { return _isAggregation; }
			set { _isAggregation = value; }
		}
		#endregion

		#region prop: Width
		private int _width = 150;

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
		private string _title = String.Empty;

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

		#region prop: IsSystem
		private bool _isSystem = false;

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
		public EntityFieldInfo()
		{
		}

		public EntityFieldInfo(MetaField field)
			: this()
		{
			this.Field = field;
		}

		public EntityFieldInfo(MetaField field, string ownFieldName)
			: this(field)
		{
			this.OwnFieldName = ownFieldName;
		}

		public EntityFieldInfo(MetaField field, string ownFieldName, int width, string title)
			: this(field, ownFieldName)
		{
			this.Width = width;
			this.Title = title;
		}
		#endregion
	}
	#endregion

	#region class: EntityGridContextKey
	/// <summary>
	/// Обьект для передачи в ContextKey для WebService
	/// </summary>
	class EntityGridContextKey
	{
		#region prop: ClassName
		private string _className;

		/// <summary>
		/// Gets or sets the name of the class.
		/// </summary>
		/// <value>The name of the class.</value>
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

		#region prop: ProfileName
		private string _profileName;

		/// <summary>
		/// Gets or sets the name of the profile.
		/// </summary>
		/// <value>The name of the profile.</value>
		public string ProfileName
		{
			get { return _profileName; }
			set { _profileName = value; }
		}
		#endregion

		#region prop: PlaceName
		private string _placeName;

		/// <summary>
		/// Gets or sets the name of the place.
		/// </summary>
		/// <value>The name of the place.</value>
		public string PlaceName
		{
			get { return _placeName; }
			set { _placeName = value; }
		}
		#endregion

		#region prop: CustomColumnsCount
		private int _customColumnsCount;

		/// <summary>
		/// Gets or sets the custom columns count.
		/// </summary>
		/// <value>The custom columns count.</value>
		public int CustomColumnsCount
		{
			get { return _customColumnsCount; }
			set { _customColumnsCount = value; }
		}
		#endregion

		#region .ctor
		public EntityGridContextKey()
		{
		}

		public EntityGridContextKey(string className)
			: this()
		{
			this._className = className;
		}

		public EntityGridContextKey(string className, string profileName)
			: this(className)
		{
			this._profileName = profileName;
		}

		public EntityGridContextKey(string className, string profileName, string placeName)
			: this(className, profileName)
		{
			this._placeName = placeName;
		}

		public EntityGridContextKey(string className, string profileName, string placeName, int customColumnCount)
			: this(className, profileName, placeName)
		{
			this._customColumnsCount = customColumnCount;
		}
		#endregion
	}
	#endregion

	public partial class EntityGrid : System.Web.UI.UserControl
	{
		private readonly string classNameKey = "ClassName";
		private readonly string viewNameKey = "ViewName";
		private readonly string placeNameKey = "PlaceName";
		private readonly string profileNameKey = "ProfileName";
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

		#region prop: ProfileId

		/// <summary>
		/// Gets or sets the profile id.
		/// </summary>
		/// <value>The profile id.</value>
		public string ProfileId
		{
			get
			{
				if (ViewState["_profileId"] != null)
					return (string)ViewState["_profileId"];
				else
					return String.Empty;
			}
			set
			{
				ViewState["_profileId"] = value;
			}
		}
		#endregion

		#region prop: UserId

		/// <summary>
		/// Gets or sets the user id.
		/// </summary>
		/// <value>The user id.</value>
		public string UserId
		{
			get
			{
				if (ViewState["_UserId"] != null)
					return (string)ViewState["_UserId"];
				else
					return String.Empty;
			}
			set
			{
				ViewState["_UserId"] = value;
			}
		}
		#endregion

		#region prop: ProfileName
		/// <summary>
		/// Gets or sets the name of the profile.
		/// </summary>
		/// <value>The name of the profile.</value>
		public string ProfileName
		{
			get
			{
				if (ViewState[profileNameKey] != null)
					return (string)ViewState[profileNameKey];
				else
					return String.Empty;
			}
			set
			{
				ViewState[profileNameKey] = value;
			}
		}
		#endregion

		#region prop: CurrentProfile
		private ListViewProfile currentProfile;

		/// <summary>
		/// Gets the current view.
		/// </summary>
		/// <value>The current view.</value>
		public ListViewProfile CurrentProfile
		{
			get
			{
				if (currentProfile == null)
				{
					ListViewProfile lvp = ListViewProfile.Load(ClassName, ProfileName, PlaceName);
					if (lvp == null)
					{
						lvp = ListViewProfile.Load(ClassName, ProfileName, String.Empty);
						if (lvp == null)
						{
							if (PlaceName == "EntitySelect")
							{
								MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(ClassName);
								lvp = new ListViewProfile();
								lvp.IsSystem = true;
								lvp.IsPublic = true;
								lvp.FieldSet = new List<string>();
								lvp.FieldSet.Add(mc.TitleFieldName);
								lvp.Filters = new FilterElementCollection();
								lvp.ColumnsUI = new ColumnPropertiesCollection();
								lvp.ColumnsUI.Add(new ColumnProperties(mc.TitleFieldName, "90%", ""));
								lvp.Id = "EntitySelect";
								lvp.Name = "EntitySelect";
							}
							else
							{
								ListViewProfile[] list = ListViewProfile.GetSystemProfiles(ClassName, String.Empty);

								if (!String.IsNullOrEmpty(ProfileName))
								{
									foreach (ListViewProfile prof in list)
										if (prof.Id == ProfileName)
											lvp = prof;
								}
								else  // if ProfileName is not set then we can use any
								{
									if (list.Length > 0)
										lvp = list[0];
								}
								if (lvp == null)
									throw new ArgumentException(String.Format("Cant find list profile: {0}", ProfileName));
							}
						}
					}
					currentProfile = lvp;
				}

				return currentProfile;
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
				MainGrid.ShowCheckboxes = value;
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
		private List<EntityGridCustomColumnInfo> _customColumns = null;
		/// <summary>
		/// Gets the custom columns.
		/// </summary>
		/// <value>The custom columns.</value>
		private List<EntityGridCustomColumnInfo> CustomColumns
		{
			get
			{
				if (_customColumns == null)
					_customColumns = new List<EntityGridCustomColumnInfo>();

				return _customColumns;
			}
		}
		#endregion

		#region prop: VisibleMetaFields
		private EntityFieldInfo[] _visibleMetaFields = null;

		/// <summary>
		/// Gets or sets the visible meta fields.
		/// </summary>
		/// <value>The visible meta fields.</value>
		private EntityFieldInfo[] VisibleMetaFields
		{
			get
			{
				if (_visibleMetaFields == null || _visibleMetaFields.Length == 0)
				{
					List<EntityFieldInfo> list = new List<EntityFieldInfo>();
					foreach (string fieldName in CurrentProfile.FieldSet)
					{
						MetaField mf = FormController.GetMetaField(ClassName, fieldName);
						if (mf == null)
							continue;
						EntityFieldInfo efi = new EntityFieldInfo(mf, fieldName);
						foreach (ColumnProperties cp in CurrentProfile.ColumnsUI)
						{
							if (String.Compare(cp.Field, fieldName, true) == 0)
							{
								if (!String.IsNullOrEmpty(cp.Width))
								{
									Unit _unitWidth = Unit.Parse(cp.Width, CultureInfo.InvariantCulture);

									if (_unitWidth.Type == UnitType.Percentage)
										MainGridExt.PercentWidth = true;

									if (_unitWidth.Type == UnitType.Pixel)
										efi.IsSystem = true;

									efi.Width = Convert.ToInt32(_unitWidth.Value, CultureInfo.InvariantCulture);
								}
								if (!String.IsNullOrEmpty(cp.Title))
									efi.Title = cp.Title;
							}
						}
						list.Add(efi);
					}
					_visibleMetaFields = list.ToArray();
				}

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

		#region prop: AddFilters
		/// <summary>
		/// Gets or sets the additional filters.
		/// </summary>
		/// <value>The dditional filters.</value>
		public FilterElementCollection AddFilters
		{
			get
			{
				if (ViewState["_addFilters"] != null && ViewState["_addFilters"] is FilterElementCollection)
					return (FilterElementCollection)ViewState["_addFilters"];
				return new FilterElementCollection();
			}
			set { ViewState["_addFilters"] = value; }
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

		#region prop: AllowSorting
		public bool AllowSorting
		{
			get
			{
				if (ViewState["__allowSorting"] == null)
					return true;
				return (bool)ViewState["__allowSorting"];
			}
			set
			{
				ViewState["__allowSorting"] = value;
			}
		}
		#endregion

		#region prop: GridFilters
		/// <summary>
		/// Gets or sets the grid filters.
		/// </summary>
		/// <value>The grid filters.</value>
		public FilterElementCollection GridFilters
		{
			get
			{
				if (ViewState["_GridFilters"] == null)
					return new FilterElementCollection();

				return (FilterElementCollection)ViewState["_GridFilters"];
			}
			set
			{
				ViewState["_GridFilters"] = value;
			}
		} 
		#endregion

		#region prop: GridSorting
		/// <summary>
		/// Gets or sets the grid sorting.
		/// </summary>
		/// <value>The grid sorting.</value>
		public SortingElementCollection GridSorting
		{
			get
			{
				if (ViewState["_GridSorting"] == null)
					return new SortingElementCollection();

				return (SortingElementCollection)ViewState["_GridSorting"];
			}
			set
			{
				ViewState["_GridSorting"] = value;
			}
		}
		#endregion

		#region prop: CustomDataSource
		/// <summary>
		/// Gets or sets the custom data source.
		/// </summary>
		/// <value>The custom data source.</value>
		public EntityObject[] CustomDataSource
		{
			get
			{
				if (ViewState["_CustomDataSource"] == null)
					return null;

				return (EntityObject[])ViewState["_CustomDataSource"];
			}
			set
			{
				ViewState["_CustomDataSource"] = value;
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

		#region event: OnChangingEntityGridColumnHeader
		public event ChangingEntityGridColumnHeaderEventHandler ChangingEntityGridColumnHeader;
		protected virtual void OnChangingEntityGridColumnHeader(ChangingEntityGridColumnHeaderEventArgs e)
		{
			if (ChangingEntityGridColumnHeader != null)
				ChangingEntityGridColumnHeader(this, e);
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

			//(?)
			//MainGrid.ShowCheckboxes = this.ShowCheckboxes;
			MainGrid.AllowSorting = AllowSorting;

			if (this.PageSize != 0)
				SetPageSize(this.PageSize);

			if (!IsPostBack || PostBackRender)
			{
				FillCustomColumnCollection();

				int customColumns = this.CustomColumns.Count;
				if (this.ShowCheckboxes)
					customColumns++;
				ViewState["CustomColumnsCount"] = customColumns.ToString();
			}
			if (ViewState["CustomColumnsCount"] != null)
			{
				MainGridExt.ContextKey = UtilHelper.JsonSerialize(new EntityGridContextKey(this.ClassName, this.ProfileName, this.PlaceName, int.Parse(ViewState["CustomColumnsCount"].ToString())));
				MainGridExt.ServicePath = ResolveUrl("~/Apps/MetaUIEntity/WebServices/EntityGridService.asmx");
			}

			if (!IsPostBack || PostBackRender)
			{
				internalBind();
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			this.MainGridExt.StyleInfo = new Mediachase.Ibn.Web.UI.WebControls.GridStylesInfo(this.HeaderCssClass, this.GridCssClass, this.FooterCssClass, this.HeaderInnerCssClass, this.GridInnerCssClass, this.GridSelectedRowCssClass);
			this.MainGridExt.DashboardMode = this.DashboardMode;

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
				string key = GetPropertyKey(PageSizePropertyKey);
				if (_pc[key] != null)
				{
					int pageSize = Convert.ToInt32(_pc[key]);
					if (pageSize != -1)
						return pageSize;
					else
						return 10000;
				}
				else
				{
					_pc[key] = "10";
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
			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.ListViewUI, new Selector(ClassName, ViewName, PlaceName));
			XPathNavigator columns = navigable.CreateNavigator().SelectSingleNode("ListViewUI/Grid/CustomColumns");

			if (columns == null)
				return;

			foreach (XPathNavigator customColumn in columns.SelectChildren("Column", string.Empty))
			{
				string type = customColumn.GetAttribute("type", string.Empty);
				string width = customColumn.GetAttribute("width", string.Empty);
				string title = customColumn.GetAttribute("title", string.Empty);
				string id = customColumn.GetAttribute("id", string.Empty);
				string dataField = customColumn.GetAttribute("dataField", string.Empty);
				string templateUrl = customColumn.GetAttribute("templateUrl", string.Empty);
				string sortExpression = customColumn.GetAttribute("sortExpression", string.Empty);

				if (type != string.Empty)
				{
					if (type == "ColumnsActions")
					{
						XPathNodeIterator iterator = customColumn.SelectChildren("Item", string.Empty);
						foreach (XPathNavigator actionItem in iterator)
						{
							string commandName = actionItem.GetAttribute("commandName", string.Empty);
							if (!String.IsNullOrEmpty(commandName))
								CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.PlaceName, this.ProfileId, this.UserId, commandName);
						}

						string allowSorting = customColumn.GetAttribute("allowSorting", string.Empty);
						bool bAllowSorting = true;
						if (!bool.TryParse(allowSorting, out bAllowSorting))
							bAllowSorting = true;
						AllowSorting = bAllowSorting;
						MainGrid.AllowSorting = AllowSorting;
					}

					int widthReal = this.CustomColumnWidth(customColumn);

					if (!String.IsNullOrEmpty(width))
					{
						Unit unitWidth = Unit.Parse(width, CultureInfo.InvariantCulture);
						if (unitWidth.Type == UnitType.Percentage)
							MainGridExt.PercentWidth = true;

						int tempWidth = Convert.ToInt32(unitWidth.Value, CultureInfo.InvariantCulture);
						if (tempWidth > 0)
							widthReal = tempWidth;
					}

					TemplateField field = new TemplateField();
					if (!String.IsNullOrEmpty(templateUrl))
						field.ItemTemplate = this.Page.LoadTemplate(templateUrl);
					else
						field.ItemTemplate = new BoundColumnTemplate(dataField);
					field.HeaderText = title;
					if (!String.IsNullOrEmpty(sortExpression))
						field.SortExpression = sortExpression;

					EntityGridCustomColumnInfo columnInfo = new EntityGridCustomColumnInfo(type, id, field, dataField);
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
				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(width, true, isSortable, true));
			}

			int counter = 0;
			foreach (EntityFieldInfo field in this.VisibleMetaFields)
			{
				int cellWidth = GetMetaFieldWidth(counter, field.Width);

				MainGridExt.ColumnsInfo.Add(new GridViewColumnInfo(cellWidth, true, true, field.IsSystem));

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
			MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(ClassName);

			MainGrid.Columns.Clear();

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

			foreach (EntityGridCustomColumnInfo customColumn in this.CustomColumns)
			{
				MainGrid.Columns.Add(customColumn.Column);
				width += customColumn.Width + 7;
			}

			int counter = 0;
			foreach (EntityFieldInfo field in this.VisibleMetaFields)
			{
				if (PlaceName == String.Empty)
					MainGrid.Columns.Add((new ListColumnFactory(this.ViewName)).GetColumn(this.Page, field.Field));
				else
					MainGrid.Columns.Add((new ListColumnFactory(this.ViewName)).GetColumn(this.Page, field.Field, PlaceName));


				int cellWidth = GetMetaFieldWidth(counter, field.Width);

				width += cellWidth;

				counter++;
			}

			width += this.VisibleMetaFields.Length * 7;


			MainGrid.Width = width;

			//Adding CssColumn
			MainGrid.Columns.Add((new ListColumnFactory(this.ViewName)).GetCssColumn(this.Page, ClassName));

			internalBindHeader();

			FilterElementCollection fec = new FilterElementCollection();
			fec.AddRange(CurrentProfile.Filters.ToArray());
			if (!String.IsNullOrEmpty(this.SearchKeyword))
			{
				FilterElement fe = CHelper.GetSearchFilterElementByKeyword(this.SearchKeyword, ClassName);
				fec.Add(fe);
			}
			fec.AddRange(AddFilters.ToArray());

			//Sorting
			SortingElementCollection sec = new SortingElementCollection();
			string key = GetPropertyKey(SortingPropertyKey);
			if (_pc[key] != null)
			{
				string sort = _pc[key];
				SortingElementType set = SortingElementType.Asc;
				if (sort.IndexOf(" DESC") >= 0)
				{
					sort = sort.Substring(0, sort.IndexOf(" DESC"));
					set = SortingElementType.Desc;
				}

				// O.R. [2009-11-02] check that sorting field exists
				if (mc.Fields.Contains(sort))
				{
					sec.Add(new SortingElement(sort, set));
				}
				else
				{
					_pc[key] = null;
				}
			}
			else if (CurrentProfile.Sorting != null)
			{
				// O.R. [2009-11-02] check that sorting fields exist
				foreach (SortingElement sortElement in CurrentProfile.Sorting)
				{
					if (!mc.Fields.Contains(sortElement.Source))
					{
						CurrentProfile.Sorting = null;
						break;
					}
				}

				if (CurrentProfile.Sorting != null)
					sec = CurrentProfile.Sorting;
			}

			if (this.GridFilters.Count > 0)
			{
				fec.AddRange(this.GridFilters);
			}

			if (this.GridSorting.Count > 0)
			{
				sec.AddRange(this.GridSorting);
			}

			if (this.CustomDataSource == null)
			{
				EntityObject[] list;
				try
				{
					list = BusinessManager.List(ClassName, fec.ToArray(), sec.ToArray());
				}
				catch
				{
					string sKey = GetPropertyKey(SortingPropertyKey);
					_pc[sKey] = null;
					if (this.Parent != null && this.Parent.Parent != null && this.Parent.Parent.Parent != null && this.Parent.Parent.Parent is Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityList)
						Mediachase.Ibn.Web.UI.MetaUIEntity.Modules.EntityList.SetProfileName(_pc, ClassName, null);
					throw;
				}

				MainGridExt.IsEmpty = (list.Length == 0);

				if (list.Length == 0)
				{
					list = new EntityObject[] { new EntityObject(ClassName) };
					this.Count = 0;
				}
				else
				{
					this.Count = list.Length;
				}

				MainGrid.DataSource = list;
			}
			else
			{
				MainGrid.DataSource = this.CustomDataSource;
				this.Count = this.CustomDataSource.Length;
			}

			MainGrid.DataBind();

			internalBindPaging();

			#region Appply paging for dashboard
			if (this.DashboardMode)
			{
				MainGridExt.BottomHeight = 16;
				//HtmlControl c = (HtmlControl)MainGrid.BottomPagerRow.FindControl("pagingContainer");
				//c.Style.Add(HtmlTextWriterStyle.Position, "static");
				//c.Style.Add("float", "right");

				//MainGridExt.ServicePath = "debug";
			}
			#endregion

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
			int counter = 0;
			int notMetaFields = 0;
			string key = GetPropertyKey(SortingPropertyKey);

			for (int i = 0; i < this.CustomColumns.Count; i++)
			{
				DataControlField controlField = MainGrid.Columns[i];

				controlField.HeaderText = CHelper.GetResFileString(this.CustomColumns[i].Title);
				controlField.ItemStyle.Width = this.CustomColumns[i].Width;
				controlField.HeaderStyle.Width = this.CustomColumns[i].Width;
				controlField.SortExpression = this.CustomColumns[i].Column.SortExpression;

				// Raising event
				ChangingEntityGridColumnHeaderEventArgs e = new ChangingEntityGridColumnHeaderEventArgs(controlField, this.CustomColumns[i].DataField);
				OnChangingEntityGridColumnHeader(e);

				#region Sorting header text (arrows up/down)
				if (_pc[key] != null)
				{
					if (_pc[key] == controlField.SortExpression)
						controlField.HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_asc.gif"));
					else if (_pc[key] == controlField.SortExpression + " DESC")
						controlField.HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_desc.gif"));
				}
				#endregion

				notMetaFields++;
			}
			counter += this.CustomColumns.Count;

			foreach (EntityFieldInfo field in this.VisibleMetaFields)
			{
				int cellWidth = GetMetaFieldWidth(counter - notMetaFields, field.Width);

				MainGrid.Columns[counter].ItemStyle.Width = cellWidth;
				MainGrid.Columns[counter].HeaderStyle.Width = cellWidth;

				if (Mediachase.Ibn.Core.MetaDataWrapper.IsSortable(field.Field) && !field.Field.IsAggregation)
				{
					string sortExpr = field.Field.Name;
					if (field.IsAggregation)
						sortExpr = String.Format("{0}.{1}", field.OwnFieldName, sortExpr);
					MainGrid.Columns[counter].SortExpression = sortExpr;
				}

				DataControlField controlField = MainGrid.Columns[counter];
				if (!String.IsNullOrEmpty(field.Title))
					controlField.HeaderText = CHelper.GetResFileString(field.Title);
				else
					controlField.HeaderText = CHelper.GetResFileString(field.Field.FriendlyName);

				// Raising event
				ChangingEntityGridColumnHeaderEventArgs e = new ChangingEntityGridColumnHeaderEventArgs(controlField, field.OwnFieldName);
				OnChangingEntityGridColumnHeader(e);

				#region Sorting header text (arrows up/down)
				if (_pc[key] != null)
				{
					if (_pc[key] == controlField.SortExpression)
						controlField.HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_asc.gif"));
					else if (_pc[key] == controlField.SortExpression + " DESC")
						controlField.HeaderText += String.Format("&nbsp;<img alt='' border='0' src='{0}' />", this.ResolveUrl("~/images/IbnFramework/sort_desc.gif"));
				}
				#endregion

				counter++;
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
				int index = i - this.CustomColumns.Count;
				if (this.ShowCheckboxes)
					index--;
				if (this.ShowCheckboxes && i == 0)
				{
					continue;
				}

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					e.Row.Cells[i].Attributes.Add("unselectable", "on");

					//e.Row.Cells[i].Attributes.Add("unselectable", "on");
					if (index <= this.VisibleMetaFields.Length && e.Row.Cells[i].Controls.Count > 0)
					{
						((BaseEntityType)e.Row.Cells[i].Controls[0]).DataItem = (EntityObject)e.Row.DataItem;
						if (index < this.VisibleMetaFields.Length && index >= 0)
						{
							EntityFieldInfo efo = this.VisibleMetaFields[index];
							if (efo.IsAggregation)
							{
								if (((EntityObject)e.Row.DataItem)[efo.OwnFieldName] is EntityObject)
									((BaseEntityType)e.Row.Cells[i].Controls[0]).DataItem = (EntityObject)(((EntityObject)e.Row.DataItem)[efo.OwnFieldName]);
							}
						}
					}
					if (index < this.VisibleMetaFields.Length && index >= 0 && e.Row.Cells[i].Controls.Count > 0)
					{
						((BaseEntityType)e.Row.Cells[i].Controls[0]).FieldName = this.VisibleMetaFields[index].Field.Name;
					}
					if (e.Row.Cells[i].Controls.Count > 0 && e.Row.Cells[i].Controls[0] is CustomColumnBaseEntityType)
					{
						((CustomColumnBaseEntityType)e.Row.Cells[i].Controls[0]).ClassName = this.ClassName;
						((CustomColumnBaseEntityType)e.Row.Cells[i].Controls[0]).ViewName = this.ViewName;
						((CustomColumnBaseEntityType)e.Row.Cells[i].Controls[0]).Place = this.PlaceName;
						if (this.ShowCheckboxes)
							index++;
						if (this.ShowCheckboxes)
							((CustomColumnBaseEntityType)e.Row.Cells[i].Controls[0]).ColumnId = this.CustomColumns[i - 1].Id;
						else
							((CustomColumnBaseEntityType)e.Row.Cells[i].Controls[0]).ColumnId = this.CustomColumns[i].Id;
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
				if (e.Row.DataItem != null)
				{
					string keyValue = String.Empty;
					EntityObject eo = (EntityObject)e.Row.DataItem;
					if (eo.PrimaryKeyId.HasValue)
						keyValue = String.Format("{0}::{1}", eo.PrimaryKeyId.ToString(), eo.MetaClassName);
					else
						keyValue = String.Format("null::{0}", eo.MetaClassName);

					e.Row.Attributes.Add(IbnGridView.primaryKeyIdAttr, keyValue);
					if (this.ShowCheckboxes)
					{
						CheckBox cb = (CheckBox)row.Cells[0].Controls[0];
						cb.Attributes.Add(IbnGridView.primaryKeyIdAttr, keyValue);
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
			string key = GetPropertyKey(SortingPropertyKey);
			if (_pc[key] != null && _pc[key] == e.SortExpression)
				_pc[key] = e.SortExpression + " DESC";
			else
				_pc[key] = e.SortExpression;
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
			return String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", this.ClassName, this.ViewName, this.PlaceName, this.ProfileName, this.ProfileId, this.UserId, key);
		}
		#endregion

		#region GetMetaFieldWidth
		private int GetMetaFieldWidth(int counter, int fieldWidth)
		{
			int cellWidth = fieldWidth;
			string key = String.Format("{0}_{1}_{2}_{3}", this.ClassName, this.ProfileName, this.PlaceName, counter.ToString());
			if (_pc[key] != null)
				cellWidth = int.Parse(_pc[key]);

			if (cellWidth == 0)
				cellWidth = 100;
			return cellWidth;
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
			EntityGrid grid = null;
			grid = GetEntityGridFromCollection(CurrentPage.Controls, GridId);

			if (grid != null)
			{
				return grid.GetCheckedCollection().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			}

			return null;
		}

		/// <summary>
		/// Gets the entity grid from collection.
		/// </summary>
		/// <param name="coll">The coll.</param>
		/// <param name="GridId">The grid id.</param>
		/// <returns></returns>
		public static EntityGrid GetEntityGridFromCollection(ControlCollection coll, string GridId)
		{
			EntityGrid retVal = null;
			foreach (Control c in coll)
			{
				if (c is EntityGrid && (GridId != string.Empty && c.ID == GridId))
				{
					retVal = (EntityGrid)c;
					break;

				}
				else
				{
					retVal = GetEntityGridFromCollection(c.Controls, GridId);
					if (retVal != null)
						break;
				}
			}
			return retVal;
		}
		#endregion
	}
}
