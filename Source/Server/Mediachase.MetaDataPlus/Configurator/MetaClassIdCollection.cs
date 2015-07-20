using System;
using System.Collections;

namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaClassIdCollection.
	/// </summary>
	public class MetaClassIdCollection: ReadOnlyCollectionBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetaClassIdCollection"/> class.
		/// </summary>
		public MetaClassIdCollection()
		{
		}

		/// <summary>
		/// Adds the specified new item.
		/// </summary>
		/// <param name="newItem">The new item.</param>
		internal void Add(int newItem)
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
		/// Gets the <see cref="Int32"/> at the specified index.
		/// </summary>
		/// <value></value>
		public int this[int index]
		{
			get
			{
				return (int)this.InnerList[index];
			}
		}
	}
}
