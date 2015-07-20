using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Mediachase.Ibn.Core.Business;
using MD45 = Mediachase.MetaDataPlus;
using MD47 = Mediachase.Ibn.Data;

using FieldTypeMapingValues = System.Collections.Generic.Dictionary<Mediachase.Ibn.Data.Meta.Management.MetaFieldType, System.Collections.Generic.Dictionary<int, string>>;
using Field45by47 = System.Collections.Generic.Dictionary<string, string>;
using Mediachase.Ibn.Core;


namespace Mediachase.Ibn.Converter
{
	/// <summary>
	/// Converts MetaClass from 4.5 to 4.7 and copies objects and meta view preferences
	/// !!! WARNING !!! Converts only simple fields (not references, cards, etc.)
	/// </summary>
	public class MetadataPlusToMetadataConverter
	{
		#region class ColumnInfo
		class ColumnInfo : IComparable<ColumnInfo>
		{
			private string _name;
			private int _index;
			private int _width = 150;

			public ColumnInfo(string name)
			{
				_name = name;
			}

			public string Name
			{
				get { return _name; }
			}

			public int Index
			{
				set { _index = value; }
			}

			public int Width
			{
				get { return _width; }
				set { _width = value; }
			}

			#region IComparable<ColumnInfo> Members

			public int CompareTo(ColumnInfo other)
			{
				int result;

				if (other == null)
				{
					result = -1;
				}
				else
					result = _index.CompareTo(other._index);

				return result;
			}

			#endregion
		}
		#endregion

		private MD45.Configurator.MetaClass _metaClass45;
		private MD47.Meta.Management.MetaClass _metaClass47;
		private string _fieldNamePrefix47;
		private Dictionary<int, Dictionary<string, object>> _additionalFields;


		FieldTypeMapingValues _enumValuesByType;
		Field45by47 _fieldMap;

		public MetadataPlusToMetadataConverter(MD45.Configurator.MetaClass metaClass45, MD47.Meta.Management.MetaClass metaClass47)
			: this(metaClass45, metaClass47, null, null)
		{
		}

		internal MetadataPlusToMetadataConverter(MD45.Configurator.MetaClass metaClass45, MD47.Meta.Management.MetaClass metaClass47, string fieldNamePrefix47, Dictionary<int, Dictionary<string, object>> additionalFields)
		{
			if (metaClass45 == null)
				throw new ArgumentNullException("metaClass45");
			if (metaClass47 == null)
				throw new ArgumentNullException("metaClass47");

			_metaClass45 = metaClass45;
			_metaClass47 = metaClass47;
			_fieldNamePrefix47 = fieldNamePrefix47;
			_additionalFields = additionalFields;
		}

		public void CopyFields()
		{
			if (_fieldMap == null)
			{
				_fieldMap = new Field45by47();
				_enumValuesByType = new FieldTypeMapingValues();

				foreach (MD45.Configurator.MetaField field45 in _metaClass45.MetaFields)
				{
					CopyFieldFrom45(_metaClass47, _fieldNamePrefix47, field45, _fieldMap, _enumValuesByType);
				}
			}
		}

		public void CopyEntities<T>(Dictionary<int, T> entitiesById) where T : EntityObject
		{
			if (!_metaClass45.IsAbstract)
			{
				CopyFields();
				CreateEntitiesFrom45(entitiesById);
			}
		}

		public void CopyObjects(Dictionary<int, MD47.Meta.MetaObject> objectsById)
		{
			if (!_metaClass45.IsAbstract)
			{
				CopyFields();
				CreateObjectsFrom45(objectsById);
			}
		}

		public void CopyViewPreferences()
		{
			CopyFields();
			CopyViewPreferencesFrom45();
		}


