using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents activity info collection.
	/// </summary>
	[Serializable]
	public class ActivityInfoCollection: Collection<ActivityInfo>
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		[XmlIgnore]
		protected object Parent { get; set; }

		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityInfoCollection"/> class.
		/// </summary>
		public ActivityInfoCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityInfoCollection"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		internal ActivityInfoCollection(object parent)
		{
			this.Parent = parent;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
		/// <param name="item">The object to insert. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void InsertItem(int index, ActivityInfo item)
		{
			base.InsertItem(index, item);

			if (this.Parent is BusinessProcessInfo)
				item.BusinessProcess = (BusinessProcessInfo)this.Parent;
			else if (this.Parent is ActivityInfo)
				item.Parent = (ActivityInfo)this.Parent;
			else
				item.Parent = null;
		}

		/// <summary>
		/// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void RemoveItem(int index)
		{
			ActivityInfo item = this[index];
			item.Parent = null;
			item.BusinessProcess = null;

			base.RemoveItem(index);
		}

		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void SetItem(int index, ActivityInfo item)
		{
			base.SetItem(index, item);

			if (this.Parent is BusinessProcessInfo)
				item.BusinessProcess = (BusinessProcessInfo)this.Parent;
			else if (this.Parent is ActivityInfo)
				item.Parent = (ActivityInfo)this.Parent;
			else
				item.Parent = null;
		}

		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (ActivityInfo item in this)
			{
				item.BusinessProcess = null;
				item.Parent = null;
			}

			base.ClearItems();
		}
		#endregion
	}
}
