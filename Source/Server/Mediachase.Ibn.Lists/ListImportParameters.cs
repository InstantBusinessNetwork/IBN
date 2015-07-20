using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Lists.Mapping;
using System.Data;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Lists
{
	/// <summary>
	/// Represents parameters batch for list initialization.
	/// </summary>
	[Serializable]
	public class ListImportParameters
	{
		#region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateListParameters"/> class.
		/// </summary>
		public ListImportParameters(int folderId, string listName, DataSet externalDataSet)
		{
			if (listName == null)
				throw new ArgumentNullException("listName");
			if (externalDataSet == null)
				throw new ArgumentNullException("externalDataSet");

			this.ListName = listName;
			this.FolderId = folderId;
			this.ExternalData = externalDataSet;

			CreateDefaultMapping();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListImportParameters"/> class.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <param name="listName">Name of the list.</param>
		/// <param name="externalDataSet">The external data set.</param>
		/// <param name="externalDataTableIndex">Index of the external data table.</param>
		public ListImportParameters(int folderId, string listName, DataSet externalDataSet, int externalDataTableIndex)
		{
			if (listName == null)
				throw new ArgumentNullException("listName");
			if (externalDataSet == null)
				throw new ArgumentNullException("externalDataSet");

			this.ListName = listName;
			this.FolderId = folderId;
			this.ExternalData = externalDataSet;
			this.ExternalDataTableIndex = externalDataTableIndex;

			CreateDefaultMapping();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListImportParameters"/> class.
		/// </summary>
		/// <param name="listInfoId">The list info id.</param>
		public ListImportParameters(int listInfoId, DataSet externalDataSet)
		{
			if (externalDataSet == null)
				throw new ArgumentNullException("externalDataSet");

			this.ListInfoId = listInfoId;
			this.ExternalData = externalDataSet;

			CreateDefaultMapping();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListImportParameters"/> class.
		/// </summary>
		/// <param name="listInfoId">The list info id.</param>
		/// <param name="externalDataSet">The external data set.</param>
		/// <param name="externalDataTableIndex">Index of the external data table.</param>
		public ListImportParameters(int listInfoId, DataSet externalDataSet, int externalDataTableIndex)
		{
			if (externalDataSet == null)
				throw new ArgumentNullException("externalDataSet");

			this.ListInfoId = listInfoId;
			this.ExternalData = externalDataSet;
			this.ExternalDataTableIndex = externalDataTableIndex;

			CreateDefaultMapping();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListImportParameters"/> class.
		/// </summary>
		public ListImportParameters()
		{
		}
		#endregion

		#region Properties

		#region List Properties
		private int? _listInfoId;

		/// <summary>
		/// Gets or sets the list id.
		/// </summary>
		/// <value>The list id.</value>
		public int? ListInfoId
		{
			get { return _listInfoId; }
			set { _listInfoId = value; }
		}

		private int? _createdListInfoId;

		/// <summary>
		/// Gets or sets the created list id.
		/// </summary>
		/// <value>The created list id.</value>
		public int? CreatedListId
		{
			get { return _createdListInfoId; }
			set { _createdListInfoId = value; }
		}

		private int _listFolderId;

		/// <summary>
		/// Gets or sets the list folder id.
		/// </summary>
		/// <value>The list folder id.</value>
		public int FolderId
		{
			get { return _listFolderId; }
			set { _listFolderId = value; }
		}

		private string _listName;

		/// <summary>
		/// Gets or sets the name of the list.
		/// </summary>
		/// <value>The name of the list.</value>
		public string ListName
		{
			get { return _listName; }
			set { _listName = value; }
		}

		private string _description;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		private int? _listType;

		/// <summary>
		/// Gets or sets the type of the list.
		/// </summary>
		/// <value>The type of the list.</value>
		public int? ListType
		{
			get { return _listType; }
			set { _listType = value; }
		}

		private int? _status;

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>The status.</value>
		public int? Status
		{
			get { return _status; }
			set { _status = value; }
		}
	

		/// <summary>
		/// Gets the name of the destination meta class.
		/// </summary>
		/// <value>The name of the destination meta class.</value>
		[XmlIgnore]
		public string DestinationMetaClassName
		{
			get
			{
				if (this.IsNewList)
					return string.Empty;

				return ListManager.GetListMetaClassName(this.ListInfoId.Value);
			}
		}

		/// <summary>
		/// Gets the name of the source table.
		/// </summary>
		/// <value>The name of the source table.</value>
		[XmlIgnore]
		public string SourceTableName
		{
			get
			{
				if (this.ExternalData == null || this.ExternalData.Tables.Count == 0)
					return string.Empty;

				return this.ExternalData.Tables[this.ExternalDataTableIndex].TableName;
			}
		}

		private string _titleFieldName;

		/// <summary>
		/// Gets or sets the name of the title field.
		/// </summary>
		/// <value>The name of the title field.</value>
		public string TitleFieldName
		{
			get { return _titleFieldName; }
			set { _titleFieldName = value; }
		}
	
		#endregion

		#region DataSet Properties
		private DataSet _dataSet;

		/// <summary>
		/// Gets or sets the external data.
		/// </summary>
		/// <value>The external data.</value>
		public DataSet ExternalData
		{
			get { return _dataSet; }
			set { _dataSet = value; }
		}

		private int _selectedTableIndex;

		/// <summary>
		/// Gets or sets the index of the external data table.
		/// </summary>
		/// <value>The index of the external data table.</value>
		public int ExternalDataTableIndex
		{
			get { return _selectedTableIndex; }
			set { _selectedTableIndex = value; }
		}

		#endregion

		#region New List Meta Fields Properties
		private List<MetaField> _newMetaFields = new List<MetaField>();

		/// <summary>
		/// Gets the new meta fields.
		/// </summary>
		/// <value>The new meta fields.</value>
		public IList<MetaField> NewMetaFields
		{
			get { return _newMetaFields; }
		}

		private bool _autoFillEnum = true; // = false;

		/// <summary>
		/// Gets or sets a value indicating whether [auto fill meta enum].
		/// </summary>
		/// <value><c>true</c> if [auto fill meta enum]; otherwise, <c>false</c>.</value>
		public bool AutoFillMetaEnum
		{
			get { return _autoFillEnum; }
			set { _autoFillEnum = value; }
		}
	
		#endregion

		#region Mapping Properties
		private MappingDocument _mappindDocument = new MappingDocument();

		/// <summary>
		/// Gets or sets the mapping document.
		/// </summary>
		/// <value>The mapping document.</value>
		public MappingDocument MappingDocument
		{
			get { return _mappindDocument; }
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether this instance is new list.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is new list; otherwise, <c>false</c>.
		/// </value>
		public bool IsNewList
		{
			get { return !this.ListInfoId.HasValue; }
		}

		#region IsEnum
		List<string> _newEnums = new List<string>();

		/// <summary>
		/// Gets or sets the new enums.
		/// </summary>
		/// <value>The new enums.</value>
		public IList<string> NewEnums
		{
			get { return _newEnums; }
		}


		private List<NewEnumInfo> _newEnumTypes = new List<NewEnumInfo>();


		/// <summary>
		/// Gets or sets the new enum types.
		/// </summary>
		/// <value>The new enum types.</value>
		public IList<NewEnumInfo> NewEnumTypes
		{
			get { return _newEnumTypes; }
		}
	
		#endregion

		#endregion

		#region Methods

		#region CreateDefaultMapping
		/// <summary>
		/// Creates the default mapping.
		/// </summary>
		public void CreateDefaultMapping()
		{
			// Clear List Mapping
			this.MappingDocument.Clear();

			if (this.IsNewList)
			{
				CreateNewListDefaultMapping();
			}
			else
			{
				CreateListDefaultMapping();
			}
		}

		/// <summary>
		/// Creates the list default mapping.
		/// </summary>
		protected virtual void CreateListDefaultMapping()
		{
			MappingElement mapping = new MappingElement(this.SourceTableName, this.DestinationMetaClassName);

			this.MappingDocument.Add(mapping);

			if (this.ExternalData == null || this.ExternalData.Tables.Count == 0)
				return;

			foreach (DataColumn column in this.GetSourceColumns())
			{
				if (column.ColumnName == "Created" ||
						column.ColumnName == "CreatorId" ||
						column.ColumnName == "Modified" ||
						column.ColumnName == "ModifierId")
					continue;

				foreach (MetaField field in this.GetDestinationMetaFields())
				{
					if (field.Name.StartsWith(column.ColumnName, StringComparison.OrdinalIgnoreCase) ||
						field.FriendlyName.StartsWith(column.ColumnName, StringComparison.OrdinalIgnoreCase))
					{
						AssignCopyValueRule(column.ColumnName, field.Name);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Creates the new list default mapping.
		/// </summary>
		protected virtual void CreateNewListDefaultMapping()
		{
			MappingElement mapping = new MappingElement(this.SourceTableName, this.DestinationMetaClassName);

			this.MappingDocument.Add(mapping);

			if (this.ExternalData == null || this.ExternalData.Tables.Count == 0)
				return;

			foreach (DataColumn column in this.GetSourceColumns())
			{
				// TODO: Exclude PrimaryKey

				if (column.ColumnName == "Created" ||
						column.ColumnName == "CreatorId" ||
						column.ColumnName == "Modified" ||
						column.ColumnName == "ModifierId")
					continue;


				MetaField field = CreateNewMetaField(column);

				this.NewMetaFields.Add(field);

				AssignCopyValueRule(column.ColumnName, field.Name);
			}
		}

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns></returns>
		private static string GetFieldName(DataColumn column)
		{
			string name = column.ColumnName;

			if (Regex.Matches(name, @"^[A-Za-z0-9]([A-Za-z0-9]|\x20)*$").Count == 0)
				return string.Format(CultureInfo.InvariantCulture, "Field{0:00}", column.Ordinal);

			return name.Replace(' ', '_');
		}

		/// <summary>
		/// Creates the new meta field.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns></returns>
		public static MetaField CreateNewMetaField(DataColumn column)
		{
			if (column == null)
				throw new ArgumentNullException("column");

			MetaField retVal = null;

			string name = GetFieldName(column);
			string friendlyName = string.IsNullOrEmpty(column.ColumnName) ? name : column.ColumnName; // TODO:
			bool isNullable = column.AllowDBNull;// true;

			if (column.DataType == typeof(Int32) ||
				column.DataType == typeof(Int16))
			{
				#region MetaFieldType.Integer
				AttributeCollection attr = new AttributeCollection();
				string strDefaultValue = "0";

				retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.Integer, isNullable, strDefaultValue, attr);

				#endregion
			}
			else if (column.DataType == typeof(Double) ||
				column.DataType == typeof(Single))
			{
				#region MetaFieldType.Float
				AttributeCollection attr = new AttributeCollection();
				string strDefaultValue = "0";

				attr.Add(McDataTypeAttribute.DecimalPrecision, 18);
				attr.Add(McDataTypeAttribute.DecimalScale, 4);

				retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.Decimal, isNullable, strDefaultValue, attr);

				#endregion
			}
			else if (column.DataType == typeof(DateTime))
			{
				#region MetaFieldType.DateTime
				AttributeCollection attr = new AttributeCollection();
				string defaultValue = isNullable ? string.Empty : "getutcdate()";

				//Attr.Add(McDataTypeAttribute.DateTimeMinValue, minValue);
				//Attr.Add(McDataTypeAttribute.DateTimeMaxValue, maxValue);
				//attr.Add(McDataTypeAttribute.DateTimeUseUserTimeZone, useUserTimeZone);

				retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.DateTime, isNullable, defaultValue, attr);

				#endregion
			}
			else if (column.DataType == typeof(Boolean))
			{
				#region MetaFieldType.CheckboxBoolean
				AttributeCollection attr = new AttributeCollection();
				string strDefaultValue = "0";

				attr.Add(McDataTypeAttribute.BooleanLabel, friendlyName);

				retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.CheckboxBoolean, isNullable, strDefaultValue, attr);

				#endregion
			}
			else if (column.DataType == typeof(Guid))
			{
				#region MetaFieldType.Guid
				AttributeCollection attr = new AttributeCollection();
				string defaultValue = isNullable ? string.Empty : "newid()";

				retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.Guid, isNullable, defaultValue, attr);

				#endregion
			}
			else // if (column.DataType == typeof(String))
			{
				if (column.MaxLength != -1) // Text
				{
					#region MetaFieldType.Text
					AttributeCollection attr = new AttributeCollection();
					string strDefaultValue = isNullable ? string.Empty : "''";

					attr.Add(McDataTypeAttribute.StringMaxLength, column.MaxLength);
					attr.Add(McDataTypeAttribute.StringIsUnique, column.Unique);

					retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.Text, isNullable, strDefaultValue, attr);

					#endregion
				}
				//else if (column.MaxLength == -1)
				//{
				//    #region MetaFieldType.Text 255 Length
				//    AttributeCollection attr = new AttributeCollection();
				//    string strDefaultValue = isNullable ? string.Empty : "''";

				//    attr.Add(McDataTypeAttribute.StringMaxLength, 255);
				//    attr.Add(McDataTypeAttribute.StringIsUnique, false);

				//    retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.Text, isNullable, strDefaultValue, attr);

				//    #endregion
				//}
				else // LongText (Default)
				{
					#region MetaFieldType.LongText
					AttributeCollection attr = new AttributeCollection();
					string strDefaultValue = isNullable ? string.Empty : "''";

					attr.Add(McDataTypeAttribute.StringLongText, true);

					retVal = CreateNewMetaField(name, friendlyName, MetaFieldType.LongText, isNullable, strDefaultValue, attr);

					#endregion
				}
			}

			return retVal;
		}

		/// <summary>
		/// Creates the new meta field.
		/// </summary>
		/// <param name="Name">The name.</param>
		/// <param name="FriendlyName">Name of the friendly.</param>
		/// <param name="TypeName">Name of the type.</param>
		/// <param name="IsNullable">if set to <c>true</c> [is nullable].</param>
		/// <param name="DefaultValue">The default value.</param>
		/// <param name="Attributes">The attributes.</param>
		/// <returns></returns>
		public static MetaField CreateNewMetaField(string name,
			string friendlyName,
			string typeName,
			bool isNullable,
			string defaultValue,
			AttributeCollection attributes)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (typeName == null)
				throw new ArgumentNullException("typeName");

			if (friendlyName == null)
				friendlyName = name;
			if (attributes == null)
				attributes = new AttributeCollection();

			// Create a new meta feild
			MetaField newField = new MetaField(name, friendlyName);
			newField.TypeName = typeName;
			newField.IsNullable = isNullable;

			if(DataContext.Current.MetaModel.RegisteredTypes[typeName]!=null)
				newField.Attributes.AddRange(DataContext.Current.MetaModel.RegisteredTypes[typeName].Attributes);

			newField.Attributes.AddRange(attributes);
			//newField.Attributes.Add(Attributes);
			newField.DefaultValue = defaultValue;

			// return a new item
			return newField;
		}
		#endregion

		#region GetSourceColumns
		/// <summary>
		/// Gets the source columns.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns></returns>
		public DataColumnCollection GetSourceColumns(string tableName)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			return this.ExternalData.Tables[tableName].Columns;
		}

		/// <summary>
		/// Gets the source columns.
		/// </summary>
		/// <returns></returns>
		public DataColumnCollection GetSourceColumns()
		{
			return this.ExternalData.Tables[this.ExternalDataTableIndex].Columns;
		}

		/// <summary>
		/// Gets the source column.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public DataColumn GetSourceColumn(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			foreach (DataColumn column in GetSourceColumns())
			{
				if (column.ColumnName == name)
					return column;
			}

			return null;
		}
		#endregion

		#region GetDestinationMetaFields
		/// <summary>
		/// Gets the destination meta fields.
		/// </summary>
		/// <returns></returns>
		public MetaField[] GetDestinationMetaFields()
		{
			if (this.IsNewList)
				return _newMetaFields.ToArray();

			// Combime NewMetaFields + Added MEtaFields
			List<MetaField> retVal = new List<MetaField>();

			MetaClass listMetaClass = ListManager.GetListMetaClass((int)this.ListInfoId);

			foreach (MetaField field in listMetaClass.Fields)
			{
				if (field.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					continue;
				retVal.Add(field);
			}

			retVal.AddRange(this.NewMetaFields);

			// TODO: Sort by name

			return retVal.ToArray();
		}

		/// <summary>
		/// Gets the destination meta field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public MetaField GetDestinationMetaField(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			foreach (MetaField mf in GetDestinationMetaFields())
			{
				if (mf.Name == name)
					return mf;
			}

			return null;
		}
		#endregion

		#region GetMappingRule
		/// <summary>
		/// Gets the name of the mapping element by column.
		/// </summary>
		/// <param name="sourceColumnName">Name of the source column.</param>
		/// <returns></returns>
		public MappingRule GetRuleByColumn(string sourceColumnName)
		{
			if (sourceColumnName == null)
				throw new ArgumentNullException("sourceColumnName");

			if (this.MappingDocument.Count == 0)
				return null;

			MappingElement mapping = this.MappingDocument[0];

			foreach (MappingRule map in mapping)
			{
				if (map.ColumnName == sourceColumnName)
					return map;
			}

			return null;
		}

		/// <summary>
		/// Gets the mapping element by meta field.
		/// </summary>
		/// <param name="metaFieldName">Name of the meta field.</param>
		/// <returns></returns>
		public MappingRule GetRuleByMetaField(string metaFieldName)
		{
			if (metaFieldName == null)
				throw new ArgumentNullException("metaFieldName");

			if (this.MappingDocument.Count == 0)
				return null;

			MappingElement mapping = this.MappingDocument[0];

			foreach (MappingRule map in mapping)
			{
				if (map.FieldName == metaFieldName)
					return map;
			}

			return null;
		}

		#endregion

		#region Assign Remove Mapping Rules
		/// <summary>
		/// Assigns the copy value rule.
		/// </summary>
		/// <param name="sourceColumnName">Name of the source column.</param>
		/// <param name="targetMetaFieldName">Name of the target meta field.</param>
		/// <returns></returns>
		public MappingRule AssignCopyValueRule(string sourceColumnName, string targetMetaFieldName)
		{
			if (sourceColumnName == null)
				throw new ArgumentNullException("sourceColumnName");
			if (targetMetaFieldName == null)
				throw new ArgumentNullException("targetMetaFieldName");

			if (this.MappingDocument.Count == 0)
				throw new ArgumentException("MappingDocument is empty, create MappingElement or call CreateDefaultMapping.");

			//RemoveRuleByColumn(srcColumnName);
			RemoveRuleByMetaField(targetMetaFieldName);

			MappingRule retVal = MappingRule.CreateCopyValue(sourceColumnName, targetMetaFieldName);

			MappingElement mapping = this.MappingDocument[0];
			mapping.Add(retVal);

			return retVal;
		}

		/// <summary>
		/// Assigns the default value rule.
		/// </summary>
		/// <param name="metaFieldName">Name of the meta field.</param>
		/// <returns></returns>
		public MappingRule AssignDefaultValueRule(string metaFieldName)
		{
			if (metaFieldName == null)
				throw new ArgumentNullException("metaFieldName");

			if (this.MappingDocument.Count == 0)
				throw new ArgumentException("MappingDocument is empty, create MappingElement or call CreateDefaultMapping.");

			//RemoveRuleByColumn(srcColumnName);
			RemoveRuleByMetaField(metaFieldName);

			string defaultValue = this.GetDestinationMetaField(metaFieldName).DefaultValue;

			MappingRule retVal = MappingRule.CreateDefaultValue(metaFieldName, defaultValue);

			MappingElement mapping = this.MappingDocument[0];
			mapping.Add(retVal);

			return retVal;
		}

		/// <summary>
		/// Assigns the default value rule.
		/// </summary>
		/// <param name="metaFieldName">Name of the meta field.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public MappingRule AssignDefaultValueRule(string metaFieldName, string defaultValue)
		{
			if (metaFieldName == null)
				throw new ArgumentNullException("metaFieldName");
			if (defaultValue == null)
				throw new ArgumentNullException("defaultValue");

			if (this.MappingDocument.Count == 0)
				throw new ArgumentException("MappingDocument is empty, create MappingElement or call CreateDefaultMapping.");

			//RemoveRuleByColumn(srcColumnName);
			RemoveRuleByMetaField(metaFieldName);

			MappingRule retVal = MappingRule.CreateDefaultValue(metaFieldName, defaultValue);

			MappingElement mapping = this.MappingDocument[0];
			mapping.Add(retVal);

			return retVal;
		}


		/// <summary>
		/// Removes the rule by column.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		public void RemoveRuleByColumn(string columnName)
		{
			if (columnName == null)
				throw new ArgumentNullException("columnName");

			if (this.MappingDocument.Count == 0)
				return;

			MappingElement mapping = this.MappingDocument[0];

			foreach (MappingRule map in mapping)
			{
				if (map.ColumnName == columnName)
				{
					mapping.Remove(map);
					return;
				}
			}

			return;
		}

		/// <summary>
		/// Removes the rule by meta field.
		/// </summary>
		/// <param name="metaFieldName">Name of the meta field.</param>
		public void RemoveRuleByMetaField(string metaFieldName)
		{
			if (metaFieldName == null)
				throw new ArgumentNullException("metaFieldName");

			if (this.MappingDocument.Count == 0)
				return;

			MappingElement mapping = this.MappingDocument[0];

			foreach (MappingRule map in mapping)
			{
				if (map.FieldName == metaFieldName)
				{
					mapping.Remove(map);
					return;
				}
			}

			return;
		}

		#endregion

		#region NewMetaFields Method
		/// <summary>
		/// Adds the new meta field.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="friendlyName">Name of the friendly.</param>
		/// <param name="typeName">Name of the type.</param>
		/// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		public MetaField AddNewMetaField(string name,
			string friendlyName,
			string typeName,
			bool isNullable,
			string defaultValue,
			AttributeCollection attributes)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			// Check Meta Field Name. Should be unique
			CheckMetaFieldName(name);

			MetaField mf = CreateNewMetaField(name,
				friendlyName,
				typeName,
				isNullable,
				defaultValue,
				attributes);

			this.NewMetaFields.Add(mf);

			return mf;
		}

		private void CheckMetaFieldName(string name)
		{
			foreach (MetaField mf in this.GetDestinationMetaFields())
			{
				if (mf.Name == name)
					throw new MetaFieldAlreadyExistsException();
			}
		}

		/// <summary>
		/// Removes the new meta field.
		/// </summary>
		/// <param name="name">The name.</param>
		public void RemoveNewMetaField(string name)
		{
			foreach (MetaField mf in this.NewMetaFields)
			{
				if (mf.Name == name)
				{
					this.NewMetaFields.Remove(mf);
					break;
				}
			}
		}
		#endregion

		#endregion
	}
}
