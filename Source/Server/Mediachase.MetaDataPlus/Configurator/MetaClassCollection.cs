using System;
using System.Collections;
using System.Data;

namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaClassCollection.
	/// </summary>
	public class MetaClassCollection : ReadOnlyCollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetaClassCollection"/> class.
		/// </summary>
		internal MetaClassCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaClassCollection"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public MetaClassCollection(IDataReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			while (reader.Read())
			{
				this.Add(MetaClass.Load(reader));
			}
		}

		/// <summary>
		/// Adds the specified new item.
		/// </summary>
		/// <param name="newItem">The new item.</param>
		internal void Add(MetaClass newItem)
		{
			this.InnerList.Add(newItem);
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
		/// Gets the <see cref="MetaClass"/> at the specified index.
		/// </summary>
		/// <value></value>
		public MetaClass this[int index]
		{
			get
			{
				return (MetaClass)this.InnerList[index];
			}
		}

		/// <summary>
		/// Gets the <see cref="MetaClass"/> with the specified name.
		/// </summary>
		/// <value></value>
		public MetaClass this[string name]
		{
			get
			{
				foreach (MetaClass item in this)
				{
					if (string.Compare(item.Name, name, true) == 0)
						return item;
				}
				return null;
			}
		}
	}
}
