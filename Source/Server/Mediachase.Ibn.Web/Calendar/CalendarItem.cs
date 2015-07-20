//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
namespace Mediachase.Web.UI.WebControls
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Collections;
	using System.Drawing;
	using System.Drawing.Design;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Reflection;
	using Mediachase.Web.UI.WebControls.Util;

	/// <summary>
	/// Represents a day within a Calendar.
	/// Renders individual elements, including borders and progress indicators.
	/// </summary>
	[ParseChildren(true)]
	[ToolboxItem(false)] 
	public class CalendarItem : BaseChildNode
	{
		private Calendar _Parent;

		private CalendarItemRenderStyle _RenderingStyle = CalendarItemRenderStyle.Auto;

		private object _DataItem;
		private int itemIndex;
		private ITemplate _Template = null;
		private DateTime _RenderedDate = DateTime.Now;
		private CalendarItem _NextItem = null;
		private bool _IsRendered = false;
		private object _IsAllDay = null;
		private object _Owner = "";


		#region Base Construct Methods
		/// <summary>
		/// Initializes a new instance of a CalendarItem.
		/// </summary>
		public CalendarItem() : base()
		{
		}

		/// <summary>
		/// Initializes Calendar Item
		/// </summary>
		/// <param name="itemIndex"></param>
		/// <param name="dataItem"></param>
		public CalendarItem(int itemIndex, object dataItem) 
		{
			this.itemIndex = itemIndex;
			this._DataItem = dataItem;
		}

		/// <summary>
		/// Initializes a new instance of a CalendarItem.
		/// </summary>
		public CalendarItem(int itemIndex, string label, DateTime startdate, string link, string description) : base()
		{
			this.itemIndex = itemIndex;
			this.Label = label;
			this.Description = description;
			this.StartDate = startdate;
			this.Link = link;
			//if(color!=null)
			//	LabelColor = Color.FromName(color);
		}

		/// <summary>
		/// Initializes a new instance of a CalendarItem.
		/// </summary>
		public CalendarItem(int itemIndex, string label, DateTime startdate, DateTime enddate, string link, string description) : base()
		{
			this.itemIndex = itemIndex;
			this.Label = label;
			this.Description = description;
			this.StartDate = startdate;
			this.EndDate = enddate;
			this.Link = link;
			//if(color!=null)
			//	LabelColor = Color.FromName(color);
		}
		#endregion

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			string name = this.GetType().Name;

			if (ID != String.Empty)
			{
				name += " - " + ID;
			}

			return name;
		}
		

		#region Public Properties

		/// <summary>
		/// Gets or sets the Value of the Owner object the CalendarItem is associated to.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemOwner"),
		TypeConverter(typeof(string)),
		]
		public object Owner
		{
			get
			{
				// Find owner in owners collection
				return this.ParentCalendar.Owners[_Owner.ToString()];
			}
			set
			{
				_Owner = value;
			}
		}

		/// <summary>
		/// The text string that will appear within a calendar item.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemLabel"),
		]
		public string Label
		{
			get
			{
				string szLabel = (string)ViewState["Label"];
				return (szLabel == null) ? "Some label" : szLabel;
			}

			set
			{
				ViewState["Label"] = value;
			}
		}

		/// <summary>
		/// The text string that will appear within a calendar item.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemLabelColor"),
		]
		public Color LabelColor
		{
			get
			{
				Object color = ViewState["LabelColor"];
				return (color == null) ? Color.White : (Color)color;
			}

			set
			{
				ViewState["LabelColor"] = value;
			}
		}

		/// <summary>
		/// The date that will appear within a calendar item.
		/// </summary>
		[
		Category("Appearance"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemStartDate")
		]
		public DateTime StartDate
		{
			get
			{
				Object dtStartDate = ViewState["StartDate"];
				return (dtStartDate == null) ? DateTime.Now : DateTime.Parse(dtStartDate.ToString());
			}
			set
			{
				ViewState["StartDate"] = value;
			}
		}

		/// <summary>
		/// Specifies if item will be rendered as box (with a border around).
		/// </summary>
		[
		Category("Appearance"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("RenderingStyle")
		]
		public CalendarItemRenderStyle RenderingStyle
		{
			get
			{
				return _RenderingStyle;
			}
			set
			{
				_RenderingStyle = value;
			}
		}


		/// <summary>
		/// The date that will appear within a calendar item.
		/// </summary>
		[
		Category("Appearance"),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemEndDate")
		]

		public DateTime EndDate
		{
			get
			{
				Object dtEndDate = ViewState["EndDate"];
				return (dtEndDate == null) ? DateTime.Now : DateTime.Parse(dtEndDate.ToString());
			}
			set
			{
				ViewState["EndDate"] = value;
			}
		}

		/// <summary>
		/// Returns adjusted end date, which will substract 1 tick of the date is of form
		/// Jul 1, 2004 0:00 and startdate != enddate
		/// </summary>
		public DateTime AdjustedEndDate
		{
			get
			{
				return EndDate == StartDate ? EndDate : EndDate.Minute == 0 ? EndDate.AddTicks(-1) : EndDate;
			}
		}

		/// <summary>
		/// The link string that will appear within a calendar item.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemLink"),
		]
		public string Link
		{
			get
			{
				string szLink = (string)ViewState["Link"];
				return (szLink == null) ? String.Empty : szLink;
			}

			set
			{
				ViewState["Link"] = value;
			}
		}

		/// <summary>
		/// The description string that will appear within a calendar item.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemDescription"),
		]
		public string Description
		{
			get
			{
				string szDescription = (string)ViewState["Description"];
				return (szDescription == null) ? String.Empty : szDescription;
			}

			set
			{
				ViewState["Description"] = value;
			}
		}


		/// <summary>
		/// Defines if event is all day event.
		/// </summary>
		[
		Category("Appearance"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemIsAllday"),
		]
		public bool IsAllDay
		{
			get
			{
				object allday = _IsAllDay;
				if(allday == null)
					return StartDate.Date != EndDate.Date ? true : false;
				else
				{
					/*
					if((bool)allday == false)
					{
						return StartDate.Date != EndDate.Date ? true : false;
					}
					else
					*/
					return (bool)allday;
				}
				//return (allday == null) ? StartDate.Date != EndDate.Date ? true : false : (bool)allday == false ? StartDate.Date != EndDate.Date ? true : false : (bool)allday;
			}

			set
			{
				_IsAllDay = value;
				//ViewState["IsAllDay"] = value;
			}
		}

		/// <summary>
		/// The Calendar control that contains this item.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Calendar ParentCalendar
		{
			get { return _Parent; }
		}

		/// <summary>
		/// Item index in the collection.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ItemIndex 
		{
			get 
			{
				return itemIndex;
			}
		}

		/// <summary>
		/// The CalendarItem DataItem.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object DataItem
		{
			get
			{
				return _DataItem;
			}
			set
			{
				_DataItem = value;
			}
		}

		/// <summary>
		/// Returns the parent object.
		/// </summary>
		public override object Parent
		{
			get { return ParentCalendar; }
		}

		/// <summary>
		/// Gets or sets the keyboard shortcut key (AccessKey) for setting focus to the item.
		/// </summary>
		[DefaultValue("")]
		[Category("Behavior")]
		[ResDescription("BaseAccessKey")]
		public virtual string AccessKey
		{
			get
			{
				object obj = ViewState["AccessKey"];
				return (obj == null) ? String.Empty : (string)obj;
			}

			set { ViewState["AccessKey"] = value; }
		}


		/// <summary>
		/// id of the window to target with a navigation upon selecting this item
		/// </summary>
		[
		Category("Behavior"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemTarget"),
		]
		public String Target
		{
			get
			{
				object str = ViewState["Target"];
				return ((str == null) ? String.Empty : (String)str);
			}
			set
			{
				ViewState["Target"] = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the item is enabled.
		/// </summary>
		[DefaultValue(true)]
		[Category("Behavior")]
		[ResDescription("BaseEnabled")]
		public virtual bool Enabled
		{
			get
			{
				object obj = ViewState["Enabled"];
				return (obj == null) ? true : (bool)obj;
			}

			set { ViewState["Enabled"] = value; }
		}

		/// <summary>
		/// Gets or sets the color for Event Bar filled element.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemEventBarFilledColor"),
		]
		public Color EventBarFilledColor
		{
			get
			{
				Object obj = ViewState["ItemEventBarFilledColor"];
				if(obj==null)
				{
					if(ParentCalendar.EventBarFilledColor.IsEmpty)
						return ParentCalendar.getPaletteColor(CalendarColorConstants.EventBarFilledColor);
					else
						return ParentCalendar.EventBarFilledColor;
				}
				else
					return (Color)obj;
			}

			set { ViewState["ItemEventBarFilledColor"] = value; }
		}
		
		/// <summary>
		/// Gets or sets the color for Event Bar empty element.
		/// </summary>
		[
		Category("Event Bar"),
		DefaultValue(""),
		PersistenceMode(PersistenceMode.Attribute),
		ResDescription("ItemEventBarEmptyColor"),
		]
		public Color EventBarEmptyColor
		{
			get
			{
				Object obj = ViewState["ItemEventBarEmptyColor"];
				if(obj==null)
				{
					if(ParentCalendar.EventBarEmptyColor.IsEmpty)
						return ParentCalendar.getPaletteColor(CalendarColorConstants.EventBarEmptyColor);
					else
						return ParentCalendar.EventBarEmptyColor;
				}
				else
					return (Color)obj;
			}

			set { ViewState["ItemEventBarEmptyColor"] = value; }
		}


		#endregion

		/// <summary>
		/// Is Item rendered already
		/// </summary>
		public bool IsRendered
		{
			get
			{
				return _IsRendered;
			}

			set
			{
				_IsRendered = value;
			}
		}

		/// <summary>
		/// Used for hash table algo
		/// </summary>
		public CalendarItem NextItem
		{
			get
			{
				return _NextItem;
			}

			set
			{
				_NextItem = value;
			}
		}
		/// <summary>
		/// Sets the parent of this item.
		/// </summary>
		/// <param name="parent">The parent Calendar.</param>
		public void SetParentCalendar(Calendar parent)
		{
			_Parent = parent;
		}

		public void ValidateItem()
		{
			if(this.StartDate > this.EndDate)
			{
				throw new IncorrectDatesException(this);
			}
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			//CalendarItem copy = (CalendarItem)base.Clone();
			CalendarItem copy = new CalendarItem(this.itemIndex, this.DataItem);
			copy.SetParentCalendar(this.ParentCalendar);
			copy.DataItem = this.DataItem;
			copy.Label = this.Label;
			copy.Description = this.Description;
			copy.Link = this.Link;
			return copy;
		}

		#region Help Rendering Functions

		/// <summary>
		/// Set rendered date.
		/// </summary>
		/// <param name="date"></param>
		          public void SetRenderedDate(DateTime date)
		{
			this._RenderedDate = date;
		}

		/// <summary>
		/// The uplevel tag name for the calendar item.
		/// </summary>
		protected string UpLevelTag
		{
			get { return Calendar.CalendarTagName; }
		}
		/// <summary>
		/// Adds attributes to the HtmlTextWriter.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);

			if (AccessKey != String.Empty)
			{
				writer.AddAttribute("accesskey", AccessKey);
			}

			if (!Enabled)
			{
				writer.AddAttribute("disabled", "true");
			}

			if (this.Target != String.Empty)
				writer.AddAttribute("Target", this.Target);

			if (Link != String.Empty)
				writer.AddAttribute(HtmlTextWriterAttribute.Href, this.Link);

			if (this.Description != String.Empty)
				writer.AddAttribute(HtmlTextWriterAttribute.Title, Description);
		}

		/// <summary>
		/// Writes attributes to the HtmlTextWriter.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected override void WriteAttributes(HtmlTextWriter writer)
		{
			base.WriteAttributes(writer);

			if (AccessKey != String.Empty)
			{
				writer.WriteAttribute("accesskey", AccessKey);
			}

			if (!Enabled)
			{
				writer.WriteAttribute("disabled", "true");
			}
		}

		/// <summary>
		/// Writes out CalendarItem attributes.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter that receives the markup.</param>
		protected virtual void WriteItemAttributes(HtmlTextWriter writer)
		{
		}

		/// <summary>
		/// Renders the item for uplevel browsers.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected override void RenderUpLevelPath(HtmlTextWriter writer)
		{
			RenderDownLevelPath(writer);
		}

		/// <summary>
		/// Renders the image tag.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		/// <param name="imageUrl">The url of the image.</param>
		protected virtual void RenderImage(HtmlTextWriter writer, string imageUrl)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Src, imageUrl);
			writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Align, "absmiddle");
			writer.RenderBeginTag(HtmlTextWriterTag.Img);
			writer.RenderEndTag();
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

		/// <summary>
		/// Renders content for downlevel browsers.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected virtual void DownLevelContent(HtmlTextWriter writer)
		{
			RenderContents(writer);
		}

		/// <summary>
		/// Renders the item for downlevel browsers.
		/// </summary>
		/// <param name="htmlWriter">The HtmlTextWriter object that receives the content.</param>
		protected override void RenderDownLevelPath(HtmlTextWriter htmlWriter)
		{
			HtmlInlineWriter writer = new HtmlInlineWriter(htmlWriter);
			DownLevelContent(writer);
			writer.AllowNewLine = true;
			writer.WriteLine();
		}

		/// <summary>
		/// Renders content for visual designers.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the content.</param>
		protected virtual void DesignerContent(HtmlTextWriter writer)
		{
			RenderContents(writer);
		}

		/// <summary>
		/// Renders the item for visual designers.
		/// </summary>
		/// <param name="htmlWriter">The HtmlTextWriter object that receives the content.</param>
		protected override void RenderDesignerPath(HtmlTextWriter htmlWriter)
		{
			HtmlInlineWriter writer = new HtmlInlineWriter(htmlWriter);
			DesignerContent(writer);
			writer.AllowNewLine = true;
			writer.WriteLine();
		}
		#endregion

		#region Templates
		/// <summary>
		/// Initialize templates, for internal use only.
		/// </summary>
		public void Initialize()
		{
			_Template = null;
			switch(this.ParentCalendar.ViewType)
			{
				case CalendarViewType.DayView:
					if(this.IsAllDay/*IsAllDayEvent(ParentCalendar.SelectedDate.Date)*/)
					{
						if(ParentCalendar.AllDayItemTemplate!=null)
							_Template = ParentCalendar.AllDayItemTemplate;
					}
					else
					{
						if(this.getRenderStyle() == CalendarItemRenderStyle.Box)
						{
							if(ParentCalendar.DayItemTemplate!=null)
								_Template = ParentCalendar.DayItemTemplate;
						}
						else
							if(ParentCalendar.DayTextItemTemplate!=null)
								_Template = ParentCalendar.DayTextItemTemplate;
					}
					break;
				case CalendarViewType.WeekView2:
				case CalendarViewType.WorkWeekView:
				case CalendarViewType.WeekView:
					if(this.getRenderStyle() == CalendarItemRenderStyle.Box)
					{
						if(ParentCalendar.WeekItemTemplate!=null)
							_Template = ParentCalendar.WeekItemTemplate;
					}
					else
						if(ParentCalendar.WeekTextItemTemplate!=null)
							_Template = ParentCalendar.WeekTextItemTemplate;
					break;
				case CalendarViewType.MonthView:
					if(this.getRenderStyle() == CalendarItemRenderStyle.Box)
					{
						if(ParentCalendar.MonthItemTemplate!=null)
							_Template = ParentCalendar.MonthItemTemplate;
					}
					else
						if(ParentCalendar.MonthTextItemTemplate!=null)
							_Template = ParentCalendar.MonthTextItemTemplate;
					break;
				case CalendarViewType.YearView:
					if(ParentCalendar.YearItemTemplate!=null)
						_Template = ParentCalendar.YearItemTemplate;
					break;
				case CalendarViewType.TaskView:
					if(ParentCalendar.TaskItemTemplate!=null)
						_Template = ParentCalendar.TaskItemTemplate;
					break;
			}

			if(_Template==null)
				if(ParentCalendar.DefaultItemTemplate!=null)
					_Template = ParentCalendar.DefaultItemTemplate;
			
			if(_Template!=null)
			{
				this.Controls.Clear();
				_Template.InstantiateIn(this);
			}
		}
		#endregion

		#region Contents Rendering
		/// <summary>
		/// Renders contents for downlevel and visual designers.
		/// </summary>
		/// <param name="htmlWriter">The HtmlTextWriter object that receives the content.</param>
		private void RenderContents(HtmlTextWriter htmlWriter)
		{
			HtmlInlineWriter writer = new HtmlInlineWriter(htmlWriter);

			switch(ParentCalendar.ViewType)
			{
				case CalendarViewType.DayView:
				case CalendarViewType.WeekView2:
				case CalendarViewType.WeekView:
				case CalendarViewType.WorkWeekView:
				case CalendarViewType.MonthView:
					// Render text box
					if(this.getRenderStyle() == CalendarItemRenderStyle.Box)
						RenderBoxTagAttributes(writer);
					else
						RenderTextTagAttributes(writer);
					RenderSubContent(writer);
					if(this.getRenderStyle() == CalendarItemRenderStyle.Box)
						RenderBoxEndTagAttributes(writer);
					else
						RenderTextEndTagAttributes(writer);
					break;
				case CalendarViewType.YearView:
					writer.RenderBeginTag(HtmlTextWriterTag.Br);
					RenderSubContent(writer);					
					break;
				case CalendarViewType.TaskView:
					// Add arrow
					//if(this.StartDate < ParentCalendar.DisplayStartDate)
					//	ParentCalendar.LeftArrowButton.RenderControl(writer);
					RenderSubContent(writer);
					//if(this.EndDate > ParentCalendar.DisplayEndDate)
					//	ParentCalendar.RightArrowButton.RenderControl(writer);
					break;
				default:
					RenderSubContent(writer);
					break;
			}
		}

		private void RenderSubContent(HtmlTextWriter writer)
		{
			// RENDER USING TEMPLATES
			if(_Template!=null)
				this.RenderControl(writer);
			else // RENDER W/O TEMPLATES
			{
				// add date if it's monthly view
				if(this.ParentCalendar.ViewType == CalendarViewType.YearView)
				{
					RenderText(writer, this.StartDate.ToString("MMM dd"));
					RenderText(writer, " ");
				}

				// add link
				this.AddAttributesToRender(writer);
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				if (Label != String.Empty)
				{
					RenderText(writer, Label);
				}
				writer.RenderEndTag();
			}
		}
		#endregion

		#region Progress Bar
		public void AddProgressAttributes(HtmlTextWriter writer, CalendarViewType span, DateTime now)
		{
			if(!ParentCalendar.EventBarShow)
				return;

			const int spanPeriod = 1; // span period in minutes

			int fullSpan = 0; // Full span, from beginning to the end
			int startSpan = 0; // Empty span in the beginning
			int midSpan = 0; // Actual time span
			int endSpan = 0; // Tail span
			
			int cols = 0;
			String color = "black";			

			//bool IsAllDay = IsAllDayEvent(now);
			switch(span)
			{
				case CalendarViewType.WorkWeekView:
					if(
						((StartDate.Date >= _RenderedDate.Date && StartDate.Date <= _RenderedDate.AddDays(5).Date)|| 
						(EndDate.Date >= _RenderedDate.Date && EndDate.Date <= _RenderedDate.AddDays(5).Date)) 
						&& EndDate.Date != StartDate.Date)
						IsAllDay = true;
					goto case CalendarViewType.DayView;
				case CalendarViewType.DayView:
					// Don't render items spanning from previous days, we use all day section for that
					if(!IsAllDay)
					{
						const int periodHeight = 20; // height in pixels

						// Calculate total minutes
						TimeSpan spanTime = Helper.GetHourDate(EndDate) - Helper.GetHourStartDate(StartDate);//.AddMinutes(-30);
						fullSpan = (int)System.Math.Ceiling((spanTime.TotalMinutes /*+ 1*/)/spanPeriod);

						startSpan = StartDate.Minute >=30 ? (StartDate.Minute-30)/spanPeriod : StartDate.Minute/spanPeriod;
						//endSpan = EndDate.Minute >=30 ? (60-EndDate.Minute)/spanPeriod : (30-EndDate.Minute)/spanPeriod;
						endSpan = (EndDate.Minute > 0 ? ((EndDate.Minute > 30 ? 60-EndDate.Minute : 30-EndDate.Minute ))/spanPeriod : 0);
						midSpan = fullSpan - startSpan - endSpan;

						// detect if events spans from previous day
						if(StartDate.Date<now.Date)
						{
							// Calculate span from current date
							TimeSpan spanTime2 = Helper.GetHourDate(EndDate) - now;
							fullSpan = (int)(((UInt16)spanTime2.TotalMinutes/* + 1*/))/spanPeriod;
							midSpan = fullSpan;
							startSpan = 0;
							//endSpan = fullSpan - midSpan;
						}

						// detect if event spans to next day
						if(EndDate.Date>now.Date)
						{
							fullSpan = (int)(24*60 - (now.Minute + now.Hour * 60))/spanPeriod;
							midSpan = fullSpan - startSpan;
							endSpan = 0;
						}

						// Add border to td
						color = ParentCalendar.getPaletteColorString(CalendarColorConstants.DefaultBackColor);
						if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
							color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);
						if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
							color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-BOTTOM:{0} 1px Solid;BORDER-TOP:{0} 1px Solid;BORDER-LEFT:{0} 1px Solid;width:2px", color));

						// Add color
						//if(ParentCalendar.EventBarEmptyColor.IsEmpty)
						//	writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ParentCalendar.getPaletteColorString(CalendarColorConstants.EventBarEmptyColor));
						//else
						writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(EventBarEmptyColor));

						writer.RenderBeginTag(HtmlTextWriterTag.Td);

						// Render output
						// ADD Table
						//<table cellpadding="0" width="100%" height="100%" cellspacing="0">
						
						writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
						writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
						writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
						writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
						writer.RenderBeginTag(HtmlTextWriterTag.Table);

						//<td width="6" style="text-align:center;BORDER-TOP:Black 1px Solid;BORDER-LEFT:Black 1px Solid;background-color:blue;height:25%;width:2px">&nbsp;</td>

						// Render Space at the beginning
						if(startSpan>0)
						{
							writer.RenderBeginTag(HtmlTextWriterTag.Tr);
							writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-BOTTOM:{1} 1px Solid;height:{0};width:2px", ((int)System.Math.Round((decimal)periodHeight*startSpan/30, 0)), color));
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							this.ParentCalendar.AddSpaceImage(writer, 5, ((int)System.Math.Round((decimal)periodHeight*startSpan/30, 0)));
							writer.RenderEndTag();
							writer.RenderEndTag();
						}

						// Render Actual data at the middle
						if(midSpan>0)
						{
							writer.RenderBeginTag(HtmlTextWriterTag.Tr);
							writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;height:{0};width:2px", /*"100%"*/((int)System.Math.Round((decimal)periodHeight*midSpan/30, 0))));

							// Add color
							//if(ParentCalendar.EventBarFilledColor.IsEmpty)
							//	writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ParentCalendar.getPaletteColorString(CalendarColorConstants.EventBarFilledColor));
							//else
							writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(EventBarFilledColor));
							
							// Add popup description about task length
							TimeSpan spanTime3 = EndDate - StartDate;
							string titleFormat = String.Format("{0}{1}{2}", spanTime3.Days == 0 ? "" : String.Format(ParentCalendar.EventBarDaysString, spanTime3.Days), spanTime3.Hours == 0 ? "" : String.Format(ParentCalendar.EventBarHoursString, spanTime3.Hours), spanTime3.Minutes == 0 ? "" : String.Format(ParentCalendar.EventBarMinutesString, spanTime3.Minutes));
							writer.AddAttribute(HtmlTextWriterAttribute.Title, titleFormat);
							writer.RenderBeginTag(HtmlTextWriterTag.Td);

							this.ParentCalendar.AddSpaceImage(writer, 5, ((int)System.Math.Round((decimal)periodHeight*midSpan/30, 0)));
							//RenderText(writer, "&nbsp&nbsp");

							writer.RenderEndTag();
							writer.RenderEndTag();
						}

						// Render Space at the end
						if(endSpan>0 || (endSpan==0 && midSpan==0 && startSpan==0))
						{
							writer.RenderBeginTag(HtmlTextWriterTag.Tr);
							writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-TOP:{1} 1px Solid;height:{0};width:5px", ((int)System.Math.Round((decimal)periodHeight*endSpan/30, 0)), color));
							
							writer.RenderBeginTag(HtmlTextWriterTag.Td);
							this.ParentCalendar.AddSpaceImage(writer, 5, ((int)System.Math.Round((decimal)periodHeight*endSpan/30, 0)));
							writer.RenderEndTag();
							writer.RenderEndTag();
						}

						writer.RenderEndTag(); // </table>
						writer.RenderEndTag(); // td
					}
					break;
				case CalendarViewType.TaskView:
					cols = (Helper.GetDaySpan(StartDate, AdjustedEndDate) + 1);
					startSpan = StartDate.Hour;
					endSpan = 23 - AdjustedEndDate.Hour;

					// detect if events spans from out of the boundaries
					if(StartDate.Date<now.Date)
					{
						// Calculate span from current date
						cols = (Helper.GetDaySpan(now.Date, AdjustedEndDate) + 1);
						startSpan = 0;
					}

					if(EndDate.Date>ParentCalendar.TimescaleEndDate)
					{
						cols = (int)((TimeSpan)(ParentCalendar.TimescaleEndDate - ParentCalendar.TimescaleStartDate)).TotalDays;
						endSpan = 0;
					}

					// Calculate total 1 hour spans
					fullSpan = cols * 24;
					midSpan = fullSpan - startSpan - endSpan;

					writer.RenderBeginTag(HtmlTextWriterTag.Tr);

					// Add border to td
					color = ParentCalendar.getPaletteColorString(CalendarColorConstants.DefaultBackColor);
					if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
						color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);
					if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
						color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);
					writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-RIGHT:{0} 1px Solid;BORDER-TOP:{0} 1px Solid;BORDER-LEFT:{0} 1px Solid;width:100%;height:5px;overflow:hidden;", color));

					// Add color
					//if(ParentCalendar.EventBarEmptyColor.IsEmpty)
					//	writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ParentCalendar.getPaletteColorString(CalendarColorConstants.EventBarEmptyColor));
					//else
					writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(EventBarEmptyColor));


					writer.AddAttribute(HtmlTextWriterAttribute.Height, "5px");
					writer.RenderBeginTag(HtmlTextWriterTag.Td);

					// Render output
					// ADD Table
					//<table cellpadding="0" width="100%" height="100%" cellspacing="0">
						
					writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
					writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
					writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
					writer.RenderBeginTag(HtmlTextWriterTag.Table);

					//<td width="6" style="text-align:center;BORDER-TOP:Black 1px Solid;BORDER-LEFT:Black 1px Solid;background-color:blue;height:25%;width:2px">&nbsp;</td>

					// Render Space at the beginning
					if(startSpan>0)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-RIGHT:{1} 1px Solid;width:{0}%;height:2px", ((int)System.Math.Round((decimal)100*startSpan/fullSpan, 0)), color));
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}

					// Render Actual data at the middle
					if(midSpan>0)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;width:{0}%;height:5px", ((int)System.Math.Round((decimal)100*midSpan/fullSpan, 0))));

						// Add color
						//if(ParentCalendar.EventBarFilledColor.IsEmpty)
						//	writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ParentCalendar.getPaletteColorString(CalendarColorConstants.EventBarFilledColor));
						//else
						writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(EventBarFilledColor));
							
						// Add popup description about task length
						TimeSpan spanTime3 = EndDate - StartDate;
						string titleFormat = String.Format("{0}{1}{2}", spanTime3.Days == 0 ? "" : String.Format(ParentCalendar.EventBarDaysString, spanTime3.Days), spanTime3.Hours == 0 ? "" : String.Format(ParentCalendar.EventBarHoursString, spanTime3.Hours), spanTime3.Minutes == 0 ? "" : String.Format(ParentCalendar.EventBarMinutesString, spanTime3.Minutes));
						writer.AddAttribute(HtmlTextWriterAttribute.Title, titleFormat);
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						//RenderText(writer, "&nbsp&nbsp");

						writer.RenderEndTag();
					}

					// Render Space at the end
					if(endSpan>0)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-LEFT:{1} 1px Solid;width:{0}%;height:2px", ((int)System.Math.Round((decimal)100*endSpan/fullSpan, 0)), color));
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}

					writer.RenderEndTag(); // </table>
					writer.RenderEndTag(); // td
					writer.RenderEndTag(); // tr
					break;
				case CalendarViewType.WeekView2:
				case CalendarViewType.MonthView:

					cols = (Helper.GetDaySpan(StartDate, AdjustedEndDate) + 1);
					startSpan = StartDate.Hour;
					endSpan = 23 - AdjustedEndDate.Hour;

					// detect if events spans from previous week
					if(StartDate.Date<now.Date)
					{
						// Calculate span from current date
						cols = (Helper.GetDaySpan(now.Date, AdjustedEndDate) + 1);
						startSpan = 0;
					}

					// detect if event spans to next week
					if(span == CalendarViewType.MonthView)
					{
						if(cols>(7 - Helper.LocalizedDayOfWeek(now.DayOfWeek, ParentCalendar.FirstDayOfWeek)))
						{
							cols = (7 - Helper.LocalizedDayOfWeek(now.DayOfWeek, ParentCalendar.FirstDayOfWeek));
							endSpan = 0;
						}
					}
					else
					{
						if(cols>1)
						{
							cols = 1; //(7 - Helper.LocalizedDayOfWeek(now.DayOfWeek));
							endSpan = 0;
						}
					}

					// Calculate total minutes
					fullSpan = cols * 24;
					midSpan = fullSpan - startSpan - endSpan;

					writer.RenderBeginTag(HtmlTextWriterTag.Tr);

					// Add border to td
					color = ParentCalendar.getPaletteColorString(CalendarColorConstants.DefaultBackColor);
					if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
						color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);
					if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
						color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);
					writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-RIGHT:{0} 1px Solid;BORDER-TOP:{0} 1px Solid;BORDER-LEFT:{0} 1px Solid;width:100%;height:5px;overflow:hidden;", color));

					// Add color
					//if(ParentCalendar.EventBarEmptyColor.IsEmpty)
					//	writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ParentCalendar.getPaletteColorString(CalendarColorConstants.EventBarEmptyColor));
					//else
					writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(EventBarEmptyColor));


					writer.AddAttribute(HtmlTextWriterAttribute.Height, "5px");
					writer.RenderBeginTag(HtmlTextWriterTag.Td);

					// Render output
					// ADD Table
					//<table cellpadding="0" width="100%" height="100%" cellspacing="0">
						
					writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
					writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
					writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
					writer.RenderBeginTag(HtmlTextWriterTag.Table);

					//<td width="6" style="text-align:center;BORDER-TOP:Black 1px Solid;BORDER-LEFT:Black 1px Solid;background-color:blue;height:25%;width:2px">&nbsp;</td>

					// Render Space at the beginning
					if(startSpan>0)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-RIGHT:{1} 1px Solid;width:{0}%;height:2px", ((int)System.Math.Round((decimal)100*startSpan/fullSpan, 0)), color));
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}

					// Render Actual data at the middle
					if(midSpan>0)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;width:{0}%;height:5px", ((int)System.Math.Round((decimal)100*midSpan/fullSpan, 0))));

						// Add color
						//if(ParentCalendar.EventBarFilledColor.IsEmpty)
						//	writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ParentCalendar.getPaletteColorString(CalendarColorConstants.EventBarFilledColor));
						//else
						writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(EventBarFilledColor));
							
						// Add popup description about task length
						TimeSpan spanTime3 = EndDate - StartDate;
						string titleFormat = String.Format("{0}{1}{2}", spanTime3.Days == 0 ? "" : String.Format(ParentCalendar.EventBarDaysString, spanTime3.Days), spanTime3.Hours == 0 ? "" : String.Format(ParentCalendar.EventBarHoursString, spanTime3.Hours), spanTime3.Minutes == 0 ? "" : String.Format(ParentCalendar.EventBarMinutesString, spanTime3.Minutes));
						writer.AddAttribute(HtmlTextWriterAttribute.Title, titleFormat);
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						//RenderText(writer, "&nbsp&nbsp");

						writer.RenderEndTag();
					}

					// Render Space at the end
					if(endSpan>0)
					{
						writer.AddAttribute(HtmlTextWriterAttribute.Style, String.Format("text-align:center;BORDER-LEFT:{1} 1px Solid;width:{0}%;height:2px", ((int)System.Math.Round((decimal)100*endSpan/fullSpan, 0)), color));
						writer.RenderBeginTag(HtmlTextWriterTag.Td);
						writer.RenderEndTag();
					}

					writer.RenderEndTag(); // </table>
					writer.RenderEndTag(); // td
					writer.RenderEndTag(); // tr
					break;
				case CalendarViewType.YearView:
					break;
			}
		}
		#endregion

		#region Box Rendering
		public void RenderBoxEndTagAttributes(HtmlTextWriter writer)
		{

			// Shadow
			if(ParentCalendar.UseShadows && ParentCalendar.ViewType != CalendarViewType.TaskView && !this.IsAllDay)
			{
				writer.RenderEndTag(); // td
				writer.RenderEndTag(); // table
			}

			writer.RenderEndTag(); // td
			writer.RenderEndTag(); // tr
			writer.RenderEndTag(); // table

			writer.RenderEndTag(); // DIV
		}

		public void RenderBoxTagAttributes(HtmlTextWriter writer)
		{
			bool DrawTopBorder = true;
			bool DrawBottomBorder = true;

			bool DrawEndBorder = true;
			bool DrawStartBorder = true;
			String height = ((Unit)this.ParentCalendar.ItemHeight).ToString();
			TimeSpan spanTime;
			int colSpan;

			// Add Shadow
			if(ParentCalendar.UseShadows && ParentCalendar.ViewType != CalendarViewType.TaskView && !this.IsAllDay)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "1");
				writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
				writer.AddStyleAttribute("FILTER", "Shadow(color=gray, direction=120, strength=4);");
				writer.AddStyleAttribute("color", "#FF0000");
				if(ParentCalendar.ViewType == CalendarViewType.MonthView)
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "20");				
				else if(ParentCalendar.ViewType == CalendarViewType.WeekView2)
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "20");				
				else
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");

				writer.RenderBeginTag(HtmlTextWriterTag.Table);		
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
			}

			// ADD Table
			//<table cellpadding="0" width="100%" height="100%" cellspacing="0">
			writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
			if(ParentCalendar.ViewType == CalendarViewType.MonthView)
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "20");				
			else if(ParentCalendar.ViewType == CalendarViewType.WeekView2)
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "20");				
			else
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");

			writer.RenderBeginTag(HtmlTextWriterTag.Table);		
	
			// Render precise time share indicator
			switch(ParentCalendar.ViewType)
			{
				case CalendarViewType.WorkWeekView:
				case CalendarViewType.DayView:
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					AddProgressAttributes(writer, ParentCalendar.ViewType, this._RenderedDate);
					//period = MatrixSpan.HourSpan;
					break;
				case CalendarViewType.MonthView:
					AddProgressAttributes(writer, ParentCalendar.ViewType, this._RenderedDate);
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					//period = MatrixSpan.DaySpan;
					break;
				case CalendarViewType.TaskView:
					AddProgressAttributes(writer, ParentCalendar.ViewType, this._RenderedDate);
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					break;
				case CalendarViewType.WeekView2:
					AddProgressAttributes(writer, ParentCalendar.ViewType, this._RenderedDate);
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					break;
				default:
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					AddProgressAttributes(writer, ParentCalendar.ViewType, this._RenderedDate);
					//period = MatrixSpan.MonthSpan;
					break;
			}


			//bool IsAllDay = IsAllDayEvent(_RenderedDate);
			switch(ParentCalendar.ViewType)
			{
				case CalendarViewType.WorkWeekView:
					/*
					if(
						((StartDate.Date >= _RenderedDate.Date && StartDate.Date <= _RenderedDate.AddDays(5).Date)|| 
						(EndDate.Date >= _RenderedDate.Date && EndDate.Date <= _RenderedDate.AddDays(5).Date)) 
						&& EndDate.Date != StartDate.Date)
						IsAllDay = true;
					*/

					if(IsAllDay)
					{ // multi day events

						//spanTime = EndDate - StartDate;
						colSpan = (Helper.GetDaySpan(StartDate, this.AdjustedEndDate.Date) + 1);

						// detect if events spans from previous week
						if(StartDate.Date<this._RenderedDate.Date)
						{
							// Calculate span from current date
							//TimeSpan spanTime2 = EndDate - _RenderedDate.Date;
							colSpan = (Helper.GetDaySpan(_RenderedDate.Date, this.AdjustedEndDate.Date) + 1);

							DrawStartBorder = false;
						}

						// detect if event spans to next week
						// it happens if last day of the week is less than EndDate of event
						// Get last day of the week
						if(ParentCalendar.Dates[ParentCalendar.Dates.Count-1].Date < this.AdjustedEndDate.Date)
							DrawEndBorder = false;
					}
					else
						goto case CalendarViewType.DayView;
					break;
				case CalendarViewType.DayView:
					if(IsAllDay)
					{ // all day events
						spanTime = AdjustedEndDate - StartDate;
						colSpan = ((Int32)spanTime.TotalHours + 1);

						// detect if events spans from previous week
						if(StartDate<this._RenderedDate)
						{
							// Calculate span from current date
							TimeSpan spanTime2 = AdjustedEndDate - _RenderedDate;
							colSpan = (int)System.Math.Ceiling(spanTime2.TotalHours);

							DrawStartBorder = false;
						}

						// detect if event spans to next day
						if(colSpan>(24 - _RenderedDate.Hour.GetHashCode()))
							DrawEndBorder = false;
					}
					else // regular day events
					{

						spanTime = AdjustedEndDate - StartDate;
						colSpan = ((Int32)spanTime.TotalHours + 1);
					
						//[2005-03-02] - Alexander Kotelnikov----
						//height = "100%";
						int iBegHour = StartDate.Hour;
						int iEndHour = AdjustedEndDate.Hour;
						int coeff = (iEndHour-iBegHour)*2;
						if(StartDate.Minute>=30)
							coeff--;
						if(AdjustedEndDate.Minute>0 && AdjustedEndDate.Minute<=30)
							coeff++;
						if(AdjustedEndDate.Minute>30)
							coeff+=2;
						//int iTotMin = (int)Math.Round((decimal)spanTime.TotalMinutes);
						//int iHeight = (int)Math.Ceiling((double)iTotMin/30);
						height = (coeff*20)+"px";
						// ---

						// detect if events spans from previous week
						if(StartDate<this._RenderedDate)
						{
							// Calculate span from current date
							TimeSpan spanTime2 = AdjustedEndDate - _RenderedDate;
							colSpan = ((Int32)spanTime2.TotalHours + 1);

							DrawTopBorder = false;
						}

						// detect if event spans to next week
						if(colSpan>(23 - _RenderedDate.Hour.GetHashCode()))
							DrawBottomBorder = false;
					}
					break;
				case CalendarViewType.WeekView2:
				case CalendarViewType.WeekView:
					
					if(ParentCalendar.ViewType == CalendarViewType.WeekView)
						height = "100%";

					//spanTime = EndDate - StartDate;
					colSpan = (Helper.GetDaySpan(StartDate, AdjustedEndDate) + 1);

					// detect if events spans from previous week
					if(StartDate.Date<this._RenderedDate.Date)
					{
						// Calculate span from current date
						//TimeSpan spanTime2 = EndDate - _RenderedDate;
						colSpan = (Helper.GetDaySpan(_RenderedDate, AdjustedEndDate) + 1);

						if(ParentCalendar.ViewType == CalendarViewType.WeekView)
							DrawTopBorder = false;
						else
							DrawStartBorder = false;
					}

					// detect if event spans to next week
					if(ParentCalendar.ViewType == CalendarViewType.WeekView)
					{
						if(colSpan>(7 - Helper.LocalizedDayOfWeek(_RenderedDate.DayOfWeek, ParentCalendar.FirstDayOfWeek)))
							DrawBottomBorder = false;
					}
					else
					{
						if(colSpan>1)
							DrawEndBorder = false;
					}

					break;
				case CalendarViewType.MonthView:

					//spanTime = EndDate - StartDate;
					colSpan = (Helper.GetDaySpan(StartDate, EndDate) + 1);

					// detect if events spans from previous week
					if(StartDate.Date<this._RenderedDate.Date)
					{
						// Calculate span from current date
						//TimeSpan spanTime2 = EndDate - _RenderedDate.Date;
						colSpan = (Helper.GetDaySpan(_RenderedDate.Date, EndDate) + 1);

						DrawStartBorder = false;
					}

					// detect if event spans to next week
					if(colSpan>(7 - Helper.LocalizedDayOfWeek(_RenderedDate.DayOfWeek, ParentCalendar.FirstDayOfWeek)))
						DrawEndBorder = false;

					break;

				case CalendarViewType.TaskView:

					//spanTime = EndDate - StartDate;
					colSpan = (Helper.GetDaySpan(StartDate, AdjustedEndDate));

					// detect if events spans from previous week
					if(StartDate.Date<this._RenderedDate.Date)
					{
						// Calculate span from current date
						//TimeSpan spanTime2 = EndDate - _RenderedDate.Date;
						colSpan = (Helper.GetDaySpan(_RenderedDate.Date, AdjustedEndDate));

						DrawStartBorder = false;
					}

					// detect if event spans to next week
					int total = (int)((TimeSpan)(ParentCalendar.TimescaleEndDate - ParentCalendar.TimescaleStartDate)).TotalDays;
					if(colSpan>total - (int)((TimeSpan)(this._RenderedDate.Date - ParentCalendar.TimescaleStartDate)).TotalDays)
						DrawEndBorder = false;

					break;
				case CalendarViewType.YearView:
					break;
				default:
					break;
			}

			// ADD DIV TAG
			String color = "black";
			String width = "1px";
			String style = "solid";

			color = ParentCalendar.getPaletteColorString(Util.CalendarColorConstants.DefaultBackColor);

			if(!ParentCalendar.ControlStyle.BorderColor.IsEmpty)
				color = ColorTranslator.ToHtml(ParentCalendar.ControlStyle.BorderColor);

			if(!ParentCalendar.ControlStyle.BorderWidth.IsEmpty)
				width = ParentCalendar.ControlStyle.BorderWidth.ToString();

			if(ParentCalendar.ControlStyle.BorderStyle.ToString() != String.Empty)
				style = ParentCalendar.ControlStyle.BorderStyle.ToString();

			if(ParentCalendar.CalendarItemDefaultStyle.HorizontalAlign.ToString().ToLower() != "notset")	//  [9/15/2004] Al.Kotelnikov
				writer.AddStyleAttribute("text-align", ParentCalendar.CalendarItemDefaultStyle.HorizontalAlign.ToString()); //  [9/15/2004] Al.Kotelnikov
			//writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Unit.Percentage(100).ToString());
			
			if(DrawEndBorder)
				writer.AddStyleAttribute("BORDER-RIGHT", String.Format("{0} {1} {2}", color, width, style));
			
			if(DrawTopBorder)
				writer.AddStyleAttribute("BORDER-TOP", String.Format("{0} {1} {2}", color, width, style));
			
			if(DrawStartBorder)
				writer.AddStyleAttribute("BORDER-LEFT", String.Format("{0} {1} {2}", color, width, style));
			
			if(DrawBottomBorder)
				writer.AddStyleAttribute("BORDER-BOTTOM", String.Format("{0} {1} {2}", color, width, style));
			
			writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(LabelColor));
			
			if(ParentCalendar.SpanType == CalendarSpanType.Overflowed)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, height);
				writer.AddStyleAttribute("overflow", "hidden");
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);

			if(ParentCalendar.SpanType == CalendarSpanType.Overflowed)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, height);
				writer.AddStyleAttribute("overflow", "hidden");
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			/*
			if(ParentCalendar.ViewType == CalendarViewType.TaskView)
				RenderBoxEndTagAttributes(writer);
			*/
		}
		#endregion

		#region Text Rendering
		public void RenderTextEndTagAttributes(HtmlTextWriter writer)
		{
			// Dont render anything if template is specified
			if(this._Template != null)
				return;

			writer.RenderEndTag(); // DIV
		}

		public void RenderTextTagAttributes(HtmlTextWriter writer)
		{
			// Dont render anything if template is specified
			if(this._Template != null)
				return;

			String height = (((Unit)this.ParentCalendar.ItemHeight).Value + 9).ToString();
			bool IsAllDay = false;

			switch(ParentCalendar.ViewType)
			{
				case CalendarViewType.WorkWeekView:
					if(
						((StartDate.Date >= _RenderedDate.Date && StartDate.Date <= _RenderedDate.AddDays(5).Date)|| 
						(EndDate.Date >= _RenderedDate.Date && EndDate.Date <= _RenderedDate.AddDays(5).Date)) 
						&& EndDate.Date != StartDate.Date)
						IsAllDay = true;

					if(IsAllDay)
					{ // multi day events
					}
					else
						goto case CalendarViewType.DayView;
					break;
				case CalendarViewType.DayView:
					if(!IsAllDay)
						height = "100%";
					break;
				case CalendarViewType.WeekView2:
				case CalendarViewType.WeekView:
					if(ParentCalendar.ViewType == CalendarViewType.WeekView)
						height = "100%";
					break;
				default:
					break;
			}
			// ADD DIV TAG
			if(ParentCalendar.SpanType == CalendarSpanType.Overflowed)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, height);
				writer.AddStyleAttribute("overflow", "hidden");
			}

			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
		}
		#endregion

		#region Date Filter Functions
		/// <summary>
		/// Will determine if current event is all day event. 
		/// </summary>
		/// <param name="now"></param>
		/// <returns></returns>
		public bool IsAllDayEvent(DateTime now)
		{
			if(((now.Date >= StartDate.Date && now.Date <= AdjustedEndDate.Date) /*|| (now.Date >= StartDate.Date && now.Date <= EndDate.Date)*/) && this.IsAllDay)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Determines if event is withing current span
		/// </summary>
		/// <param name="span"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		public bool IsWithinSpanEvent(MatrixSpan span, DateTime now)
		{
			// June 16 04: added .AddTicks(-1) method call to endDate to allow filter events that end at 0:00
			return IsWithinSpanEvent(StartDate, AdjustedEndDate, span, now);
		}

		public bool IsWithinSpanEvent(DateTime startDate, DateTime endDate, MatrixSpan span, DateTime now)
		{
			switch(span)
			{
				case MatrixSpan.WeekHourSpan:
					if(!IsAllDayEvent(now) && (startDate.Date == endDate.Date && Helper.GetHourStartDate(startDate) <= now && 
						((Helper.GetHourDate(endDate) == now && now.Minute!=30) || Helper.GetHourDate(endDate) > now)))
						return true;
					break;
				case MatrixSpan.HourSpan:
					// Don't add up all day events
					if(!IsAllDayEvent(now) && (Helper.GetHourStartDate(startDate) <= now && Helper.GetHourDate(endDate) /*>= end date should be greater than now to be included in the span */>= now))
						return true;
					break;
				case MatrixSpan.WeekDaySpan:
					// June 16 2004: Added IsAllDayEvent(now)
					if((endDate.Date >= now.Date && startDate.Date <= now.Date && IsAllDayEvent(now) /*startDate.Date != endDate.Date*/))
						return true;
					break;
				case MatrixSpan.DaySpan:
					if(endDate.Date >= now.Date && startDate.Date <= now.Date)
						return true;
					break;
				case MatrixSpan.MonthSpan:
					if(endDate >= now && startDate <= now)
						return true;
					break;
			}
			return false;
		}

		/// <summary>
		/// Determines if current event belongs to the span
		/// </summary>
		/// <param name="index">item number within matrix element, 0 can mean week or month starting</param>
		/// <param name="span"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		public bool IsCurrentSpanEvent(int index, MatrixSpan span, DateTime now)
		{
			// June 16 04: added .AddTicks(-1) method call to endDate to allow filter events that end at 0:00
			return IsCurrentSpanEvent(StartDate, AdjustedEndDate, index, span, now);
		}

		public bool IsCurrentSpanEvent(DateTime startDate, DateTime endDate, int index, MatrixSpan span, DateTime now)
		{
			switch(span)
			{
				case MatrixSpan.WeekHourSpan:
					if(!IsAllDayEvent(now) && endDate.Date == startDate.Date && ((index==0 && endDate >= now && startDate <= now) || Helper.GetHourStartDate(startDate) == now))
						return true;
					break;
				case MatrixSpan.HourSpan:
					// Don't render items spanning from previous days, we use all day section for that
					//if(!IsAllDayEvent(item, now) && (GetHourCycleDate(item.StartDate) == now || (item.StartDate now == item.EndDate.Date))
					//if(!IsAllDayEvent(item, now) && (GetHourStartDate(item.StartDate) <= now && GetHourDate(item.EndDate) >= now))
					
					
					if(!IsAllDayEvent(now) && (index==0 && endDate >= now && startDate <= now) || Helper.GetHourStartDate(startDate) == now && !IsAllDayEvent(now))
						return true;
					break;
				case MatrixSpan.WeekDaySpan: // only events spanning multiple days
					// June 16 2004: Added IsAllDayEvent(now)
					// June 28 2004: Move IsAllDayEvent to new place next to &&
					if(((startDate.Date == now) || (index==0 && endDate.Date >= now && startDate.Date <= now)) && IsAllDayEvent(now)/*startDate.Date != endDate.Date*/)
						return true;
					break;
				case MatrixSpan.DaySpan:
					if((startDate.Date == now) || (index==0 && endDate.Date >= now && startDate.Date <= now))
						return true;
					break;
				case MatrixSpan.MonthSpan:
					if((startDate == now) || (index==0 && endDate >= now && startDate <= now))
						return true;
					break;
			}

			return false;
		}
		#endregion

		/// <summary>
		/// Raises Bubble event
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			if(e is CommandEventArgs)
			{
				CalendarItemCommandEventArgs args = new CalendarItemCommandEventArgs(this, source, (CommandEventArgs)e);
				RaiseBubbleEvent(this, args);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets rendering style for the calendar item
		/// </summary>
		/// <returns></returns>
		public CalendarItemRenderStyle getRenderStyle()
		{
			CalendarItemRenderStyle renderStyle = RenderingStyle;

			if(/*ParentCalendar.SpanType != CalendarSpanType.None && */ParentCalendar.ViewType != CalendarViewType.YearView)
				if(ParentCalendar.ViewType != CalendarViewType.TaskView)
				{
					if(RenderingStyle == CalendarItemRenderStyle.Auto) // decide on style
					{
						// default is BOX
						renderStyle = CalendarItemRenderStyle.Box;

						// determine if we want text instead
						if(ParentCalendar.ViewType == CalendarViewType.MonthView ||
							ParentCalendar.ViewType == CalendarViewType.WeekView2)
						{
							if(StartDate.Date==EndDate.Date && !this.IsAllDay)
								return CalendarItemRenderStyle.Text;
						}

					} // else return chosen one

				}
			return renderStyle;
		}
	}

	/// <summary>
	/// Specifies the rendering style for calendar item.
	/// </summary>
	public enum CalendarItemRenderStyle
	{
		/// <summary>
		/// Control will decide the most appropriate way to render items.
		/// </summary>
		Auto, 
		/// <summary>
		/// Items will be always rendered as text
		/// </summary>
		Text,
		/// <summary>
		/// Items will be always rendered as box (with a border)
		/// </summary>
		Box
	}

	#region Exceptions
	[Serializable]
	public class IncorrectDatesException : System.Exception
	{
		public IncorrectDatesException()
			: this(null)
		{
		}

		public IncorrectDatesException(CalendarItem item)
			: base(String.Format("End Date '{1}' must be greater than Start Date '{0}'.", item.StartDate, item.EndDate))
		{
		}

		public IncorrectDatesException(CalendarItem item, Exception  innerException)
			: base(String.Format("End Date '{1}' must be greater than Start Date '{0}'.", item.StartDate, item.EndDate), innerException)
		{
		}
	}
	#endregion

}