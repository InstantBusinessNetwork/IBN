//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------
namespace Mediachase.Web.UI.WebControls
{
	using System;
	using System.ComponentModel;
	using System.Drawing.Design;
	using System.Web.UI;

	/// <summary>
	/// Collection of CalendarItems within a Calendar.
	/// </summary>
	[Editor(typeof(Mediachase.Web.UI.WebControls.Design.CalendarItemCollectionEditor), typeof(UITypeEditor))]
	public class CalendarItemCollection : BaseChildNodeCollection
	{
		private Calendar _Parent;

		/// <summary>
		/// Initializes a new instance of a CalendarItemCollection.
		/// </summary>
		public CalendarItemCollection() : base()
		{
			_Parent = null;
		}

		/// <summary>
		/// Initializes a new instance of a CalendarItemCollection.
		/// </summary>
		/// <param name="parent">The parent Calendar of this collection.</param>
		public CalendarItemCollection(Calendar parent) : base()
		{
			_Parent = parent;
		}

		/// <summary>
		/// Creates a new deep copy of the current collection.
		/// </summary>
		/// <returns>A new object that is a deep copy of this instance.</returns>
		public override object Clone()
		{
			CalendarItemCollection copy = (CalendarItemCollection)base.Clone();

			copy._Parent = this._Parent;
			foreach(CalendarItem item in copy)
			{
				item.SetParentCalendar(this._Parent);
			}

			return copy;
		}

		/// <summary>
		/// The parent Calendar containing this collection of items.
		/// </summary>
		private Calendar ParentCalendar
		{
			get { return _Parent; }
		}

		/// <summary>
		/// Tracks the number of calendaritems after a clear operation.
		/// </summary>
		protected override void OnClear()
		{
			base.OnClear();

			if (!Reloading && (ParentCalendar != null))
			{
			}

			foreach (CalendarItem item in List)
			{
				item.SetParentCalendar(null);
			}
		}

		/// <summary>
		/// Tracks the number of calendaritems after a remove operation.
		/// </summary>
		/// <param name="index">The index of the item being removed.</param>
		/// <param name="value">The item being removed.</param>
		protected override void OnRemove(int index, object value)
		{
			if (value is CalendarItem)
			{
				if (!Reloading && (ParentCalendar != null))
				{
				}
			}

			base.OnRemove(index, value);
		}

		/// <summary>
		/// Cleans up SelectedIndex property
		/// </summary>
		/// <param name="index">The index of the object that was removed.</param>
		/// <param name="value">The object that was removed.</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);

			((CalendarItem)value).SetParentCalendar(null);
		}

		/// <summary>
		/// Tracks the number of calendaritems after an insert operation.
		/// </summary>
		/// <param name="index">The index of the item being inserted.</param>
		/// <param name="value">The item being inserted.</param>
		protected override void OnInsert(int index, object value)
		{
			CalendarItem item = (CalendarItem)value;

			if (item.ParentCalendar != null)
			{
				if(item.ParentCalendar.Items.Contains(item))
					item.ParentCalendar.Items.Remove(item);
			}

			SetItemProperties(item);

			base.OnInsert(index, item);

		}

		/// <summary>
		/// Sets properties of the CalendarItem before being added.
		/// </summary>
		/// <param name="item">The CalendarItem to be set.</param>
		private void SetItemProperties(CalendarItem item)
		{
			item.SetParentCalendar(ParentCalendar);
		}

		/// <summary>
		/// Adds a CalendarItem to the collection.
		/// </summary>
		/// <param name="item">The CalendarItem to add.</param>
		public void Add(CalendarItem item)
		{
			if (!Contains(item))
			{
				List.Add(item);
			}
		}

		/// <summary>
		/// Returns collection with items falling into defined date boundaries.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public CalendarItemCollection GetRange(DateTime start, DateTime end)
		{
			CalendarItemCollection col = new CalendarItemCollection();
			col._Parent = _Parent;
			foreach (CalendarItem item in this)
			{
				if((item.StartDate >= start && item.StartDate <= end) || (item.EndDate >= start && item.EndDate <= end))
					col.Add(item);
			}

			return col;
		}

		/// <summary>
		/// Adds a CalendarItem to the collection at a specific index.
		/// </summary>
		/// <param name="index">The index at which to add the item.</param>
		/// <param name="item">The CalendarItem to add.</param>
		public void AddAt(int index, CalendarItem item)
		{
			if (!Contains(item))
			{
				List.Insert(index, item);
			}
		}

		/// <summary>
		/// Determines if a CalendarItem is in the collection.
		/// </summary>
		/// <param name="item">The CalendarItem to search for.</param>
		/// <returns>true if the CalendarItem exists within the collection. false otherwise.</returns>
		public bool Contains(CalendarItem item)
		{
			return List.Contains(item);
		}

		/// <summary>
		/// Determines zero-based index of a CalendarItem within the collection.
		/// </summary>
		/// <param name="item">The CalendarItem to locate within the collection.</param>
		/// <returns>The zero-based index.</returns>
		public int IndexOf(CalendarItem item)
		{
			return List.IndexOf(item);
		}

		/// <summary>
		/// Removes a CalendarItem from the collection.
		/// </summary>
		/// <param name="item">The CalendarItem to remove.</param>
		public void Remove(CalendarItem item)
		{
			List.Remove(item);
		}

		/// <summary>
		/// Indexer into the collection.
		/// </summary>
		public CalendarItem this[int index]
		{
			get { return (CalendarItem)List[index]; }
		}
	}
}
