using System;
using System.Collections;

namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaFieldCollection.
	/// </summary>
	public class MetaFieldCollection : ReadOnlyCollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetaFieldCollection"/> class.
		/// </summary>
		internal MetaFieldCollection()
		{
		}

		/// <summary>
		/// Adds the specified new item.
		/// </summary>
		/// <param name="newItem">The new item.</param>
		internal void Add(MetaField newItem)
		{
			this.InnerList.Add(newItem);
		}

		/// <summary>
		/// Deletes the specified mate field id.
		/// </summary>
		/// <param name="MateFieldId">The mate field id.</param>
		internal void Delete(int mateFieldId)
		{
			foreach (MetaField item in this)
			{
				if (item.Id == mateFieldId)
				{
					this.InnerList.Remove(item);
					break;
				}
			}
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		internal void Clear()
		{
			this.InnerList.Clear();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public int IndexOf(MetaField field)
		{
			if (field == null)
				throw new ArgumentNullException("field");

			for (int Index = 0; Index < this.Count; Index++)
			{
				if (this[Index].Id == field.Id)
					return Index;
			}
			return -1;
		}

		/// <summary>
		/// Determines whether [contains] [the specified field].
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified field]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(MetaField field)
		{
			if (field == null)
				throw new ArgumentNullException("field");

			foreach (MetaField item in this)
			{
				if (item.Id == field.Id)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the <see cref="MetaField"/> at the specified index.
		/// </summary>
		/// <value></value>
		public MetaField this[int index]
		{
			get
			{
				return (MetaField)this.InnerList[index];
			}
		}

		/// <summary>
		/// Gets the <see cref="MetaField"/> with the specified name.
		/// </summary>
		/// <value></value>
		public MetaField this[string name]
		{
			get
			{
				foreach (MetaField item in this)
				{
					if (string.Compare(item.Name, name, true) == 0)
						return item;
				}
				return null;
			}
		}
	}
}
