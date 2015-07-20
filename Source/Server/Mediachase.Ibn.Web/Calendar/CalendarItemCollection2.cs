//------------------------------------------------------------------------------
// Copyright (c) 2000-2003 Microsoft Corporation. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls
{
    using System;
    using System.Web.UI;

    /// <summary>
    /// Represents a collection of CalendarItem controls.
    /// </summary>
    public class CalendarItemCollection2 : ControlCollection
    {
        /// <summary>
        /// Initializes a new instance of a CalendarItemCollection. 
        /// </summary>
        /// <param name="owner">The parent Calendar control.</param>
        public CalendarItemCollection2(Calendar owner) : base(owner)
        {
        }

        /// <summary>
        /// Verifies that a child control is a PageView.
        /// If it is, then certain properties are set.
        /// If it is not, then an exception is thrown.
        /// </summary>
        /// <param name="child">The child control.</param>
        private void VerifyChild(Control child)
        {
            if (child is CalendarItem)
            {
                //((CalendarItem)child).ParentCalendar = (Calendar)Owner;
                return;
            }

            throw new Exception(String.Format("InvalidChildType"));
        }

        /// <summary>
        /// Adds a control to the collection.
        /// </summary>
        /// <param name="child">The child control.</param>
        public override void Add(Control child)
        {
            VerifyChild(child);
            base.Add(child);
        }

        /// <summary>
        /// Adds a control to the collection at a specific index.
        /// </summary>
        /// <param name="index">The index where the control should be added.</param>
        /// <param name="child">The child control.</param>
        public override void AddAt(int index, Control child)
        {
            VerifyChild(child);
            base.AddAt(index, child);
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="value">The item to remove from the collection.</param>
        public override void Remove(Control value)
        {
            int index = IndexOf(value);
            base.Remove(value);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
        }

		/// <summary>
		/// Determines zero-based index of a CalendarItem within the collection.
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		public new int IndexOf(Control child)
		{
			VerifyChild(child);
			return base.IndexOf(child);
		}

		/// <summary>
		/// Indexer into the collection.
		/// </summary>
		public new CalendarItem this[int index]
		{
			get { return (CalendarItem)this[index]; }
		}
    }
}