		#region private void CreateEntitiesFrom45<T>(Dictionary<int, T> entitiesById) where T : EntityObject
		private void CreateEntitiesFrom45<T>(Dictionary<int, T> entitiesById) where T : EntityObject
		{
			Dictionary<int, Dictionary<string, object>> objects = GetObjects();

			foreach (int object45Id in objects.Keys)
			{
				T entity = BusinessManager.InitializeEntity<T>(_metaClass47.Name);
				if (entitiesById != null)
				{
					entitiesById.Add(object45Id, entity);
				}

				Dictionary<string, object> values = objects[object45Id];
				foreach (string name in values.Keys)
				{
					entity.Properties[name].Value = values[name];
				}

				entity.PrimaryKeyId = BusinessManager.Create(entity);
			}
		}
		#endregion
		#region private void CreateObjectsFrom45(Dictionary<int, MD47.Meta.MetaObject> objectsById)
		/// <returns>Objects count.</returns>
		private void CreateObjectsFrom45(Dictionary<int, MD47.Meta.MetaObject> objectsById)
		{
			Dictionary<int, Dictionary<string, object>> objects = GetObjects();
			foreach (int object45Id in objects.Keys)
			{
				MD47.Meta.MetaObject object47 = new MD47.Meta.MetaObject(_metaClass47);
				if (objectsById != null)
				{
					objectsById.Add(object45Id, object47);
				}

				Dictionary<string, object> values = objects[object45Id];
				foreach (string name in values.Keys)
				{
					object47.Properties[name].Value = values[name];
				}

				try
				{
					object47.Save();
				}
				catch (MD47.Meta.MetaObjectValidationException ex)
				{
					StringBuilder sb = new StringBuilder();
					foreach (MD47.IValidator validator in ex.InvalidValidator)
					{
						sb.Append("\n");
						sb.Append(validator.ErrorMessage);
					}
					throw new MetadataConverterException(sb.ToString(), ex);
				}
			}
		}
		#endregion

		#region private Dictionary<int, Dictionary<string, object>> GetObjects()
		private Dictionary<int, Dictionary<string, object>> GetObjects()
		{
			Dictionary<int, Dictionary<string, object>> objects = new Dictionary<int, Dictionary<string, object>>();

			// 2008-08-08: Fix. There is already an open DataReader associated with this Command which must be closed first.
			foreach (MD45.Configurator.MetaField metaField in _metaClass45.MetaFields)
			{
				metaField.LoadDictionary();
			}

			MD45.MetaObject[] objects45 = MD45.MetaObject.GetList(_metaClass45);

			foreach (MD45.MetaObject object45 in objects45)
			{
				int object45Id = object45.Id;
				if (_additionalFields == null || _additionalFields.ContainsKey(object45Id))
				{
					Dictionary<string, object> values = new Dictionary<string, object>();
					objects.Add(object45Id, values);

					foreach (MD47.Meta.Management.MetaField field47 in _metaClass47.Fields)
					{
						bool fieldPresent = true;
						object value = null;

						// Skip ReadOnly fields
						if (!field47.ReadOnly)
						{
							GetFieldValue(object45, field47, ref fieldPresent, ref value);

							if (fieldPresent)
							{
								if (field47.IsNullable || value != null)
								{
									values[field47.Name] = value;
								}
							}
						}
					}
				}
			}

			// Set additional values
			if (_additionalFields != null)
			{
				foreach (int id in _additionalFields.Keys)
				{
					Dictionary<string, object> values;
					if (objects.ContainsKey(id))
					{
						values = objects[id];
					}
					else
					{
						values = new Dictionary<string, object>();
						objects.Add(id, values);
					}

					Dictionary<string, object> additionalValues = _additionalFields[id];
					foreach (string name in additionalValues.Keys)
					{
						values[name] = additionalValues[name];
					}
				}
			}

			return objects;
		}
		#endregion
		#region private void GetFieldValue(MD45.MetaObject object45, MD47.Meta.Management.MetaField field47, ref bool fieldPresent, ref object value)
		private void GetFieldValue(MD45.MetaObject object45, MD47.Meta.Management.MetaField field47, ref bool fieldPresent, ref object value)
		{
			string field47Name = field47.Name;

			if (_fieldMap.ContainsKey(field47Name))
			{
				fieldPresent = true;
				string field45Name = _fieldMap[field47Name];

				if (field47.IsEnum || field47.IsMultivalueEnum)
				{
					if (object45[field45Name] != null)
					{
						Dictionary<int, string> enumValues;
						//Try find mapping id for this FieldType
						if (!_enumValuesByType.TryGetValue(field47.GetMetaType(), out enumValues))
							throw new ArgumentException("Not mapped enum item");

						if (field47.IsMultivalueEnum)
						{
							List<string> valueList = new List<string>();
							foreach (MD45.Configurator.MetaDictionaryItem enumItem in (MD45.Configurator.MetaDictionaryItem[])object45[field45Name])
							{
								valueList.Add(enumItem.Value);
							}

							if (valueList.Count > 0)
								value = valueList.ToArray();
						}
						else
						{
							value = ((MD45.Configurator.MetaDictionaryItem)object45[field45Name]).Value;
						}
					}
				}
				else if (field47.GetMetaType().McDataType == MD47.Meta.Management.McDataType.File)
				{
					MD45.MetaFile metaFile45 = object45[field45Name] as MD45.MetaFile;

					if (metaFile45 != null)
					{
						//Begin copy file to 47
						if (metaFile45.Buffer != null)
						{
							using (MemoryStream stream = new MemoryStream(metaFile45.Buffer))
							{
								value = new MD47.Meta.FileInfo(metaFile45.Name, stream);
							}
						}
						else
							value = new MD47.Meta.FileInfo(metaFile45.Name);
					}
				}
				else
				{
					switch (field47Name)
					{
						case "Created":
							value = object45.Created;
							break;
						case "CreatorId":
							value = object45.CreatorId;
							break;
						case "Modified":
							value = object45.Modified;
							break;
						case "ModifierId":
							value = object45.ModifierId;
							break;
						default:
							value = object45[field45Name];
							break;
					}
				}
			}
			else
			{
				fieldPresent = false;
			}
		}
		#endregion

