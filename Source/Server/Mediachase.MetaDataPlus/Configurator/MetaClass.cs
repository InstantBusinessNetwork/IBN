using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Configurator
{
	public enum MetaClassType
	{
		System,
		User,
		Abstract
	}
	/// <summary>
	/// Summary description for MetaClass.
	/// </summary>
	public class MetaClass
	{
		private int _id;
		private string _namespace = string.Empty;
		private string _name = string.Empty;
		private string _friendlyName = string.Empty;
		private string _description = string.Empty;
		private string _tableName = string.Empty;
		private bool _isSystem;
		private bool _isAbstract;

		private int _parentMetaClassId;
		private MetaClass _parentMetaClass;
		private MetaFieldCollection _fields;
		private MetaClassCollection _childClasses;

		private string _fieldListChangedSqlScript = string.Empty;

		private object _tag;
		private MetaAttributeCollection _attributes;

		protected MetaClass()
		{
		}

		#region Static Methods

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="Parent">The parent.</param>
		/// <param name="IsSystem">if set to <c>true</c> [is system].</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string metaNamespace, string name, string friendlyName, string tableName, MetaClass parent, bool isSystem, string description)
		{
			return MetaClass.Create(metaNamespace, name, friendlyName, tableName, parent == null ? 0 : parent.Id, isSystem ? MetaClassType.System : MetaClassType.User, description);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="Parent">The parent.</param>
		/// <param name="IsSystem">if set to <c>true</c> [is system].</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string name, string friendlyName, string tableName, MetaClass parent, bool isSystem, string description)
		{
			return MetaClass.Create(isSystem ? MetaNamespace.SystemRoot : MetaNamespace.UserRoot, name, friendlyName, tableName, parent == null ? 0 : parent.Id, isSystem ? MetaClassType.System : MetaClassType.User, description);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="ParentId">The parent id.</param>
		/// <param name="IsSystem">if set to <c>true</c> [is system].</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string name, string friendlyName, string tableName, int parentId, bool isSystem, string description)
		{
			return MetaClass.Create(isSystem ? MetaNamespace.SystemRoot : MetaNamespace.UserRoot, name, friendlyName, tableName, parentId, isSystem ? MetaClassType.System : MetaClassType.User, description);
		}

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="Parent">The parent.</param>
		/// <param name="type">The type.</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string metaNamespace, string name, string friendlyName, string tableName, MetaClass parent, MetaClassType type, string description)
		{
			return MetaClass.Create(metaNamespace, name, friendlyName, tableName, parent == null ? 0 : parent.Id, type, description);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="Parent">The parent.</param>
		/// <param name="type">The type.</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string name, string friendlyName, string tableName, MetaClass parent, MetaClassType type, string description)
		{
			return MetaClass.Create((type == MetaClassType.System) ? MetaNamespace.SystemRoot : MetaNamespace.UserRoot, name, friendlyName, tableName, parent == null ? 0 : parent.Id, type, description);
		}

		/// <summary>
		/// Creates the specified name.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="ParentId">The parent id.</param>
		/// <param name="type">The type.</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string name, string friendlyName, string tableName, int parentId, MetaClassType type, string description)
		{
			return MetaClass.Create((type == MetaClassType.System) ? MetaNamespace.SystemRoot : MetaNamespace.UserRoot, name, friendlyName, tableName, parentId, type, description);
		}

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="ParentId">The parent id.</param>
		/// <param name="IsSystem">if set to <c>true</c> [is system].</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string metaNamespace, string name, string friendlyName, string tableName, int parentId, bool isSystem, string description)
		{
			return MetaClass.Create(metaNamespace, name, friendlyName, tableName, parentId, isSystem ? MetaClassType.System : MetaClassType.User, description);
		}

		/// <summary>
		/// Creates the specified namespace.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TableName">Name of the table.</param>
		/// <param name="ParentId">The parent id.</param>
		/// <param name="type">The type.</param>
		/// <param name="Description">The description.</param>
		/// <returns></returns>
		public static MetaClass Create(string metaNamespace, string name, string friendlyName, string tableName, int parentId, MetaClassType type, string description)
		{
			#region ArgumentNullExceptions
			if (metaNamespace == null)
				throw new ArgumentNullException("Namespace", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (name == null)
				throw new ArgumentNullException("Name", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (friendlyName == null)
				throw new ArgumentNullException("FriendlyName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			if (tableName == null)
				throw new ArgumentNullException("TableName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SqlParameter Retval = new SqlParameter("@Retval", SqlDbType.Int, 4);
			Retval.Direction = ParameterDirection.Output;

			SqlParameter[] sqlParameters = new SqlParameter[] {
																   new SqlParameter("@Namespace",  metaNamespace),
																   new SqlParameter("@Name", name),
																   new SqlParameter("@FriendlyName", friendlyName),
																   new SqlParameter("@TableName", tableName),
																   new SqlParameter("@Description", SqlHelper.Null2DBNull(description)),
																   new SqlParameter("@ParentClassId", parentId),
																   new SqlParameter("@IsSystem", type==MetaClassType.System),
																   new SqlParameter("@IsAbstract", type==MetaClassType.Abstract),
																   Retval
															   };

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_CreateMetaClass"), sqlParameters);

			return MetaClass.Load((int)Retval.Value);
		}



		/// <summary>
		/// Returns MetaClassId, Namespace, Name, FriendlyName, IsSystem, ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag fields.
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetDataReader()
		{
			return MetaClass.GetDataReader(true);
		}

		/// <summary>
		/// Returns MetaClassId, Namespace, Name, FriendlyName, IsSystem, ParentClassId, TableName, Description, FieldListChangedSqlScript, Tag fields.
		/// </summary>
		/// <param name="ListInherited"></param>
		/// <returns></returns>
		public static IDataReader GetDataReader(bool listInherited)
		{
			return SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaClassList"));
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <returns></returns>
		public static MetaClassCollection GetList()
		{
			return MetaClass.GetList(true);
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="ListInherited">if set to <c>true</c> [list inherited].</param>
		/// <returns></returns>
		public static MetaClassCollection GetList(bool listInherited)
		{
			MetaClassCollection retVal = new MetaClassCollection();

			using (SqlDataReader reader = (SqlDataReader)GetDataReader())
			{
				while (reader.Read())
				{
					MetaClass newItem = MetaClass.Load(null, reader);

					if (listInherited || newItem.Parent == null)
						retVal.Add(newItem);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Gets the child list.
		/// </summary>
		/// <param name="parentMetaClass">The parent meta class.</param>
		/// <returns></returns>
		internal static MetaClassCollection GetChildList(MetaClass parentMetaClass)
		{
			#region ArgumentNullExceptions
			if (parentMetaClass == null)
				throw new ArgumentNullException("parentMetaClass", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaClassCollection retVal = new MetaClassCollection();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadChildMetaClassList"),
					  new SqlParameter("@MetaClassId", parentMetaClass.Id)))
			{
				while (reader.Read())
				{
					MetaClass newClass = MetaClass.Load(parentMetaClass, reader);
					retVal.Add(newClass);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Gets the owner meta classes.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <returns></returns>
		internal static MetaClassIdCollection GetOwnerMetaClasses(int metaFieldId)
		{
			MetaClassIdCollection retVal = new MetaClassIdCollection();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaClassListByMetaField"),
					  new SqlParameter("@MetaFieldId", metaFieldId)))
			{
				while (reader.Read())
				{
					retVal.Add((int)reader["MetaClassId"]);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Loads the specified reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public static MetaClass Load(IDataReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));

			return Load(null, reader);
		}

		/// <summary>
		/// Loads the specified parent meta class.
		/// </summary>
		/// <param name="parentMetaClass">The parent meta class.</param>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		internal static MetaClass Load(MetaClass parentMetaClass, IDataReader reader)
		{
			MetaClass retVal = new MetaClass();

			// Load MetaClass Information [11/18/2004]
			retVal._id = (int)reader["MetaClassId"];
			retVal._namespace = (string)reader["Namespace"];
			retVal._name = (string)reader["Name"];
			retVal._friendlyName = (string)reader["FriendlyName"];

			if (reader["Description"] != DBNull.Value)
				retVal._description = (string)reader["Description"];

			retVal._tableName = (string)reader["TableName"];
			retVal._isSystem = (bool)reader["IsSystem"];

			if (reader["IsAbstract"] != DBNull.Value)
				retVal._isAbstract = (bool)reader["IsAbstract"];

			retVal._parentMetaClass = parentMetaClass;
			retVal._parentMetaClassId = (int)SqlHelper.DBNull2Null(reader["ParentClassId"], (object)0);

			retVal._fieldListChangedSqlScript = (string)SqlHelper.DBNull2Null(reader["FieldListChangedSqlScript"]);

			retVal._tag = SqlHelper.DBNull2Null(reader["Tag"]);

			return retVal;
		}

		/// <summary>
		/// Gets the list.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="bDeep">if set to <c>true</c> [b deep].</param>
		/// <returns></returns>
		public static MetaClassCollection GetList(string metaNamespace, bool deep)
		{
			#region ArgumentNullExceptions
			if (metaNamespace == null)
				throw new ArgumentNullException("Namespace", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaClassCollection retVal = new MetaClassCollection();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaClassByNamespace"),
					  new SqlParameter("@Namespace", metaNamespace),
					  new SqlParameter("@Deep", deep)))
			{
				while (reader.Read())
				{
					//MetaClass tmpValue = new MetaClass();

					retVal.Add(MetaClass.Load(null, reader));
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
		public static MetaClass Load(string name)
		{
			#region ArgumentNullExceptions
			if (name == null)
				throw new ArgumentNullException("Name", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaClass retVal = null;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaClassByName"),
					  new SqlParameter("@Name", name)))
			{
				if (reader.Read())
				{
					retVal = MetaClass.Load(null, reader);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Loads the specified id.
		/// </summary>
		/// <param name="Id">The id.</param>
		/// <returns></returns>
		public static MetaClass Load(int metaClassId)
		{
			MetaClass retVal = null;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaClassById"),
					  new SqlParameter("@MetaClassId", metaClassId)))
			{
				if (reader.Read())
				{
					retVal = MetaClass.Load(null, reader);
				}
				reader.Close();
			}

			return retVal;
		}

		/// <summary>
		/// Deletes the specified meta class id.
		/// </summary>
		/// <param name="MetaClassId">The meta class id.</param>
		public static void Delete(int metaClassId)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaClass"),
				new SqlParameter("@MetaClassId", metaClassId)
				);
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
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				#region ArgumentNullExceptions
				if (value == null)
					throw new ArgumentNullException("value", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
				#endregion

				SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
				spTag.Value = SqlHelper.Null2DBNull(this.Tag);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaClass"),
					new SqlParameter("@MetaClassId", this.Id),
					new SqlParameter("@Namespace", this.Namespace),
					new SqlParameter("@Name", value),
					new SqlParameter("@FriendlyName", this.FriendlyName),
					new SqlParameter("@Description", this.Description),
					spTag
					);
				_name = value;
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

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaClass"),

					new SqlParameter("@MetaClassId", this.Id),
					new SqlParameter("@Namespace", this.Namespace),
					new SqlParameter("@Name", this.Name),
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

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaClass"),
					new SqlParameter("@MetaClassId", this.Id),
					new SqlParameter("@Namespace", this.Namespace),
					new SqlParameter("@Name", this.Name),
					new SqlParameter("@FriendlyName", this.FriendlyName),
					new SqlParameter("@Description", SqlHelper.Null2DBNull(value)),
					spTag
					);

				_description = value;
			}
		}

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		/// <value>The name of the table.</value>
		public string TableName
		{
			get
			{
				return _tableName;
			}
		}

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public MetaClass Parent
		{
			get
			{
				if (_parentMetaClass == null && _parentMetaClassId != 0)
				{
					_parentMetaClass = MetaClass.Load(_parentMetaClassId);
				}

				return _parentMetaClass;
			}
		}

		/// <summary>
		/// Gets the child classes.
		/// </summary>
		/// <value>The child classes.</value>
		public MetaClassCollection ChildClasses
		{
			get
			{
				if (_childClasses == null)
					this._childClasses = MetaClass.GetChildList(this);

				return _childClasses;
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
				return _isSystem;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is abstract.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is abstract; otherwise, <c>false</c>.
		/// </value>
		public bool IsAbstract
		{
			get
			{
				return _isAbstract;
			}
		}

		/// <summary>
		/// Gets the meta field list.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <param name="Deep">if set to <c>true</c> [deep].</param>
		/// <returns></returns>
		public MetaFieldCollection GetMetaFieldList(string metaNamespace, bool deep)
		{
			#region ArgumentNullExceptions
			if (metaNamespace == null)
				throw new ArgumentNullException("Namespace", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			string NamespaceFormat = string.Format("{0}.", metaNamespace);

			MetaFieldCollection retVal = new MetaFieldCollection();
			foreach (MetaField item in this.MetaFields)
			{
				if (deep)
				{
					if (string.Compare(item.Namespace, metaNamespace, true) == 0 || item.Namespace.StartsWith(NamespaceFormat))
						retVal.Add(item);
				}
				else
				{
					if (string.Compare(item.Namespace, metaNamespace, true) == 0)
						retVal.Add(item);
				}
			}

			return retVal;
		}

		/// <summary>
		/// Gets the meta field list.
		/// </summary>
		/// <param name="Namespace">The namespace.</param>
		/// <returns></returns>
		public MetaFieldCollection GetMetaFieldList(string metaNamespace)
		{
			return this.GetMetaFieldList(metaNamespace, false);
		}

		/// <summary>
		/// Gets the system meta fields.
		/// </summary>
		/// <value>The system meta fields.</value>
		public MetaFieldCollection SystemMetaFields
		{
			get
			{
				MetaFieldCollection retVal = new MetaFieldCollection();
				foreach (MetaField item in this.MetaFields)
				{
					if (item.IsSystem)
						retVal.Add(item);
				}

				return retVal;
			}
		}

		public MetaFieldCollection UserMetaFields
		{
			get
			{
				MetaFieldCollection retVal = new MetaFieldCollection();
				foreach (MetaField item in this.MetaFields)
				{
					if (item.IsUser)
						retVal.Add(item);
				}
				return retVal;
			}
		}

		/// <summary>
		/// Gets the meta fields.
		/// </summary>
		/// <value>The meta fields.</value>
		public MetaFieldCollection MetaFields
		{
			get
			{
				if (_fields == null)
					_fields = MetaField.GetList(this);

				return _fields;
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
				if (value != null && value.GetType() != typeof(byte[]))
					throw new ArgumentException("Only byte[] or Null are supported now.");

				SqlParameter spTag = new SqlParameter("@Tag", SqlDbType.Image);
				spTag.Value = SqlHelper.Null2DBNull(this.Tag);

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaClass"),
					new SqlParameter("@MetaClassId", this.Id),
					new SqlParameter("@Namespace", this.Namespace),
					new SqlParameter("@Name", this.Name),
					new SqlParameter("@FriendlyName", this.FriendlyName),
					new SqlParameter("@Description", this.Description),
					spTag
					);

				_tag = value;
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

				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaClass"),
					new SqlParameter("@MetaClassId", this.Id),
					new SqlParameter("@Namespace", value),
					new SqlParameter("@Name", this.Name),
					new SqlParameter("@FriendlyName", this.FriendlyName),
					new SqlParameter("@Description", this.Description),
					spTag
					);

				_namespace = value;
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

		/// <summary>
		/// Gets the type of the meta class.
		/// </summary>
		/// <value>The type of the meta class.</value>
		public MetaClassType MetaClassType
		{
			get
			{
				if (IsSystem)
					return MetaClassType.System;
				if (IsAbstract)
					return MetaClassType.Abstract;
				return MetaClassType.User;
			}
		}

		#endregion

		#region Gets , Sets FieldWeight
		/// <summary>
		/// Gets the field weight.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <returns></returns>
		public int GetFieldWeight(int metaFieldId)
		{
			int RetVal = 0;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldWeight"),
					  new SqlParameter("@MetaClassId", Id), new SqlParameter("@MetaFieldId", metaFieldId)))
			{
				if (reader.Read())
				{
					RetVal = (int)reader["Weight"];
				}
				reader.Close();
			}

			return RetVal;
		}

		/// <summary>
		/// Gets the field weight.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public int GetFieldWeight(MetaField field)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldWeight(field.Id);
		}

		/// <summary>
		/// Gets the field weight.
		/// </summary>
		/// <param name="FieldName">Name of the field.</param>
		/// <returns></returns>
		public int GetFieldWeight(string fieldName)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldWeight(this.MetaFields[fieldName].Id);
		}

		/// <summary>
		/// Sets the field weight.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <param name="Weight">The weight.</param>
		public void SetFieldWeight(int metaFieldId, int weight)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaFieldWeight"),
				new SqlParameter("@MetaClassId", this.Id),
				new SqlParameter("@MetaFieldId", metaFieldId),
				new SqlParameter("@Weight", weight)
				);
		}

		/// <summary>
		/// Sets the field weight.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="Weight">The weight.</param>
		public void SetFieldWeight(MetaField field, int weight)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldWeight(field.Id, weight);
		}

		/// <summary>
		/// Sets the field weight.
		/// </summary>
		/// <param name="FieldName">Name of the field.</param>
		/// <param name="Weight">The weight.</param>
		public void SetFieldWeight(string fieldName, int weight)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldWeight(this.MetaFields[fieldName].Id, weight);
		}
		#endregion

		#region Gets , Sets FieldEnabled
		/// <summary>
		/// Gets the field enabled.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <returns></returns>
		public bool GetFieldEnabled(int metaFieldId)
		{
			bool RetVal = false;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldWeight"),
					  new SqlParameter("@MetaClassId", Id), new SqlParameter("@MetaFieldId", metaFieldId)))
			{
				if (reader.Read())
				{
					RetVal = (bool)reader["Enabled"];
				}
				reader.Close();
			}

			return RetVal;
		}

		/// <summary>
		/// Gets the field enabled.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public bool GetFieldEnabled(MetaField field)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldEnabled(field.Id);
		}

		public bool GetFieldEnabled(string fieldName)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldEnabled(this.MetaFields[fieldName].Id);
		}

		/// <summary>
		/// Sets the field enabled.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <param name="Enabled">if set to <c>true</c> [enabled].</param>
		public void SetFieldEnabled(int metaFieldId, bool enabled)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaFieldEnabled"),
				new SqlParameter("@MetaClassId", this.Id),
				new SqlParameter("@MetaFieldId", metaFieldId),
				new SqlParameter("@Enabled", enabled)
				);
		}

		/// <summary>
		/// Sets the field enabled.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="Enabled">if set to <c>true</c> [enabled].</param>
		public void SetFieldEnabled(MetaField field, bool enabled)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldEnabled(field.Id, enabled);
		}

		/// <summary>
		/// Sets the field enabled.
		/// </summary>
		/// <param name="FieldName">Name of the field.</param>
		/// <param name="Enabled">if set to <c>true</c> [enabled].</param>
		public void SetFieldEnabled(string fieldName, bool enabled)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldEnabled(this.MetaFields[fieldName].Id, enabled);
		}


		#endregion

		#region Gets , Sets FeildIsRequired
		/// <summary>
		/// Gets the field is required.
		/// </summary>
		/// <param name="metaFieldId">The meta field id.</param>
		/// <returns></returns>
		public bool GetFieldIsRequired(int metaFieldId)
		{
			bool RetVal = false;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current,
				CommandType.StoredProcedure,
				AsyncResources.GetConstantValue("SP_LoadMetaFieldIsRequired"),
					  new SqlParameter("@MetaClassId", Id), new SqlParameter("@MetaFieldId", metaFieldId)))
			{
				if (reader.Read())
				{
					RetVal = (bool)reader["IsRequired"];
				}
				reader.Close();
			}

			return RetVal;
		}

		/// <summary>
		/// Gets the field is required.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public bool GetFieldIsRequired(MetaField field)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldIsRequired(field.Id);
		}

		/// <summary>
		/// Gets the field is required.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public bool GetFieldIsRequired(string fieldName)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldIsRequired(this.MetaFields[fieldName].Id);
		}

		/// <summary>
		/// Sets the field enabled.
		/// </summary>
		/// <param name="metaFieldId">The meta field id.</param>
		/// <param name="isRequired">if set to <c>true</c> [is required].</param>
		public void SetFieldIsRequired(int metaFieldId, bool isRequired)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure,
				AsyncResources.GetConstantValue("SP_UpdateMetaFieldIsRequired"),
				new SqlParameter("@MetaClassId", this.Id),
				new SqlParameter("@MetaFieldId", metaFieldId),
				new SqlParameter("@IsRequired", isRequired)
				);
		}

		public void SetFieldIsRequired(MetaField field, bool isRequired)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldIsRequired(field.Id, isRequired);
		}

		/// <summary>
		/// Sets the field enabled.
		/// </summary>
		/// <param name="FieldName">Name of the field.</param>
		/// <param name="Enabled">if set to <c>true</c> [enabled].</param>
		public void SetFieldIsRequired(string fieldName, bool enabled)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldIsRequired(this.MetaFields[fieldName].Id, enabled);
		}


		#endregion

		#region Gets , Sets FieldWidth
		/// <summary>
		/// Gets the field width.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <returns></returns>
		public int GetFieldWidth(int metaFieldId)
		{
			int RetVal = 0;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current,
				CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFieldWidth"),
					  new SqlParameter("@MetaClassId", Id), new SqlParameter("@MetaFieldId", metaFieldId)))
			{
				if (reader.Read())
				{
					RetVal = (int)reader["Width"];
				}
				reader.Close();
			}

			return RetVal;
		}

		/// <summary>
		/// Gets the field width.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public int GetFieldWidth(MetaField field)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldWidth(field.Id);
		}

		/// <summary>
		/// Gets the width of the field.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public int GetFieldWidth(string fieldName)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			return this.GetFieldWidth(this.MetaFields[fieldName].Id);
		}

		/// <summary>
		/// Sets the field width.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <param name="Width">if set to <c>true</c> [width].</param>
		public void SetFieldWidth(int metaFieldId, int width)
		{
			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure,
				AsyncResources.GetConstantValue("SP_UpdateMetaFieldWidth"),
				new SqlParameter("@MetaClassId", this.Id),
				new SqlParameter("@MetaFieldId", metaFieldId),
				new SqlParameter("@Width", width)
				);
		}

		/// <summary>
		/// Sets the field width.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="Width">if set to <c>true</c> [width].</param>
		public void SetFieldWidth(MetaField field, int width)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldWidth(field.Id, width);
		}

		/// <summary>
		/// Sets the field width.
		/// </summary>
		/// <param name="FieldName">Name of the field.</param>
		/// <param name="Width">if set to <c>true</c> [width].</param>
		public void SetFieldWidth(string fieldName, int width)
		{
			#region ArgumentNullExceptions
			if (fieldName == null)
				throw new ArgumentNullException("FieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			SetFieldWidth(this.MetaFields[fieldName].Id, width);
		}


		#endregion

		#region ExecuteFieldListChangedSqlScript

		/// <summary>
		/// Gets or sets the field list changed SQL script.
		/// </summary>
		/// <value>The field list changed SQL script.</value>
		public string FieldListChangedSqlScript
		{
			get
			{
				return _fieldListChangedSqlScript;
			}
			set
			{
				SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaSqlScriptTemplate"),
					new SqlParameter("@MetaClassId", this.Id),
					new SqlParameter("@FieldListChanged", SqlHelper.Null2DBNull(value))
					);

				_fieldListChangedSqlScript = value;

				ExecuteFieldListChangedSqlScript();
			}
		}

		/// <summary>
		/// Executes the field list changed SQL script.
		/// </summary>
		protected virtual void ExecuteFieldListChangedSqlScript()
		{
			if (!string.IsNullOrEmpty(this.FieldListChangedSqlScript))
			{
				string retVal = ExecuteFieldListChangedSqlScript(this.FieldListChangedSqlScript);

				// Execute Script [12/3/2004]
				if (!string.IsNullOrEmpty(retVal))
				{
					SqlHelper.ExecuteScript(MetaDataContext.Current, retVal);
				}
			}
		}

		/// <summary>
		/// Executes the field list changed SQL script.
		/// </summary>
		/// <param name="Script">The script.</param>
		/// <returns></returns>
		protected virtual string ExecuteFieldListChangedSqlScript(string script)
		{
			Regex regExp = new Regex(@"\x5B\x25\x3D(?<Name>\w+)(\x28((\s*(?<Param>((\x22[^\x22]+\x22)|([\w@]+)))\s*),?)*\x29)?\x3D\x25\x5D",
				RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

			MatchCollection matches = regExp.Matches(script);

			StringBuilder builder = new StringBuilder(script.Length);
			int builderPos = 0;

			StringDictionary processActiveTag = new StringDictionary();

			// Modify String [12/3/2004]
			foreach (Match matchItem in matches)
			{
				if ((matchItem.Index - builderPos) > 0)
				{
					builder.Append(script, builderPos, matchItem.Index - builderPos);
					builderPos += matchItem.Index - builderPos;
				}

				string activeTag = matchItem.Value;
				if (!processActiveTag.ContainsKey(activeTag))
				{
					string activeTagName = matchItem.Groups["Name"].Value;
					StringCollection activeTagParams = new StringCollection();

					if (matchItem.Groups["Param"].Success)
					{
						foreach (Capture captureItem in matchItem.Groups["Param"].Captures)
						{
							activeTagParams.Add(captureItem.Value.Trim('"'));
						}
					}

					// Process AT Command [12/7/2004]
					StringBuilder activeTagValue = new StringBuilder();

					switch (activeTagName.ToLower())
					{

						case "currentmetaclassid":
							#region currentmetaclassid
							{
								activeTagValue.Append(this.Id.ToString());
							}
							#endregion
							break;

						case "currentmetaclass":
							#region currentmetaclass
							{
								string alias = (activeTagParams.Count >= 1) ? activeTagParams[0] : string.Empty;
								if (!string.IsNullOrEmpty(alias))
									activeTagValue.AppendFormat("{0} {1}", this.TableName, alias);
								else
									activeTagValue.Append(this.TableName);
							}
							#endregion
							break;
						case "metafields":
							#region metafields
							{
								string alias = (activeTagParams.Count >= 1) ? activeTagParams[0] : string.Empty;
								MetaClass owner = (activeTagParams.Count >= 2) ? MetaClass.Load(activeTagParams[1]) : this;

								foreach (MetaField field in owner.MetaFields)
								{
									if (activeTagValue.Length > 0)
										activeTagValue.Append(", ");

									if (!string.IsNullOrEmpty(alias))
										activeTagValue.AppendFormat("{0}.[{1}]", alias, field.Name);
									else
										activeTagValue.AppendFormat("[{0}]", field.Name);
								}

								if (activeTagValue.Length == 0)
									activeTagValue.Append("NULL as [Null]");
							}
							#endregion
							break;
						case "usermetafields":
							#region usermetafields
							{
								string alias = (activeTagParams.Count >= 1) ? activeTagParams[0] : string.Empty;
								MetaClass owner = (activeTagParams.Count >= 2) ? MetaClass.Load(activeTagParams[1]) : this;

								foreach (MetaField field in owner.UserMetaFields)
								{
									if (activeTagValue.Length > 0)
										activeTagValue.Append(", ");

									if (!string.IsNullOrEmpty(alias))
										activeTagValue.AppendFormat("{0}.[{1}]", alias, field.Name);
									else
										activeTagValue.AppendFormat("[{0}]", field.Name);
								}

								if (activeTagValue.Length == 0)
									activeTagValue.Append("NULL as [Null]");

							}
							#endregion
							break;
						case "systemmetafields":
							#region systemmetafields
							{
								string alias = (activeTagParams.Count >= 1) ? activeTagParams[0] : string.Empty;
								MetaClass owner = (activeTagParams.Count >= 1) ? MetaClass.Load(activeTagParams[0]) : this;

								foreach (MetaField field in owner.SystemMetaFields)
								{
									if (activeTagValue.Length > 0)
										activeTagValue.Append(", ");

									if (!string.IsNullOrEmpty(alias))
										activeTagValue.AppendFormat("{0}.[{1}]", alias, field.Name);
									else
										activeTagValue.AppendFormat("[{0}]", field.Name);

								}

								if (activeTagValue.Length == 0)
									activeTagValue.Append("NULL as [Null]");
							}
							#endregion
							break;
						case "searchbykeyword":
							#region searchbykeyword
							{
								MetaClass owner = (activeTagParams.Count >= 3) ? MetaClass.Load(activeTagParams[2]) : this;

								if (owner != null)
								{
									foreach (MetaField field in owner.MetaFields)
									{
										if (field.AllowSearch)
										{
											switch (field.DataType)
											{
												case MetaDataType.Char:
												case MetaDataType.NChar:
												case MetaDataType.Text:
												case MetaDataType.NText:
												case MetaDataType.VarChar:
												case MetaDataType.NVarChar:
												case MetaDataType.Email:
												case MetaDataType.ShortString:
												case MetaDataType.LongHtmlString:
												case MetaDataType.LongString:
												case MetaDataType.Url:
													if (activeTagValue.Length > 0)
														activeTagValue.Append(" OR ");
													activeTagValue.AppendFormat("{0}.[{1}] LIKE {2}", (activeTagParams.Count >= 1) ? activeTagParams[0] : "MDPO", field.Name, (activeTagParams.Count >= 2) ? activeTagParams[1] : "@Keyword");
													break;
											}
										}
									}
								}

								if (activeTagValue.Length == 0)
									activeTagValue.Append("1<>1");
							}
							#endregion
							break;
						case "fulltextquery":
							#region fulltextquery
							{
								MetaClass owner = (activeTagParams.Count >= 3) ? MetaClass.Load(activeTagParams[2]) : this;

								foreach (MetaField field in owner.MetaFields)
								{
									if (field.AllowSearch)
									{
										switch (field.DataType)
										{
											case MetaDataType.Char:
											case MetaDataType.NChar:
											case MetaDataType.Text:
											case MetaDataType.NText:
											case MetaDataType.VarChar:
											case MetaDataType.NVarChar:
											case MetaDataType.Email:
											case MetaDataType.ShortString:
											case MetaDataType.LongHtmlString:
											case MetaDataType.LongString:
											case MetaDataType.Url:
												if (activeTagValue.Length > 0)
													activeTagValue.Append(" OR ");
												activeTagValue.AppendFormat("CONTAINS({0}.[{1}], {2})", (activeTagParams.Count >= 1) ? activeTagParams[0] : "MDPO", field.Name, (activeTagParams.Count >= 2) ? activeTagParams[1] : "@Keyword");
												break;
										}
									}
								}

								if (activeTagValue.Length == 0)
									activeTagValue.Append("1<>1");
							}
							#endregion
							break;
						case "foreach":
							#region	  foreach
							{
								string sqlQuery = activeTagParams[0];
								string unionLine = activeTagParams[1];
								string stringFormat = activeTagParams[2];

								using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.Text, sqlQuery))
								{
									while (reader.Read())
									{
										if (activeTagValue.Length != 0)
										{
											activeTagValue.Append("\r\n");
											activeTagValue.Append(unionLine);
											activeTagValue.Append("\r\n");
										}

										object[] param = new object[reader.FieldCount];
										reader.GetValues(param);

										activeTagValue.AppendFormat(stringFormat, param);
									}
								}

								if (activeTagValue.Length == 0)
									activeTagValue.Append("1<>1");
								else
									activeTagValue = new StringBuilder(ExecuteFieldListChangedSqlScript(activeTagValue.ToString()));
							}
							#endregion
							break;
					}

					processActiveTag.Add(activeTag, activeTagValue.ToString());
					// End
					//builder.Append(ATName);
				}

				builder.Append(processActiveTag[activeTag]);
				builderPos += matchItem.Length;
			}

			if (builderPos < script.Length)
			{
				builder.Append(script, builderPos, script.Length - builderPos);
			}

			//////////////////////////////////////////////////////////////////////////
			///
			return builder.ToString();
		}
		#endregion

		#region Add Field Methods
		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="field">The field.</param>
		public virtual void AddField(MetaField field)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			this.AddField(field.Id, 0);
		}

		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="Weight">The weight.</param>
		public virtual void AddField(MetaField field, int weight)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			if (this.IsSystem)
				throw new NotSupportedException(AsyncResources.GetConstantValue("SYS_CLASS_READ_ONLY_ERR_MSG"));

			// Solve Meta Field Duplication Problem
			MetaFieldCollection tmpCol = this.MetaFields;

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_AddMetaFieldToMetaClass"),
				new SqlParameter("@MetaClassId", this.Id),
				new SqlParameter("@MetaFieldId", field.Id),
				new SqlParameter("@Weight", weight));

			tmpCol.Add(field);
			field.SetOwnerMetaClass(this);

			ExecuteFieldListChangedSqlScript();
		}

		/// <summary>
		/// Adds the field.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		/// <param name="Weight">The weight.</param>
		public virtual void AddField(int metaFieldId, int weight)
		{
			this.AddField(MetaField.Load(metaFieldId), weight);
		}
		#endregion

		#region Delete Field Methods

		/// <summary>
		/// Tests the meta field.
		/// </summary>
		/// <param name="MataFieldId">The mata field id.</param>
		protected void TestMetaField(int mataFieldId)
		{
			foreach (MetaField field in this.MetaFields)
			{
				if (field.Id == mataFieldId)
				{
					if (field.IsSystem)
					{
						throw new ArgumentException(AsyncResources.GetConstantValue("INHERITED_FIELD_ERR_MSG"));
					}
					return;
				}
			}

			throw new ArgumentException(AsyncResources.GetConstantValue("INVALIDE_FIELD_ERR_MSG"));
		}

		//		public void Refresh()
		//		{
		//		}

		/// <summary>
		/// Deletes the field.
		/// </summary>
		/// <param name="MetaFieldId">The meta field id.</param>
		public virtual void DeleteField(int metaFieldId)
		{
			if (this.IsSystem)
				throw new NotSupportedException(AsyncResources.GetConstantValue("SYS_CLASS_READ_ONLY_ERR_MSG"));

			TestMetaField(metaFieldId);

			SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaFieldFromMetaClass"),
				new SqlParameter("@MetaClassId", this.Id),
				new SqlParameter("@MetaFieldId", metaFieldId)
				);

			this.MetaFields.Delete(metaFieldId);

			ExecuteFieldListChangedSqlScript();
		}

		/// <summary>
		/// Deletes the field.
		/// </summary>
		/// <param name="MetaFieldName">Name of the meta field.</param>
		public virtual void DeleteField(string metaFieldName)
		{
			#region ArgumentNullExceptions
			if (metaFieldName == null)
				throw new ArgumentNullException("MetaFieldName", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			MetaField fieldToDelete = this.MetaFields[metaFieldName];
			this.DeleteField(fieldToDelete);
		}

		/// <summary>
		/// Deletes the field.
		/// </summary>
		/// <param name="field">The field.</param>
		public virtual void DeleteField(MetaField field)
		{
			#region ArgumentNullExceptions
			if (field == null)
				throw new ArgumentNullException("field", AsyncResources.GetConstantValue("ARG_NULL_ERR_MSG"));
			#endregion

			this.DeleteField(field.Id);
		}
		#endregion

	}
}
