//------------------------------------------------------------------------------
// Copyright (c) 2003-2004 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
namespace Mediachase.Web.UI.WebControls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Design;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Reflection;
	using System.Globalization;
	using Mediachase.Web.UI.WebControls.Util;


	#region Calendar event definitions
	/// <summary>
	/// Event arguments for the OnSelectedViewChange event 
	/// </summary>
	public class CalendarViewSelectEventArgs : EventArgs
	{
		private DateTime _newDate;
		private CalendarViewType _newView;

		/// <summary>
		/// The Date.
		/// </summary>
		public DateTime NewDate
		{
			get { return _newDate; }
		}


		/// <summary>
		/// Calendar view type.
		/// </summary>
		public CalendarViewType NewViewType
		{
			get { return _newView; }
		}

		/// <summary>
		/// Initializes a new instance of a CalendarViewClickEventArgs object.
		/// </summary>
		/// <param name="date">new date, if any</param>
		/// <param name="view">new view if any</param>
		public CalendarViewSelectEventArgs(DateTime date, CalendarViewType view)
		{
			_newDate = date;
			_newView = view;
		}
	}

	/// <summary>
	/// Command Event arguments for the Calendar events
	/// </summary>
	public class CalendarItemCommandEventArgs : CommandEventArgs
	{
		private CalendarItem _Item;
		private object       _cmdSrc;

		/// <summary>
		/// Calendar Item.
		/// </summary>
		public CalendarItem Item
		{
			get { return _Item; }
		}

		/// <summary>
		/// Initializes a new instance of a CalendarItemEventArgs object.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="commandSource"></param>
		/// <param name="originalArgs"></param>
		public CalendarItemCommandEventArgs(CalendarItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
		{
			_Item = item;
			_cmdSrc = commandSource;
		}

		/// <summary>
		/// The source of the command
		/// </summary>
		public object CommandSource
		{
			get
			{
				return _cmdSrc;
			}
		}
	}

	/// <summary>
	/// Event arguments for the Calendar events
	/// </summary>
	public class CalendarItemEventArgs : CommandEventArgs
	{
		private CalendarItem _Item;
		private object       _cmdSrc;

		/// <summary>
		/// Calendar Item.
		/// </summary>
		public CalendarItem Item
		{
			get { return _Item; }
		}

		/// <summary>
		/// Initializes a new instance of a CalendarItemEventArgs object.
		/// </summary>
		/// <param name="item">CalendarItem object</param>
		public CalendarItemEventArgs(CalendarItem item) : base(new CommandEventArgs("ItemCreate", ""))
		{
			_Item = item;
		}


		/// <summary>
		/// Initializes a new instance of a CalendarItemEventArgs object.
		/// </summary>
		/// <param name="item"></param>
		public CalendarItemEventArgs(CalendarItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
		{
			_Item = item;
			_cmdSrc = commandSource;
		}

		/// <summary>
		/// The source of the command
		/// </summary>
		public object CommandSource
		{
			get
			{
				return _cmdSrc;
			}
		}
	}

	/// <summary>
	/// Delegate to handle change events on the Calendar View.
	/// </summary>
	public delegate void SelectEventHandler(object sender, CalendarViewSelectEventArgs e);

	/// <summary>
	/// Delegate to handle databind events in Calendar.
	/// </summary>
	/// <example>
	/// <code>
	/// private void InitializeComponent()
	/// {
	///   CalendarCtrl.ItemCreated += new CalendarItemEventHandler(this.OnItemCreated);
	/// }
	/// 
	/// private void OnItemCreated(object sender, CalendarItemEventArgs e)
	///	{
	///	  Response.Write(e.Item.Label);
	///	}
	/// </code>
	/// </example>
	public delegate void CalendarItemEventHandler(object sender, CalendarItemEventArgs e);

	/// <summary>
	/// Delegate to handle item command events in Calendar.
	/// </summary>
	public delegate void CalendarItemCommandEventHandler(object sender, CalendarItemCommandEventArgs e);
	#endregion

	/// <summary>
	/// Calendar WebControl.
	/// </summary>
	/// <remarks>
	/// Represents a control that contains a monthly calendar. You can use
	/// designer to control the properties and add new Items or you can use
	/// databinding to populate control. You can override default rendering
	/// by specifying templates. Look into specific template documentation for
	/// code examples.
	/// 
	/// If template is not specified default layout is used.
	/// 
	/// Use "CalendarViewChange" to react on event when calendar view is changed 
	/// internally. You'll find example project that does that in the distribute package.
	/// 
	/// Use OnItemCreated and OnItemDataBound event to modify data bound items.
	/// 
	/// View type is controlled by ViewType property which can be changed in design ot
	/// runtime. Views allowed are YearView, MonthView, WorkWeekView, WeekView, WeekView2, DayView.
	/// 
	/// Use SpanType property to specify if you want spanning event. There are 2 options available
	/// Multiline, Overflow. Multiline will render
	/// spanning box and it's height will be based on how much text is entered. Overflow will 
	/// also renders box spanning across the days/hours but height is fixed (for month view) and text
	/// that didn't fit the space provided will be hidden.
	/// </remarks>
	/// <example>
	/// <code>
	/// 
	/// // Populating control in design time
	/// &lt;ie:Calendar runat="server" Width="100%" CellPadding="1" CellSpacing="0" Font-Names="Arial" Palette="Windows" BorderWidth="1px"&gt;
	///		&lt;CalendarItems&gt;
	///			&lt;ie:CalendarItem StartDate="2003-05-12" LabelColor="IndianRed" Label="Some label" EndDate="06/01/2003 21:12:16" description="Event A ends on blah blah" link="http://www.mediachase.com"/&gt;
	///		&lt;/CalendarItems&gt;
	/// &lt;/ie:Calendar&gt;"),
	/// 
	/// // Populating control using databinding
	/// public void PopulateControl()
	/// {
	///		// Populating control using databinding
	///		CalendarCtrl.DataSource = createDataSource();
	///		CalendarCtrl.DataBind();
	///	}
	///	
	///	private ArrayList createDataSource()
	///	{
	///		ArrayList list = new ArrayList();
	///		list.add(new SomeClass("some value", "someother value"));
	///		list.add(new SomeClass("some value2", "someother value2"));
	///		return list;
	///	}
	/// 
	/// // You can access elements of SomeClass by using DataItem element.
	/// private class SomeClass
	/// {
	///		Public String Value1 = "";
	///		Public String Value2 = "";
	///		Public DateTime Start = DateTime.Now;
	///		Public DateTime End = DateTime.Now;
	///     public SomeClass(String value1, String value2)
	///     {
	///       Value1 = value1;
	///       Value2 = value2;
	///     }
	/// }
	/// </code>
	/// </example>
	[
	ParseChildren(true),
	LicenseProviderAttribute(typeof(LicFileLicenseProvider)),
	Designer(typeof(Mediachase.Web.UI.WebControls.Design.CalendarDesigner)),
	ToolboxBitmap(typeof(Mediachase.Web.UI.WebControls.Calendar)),
	ToolboxData(@"<{0}:Calendar id=""CalendarCtrl"" Width=""100%"" runat=""server"" BorderWidth=""1px"" Palette=""Default"" BorderStyle=""Solid"" SpanType=""Overflowed"">
                  </{0}:Calendar>"),
	]
	public class Calendar : BasePostBackControl, INamingContainer
	{
		#region Calendar variables

		private bool IsDemoVersion = false;

		/// <summary>
		/// The namespace for the Calendar and its children.
		/// </summary>
		public const string TagNamespace = "Portal";

		/// <summary>
		/// The Calendar tag name.
		/// </summary>
		public const string CalendarTagName = "Calendar";

		/// <summary>
		/// The CalendarItem tag name.
		/// </summary>
		public const string CalendarItemTagName = "CalendarItem";

		/// <summary>
		/// Number of weeks in a month
		/// </summary>
		private const int MONTH_NO_WEEKS = 6;

		/// <summary>
		/// License
		/// </summary>
		private License license = null;

		/// <summary>
		/// Event fired when a Calendar view is changed.
		/// </summary>
		[ResDescription("CalendarViewChange")]
		public event SelectEventHandler SelectedViewChange;

		private static readonly object EventItemCreated = new object();
		private static readonly object EventItemCommand = new object();
		private static readonly object EventItemDataBound = new object();

		private CalendarItemCollection _Items;
		
		private DateTime _SelectedDate;
		private DateTime _HighlightedDate;
		//private bool _ShowTime;
		private int _DayStartHour = 8;
		private int _DayEndHour = 17;
		private CalendarViewType _ViewType;
		private bool _AbbreviatedDayNames;
		private int _MaxDisplayedItems;

		private CssTableItemStyle _CalendarItemDefaultStyle;
		private CssTableItemStyle _CalendarItemInactiveStyle;
		private CssTableItemStyle _CalendarItemHoverStyle;
		private CssTableItemStyle _CalendarItemSelectedStyle;
		private CssTableItemStyle _CalendarHeaderStyle;
		private CssTableItemStyle _CalendarItemHolidayStyle;

		// Templates
		ITemplate _DefaultItemTemplate = null;
		ITemplate _DayItemTemplate = null;
		ITemplate _DayTextItemTemplate = null;
		ITemplate _AllDayItemTemplate = null;
		ITemplate _WeekItemTemplate = null;
		ITemplate _WeekTextItemTemplate = null;
		ITemplate _MonthItemTemplate = null;
		ITemplate _MonthTextItemTemplate = null;
		ITemplate _YearItemTemplate = null;
		ITemplate _TaskItemTemplate = null;
		ITemplate _TaskItemBoxTemplate = null;
		ITemplate _TaskItemHeaderTemplate = null;


		// Databinding
		private object _DataSource;
		private object _HolidayDataSource;
		private object _OwnerDataSource;

		// Holidays
		private HolidayCollection _holidays;

		// Owners
		private OwnerCollection _Owners;
		private Owner _CurrentOwner;

		// Work Week
		private CalendarDayOfWeek _WorkWeek;
		private CalendarDayOfWeek _FirstDayOfWeek;

		// Dates
		private DateCollection _Dates;

		// Validation variable
		private bool _IsValidated = false;

		#endregion

		#region Calendar Init
		/// <summary>
		/// Initializes a new instance of a Calendar.
		/// </summary>
		public Calendar() : base()
		{
			#if (DEMO)
			IsDemoVersion = true;
			#endif 

			// only check license in design time
			// we don't use runtime license check to make control easier to deploy
			//if(LicenseManager.CurrentContext.UsageMode == LicenseUsageMode.Designtime)
			//	license = LicenseManager.Validate(typeof(Calendar), this);

			_MaxDisplayedItems = 6;
			//_ShowTime = true;
			_AbbreviatedDayNames = true;
			_ViewType = CalendarViewType.MonthView;
			_SelectedDate = DateTime.Now.Date;
			_HighlightedDate = DateTime.Now.Date;
			_Items = new CalendarItemCollection(this);
			//_holidays = new HolidayCollection(this);

			_CalendarItemDefaultStyle = new CssTableItemStyle();
			_CalendarItemHoverStyle = new CssTableItemStyle();
			_CalendarItemSelectedStyle = new CssTableItemStyle();
			_CalendarItemInactiveStyle = new CssTableItemStyle();
			_CalendarItemHolidayStyle = new CssTableItemStyle();
			//_CalendarDefaultStyle = new TableStyle();
			_CalendarHeaderStyle = new CssTableItemStyle();

			_WorkWeek = CalendarDayOfWeek.Monday | CalendarDayOfWeek.Tuesday | CalendarDayOfWeek.Wednesday | CalendarDayOfWeek.Thursday | CalendarDayOfWeek.Friday;
			_FirstDayOfWeek = CalendarDayOfWeek.Sunday;

			_Dates = new DateCollection(this);
		}

		/// <summary>
		/// Clean up all global variables.
		/// </summary>
		public override void Dispose () 
		{
			if (license != null) 
			{
				license.Dispose();
				license = null;
			}
			base.Dispose();
		}

		/// <summary>
		/// Internal functions creates styles.
		/// </summary>
		/// <returns></returns>
		protected override Style CreateControlStyle ()
		{
			TableStyle retVal = new TableStyle (ViewState);
			retVal.CellSpacing = 0;
			return retVal;
		} 
		#endregion

		#region Calendar public properties

		/// <summary>
		/// Color palette to use with calendar control.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		Editor(typeof(Mediachase.Web.UI.WebControls.Design.CalendarColorsEditor), typeof(UITypeEditor)),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ColorPalette"),
		]
		public CalendarColorPalette Palette
		{
			get
			{
				Object obj = ViewState["ColorPalette"];
				return (obj == null) ? CalendarColorPalette.Default : (CalendarColorPalette)obj;
			}

			set
			{
				ViewState["ColorPalette"] = value;
			}
		}

		/// <summary>
		/// Display shadows for items.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("UseShadows"),
		]
		public bool UseShadows
		{
			get
			{
				Object obj = ViewState["UseShadows"];
				return (obj == null) ? true : (bool)obj;
			}

			set
			{
				ViewState["UseShadows"] = value;
			}
		}

		/// <summary>
		/// calendar item control height.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue("18"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemHeight"),
		]
		public Unit ItemHeight
		{
			get
			{
				Object obj = ViewState["ItemHeight"];
				return (obj == null) ? 18 : (Unit)obj;
			}

			set
			{
				ViewState["ItemHeight"] = value;
			}
		}

		//  [9/15/2004] Begin
		/// <summary>
		/// Calendar Task Item Control Width.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue("14"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("TaskItemWidth"),
		]
		public Unit TaskItemWidth
		{
			get
			{
				Object obj = ViewState["TaskItemWidth"];
				return (obj == null) ? 14 : (Unit)obj;
			}

			set
			{
				ViewState["TaskItemWidth"] = value;
			}
		}
		//  [9/15/2004] --- End

		/// <summary>
		/// calendar item control height.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue("..."),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("MoreItemsText"),
		]
		public string MoreItemsText
		{
			get
			{
				object o = ViewState["MoreItemsText"];
				if(o!=null)
					return (string)o;
				return "...";
			}
			set
			{
				ViewState["MoreItemsText"] = value;
			}
		} 

		/// <summary>
		/// calendar item control height.
		/// </summary>
		[
		Category("Appearance"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("AlternativeBorderColor"),
		]
		public Color AlternativeBorderColor
		{
			get
			{
				object o = ViewState["AlternativeBorderColor"];
				if(o!=null)
					return (Color)o;
				return Color.Empty;
			}
			set
			{
				ViewState["AlternativeBorderColor"] = value;
			}
		}

		/// <summary>
		/// Defines cellpadding for root table.
		/// </summary>
		[Bindable(true)]
		[DefaultValue(-1)]
		[Category("Layout")]
		[ResDescription("CellPadding")]
		public virtual int CellPadding
		{
			get
			{
				if(!ControlStyleCreated)
					return -1;
				return ((TableStyle)ControlStyle).CellPadding;
			}
			set
			{
				((TableStyle)ControlStyle).CellPadding = value;
			}
		}

		/// <summary>
		/// Defines cell spacing for root table
		/// </summary>
		[Bindable(true)]
		[DefaultValue(-1)]
		[Category("Layout")]
		[ResDescription("CellSpacing")]
		public virtual int CellSpacing
		{
			get
			{
				if(!ControlStyleCreated)
					return -1;
				return ((TableStyle)ControlStyle).CellSpacing;
			}
			set
			{
				((TableStyle)ControlStyle).CellSpacing = value;
			}
		}

		/// <summary>
		/// Gets or sets control span type. 
		/// This only affects monthly calendar view. And will render
		/// span bars.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(CalendarSpanType.Overflowed),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("SpanType"),
		]
		public CalendarSpanType SpanType
		{
			get
			{
				Object obj = ViewState["SpanType"];
				return((obj == null) ? CalendarSpanType.Overflowed : (CalendarSpanType)obj);
			}

			set { ViewState["SpanType"] = value; }
		}

		/// <summary>
		/// Gets or sets work day start hour. Hours before that will 
		/// be rendered with inactive style or wont be rendered at all.
		/// Default value is 8.
		/// </summary>
		[
		Category("Day View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DayStartHour"),
		]
		public int DayStartHour
		{
			get
			{
				Object obj = ViewState["DayStartHour"];
				return((obj == null) ? _DayStartHour : (int)obj);
			}

			set { ViewState["DayStartHour"] = value; }
		}

		/// <summary>
		/// Gets or sets work day end hour. Hours after that will 
		/// be rendered with inactive style or wont be rendered at all.
		/// Default value is 17.
		/// </summary>
		[
		Category("Day View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DayEndHour"),
		]
		public int DayEndHour
		{
			get
			{
				Object obj = ViewState["DayEndHour"];
				return((obj == null) ? _DayEndHour : (int)obj);
			}
			set { 
				if(value>0 && value < 24)
					ViewState["DayEndHour"] = value; 
			}
		}

		/// <summary>
		/// Sets month start date.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("MonthStartDate"),
		]
		public DateTime MonthStartDate
		{
			get
			{
				Object obj = ViewState["MonthStartDate"];
				return (obj == null) ? DateTime.MinValue : (DateTime)obj;
			}
			set
			{
				ViewState["MonthStartDate"] = value;
			}
		}

		/// <summary>
		/// Gets or sets view type.
		/// <see cref="CalendarViewType"/>
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ViewType"),
		]
		public CalendarViewType ViewType
		{
			get
			{
				Object obj = ViewState["ViewType"];
				return((obj == null) ? _ViewType : (CalendarViewType)obj);
			}

			set { 
				ViewState["ViewType"] = value; 
			}
		}

		/// <summary>
		/// Gets or sets if day names are Abbreviated.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(true),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("AbbreviatedDayNames"),
		]
		public bool AbbreviatedDayNames
		{
			get
			{
				Object obj = ViewState["AbbreviatedDayNames"];
				return((obj == null) ? _AbbreviatedDayNames : (bool)obj);
			}

			set { ViewState["AbbreviatedDayNames"] = value; }
		}


		/// <summary>
		/// Gets or sets a value that indicates whether the control has multiple CalendarItem owners.
		/// </summary>
		[
		Category("Day View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("MultiOwner"),
		]
		public bool MultiOwner
		{
			get
			{
				Object obj = ViewState["MultiOwner"];
				return((obj == null) ? false : (bool)obj);
			}

			set { ViewState["MultiOwner"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that defines how hours will be rendered within a day view.
		/// </summary>
		[
		Category("Day View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DayHourFormat"),
		]
		public string DayHourFormat
		{
			get
			{
				Object obj = ViewState["DayHourFormat"];
				return((obj == null) ? System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortTimePattern : (string)obj);
			}

			set { ViewState["DayHourFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets maximum displayed items in the cell. Only affects 
		/// month and year view, default value is 6.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue("6"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("MaxDisplayedItems"),
		]
		public int MaxDisplayedItems
		{
			get
			{
				Object obj = ViewState["MaxDisplayedItems"];
				return((obj == null) ? _MaxDisplayedItems : (int)obj);
			}

			set { ViewState["MaxDisplayedItems"] = value; }
		}


		/// <summary>
		/// Gets or sets view state mode. 
		/// Reduced viewstate won't track items data. 
		/// You will have to rebind everytime.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(true),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("UseReducedViewState"),
		]
		private bool UseReducedViewState
		{
			get
			{
				Object obj = ViewState["UseReducedViewState"];
				return((obj == null) ? true : (bool)obj);
			}

			set { ViewState["UseReducedViewState"] = value; }
		}

		/// <summary>
		/// Gets or sets the the if events out of scope are removed.
		/// True by default, will affect performance if set to false.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("RemoveItems"),
		]
		public bool RemoveItems
		{
			get
			{
				Object obj = ViewState["RemoveItems"];
				return((obj == null) ? true : (bool)obj);
			}

			set { ViewState["RemoveItems"] = value; }
		}

		/// <summary>
		/// Gets or sets the the link to be used with hours. If none set link won't be rendered. 
		/// Format must include replacement parameter {0} which will be replaced by full date/time 
		/// variable during runtime.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("NewLinkFormat"),
		]
		public String NewLinkFormat
		{
			get
			{
				Object obj = ViewState["NewLinkFormat"];
				return((obj == null) ? "" : (String)obj);
			}

			set { ViewState["NewLinkFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets the the link to be used with hours. If none set link won't be rendered. 
		/// Format must include replacement parameter {0} which will be replaced by full date/time 
		/// variable during runtime.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ContextMenuFormat"),
		]
		public String ContextMenuFormat
		{
			get
			{
				Object obj = ViewState["ContextMenuFormat"];
				return((obj == null) ? "" : (String)obj);
			}

			set { ViewState["ContextMenuFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets the link to be used with hours on Days view. If none set link won't be rendered. 
		/// Format must include replacement parameter {0} which will be replaced by full date/time 
		/// variable during runtime.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DayLinkFormat"),
		]
		public String DayLinkFormat
		{
			get
			{
				Object obj = ViewState["DayLinkFormat"];
				return((obj == null) ? "" : (String)obj);
			}

			set { ViewState["DayLinkFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets the link to be used with multi owner day view. If none set link NewLinkFormat property will be used. 
		/// Format must include replacement parameter {0} which will be replaced by full date/time 
		/// variable during runtime and parameter {1} which will be replaced by owner key.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("NewMultiOwnerLinkFormat"),
		]
		public String NewMultiOwnerLinkFormat
		{
			get
			{
				Object obj = ViewState["NewMultiOwnerLinkFormat"];
				return((obj == null) ? "" : (String)obj);
			}

			set { ViewState["NewMultiOwnerLinkFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets the the link to be used with owners. If none set link won't be rendered. 
		/// Format must include replacement parameter {0} which will be replaced by full date/time 
		/// variable during runtime and {1} which will be replaced by owner id.
		/// </summary>
		[
		Category("Day View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("MultiOwnerFormat"),
		]
		public String MultiOwnerFormat
		{
			get
			{
				Object obj = ViewState["MultiOwnerFormat"];
				return((obj == null) ? "" : (String)obj);
			}

			set { ViewState["MultiOwnerFormat"] = value; }
		}

		/// <summary>
		/// Holidays
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ResDescription("Holidays"),
		]
		public HolidayCollection Holidays
		{
			get
			{
				if(_holidays==null)
					_holidays = new HolidayCollection(this);

				return _holidays;
			}
		}

		/// <summary>
		/// Gets or sets the data source that populates the items of the control.
		/// Must implement IEnumerable interface.
		/// </summary>
		/// <remarks>
		/// Use the <b>DataSource</b> property to specify the source of values to 
		/// bind to a calendar control. The data source must be an object that implements the 
		/// <see cref="System.Collections.IEnumerable"/> interface (such as 
		/// <see cref="System.Data.DataView"/>, <see cref="System.Collections.ArrayList"/>, and 
		/// <see cref="System.Collections.Hashtable"/>) to bind to a calendar control.
		/// </remarks>
		[
		Bindable(true),
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ResDescription("OwnerDataSource"),
		]
		public virtual Object OwnerDataSource
		{
			get { return _OwnerDataSource; }
			set 
			{ 
				if( (value!=null) && (value is IListSource || value is IEnumerable) )
				{
					_OwnerDataSource = value;
				} 
				else
				{
					throw new ArgumentException("Invalid OwnerDataSource Type");
				}
			}
		}

		/// <summary>
		/// Gets or sets the specific data member in a multimember data source to bind to a calendar control.
		/// </summary>
		/// <remarks>
		/// Use the DataMember property to specify a member from a multimember 
		/// data source to bind to the list control. For example, if you have 
		/// a data source, with more than one table, specified in the <see cref="DataSource"/> property, 
		/// use the DataMember property to specify which table to bind to a 
		/// calendar control.
		/// </remarks>
		[DefaultValue("")]
		[Category("Data")]
		[ResDescription("OwnerDataMember")]
		public string OwnerDataMember
		{
			get
			{
				object o = ViewState["OwnerDataMember"];
				if(o!=null)
					return (string)o;
				return String.Empty;
			}
			set
			{
				ViewState["OwnerDataMember"] = value;
			}
		}

		/// <summary>
		/// This property enables you to obtain a reference to the list of the owners that are currently stored in the Calendar. 
		/// With this reference, you can add , remove and obtain a count of the owners in the collection. 
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ResDescription("Owners"),
		]
		public OwnerCollection Owners
		{
			get
			{
				if(_Owners==null)
					_Owners = new OwnerCollection(this);

				return _Owners;
			}
		}

		/// <summary>
		/// Gets or sets a string that specifies the property of the data source to map to the Owner property of the CalendarItem objects.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("OwnerDataTextField"),
		]
		public string OwnerDataTextField
		{
			get
			{
				Object obj = ViewState["OwnerDataTextField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["OwnerDataTextField"] = value; }
		}

		/// <summary>
		/// Gets or sets a string that specifies the property of the data source to map to the Owner property of the CalendarItem objects.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("OwnerDataValueField"),
		]
		public string OwnerDataValueField
		{
			get
			{
				Object obj = ViewState["OwnerDataValueField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["OwnerDataValueField"] = value; }
		}

		/// <summary>
		/// Gets or sets the data source that populates the items of the control.
		/// Must implement IEnumerable interface.
		/// </summary>
		/// <remarks>
		/// Use the <b>DataSource</b> property to specify the source of values to 
		/// bind to a calendar control. The data source must be an object that implements the 
		/// <see cref="System.Collections.IEnumerable"/> interface (such as 
		/// <see cref="System.Data.DataView"/>, <see cref="System.Collections.ArrayList"/>, and 
		/// <see cref="System.Collections.Hashtable"/>) to bind to a calendar control.
		/// </remarks>
		[
		Bindable(true),
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ResDescription("HolidayDataSource"),
		]
		public virtual Object HolidayDataSource
		{
			get { return _HolidayDataSource; }
			set 
			{ 
				if( (value!=null) && (value is IListSource || value is IEnumerable) )
				{
					_HolidayDataSource = value;
				} 
				else
				{
					throw new ArgumentException("Invalid HolidayDataSource Type");
				}
			}
		}

		/// <summary>
		/// Gets or sets the specific data member in a multimember data source to bind to a calendar control.
		/// </summary>
		/// <remarks>
		/// Use the DataMember property to specify a member from a multimember 
		/// data source to bind to the list control. For example, if you have 
		/// a data source, with more than one table, specified in the <see cref="DataSource"/> property, 
		/// use the DataMember property to specify which table to bind to a 
		/// calendar control.
		/// </remarks>
		[DefaultValue("")]
		[Category("Data")]
		[ResDescription("HolidayDataMember")]
		public string HolidayDataMember
		{
			get
			{
				object o = ViewState["HolidayDataMember"];
				if(o!=null)
					return (string)o;
				return String.Empty;
			}
			set
			{
				ViewState["HolidayDataMember"] = value;
			}
		}


		/// <summary>
		/// Gets or sets the field of the holiday data source that provides the text/title content.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("HolidayDataTextField"),
		]
		public string HolidayDataTextField
		{
			get
			{
				Object obj = ViewState["HolidayDataTextField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["HolidayDataTextField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the holiday data source that provides the date.
		/// Not used when all templates are specified.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("HolidayDataDateField"),
		]
		public string HolidayDataDateField
		{
			get
			{
				Object obj = ViewState["HolidayDataDateField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["HolidayDataDateField"] = value; }
		}

		/// <summary>
		/// Gets or sets the data source that populates the items of the control.
		/// Must implement IEnumerable interface.
		/// </summary>
		/// <remarks>
		/// Use the <b>DataSource</b> property to specify the source of values to 
		/// bind to a calendar control. The data source must be an object that implements the 
		/// <see cref="System.Collections.IEnumerable"/> interface (such as 
		/// <see cref="System.Data.DataView"/>, <see cref="System.Collections.ArrayList"/>, and 
		/// <see cref="System.Collections.Hashtable"/>) to bind to a calendar control.
		/// </remarks>
		[
		Bindable(true),
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ResDescription("DataSource"),
		]
		public virtual Object DataSource
		{
			get { return _DataSource; }
			set 
			{ 
				if( (value!=null) && (value is IListSource || value is IEnumerable) )
				{
					_DataSource = value;
				} 
				else
				{
					throw new ArgumentException("Invalid DataSource Type");
				}
			}
		}

		/// <summary>
		/// Gets or sets the active Calendar Owner.
		/// </summary>
		[
		Bindable(false),
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ResDescription("CurrentOwner"),
		]
		public Owner CurrentOwner
		{
			get { return _CurrentOwner; }
			set 
			{ 
				_CurrentOwner = value;
			}
		}

		/// <summary>
		/// Gets or sets the specific data member in a multimember data source to bind to a calendar control.
		/// </summary>
		/// <remarks>
		/// Use the DataMember property to specify a member from a multimember 
		/// data source to bind to the list control. For example, if you have 
		/// a data source, with more than one table, specified in the <see cref="DataSource"/> property, 
		/// use the DataMember property to specify which table to bind to a 
		/// calendar control.
		/// </remarks>
		[DefaultValue("")]
		[Category("Data")]
		[ResDescription("DataMember")]
		public string DataMember
		{
			get
			{
				object o = ViewState["DataMember"];
				if(o!=null)
					return (string)o;
				return String.Empty;
			}
			set
			{
				ViewState["DataMember"] = value;
			}
		}


		/// <summary>
		/// Gets or sets the field of the data source that provides the text/title content.
		/// Not used when all templates are specified.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataTextField"),
		]
		public string DataTextField
		{
			get
			{
				Object obj = ViewState["DataTextField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataTextField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the url content.
		/// Not used when all templates are specified.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataLinkField"),
		]
		public string DataLinkField
		{
			get
			{
				Object obj = ViewState["DataLinkField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataLinkField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the url format.
		/// Used to define format for generating actual link output
		/// </summary>
		/// <example>
		/// <code>For example: 
		/// CalendarCtrl.DataLinkField="EventId";
		/// CalendarCtrl.DataLinkFormat="events/ShowEventDetails.aspx?id={0}";
		/// 
		/// Here "{0}" will be replaced with a value from DataLinkField.
		/// </code>
		/// </example>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataLinkFormat"),
		]
		public string DataLinkFormat
		{
			get
			{
				Object obj = ViewState["DataLinkFormat"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataLinkFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the start date value.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataStartDateField"),
		]
		public string DataStartDateField
		{
			get
			{
				Object obj = ViewState["DataStartDateField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataStartDateField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the end date value.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataEndDateField"),
		]
		public string DataEndDateField
		{
			get
			{
				Object obj = ViewState["DataEndDateField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataEndDateField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the description content.
		/// Not used when all templates are specified.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataDescriptionField"),
		]
		public string DataDescriptionField
		{
			get
			{
				Object obj = ViewState["DataDescriptionField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataDescriptionField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the background color for spanning element.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataColorField"),
		]
		public string DataColorField
		{
			get
			{
				Object obj = ViewState["DataColorField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataColorField"] = value; }
		}

		/// <summary>
		/// Gets or sets the field of the data source that provides the owner content.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataOwnerField"),
		]
		public string DataOwnerField
		{
			get
			{
				Object obj = ViewState["DataOwnerField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataOwnerField"] = value; }
		}
/*
		/// <summary>
		/// Gets or sets the field of the data source that provides the isallday calendaritem property.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DataIsAllDayField"),
		]
		public string DataIsAllDayField
		{
			get
			{
				Object obj = ViewState["DataIsAllDayField"];
				return((obj == null) ? String.Empty : (string)obj);
			}

			set { ViewState["DataIsAllDayField"] = value; }
		}
*/
		/// <summary>
		/// Path to the directory holding images required by the control
		/// </summary>
		[
		Category("Data"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("CalendarSystemImagesPath"),
		]
		public string SystemImagesPath
		{
			get
			{
				object str = ViewState["SystemImagesPath"];
				return ((str == null) ? AddPathToFilename("~/Images/IbnFramework/") : (string)str);
			}
			set
			{
				String str = value;
				if (str.Length > 0 && str[str.Length - 1] != '/')
					str = str + '/';
				ViewState["SystemImagesPath"] = str;
			}
		}

		/// <summary>
		/// Gets the collection of items in the control.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(null),
		MergableProperty(false),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("CalendarItems"),
		]
		public virtual CalendarItemCollection Items
		{
			get { this.EnsureChildControls(); return _Items; }
		}

		/// <summary>
		/// The selected date. Defines which view is rendered.
		/// <see cref="HighlightedDate">
		/// </see>
		/// </summary>
		[
		Category("Data"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("SelectedDate"),
		]
		public DateTime SelectedDate
		{
			get
			{
				Object obj = ViewState["SelectedDate"];
				return (obj == null) ? _SelectedDate : (DateTime)obj;
			}
			set
			{
				ViewState["SelectedDate"] = value;
			}
		}

		/// <summary>
		/// Returns actual displayed Start Date depending on the view type.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DisplayStartDate"),
		]
		public DateTime DisplayStartDate
		{
			get
			{
				DateTime returnDate = DateTime.MinValue;
				switch(ViewType)
				{
					case CalendarViewType.TaskView:
						returnDate = TimescaleStartDate;
						break;
					case CalendarViewType.DayView:
						returnDate = SelectedDate.Date;
						break;
					case CalendarViewType.WorkWeekView:
						returnDate = SelectedDate.Date.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek));
						break;
					case CalendarViewType.WeekView2:
						returnDate = SelectedDate.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek));
						if(returnDate.DayOfWeek == DayOfWeek.Monday) // when week starts on monday move one day backwards
							returnDate = returnDate.AddDays(-1);
						else if(returnDate.DayOfWeek == DayOfWeek.Saturday) // Arabic starts on Saturday
							returnDate = returnDate.AddDays(-1);

						returnDate = returnDate.AddDays(1);
						break;
					case CalendarViewType.WeekView:
						returnDate = SelectedDate.Date.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek));
						break;
					case CalendarViewType.MonthView:
						returnDate = Helper.GetMonthStartDate(SelectedDate.Date, this.MonthStartDate, this.FirstDayOfWeek);
						break;
					case CalendarViewType.YearView:
						returnDate = new DateTime(SelectedDate.Year, 1, 1);
						break;
				}
				return returnDate;
			}
		}


		/// <summary>
		/// Returns actual displayed Start Date depending on the view type.
		/// </summary>
		[
		Category("Data"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DisplayEndDate"),
		]
		public DateTime DisplayEndDate
		{
			get
			{
				DateTime returnDate = DateTime.MaxValue;
				switch(ViewType)
				{
					case CalendarViewType.TaskView:
						returnDate = TimescaleEndDate;
						break;
					case CalendarViewType.DayView:
						returnDate = SelectedDate.Date.AddDays(1).AddTicks(-1);
						break;
					case CalendarViewType.WorkWeekView:
						//returnDate = SelectedDate.Date.AddDays(-SelectedDate.DayOfWeek.GetHashCode() + 1).AddDays(6).AddTicks(-1);
						returnDate = SelectedDate.Date.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek)).AddDays(7).AddTicks(-1);
						break;
					case CalendarViewType.WeekView2:
					case CalendarViewType.WeekView:
						//returnDate = SelectedDate.Date.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek)).AddDays(7).AddTicks(-1);
						returnDate = DisplayStartDate.AddDays(7).AddTicks(-1);
						break;
					case CalendarViewType.MonthView:
						returnDate = Helper.GetMonthStartDate(SelectedDate.Date, this.MonthStartDate, this.FirstDayOfWeek).AddDays(MONTH_NO_WEEKS * 7 + 1).AddTicks(-1);
						break;
					case CalendarViewType.YearView:
						returnDate = new DateTime(SelectedDate.Year + 1, 1, 1).AddTicks(-1);
						break;
				}
				return returnDate;
			}
		}

		/// <summary>
		/// The highlighted date. Specified highlighted date.
		/// <see cref="SelectedDate">
		/// </see>
		/// 
		/// Since 1.0.4
		/// </summary>
		[
		Category("Data"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("HighlightedDate"),
		]
		public DateTime HighlightedDate
		{
			get
			{
				Object obj = ViewState["HighlightedDate"];
				return (obj == null) ? _HighlightedDate : (DateTime)obj;
			}
			set
			{
				ViewState["HighlightedDate"] = value;
			}
		}

		/// <summary>
		/// The default style for calendar item.
		/// </summary>
		[
		Category("Styles"),
		DefaultValue(typeof(CssTableItemStyle), ""),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		ResDescription("CalendarItemDefaultStyle"),
		]
		public CssTableItemStyle CalendarItemDefaultStyle
		{
			get { return _CalendarItemDefaultStyle; }
			set
			{
				_CalendarItemDefaultStyle = value;
				if (IsTrackingViewState)
				{
					((IStateManager)_CalendarItemDefaultStyle).TrackViewState();
					//_CalendarItemDefaultStyle.Dirty = true;
				}
			}
		}

		/// <summary>
		/// The header style for calendar.
		/// </summary>
		[
		Category("Styles"),
		DefaultValue(typeof(CssTableItemStyle), ""),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("CalendarHeaderStyle"),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public CssTableItemStyle CalendarHeaderStyle
		{
			get { return _CalendarHeaderStyle; }
			set
			{
				_CalendarHeaderStyle = value;
				if (IsTrackingViewState)
				{
					((IStateManager)_CalendarHeaderStyle).TrackViewState();
					//_CalendarHeaderStyle.Dirty = true;
				}
			}
		}
		

		/// <summary>
		/// The default style for calendar item when they are selected.
		/// </summary>
		[
		Category("Styles"),
		DefaultValue(typeof(CssTableItemStyle), ""),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("CalendarItemSelectedStyle"),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public CssTableItemStyle CalendarItemSelectedStyle
		{
			get { return _CalendarItemSelectedStyle; }
			set
			{
				_CalendarItemSelectedStyle = value;
				if (IsTrackingViewState)
				{
					((IStateManager)_CalendarItemSelectedStyle).TrackViewState();
					//_CalendarItemSelectedStyle.Dirty = true;
				}
			}
		}

		/// <summary>
		/// The inactive style for calendar when they are selected.
		/// </summary>
		[
		Category("Styles"),
		DefaultValue(typeof(CssTableItemStyle), ""),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("CalendarItemInactiveStyle"),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public CssTableItemStyle CalendarItemInactiveStyle
		{
			get { return _CalendarItemInactiveStyle; }
			set
			{
				_CalendarItemInactiveStyle = value;
				if (IsTrackingViewState)
				{
					((IStateManager)_CalendarItemInactiveStyle).TrackViewState();
					//_CalendarItemInactiveStyle.Dirty = true;
				}
			}
		}

		/// <summary>
		/// The holiday style for calendar when they are selected.
		/// </summary>
		[
		Category("Styles"),
		DefaultValue(typeof(CssTableItemStyle), ""),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("CalendarItemHolidayStyle"),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public CssTableItemStyle CalendarItemHolidayStyle
		{
			get { return _CalendarItemHolidayStyle; }
			set
			{
				_CalendarItemHolidayStyle = value;
				if (IsTrackingViewState)
				{
					((IStateManager)_CalendarItemHolidayStyle).TrackViewState();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether an automatic postback to the server 
		/// will occur whenever the user changes the selected index.
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(false),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("AutoPostBack"),
		]
		public bool AutoPostBack
		{
			get
			{
				Object obj = ViewState["AutoPostBack"];
				return ((obj == null) ? false : (bool)obj);
			}
			set { ViewState["AutoPostBack"] = value; }
		}

		
		/// <summary>
		/// Default Template. It will be used if no other template is specified.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;DefaultItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %> - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt; 
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/DefaultItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate DefaultItemTemplate
		{
			get 
			{
				return _DefaultItemTemplate;
			}
			set 
			{
				_DefaultItemTemplate = value;
			}
		}
		
		/// <summary>
		/// Template for day item. Use it to customize how data is rendered inside
		/// hour cell in day view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;DayItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %> - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt; 
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/DayItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate DayItemTemplate
		{
			get 
			{
				return _DayItemTemplate;
			}
			set 
			{
				_DayItemTemplate = value;
			}
		}

		/// <summary>
		/// Template for day text item. Use it to customize how data is rendered inside
		/// hour cell in day view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;DayTextItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %> - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt; 
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/DayTextItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate DayTextItemTemplate
		{
			get 
			{
				return _DayTextItemTemplate;
			}
			set 
			{
				_DayTextItemTemplate = value;
			}
		}

		/// <summary>
		/// Template for all day item. Use it to customize how data is rendered inside
		/// all day cell in day view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;AllDayItemTemplate&gt;
		///   All Day: &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %> - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/AllDayItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate AllDayItemTemplate
		{
			get 
			{
				return _AllDayItemTemplate;
			}
			set 
			{
				_AllDayItemTemplate = value;
			}
		}

		/// <summary>
		/// Template for week item. Use it to customize how data is rendered inside
		/// day cell in week view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;WeekItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %&gt; - &gt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/WeekItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate WeekItemTemplate
		{
			get 
			{
				return _WeekItemTemplate;
			}
			set 
			{
				_WeekItemTemplate = value;
			}
		}

		/// <summary>
		/// Template for week text item. Use it to customize how data is rendered inside
		/// day cell in week view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;WeekTextItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %&gt; - &gt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/WeekTextItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate WeekTextItemTemplate
		{
			get 
			{
				return _WeekTextItemTemplate;
			}
			set 
			{
				_WeekTextItemTemplate = value;
			}
		}

		/// <summary>
		/// Template for month item. Use it to customize how data is rendered inside
		/// day cell in month view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;MonthItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %&gt; - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/MonthItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate MonthItemTemplate
		{
			get 
			{
				return _MonthItemTemplate;
			}
			set 
			{
				_MonthItemTemplate = value;
			}
		}


		/// <summary>
		/// Template for month text item. Use it to customize how data is rendered inside
		/// day cell in month view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;MonthTextItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %&gt; - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/MonthTextItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate MonthTextItemTemplate
		{
			get 
			{
				return _MonthTextItemTemplate;
			}
			set 
			{
				_MonthTextItemTemplate = value;
			}
		}

		/// <summary>
		/// Template for task item. Use it to customize how data is rendered inside
		/// tasks bar within task view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;TaskItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %&gt; - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/TaskItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate TaskItemTemplate
		{
			get 
			{
				return _TaskItemTemplate;
			}
			set 
			{
				_TaskItemTemplate = value;
			}
		}
		
		/// <summary>
		/// Template for task block item. Use it to customize how data is rendered inside
		/// tasks bar within task view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;TaskItemBoxTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/TaskItemBoxTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate TaskItemBoxTemplate
		{
			get 
			{
				return _TaskItemBoxTemplate;
			}
			set 
			{
				_TaskItemBoxTemplate = value;
			}
		}

		/// <summary>
		/// Template for task header. Use it to customize how data is rendered inside
		/// tasks bar within task view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;TaskItemHeaderTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/TaskItemHeaderTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(TemplateItem))
		]
		public ITemplate TaskItemHeaderTemplate
		{
			get 
			{
				return _TaskItemHeaderTemplate;
			}
			set 
			{
				_TaskItemHeaderTemplate = value;
			}
		}

		/// <summary>
		/// Template for year item. Use it to customize how data is rendered inside
		/// month cell in year view.
		/// </summary>
		/// <example>
		/// <code>
		/// &lt;YearItemTemplate&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "startdate") %&gt; - &lt;%# DataBinder.Eval(Container.DataItem, "enddate") %&gt;
		///   &lt;%# DataBinder.Eval(Container.DataItem, "title") %&gt;
		/// &lt;/YearItemTemplate&gt;
		/// </code>
		/// </example>
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false),
		TemplateContainer(typeof(CalendarItem))
		]
		public ITemplate YearItemTemplate
		{
			get 
			{
				return _YearItemTemplate;
			}
			set 
			{
				_YearItemTemplate = value;
			}
		}

		/// <summary>
		/// Gets or sets to either show Event Bar or not.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("EventBarShow"),
		]
		public bool EventBarShow
		{
			get
			{
				Object obj = ViewState["EventBarShow"];
				return((obj == null) ? true : (Boolean)obj);
			}

			set { ViewState["EventBarShow"] = value; }
		}

		/// <summary>
		/// Gets or sets the format used to display day(s) duration.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("EventBarDaysString"),
		]
		public string EventBarDaysString
		{
			get
			{
				Object obj = ViewState["EventBarDaysString"];
				return((obj == null) ? "{0} day(s)" : (string)obj);
			}

			set { ViewState["EventBarDaysString"] = value; }
		}

		/// <summary>
		/// Gets or sets the format used to display hour(s) duration.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("EventBarHoursString"),
		]
		public string EventBarHoursString
		{
			get
			{
				Object obj = ViewState["EventBarHoursString"];
				return((obj == null) ? " {0} hour(s)" : (string)obj);
			}

			set { ViewState["EventBarHoursString"] = value; }
		}

		/// <summary>
		/// Gets or sets the format used to display hour(s) duration.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("EventBarMinutesString"),
		]
		public string EventBarMinutesString
		{
			get
			{
				Object obj = ViewState["EventBarMinutesString"];
				return((obj == null) ? " {0} minute(s)" : (string)obj);
			}

			set { ViewState["EventBarMinutesString"] = value; }
		}


		/// <summary>
		/// Gets or sets the color for Event Bar filled element.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("EventBarFilledColor"),
		]
		public Color EventBarFilledColor
		{
			get
			{
				Object obj = ViewState["EventBarFilledColor"];
				return((obj == null) ? Color.Empty : (Color)obj);
			}

			set { ViewState["EventBarFilledColor"] = value; }
		}
		
		/// <summary>
		/// Gets or sets the color for Event Bar empty element.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("EventBarEmptyColor"),
		]
		public Color EventBarEmptyColor
		{
			get
			{
				Object obj = ViewState["EventBarEmptyColor"];
				return((obj == null) ? Color.Empty : (Color)obj);
			}

			set { ViewState["EventBarEmptyColor"] = value; }
		}

		/// <summary>
		/// Gets or sets the format for bottom timescale date in task view.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("TimescaleBottomLabelFormat"),
		]
		public String TimescaleBottomLabelFormat
		{
			get
			{
				Object obj = ViewState["TimescaleBottomLabelFormat"];
				return((obj == null) ? "dd" : (String)obj);
			}

			set { ViewState["TimescaleBottomLabelFormat"] = value; }
		}

		/// <summary>
		/// Gets or sets the format for top timescale date in task view.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("TimescaleTopLabelFormat"),
		]
		public String TimescaleTopLabelFormat
		{
			get
			{
				Object obj = ViewState["TimescaleTopLabelFormat"];
				return((obj == null) ? "dd, MMMM" : (String)obj);
			}

			set { ViewState["TimescaleTopLabelFormat"] = value; }
		}
	

		/// <summary>
		/// Timescale format for top element.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("TimescaleTopSpan"),
		]
		public TimescaleTopSpan TimescaleTopFormat
		{
			get
			{
				Object obj = ViewState["TimescaleTopSpan"];
				return (obj == null) ? TimescaleTopSpan.Week : (TimescaleTopSpan)obj;
			}

			set
			{
				ViewState["TimescaleTopSpan"] = value;
			}
		}

		/// <summary>
		/// Timescale start date. By default is SelectedDate.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("TimescaleStartDate")
		]
		public DateTime TimescaleStartDate
		{
			get
			{
				Object obj = ViewState["TimescaleStartDate"];
				return (obj == null) ? SelectedDate.Date : ((DateTime)obj).Date;
			}
			set
			{
				ViewState["TimescaleStartDate"] = value;
			}
		}

		/// <summary>
		/// Timescale end date.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(typeof(DateTime), ""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("TimescaleEndDate")
		]
		public DateTime TimescaleEndDate
		{
			get
			{
				Object obj = ViewState["TimescaleEndDate"];
				return (obj == null) ? Helper.GetTaskEndDate(SelectedDate).Date : ((DateTime)obj).Date;
			}
			set
			{
				ViewState["TimescaleEndDate"] = value;
			}
		}


		/// <summary>
		/// Gets or sets the the default if we merge events with same name or not.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("MergeEvents"),
		]
		public bool MergeEvents
		{
			get
			{
				Object obj = ViewState["MergeEvents"];
				return((obj == null) ? false : (bool)obj);
			}

			set { ViewState["MergeEvents"] = value; }
		}

		//  [9/15/2004] --- Begin
		/// <summary>
		/// Gets or sets drill down.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue("true"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("DrillDownEnabled"),
		]
		public bool DrillDownEnabled
		{
			get
			{
				Object obj = ViewState["DrillDownEnabled"];
				return((obj == null) ? true : (bool)obj);
			}

			set { ViewState["DrillDownEnabled"] = value; }
		}
		//  [9/15/2004] --- End

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateCollection Dates 
		{
			get
		  {
			return this._Dates;
		  }
		} 


		/*
		/// <summary>
		/// Gets or sets the the default if control shows arrows.
		/// </summary>
		[
		Category("Task View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ShowArrows"),
		]
		public bool ShowArrows
		{
			get
			{
				Object obj = ViewState["ShowArrows"];
				return((obj == null) ? true : (bool)obj);
			}

			set { ViewState["ShowArrows"] = value; }
		}
		*/

		/*
		/// <summary>
		/// Gets or sets the the default button used as left arrow.
		/// </summary>
		[
		Category("Task View"),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("LeftArrowButton"),
		]
		public ImageButton LeftArrowButton
		{
			get
			{
				if(_LeftArrowImageButton == null)
				{
					_LeftArrowImageButton = new ImageButton();
					_LeftArrowImageButton.ImageUrl = SystemImagesPath + "left.gif";
				}
				return _LeftArrowImageButton;
			}

			set { _LeftArrowImageButton = value; }
		}

		/// <summary>
		/// Gets or sets the the default button used as left arrow.
		/// </summary>
		[
		Category("Task View"),
		PersistenceMode(PersistenceMode.InnerProperty),
		ResDescription("RightArrowButton"),
		]
		public ImageButton RightArrowButton
		{
			get
			{
				if(_RightArrowImageButton == null)
				{
					_RightArrowImageButton = new ImageButton();
					_RightArrowImageButton.EnableViewState = true;
					_RightArrowImageButton.ImageUrl = SystemImagesPath + "right.gif";
				}
				return _RightArrowImageButton;
			}

			set { _RightArrowImageButton = value; }
		}
		*/

		/// <summary>
		/// Gets or sets the the default height for week day cell. Default is 180 pixels.
		/// </summary>
		[
		Category("Week View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("WeekDayHeight"),
		]
		public Unit WeekDayHeight
		{
			get
			{
				Object obj = ViewState["WeekDayHeight"];
				return((obj == null) ? 180 : (Unit)obj);
			}

			set { ViewState["WeekDayHeight"] = value; }
		}

		/// <summary>
		/// Gets or sets the the default format for week label.
		/// </summary>
		[
		Category("Week View"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("WeekTopLabelFormat"),
		]
		public string WeekTopLabelFormat
		{
			get
			{
				Object obj = ViewState["WeekTopLabelFormat"];
				if(obj == null)
				{
					if(this.AbbreviatedDayNames)
						return "ddd, MMM dd";
					else
						return "dddd, MMMM dd";
				}
				else
					return obj.ToString();
				//return((obj == null) ? "dddd, MMMM dd" : (string)obj);
			}

			set { ViewState["WeekTopLabelFormat"] = value; }
		}

		/// <summary>
		/// Gets or Sets Work Days Of the Week.
		/// </summary>
		[
		Category("Week View"),
		DefaultValue("Monday, Tuesday"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("WorkWeek"),
		Editor(typeof(Mediachase.Web.UI.WebControls.Design.CheckBoxEditor), typeof(UITypeEditor)),
		]
		public CalendarDayOfWeek WorkWeek
		{
			get
			{
				if(_WorkWeek==0)
					_WorkWeek = CalendarDayOfWeek.Monday;
				return(_WorkWeek);
			}

			set { _WorkWeek = value; }
		}
/*
		public CalendarDayOfWeek WorkWeek
		{
			get
			{
				Object obj = ViewState["WorkWeek"];
				return((obj == null) ? CalendarDayOfWeek.Monday : (CalendarDayOfWeek)obj);
			}

			set { ViewState["WorkWeek"] = value; }
		}

*/

		/// <summary>
		/// Gets or Sets First Day Of the Week.
		/// </summary>
		[
		Category("Week View"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("FirstDayOfWeek")
		]
		public CalendarDayOfWeek FirstDayOfWeek
		{
			get
			{
				return(_FirstDayOfWeek);
			}

			set { _FirstDayOfWeek = value; }
		}
		#endregion

		#region Calendar rendering functions

		/// <summary>
		/// Creates a new collection of child controls for the current control.
		/// </summary>
		/// <returns>A PageViewCollection object that contains the currents control's children.</returns>
		/*
		protected override ControlCollection CreateControlCollection()
		{
			return new CalendarItemCollection2(this);
		}
		*/


		/// <summary>
		/// Overridden. Filters out all objects except CalendarItem objects.
		/// </summary>
		/// <param name="obj">The parsed element.</param>
		protected override void AddParsedSubObject(object obj)
		{
			if (obj is CalendarItem)
			{
				((CalendarItem)obj).ValidateItem();
			}
			base.AddParsedSubObject(obj);
		}


		// this is a nice little utility function which lets me use my specified writer for only this control's tag
		private HtmlTextWriter tagWriter;
		private HtmlTextWriter getCorrectTagWriter( HtmlTextWriter writer ) 
		{
			this.tagWriter = writer;
			if ( writer is System.Web.UI.Html32TextWriter ) 
				this.tagWriter =  new HtmlTextWriter( writer.InnerWriter );
			return this.tagWriter;
		}

		/// <summary>
		/// Prerender Handler
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) 
		{
			base.OnPreRender(e);
			if(base.Page != null)
			{
				//base.Page.RegisterPostBackScript();
			}
		}

		/// <summary>
		/// Overridden. Renders the CalendarItems within the Items collection.
		/// </summary>
		/// <param name="writer">The output stream that renders HTML content to the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			CheckLicense();
			PopulateDatesCollection();
			if(RemoveItems && this.RenderPath != RenderPathID.DesignerPath) // Remove out of scope events
				RemoveUnusedItems();


			// Validate all items
			// this.ValidateControls();

			writer = getCorrectTagWriter(writer);

			// Render main table container
			if (IsDemoVersion == true && !IsDesignMode) 
			{
				Items.Add(new CalendarItem(Items.Count, "<font color=red>This is unlicensed component, please contact <a href=http://center.mediachase.com>Mediachase</a> to get a license.</font>", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), "http://center.mediachase.com", "Get a license today!"));
			}

			switch(ViewType)
			{
				case CalendarViewType.DayView:
					if(this.MultiOwner)
					{
						RenderBeginTable(writer, false); 
						this.RenderDayMultiUser(writer);
						RenderEndTable(writer);
					}
					else
					{
						RenderBeginTable(writer, false); 
						RenderDayViewBar(writer);
						RenderEndTable(writer);
					}
					break;
				case CalendarViewType.WeekView2:
					RenderBeginTable(writer, false); 
					RenderWeekViewBar2(writer);
					RenderEndTable(writer);
					break;
				case CalendarViewType.WeekView:
					RenderBeginTable(writer, false); 
					RenderWeekViewBar(writer);
					RenderEndTable(writer);
					break;
				case CalendarViewType.WorkWeekView:
					RenderBeginTable(writer, false); 
					RenderWorkWeekView(writer);
					RenderEndTable(writer);
					break;
				case CalendarViewType.MonthView:
					RenderBeginTable(writer, false); 
					RenderMonthView(writer);
					RenderEndTable(writer);
					break;
				case CalendarViewType.YearView:
					RenderBeginTable(writer, false); 
					RenderYeahView(writer);
					RenderEndTable(writer);
					break;
				case CalendarViewType.TaskView:
					RenderBeginTable(writer, false); 
					RenderTaskView(writer);
					RenderEndTable(writer);
					break;
				default:
					RenderBeginTable(writer, false); 
					RenderMonthView(writer);
					RenderEndTable(writer);
					break;
			}
		
			// Render main table container
			if (IsDemoVersion == true) 
			{
				RenderText(writer, "<font color=red>This is unlicensed component, please contact <a href=http://center.mediachase.com>Mediachase</a> to get a license.</font>");
			}
		}

		private void ValidateControls()
		{
			if(_IsValidated)
				return;

			foreach(CalendarItem item in Items)
				item.ValidateItem();
			_IsValidated = true;
		}

		/// <summary>
		/// Removes unused items from the collection. It's usefull because alot of
		/// recursions are done with a collection.
		/// </summary>
		private void RemoveUnusedItems()
		{			
			DateTime start = this.DisplayStartDate;
			DateTime end = this.DisplayEndDate;

			ClearChildViewState();
			for(int index = Items.Count - 1; index>0; index--)
			{
				CalendarItem item = Items[index];
				if((item.StartDate >= start && item.StartDate <= end) || (item.EndDate >= start && item.EndDate <= end) || (item.EndDate >= end && item.StartDate <= start)){}
				else{Items.Remove(item);Controls.Remove(item);}
			}
			this.TrackViewState();
		}

		/// <summary>
		/// Populates dates collection with currently displayed dates within calendar
		/// </summary>
		private void PopulateDatesCollection()
		{
			switch(ViewType)
			{
				case CalendarViewType.WorkWeekView:
					for(DateTime cycleDate = this.DisplayStartDate; cycleDate < this.DisplayEndDate; cycleDate = cycleDate.AddDays(1))
					{
						if((Helper.GetCalDayOfWeek(cycleDate.DayOfWeek)&this.WorkWeek)!=0)
							Dates.Add(cycleDate);
					}
					break;
				default:
					for(DateTime cycleDate = this.DisplayStartDate; cycleDate < this.DisplayEndDate; cycleDate = cycleDate.AddDays(1))
						Dates.Add(cycleDate);
					break;
			}			
		}

		/// <summary>
		/// The rendering path for uplevel browsers.
		/// </summary>
		/// <param name="writer">The output stream that renders HTML content to the client.</param>
		protected override void RenderUpLevelPath(HtmlTextWriter writer)
		{
			RenderDownLevelPath(writer);
		}

		/// <summary>
		/// The rendering path for downlevel browsers.
		/// </summary>
		/// <param name="writer">The output stream that renders HTML content to the client.</param>
		protected override void RenderDownLevelPath(HtmlTextWriter writer)
		{
			base.RenderDownLevelPath(writer);
		}


		/// <summary>
		/// Performs a case insensitive check of the Style collection by looping through it.
		/// CssStyleCollection is case sensitive unlike CssCollection.
		/// </summary>
		/// <param name="name">The name to search for.</param>
		/// <returns>The value or null.</returns>
		private string GetStyle(string name)
		{
			// First try it in case it works
			string val = Style[name];
			if (val != null)
				return val;

			// Go for a case insensitive search
			foreach (string key in Style.Keys)
			{
				if (String.Compare(key, name, true) == 0)
				{
					return Style[key];
				}
			}

			return null;
		}

		/// <summary>
		/// Tests for the existence of a border style being set.
		/// </summary>
		/// <param name="type">The type of border setting (color, width, style).</param>
		/// <returns>true if nothing was set.</returns>
		private bool NotExistBorder(string type)
		{
			return ((GetStyle("border") == null) && (GetStyle("border-" + type) == null) &&
				(GetStyle("border-top") == null) && (GetStyle("border-top-" + type) == null) &&
				(GetStyle("border-bottom") == null) && (GetStyle("border-bottom-" + type) == null) &&
				(GetStyle("border-left") == null) && (GetStyle("border-left-" + type) == null) &&
				(GetStyle("border-right") == null) && (GetStyle("border-right-" + type) == null));
		}

		/// <summary>
		/// Opens the downlevel table.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the control content.</param>
		/// <param name="isDesignMode">Indicates whether this is design mode.</param>
		private void RenderBeginTable(HtmlTextWriter writer, bool isDesignMode)
		{
			((TableStyle)ControlStyle).GridLines = GridLines.Vertical;
			AddAttributesToRender(writer);

			String color = "black";

			color = getPaletteColorString(CalendarColorConstants.DefaultBackColor);

			if(!this.ControlStyle.BorderColor.IsEmpty)
				color = ColorTranslator.ToHtml(ControlStyle.BorderColor);
			
			if(!this.ControlStyle.BorderColor.IsEmpty)
				color = ColorTranslator.ToHtml(ControlStyle.BorderColor);
			
			if(this.ControlStyle.BorderStyle == BorderStyle.NotSet)
				this.ControlStyle.BorderStyle = BorderStyle.Solid;
				//writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle, BorderStyle.Solid.ToString());

			//if(((TableStyle)ControlStyle).CellSpacing == )
				//writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");

			//((TableStyle)ControlStyle).CellSpacing = 0;
			//((TableStyle)ControlStyle).CellPadding = 1;

			//if(((TableStyle)ControlStyle).CellPadding == null)
				//writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "1");
			
			//AK fixing
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "table");
			//END AK

			writer.AddAttribute(HtmlTextWriterAttribute.Bordercolor, color);
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
		}

		/// <summary>
		/// Closes the downlevel table.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the control content.</param>
		private void RenderEndTable(HtmlTextWriter writer)
		{
			//writer.RenderEndTag();      // TD
			//writer.RenderEndTag();      // TR
			writer.RenderEndTag();      // TABLE
		}

		/// <summary>
		/// Renders the text property.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		/// <param name="text">The text to render.</param>
		protected virtual void RenderText(HtmlTextWriter writer, string text)
		{
			writer.Write(text);
		}

		public void AddSpaceImage(HtmlTextWriter writer, int width, int height)
		{
			if (this.Page != null)
				writer.AddAttribute(HtmlTextWriterAttribute.Src, this.Page.ResolveUrl(this.SystemImagesPath + "spacer.gif"));
			writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Height, height.ToString());
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
		}

		#endregion

		#region Calendar Month View
		/// <summary>
		/// Renders daily view, which displays a typical calendar 
		/// month with events rendered within a day cell.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		private void RenderMonthView(HtmlTextWriter writer)
		{
			DateTime cycleDate = DateTime.MinValue;
			if(MonthStartDate>DateTime.MinValue)
			{
				DateTime monthStart = MonthStartDate;
				cycleDate = monthStart.AddDays(-Helper.LocalizedDayOfWeek(monthStart.DayOfWeek, this.FirstDayOfWeek));
			}
			else
				cycleDate = Helper.GetMonthStartDate(SelectedDate, this.MonthStartDate, this.FirstDayOfWeek);

			// Create weekday row
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			for(int index=0;index<7;index++)//string week in this.getDayNames())
			{
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "17");
				//RenderPaletteColor(writer, HtmlTextWriterStyle.BorderColor, CalendarHeaderStyle.BorderColor, CalendarColorConstants.DefaultBackColor);
				RenderCellBorderAttributes(writer);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				if(this.AbbreviatedDayNames)
					RenderText(writer, cycleDate.ToString("ddd"));
				else
					RenderText(writer, cycleDate.ToString("dddd"));
				writer.RenderEndTag();
				cycleDate = cycleDate.AddDays(1);
			}

			cycleDate = cycleDate.AddDays(-7);

			// End weekday row
			writer.RenderEndTag();

			// Internal dates - month displayed
			for(int weekIndex=0;weekIndex<MONTH_NO_WEEKS;weekIndex++)
				RenderMonthWeekBar(writer, ref cycleDate);
		}

		/// <summary>
		/// Shared code used by Month view.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="cycleDate"></param>
		private void RenderMonthWeekBar(HtmlTextWriter writer, ref DateTime cycleDate)
		{
			int DaysToRender = 7;

			// Created matrix
			Matrix matrix = (Matrix)CreateMatrix(cycleDate.Date, cycleDate.Date.AddDays(DaysToRender), MatrixSpan.DaySpan);

			int EventPerDayMax = matrix.Rows;

			// Render day links
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			for(int weekdayIndex=0;weekdayIndex<DaysToRender;weekdayIndex++)
			{
				AddMonthCellAttributes(writer, cycleDate);
				//if(EventPerDayMax == 0)
				//	writer.AddAttribute(HtmlTextWriterAttribute.Height, "70");

				// Add border
				RenderCellTopBorderAttributes(writer);
				writer.AddAttribute(HtmlTextWriterAttribute.Height, ItemHeight.ToString());
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				// Start new table to allow aligning
				writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, Unit.Percentage(100).ToString());
				writer.RenderBeginTag(HtmlTextWriterTag.Table);
				writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, "10");
				AddMonthCellAttributes(writer, cycleDate);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				AddMonthLinkTag(writer, cycleDate);
				writer.RenderEndTag(); // td
				writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
				writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
				Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
				Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
				AddMonthCellAttributes(writer, cycleDate);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				RenderHolidayHeader(writer, cycleDate);
				writer.RenderEndTag(); // td
				writer.RenderEndTag(); // table

				writer.RenderEndTag(); // date link td
			
				cycleDate = cycleDate.AddDays(1);
			}
			writer.RenderEndTag(); // date link tr
			
			DateTime newDate = new DateTime(cycleDate.Ticks);
			// render matrix / day bars
			int dayIndex = 1; // number of days to add in cycle, based on colspan
			for(int rowIndex=0;rowIndex<EventPerDayMax;rowIndex++)
			{
				// return cycle back one week
				newDate = newDate.AddDays(-DaysToRender);

				writer.RenderBeginTag(HtmlTextWriterTag.Tr);

				for(int columnIndex=0;columnIndex<DaysToRender;)
				{
					if(matrix[rowIndex, columnIndex]>=0)
					{
						CalendarItem item = Items[(int)matrix[rowIndex, columnIndex]];

						dayIndex = AddMonthItemAttributes(writer, newDate, item);
						columnIndex += dayIndex;
						item.SetRenderedDate(newDate);
						item.Render(writer, RenderPath);
						writer.RenderEndTag(); // item td
					}
					else if(matrix[rowIndex, columnIndex]==(int)MatrixConstants.EmptyField)
					{
						AddMonthCellAttributes(writer, newDate);
						Helper.AddLinkAttributes(writer, NewLinkFormat, newDate);
						Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag(); // item td
						dayIndex = 1;
						columnIndex++;
					}
					else if(matrix[rowIndex, columnIndex]==(int)MatrixConstants.MoreElementsField)
					{
						AddMonthCellAttributes(writer, newDate);
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
            string href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + newDate.ToShortDateString() + "," + CalendarViewType.DayView.GetHashCode()); 
						writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
						writer.RenderBeginTag(HtmlTextWriterTag.A);
						writer.Write(this.MoreItemsText);
						writer.RenderEndTag(); // </a>
						writer.RenderEndTag(); // item td
						dayIndex = 1;
						columnIndex++;
					}
					else if(matrix[rowIndex, columnIndex]==(int)MatrixConstants.SpanField)
					{
						throw new Exception(String.Format("Incorrect matrix value"));
					}

					newDate = newDate.AddDays(dayIndex);
				}
				writer.RenderEndTag(); // item tr
			}

			// return cycle back one week
			cycleDate = cycleDate.AddDays(-DaysToRender);

			// Render space cells
			this.RenderText(writer, "<!-- space cell -->");
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			for(int weekdayIndex=0;weekdayIndex<DaysToRender;weekdayIndex++)
			{
				AddMonthCellAttributes(writer, cycleDate);
				if(EventPerDayMax == 0)
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "70");
				else if(EventPerDayMax == 1)
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "50");
				else if(EventPerDayMax == 2)
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "30");
				else if(EventPerDayMax == 3)
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");

				Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
				Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.RenderEndTag(); // space td

				cycleDate = cycleDate.AddDays(1);
			}
			writer.RenderEndTag(); // space tr
			this.RenderText(writer, "<!-- end: space cell -->");

		}

		private void AddMonthLinkTag(HtmlTextWriter writer, DateTime cycleDate)
		{
			// render link
			if (this.DrillDownEnabled)
			{
        string href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + cycleDate.ToShortDateString() + "," + CalendarViewType.DayView.GetHashCode()); 

				writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
				writer.RenderBeginTag(HtmlTextWriterTag.A);

				if(cycleDate.Day == 1)
					RenderText(writer, "<nobr>" + cycleDate.ToString("MMM") + " " + cycleDate.Day.ToString() + "</nobr>");
				else
					RenderText(writer, "<nobr>" + cycleDate.Day.ToString() + "</nobr>");
					
				writer.RenderEndTag();
			}
			else
			{
				string href = cycleDate.ToShortDateString(); 

				writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				if(cycleDate.Day == 1)
					RenderText(writer, "<nobr>" + cycleDate.ToString("MMM") + " " + cycleDate.Day.ToString() + "</nobr>");
				else
					RenderText(writer, "<nobr>" + cycleDate.Day.ToString() + "</nobr>");
					
				writer.RenderEndTag();
			}
		}

		private void RenderHolidayHeader(HtmlTextWriter writer, DateTime cycleDate)
		{
			if(Holidays.IsHoliday(cycleDate))
			{
				writer.AddStyleAttribute("overflow", "none");
				writer.RenderBeginTag(HtmlTextWriterTag.Span);
				foreach(Holiday item in Holidays)
				{
					if(item.IsHoliday(cycleDate))
						RenderText(writer, item.Name+" "); // [13/1/2005] - + " "
				}
				writer.RenderEndTag();
			}					
		}

		// June 16 04: added .AddTicks(-1) method call to endDate to allow filter events that end at 0:00
		private int AddMonthItemAttributes(HtmlTextWriter writer, DateTime cycleDate, CalendarItem item)
		{
			int colSpan = 1;
				
			colSpan = (Helper.GetDaySpan(item.StartDate, item.AdjustedEndDate) + 1);

			// detect if events spans from previous week
			if(item.StartDate.Date<cycleDate.Date)
			{
				// Calculate span from current date
				colSpan = (Helper.GetDaySpan(cycleDate.Date, item.AdjustedEndDate) + 1);
			}

			// detect if event spans to next week
			if(colSpan>(7 - Helper.LocalizedDayOfWeek(cycleDate.DayOfWeek, this.FirstDayOfWeek)))
			{
				colSpan = (7 - Helper.LocalizedDayOfWeek(cycleDate.DayOfWeek, this.FirstDayOfWeek));
			}

			if(item.getRenderStyle() == CalendarItemRenderStyle.Box)
			{
				CalendarItemDefaultStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);
			}
			else
				AddMonthCellColorAttributes(writer, cycleDate);
			// [13/1/2005] - Begin
			if(colSpan>0) // [13/1/2005] - End
				writer.AddAttribute("colspan", colSpan.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Height, ItemHeight.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);

			return colSpan;
		}

		private void AddMonthCellAttributes(HtmlTextWriter writer, DateTime cycleDate)
		{
			AddMonthCellColorAttributes(writer, cycleDate);
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "14%");
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
		}

		private void AddMonthCellColorAttributes(HtmlTextWriter writer, DateTime cycleDate)
		{
			// Internal dates - month displayed
			DateTime monthCurrent = this.SelectedDate.Date;
			DateTime monthStart = DateTime.MinValue;
			if (this.MonthStartDate>DateTime.MinValue)
				monthStart = this.MonthStartDate;
			else
				monthStart = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
			DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);
			int weekDay = Helper.LocalizedDayOfWeek(monthStart.DayOfWeek, this.FirstDayOfWeek);

			if(Holidays.IsHoliday(cycleDate.Date))
			{
				CalendarItemHolidayStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemHolidayStyle.BackColor, CalendarColorConstants.ItemInactiveBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemHolidayStyle.ForeColor, CalendarColorConstants.ItemInactiveForeColor);
				return;
			}

			if(cycleDate.Date < monthStart) // prev month
			{
				CalendarItemInactiveStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemInactiveBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemInactiveForeColor);
			}
			else if(cycleDate.Date > monthEnd) // next month
			{
				CalendarItemInactiveStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemInactiveBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemInactiveForeColor);
			}
			else if(cycleDate.Date == HighlightedDate.Date) // current date
			{
				CalendarItemSelectedStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemSelectedStyle.BackColor, CalendarColorConstants.ItemSelectedBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemSelectedStyle.ForeColor, CalendarColorConstants.ItemSelectedForeColor);
			}
			else
			{

				CalendarItemDefaultStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);
			}
		}

		#endregion

		#region Calendar Year Views
		/// <summary>
		/// Renders monthly view, which displays a a year with 4 qauters defined, where events
		/// rendered inside each month.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		private void RenderYeahView(HtmlTextWriter writer)
		{
			// Create month row
			DateTime currentDate = SelectedDate;
			
			DateTime cycleDate = currentDate.AddMonths(-currentDate.Month + 1);
			for(int index=0;index<4;index++) 
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				
				// Render quarter header
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

				writer.AddAttribute(HtmlTextWriterAttribute.Width, "40");
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "70");
				writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
				RenderCellBorderAttributes(writer);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				RenderText(writer, String.Format("{0}", index+1));
				writer.RenderEndTag();

				for(int quarterIndex=0;quarterIndex<3;quarterIndex++)
				{
					if(cycleDate.Month == HighlightedDate.Month && cycleDate.Year == HighlightedDate.Year) // current month, select
					{
						CalendarItemSelectedStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemSelectedStyle.BackColor, CalendarColorConstants.ItemSelectedBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemSelectedStyle.ForeColor, CalendarColorConstants.ItemSelectedForeColor);

						AddHoverAttributesToRender(writer, CalendarItemSelectedStyle);
					}
					else
					{
						CalendarItemDefaultStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);

						AddHoverAttributesToRender(writer, CalendarItemDefaultStyle);
					}

					writer.AddAttribute(HtmlTextWriterAttribute.Width, "33%");
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "70");
					writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
					RenderCellBorderAttributes(writer);
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					string href = "";

					if (this.DrillDownEnabled)
					{
						// render link
            href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + cycleDate.ToShortDateString() + "," + CalendarViewType.MonthView.GetHashCode()); 

						writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
						writer.RenderBeginTag(HtmlTextWriterTag.A);

						if(cycleDate.Month == 1)
							RenderText(writer, String.Format("{1}, {0}", cycleDate.ToString("yyyy"), cycleDate.ToString("MMMM")));
						else
							RenderText(writer, cycleDate.ToString("MMMM"));
					
						writer.RenderEndTag();
					}
					else
					{
						// render text
						href = cycleDate.ToShortDateString(); 

						writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
						writer.RenderBeginTag(HtmlTextWriterTag.Div);

						if(cycleDate.Month == 1)
							RenderText(writer, String.Format("{1}, {0}", cycleDate.ToString("yyyy"), cycleDate.ToString("MMMM")));
						else
							RenderText(writer, cycleDate.ToString("MMMM"));
					
						writer.RenderEndTag();
					}
					
					// Render items within that date
					int iDisplayedItems = 0; // counts items already rendered
					foreach (CalendarItem item in Items)
					{
						bool renderItem = false;
						if(item.StartDate.Date.Month == cycleDate.Date.Month && item.StartDate.Date.Year == cycleDate.Date.Year)
							renderItem = true;
						else if(item.StartDate.Date.Month <= cycleDate.Date.Month && item.StartDate.Date.Year <= cycleDate.Date.Year && item.EndDate.Date.Month >= cycleDate.Date.Month && item.EndDate.Date.Year >= cycleDate.Date.Year)
							renderItem = true;

						if(renderItem)
						{
							if(iDisplayedItems>=MaxDisplayedItems && MaxDisplayedItems != 0) // don't render anymore items, just render muli language "..."
							{
								writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
								writer.RenderBeginTag(HtmlTextWriterTag.A);
								RenderText(writer, "<br>" + MoreItemsText);
								writer.RenderEndTag();
								break;
							}
							item.SetRenderedDate(cycleDate);
							item.Render(writer, RenderPath);
							iDisplayedItems++;
						}
					}

					writer.RenderEndTag();

					cycleDate = cycleDate.AddMonths(1);
				}
				
				writer.RenderEndTag();
			}
		}
		#endregion

		#region Calendar Week View
		private void RenderWeekViewBar(HtmlTextWriter writer)
		{
			int width = 0;
			DateTime cycleDate = SelectedDate.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek));

			// Create inverse matrix
			Matrix matrix = (Matrix)CreateMatrix(cycleDate.Date, cycleDate.Date.AddDays(7), MatrixSpan.DaySpan);
			matrix = (Matrix)matrix.Transpose();

			// Calculate maximum number of columns
			// 1. Cycle through all items and determine
			// max columns
			int NumberOfColumnsMax = matrix.Columns;

			// Render data
			for(int index=0;index<7;index++) 
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				
				// Render quarter header
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

				writer.AddAttribute(HtmlTextWriterAttribute.Width, "1%");
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "70");
				writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
				RenderCellBorderAttributes(writer);

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				string href = "";
				
				if (this.DrillDownEnabled)
				{
					// render link
          href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + cycleDate.ToShortDateString() + "," + CalendarViewType.DayView.GetHashCode()); 
					writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
					CalendarHeaderStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

					writer.RenderBeginTag(HtmlTextWriterTag.A);
					if(this.AbbreviatedDayNames)
						RenderText(writer, String.Format("{0}<br>{1}", cycleDate.ToString("ddd"), cycleDate.Day));
					else
						RenderText(writer, String.Format("{0}<br>{1}", cycleDate.ToString("dddd"), cycleDate.Day));

					writer.RenderEndTag(); // </A>
				}
				else
				{
					href = cycleDate.ToShortDateString(); 
					writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
					CalendarHeaderStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

					writer.RenderBeginTag(HtmlTextWriterTag.Div);
					if(this.AbbreviatedDayNames)
						RenderText(writer, String.Format("{0}<br>{1}", cycleDate.ToString("ddd"), cycleDate.Day));
					else
						RenderText(writer, String.Format("{0}<br>{1}", cycleDate.ToString("dddd"), cycleDate.Day));

					writer.RenderEndTag();
				}

				writer.RenderEndTag(); // </TD>

				int RenderedColumns = 0;
				int RenderedSpaceColumns = 0;
				for(int columnIndex=0;columnIndex<NumberOfColumnsMax;columnIndex++)
				{
					if(matrix[index, columnIndex] >= 0)
					{
						CalendarItem item = Items[(int)matrix[index, columnIndex]];

						// Render current one
						AddWeekCellAttributes(writer, cycleDate);
						int rowSpan = AddWeekItemAttributes(writer, cycleDate, item);
						width = (int)(100 / (NumberOfColumnsMax));
						writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						item.SetRenderedDate(cycleDate);
						item.Render(writer, RenderPath);
						writer.RenderEndTag();
						RenderedColumns++;
					}
					else if(matrix[index, columnIndex] == (int)MatrixConstants.SpanField) // found row spanned event
					{
						// Find out if we need to add colspan column here or not
						// 1. Analize matrix to determine actual column position = matrix column
						//		1a. Maybe that can be done during matrix population by putting event only in the column
						//		it will be
						// 2. Analize matrix to determine if any items will be created in this row
						//		Do it by checking RenderedColumns property
						// 3. Add this column as rendered
						//		because it's spanned column
						// 4. Render space column
						//		render space column with colspan = column number
						//		add one rendered column
						

						// Step 1:
						RenderedSpaceColumns+=columnIndex;

						// Step 2:
						RenderedSpaceColumns-=RenderedColumns;

						// Step 3:
						RenderedColumns++;

						// Step 4:
						if(RenderedSpaceColumns>0) // render space column
						{
							AddWeekCellAttributes(writer, cycleDate);
							width = (int)(100 / (NumberOfColumnsMax));
							writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Colspan, RenderedSpaceColumns.ToString());
							Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
							Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
							RenderedColumns+=RenderedSpaceColumns;
							RenderedSpaceColumns = 0;
						}
					}
					//else if(matrix[index, columnIndex] == -1) // found empty row
					//{
					//	RenderedColumns++;
					//}
				}

				//RenderedColumns+=RenderedSpaceColumns;
				// Fill rest of the space
				if(NumberOfColumnsMax-(RenderedColumns) > 0)
				{
					AddWeekCellAttributes(writer, cycleDate);
					writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
					writer.AddAttribute(HtmlTextWriterAttribute.Colspan, (NumberOfColumnsMax-(RenderedColumns)).ToString());
					Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
					Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					writer.RenderEndTag();
				}
				else if(RenderedColumns==0)
				{
					AddWeekCellAttributes(writer, cycleDate);
					writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
					Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
					Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					writer.RenderEndTag();
				}


				cycleDate = cycleDate.AddDays(1);
				
				writer.RenderEndTag();
			}
		}

		private void AddWeekCellAttributes(HtmlTextWriter writer, DateTime cycleDate)
		{
			String height = WeekDayHeight.ToString();
			if(cycleDate.Date == HighlightedDate.Date) // current day, select
			{
				CalendarItemSelectedStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemSelectedStyle.BackColor, CalendarColorConstants.ItemSelectedBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemSelectedStyle.ForeColor, CalendarColorConstants.ItemSelectedForeColor);
			}
			else if(Helper.IsWeekend(cycleDate))
			{
				height = (WeekDayHeight.Value / 2).ToString();
				CalendarItemInactiveStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemInactiveBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemInactiveForeColor);

				AddHoverAttributesToRender(writer, CalendarItemInactiveStyle);
			}
			else
			{
				CalendarItemDefaultStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);

				AddHoverAttributesToRender(writer, CalendarItemDefaultStyle);
			}

			// Netscape trick with 1% seem to fill full td tag
			// 100% and 70 didn't work for netscape
			//writer.AddAttribute(HtmlTextWriterAttribute.Height, "1%");
			writer.AddAttribute(HtmlTextWriterAttribute.Height, height);
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			RenderCellBorderAttributes(writer);
		}

		private int AddWeekItemAttributes(HtmlTextWriter writer, DateTime cycleDate, CalendarItem item)
		{
			//TimeSpan spanTime = item.EndDate - item.StartDate;
			int colSpan = (Helper.GetDaySpan(item.StartDate, item.EndDate) + 1);

			// detect if events spans from previous week
			if(item.StartDate.Date<cycleDate.Date)
			{
				// Calculate span from current date
				//TimeSpan spanTime2 = item.EndDate - cycleDate;
				colSpan = (Helper.GetDaySpan(cycleDate, item.EndDate) + 1);
			}

			// detect if event spans to next week
			if(this.ViewType == CalendarViewType.WeekView)
			{
				if(colSpan>(7 - cycleDate.DayOfWeek.GetHashCode()))
					colSpan = (7 - cycleDate.DayOfWeek.GetHashCode()) + 1;
			}
			else
			{
				if(colSpan>1)
					colSpan = 1;
			}
			// [13/1/2005] - Begin
			if(colSpan>0) // [13/1/2005] - End
				writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, colSpan.ToString());
			//AddMonthCellAttributes(writer, cycleDate);
			return colSpan;
		}

		private void RenderWeekViewBarDay2(HtmlTextWriter writer, DateTime now)
		{
			//RenderCellBorderAttributes(writer);
			AddWeekCellAttributes(writer, now);
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "50%");
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
		
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			(new CalendarDay(this, now.Date, false, now.Date == SelectedDate.Date, HighlightedDate.Date == now.Date)).Render(writer, NewLinkFormat, ContextMenuFormat);			
			writer.RenderEndTag();
		}

		private void RenderWeekViewBarDayHeader2(HtmlTextWriter writer, DateTime now)
		{
			CalendarHeaderStyle.AddAttributesToRender(writer);
			RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
			RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

			writer.AddAttribute(HtmlTextWriterAttribute.Width, "50%");
			writer.AddAttribute(HtmlTextWriterAttribute.Height, "33%");
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			RenderCellBorderAttributes(writer);

			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			string href = "";

			if (this.DrillDownEnabled)
			{
        href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + now.ToShortDateString() + "," + CalendarViewType.DayView.GetHashCode()); 
				writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.Write(now.ToString(WeekTopLabelFormat));
				writer.RenderEndTag(); // </A>
			}
			else
			{
				href = now.ToShortDateString(); 
				writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

				writer.RenderBeginTag(HtmlTextWriterTag.Div);
				writer.Write(now.ToString(WeekTopLabelFormat));
				writer.RenderEndTag();
			}

			writer.RenderEndTag();//td
		}

		private void RenderWeekViewBar2(HtmlTextWriter writer)
		{
			DateTime cycleDate = SelectedDate.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek));

			if(cycleDate.DayOfWeek == DayOfWeek.Monday) // when week starts on monday move one day backwards
				cycleDate = cycleDate.AddDays(-1);
			else if(cycleDate.DayOfWeek == DayOfWeek.Saturday) // Arabic starts on Saturday
				cycleDate = cycleDate.AddDays(-1);
			
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			// Monday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(1));
			// Thursday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(4));
			writer.RenderEndTag();//tr

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			// Monday
			RenderWeekViewBarDay2(writer, cycleDate.AddDays(1));
			
			// Thursday
			RenderWeekViewBarDay2(writer, cycleDate.AddDays(4));
			

			writer.RenderEndTag(); //tr

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			// Tuesday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(2));
			// Friday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(5));
			writer.RenderEndTag();//tr
			
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			// Tuesday	
			RenderWeekViewBarDay2(writer, cycleDate.AddDays(2));
			
			// Friday	
			RenderWeekViewBarDay2(writer, cycleDate.AddDays(5));
			

			writer.RenderEndTag(); //tr

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			// Wednesday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(3));
			// Saturday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(6));
			writer.RenderEndTag();//tr
			
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			// Wednesday	
			writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "3");
			RenderWeekViewBarDay2(writer, cycleDate.AddDays(3));
			
			// Weekend
			// Saturday
			//RenderCellBorderAttributes(writer);
			DateTime now = cycleDate.AddDays(6);
			AddWeekCellAttributes(writer, now);
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "50%");
			//writer.AddAttribute(HtmlTextWriterAttribute.Height, (WeekDayHeight.Value / 2).ToString());
			//writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			(new CalendarDay(this, now, false, now.Date == SelectedDate.Date, HighlightedDate.Date == now)).Render(writer, NewLinkFormat, ContextMenuFormat);
			writer.RenderEndTag();

			writer.RenderEndTag();//tr

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			// Sunday header
			RenderWeekViewBarDayHeader2(writer, cycleDate.AddDays(7));
			writer.RenderEndTag();//tr

			// Sunday
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			//RenderCellBorderAttributes(writer);
			//writer.AddAttribute(HtmlTextWriterAttribute.Height, "90");
			AddWeekCellAttributes(writer, now);
			//writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			now = cycleDate.AddDays(7);
			(new CalendarDay(this, now, false, now.Date == SelectedDate.Date, HighlightedDate.Date == now)).Render(writer, NewLinkFormat, ContextMenuFormat);
			writer.RenderEndTag(); //td

			writer.RenderEndTag();//tr
		}

		private void RenderWorkWeekView(HtmlTextWriter writer)
		{
			int width = 0;
			//DateTime currentDate = SelectedDate.AddDays(-Helper.LocalizedDayOfWeek(SelectedDate.DayOfWeek, this.FirstDayOfWeek));
			DateTime currentDate = DisplayStartDate;

			// Render date
			DateTime cycleDate = currentDate.Date;

			Matrix matrix = null; // generic matrix

			//if(currentDate.DayOfWeek == DayOfWeek.Sunday) // US Format, add 1 day to start from monday
			//	currentDate = currentDate.AddDays(1);

			// List of matrices
			ArrayList matrixList = new ArrayList();

			// Maximum Number of total columns
			int TotalNumberOfColumnsMax = 0;

			// Total span, used by multi day events
			int TotalSpan = 0;

			// Create array of inverse matrixes
			for(int dayofweek = 0;dayofweek<7;dayofweek++)
			{
				if(Dates.Contains(currentDate.Date))
				{
					matrix = (Matrix)CreateMatrix(currentDate.Date, currentDate.Date.AddDays(1).AddTicks(-1), MatrixSpan.WeekHourSpan);
					matrix = (Matrix)matrix.Transpose();
					TotalNumberOfColumnsMax += (UInt16)matrix.Columns;

					// Calculate total span here
					TotalSpan += matrix.Columns <= 0 ? 1 : matrix.Columns;

					// Crate a clone and add matrix to the list
					matrixList.Add(matrix.Clone());
				}
				else
				{
					matrixList.Add(null); // add placeholder
				}
				currentDate = currentDate.AddDays(1);
			}
			currentDate = currentDate.AddDays(-7);
			
			// Create weekday row
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			CalendarHeaderStyle.AddAttributesToRender(writer);
			RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
			RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
			writer.AddAttribute(HtmlTextWriterAttribute.Height, "17");
			//RenderCellBorderAttributes(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			writer.RenderEndTag();

			for(int weekIndex = 0; weekIndex < 7; weekIndex++)
			{
				if(Dates.Contains(currentDate.Date))
				{
					CalendarHeaderStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "17");
					int _col_span = ((Matrix)matrixList[weekIndex]).Columns;
					if(_col_span>0)
						writer.AddAttribute(HtmlTextWriterAttribute.Colspan, ((Matrix)matrixList[weekIndex]).Columns.ToString());
					RenderCellBorderAttributes(writer);
					//RenderPaletteColor(writer, HtmlTextWriterStyle.BorderColor, CalendarHeaderStyle.BorderColor, CalendarColorConstants.DefaultBackColor);
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					string href = "";
					if (this.DrillDownEnabled)
					{
            href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + currentDate.ToShortDateString() + "," + CalendarViewType.DayView.GetHashCode()); 
						writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
						writer.RenderBeginTag(HtmlTextWriterTag.A);
						RenderText(writer, currentDate.ToString(WeekTopLabelFormat));
						writer.RenderEndTag(); // </A>
					}
					else
					{
						href = currentDate.ToShortDateString(); 
						writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
						writer.RenderBeginTag(HtmlTextWriterTag.Div);
						RenderText(writer, currentDate.ToString(WeekTopLabelFormat));
						writer.RenderEndTag();
					}

					writer.RenderEndTag();
				}
				currentDate = currentDate.AddDays(1);
			}

			currentDate = currentDate.AddDays(-7);

			// End weekday row
			writer.RenderEndTag();

			bool isRenderedTopBorder = false;

			// Render Multi Day Items
			RenderWorkWeekMultiDayBar(writer, ref cycleDate, matrixList);
			// END: Rendering MultiDay Items
			
			// Precalculate date start time and end time
			int StartHourIndex = this.DayStartHour; // By default starts at 0 hour
			int EndHourIndex = this.DayEndHour; // By default ends at 23 hour
			bool renderCurrentHour = false;

			for(int index=0;index<24*2;index++)
			{
				// don't render items that happen later
				if((int)index/2 > this.DayEndHour)
					renderCurrentHour = false;

				// determines if we render this hour or not
				// we render if either items present or it's within active borders
				for(int dayofweek=0;dayofweek<7;dayofweek++)
				{
					if(Dates.Contains(currentDate.Date))
					{
						// substitute matrix
						matrix = (Matrix)matrixList[dayofweek];
						int NumberOfColumnsMax = matrix.Columns;
						for(int columnIndex=0;columnIndex<NumberOfColumnsMax;columnIndex++)
						{
							if(matrix[index, columnIndex] != (int)MatrixConstants.EmptyField)
								renderCurrentHour = true;

							for(int index2=index;index2<24*2;index2++)
								if(matrix[index2, columnIndex] != (int)MatrixConstants.EmptyField && (int)index/2>this.DayEndHour)
									renderCurrentHour = true;
						}

						// always render active hours
						if((int)index/2 >= this.DayStartHour && (int)index/2 <= this.DayEndHour)
							renderCurrentHour = true;

						if(renderCurrentHour)
						{
							if((int)(index/2)<StartHourIndex)
								StartHourIndex = (int)(index/2);
							else if((int)(index/2)>EndHourIndex)
								EndHourIndex = (int)(index/2);
						}
					}
					currentDate = currentDate.AddDays(1);
				}
				currentDate = currentDate.AddDays(-7);	
				
			}

			// Adjust current date using calculations from above
			cycleDate = cycleDate.AddHours(StartHourIndex);

			renderCurrentHour = false;
			// Render Day

			//  [9/9/2004] -- Begin
			int _Max = 1;
			for(int dayofweek=0;dayofweek<7;dayofweek++)
			{
				matrix = (Matrix)matrixList[dayofweek];	
				if(matrix!=null && matrix.Columns>_Max)
					_Max = matrix.Columns;
			}
			int[,] _repeat = new int[7,_Max];
			for(int i=0; i<7; i++)
				for(int j=0; j<_Max; j++)
				_repeat[i,j]=0;
			//  [9/9/2004] -- End
			
			for(int index=StartHourIndex*2;index<(EndHourIndex+1)*2;index++)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);

				// Render hour header
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

				writer.AddAttribute(HtmlTextWriterAttribute.Width, "1%");
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "20");
				writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
				writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
				writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "1");
			
				if(index%2!=0)
					RenderCellBorderAttributes(writer);

				if(!isRenderedTopBorder)
					RenderCellTopBorderAttributes(writer);

				writer.RenderBeginTag(HtmlTextWriterTag.Td);
			
				// Output hour every other cell
				if(index%2==0)
				{
					// Render linked hour if format is specified
					if(NewLinkFormat.Length > 0)
					{
						string href = "javascript:" + String.Format(NewLinkFormat, cycleDate);
						writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

						writer.RenderBeginTag(HtmlTextWriterTag.A);
						writer.Write("<nobr>" + cycleDate.ToString(DayHourFormat) + "</nobr>");
						
						writer.RenderEndTag(); // </A>
					}
					else
						RenderText(writer, String.Format("<nobr>{0}</nobr>", cycleDate.ToString(DayHourFormat)));
				}

				writer.RenderEndTag();
				
				for(int dayofweek=0;dayofweek<7;dayofweek++)
				{
					if(Dates.Contains(cycleDate.Date))
					{
						// substitute matrix
						matrix = (Matrix)matrixList[dayofweek];
						
						int NumberOfColumnsMax = matrix.Columns;

						// CODE FROM DAY RENDERING
						// Rendering elements
						int RenderedColumns = 0;
						int RenderedSpaceColumns = 0;
						for(int columnIndex=0;columnIndex<NumberOfColumnsMax;columnIndex++)
						{
							if(matrix[index, columnIndex] >= 0)
							{
								CalendarItem item = Items[(int)matrix[index, columnIndex]];

								// Render current one
								AddDayCellAttributes(writer, cycleDate);
								//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
								writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");

								//  [9/9/2004] -- Begin
								int _colspan=1;

								if(columnIndex != NumberOfColumnsMax-1)
								{
									bool seal = true;
									_repeat[dayofweek,columnIndex]=0;
									do
									{
										for(int columnIndex2=columnIndex+1;columnIndex2<NumberOfColumnsMax;columnIndex2++)
										{
											if(matrix[index+_repeat[dayofweek,columnIndex], columnIndex2]!=-1)
											{
												seal=false;
												break;
											}
										}
										if(!seal)
											break;
										_repeat[dayofweek,columnIndex]++;
									} 
									while(index+_repeat[dayofweek,columnIndex]<matrix.Rows && matrix[index+_repeat[dayofweek,columnIndex], columnIndex] == -2);
					
									if(seal)
									{
										_colspan=NumberOfColumnsMax-columnIndex;
										_repeat[dayofweek,columnIndex]--;
									}
									else
										_repeat[dayofweek,columnIndex]=0;
								}
								if(_colspan>1)
								{
									writer.AddAttribute(HtmlTextWriterAttribute.Colspan, _colspan.ToString());
									RenderedColumns+=_colspan-1;
								}
								//  [9/9/2004] -- End
								
								int rowSpan = AddDayItemAttributes(writer, cycleDate, item);
								width = (int)(100/Dates.Count / (NumberOfColumnsMax))*_colspan; //  [9/9/2004]
								
								writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
							
								if(!isRenderedTopBorder)
									RenderCellTopBorderAttributes(writer);

								writer.RenderBeginTag(HtmlTextWriterTag.Td);
								item.SetRenderedDate(cycleDate);
								item.Render(writer, RenderPath);
								writer.RenderEndTag();
								RenderedColumns++;
							}
							else if(matrix[index, columnIndex] == (int)MatrixConstants.SpanField) // found row spanned event
							{
								// Find out if we need to add colspan column here or not
								// 1. Analize matrix to determine actual column position = matrix column
								//		1a. Maybe that can be done during matrix population by putting event only in the column
								//		it will be
								// 2. Analize matrix to determine if any items will be created in this row
								//		Do it by checking RenderedColumns property
								// 3. Add this column as rendered
								//		because it's spanned column
								// 4. Render space column
								//		render space column with colspan = column number
								//		add one rendered column
					

								// Step 1:
								RenderedSpaceColumns+=columnIndex;
								
								// Step 2:
								RenderedSpaceColumns-=RenderedColumns;

								// Step 3:
								RenderedColumns++;

								//  [9/9/2004] -- Begin
								if(_repeat[dayofweek, columnIndex]>0)
								{
									RenderedColumns += NumberOfColumnsMax-columnIndex;
									_repeat[dayofweek, columnIndex]--;
								}
								//  [9/9/2004] -- End

								// Step 4:
								if(RenderedSpaceColumns>0) // render space column
								{
									AddDayCellAttributes(writer, cycleDate);
									//writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
									//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
									width = (int)(20 / (NumberOfColumnsMax));
									writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
									writer.AddAttribute(HtmlTextWriterAttribute.Colspan, RenderedSpaceColumns.ToString());

									if(!isRenderedTopBorder)
										RenderCellTopBorderAttributes(writer);

									Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
									Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
									writer.RenderBeginTag(HtmlTextWriterTag.Td);
									writer.RenderEndTag();
									RenderedColumns+=RenderedSpaceColumns;
									RenderedSpaceColumns = 0;
								}
							}
						}

						//RenderedColumns+=RenderedSpaceColumns;
						// Fill rest of the space
						if(NumberOfColumnsMax-(RenderedColumns) > 0)
						{
							AddDayCellAttributes(writer, cycleDate);
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
							int colspan = NumberOfColumnsMax - RenderedColumns;
							writer.AddAttribute(HtmlTextWriterAttribute.Width, ((int)(100 / Dates.Count/NumberOfColumnsMax)*colspan).ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Colspan, colspan.ToString());

							if(!isRenderedTopBorder)
								RenderCellTopBorderAttributes(writer);
						
							Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
							Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}
						else if(RenderedColumns==0)
						{
							AddDayCellAttributes(writer, cycleDate);
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Width, ((int)(100 / Dates.Count)).ToString() + "%");

							if(!isRenderedTopBorder)
								RenderCellTopBorderAttributes(writer);

							Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
							Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}
					}
					cycleDate = Helper.IncrementDate(MatrixSpan.DaySpan, cycleDate);
				}
				isRenderedTopBorder = true;
				writer.RenderEndTag(); //tr

				cycleDate = cycleDate.AddDays(-7); // return back to day 1
				cycleDate = Helper.IncrementDate(MatrixSpan.HourSpan, cycleDate);
			}
		}

		/// <summary>
		/// Renders Multi day bar for a week view.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="cycleDate"></param>
		/// <param name="matrixList"></param>
		private void RenderWorkWeekMultiDayBar(HtmlTextWriter writer, ref DateTime cycleDate, ArrayList matrixList)
		{
			int DaysToRender = 7;
			int dayIndex = 1; // number of days to add in cycle, based on colspan

			DateTime newDate = new DateTime(cycleDate.Ticks);
			newDate = newDate.AddDays(DaysToRender);

			// Created matrix / for full week
			Matrix matrix = (Matrix)CreateMatrix(cycleDate.Date, cycleDate.Date.AddDays(DaysToRender), MatrixSpan.WeekDaySpan);

			int EventPerDayMax = matrix.Rows;

			// Reorganize matrix week according to workweek parameters 
			// It will go through the matrix and adjust starting points for each item if the starting point date is not 
			// in WorkWeek array. Elements that are out of scope will be removed.
			for(int rowIndex=0;rowIndex<EventPerDayMax;rowIndex++)
			{
				newDate = newDate.AddDays(-DaysToRender);
				for(int columnIndex=0;columnIndex<DaysToRender;columnIndex++)
				{
					if(matrix[rowIndex, columnIndex]>=0)
					{
						if(!Dates.Contains(newDate)) // encounted start of the item, need to move it to the next day/column
						{
							if(columnIndex + 1 == DaysToRender) // out of limits already, just reset the cell thus removing the item
								matrix[rowIndex, columnIndex] = (int)MatrixConstants.EmptyField;
							else // move id to the next column and reset the current column
							{
								matrix[rowIndex, columnIndex+1] = matrix[rowIndex, columnIndex];
								matrix[rowIndex, columnIndex] = (int)MatrixConstants.EmptyField;
							}
						}
					}
					newDate = newDate.AddDays(1);
				}
			}

			// Add empty row
			/*
			if(EventPerDayMax>0)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);

				int TopMaxColumns = 1;
				for(int weekIndex = 0; weekIndex < 5; weekIndex++)
				{
					TopMaxColumns+=((Matrix)matrixList[weekIndex]).Columns == 0 ? 1 : ((Matrix)matrixList[weekIndex]).Columns;
				}
				writer.AddAttribute(HtmlTextWriterAttribute.Colspan, TopMaxColumns.ToString());
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.RenderEndTag();
				writer.RenderEndTag();
			}
			*/

			// render matrix / day bars
			
			for(int rowIndex=0;rowIndex<EventPerDayMax;rowIndex++)
			{
				// return cycle back one week
				newDate = newDate.AddDays(-DaysToRender);
			
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.RenderEndTag();

				for(int columnIndex=0, matrixIndex = 0;columnIndex<DaysToRender;matrixIndex++)
				{
					if(Dates.Contains(newDate.Date))
					{
						int span = ((Matrix)matrixList[columnIndex]).Columns == 0 ? 1 : ((Matrix)matrixList[columnIndex]).Columns;
						if(matrix[rowIndex, columnIndex]>=0)
						{
							CalendarItem item = Items[(int)matrix[rowIndex, columnIndex]];
							dayIndex = AddWorkWeekMultiDayItemAttributes(writer, newDate, item);
						
							// Need to calculate colspan here
							// It should take in consideration Matrix columns

							int colSpan = 0;
							//int indexDiff = 0; // compinsates for skipped days, because end dayofweek can be bigger
							/*
							bool started = false;
							for(int index = (int)newDate.DayOfWeek; (!started || index!=(int)newDate.DayOfWeek) && index < (int)newDate.DayOfWeek + dayIndex;)
							{
								if(matrixList[index]!=null)
									colSpan += ((Matrix)matrixList[index]).Columns <= 0 ? 1 : ((Matrix)matrixList[index]).Columns;
								//else if(index >= (int)newDate.DayOfWeek)// compinsate
								//	dayIndex++;
								started = true;
								index = index>=6 ? 0 : index+1;
							}
							*/

							int index = (int)(newDate.DayOfWeek - Helper.GetDayOfWeek(this.FirstDayOfWeek));
							
							if(index<0)
								index+=matrixList.Count;

							int loopIndex = dayIndex;
							while(loopIndex>0)
							{
								if(matrixList[index]!=null)
								{
									colSpan += ((Matrix)matrixList[index]).Columns <= 0 ? 1 : ((Matrix)matrixList[index]).Columns;
									loopIndex--;
								}
								else
									dayIndex++;

								index++;
							}

							// [13/1/2005] - Begin
							if(colSpan>0) // [13/1/2005] - End
								writer.AddAttribute("colspan", colSpan.ToString());
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							columnIndex += dayIndex;
							item.SetRenderedDate(newDate);
							item.Render(writer, RenderPath);
							//writer.RenderEndTag(); // item span
							writer.RenderEndTag(); // item td
						}
						else
						{
							//AddMonthCellAttributes(writer, newDate);
							CalendarHeaderStyle.AddAttributesToRender(writer);
							RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
							RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);
							//int colSpan = 0;
							//colSpan = ((Matrix)matrixList[(int)newDate.DayOfWeek - 1]).Columns <= 0 ? 1 : ((Matrix)matrixList[(int)newDate.DayOfWeek - 1]).Columns;
							// [13/1/2005] - Begin
							if(span>0) // [13/1/2005] - End
								writer.AddAttribute("colspan", span.ToString());
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag(); // item td
							
							//dayIndex = span;
							columnIndex++;
						}
					}
					else
					{
						columnIndex++;
					}
					newDate = newDate.AddDays(dayIndex);
					//Reset dayIndex
					dayIndex = 1;
				}
				writer.RenderEndTag(); // item tr
			
			}
		}

		private int AddWorkWeekMultiDayItemAttributes(HtmlTextWriter writer, DateTime cycleDate, CalendarItem item)
		{
			int colSpan = 0;
			for(DateTime date = cycleDate.Date; date<=Dates[Dates.Count-1]; date=date.AddDays(1))
			{
				if(Dates.Contains(date))
				{
					// Jun 16 2004: added method AddTicks(-1)
					if(item.StartDate.Date <= date && item.AdjustedEndDate.Date >= date)
						colSpan++;
				}
			}

			if(colSpan==0)
				colSpan++;
			/*
			//TimeSpan spanTime = item.EndDate - item.StartDate;
			int colSpan = (Helper.GetDaySpan(item.StartDate, item.EndDate) + 1);

			// detect if events spans from previous week
			if(item.StartDate.Date<cycleDate.Date)
			{
				// Calculate span from current date
				//TimeSpan spanTime2 = item.EndDate - cycleDate.Date;
				colSpan = (Helper.GetDaySpan(cycleDate.Date, item.EndDate) + 1);
			}

			// ERROR: change this!!!
			// detect if event spans to next week
			if(colSpan>(5 - (int)cycleDate.DayOfWeek))
			{
				colSpan = (5 - (int)cycleDate.DayOfWeek) + 1;
			}
			*/

			//writer.AddAttribute("colspan", colSpan.ToString());
			//AddMonthCellAttributes(writer, cycleDate);

			CalendarHeaderStyle.AddAttributesToRender(writer);
			RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
			RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);

			//RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
			//RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);

			writer.AddAttribute(HtmlTextWriterAttribute.Height, "18");
			return colSpan;
		}

		#endregion

		#region Calendar Day Views
		/// <summary>
		/// Renders day view, which displays a day, where events
		/// rendered inside each hour cell.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		private void RenderDayViewBar(HtmlTextWriter writer)
		{
			int width = 0;
			DateTime currentDate = SelectedDate;

			// Create inverse matrix
			Matrix matrix = (Matrix)CreateMatrix(currentDate.Date, currentDate.Date.AddDays(1).AddTicks(-1), MatrixSpan.HourSpan);
			matrix = (Matrix)matrix.Transpose();

			// Calculate maximum number of columns
			// 1. Cycle through all items and determine
			// max columns
			int NumberOfColumnsMax = matrix.Columns;

			//  [9/15/2004]	---	Begin
			if(NumberOfColumnsMax>0)
			{
				bool fl = true;
				for(int i=0; i< matrix.Rows; i++)
					if(matrix[i,NumberOfColumnsMax-1]!=-1)
					{
						fl=false;
						break;
					}
				if(fl)
					NumberOfColumnsMax--;
			}
			//  [9/15/2004]	---	End

			foreach (CalendarItem item in Items)
			{
				if(item.IsAllDayEvent(currentDate))
				{
					// Render all day items
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);

					CalendarHeaderStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
					//RenderCellBorderAttributes(writer);
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					writer.RenderEndTag();

					CalendarItemDefaultStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);

					writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
					writer.AddAttribute(HtmlTextWriterAttribute.Colspan, NumberOfColumnsMax == 0 ? "2" : (NumberOfColumnsMax + 1).ToString());
					writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
					writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
					
					// only render top border once
					//if(!renderedAllDay)
					//	RenderCellBorderAttributes(writer);

					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					item.SetRenderedDate(currentDate);
					item.Render(writer, RenderPath);
					writer.RenderEndTag();

					// close tr tag
					writer.RenderEndTag();
				}
			}
			
			bool renderCurrentHour = false;

			// Calculate number of maximum rows
			/* Not needed for now
			int NumberOfRowsMax = 0;
			for(int index=0;index<24*2;index++) 
			{
				// don't render items that happen later
				if((int)index/2 > this.DayEndHour)
					renderCurrentHour = false;

				// determines if we render this hour or not
				// we render if either items present or it's within active borders
				for(int columnIndex=0;columnIndex<NumberOfColumnsMax;columnIndex++)
				{
					if(matrix[index, columnIndex] != (int)MatrixConstants.EmptyField)
						renderCurrentHour = true;

					for(int index2=index;index2<24*2;index2++)
						if(matrix[index2, columnIndex] != (int)MatrixConstants.EmptyField  && (int)index/2>this.DayEndHour)
							renderCurrentHour = true;
				}

				// always render active hours
				if(index>=this.DayStartHour && (int)index/2 <= this.DayEndHour)
					renderCurrentHour = true;

				if(renderCurrentHour)
					NumberOfRowsMax++;
			}
			*/
			
			// Render date
			DateTime cycleDate = currentDate.Date;
			renderCurrentHour = false; 
			bool isRenderedTopBorder = false;

			//  [9/15/2004] --- Begin
			int[] _repeat = new int[NumberOfColumnsMax+1];
			for(int i=0; i<NumberOfColumnsMax+1; i++)
				_repeat[i]=0;
			//  [9/15/2004] --- End

			for(int index=0;index<24*2;index++)
			{
				// don't render items that happen later
				if((int)index/2 > this.DayEndHour)
					renderCurrentHour = false;

				// determines if we render this hour or not
				// we render if either items present or it's within active borders
				for(int columnIndex=0;columnIndex<NumberOfColumnsMax;columnIndex++)
				{
					if(matrix[index, columnIndex] != (int)MatrixConstants.EmptyField)
					{
						renderCurrentHour = true;
						break;
					}

					for(int index2=index;index2<24*2;index2++)
						if(matrix[index2, columnIndex] != (int)MatrixConstants.EmptyField && (int)index/2>this.DayEndHour)
						{
							renderCurrentHour = true;
							break;
						}
				}

				// always render active hours
				if((int)index/2 >= this.DayStartHour && (int)index/2 <= this.DayEndHour)
					renderCurrentHour = true;

				if(renderCurrentHour)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);

					// Render hour header
						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

						writer.AddAttribute(HtmlTextWriterAttribute.Width, "1%");
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
						writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
						writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
						writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "1");
					
						if(index%2!=0)
							RenderCellBorderAttributes(writer);

						if(!isRenderedTopBorder)
							RenderCellTopBorderAttributes(writer);

						writer.RenderBeginTag(HtmlTextWriterTag.Td);
					
						// Output hour every other cell
						if(index%2==0)
						{
							// Render linked hour if format is specified
							if(NewLinkFormat.Length > 0)
							{
								string href = "javascript:" + String.Format(NewLinkFormat, cycleDate); 
								writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
								CalendarHeaderStyle.AddAttributesToRender(writer);
								RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
								RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

								writer.RenderBeginTag(HtmlTextWriterTag.A);
								writer.Write("<nobr>" + cycleDate.ToString(DayHourFormat) + "</nobr>");
								writer.RenderEndTag(); // </A>
							}
							else
								RenderText(writer, String.Format("<nobr>{0}</nobr>", cycleDate.ToString(DayHourFormat)));
						}

						writer.RenderEndTag();

					// Rendering elemens
					int RenderedColumns = 0;
					int RenderedSpaceColumns = 0;
					for(int columnIndex=0;columnIndex<NumberOfColumnsMax;columnIndex++)
					{
						if(matrix[index, columnIndex] >= 0)
						{
							CalendarItem item = Items[(int)matrix[index, columnIndex]];

							// Render current one
							AddDayCellAttributes(writer, cycleDate);
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
							
							//  [9/15/2004] --- Begin
							int _colspan=1;

							if(columnIndex != NumberOfColumnsMax-1)
							{
								bool seal = true;
								_repeat[columnIndex]=0;
								do
								{
									for(int columnIndex2=columnIndex+1;columnIndex2<NumberOfColumnsMax;columnIndex2++)
									{
										if(matrix[index+_repeat[columnIndex], columnIndex2]!=-1)
										{
											seal=false;
											break;
										}
									}
									if(!seal)
										break;
									_repeat[columnIndex]++;
								} 
								while(index+_repeat[columnIndex]<matrix.Rows && matrix[index+_repeat[columnIndex], columnIndex] == -2);
					
								if(seal)
								{
									_colspan=NumberOfColumnsMax-columnIndex;
									_repeat[columnIndex]--;
								}
								else
									_repeat[columnIndex]=0;
							}
							if(_colspan>1)
							{
								writer.AddAttribute(HtmlTextWriterAttribute.Colspan, _colspan.ToString());
								RenderedColumns+=_colspan-1;
							}
							//  [9/15/2004] --- End

							int rowSpan = AddDayItemAttributes(writer, cycleDate, item);
							width = (int)(100/ (NumberOfColumnsMax))*_colspan; //  [9/15/2004]
							writer.AddAttribute(HtmlTextWriterAttribute.Height, (25*rowSpan).ToString());
							writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");

							// Render top border
							if(!isRenderedTopBorder)
								RenderCellTopBorderAttributes(writer);

							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							item.SetRenderedDate(cycleDate);
							item.Render(writer, RenderPath);
							writer.RenderEndTag();
							RenderedColumns++;
						}
						else if(matrix[index, columnIndex] == (int)MatrixConstants.SpanField) // found row spanned event
						{
							// Find out if we need to add colspan column here or not
							// 1. Analize matrix to determine actual column position = matrix column
							//		1a. Maybe that can be done during matrix population by putting event only in the column
							//		it will be
							// 2. Analize matrix to determine if any items will be created in this row
							//		Do it by checking RenderedColumns property
							// 3. Add this column as rendered
							//		because it's spanned column
							// 4. Render space column
							//		render space column with colspan = column number
							//		add one rendered column
						

							// Step 1:
							RenderedSpaceColumns+=columnIndex;

							// Step 2:
							RenderedSpaceColumns-=RenderedColumns;

							// Step 3:
							RenderedColumns++;

							//  [9/15/2004] --- Begin
							if(_repeat[columnIndex]>0)
							{
								RenderedColumns += NumberOfColumnsMax-columnIndex;
								_repeat[columnIndex]--;
							}
							//  [9/15/2004] --- End

							// Step 4:
							if(RenderedSpaceColumns>0) // render space column
							{
								AddDayCellAttributes(writer, cycleDate);
								writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
								//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
								width = (int)(100 / (NumberOfColumnsMax));
								writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
								writer.AddAttribute(HtmlTextWriterAttribute.Colspan, RenderedSpaceColumns.ToString());

								if(!isRenderedTopBorder)
									RenderCellTopBorderAttributes(writer);

								Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
								Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
								writer.RenderBeginTag(HtmlTextWriterTag.Td);
								writer.RenderEndTag();
								RenderedColumns+=RenderedSpaceColumns;
								RenderedSpaceColumns = 0;
							}
						}
					}

					//RenderedColumns+=RenderedSpaceColumns;
					// Fill rest of the space
					if(NumberOfColumnsMax-(RenderedColumns) > 0)
					{
						AddDayCellAttributes(writer, cycleDate);
						//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
						writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
						writer.AddAttribute(HtmlTextWriterAttribute.Colspan, (NumberOfColumnsMax-(RenderedColumns)).ToString());

						if(!isRenderedTopBorder)
							RenderCellTopBorderAttributes(writer);

						Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
						Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}
					else if(RenderedColumns==0)
					{
						AddDayCellAttributes(writer, cycleDate);
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
						//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
						writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");

						if(!isRenderedTopBorder)
							RenderCellTopBorderAttributes(writer);

						Helper.AddLinkAttributes(writer, NewLinkFormat, cycleDate);
						Helper.AddOnContextMenuAttribute(writer, ContextMenuFormat, cycleDate);
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}

					writer.RenderEndTag(); //tr
					isRenderedTopBorder = true;
				}

				cycleDate = Helper.IncrementDate(MatrixSpan.HourSpan, cycleDate);				
			}
		}

		/// <summary>
		/// Renders day view, which displays a day, where events
		/// rendered inside each hour cell.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		private void RenderDayMultiUser(HtmlTextWriter writer)
		{
			int width = 0;
			DateTime currentDate = SelectedDate;

			// List of matrixes
			ArrayList matrixList = new ArrayList();

			// Top border attribute
			bool isRenderedTopBorder = false;

			// Create inverse matrix
			Matrix matrix;
			
			// Create array of matrixes, one for each owner
			foreach(Owner owner in Owners)
			{
				this.CurrentOwner = owner;
				matrix = (Matrix)CreateMatrix(currentDate.Date, currentDate.Date.AddDays(1).AddTicks(-1), MatrixSpan.HourSpan);
				matrix = (Matrix)matrix.Transpose();
				matrixList.Add(matrix.Clone());
			}

			// Calculate maximum number of columns
			// 1. Cycle through all items and determine
			// max columns
			// int NumberOfColumnsMax = matrix.Columns;

			// Render owner header
			if(this.MultiOwner)
			{
				// Extra td for first element
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.RenderEndTag();

				foreach(Owner owner in Owners)
				{
					int columnsMax = ((Matrix)matrixList[Owners.IndexOf(owner)]).Columns;
					CalendarHeaderStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "17");
					writer.AddAttribute(HtmlTextWriterAttribute.Colspan, columnsMax == 0 ? "1" : (columnsMax).ToString());
					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					if(MultiOwnerFormat.Length>0)
					{
						Helper.AddOwnerAttributes(writer, MultiOwnerFormat, currentDate.Date, owner.Value.ToString());
						writer.RenderBeginTag(HtmlTextWriterTag.A);
						this.RenderText(writer, "<nobr>"+owner.Text.ToString()+"</nobr>");
						writer.RenderEndTag();
					}
					else
						this.RenderText(writer, "<nobr>"+owner.Text.ToString()+"</nobr>");
					writer.RenderEndTag();
				}
				writer.RenderEndTag();
			}
			// Render all day items



			// Calculate max number of items
			int alldayElementsMax = 0;
			int alldayElementsCurrent = 0;
			foreach(Owner owner in Owners)
			{
				alldayElementsCurrent = 0;
				foreach (CalendarItem item in Items)
				{
					if(item.IsAllDayEvent(currentDate))
					{
						if(item.Owner == owner)
						{
							alldayElementsCurrent++;
						}
					}
				}

				if(alldayElementsCurrent>alldayElementsMax)
					alldayElementsMax = alldayElementsCurrent;
			}

			IMatrix itemsMatrix = new Matrix(alldayElementsMax, Owners.Count); 

			// Initialize matrix with -1 values
			for (int i = 0; i < alldayElementsMax; i++)
				for (int j = 0; j < Owners.Count; j++)
					itemsMatrix[i,j] = (int)MatrixConstants.EmptyField;

			int alldaycolumnIndex = 0;
			foreach(Owner owner in Owners)
			{
				int rowIndex = 0;
				foreach (CalendarItem item in Items)
				{
					if(item.IsAllDayEvent(currentDate))
					{
						if(item.Owner == owner)
						{
							itemsMatrix[rowIndex, alldaycolumnIndex] = Items.IndexOf(item);
							rowIndex++;
						}
					}
				}
				alldaycolumnIndex++;
			}

			for(int row=0;row<itemsMatrix.Rows;row++)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				for(int index = 0;index<Owners.Count;index++)
				{
					int columnsMax = ((Matrix)matrixList[index]).Columns;
					if(itemsMatrix[row, index] != (int)MatrixConstants.EmptyField)
					{
						CalendarItem item = Items[(int)itemsMatrix[row, index]];

						if(index==0) // only render top element
						{
							CalendarHeaderStyle.AddAttributesToRender(writer);
							RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
							RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}

						// Render top border
						if(!isRenderedTopBorder)
						{
							RenderCellTopBorderAttributes(writer);
						}

						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);

						writer.AddAttribute(HtmlTextWriterAttribute.Width, (100 / Owners.Count).ToString() + "%");
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
						writer.AddAttribute(HtmlTextWriterAttribute.Colspan, columnsMax == 0 ? "1" : (columnsMax).ToString());
						writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
						writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
					
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						if(item.Owner == Owners[index])
						{
							item.SetRenderedDate(currentDate);
							item.Render(writer, RenderPath);
						}
						writer.RenderEndTag();
					}
					else
					{
						if(index==0) // only render top element
						{
							CalendarHeaderStyle.AddAttributesToRender(writer);
							RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
							RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}

						// Render top border
						if(!isRenderedTopBorder)
						{
							RenderCellTopBorderAttributes(writer);
						}

						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);

						writer.AddAttribute(HtmlTextWriterAttribute.Width, (100 / Owners.Count).ToString() + "%");
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
						writer.AddAttribute(HtmlTextWriterAttribute.Colspan, columnsMax == 0 ? "1" : (columnsMax).ToString());
						writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
						writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
					
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}
				}
				isRenderedTopBorder = true;
				
				// close tr tag
				writer.RenderEndTag();
			}

			/*
			foreach (CalendarItem item in Items)
			{
				if(item.IsAllDayEvent(currentDate))
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					foreach(Owner owner in Owners)
					{
						int columnsMax = ((Matrix)matrixList[Owners.IndexOf(owner)]).Columns;

						if(Owners.IndexOf(owner)==0) // only render top element
						{
							CalendarHeaderStyle.AddAttributesToRender(writer);
							RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
							RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
							//RenderCellBorderAttributes(writer);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}

						// Render top border
						if(!isRenderedTopBorder)
						{
							RenderCellTopBorderAttributes(writer);
						}

						CalendarHeaderStyle.AddAttributesToRender(writer);
						RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.SubHeaderBackColor);
						RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.SubHeaderForeColor);

						writer.AddAttribute(HtmlTextWriterAttribute.Width, (100 / Owners.Count).ToString() + "%");
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
						writer.AddAttribute(HtmlTextWriterAttribute.Colspan, columnsMax == 0 ? "1" : (columnsMax).ToString());
						writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
						writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
					
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						if(item.Owner == owner)
						{
							item.SetRenderedDate(currentDate);
							item.Render(writer, RenderPath);
						}
						writer.RenderEndTag();
					}

					isRenderedTopBorder = true;
					// close tr tag
					writer.RenderEndTag();
				}
			}
			*/
			
			bool renderCurrentHour = false;

			// Render date
			DateTime cycleDate = currentDate.Date;
			renderCurrentHour = false; 
			isRenderedTopBorder = false;
			for(int index=0;index<24*2;index++)
			{
				// don't render items that happen later
				if((int)index/2 > this.DayEndHour)
					renderCurrentHour = false;

				// always render active hours
				if((int)index/2 >= this.DayStartHour && (int)index/2 <= this.DayEndHour)
					renderCurrentHour = true;

				// determines if we render this hour or not
				// we render if either items present or it's within active borders
				// NEEDS to go through all matrixes
				if(!renderCurrentHour) // skip expensive algorithm if already rendering current hour
				for(int matrixIndex = 0; matrixIndex<matrixList.Count;++matrixIndex)//  [11/10/2004] - matrixIndex<matrixList.Count-1 --> matrixIndex<matrixList.Count
				{
					matrix = (Matrix)matrixList[matrixIndex];
					for(int columnIndex=0;columnIndex<matrix.Columns;columnIndex++)
					{
						if(matrix[index, columnIndex] != (int)MatrixConstants.EmptyField)
						{
							renderCurrentHour = true;
							break;
						}

						for(int index2=index;index2<24*2;index2++)
							if(matrix[index2, columnIndex] != (int)MatrixConstants.EmptyField && (int)index/2>this.DayEndHour)
							{
								renderCurrentHour = true;
								break;
							}
					}
				}


				if(renderCurrentHour)
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);

					foreach(Owner owner in Owners)
					{
						// Set current owner
						this.CurrentOwner = owner;

						// Set current matrix
						matrix = (Matrix)matrixList[Owners.IndexOf(this.CurrentOwner)];

						// Render hour header
						if(this.MultiOwner && Owners.IndexOf(CurrentOwner)==0) // only render top element
						{
							CalendarHeaderStyle.AddAttributesToRender(writer);
							RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
							RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

							writer.AddAttribute(HtmlTextWriterAttribute.Width, "1%");
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
							writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
							writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
							writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "1");
					
							if(index%2!=0)
								RenderCellBorderAttributes(writer);

							if(!isRenderedTopBorder)
								RenderCellTopBorderAttributes(writer);

							writer.RenderBeginTag(HtmlTextWriterTag.Td);
					
							// Output hour every other cell
							if(index%2==0)
							{
								// Render linked hour if format is specified
								if(NewLinkFormat.Length > 0)
								{
									string href = "javascript:" + String.Format(NewLinkFormat, cycleDate); 

									writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
									CalendarHeaderStyle.AddAttributesToRender(writer);
									RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
									RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);

									writer.RenderBeginTag(HtmlTextWriterTag.A);
									writer.Write("<nobr>" + cycleDate.ToString(DayHourFormat) + "</nobr>");
									writer.RenderEndTag(); // </A>
								}
								else
									RenderText(writer, String.Format("<nobr>{0}</nobr>", cycleDate.ToString(DayHourFormat)));
							}

							writer.RenderEndTag();
						}

						// Rendering elemens
						int RenderedColumns = 0;
						int RenderedSpaceColumns = 0;
						for(int columnIndex=0;columnIndex<matrix.Columns;columnIndex++)
						{
							if(matrix[index, columnIndex] >= 0)
							{
								CalendarItem item = Items[(int)matrix[index, columnIndex]];

								// Render current one
								AddDayCellAttributes(writer, cycleDate);
								//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
							
								int rowSpan = AddDayItemAttributes(writer, cycleDate, item);
								width = (int)(100 / (matrix.Columns) / Owners.Count);
								writer.AddAttribute(HtmlTextWriterAttribute.Height, (25*rowSpan).ToString());
								writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
								writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");

								// Render top border
								if(!isRenderedTopBorder)
									RenderCellTopBorderAttributes(writer);

								writer.RenderBeginTag(HtmlTextWriterTag.Td);
								item.SetRenderedDate(cycleDate);
								item.Render(writer, RenderPath);
								writer.RenderEndTag();
								RenderedColumns++;
							}
							else if(matrix[index, columnIndex] == (int)MatrixConstants.SpanField) // found row spanned event
							{
								// Find out if we need to add colspan column here or not
								// 1. Analize matrix to determine actual column position = matrix column
								//		1a. Maybe that can be done during matrix population by putting event only in the column
								//		it will be
								// 2. Analize matrix to determine if any items will be created in this row
								//		Do it by checking RenderedColumns property
								// 3. Add this column as rendered
								//		because it's spanned column
								// 4. Render space column
								//		render space column with colspan = column number
								//		add one rendered column
						

								// Step 1:
								RenderedSpaceColumns+=columnIndex;

								// Step 2:
								RenderedSpaceColumns-=RenderedColumns;

								// Step 3:
								RenderedColumns++;

								// Step 4:
								if(RenderedSpaceColumns>0) // render space column
								{
									AddDayCellAttributes(writer, cycleDate);
									writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
									//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
									width = (int)(100 / (matrix.Columns) / Owners.Count);
									writer.AddAttribute(HtmlTextWriterAttribute.Width, width.ToString() + "%");
									writer.AddAttribute(HtmlTextWriterAttribute.Colspan, RenderedSpaceColumns.ToString());

									if(!isRenderedTopBorder)
										RenderCellTopBorderAttributes(writer);

									Helper.AddMultiOwnerLinkAttributes(writer, this.NewMultiOwnerLinkFormat, NewLinkFormat, cycleDate, this.CurrentOwner.Value.ToString());//  [9/15/2004] .Text - > .Value.ToString()	
									Helper.AddOnContextMenuOwnerAttribute(writer, this.ContextMenuFormat, cycleDate, this.CurrentOwner.Value.ToString());
									writer.RenderBeginTag(HtmlTextWriterTag.Td);
									writer.RenderEndTag();
									RenderedColumns+=RenderedSpaceColumns;
									RenderedSpaceColumns = 0;
								}
							}
						}

						//RenderedColumns+=RenderedSpaceColumns;
						// Fill rest of the space
						if(matrix.Columns-(RenderedColumns) > 0)
						{
							AddDayCellAttributes(writer, cycleDate);
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
							writer.AddAttribute(HtmlTextWriterAttribute.Width, (100 / Owners.Count).ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Colspan, (matrix.Columns-(RenderedColumns)).ToString());

							if(!isRenderedTopBorder)
								RenderCellTopBorderAttributes(writer);

							Helper.AddMultiOwnerLinkAttributes(writer, this.NewMultiOwnerLinkFormat, NewLinkFormat, cycleDate, this.CurrentOwner.Value.ToString());//  [9/15/2004] .Text - > .Value.ToString()
							Helper.AddOnContextMenuOwnerAttribute(writer, this.ContextMenuFormat, cycleDate, this.CurrentOwner.Value.ToString());
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}
						else if(RenderedColumns==0)
						{
							AddDayCellAttributes(writer, cycleDate);
							writer.AddAttribute(HtmlTextWriterAttribute.Height, "25");
							//writer.AddAttribute(HtmlTextWriterAttribute.Height, ((int)100 / NumberOfRowsMax).ToString() + "%");
							writer.AddAttribute(HtmlTextWriterAttribute.Width, (100 / Owners.Count).ToString() + "%");

							if(!isRenderedTopBorder)
								RenderCellTopBorderAttributes(writer);

							Helper.AddMultiOwnerLinkAttributes(writer, this.NewMultiOwnerLinkFormat, NewLinkFormat, cycleDate, this.CurrentOwner.Value.ToString());//  [9/15/2004] .Text - > .Value.ToString()
							Helper.AddOnContextMenuOwnerAttribute(writer, this.ContextMenuFormat, cycleDate, this.CurrentOwner.Value.ToString());
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag();
						}

					} // end of owners cycle
					writer.RenderEndTag(); //tr
					isRenderedTopBorder = true;
				}
				
				cycleDate = Helper.IncrementDate(MatrixSpan.HourSpan, cycleDate);				
			}
		}

		private int AddDayItemAttributes(HtmlTextWriter writer, DateTime cycleDate, CalendarItem item)
		{
			int colSpan = Helper.GetHourSpan(cycleDate, item);
			// 1/13/2005  - Begin
			if (colSpan>0)// 1/13/2005 - End
				writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, colSpan.ToString());
			return colSpan;
		}

		private void AddDayCellAttributes(HtmlTextWriter writer, DateTime cycleDate)
		{
			if(cycleDate.Hour == HighlightedDate.Hour && cycleDate.Minute == HighlightedDate.Minute && cycleDate.Date == HighlightedDate.Date) // current day, select
			{
				CalendarItemSelectedStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemSelectedStyle.BackColor, CalendarColorConstants.ItemSelectedBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemSelectedStyle.ForeColor, CalendarColorConstants.ItemSelectedForeColor);

				AddHoverAttributesToRender(writer, CalendarItemSelectedStyle);
			}
			else if(cycleDate.Hour > this.DayEndHour || cycleDate.Hour < this.DayStartHour || Helper.IsWeekend(cycleDate, WorkWeek))
			{
				CalendarItemInactiveStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemInactiveBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemInactiveForeColor);

				AddHoverAttributesToRender(writer, CalendarItemInactiveStyle);
			}
			else
			{
				CalendarItemDefaultStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);

				AddHoverAttributesToRender(writer, CalendarItemDefaultStyle);
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			RenderCellBorderAttributes(writer, cycleDate.Minute == 0 ? true : false);
		}
		#endregion

		#region Calendar Task View
		/// <summary>
		/// Renders task view, which displays a gantt like 
		/// task view starting with a currently selected day for a month.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		private void RenderTaskView(HtmlTextWriter writer)
		{
			//Initialize constants
			DateTime renderStartDate = TimescaleStartDate; //SelectedDate.Date;
			DateTime renderEndDate = TimescaleEndDate; //Helper.GetTaskEndDate(SelectedDate); // can be customized for zooming later
			int spanMax = (Helper.GetDaySpan(renderStartDate, renderEndDate) + 1);

			// Create weekday row
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			// Render empty top left space
			writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "2");
			RenderCellBorderAttributes(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			if(TaskItemHeaderTemplate != null)
			{
				TemplateItem header = new TemplateItem();
				TaskItemHeaderTemplate.InstantiateIn(header);
				header.RenderControl(writer);
			}
			writer.RenderEndTag();


			for(DateTime cycleDate = TimescaleStartDate; cycleDate <= TimescaleEndDate; cycleDate = Helper.IncrementDate(TimescaleTopFormat, cycleDate, this.FirstDayOfWeek))
			{
				int colSpan = 0;
				bool exitLoop = false;

				// Detect if it spans out of boundaries
				if(Helper.IncrementDate(TimescaleTopFormat, cycleDate, this.FirstDayOfWeek) > this.TimescaleEndDate)
				{
					colSpan = (int)((TimeSpan)(this.TimescaleEndDate - cycleDate)).TotalDays + 1;
					exitLoop = true;
				}
				else
					colSpan = (int)((TimeSpan)(Helper.IncrementDate(TimescaleTopFormat, cycleDate, this.FirstDayOfWeek) - cycleDate)).TotalDays;

				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "17");
				// 13/1/2005 - Begin
				if (colSpan>0) // 13/1/2005 - End
					writer.AddAttribute(HtmlTextWriterAttribute.Colspan, colSpan.ToString());
				RenderCellBorderAttributes(writer);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				if(colSpan >= 7) // only render those items that can be fully displayed
				{
					string href = "";

					CalendarHeaderStyle.AddAttributesToRender(writer);
					RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
					RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
					
					//  [9/15/2004]	---	Begin
					if(this.DrillDownEnabled)
					{
						if(this.TimescaleTopFormat == TimescaleTopSpan.Month)
              href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + cycleDate.ToShortDateString() + "," + CalendarViewType.MonthView.GetHashCode()); 
						else
              href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + cycleDate.ToShortDateString() + "," + CalendarViewType.WeekView.GetHashCode()); 
						writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
						writer.RenderBeginTag(HtmlTextWriterTag.A);
						RenderText(writer, cycleDate.ToString(TimescaleTopLabelFormat));
						writer.RenderEndTag(); // </A>
					}
					else
					{
						href = cycleDate.ToShortDateString();
						writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
						writer.RenderBeginTag(HtmlTextWriterTag.Div);
						RenderText(writer, cycleDate.ToString(TimescaleTopLabelFormat));
						writer.RenderEndTag(); // </Div>
					}
					//  [9/15/2004]	---	End
				}
				writer.RenderEndTag();

				if(exitLoop)
					break;
			}

			writer.RenderEndTag(); //tr

			// Render month day numbers
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			
			for(DateTime cycleDate = renderStartDate;cycleDate <= renderEndDate;cycleDate = Helper.IncrementDate(MatrixSpan.DaySpan, cycleDate))
			{
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "17");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, TaskItemWidth.ToString()); //  [9/15/2004]
				writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
				RenderCellBorderAttributes(writer);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				CalendarHeaderStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarHeaderStyle.BackColor, CalendarColorConstants.HeaderBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarHeaderStyle.ForeColor, CalendarColorConstants.HeaderForeColor);
				
				//  [9/15/2004]	---	Begin
				if(this.DrillDownEnabled)
				{
					String href = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, "OnSelectedViewChange," + cycleDate.ToShortDateString() + "," + CalendarViewType.DayView.GetHashCode()); 

					writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
					writer.RenderBeginTag(HtmlTextWriterTag.A);
					RenderText(writer, cycleDate.ToString(TimescaleBottomLabelFormat));
					writer.RenderEndTag(); // </A>
				}
				else
				{
					String href = cycleDate.ToShortDateString(); 
					writer.AddAttribute(HtmlTextWriterAttribute.Value, href);
					writer.RenderBeginTag(HtmlTextWriterTag.Div);
					RenderText(writer, cycleDate.ToString(TimescaleBottomLabelFormat));
					writer.RenderEndTag(); // </Div>
				}
				//  [9/15/2004]	---	End

				writer.RenderEndTag(); //td
			}
			
			writer.RenderEndTag(); //tr

			// Render tasks and spans here
			for(int index = 0;index<Items.Count;index++)
			{
				CalendarItem item = Items[index];
				if(!this.RemoveItems || (item.StartDate.Date >= renderStartDate && item.StartDate.Date <= renderEndDate) || (item.EndDate.Date >= renderStartDate && item.EndDate.Date <= renderEndDate) || (item.StartDate.Date <= renderStartDate && item.EndDate.Date >= renderEndDate))
				{
					int rowSpannedTotal = 0;
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);

					// Render task name
					RenderCellBorderAttributes(writer);
					if(this.EventBarShow)
						writer.AddAttribute(HtmlTextWriterAttribute.Height, (ItemHeight.Value+10).ToString());
					else
						writer.AddAttribute(HtmlTextWriterAttribute.Height, (ItemHeight.Value+6).ToString());

					writer.RenderBeginTag(HtmlTextWriterTag.Td);
					item.Render(writer, RenderPath);
					writer.RenderEndTag(); //td

					//Hashtable hash;
					//if(this.MergeEvents)
					//	hash = MergeItems();

					// Render box span
					for(DateTime cycleDate = renderStartDate;cycleDate <= renderEndDate;)
					{
						bool isRendered = false;
						DateTime endCycleDate = renderEndDate.AddDays(1);
						if(item.IsWithinSpanEvent(MatrixSpan.DaySpan, cycleDate))
						{
							int colSpan = AddTaskItemAttributes(writer, cycleDate, item);
							RenderCellBorderAttributes(writer);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							item.SetRenderedDate(cycleDate);
							//writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (14 * colSpan).ToString() + "px");
							item.RenderBoxTagAttributes(writer);
							if(TaskItemBoxTemplate!=null) // render using template
							{
								writer.AddStyleAttribute("overflow", "hidden");
								//writer.AddStyleAttribute("border", "solid 1px");
								string strWidth = this.TaskItemWidth.ToString();
								int iWidth = int.Parse(strWidth.Substring(0,strWidth.IndexOf("px")));
								if(colSpan>2)
									writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (iWidth * colSpan + colSpan).ToString() + "px");
								else
									writer.AddStyleAttribute(HtmlTextWriterStyle.Width, (colSpan*(iWidth-2)).ToString() + "px");
								writer.RenderBeginTag(HtmlTextWriterTag.Div);
								item.Controls.Clear();// = (CalendarItem)item.Clone();
								CalendarItem item2 = (CalendarItem)item.Clone();
								TaskItemBoxTemplate.InstantiateIn(item2);
								item2.DataBind();
								item2.RenderControl(writer);
								writer.RenderEndTag(); //div
							}
							item.RenderBoxEndTagAttributes(writer);
							writer.RenderEndTag(); //td

							rowSpannedTotal += colSpan;
							cycleDate = cycleDate.AddDays(colSpan);

							if(this.MergeEvents)
							if(index != Items.Count - 1)
							{
								CalendarItem nextItem = Items[index+1];
								// June 28 2004: Added .Date to the Start and EndDate
								if(nextItem.StartDate.Date>item.EndDate.Date && nextItem.Label.CompareTo(item.Label)==0)
								{
									index++;
									item = nextItem;
									endCycleDate = item.StartDate.Date;
								}
							}
							//isRendered = true;
						} 
						else // render space before event
						{
							//endCycleDate = item.StartDate.Date;
							if((item.StartDate.Date >= renderStartDate && item.StartDate.Date <= renderEndDate) || (item.AdjustedEndDate.Date >= renderStartDate && item.AdjustedEndDate.Date <= renderEndDate) || (item.StartDate.Date <= renderStartDate && item.AdjustedEndDate.Date >= renderEndDate))
								endCycleDate = item.StartDate.Date;
							else
								endCycleDate = renderEndDate.AddDays(1);
						}

						while(cycleDate < endCycleDate)
						{
							int colSpan = 0;
							bool finished = false;
							if(Helper.IsWeekend(cycleDate, this.WorkWeek))//weekend, apply weekdaystyle
							{
								CalendarItemInactiveStyle.AddAttributesToRender(writer);
								RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemInactiveBackColor);
								RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemInactiveForeColor);
								colSpan = 1;
							}
							else // colspan until weekend
							{
								colSpan = 1;
								while((Helper.GetCalDayOfWeek(cycleDate.AddDays(colSpan).DayOfWeek)&WorkWeek)!=0 && 
									(Helper.GetCalDayOfWeek(cycleDate.AddDays(colSpan).DayOfWeek)&FirstDayOfWeek)==0)
								{
									colSpan++;
								}
								//colSpan = 1;//6 - (int)cycleDate.DayOfWeek;
							}

							// Detect if colspan goes out of endCycleDate boundaries
							if(cycleDate.AddDays(colSpan) >= endCycleDate)
							{
								int diff = (int)((TimeSpan)(cycleDate.AddDays(colSpan).Date - endCycleDate.Date)).TotalDays;
								colSpan -= diff;
								finished = true;
							}

							// Detect if colspan goes out of boundaries
							rowSpannedTotal += colSpan;
							if(rowSpannedTotal > spanMax)
								colSpan -= rowSpannedTotal - spanMax;

							cycleDate = cycleDate.AddDays(colSpan);
							// [13/1/2005] - Begin
							if(colSpan>0) // [13/1/2005] - End
								writer.AddAttribute(HtmlTextWriterAttribute.Colspan, colSpan.ToString());
							RenderCellBorderAttributes(writer);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							writer.RenderEndTag(); //td

							if(finished)
								break;
						}

						if(isRendered)
							break;

						// For some reason the expression in for statement doesn't cause termination
						//if(cycleDate.Date < renderEndDate.Date)
						//	break;

					}
					writer.RenderEndTag(); //tr
				}
			}
		}

		private Hashtable MergeItems()
		{
			Hashtable hash = new Hashtable();
			foreach(CalendarItem item in Items)
			{
				if(hash.ContainsKey(item.Label))
				{
					CalendarItem elem = (CalendarItem)hash[item.Label];
					while(elem.NextItem!=null)
						elem = elem.NextItem;
					elem.NextItem = item;
				}
				else
				{
					hash.Add(item.Label, item);
				}
			}
			return hash;
		}

		private int AddTaskItemAttributes(HtmlTextWriter writer, DateTime cycleDate, CalendarItem item)
		{
			//TimeSpan spanTime = item.EndDate - item.StartDate;
			int colSpan = (Helper.GetDaySpan(item.StartDate, item.AdjustedEndDate) + 1);

			// detect if events spans from previous period
			if(item.StartDate.Date<cycleDate.Date)
			{
				// Calculate span from current date
				colSpan = (Helper.GetDaySpan(cycleDate.Date, item.AdjustedEndDate) + 1);
			}

			// detect if event spans to next week
			/*
			int total = (int)((TimeSpan)(TimescaleEndDate - TimescaleStartDate)).TotalDays;
			if(colSpan>total)
				colSpan = total;
			*/
			int total = (int)((TimeSpan)(TimescaleEndDate - TimescaleStartDate)).TotalDays;
			if(colSpan>total - (int)((TimeSpan)(cycleDate - TimescaleStartDate)).TotalDays)
				colSpan = (int)((TimeSpan)(TimescaleEndDate - cycleDate)).TotalDays + 1;

			// [13/1/2005] - Begin
			if(colSpan>0) // [13/1/2005] - End
				writer.AddAttribute(HtmlTextWriterAttribute.Colspan, colSpan.ToString());
			AddTaskCellAttributes(writer, cycleDate);

			RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemInactiveStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
			RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemInactiveStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);

			writer.AddAttribute(HtmlTextWriterAttribute.Height, "18");
			return colSpan;
		}

		private void AddTaskCellAttributes(HtmlTextWriter writer, DateTime cycleDate)
		{
			if(cycleDate.Date == HighlightedDate.Date) // current date
			{
				CalendarItemSelectedStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemSelectedStyle.BackColor, CalendarColorConstants.ItemSelectedBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemSelectedStyle.ForeColor, CalendarColorConstants.ItemSelectedForeColor);
			}
			else
			{
				CalendarItemDefaultStyle.AddAttributesToRender(writer);
				RenderPaletteColor(writer, HtmlTextWriterStyle.BackgroundColor, CalendarItemDefaultStyle.BackColor, CalendarColorConstants.ItemDefaultBackColor);
				RenderPaletteColor(writer, HtmlTextWriterStyle.Color, CalendarItemDefaultStyle.ForeColor, CalendarColorConstants.ItemDefaultForeColor);
			}
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
		}
		#endregion

		#region Calendar Render Shared Functions
		private void CheckLicense()
		{
			/*
			if (IsDemoVersion == true && LicenseManager.CurrentContext.UsageMode != LicenseUsageMode.Designtime) 
			{
				Items.Add(new CalendarItem(0, "<font color=red>This is unlicensed version, please contact us to get a license.</font>", DateTime.Now, "http://center.mediachase.com", "Get license"));
			}
			*/
		}

		/// <summary>
		/// Renders Different border based on odd or even row number
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="odd"></param>
		public void RenderCellBorderAttributes(HtmlTextWriter writer, bool odd)
		{
			String color = "black";
			String width = "1px";
			String style = "solid";

			if(odd)
			{
				color = getPaletteColorString(CalendarColorConstants.ItemAlternativeBorderColor);
				if(!this.AlternativeBorderColor.IsEmpty)
					color = ColorTranslator.ToHtml(AlternativeBorderColor);
			}
			else
			{
				color = getPaletteColorString(CalendarColorConstants.ItemBorderColor);
				if(!this.BorderColor.IsEmpty)
					color = ColorTranslator.ToHtml(ControlStyle.BorderColor);
			}

			if(!this.ControlStyle.BorderWidth.IsEmpty)
				width = ControlStyle.BorderWidth.ToString();

			if(this.ControlStyle.BorderStyle.ToString() != String.Empty)
				style = ControlStyle.BorderStyle.ToString();

			writer.AddStyleAttribute("border-bottom", String.Format("{0} {1} {2}", color, width, style));
		}

		public void RenderCellTopBorderAttributes(HtmlTextWriter writer)
		{
			String color = "black";
			String width = "1px";
			String style = "solid";

			color = getPaletteColorString(CalendarColorConstants.DefaultBackColor);

			if(!this.ControlStyle.BorderColor.IsEmpty)
				color = ColorTranslator.ToHtml(ControlStyle.BorderColor);

			if(!this.ControlStyle.BorderWidth.IsEmpty)
				width = ControlStyle.BorderWidth.ToString();

			if(this.ControlStyle.BorderStyle.ToString() != String.Empty)
				style = ControlStyle.BorderStyle.ToString();

			writer.AddStyleAttribute("border-top", String.Format("{0} {1} {2}", color, width, style));
		}

		public void RenderCellBorderAttributes(HtmlTextWriter writer)
		{
			String color = "black";
			String width = "1px";
			String style = "solid";

			color = getPaletteColorString(CalendarColorConstants.DefaultBackColor);

			if(!this.ControlStyle.BorderColor.IsEmpty)
				color = ColorTranslator.ToHtml(ControlStyle.BorderColor);

			if(!this.ControlStyle.BorderWidth.IsEmpty)
				width = ControlStyle.BorderWidth.ToString();

			if(this.ControlStyle.BorderStyle.ToString() != String.Empty)
				style = ControlStyle.BorderStyle.ToString();

			writer.AddStyleAttribute("border-bottom", String.Format("{0} {1} {2}", color, width, style));
			//writer.AddAttribute(HtmlTextWriterAttribute.Bordercolor, color);
		}

		/// <summary>
		/// The rendering path for visual designers.
		/// </summary>
		/// <param name="writer">The output stream that renders HTML content to the client.</param>
		protected override void RenderDesignerPath(HtmlTextWriter writer)
		{
			base.RenderDesignerPath(writer);
		}

		public void RenderPaletteColor(HtmlTextWriter writer, HtmlTextWriterStyle style, Color defined, CalendarColorConstants palette)
		{
			bool isRendered = true;
			isRendered = !defined.IsEmpty;

			switch(palette)
			{
				case CalendarColorConstants.ItemInactiveBackColor:
				case CalendarColorConstants.ItemInactiveForeColor:
					if(this.CalendarItemInactiveStyle.CssClass.Length > 0)
						isRendered = true;
					break;
				case CalendarColorConstants.HeaderBackColor:
				case CalendarColorConstants.HeaderForeColor:
					if(this.CalendarHeaderStyle.CssClass.Length > 0)
						isRendered = true;
					break;
				case CalendarColorConstants.ItemDefaultBackColor:
				case CalendarColorConstants.ItemDefaultForeColor:
					if(this.CalendarItemDefaultStyle.CssClass.Length > 0)
						isRendered = true;
					break;
				case CalendarColorConstants.ItemSelectedBackColor:
				case CalendarColorConstants.ItemSelectedForeColor:
					if(this.CalendarItemSelectedStyle.CssClass.Length > 0)
						isRendered = true;
					break;
			}
			
			if(!isRendered)
				writer.AddStyleAttribute(style, getPaletteColorString(palette));
		}

		public String getPaletteColorString(CalendarColorConstants colorType)
		{
			return ColorTranslator.ToHtml(CalendarColors.GetPaletteColors(Palette)[(int)colorType]);
		}

		public Color getPaletteColor(CalendarColorConstants colorType)
		{
			return CalendarColors.GetPaletteColors(this.Palette)[(int)colorType];
		}

		public string[] getDayNames()
		{
			int firstDay = System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek.GetHashCode();
			string[] days = null;
			string[] days2 = new string[7];
			if(this.AbbreviatedDayNames)
				days = System.Globalization.DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;
			else
				days = System.Globalization.DateTimeFormatInfo.CurrentInfo.DayNames;

			days.CopyTo(days2, 0);
			for(int index=firstDay, index2 = 0;index<7;index++,index2++)
				days2[index2] = days[index];

			for(int index=0, index2 = 7 - firstDay;index<firstDay;index++,index2++)
				days2[index2] = days[index];

			return days2;
		}

		private void AddHoverAttributesToRender(HtmlTextWriter writer, CssTableItemStyle current)
		{
		}
		#endregion

		#region Calendar Common Functions

		/// <summary>
		/// Generic function for creating matrix populated with events
		/// on appropriate positions.
		/// </summary>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="period"></param>
		/// <returns></returns>
		public IMatrix CreateMatrix(DateTime startDate, DateTime endDate, MatrixSpan period)
		{
			// Cycle date
			DateTime cycleDate = new DateTime(startDate.Ticks);

			// Maximum Rows per Element
			int rowsMax = 0;

			// initialize constants here
			int columns = 0;

			// Array list with index'es of columns
			ArrayList usedColsArray = new ArrayList();

			// remove unused items
			ItemLinkedList items = new ItemLinkedList();
			foreach (CalendarItem item in Items)
			{
				if(item.EndDate >= startDate && endDate >= item.StartDate)
				{
					items.Add(item);
				}
			}

			// Initialized number of columns based on span length
			switch(period)
			{
				case MatrixSpan.WeekHourSpan:
				case MatrixSpan.HourSpan: // calculates number of hours 
					//columns = (int)((TimeSpan)(endDate - startDate)).TotalHours * 2;
					//1.2.0: columns = (int)System.Math.Ceiling(((TimeSpan)(Helper.GetHourDate(endDate) 
					//	- Helper.GetHourStartDate(startDate))).TotalHours * 2); // changed cause hour span resolution is actually 30 minutes
					// 1.3.0
					columns = (int)System.Math.Ceiling(((TimeSpan)(endDate 
						- startDate)).TotalHours * 2); // changed cause hour span resolution is actually 30 minutes
					break;
				case MatrixSpan.WeekDaySpan:
				case MatrixSpan.DaySpan:
					columns = Helper.GetDaySpan(startDate, endDate);
					break;
				case MatrixSpan.MonthSpan:
					columns = endDate.Month - startDate.Month;
					break;
			}

			// Create Array consisting of Linked List's for each cell to improve performance
			object[] llEventsArray = new object[columns];
			// Initialize ll array
			for(int index=0;index<columns;index++)
			{
				llEventsArray[index] = new ItemLinkedList();
				/*
				foreach (CalendarItem item in Items)
				{
					if(item.IsWithinSpanEvent(period, cycleDate))
					{
						RowsCurrent++;
						((ItemLinkedList)llEventsArray[columnIndex]).Add(item);
					}
				}

				// Initialized number of columns based on span length
				cycleDate = Helper.IncrementDate(period, cycleDate);
				*/
			}



			//TODO: add optimization code here
			// Add hash table with linked list for each data cell
			// data cell is a row in day view, month day cell in month view
			// Calculate maximum number of rows for element
			#region Optimized CODE
			/*
			foreach(CalendarItem item in items)
			{
				int startIndex = 0;
				int endIndex = 0;
				switch(period)
				{
					case MatrixSpan.WeekHourSpan:
					case MatrixSpan.HourSpan:
						if(item.StartDate<startDate)
						{
							item.EndDate
						}
						break;
					case MatrixSpan.WeekDaySpan:
					case MatrixSpan.DaySpan:
						break;
					case MatrixSpan.MonthSpan:
						columns = endDate.Month - startDate.Month;
						break;
				}
				((ItemLinkedList)llEventsArray[columnIndex]).Add(item);
			}
			*/
			/*
			foreach (CalendarItem item in Items)
			{
				int columnIndex = 0;
				columnIndex = 1;

				if(this.ViewType == CalendarViewType.DayView && this.MultiOwner && item.Owner != this.CurrentOwner)
					continue;

				if(item.IsWithinSpanEvent(MatrixSpan.MonthSpan, cycleDate.Date))
				{
					if(item.StartDate < cycleDate.Date) // first item
					{
						((ItemLinkedList)llEventsArray[columnIndex]).Add(item);
					}
					RowsCurrent++;
					((ItemLinkedList)llEventsArray[columnIndex]).Add(item);
				}
			}
			*/		
			#endregion

			#region OLD CODE that was optimized
			for(int columnIndex=0;columnIndex<columns;columnIndex++)
			{
				int RowsCurrent = 0;
				int itemsPerIncrement = 0; // determines how many items are rendered/started in current increment

				// Check for workweek
				foreach (CalendarItem item in items)
				{
					if(this.ViewType == CalendarViewType.DayView && this.MultiOwner && item.Owner != this.CurrentOwner)
						continue;

					// Skip max number for day view
					if(this.ViewType != CalendarViewType.DayView && period != MatrixSpan.WeekHourSpan)
					{
						if(item.IsCurrentSpanEvent(columnIndex, period, cycleDate))
							itemsPerIncrement++;

						// dont render more than specified
						if(itemsPerIncrement > this.MaxDisplayedItems && this.MaxDisplayedItems != 0)
						{
							// Increment for dummy node
							RowsCurrent++;
							((ItemLinkedList)llEventsArray[columnIndex]).Add(item);
							break;
						}
					}

					if(item.IsWithinSpanEvent(period, cycleDate))
					{
						RowsCurrent++;
						((ItemLinkedList)llEventsArray[columnIndex]).Add(item);
					}
				}

				if(RowsCurrent > rowsMax)
					rowsMax = RowsCurrent;

				// Initialized number of columns based on span length
				cycleDate = Helper.IncrementDate(period, cycleDate);
			}
			#endregion

			// Check if Maximum number of rows is reached and adjust maximum
			// add 1 extra space for more element index
			//if(this.ViewType!=CalendarViewType.DayView || this.ViewType!=CalendarViewType.TaskView)
			//if(this.MaxDisplayedItems < rowsMax)
			//	rowsMax = MaxDisplayedItems + 1;

			// Item MATRIX, rank = 7
			IMatrix matrix = new Matrix(rowsMax, columns); 

			// Initialize matrix with -1 values
			for (int i = 0; i < rowsMax; i++)
				for (int j = 0; j < columns; j++)
					matrix[i,j] = (int)MatrixConstants.EmptyField;


			// Return cycle back
			switch(period)
			{
				case MatrixSpan.WeekHourSpan:
				case MatrixSpan.HourSpan:
					// return cycle back one day
					cycleDate = cycleDate.AddHours(-24);
					break;
				case MatrixSpan.WeekDaySpan:
				case MatrixSpan.DaySpan:
					// return cycle back one week
					cycleDate = cycleDate.AddDays(-7);
					break;
				case MatrixSpan.MonthSpan:
					// return cycle back one week
					cycleDate = cycleDate.AddMonths(-12);
					break;
			}

			// populate matrix
			for(int columnIndex=0;columnIndex<columns&&rowsMax>0;columnIndex++)
			{
				// Render items within that date
				int rowIndex = 0;

				int itemsPerIncrement = 0; // determines how many items are rendered/started in current increment

				ItemLinkedList llItems = (ItemLinkedList)llEventsArray[columnIndex];

				
				if(llItems.Count > 0)
				foreach(CalendarItem item in llItems)
				{
				/*
				foreach (CalendarItem item in Items)
				{
				*/
					if(this.ViewType == CalendarViewType.DayView && this.MultiOwner && item.Owner != this.CurrentOwner)
						continue;

					// show if current item falls into date span
					bool foundValue = false;

					foundValue = item.IsCurrentSpanEvent(columnIndex, period, cycleDate);

					if(foundValue)
						itemsPerIncrement++;

					// Skip max number for day view
					if(this.ViewType != CalendarViewType.DayView && period != MatrixSpan.WeekHourSpan)
					{
						// dont render more than specified
						if(itemsPerIncrement > this.MaxDisplayedItems && this.MaxDisplayedItems != 0 && rowIndex < rowsMax)
						{
							matrix[rowIndex, columnIndex] = (int)MatrixConstants.MoreElementsField;
							break;
						}
					}

					if(foundValue && rowIndex < rowsMax)
						PopulateMatrix(item, cycleDate, ref matrix, ref rowIndex, columnIndex, period);
					
				}

				// Initialized number of columns based on span length
				cycleDate = Helper.IncrementDate(period, cycleDate);
			}

			return matrix;
		}

		private void PopulateMatrix(CalendarItem item, DateTime cycleDate, ref IMatrix matrix, ref int rowIndex, int columnIndex, MatrixSpan period)
		{
			// Don't overwright spanning items, write into next row(column?) instead
			while(matrix[rowIndex, columnIndex] == (int)MatrixConstants.SpanField)
				rowIndex++;

			// Check if Maximum number of rows is reached
			/*
			if(this.ViewType!=CalendarViewType.DayView || this.ViewType!=CalendarViewType.TaskView)
			if(this.MaxDisplayedItems < rowIndex)
			{
				// reached maximum number of items we can display in current view
				matrix[rowIndex, columnIndex] = (int)MatrixConstants.MoreElementsField; // -3 = more items available
				break;
			}
			*/

			matrix[rowIndex, columnIndex] = Items.IndexOf(item);
						
			// Repopulate item till the end
			int span = 0;

			// Init variables
			DateTime startSpanDate = item.StartDate;
			DateTime endSpanDate = item.AdjustedEndDate;
						
			// detect if events spans from previous element
			if(item.StartDate<cycleDate)
			{
				// Calculate span from current date
				startSpanDate = cycleDate;
			}

			switch(period)
			{
				case MatrixSpan.WeekHourSpan:
				case MatrixSpan.HourSpan:
					//span = (int)System.Math.Ceiling((Helper.GetHourDate(item.EndDate) 
					//	- Helper.GetHourStartDate(item.StartDate)).TotalMinutes/60*2) - 1;
					span = Helper.GetHourSpan(cycleDate, item) - 1; // -1 cause matrix starts with 0
					break;
				case MatrixSpan.WeekDaySpan:
				case MatrixSpan.DaySpan:
					span = Helper.GetDaySpan(startSpanDate, endSpanDate);
					break;
				case MatrixSpan.MonthSpan:
					span = endSpanDate.Month - startSpanDate.Month;
					break;
			}

			for(int index=1;index<=span;index++)
				if(columnIndex+index<matrix.Columns)
					matrix[rowIndex, columnIndex+index] = (int)MatrixConstants.SpanField;

			rowIndex++;
		}
		#endregion

		#region Calendar Databinding

		/// <summary>
		/// Binds the control and all its child controls to the data source specified by the <see cref="DataSource">DataSource</see> property.
		/// </summary>
		/// <remarks>
		/// Use the DataBind method to bind the data source specified by the DataSource property with the data list control. By binding the data source with a data listing control, the information in the data source is displayed in a data listing control.
		/// The DataBind method is also commonly used to synchronize the data source and a data listing control after information in the data source is updated. This allows any changes in the data source to also be updated in a data listing control.
		/// </remarks>
		public override void DataBind() 
		{
			OnDataBinding(EventArgs.Empty);
		}

		/// <summary>
		/// Raises the DataBinding event. This notifies a control to perform any data binding logic that is associated with it.
		/// </summary>
		/// <param name="e">An EventArgs object that contains the event data.</param>
		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);

			// Reset the control's state.
			Controls.Clear();
			ClearChildViewState();

			// Create the control hierarchy using the data source.
			CreateControlHierarchy(true);
			this.ChildControlsCreated = true;

			//TrackViewState();
		}

		/// <summary>
		/// This member overrides <see cref="Control.CreateChildControls"/>.
		/// </summary>
		protected override void CreateChildControls() 
		{
			Controls.Clear();

			// Create from DataSource
			/*if (ViewState["ItemCount"] != null && false)//never create from viewstate because we dont keep it anymore 
			{
				CreateControlHierarchy(false);
			}*/

			ClearChildViewState();
		}

		private void CreateControlHierarchy(bool useDataSource) 
		{
			ClearChildViewState();

			IEnumerable dataSource = null;
			int count = -1;

			if (useDataSource == false) 
			{
				// ViewState must have a non-null value for ItemCount because this is checked 
				//  by CreateChildControls.
				count = -1;// remove cached items count (int)ViewState["ItemCount"];
				if (count != -1) 
					dataSource = new CalendarDataSource(count);
			}
			else 
			{
				dataSource = DataSourceHelper.GetResolvedDataSource (DataSource, DataMember); 
			}

			Controls.Clear();
			//if(Items!=null)
			//	Items.Clear();

			//this.EnsureChildControls();


			// Bind Holidays
			// Step 1. Bind Calendar Holiday Sources
			if(this.HolidayDataSource!=null)
			{
				IEnumerable	dsCollection = DataSourceHelper.GetResolvedDataSource(this.HolidayDataSource,this.HolidayDataMember);
				if(dsCollection!=null)
				{
					this.Holidays.Clear();

					if(this.HolidayDataDateField==String.Empty)
						throw new ArgumentNullException("HolidayExceptionDateField");

					foreach(object dataItem in dsCollection)
					{
						DateTime holTime = DateTime.Parse(DataBinder.Eval(dataItem,this.HolidayDataDateField,null));
						string name = "";
						if (HolidayDataTextField != String.Empty)
							name = DataBinder.GetPropertyValue(dataItem, HolidayDataTextField, null);

						this.Holidays.Add(new Holiday(holTime, name));
					}
				}
			}

			// Bind Owners
			// Step 2. Bind Calendar Owner Sources
			if(this.OwnerDataSource!=null)
			{
				IEnumerable	dsCollection = DataSourceHelper.GetResolvedDataSource(this.OwnerDataSource,this.OwnerDataMember);
				if(dsCollection!=null)
				{
					this.Owners.Clear();

					if(this.OwnerDataTextField==String.Empty)
						throw new ArgumentNullException("OwnerExceptionDataTextField");

					if(this.OwnerDataValueField==String.Empty)
						throw new ArgumentNullException("OwnerExceptionDataValueField");

					foreach(object dataItem in dsCollection)
					{
						string text = DataBinder.GetPropertyValue(dataItem, OwnerDataTextField, null);
						object Value = DataBinder.GetPropertyValue(dataItem, OwnerDataValueField, null);

						this.Owners.Add(new Owner(Value, text));
					}
				}
			}

			if (dataSource != null && useDataSource) 
			{
				int index = 0;

				if(Items != null)
					Items.Clear();

				count = 0;
				foreach (object dataItem in dataSource) 
				{
					CalendarItem item = null;
					item = CreateItem(index, useDataSource, dataItem);

					count++;
					index++;
				}
			}
			else
			{
				count = 0;
				//CalendarItemCollection items = (CalendarItemCollection)Items.Clone();
				//Items.Clear();
				//_Items = items;

				foreach (CalendarItem item in Items) 
				{
					item.Initialize();
					if(useDataSource)
						item.DataBind();
						
					Controls.Add(item);
					count++;
				}
			}

			if (useDataSource) 
			{
				// Save the number of items contained for use in round trips.
				ViewState["ItemCount"] = ((dataSource != null) ? count : -1);
			}
		}

		private CalendarItem CreateItem(int itemIndex, bool dataBind, object dataItem) 
		{
			String label = null;
			String link = null;
			String description = null;
			String color = null;
			String owner = null;
			DateTime startDate = DateTime.Now.Date;
			DateTime endDate = DateTime.MinValue;

			if (DataTextField != String.Empty)
				label = DataBinder.GetPropertyValue(dataItem, DataTextField, null);
			if (DataLinkField != String.Empty)
			{
				if (DataLinkFormat != String.Empty) // apply formatting
					link = String.Format(DataLinkFormat, DataBinder.GetPropertyValue(dataItem, DataLinkField, null));
				else
					link = DataBinder.GetPropertyValue(dataItem, DataLinkField, null);
			}

			if (DataStartDateField != String.Empty)
				startDate = DateTime.Parse(DataBinder.GetPropertyValue(dataItem, DataStartDateField, null));
			if (DataEndDateField != String.Empty)
				endDate = DateTime.Parse(DataBinder.GetPropertyValue(dataItem, DataEndDateField, null));
			if (DataDescriptionField != String.Empty)
				description = DataBinder.GetPropertyValue(dataItem, DataDescriptionField, null);
			if (DataColorField != String.Empty)
				color = DataBinder.GetPropertyValue(dataItem, DataColorField, null);
			if (DataOwnerField != String.Empty)
				owner = DataBinder.GetPropertyValue(dataItem, DataOwnerField, null);			

			CalendarItem item = null;

			//CalendarItem item = new CalendarItem(itemIndex);

			if(endDate==DateTime.MinValue)
				item = new CalendarItem(itemIndex, label, startDate, startDate, link, description);
			else
				item = new CalendarItem(itemIndex, label, startDate, endDate, link, description);

			if(color!=null)
				item.LabelColor = Color.FromName(color);

			if(owner!=null)
				item.Owner = owner;
			
			CalendarItemEventArgs e = new CalendarItemEventArgs(item);
			
			if (dataBind) 
			{
				item.DataItem = dataItem;
			}
			OnItemCreated(e);

			Items.Add(item);
			
			item.Initialize();
			item.ValidateItem();

			// REMOVED v1.4: 
			Controls.Add(item);			

			if (dataBind) 
			{
				item.DataBind();
				OnItemDataBound(e);

				//item.DataItem = null;
			}

			return item;
		}

		public sealed class CalendarDataSource : ICollection 
		{

			private int dataItemCount;

			public CalendarDataSource(int dataItemCount) 
			{
				this.dataItemCount = dataItemCount;
			}

			public int Count 
			{
				get 
				{
					return dataItemCount;
				}
			}

			public bool IsReadOnly 
			{
				get 
				{
					return false;
				}
			}

			public bool IsSynchronized 
			{
				get 
				{
					return false;
				}
			}

			public object SyncRoot 
			{
				get 
				{
					return this;
				}
			}

			public void CopyTo(Array array, int index) 
			{
				for (IEnumerator e = this.GetEnumerator(); e.MoveNext();)
					array.SetValue(e.Current, index++);
			}

			public IEnumerator GetEnumerator() 
			{
				return new CalendarDataSourceEnumerator(dataItemCount);
			}


			private class CalendarDataSourceEnumerator : IEnumerator 
			{

				private int count;
				private int index;

				public CalendarDataSourceEnumerator(int count) 
				{
					this.count = count;
					this.index = -1;
				}

				public object Current 
				{
					get 
					{
						return null;
					}
				}

				public bool MoveNext() 
				{
					index++;
					return index < count;
				}

				public void Reset() 
				{
					this.index = -1;
				}
			}
		}


		/*
		/// <summary>
		/// Raises the DataBinding event. This notifies a control to perform any data binding logic that is associated with it.
		/// </summary>
		/// <param name="e">An EventArgs object that contains the event data.</param>

		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);

			if (_DataSource != null)
			{
				Items.Clear();
				_ClearChildViewState = true;

				foreach (Object dataItem in _DataSource)
				{
					String label = null;
					String link = null;
					String description = null;
					DateTime startDate = DateTime.Now.Date;
					DateTime endDate = DateTime.MinValue;

					if (DataTextField != String.Empty)
						label = DataBinder.GetPropertyValue(dataItem, DataTextField, null);
					if (DataLinkField != String.Empty)
					{
						if (DataLinkFormat != String.Empty) // apply formatting
						{
							link = String.Format(DataLinkFormat, DataBinder.GetPropertyValue(dataItem, DataLinkField, null));
						}							
						else
							link = DataBinder.GetPropertyValue(dataItem, DataLinkField, null);
					}

					if (DataStartDateField != String.Empty)
						startDate = DateTime.Parse(DataBinder.GetPropertyValue(dataItem, DataStartDateField, null));
					if (DataEndDateField != String.Empty)
						endDate = DateTime.Parse(DataBinder.GetPropertyValue(dataItem, DataEndDateField, null));
					if (DataDescriptionField != String.Empty)
						description = DataBinder.GetPropertyValue(dataItem, DataDescriptionField, null);

					CalendarItem item = null;
					if(label != null)
					{
						if(endDate==DateTime.MinValue)
							item = new CalendarItem(label, startDate, startDate, link, description);
						else
							item = new CalendarItem(label, startDate, endDate, link, description);

						item.DataItem = dataItem;
						//item.DataBind();
					}

					if (item == null)
						continue;

					Items.Add(item);
				}
			}
		}
		*/
		#endregion

		#region Calendar event handlers

		/// <summary>
		/// Handles events bubbled from child colntrols
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			if(e is CalendarItemCommandEventArgs)
			{
				CalendarItemCommandEventArgs ea = (CalendarItemCommandEventArgs)e;
				OnItemCommand(ea);
				return true;
			}
			else
			{
				base.OnBubbleEvent(source, e);
				return false;
			}
		}

		/// <summary>
		/// Called when a Calendar View changes.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		          public virtual void DoSelectedViewChange(CalendarViewSelectEventArgs e)
		{
			this.SelectedDate = e.NewDate;
			this.ViewType = e.NewViewType;
			OnSelectedViewChange(e);
		}

		/// <summary>
		/// Event handler for selection changes.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		          public virtual void OnSelectedViewChange(CalendarViewSelectEventArgs e)
		{
			if (SelectedViewChange != null)
				SelectedViewChange(this, e);
		}  

		/// <summary>
		/// Called when a downlevel browser submits the form
		/// </summary>
		/// <param name="eventArg">Event argument.</param>
		protected override void RaisePostBackEvent(string eventArg)
		{
			ProcessEvents(eventArg);
		}

		/// <summary>
		/// Called when the Calendar on the client-side submitted the form.
		/// </summary>
		/// <param name="eventArg">Event argument.</param>
		protected bool ProcessEvents(string eventArg)
		{
			if (eventArg == null || eventArg == String.Empty || eventArg == " ") // Don't know why, but the framework is giving a " " eventArg instead of null
				return false; 

			String[] events = eventArg.Split(new Char[] {';'});
			foreach (string strWholeEvent in events)
			{
				String[] parms = strWholeEvent.Split(new Char[] {','});
				if (parms[0].Length > 0)
				{
					if (parms[0].Equals("OnSelectedViewChange") && parms.GetLength(0) == 3)
					{
						CalendarViewSelectEventArgs e = new CalendarViewSelectEventArgs(DateTime.Parse(parms[1]), (CalendarViewType)Int32.Parse(parms[2]));
						DoSelectedViewChange(e);
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Raises the <see cref="ItemCreated"/> event. This allows you to provide a custom handler for the event.
		/// </summary>
		/// <param name="e">A <see cref="CalendarItemEventArgs"/> that contains event data.</param>
		/// Use the OnItemCreated method to provide a custom handler for the ItemCreated event.
		/// 
		/// <remarks>
		/// The ItemCreated event is raised when an item in the Calendar control is created, both during round-trips and at the time data is bound to the control.
		/// The ItemCreated event is commonly used to control the content and appearance of a cell in the Calendar control.
		/// Raising an event invokes the event handler through a delegate. For more information, see Raising an Event.
		/// The OnItemCreated method also allows derived classes to handle the event without attaching a delegate. This is the preferred technique for handling the event in a derived class.
		/// </remarks>
		protected virtual void OnItemCreated(CalendarItemEventArgs e) 
		{
			CalendarItemEventHandler onItemCreatedHandler = (CalendarItemEventHandler)Events[EventItemCreated];
			if (onItemCreatedHandler != null) onItemCreatedHandler(this, e);
		}

		/// <summary>
		/// Raises the <see cref="ItemCommand"/> event. This allows you to provide a custom handler for the event.
		/// </summary>
		/// <param name="e">A <see cref="CalendarItemEventArgs"/> that contains event data.</param>
		/// Use the OnItemCommand method to provide a custom handler for the ItemCommand event.
		/// 
		/// <remarks>
		/// The OnItemCommand event is raised when an item in the Calendar control is clicked.
		/// </remarks>
		protected virtual void OnItemCommand(CalendarItemCommandEventArgs e) 
		{
			CalendarItemCommandEventHandler onItemCommandHandler = (CalendarItemCommandEventHandler)Events[EventItemCommand];
			if (onItemCommandHandler != null) onItemCommandHandler(this, e);
		}

		/// <summary>
		/// Raises the <see cref="OnItemDataBound"/> event. This allows you to provide a custom handler for the event.
		/// </summary>
		/// <param name="e">A <see cref="CalendarItemEventArgs"/> that contains event data.</param>
		/// Use the OnItemCreated method to provide a custom handler for the ItemCreated event.
		/// 
		/// <remarks>
		/// Use the OnItemDataBound method to provide a custom handler for the ItemDataBound event.
		/// The ItemDataBound event is raised after an item is data bound to the Calendar control. This event provides you with the last opportunity to access the data item before it is displayed on the client. After this event is raised, the data item is nulled out and no longer available.
		/// Raising an event invokes the event handler through a delegate. For more information, see Raising an Event.
		/// The OnItemDataBound method also allows derived classes to handle the event without attaching a delegate. This is the preferred technique for handling the event in a derived class.
		/// </remarks>
		protected virtual void OnItemDataBound(CalendarItemEventArgs e) 
		{
			CalendarItemEventHandler onItemDataBoundHandler = (CalendarItemEventHandler)Events[EventItemDataBound];
			if (onItemDataBoundHandler != null) onItemDataBoundHandler(this, e);
		}

		/// <summary>
		/// ItemCreated Event. Raised when an item is created and is ready for customization.
		/// </summary>
		/// <example>
		/// <code>
		/// private void InitializeComponent()
		/// {
		///   CalendarCtrl.ItemCreated += new CalendarItemEventHandler(this.OnItemCreated);
		/// }
		/// 
		/// private void OnItemCreated(object sender, CalendarItemEventArgs e)
		///	{
		///	  Response.Write(e.Item.Label);
		///	}
		/// </code>
		/// </example>
		[
		Category("Behavior"),
		Description("Raised when an item is created and is ready for customization.")
		]
		public event CalendarItemEventHandler ItemCreated 
		{
			add 
			{
				Events.AddHandler(EventItemCreated, value);
			}
			remove 
			{
				Events.RemoveHandler(EventItemCreated, value);
			}
		}


		/// <summary>
		/// ItemCommand Event. Raised when an item is clicked.
		/// </summary>
		/// <example>
		/// <code>
		/// private void InitializeComponent()
		/// {
		///   CalendarCtrl.ItemCommand += new CalendarItemEventHandler(this.OnItemCommand);
		/// }
		/// 
		/// private void OnItemCommand(object sender, CalendarItemEventArgs e)
		///	{
		///	  Response.Write(e.Item.Label);
		///	}
		/// </code>
		/// </example>
		[
		Category("Behavior"),
		Description("Raised when an item is clicked.")
		]
		public event CalendarItemCommandEventHandler ItemCommand 
		{
			add 
			{
				Events.AddHandler(EventItemCommand, value);
			}
			remove 
			{
				Events.RemoveHandler(EventItemCommand, value);
			}
		}
		/// <summary>
		/// ItemDataBound. Raised when an item is data-bound.
		/// </summary>
		/// <example>
		/// <code>
		/// private void InitializeComponent()
		/// {
		///   CalendarCtrl.ItemDataBound += new CalendarItemEventHandler(this.OnItemDataBound);
		/// }
		/// 
		/// private void OnItemDataBound(object sender, CalendarItemEventArgs e)
		///	{
		///	  Response.Write(e.Item.Label);
		///	}
		/// </code>
		/// </example>
		[
		Category("Behavior"),
		Description("Raised when an item is data-bound.")
		]
		public event CalendarItemEventHandler ItemDataBound 
		{
			add 
			{
				Events.AddHandler(EventItemDataBound, value);
			}
			remove 
			{
				Events.RemoveHandler(EventItemDataBound, value);
			}
		}

		#endregion 

		#region Calendar state management
		/// <summary>
		/// Loads the control's previously saved view state.
		/// </summary>
		/// <param name="savedState">An object that contains the saved view state values for the control.</param>
		protected override void LoadViewState(object savedState)
		{
			if (savedState != null)
			{
				object[] state = (object[])savedState;

				base.LoadViewState(state[0]);
				if(!UseReducedViewState)
					((IStateManager)Items).LoadViewState(state[1]);
			}
		}

		/// <summary>
		/// Saves the changes to the control's view state to an Object.
		/// </summary>
		/// <returns>The object that contains the view state changes.</returns>
		protected override object SaveViewState()
		{
			object[] state = null;
			if(UseReducedViewState)
				state = new object[]
				{
					base.SaveViewState(),
				};
			else
				state = new object[]
				{
					base.SaveViewState(),
					((IStateManager)Items).SaveViewState(),
				};

			// Check to see if we're really saving anything
			foreach (object obj in state)
			{
				if (obj != null)
				{
					return state;
				}
			}

			return null;
			
		}

		/// <summary>
		/// Instructs the control to track changes to its view state.
		/// </summary>
		protected override void TrackViewState()
		{
			base.TrackViewState();
			if(!UseReducedViewState)
				((IStateManager)Items).TrackViewState();
		}
		#endregion

		#region Dummy Template Class

		private class TemplateItem : Control, INamingContainer 
		{
			public TemplateItem() 
			{
			}
		}

		#endregion
	}

	#region Public Enums
	/// <summary>
	/// Specifies the the spanning model used for control.
	/// </summary>
	public enum CalendarSpanType
	{
		/*
		/// <summary>
		/// No spanning is applied
		/// </summary>
		None, 
		*/
		/// <summary>
		/// Span will be used with text rendered as multiline
		/// </summary>
		Multiline, 

		/// <summary>
		/// Text that can't be rendered will be hidden
		/// </summary>
		Overflowed
	}

	/// <summary>
	/// Defines Matrix Span Mode, used internally
	/// </summary>
	public enum MatrixSpan
	{
		/// <summary>
		/// Hour span constant
		/// </summary>
		HourSpan, 
		/// <summary>
		/// Hour in week view constant
		/// </summary>
		WeekHourSpan, 
		/// <summary>
		/// Day span in week view
		/// </summary>
		WeekDaySpan, 
		/// <summary>
		/// Day span in month view
		/// </summary>
		DaySpan, 
		/// <summary>
		/// Month span in year view
		/// </summary>
		MonthSpan
	}


	/// <summary>
	/// Constants used by the matrix
	/// </summary>
	public enum MatrixConstants
	{
		/// <summary>
		/// Empty field, not populated
		/// </summary>
		EmptyField = -1, 
		/// <summary>
		/// Spanning continues
		/// </summary>
		SpanField = -2,
		/// <summary>
		/// More elements exists but were not rendered due to parameters specified by the user
		/// </summary>
		MoreElementsField = -3
	}

	#endregion

}