		#region private static void CopyFieldFrom45(MD47.Meta.Management.MetaClass metaClass47, string fieldNamePrefix47, MD45.Configurator.MetaField field45, Field47by45 fieldMap, FieldTypeMapingValues enumValuesByType)
		private static void CopyFieldFrom45(MD47.Meta.Management.MetaClass metaClass47, string fieldNamePrefix47, MD45.Configurator.MetaField field45, Field45by47 fieldMap, FieldTypeMapingValues enumValuesByType)
		{
			MD47.Meta.Management.MetaField field47 = null;

			if (!field45.IsSystem)
			{
				using (MD47.Meta.Management.MetaFieldBuilder builder = new MD47.Meta.Management.MetaFieldBuilder(metaClass47))
				{
					string name = fieldNamePrefix47 + field45.Name;
					string friendlyName = field45.FriendlyName;
					bool allowNulls = field45.AllowNulls;

					switch (field45.DataType)
					{
						case MD45.Configurator.MetaDataType.UniqueIdentifier:
							field47 = builder.CreateGuid(name, friendlyName, allowNulls);
							break;
						case MD45.Configurator.MetaDataType.Money:
						case MD45.Configurator.MetaDataType.SmallMoney:
							field47 = builder.CreateCurrency(name, friendlyName, allowNulls, 0, true);
							break;
						case MD45.Configurator.MetaDataType.Float:
						case MD45.Configurator.MetaDataType.Numeric:
							field47 = builder.CreateFloat(name, friendlyName, allowNulls, 0);
							break;
						case MD45.Configurator.MetaDataType.File:
						case MD45.Configurator.MetaDataType.ImageFile:
							field47 = builder.CreateFile(name, friendlyName, allowNulls, string.Empty);
							break;
						case MD45.Configurator.MetaDataType.Integer:
						case MD45.Configurator.MetaDataType.Decimal:
						case MD45.Configurator.MetaDataType.Int:
						case MD45.Configurator.MetaDataType.BigInt:
						case MD45.Configurator.MetaDataType.Real:
						case MD45.Configurator.MetaDataType.SmallInt:
						case MD45.Configurator.MetaDataType.TinyInt:
							field47 = builder.CreateInteger(name, friendlyName, allowNulls, 0);
							break;
						case MD45.Configurator.MetaDataType.Boolean:
						case MD45.Configurator.MetaDataType.Bit:
							field47 = builder.CreateCheckBoxBoolean(name, friendlyName, allowNulls, false, friendlyName);
							break;
						case MD45.Configurator.MetaDataType.Date:
						case MD45.Configurator.MetaDataType.DateTime:
						case MD45.Configurator.MetaDataType.SmallDateTime:
						case MD45.Configurator.MetaDataType.Timestamp:
							field47 = builder.CreateDate(name, friendlyName, allowNulls);
							break;
						case MD45.Configurator.MetaDataType.Email:
							field47 = builder.CreateEmail(name, friendlyName, allowNulls, field45.Length, false);
							break;
						case MD45.Configurator.MetaDataType.Url:
							field47 = builder.CreateUrl(name, friendlyName, allowNulls, field45.Length, false, String.Empty);
							break;
						case MD45.Configurator.MetaDataType.ShortString:
						case MD45.Configurator.MetaDataType.Char:
						case MD45.Configurator.MetaDataType.NChar:
						case MD45.Configurator.MetaDataType.NVarChar:
						case MD45.Configurator.MetaDataType.VarChar:
							field47 = builder.CreateText(name, friendlyName, allowNulls, field45.Length, false);
							break;
						case MD45.Configurator.MetaDataType.LongHtmlString:
							field47 = builder.CreateHtml(name, friendlyName, allowNulls);
							break;
						case MD45.Configurator.MetaDataType.LongString:
						case MD45.Configurator.MetaDataType.Binary:
						case MD45.Configurator.MetaDataType.Image:
						case MD45.Configurator.MetaDataType.NText:
						case MD45.Configurator.MetaDataType.Text:
						case MD45.Configurator.MetaDataType.VarBinary:
							field47 = builder.CreateLongText(name, friendlyName, allowNulls);
							break;
						case MD45.Configurator.MetaDataType.DictionarySingleValue:
						case MD45.Configurator.MetaDataType.EnumSingleValue:
						case MD45.Configurator.MetaDataType.DictionaryMultivalue:
						case MD45.Configurator.MetaDataType.EnumMultivalue:
							{
								bool isMultiValue = (field45.DataType == MD45.Configurator.MetaDataType.DictionaryMultivalue) || (field45.DataType == MD45.Configurator.MetaDataType.EnumMultivalue);
								MD45.Configurator.MetaDictionary dictionary45 = MD45.Configurator.MetaDictionary.Load(field45.Id);

								MD47.Meta.Management.MetaFieldType enum47 = null;

								// Try to find existing enum type
								foreach (MD47.Meta.Management.MetaFieldType fieldType47 in MD47.DataContext.Current.MetaModel.RegisteredTypes)
								{
									if (fieldType47.Name == name)
									{
										if (fieldType47.McDataType == MD47.Meta.Management.McDataType.Enum)
										{
											enum47 = fieldType47;
										}
										else
										{
											//Do not allow identical type names for Enum and Basic types
											name = name + "_" + builder.MetaClass.Name;
										}
										break;
									}
								}

								if (enum47 == null)
									enum47 = MD47.Meta.Management.MetaEnum.Create(name, friendlyName, isMultiValue);

								Dictionary<int, string> enumValues47By45 = CreateEnumType47FromMetaDictionary(enum47, dictionary45);

								enumValuesByType.Add(enum47, enumValues47By45);

								string defaultValue = string.Empty;
								if (!allowNulls)
								{
									MD47.Meta.MetaEnumItem[] items = MD47.Meta.Management.MetaEnum.GetItems(enum47);
									if (items != null && items.Length > 0)
										defaultValue = items[0].Handle.ToString(CultureInfo.InvariantCulture);
								}

								field47 = builder.CreateEnumField(name, friendlyName, enum47.Name, allowNulls, defaultValue, true);
							}
							break;
						case MD45.Configurator.MetaDataType.StringDictionary:
							//TODO:
							break;
						default:
							break;
					}
				}
			}

			if (field47 != null)
			{
				fieldMap.Add(field47.Name, field45.Name);
			}
		}
		#endregion
		#region private static Dictionary<int, string> CreateEnumType47FromMetaDictionary(MD47.Meta.Management.MetaFieldType enumType, MD45.Configurator.MetaDictionary dictionary45)
		/// <summary>
		/// Fills the Enum with values from dictionary.
		/// </summary>
		/// <returns></returns> 
		private static Dictionary<int, string> CreateEnumType47FromMetaDictionary(MD47.Meta.Management.MetaFieldType enumType, MD45.Configurator.MetaDictionary dictionary45)
		{
			Dictionary<int, string> enumValues47By45 = new Dictionary<int, string>();

			if (dictionary45 != null)
			{
				foreach (MD45.Configurator.MetaDictionaryItem item45 in dictionary45)
				{
					bool found = false;
					int value45 = item45.Id;
					string value47 = item45.Value;

					foreach (MD47.Meta.MetaEnumItem item47 in MD47.Meta.Management.MetaEnum.GetItems(enumType))
					{
						if (item47.Name == value47)
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						MD47.Meta.Management.MetaEnum.AddItem(enumType, value47, item45.Index);
					}

					enumValues47By45.Add(value45, value47);
				}
			}

			return enumValues47By45;
		}
		#endregion

