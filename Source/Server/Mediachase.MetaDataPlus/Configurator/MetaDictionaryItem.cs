using System;
using System.Data;
using System.Data.SqlClient;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Configurator
{
	public enum MetaDictionaryItemState
	{
		Detached = 0,
		Attached = 1
	}

	/// <summary>
	/// Represents a MetaDictionary Item.
	/// </summary>
	public class MetaDictionaryItem
	{
		private int _id;
		private int _metaFieldId;

		private string _defaultValue = string.Empty;
		private string _value = string.Empty;

		private object _defaultTag;
		private object _tag;

		// OZ 2007-11-07 Addon
		private int _index;

		private MetaDictionaryItemState _state = MetaDictionaryItemState.Detached;

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaDictionaryItem"/> class.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <param name="Value">The value.</param>
		internal MetaDictionaryItem(int id, int metaFieldId, string value)
		{
			_state = MetaDictionaryItemState.Attached;
			_id = id;
			_metaFieldId = metaFieldId;
			_defaultValue = value;
			_value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaDictionaryItem"/> class.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Value">The value.</param>
		internal MetaDictionaryItem(int id, int metaFieldId, string defaultValue, string value)
		{
			_state = MetaDictionaryItemState.Attached;
			_id = id;
			_metaFieldId = metaFieldId;
			_defaultValue = defaultValue;
			_value = value;
		}

		internal MetaDictionaryItem(int id, int metaFieldId, string defaultValue, string value, int index)
		{
			_state = MetaDictionaryItemState.Attached;
			_id = id;
			_metaFieldId = metaFieldId;
			_defaultValue = defaultValue;
			_value = value;
			_index = index;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaDictionaryItem"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		internal MetaDictionaryItem(SqlDataReader reader)
		{
			_state = MetaDictionaryItemState.Attached;

			_id = (int)reader["MetaDictionaryId"];
			_metaFieldId = (int)reader["MetaFieldId"];

			//_defaultValue = (string)reader["DefaultValue"];
			_value = (string)SqlHelper.DBNull2Null(reader["Value"], string.Empty);

			//_defaultTag = SqlHelper.DBNull2Null(reader["DefaultTag"]);
			_tag = SqlHelper.DBNull2Null(reader["Tag"]);

			_index = (int)reader["Index"];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaDictionaryItem"/> class.
		/// </summary>
		/// <param name="Value">The value.</param>
		public MetaDictionaryItem(string value)
		{
			_defaultValue = value;
			_value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetaDictionaryItem"/> class.
		/// </summary>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Value">The value.</param>
		public MetaDictionaryItem(string defaultValue, string value)
		{
			_defaultValue = defaultValue;
			_value = value;
		}

		/// <summary>
		/// Gets the state.
		/// </summary>
		/// <value>The state.</value>
		public MetaDictionaryItemState State
		{
			get
			{
				return _state;
			}
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public int Id
		{
			get
			{
				return _id;
			}
		}

		/// <summary>
		/// Gets the owner meta field id.
		/// </summary>
		/// <value>The owner meta field id.</value>
		public int OwnerMetaFieldId
		{
			get
			{
				return _metaFieldId;
			}
		}

		public int Index
		{
			get { return _index; }
			internal 
			set 
			{
				if (_index == value)
					return;

				if (this.State == MetaDictionaryItemState.Attached)
				{
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, 
						AsyncResources.GetConstantValue("SP_UpdateMetaDictionaryIndex"),
						new SqlParameter("@MetaDictionaryId", this.Id),
						//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
						new SqlParameter("@Index", value)
						);
				}

				_index = value; 
			}
		}
	

		/// <summary>
		/// Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		internal string DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				if (this.State == MetaDictionaryItemState.Attached)
				{
					SqlParameter spDefaultTag = new SqlParameter("@DefaultTag", SqlDbType.Image);
					spDefaultTag.Value = SqlHelper.Null2DBNull(this.DefaultTag);

					SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
					spTag.Value = SqlHelper.Null2DBNull(this.Tag);

					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, 
						AsyncResources.GetConstantValue("SP_UpdateMetaDictionary"),
						new SqlParameter("@MetaDictionaryId", this.Id),
						//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
						//new SqlParameter("@DefaultValue", value),
						//spDefaultTag,
						new SqlParameter("@Value", this.Value),
						spTag
						);
				}

				_defaultValue = value;
			}
		}

		public object DefaultTag
		{
			get
			{
				return _defaultTag;
			}
			set
			{
				if (value != null && value.GetType() != typeof(byte[]))
					throw new ArgumentException("Only byte[] or Null are supported now.");

				if (this.State == MetaDictionaryItemState.Attached)
				{
					SqlParameter spDefaultTag = new SqlParameter("@DefaultTag", SqlDbType.Image);
					spDefaultTag.Value = SqlHelper.Null2DBNull(value);

					SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
					spTag.Value = SqlHelper.Null2DBNull(this.Tag);

					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaDictionary"),
						new SqlParameter("@MetaDictionaryId", this.Id),
						//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
						//new SqlParameter("@DefaultValue", value),
						//spDefaultTag,
						new SqlParameter("@Value", this.Value),
						spTag
						);
				}

				_defaultTag = value;
			}
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (this.State == MetaDictionaryItemState.Attached)
				{
					SqlParameter spDefaultTag = new SqlParameter("@DefaultTag", SqlDbType.Image);
					spDefaultTag.Value = SqlHelper.Null2DBNull(this.DefaultTag);

					SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
					spTag.Value = SqlHelper.Null2DBNull(this.Tag);

					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaDictionary"),
											   new SqlParameter("@MetaDictionaryId", this.Id),
												//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
												//new SqlParameter("@DefaultValue", this.DefaultValue),
												//spDefaultTag,
												new SqlParameter("@Value", value),
												spTag
										   );
				}

				_value = value;
			}
		}

		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				if (value != null && value.GetType() != typeof(byte[]))
					throw new ArgumentException("Only byte[] or Null are supported now.");

				if (this.State == MetaDictionaryItemState.Attached)
				{
					SqlParameter spDefaultTag = new SqlParameter("@DefaultTag", SqlDbType.Image);
					spDefaultTag.Value = SqlHelper.Null2DBNull(this.DefaultValue);

					SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
					spTag.Value = SqlHelper.Null2DBNull(value);

					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaDictionary"),
						new SqlParameter("@MetaDictionaryId", this.Id),
						//new SqlParameter("@Language", SqlHelper.Null2DBNull(MetaDataContext.Current.Language)),
						//new SqlParameter("@DefaultValue", value),
						//spDefaultTag,
						new SqlParameter("@Value", this.Value),
						spTag
						);
				}

				_tag = value;
			}
		}
	}
}
