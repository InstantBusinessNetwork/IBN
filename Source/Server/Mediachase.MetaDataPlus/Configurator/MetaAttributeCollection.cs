using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Mediachase.MetaDataPlus.Common;
using System.Globalization;

namespace Mediachase.MetaDataPlus.Configurator
{
	public enum	MetaAttributeOwnerType
	{
		NotSet		=	0,
		MetaClass	=	1,
		MetaField	=	2
	}

	/// <summary>
	/// Summary description for MetaAttributeCollection.
	/// </summary>
	public class MetaAttributeCollection: IEnumerable
	{
		private	int							_OwnerId				=	0;
		private MetaAttributeOwnerType		_OwnerType = MetaAttributeOwnerType.NotSet;

		private Hashtable contents	=	new Hashtable();

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaAttributeCollection"/> class.
		/// </summary>
		/// <param name="OwnerId">The owner id.</param>
		/// <param name="OwnerType">Type of the owner.</param>
		/// <param name="reader">The reader.</param>
		internal MetaAttributeCollection(int OwnerId, MetaAttributeOwnerType OwnerType, SqlDataReader reader)
		{
			_OwnerId = OwnerId;
			_OwnerType = OwnerType;
			
			while(reader.Read())
			{
				this.contents.Add((string)reader["Key"], (string)reader["Value"]);
			}
		}

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that can iterate through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/>
		/// that can be used to iterate through the collection.
		/// </returns>
		public virtual IEnumerator GetEnumerator()
		{
			return this.contents.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public virtual void Add(string key, string value)
		{
			if(key==null)
				throw new ArgumentNullException("key");

			if(value==null)
				throw new ArgumentNullException("value");

			key = key.ToLower(CultureInfo.InvariantCulture);

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current,CommandType.StoredProcedure,AsyncResources.GetConstantValue("SP_AddMetaAttribute"),
				new SqlParameter("@AttrOwnerId", this._OwnerId), 
				new SqlParameter("@AttrOwnerType", (int)this._OwnerType),
				new SqlParameter("@Key", key),
				new SqlParameter("@Value", value));

			this.contents.Add(key, value);
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public virtual void Clear()
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current,CommandType.StoredProcedure,AsyncResources.GetConstantValue("SP_DeleteMetaAttribute"),
				new SqlParameter("@AttrOwnerId", _OwnerId), 
				new SqlParameter("@AttrOwnerType", (int)_OwnerType));

			this.contents.Clear();
		}

		/// <summary>
		/// Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool ContainsKey(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			return this.contents.ContainsKey(key.ToLower(CultureInfo.InvariantCulture));
		}
 
		/// <summary>
		/// Determines whether the specified value contains value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value contains value; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool ContainsValue(string value)
		{
			return this.contents.ContainsValue(value);
		}
 
		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="index">The index.</param>
		public virtual void CopyTo(Array array, int index)
		{
			this.contents.CopyTo(array, index);
		}

		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		public virtual void Remove(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			key = key.ToLower(CultureInfo.InvariantCulture);

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current,CommandType.StoredProcedure,AsyncResources.GetConstantValue("SP_DeleteMetaAttribute"),
				new SqlParameter("@AttrOwnerId", _OwnerId), 
				new SqlParameter("@AttrOwnerType", (int)_OwnerType),
				new SqlParameter("@Key", key));

			this.contents.Remove(key);
		}

		/// <summary>
		/// Gets or sets the <see cref="String"/> with the specified key.
		/// </summary>
		/// <value></value>
		public virtual string this[string key]
		{
			get
			{
				if(key==null)
					throw new ArgumentNullException("key");

				return (string) this.contents[key.ToLower(CultureInfo.InvariantCulture)];
			}
			set
			{
				if(key==null)
					throw new ArgumentNullException("key");

				if(value==null)
					throw new ArgumentNullException("value");

				key = key.ToLower(CultureInfo.InvariantCulture);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current,CommandType.StoredProcedure,AsyncResources.GetConstantValue("SP_AddMetaAttribute"),
					new SqlParameter("@AttrOwnerId", _OwnerId), 
					new SqlParameter("@AttrOwnerType", (int)_OwnerType),
					new SqlParameter("@Key", key),
					new SqlParameter("@Value", value));

				this.contents[key] = value;
			}
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public virtual int Count
		{
			get
			{
				return this.contents.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is synchronized.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is synchronized; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsSynchronized
		{
			get
			{
				return this.contents.IsSynchronized;
			}
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		public virtual ICollection Keys
		{
			get
			{
				return this.contents.Keys;
			}
		}

		/// <summary>
		/// Gets the sync root.
		/// </summary>
		/// <value>The sync root.</value>
		public virtual object SyncRoot
		{
			get
			{
				return this.contents.SyncRoot;
			}
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public virtual ICollection Values
		{
			get
			{
				return this.contents.Values;
			}
		}
	}
}