		#region private void CopyViewPreferencesFrom45()
		private void CopyViewPreferencesFrom45()
		{
			// Retrieve meta view for list 
			ListViewProfile profile = null;

			ListViewProfile[] views = ListViewProfile.GetSystemProfiles(_metaClass47.Name, "EntityList");
			if (views.Length > 0)
			{
				profile = views[0];
			}

			if (profile == null)
			{
				//Create meta view
				profile = new ListViewProfile();
				profile.Id = Guid.NewGuid().ToString("D");
				profile.Name = "{IbnFramework.ListInfo:lvpGeneralView}";
				profile.IsPublic = true;
				//defaultView = MD47.DataContext.Current.MetaModel.CreateMetaView(_metaClass47.Name, _metaClass47.Name, _metaClass47.FriendlyName, new string[] { });
			}

			//Try to load default meta view preference
			//Mediachase.Ibn.Core.McMetaViewPreference defaultPreference = Mediachase.Ibn.Core.UserMetaViewPreference.LoadDefault(defaultView);
			//if (defaultPreference == null)
			//{
			//    // Create preference
			//    Mediachase.Ibn.Core.McMetaViewPreference.CreateDefaultPreference(defaultView);
			//    defaultPreference = Mediachase.Ibn.Core.UserMetaViewPreference.LoadDefault(defaultView);
			//}

			Dictionary<string, MD47.Meta.Management.MetaField> fields47 = new Dictionary<string, MD47.Meta.Management.MetaField>();
			foreach (MD47.Meta.Management.MetaField field47 in _metaClass47.Fields)
			{
				fields47.Add(field47.Name, field47);
			}

			List<ColumnInfo> columns = new List<ColumnInfo>();

			foreach (MD45.Configurator.MetaField field45 in _metaClass45.MetaFields)
			{
				if (field45.Enabled)
				{
					string fieldName47 = GetFieldName47ByFieldName45(field45.Name);
					if (fields47.ContainsKey(fieldName47))
					{
						ColumnInfo column = new ColumnInfo(fieldName47);

						column.Index = field45.Weight;
						if (field45.Weight != 0)
						{
							column.Width = field45.Width;
						}
						else
						{
							column.Width = 150;
						}

						columns.Add(column);
					}
				}
			}

			columns.Sort();

			foreach (ColumnInfo column in columns)
			{
				profile.FieldSet.Add(column.Name);
				profile.ColumnsUI.Add(new ColumnProperties(column.Name, column.Width.ToString(CultureInfo.InvariantCulture), string.Empty));
			}

			ListViewProfile.SaveSystemProfile(_metaClass47.Name, "EntityList", -1, profile);
		}
		#endregion

		#region private static string GetFieldName47ByFieldName45(string fieldName45)
		private static string GetFieldName47ByFieldName45(string fieldName45)
		{
			string retVal = fieldName45;

			switch (fieldName45)
			{
				case "CreationDate":
					retVal = "Created";
					break;
				case "LastSavedDate":
					retVal = "Modified";
					break;
				case "LastEditorId":
					retVal = "ModifierId";
					break;
			}

			return retVal;
		}
		#endregion
	}
}
