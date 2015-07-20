/**
 * Namespace: Mediachase.Web.UI.WebControls
 * Class:     CalendarDay
 *
 * Author:  Alexandre Siniouguine
 * Contact: <say@mediachase.com>
 *
 * (C) Mediachase (2003)
 */

using System;
using System.Web;
using System.Web.UI;
using Mediachase.Web.UI.WebControls.Util;

namespace Mediachase.Web.UI.WebControls
{

	/// <summary>
	/// Calendar Day class. Used by some of the week views. Internal class.
	/// </summary>
	public class CalendarDay
	{
		private DateTime date;
		private bool     isWeekend;
		private bool     isToday;
		private bool     isSelected;
		private bool     isSelectable;
		private Calendar parentCalendar;

		/// <summary>
		/// Coonstruction
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="date"></param>
		/// <param name="isWeekend"></param>
		/// <param name="isToday"></param>
		/// <param name="isSelected"></param>
		public CalendarDay(Calendar parent, DateTime date, bool isWeekend, bool isToday, bool isSelected)
		{
			this.parentCalendar = parent;
			this.date = date;
			this.isWeekend = isWeekend;
			this.isToday = isToday;
			this.isSelected = isSelected;
		}

		/// <summary>
		/// Current date
		/// </summary>
		public DateTime Date
		{
			get
			{
				return date;
			}
		}

		/// <summary>
		/// Is this day selectable
		/// </summary>
		public bool IsSelectable
		{
			get
			{
				return isSelectable;
			}
			set
			{
				isSelectable = value;
			}
		}

/// <summary>
/// Is this day selected
/// </summary>
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
		}

		/// <summary>
		/// Is this today day
		/// </summary>
		public bool IsToday
		{
			get
			{
				return isToday;
			}
		}

		/// <summary>
		/// Is weekend
		/// </summary>
		public bool IsWeekend
		{
			get
			{
				return isWeekend;
			}
		}

		/// <summary>
		/// Renders day
		/// </summary>
		/// <param name="writer"></param>
		public void Render(HtmlTextWriter writer, string NewLinkFormat, string ContextMenuFormat)
		{
			//  [1/12/2005] -- Begin
			string href = "";
			if(NewLinkFormat.Length > 0)
				href = "javascript:" + String.Format(NewLinkFormat, this.Date);

			string href2 = "";
			if(ContextMenuFormat.Length > 0)
				href2 = "return " + String.Format(ContextMenuFormat, this.Date);
			writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
			//  [1/12/2005] -- End

			writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "1");
			writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);

			// Render quarter header
			// writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "50%");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			
			//  [1/12/2005] -- Begin
			if (href!="" || href2!="")
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
				if (href!="")
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Onclick, href);
					writer.AddStyleAttribute("cursor", "hand");
				}
				if (href2!="")
				{
					writer.AddAttribute("oncontextmenu", href2);
				}
				writer.RenderBeginTag(HtmlTextWriterTag.Table);
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.RenderEndTag();
				writer.RenderEndTag();
				writer.RenderEndTag();
			}
			//  [1/12/2005] -- End

			writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "1");
			writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
			writer.RenderBeginTag(HtmlTextWriterTag.Table);

			foreach(CalendarItem item in parentCalendar.Items)
			{
				if(item.IsWithinSpanEvent(MatrixSpan.DaySpan, this.Date))
				{
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
					writer.RenderBeginTag(HtmlTextWriterTag.Td);

					item.SetRenderedDate(Date);
					item.Render(writer, parentCalendar.RenderPath);

					writer.RenderEndTag();//td
					writer.RenderEndTag();//tr
				}
			}

			writer.RenderEndTag();//table

			//  [1/12/2005] -- Begin
			if (href!="" || href2!="")
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
				writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
				writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
				if (href!="")
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Onclick, href);
					writer.AddStyleAttribute("cursor", "hand");
				}
				if (href2!="")
				{
					writer.AddAttribute("oncontextmenu", href2);
				}
				writer.RenderBeginTag(HtmlTextWriterTag.Table);
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.RenderEndTag();
				writer.RenderEndTag();
				writer.RenderEndTag();
			}
			//  [1/12/2005] -- End

			writer.RenderEndTag();//td
			writer.RenderEndTag();//tr

			writer.RenderEndTag();//table
		}
	}
}