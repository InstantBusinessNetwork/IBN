using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Represents a MetaDictionary.
	/// </summary>
	public class MetaDictionary : ReadOnlyCollectionBase
	{
		private int _ownerMetaFieldId;

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaDictionary"/> class.
		/// </summary>
		protected MetaDictionary()
		{
		}

		/// <summary>
		/// Reloads the meta dictionary.
		/// </summary>
		void ReloadMetaDictionary()
		{
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaDictionary"),
					  new SqlParameter("@MetaFieldId", this.OwnerMetaFieldId)))
			{
				while (reader.Read())
				{
					MetaDictionaryItem newItem = new MetaDictionaryItem(reader);
					this.InnerList.Add(newItem);
				}
				reader.Close();
			}

			// ReIndex
			ReindexItems();
		}

		#region Statics Methods
		/// <summary>
		/// Loads the specified meta field id.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <returns></returns>
		public static MetaDictionary Load(int metaFieldId)
		{
			MetaDictionary retVal = new MetaDictionary();

			retVal._ownerMetaFieldId = metaFieldId;

			retVal.ReloadMetaDictionary();

			return retVal;
		}

		/// <summary>
		/// Loads the specified field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public static MetaDictionary Load(MetaField field)
		{
			if (field == null)
				throw new ArgumentNullException("field");

			return MetaDictionary.Load(field.Id);
		}

		/// <summary>
		/// Determines whether [contains] [the specified meta dictionary item id].
		/// </summary>
		/// <param name="MetaDictionaryItemId">The meta dictionary item id.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified meta dictionary item id]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(int metaDictionaryItemId)
		{
			return GetItem(metaDictionaryItemId) != null;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <param name="MetaDictionaryItemId">The meta dictionary item id.</param>
		/// <returns></returns>
		public MetaDictionaryItem GetItem(int metaDictionaryItemId)
		{
			foreach (MetaDictionaryItem item in this)
			{
				if (item.Id == metaDictionaryItemId)
					return item;
			}
			return null;
		}

		/// <summary>
		/// Determines whether [contains] [the specified value].
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
		/// </returns>
		internal bool Contains(string defaultValue)
		{
			return GetItem(defaultValue) != null;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <returns></returns>
		internal MetaDictionaryItem GetItem(string defaultValue)
		{
			foreach (MetaDictionaryItem item in this)
			{
				if (item.DefaultValue == defaultValue)
					return item;
			}
			return null;
		}

		#endregion

		#region Common Information
		/// <summary>
		/// Gets the owner meta field id.
		/// </summary>
		/// <value>The owner meta field id.</value>
		public int OwnerMetaFieldId
		{
			get
			{
				return _ownerMetaFieldId;
			}
		}

		/// <summary>
		/// Gets the <see cref="MetaDictionaryItem"/> at the specified index.
		/// </summary>
		/// <value></value>
		public MetaDictionaryItem this[int index]
		{
			get
			{
				return (MetaDictionaryItem)this.InnerList[index];
			}
		}
		#endregion

		#region Add
		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public int Add(string value)
		{
			return this.Add(value, value);
		}

		/// <summary>
		/// Adds the specified default value.
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		internal int Add(string defaultValue, string value)
		{
			return this.Add(defaultValue, value, null, null);
		}

		/// <summary>
		/// Adds the specified default value.
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Value">The value.</param>
		/// <param name="DefaultTag">The default tag.</param>
		/// <param name="Tag">The tag.</param>
		/// <returns></returns>
		internal int Add(string defaultValue, string value, object defaultTag, object tag)
		{
			SqlParameter spDefaultTag = new SqlParameter("@DefaultTag", SqlDbType.Image);
			spDefaultTag.Value = SqlHelper.Null2DBNull(defaultTag);

			SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
			spTag.Value = SqlHelper.Null2DBNull(tag);

			SqlParameter Retval = new SqlParameter("@Retval", SqlDbType.Int, 4);
			Retval.Direction = ParameterDirection.Output;

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_AddMetaDictionary"),
				new SqlParameter("@MetaFieldId", this.OwnerMetaFieldId),
				//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
				//new SqlParameter("@DefaultValue", value),
				//spDefaultTag,
				new SqlParameter("@Value", value),
				spTag,
				new SqlParameter("@Index", this.Count),
				Retval
				);

			MetaDictionaryItem retVal = new MetaDictionaryItem((int)Retval.Value, this.OwnerMetaFieldId, defaultValue, value, this.Count);

			return this.InnerList.Add(retVal);
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public int Add(MetaDictionaryItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			return this.Add(item.DefaultValue, item.Value, item.DefaultTag, item.Tag);
		}

		#endregion

		#region Inset
		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		public void Insert(int index, string value)
		{
			this.Insert(index, value, value);
		}

		/// <summary>
		/// Adds the specified default value.
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		internal void Insert(int index, string defaultValue, string value)
		{
			this.Insert(index, defaultValue, value, null, null);
		}

		/// <summary>
		/// Adds the specified default value.
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Value">The value.</param>
		/// <param name="DefaultTag">The default tag.</param>
		/// <param name="Tag">The tag.</param>
		/// <returns></returns>
		internal void Insert(int index, string defaultValue, string value, object defaultTag, object tag)
		{
			if (index < 0 && index >= this.Count)
				throw new ArgumentOutOfRangeException("index");

			SqlParameter spDefaultTag = new SqlParameter("@DefaultTag", SqlDbType.Image);
			spDefaultTag.Value = SqlHelper.Null2DBNull(defaultTag);

			SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
			spTag.Value = SqlHelper.Null2DBNull(tag);

			SqlParameter Retval = new SqlParameter("@Retval", SqlDbType.Int, 4);
			Retval.Direction = ParameterDirection.Output;

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_AddMetaDictionary"),
				new SqlParameter("@MetaFieldId", this.OwnerMetaFieldId),
				//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
				//new SqlParameter("@DefaultValue", value),
				//spDefaultTag,
				new SqlParameter("@Value", value),
				spTag,
				new SqlParameter("@Index", index),
				Retval
				);

			MetaDictionaryItem retVal = new MetaDictionaryItem((int)Retval.Value, this.OwnerMetaFieldId, defaultValue, value, index);

			this.InnerList.Insert(index, retVal);

			ReindexItems();
		}

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public void Insert(int index, MetaDictionaryItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			this.Insert(index, item.DefaultValue, item.Value, item.DefaultTag, item.Tag);
		}

		#endregion

		#region Delete
		/// <summary>
		/// Deletes at.
		/// </summary>
		/// <param name="Index">The index.</param>
		public void DeleteAt(int index)
		{
			MetaDictionaryItem item = this[index];

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaDictionary"),
				new SqlParameter("@MetaDictionaryId", item.Id)
				);

			this.InnerList.RemoveAt(index);

			// Decrease Index
			ReindexItems();
		}

		/// <summary>
		/// Deletes the specified meta dictionary id.
		/// </summary>
		/// <param name="MetaDictionaryId">The meta dictionary id.</param>
		public void Delete(int metaDictionaryId)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaDictionary"),
									   new SqlParameter("@MetaDictionaryId", metaDictionaryId)
								   );
			foreach (MetaDictionaryItem item in this)
			{
				if (item.Id == metaDictionaryId)
				{
					this.InnerList.Remove(item);
					break;
				}
			}

			ReindexItems();
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(MetaDictionaryItem item)
		{
			if (this.InnerList.Contains(item))
			{
				this.DeleteAt(this.InnerList.IndexOf(item));
				return;
			}

			throw new ArgumentException("Could noy find MetaDictionaryItem.", "item");
		}
		#endregion

		#region Update
		/// <summary>
		/// Updates the specified meta dictionary item id.
		/// </summary>
		/// <param name="MetaDictionaryItemId">The meta dictionary item id.</param>
		/// <param name="Value">The value.</param>
		public void Update(int metaDictionaryItemId, string value)
		{
			foreach (MetaDictionaryItem item in this)
			{
				if (item.Id == metaDictionaryItemId)
				{
					item.Value = value;
					return;
				}
			}
			throw new ArgumentOutOfRangeException("MetaDictionaryItemId", metaDictionaryItemId, "Could not find a MetaDictionaryItem.");
		}

		public void Update(int metaDictionaryItemId, int newIndex, string value)
		{
			foreach (MetaDictionaryItem item in this)
			{
				if (item.Id == metaDictionaryItemId)
				{
					item.Value = value;
					if (item.Index != newIndex)
					{
						Move(item.Index, newIndex);
					}
					return;
				}
			}

			throw new ArgumentOutOfRangeException("MetaDictionaryItemId", metaDictionaryItemId, "Could not find a MetaDictionaryItem.");
		}
		#endregion

		#region SortByValue
		protected class SortValueAscending : IComparer
		{
			int IComparer.Compare(Object x, Object y)
			{
				MetaDictionaryItem itemX = x as MetaDictionaryItem;
				MetaDictionaryItem itemY = y as MetaDictionaryItem;

				return string.Compare(itemX.Value, itemY.Value, true);
			}
		}

		protected class SortValueDescending : IComparer
		{
			int IComparer.Compare(Object x, Object y)
			{
				MetaDictionaryItem itemX = x as MetaDictionaryItem;
				MetaDictionaryItem itemY = y as MetaDictionaryItem;

				return string.Compare(itemY.Value, itemX.Value, true);
			}
		}

		public void SortByValue(bool ascending)
		{
			// Step 2. Create comparer
			IComparer comparer = ascending ? (IComparer)new SortValueAscending() : (IComparer)new SortValueDescending();

			// Step 2. Sort
			this.InnerList.Sort(comparer);

			// Step 3. Reindex MetaDictionaryItem Index
			ReindexItems();
		}

		private void ReindexItems()
		{
			for (int index = 0; index < this.Count; index++)
			{
				this[index].Index = index;
			}
		}
		#endregion

		#region Move
		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex < 0 && oldIndex >= this.Count)
				throw new ArgumentOutOfRangeException("oldIndex");
			if (newIndex < 0 && newIndex >= this.Count)
				throw new ArgumentOutOfRangeException("newIndex");

			MetaDictionaryItem item = this[oldIndex];

			this.InnerList.RemoveAt(oldIndex);

			if (newIndex == this.Count)
				this.InnerList.Add(item);
			else
				this.InnerList.Insert(newIndex, item);

			ReindexItems();

		}
		#endregion
	}
}
