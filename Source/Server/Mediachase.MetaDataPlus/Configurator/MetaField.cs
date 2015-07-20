using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaField.
	/// </summary>
	public class MetaField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetaField"/> class.
		/// </summary>
		protected MetaField()
		{
		}

		private int _id;
		private string _namespace = string.Empty;
		private string _name = string.Empty;
		private string _friendlyName = string.Empty;
		private string _description = string.Empty;
		private MetaDataType _dataType = MetaDataType.BigInt;
		private int _length;
		private bool _allowNulls;
		private bool _saveHistory;
		private bool _multilanguageValue;
		private bool _allowSearch;

		private MetaClassIdCollection _ownerMetaClasses;
		private int _systemMetaClassId;

		private MetaDictionary _dictionary;

		private MetaClass _ownerMetaClass;
		private object _tag;

		private MetaAttributeCollection _attributes;

		#region Static Methods
		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="Description">The description.</param>
		/// <param name="DataType">Type of the data.</param>
		/// <returns></returns>
		public static MetaField Create(
			string metaNamespace,
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType)
		{
			return MetaField.Create(metaNamespace, name, friendlyName, description, dataType, 0);
		}

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="Description">The description.</param>
		/// <param name="DataType">Type of the data.</param>
		/// <param name="Length">The length.</param>
		/// <returns></returns>
		public static MetaField Create(
			string metaNamespace,
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType,
			int length)
		{
			return MetaField.Create(metaNamespace, name, friendlyName, description, dataType, length,
				true, false, false, false);
		}

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="Description">The description.</param>
		/// <param name="DataType">Type of the data.</param>
		/// <param name="AllowNulls">if set to <c>true</c> [allow nulls].</param>
		/// <param name="SaveHistory">if set to <c>true</c> [save history].</param>
		/// <param name="AllowSearch">if set to <c>true</c> [allow search].</param>
		/// <returns></returns>
		public static MetaField Create(
			string metaNamespace,
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType,
			bool allowNulls,
			bool saveHistory,
			bool allowSearch)
		{
			return MetaField.Create(metaNamespace, name, friendlyName, description, dataType, 0,
				allowNulls, saveHistory, false, allowSearch);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="Description">The description.</param>
		/// <param name="DataType">Type of the data.</param>
		/// <returns></returns>
		public static MetaField Create(
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType)
		{
			return MetaField.Create(MetaNamespace.UserRoot, name, friendlyName, description, dataType, 0);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="Description">The description.</param>
		/// <param name="DataType">Type of the data.</param>
		/// <param name="Length">The length.</param>
		/// <returns></returns>
		public static MetaField Create(
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType,
			int length)
		{
			return MetaField.Create(MetaNamespace.UserRoot, name, friendlyName, description, dataType, length,
				true, false, false, false);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="Description">The description.</param>
		/// <param name="DataType">Type of the data.</param>
		/// <param name="AllowNulls">if set to <c>true</c> [allow nulls].</param>
		/// <param name="SaveHistory">if set to <c>true</c> [save history].</param>
		/// <param name="AllowSearch">if set to <c>true</c> [allow search].</param>
		/// <returns></returns>
		public static MetaField Create(string name,
			string friendlyName,
			string description,
			MetaDataType dataType,
			bool allowNulls,
			bool saveHistory,
			bool allowSearch)
		{
			return MetaField.Create(MetaNamespace.UserRoot, name, friendlyName, description, dataType, 0,
				allowNulls, saveHistory, false, allowSearch);
		}

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="metaNamespace">The namespace.</param>
		/// <param name="name">The name.</param>
		/// <param name="friendlyName">Name of the friendly.</param>
		/// <param name="description">The description.</param>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="length">The length.</param>
		/// <param name="allowNulls">if set to <c>true</c> [allow nulls].</param>
		/// <param name="saveHistory">if set to <c>true</c> [save history].</param>
		/// <param name="multilanguageValue">if set to <c>true</c> [multilanguage value].</param>
		/// <param name="allowSearch">if set to <c>true</c> [allow search].</param>
		/// <returns></returns>
		public static MetaField Create(
			string metaNamespace,
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType,
			int length,
			bool allowNulls,
			bool saveHistory,
			bool multilanguageValue,
			bool allowSearch)
		{
			#region ArgumentNullExceptions
			if (metaNamespace == null)
				throw new ArgumentNullException("Namespace", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (name == null)
				throw new ArgumentNullException("Name", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (friendlyName == null)
				throw new ArgumentNullException("FriendlyName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SqlParameter Retval = new SqlParameter("@Retval", SqlDbType.Int, 4);
			Retval.Direction = ParameterDirection.Output;

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_AddMetaField"),
										new SqlParameter("@Namespace", metaNamespace),
									   new SqlParameter("@Name", name),
									   new SqlParameter("@FriendlyName", friendlyName),
									   new SqlParameter("@Description", description),
									   new SqlParameter("@DataTypeId", (int)dataType),
									   new SqlParameter("@Length", length),
									   new SqlParameter("@AllowNulls", allowNulls),
									   new SqlParameter("@SaveHistory", saveHistory),
				//new SqlParameter("@MultiLanguageValue", multilanguageValue),
									   new SqlParameter("@AllowSearch", allowSearch),
									   Retval
								  );

			return MetaField.Load((int)Retval.Value);
		}

		/// <summary>
		/// Creates the virtual.
		/// </summary>
		/// <param name="metaNamespace">The namespace.</param>
		/// <param name="name">The name.</param>
		/// <param name="friendlyName">Name of the friendly.</param>
		/// <param name="description">The description.</param>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="length">The length.</param>
		/// <param name="allowNulls">if set to <c>true</c> [allow nulls].</param>
		/// <param name="saveHistory">if set to <c>true</c> [save history].</param>
		/// <param name="multilanguageValue">if set to <c>true</c> [multi language value].</param>
		/// <param name="allowSearch">if set to <c>true</c> [allow search].</param>
		/// <returns></returns>
		public static MetaField CreateVirtual(
			string metaNamespace,
			string name,
			string friendlyName,
			string description,
			MetaDataType dataType,
			int length,
			bool allowNulls,
			bool saveHistory,
			bool multilanguageValue,
			bool allowSearch)
		{
			#region ArgumentNullExceptions
			if (metaNamespace == null)
				throw new ArgumentNullException("Namespace", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (name == null)
				throw new ArgumentNullException("Name", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (friendlyName == null)
				throw new ArgumentNullException("FriendlyName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaField retVal = new MetaField();

			// Load MetaField Information [11/18/2004]
			retVal._id = -1;
			retVal._namespace = metaNamespace;
			retVal._name = name;
			retVal._friendlyName = friendlyName;

			retVal._description = description;

			retVal._dataType = dataType;
			retVal._length = length;

			retVal._allowNulls = allowNulls;
			retVal._saveHistory = saveHistory;
			retVal._multilanguageValue = multilanguageValue;
			retVal._allowSearch = allowSearch;

			retVal._systemMetaClassId = -1;

			return retVal;
		}


		/// <summary>
		/// Gets the data reader.
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetDataReader()
		{
			return SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldList"));
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <returns></returns>
		public static MetaFieldCollection GetList()
		{
			MetaFieldCollection retVal = new MetaFieldCollection();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldList")))
			{
				while (reader.Read())
				{
					MetaField newItem = MetaField.Load(reader);
					retVal.Add(newItem);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Gets the data reader.
		/// </summary>
		/// <param name="MetaClassId">The meta class id.</param>
		/// <returns></returns>
		public static IDataReader GetDataReader(int metaClassId)
		{
			return (IDataReader)SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldListByMetaClassId"), new SqlParameter("@MetaClassId", metaClassId));
		}

		/// <summary>
		/// Gets the data reader.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		public static IDataReader GetDataReader(MetaClass metaClass)
		{
			#region ArgumentNullExceptions
			if (metaClass == null)
				throw new ArgumentNullException("metaClass", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return GetDataReader(metaClass.Id);
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="MetaClassId">The meta class id.</param>
		/// <returns></returns>
		public static MetaFieldCollection GetList(int metaClassId)
		{
			return GetList(MetaClass.Load(metaClassId));
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Deep">if set to <c>true</c> [deep].</param>
		/// <returns></returns>
		public static MetaFieldCollection GetList(string metaNamespace, bool deep)
		{
			#region ArgumentNullExceptions
			if (metaNamespace == null)
				throw new ArgumentNullException("Namespace", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaFieldCollection retVal = new MetaFieldCollection();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldByNamespace"),
					  new SqlParameter("@Namespace", metaNamespace), new SqlParameter("@Deep", deep)))
			{
				while (reader.Read())
				{
					retVal.Add(MetaField.Load(reader));
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="NamespaceItems">The namespace items.</param>
		/// <returns></returns>
		public static MetaFieldCollection GetList(params string[] namespaceItems)
		{
			#region ArgumentNullExceptions
			if (namespaceItems == null)
				throw new ArgumentNullException("NamespaceItems", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaFieldCollection retVal = new MetaFieldCollection();

			foreach (string Namespace in namespaceItems)
			{
				using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldByNamespace"),
						  new SqlParameter("@Namespace", Namespace), new SqlParameter("@Deep", false)))
				{
					while (reader.Read())
					{
						retVal.Add(MetaField.Load(reader));
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		internal static MetaFieldCollection GetList(MetaClass metaClass)
		{
			#region ArgumentNullExceptions
			if (metaClass == null)
				throw new ArgumentNullException("metaClass", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaFieldCollection retVal = new MetaFieldCollection();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldListByMetaClassId"),
					  new SqlParameter("@MetaClassId", metaClass.Id)))
			{
				while (reader.Read())
				{
					MetaField newItem = MetaField.Load(metaClass, reader);
					newItem.SetOwnerMetaClass(metaClass);

					retVal.Add(newItem);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Loads the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public static MetaField Load(string name)
		{
			#region ArgumentNullExceptions
			if (name == null)
				throw new ArgumentNullException("Name", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaField retVal = null;
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldByName"),
					  new SqlParameter("@Name", name)))
			{
				if (reader.Read())
					retVal = MetaField.Load(reader);
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Loads the specified meta field id.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <returns></returns>
		public static MetaField Load(int metaFieldId)
		{
			MetaField retVal = null;
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaField"),
					  new SqlParameter("@MetaFieldId", metaFieldId)))
			{
				if (reader.Read())
					retVal = MetaField.Load(reader);
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Loads the specified reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public static MetaField Load(SqlDataReader reader)
		{
			#region ArgumentNullExceptions
			if (reader == null)
				throw new ArgumentNullException("reader", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return MetaField.Load(null, reader);
		}

		/// <summary>
		/// Loads the specified meta class.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		protected static MetaField Load(MetaClass metaClass, SqlDataReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			MetaField retVal = new MetaField();

			// Load MetaField Information [11/18/2004]
			retVal._id = (int)reader["MetaFieldId"];
			retVal._namespace = (string)reader["Namespace"];
			retVal._name = (string)reader["Name"];
			retVal._friendlyName = (string)reader["FriendlyName"];

			if (reader["Description"] != DBNull.Value)
				retVal._description = (string)reader["Description"];

			retVal._dataType = (MetaDataType)reader["DataTypeId"];
			retVal._length = (int)reader["Length"];

			retVal._allowNulls = (bool)reader["AllowNulls"];
			retVal._saveHistory = (bool)reader["SaveHistory"];
			//retVal._multilanguageValue = (bool)reader["MultiLanguageValue"];
			retVal._allowSearch = (bool)reader["AllowSearch"];

			retVal._systemMetaClassId = (int)reader["SystemMetaClassId"];

			retVal._tag = SqlHelper.DBNull2Null(reader["Tag"]);

			return retVal;
		}

		/// <summary>
		/// Deletes the specified meta field id.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		public static void Delete(int metaFieldId)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaField"),
									   new SqlParameter("@MetaFieldId", metaFieldId)
									);
		}

		/// <summary>
		/// Gets the name of the unique.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <returns></returns>
		public static string GetUniqueName(string name)
		{
			SqlParameter UniqueName = new SqlParameter("@UniqueName", SqlDbType.NVarChar, 256);
			UniqueName.Direction = ParameterDirection.Output;

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_GetUniqueFieldName"),
				new SqlParameter("@Name", name),
				UniqueName
				);

			return (string)UniqueName.Value;
		}
		#endregion

		#region Common Information

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
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		public MetaDataContext Context
		{
			get
			{
				return MetaDataContext.Current;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is system.
		/// </summary>
		/// <value><c>true</c> if this instance is system; otherwise, <c>false</c>.</value>
		public bool IsSystem
		{
			get
			{
				return _systemMetaClassId != 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is user.
		/// </summary>
		/// <value><c>true</c> if this instance is user; otherwise, <c>false</c>.</value>
		public bool IsUser
		{
			get
			{
				return !this.IsSystem;
			}
		}

		/// <summary>
		/// Checks the owner meta class.
		/// </summary>
		protected void CheckOwnerMetaClass()
		{
			if (this.OwnerMetaClass == null)
				throw new NotSupportedException("The OwnerMetaClass is Unknown.");
		}

		/// <summary>
		/// Sets the owner meta class.
		/// </summary>
		/// <param name="OwnerMetaClass">The owner meta class.</param>
		internal void SetOwnerMetaClass(MetaClass OwnerMetaClass)
		{
			_ownerMetaClass = OwnerMetaClass;
		}

		/// <summary>
		/// Gets the owner meta class.
		/// </summary>
		/// <value>The owner meta class.</value>
		public MetaClass OwnerMetaClass
		{
			get
			{
				return _ownerMetaClass;
			}
		}

		/// <summary>
		/// Gets the owner meta class id list.
		/// </summary>
		/// <value>The owner meta class id list.</value>
		public MetaClassIdCollection OwnerMetaClassIdList
		{
			get
			{
				if (_ownerMetaClasses == null)
					_ownerMetaClasses = MetaClass.GetOwnerMetaClasses(Id);

				return _ownerMetaClasses;
			}
		}

		/// <summary>
		/// Gets or sets the namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				#region ArgumentNullExceptions
				if (value == null)
					throw new ArgumentNullException("value", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
				#endregion

				SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
				spTag.Value = SqlHelper.Null2DBNull(this.Tag);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaField"),
					new SqlParameter("@MetaFieldId", this.Id),
					new SqlParameter("@Namespace", value),
					new SqlParameter("@FriendlyName", this.FriendlyName),
					new SqlParameter("@Description", this.Description),
					spTag
					);

				_namespace = value;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets or sets the name of the friendly.
		/// </summary>
		/// <value>The name of the friendly.</value>
		public string FriendlyName
		{
			get
			{
				return _friendlyName;
			}
			set
			{
				#region ArgumentNullExceptions
				if (value == null)
					throw new ArgumentNullException("value", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
				#endregion

				SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
				spTag.Value = SqlHelper.Null2DBNull(this.Tag);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaField"),
										   new SqlParameter("@MetaFieldId", this.Id),
											new SqlParameter("@Namespace", this.Namespace),
										   new SqlParameter("@FriendlyName", value),
										   new SqlParameter("@Description", this.Description),
											spTag
									   );

				_friendlyName = value;
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
				spTag.Value = SqlHelper.Null2DBNull(this.Tag);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaField"),
										   new SqlParameter("@MetaFieldId", this.Id),
											new SqlParameter("@Namespace", this.Namespace),
										   new SqlParameter("@FriendlyName", this.FriendlyName),
										   new SqlParameter("@Description", SqlHelper.Null2DBNull(value)),
											spTag
									   );

				_description = value;
			}
		}

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (value.GetType() != typeof(byte[]))
					throw new ArgumentException("Only byte[] are supported now.");

				SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
				spTag.Value = SqlHelper.Null2DBNull(value);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaField"),
					new SqlParameter("@MetaFieldId", this.Id),
					new SqlParameter("@Namespace", this.Namespace),
					new SqlParameter("@FriendlyName", this.FriendlyName),
					new SqlParameter("@Description", this.Description),
					spTag
					);

				_tag = value;
			}
		}

		//  [1/14/2005]
		/// <summary>
		/// Gets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public MetaAttributeCollection Attributes
		{
			get
			{
				if (_attributes == null)
				{
					using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaAttributes"),
							  new SqlParameter("@AttrOwnerId", this.Id),
							  new SqlParameter("@AttrOwnerType", (int)MetaAttributeOwnerType.MetaClass)))
					{
						_attributes = new MetaAttributeCollection(this.Id, MetaAttributeOwnerType.MetaClass, reader);
					}
				}

				return _attributes;
			}
		}

		#endregion

		#region Data Description

		/// <summary>
		/// Gets the type of the data.
		/// </summary>
		/// <value>The type of the data.</value>
		public MetaDataType DataType
		{
			get
			{
				return _dataType;
			}
		}

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <value>The length.</value>
		public int Length
		{
			get
			{
				return _length;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [allow nulls].
		/// </summary>
		/// <value><c>true</c> if [allow nulls]; otherwise, <c>false</c>.</value>
		public bool AllowNulls
		{
			get
			{
				return _allowNulls;
			}
		}

		/// <summary>
		/// Gets the dictionary.
		/// </summary>
		/// <value>The dictionary.</value>
		public MetaDictionary Dictionary
		{
			get
			{
				LoadDictionary();
				return _dictionary;
			}
		}

		#endregion

		#region Meta System Information

		/// <summary>
		/// Gets or sets a value indicating whether [allow search].
		/// </summary>
		/// <value><c>true</c> if [allow search]; otherwise, <c>false</c>.</value>
		public bool AllowSearch
		{
			get
			{
				return _allowSearch;
			}
			set
			{
				if (this.Context.FullTextQueriesEnable &&
					this.Context.Transaction != null)
				{
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current.ConnectionString, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_MetaFieldAllowSearch"),
						new SqlParameter("@MetaFieldId", this.Id),
						new SqlParameter("@AllowSearch", value)
						);
				}
				else
				{
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_MetaFieldAllowSearch"),
						new SqlParameter("@MetaFieldId", this.Id),
						new SqlParameter("@AllowSearch", value)
						);
				}

				_allowSearch = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [multilanguage value].
		/// </summary>
		/// <value><c>true</c> if [multilanguage value]; otherwise, <c>false</c>.</value>
		public bool MultilanguageValue
		{
			get
			{
				return _multilanguageValue;
			}
			internal set
			{
				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_MetaFieldAllowMultiLanguage"),
					new SqlParameter("@MetaFieldId", this.Id),
					new SqlParameter("@MultiLanguageValue", value)
					);

				_multilanguageValue = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [save history].
		/// </summary>
		/// <value><c>true</c> if [save history]; otherwise, <c>false</c>.</value>
		public bool SaveHistory
		{
			get
			{
				return _saveHistory;
			}
			set
			{
				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_MetaFieldSaveHistory"),
					new SqlParameter("@MetaFieldId", this.Id),
					new SqlParameter("@SaveHistory", value)
					);

				_saveHistory = value;
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MetaField"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get
			{
				CheckOwnerMetaClass();
				return this.OwnerMetaClass.GetFieldEnabled(this);
			}
			set
			{
				CheckOwnerMetaClass();
				this.OwnerMetaClass.SetFieldEnabled(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>The weight.</value>
		public int Weight
		{
			get
			{
				CheckOwnerMetaClass();
				return this.OwnerMetaClass.GetFieldWeight(this);
			}
			set
			{
				CheckOwnerMetaClass();
				this.OwnerMetaClass.SetFieldWeight(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the is required.
		/// </summary>
		/// <value>The is required.</value>
		public bool IsRequired
		{
			get
			{
				CheckOwnerMetaClass();
				return this.OwnerMetaClass.GetFieldIsRequired(this);
			}
			set
			{
				CheckOwnerMetaClass();
				this.OwnerMetaClass.SetFieldIsRequired(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public int Width
		{
			get
			{
				CheckOwnerMetaClass();
				return this.OwnerMetaClass.GetFieldWidth(this);
			}
			set
			{
				CheckOwnerMetaClass();
				this.OwnerMetaClass.SetFieldWidth(this, value);
			}
		}


		public void LoadDictionary()
		{
			if (_dictionary == null)
			{
				switch (_dataType)
				{
					case MetaDataType.DictionaryMultivalue:
					case MetaDataType.DictionarySingleValue:
					case MetaDataType.EnumMultivalue:
					case MetaDataType.EnumSingleValue:
						_dictionary = MetaDictionary.Load(Id);
						break;
				}
			}
		}
	}
}
