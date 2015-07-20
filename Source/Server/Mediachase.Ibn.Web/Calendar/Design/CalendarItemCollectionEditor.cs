//------------------------------------------------------------------------------
// Copyright (c) 2003 Mediachase. All Rights Reserved.
//------------------------------------------------------------------------------

namespace Mediachase.Web.UI.WebControls.Design
{
    using System;
    using System.ComponentModel.Design;

    /// <summary>
    /// Designer editor for the CalendarItem collection.
    /// </summary>
    public class CalendarItemCollectionEditor : ItemCollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the CalendarItemCollectionEditor class.
        /// </summary>
        /// <param name="type">The type of collection this object is to edit.</param>
        public CalendarItemCollectionEditor(Type type) : base(type)
        {
        }

        /// <summary>
        /// Returns the type of objects the editor can create.
        /// </summary>
        /// <returns>An array of types.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[]
            {
                typeof(CalendarItem),
            };
        }
    }
}
