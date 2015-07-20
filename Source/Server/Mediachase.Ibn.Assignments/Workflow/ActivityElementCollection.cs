using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Mediachase.Ibn.Assignments.Workflow
{
	/// <summary>
	/// Represents activity element collection.
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(ActivityBlock))]
	[XmlInclude(typeof(CreateAssignmentActivity))]
	public class ActivityElementCollection : System.Collections.ObjectModel.Collection<ActivityElement>
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		[XmlIgnore]
		protected ActivityElement Parent { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityElementCollection"/> class.
		/// </summary>
		public ActivityElementCollection(ActivityElement parentElement)
		{
			this.Parent = parentElement;
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
		protected override void InsertItem(int index, ActivityElement item)
		{
			base.InsertItem(index, item);

			item.Parent = this.Parent;
		}

		/// <summary>
		/// Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void SetItem(int index, ActivityElement item)
		{
			base.SetItem(index, item);

			item.Parent = this.Parent;
		}

		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (ActivityElement item in this)
			{
				item.Parent = null;
			}

			base.ClearItems();
		}

		/// <summary>
		/// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
		protected override void RemoveItem(int index)
		{
			ActivityElement item = this[index];
			item.Parent = null;

			base.RemoveItem(index);
		}

		#endregion

		
	}
}
