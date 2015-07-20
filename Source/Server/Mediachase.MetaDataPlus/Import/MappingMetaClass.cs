using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.MetaDataPlus.Import
{
	/// <summary>
	/// Summary description for MappingMetaObject.
	/// </summary>
	public class MetaObjectResolverEventArgs : EventArgs
	{
		private object _value = null;
		private string _destFieldName = null;
		private MetaDataType _destType;
		//int _RowIndex;
		private ArrayList _warnings;

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		//public int RowIndex
		//{
		//    get
		//    {
		//        return _RowIndex;
		//    }
		//}

		public MdpImportWarning[] Warnings
		{
			get
			{
				return (MdpImportWarning[])_warnings.ToArray(typeof(MdpImportWarning));
			}
		}

		public void AddWarning(MdpImportWarning message)
		{
			if (_warnings == null)
				_warnings = new ArrayList();

			_warnings.Add(message);
		}

		public string DestFieldName
		{
			get
			{
				return _destFieldName;
			}
		}

		public MetaDataType DestType
		{
			get
			{
				return _destType;
			}
		}

		public MetaObjectResolverEventArgs(object value, string destFieldName, MetaDataType destType)
		{
			_value = value;
			_destFieldName = destFieldName;
			_destType = destType;
		}
	}

	public delegate void ConvertToFileHandle(object sender, MetaObjectResolverEventArgs e);
	public delegate void ConvertToDictionaryHandle(object sender, MetaObjectResolverEventArgs e);

	public abstract class MappingMetaClass
	{
		private string _innerMetaClassName;

		private ColumnInfo[] _systemColumns;
		private ColumnInfo[] _userColumns;

		public event ConvertToFileHandle OnConvertToFile;
		public event ConvertToDictionaryHandle OnConvertToDictionary;

		protected MappingMetaClass()
		{
		}

		protected string InnerMetaClassName
		{
			get { return _innerMetaClassName; }
			set { _innerMetaClassName = value; }
		}

		public virtual ColumnInfo[] ColumnInfos
		{
			get
			{
				ArrayList array = new ArrayList();

				array.AddRange(SystemColumnInfos);
				array.AddRange(UserColumnInfos);

				return (ColumnInfo[])array.ToArray(typeof(ColumnInfo));
			}
		}

		public virtual ColumnInfo[] SystemColumnInfos
		{
			get
			{
				if (_systemColumns == null)
				{
					ArrayList list = new ArrayList();

					FillSystemColumnInfo(list);

					_systemColumns = (ColumnInfo[])list.ToArray(typeof(ColumnInfo));
				}
				return _systemColumns;
			}
		}

		public virtual ColumnInfo[] UserColumnInfos
		{
			get
			{
				if (_userColumns == null)
				{
					ArrayList array = new ArrayList();

					FillUserColumnInfo(array);

					_userColumns = (ColumnInfo[])array.ToArray(typeof(ColumnInfo));
				}
				return _userColumns;
			}
		}

		public ColumnInfo GetColumnInfo(string name)
		{
			foreach (ColumnInfo col in ColumnInfos)
			{
				if (col.FieldName == name)
					return col;
			}
			return null;
		}

		protected virtual void FillSystemColumnInfo(ArrayList array)
		{
		}

		protected virtual void FillUserColumnInfo(ArrayList array)
		{
			MetaClass metaClass = MetaClass.Load(_innerMetaClassName);
			foreach (MetaField field in metaClass.UserMetaFields)
			{
				array.Add(new ColumnInfo(field, FillTypes.CopyValue | FillTypes.Default | FillTypes.Custom));
			}
		}

		protected virtual int CreateSystemRow(FillDataMode mode, params object[] items)
		{
			return -1;
		}

		protected virtual void CreateUserRow(MetaObject metaObject, Rule rule, params object[] items)
		{
			for (int Index = 0; Index < this.UserColumnInfos.Length; Index++)
			{
				RuleItem mapping = rule[this.UserColumnInfos[Index].Field.Name];

				if (mapping == null || mapping.FillType != FillTypes.Default)
				{
					metaObject[this.UserColumnInfos[Index].Field.Name] = items[Index];
				}
			}
		}

		/*		
				private MetaDictionaryItem findDictionaryItem(string value, string DestFieldName)
				{
					MetaField		field = MetaField.Load(DestFieldName);
					MetaDictionary	dictionary = field.Dictionary;
			
					foreach (MetaDictionaryItem item in dictionary)
					{
						if (String.Compare(value, item.Value, true)==0)
							return item;
					}
					return null;
				}

				protected virtual object ConvertToDictionary(object value, MetaDataType DestType, string DestFieldName)
				{
					string[] values = value.ToString().Split(new char[] {','});
					if (DestType==MetaDataType.DictionaryMultivalue || DestType==MetaDataType.EnumMultivalue)
					{
						value = new MetaDictionaryItem[values.Length];
						for (int i = 0 ; i < values.Length ; i++)
						{
							((MetaDictionaryItem[])value)[i] = findDictionaryItem(values[i].Trim(), DestFieldName);
							if (((MetaDictionaryItem[])value)[i]==null)
								throw new Mediachase.MetaDataPlus.Import.InvalidCastException(DestType.ToString(), value.GetType().ToString());
						}
					}
					else
					{
						value = findDictionaryItem(value.ToString().Trim(), DestFieldName);
						if (value==null)
							throw new Mediachase.MetaDataPlus.Import.InvalidCastException(DestType.ToString(), value.GetType().ToString());
					}
					return value;
				}
		*/
		protected virtual object ConvertToDictionary(object value, MetaDataType destType, string destFieldName, int rowIndex, out MdpImportWarning[] warnings)
		{
			warnings = null;
			if (OnConvertToDictionary != null)
			{
				MetaObjectResolverEventArgs e = new MetaObjectResolverEventArgs(value, destFieldName, destType);

				OnConvertToDictionary(this, e);

				warnings = e.Warnings;

				return e.Value;
			}
			else return null;
		}

		protected virtual object ConvertToFile(object value, MetaDataType dest, string destFieldName, int rowIndex, out MdpImportWarning[] warnings)
		{
			warnings = null;
			if (OnConvertToFile != null)
			{
				MetaObjectResolverEventArgs e = new MetaObjectResolverEventArgs(value, destFieldName, dest);

				OnConvertToFile(this, e);

				warnings = e.Warnings;

				return e.Value;
			}
			else return null;
		}

		protected virtual object ConvertDataType(Type source, MetaDataType destination, object value, string destFieldName, bool allowNull, int rowIndex, out MdpImportWarning[] warnings)
		{
			warnings = null;
			switch (destination)
			{
				case MetaDataType.BigInt:
					return Convert.ChangeType(value, typeof(Int64));
				case MetaDataType.Binary:
				case MetaDataType.Image:
				case MetaDataType.VarBinary:
					return Convert.ChangeType(value, typeof(byte[]));	// TODO: does it work?
				case MetaDataType.Bit:
				case MetaDataType.Boolean:
					switch (value.ToString().ToLower())
					{
						case "true":
							return true;
						case "false":
							return false;
						default:
							try
							{
								return (int.Parse(value.ToString()) != 0);
							}
							catch
							{
							}
							return false;
					}
				case MetaDataType.Char:
				case MetaDataType.NChar:
				case MetaDataType.NText:
				case MetaDataType.NVarChar:
				case MetaDataType.UniqueIdentifier:
				case MetaDataType.Text:
				case MetaDataType.VarChar:
				case MetaDataType.Sysname:
				case MetaDataType.ShortString:
				case MetaDataType.LongString:
				case MetaDataType.Url:
				case MetaDataType.Email:
				case MetaDataType.LongHtmlString:
					return value.ToString();
				case MetaDataType.DateTime:
					return Convert.ChangeType(value, typeof(DateTime));
				case MetaDataType.Decimal:
				case MetaDataType.Numeric:
					return Convert.ChangeType(value, typeof(Decimal));
				case MetaDataType.Money:
				case MetaDataType.SmallMoney:							// TODO:?? + temporal code for currency symbol
					if (value is string)
						return Convert.ChangeType(value.ToString().TrimStart('$'), typeof(Decimal));
					else
						return Convert.ChangeType(value, typeof(Decimal));
				case MetaDataType.Float:
					return Convert.ChangeType(value, typeof(Double));
				case MetaDataType.Int:
				case MetaDataType.Integer:
					return Convert.ChangeType(value, typeof(Int32));
				case MetaDataType.Real:
					return Convert.ChangeType(value, typeof(Single));
				case MetaDataType.SmallDateTime:
					return Convert.ChangeType(value, typeof(DateTime));		// TODO:??
				case MetaDataType.SmallInt:
					return Convert.ChangeType(value, typeof(Int16));
				case MetaDataType.TinyInt:
					return Convert.ChangeType(value, typeof(Byte));
				case MetaDataType.Date:
					return Convert.ChangeType(value, typeof(DateTime));
				case MetaDataType.DictionaryMultivalue:
				case MetaDataType.EnumMultivalue:
					value = ConvertToDictionary(value, destination, destFieldName, rowIndex, out warnings);
					if (value == null && !allowNull)
						throw new Mediachase.MetaDataPlus.Import.InvalidCastException(destination.ToString(), source.ToString());
					break;
				case MetaDataType.DictionarySingleValue:
				case MetaDataType.EnumSingleValue:
					value = ConvertToDictionary(value, destination, destFieldName, rowIndex, out warnings);
					if (value == null && !allowNull)
						throw new Mediachase.MetaDataPlus.Import.InvalidCastException(destination.ToString(), source.ToString());
					break;
				case MetaDataType.File:
				case MetaDataType.ImageFile:
					value = ConvertToFile(value, destination, destFieldName, rowIndex, out warnings);
					if (value == null && !allowNull)
						throw new Mediachase.MetaDataPlus.Import.InvalidCastException(destination.ToString(), source.ToString());
					break;
				default:
					throw new Mediachase.MetaDataPlus.Import.InvalidCastException(destination.ToString(), source.ToString());

			}
			return value;
		}

		public virtual Rule CreateClassRule()
		{
			return new Rule(_innerMetaClassName);
		}

		#region FillData
		private object[] FillItemList(ColumnInfo[] columns, DataRow row, int rowIndex, Rule rule, ArrayList warningList)
		{
			ArrayList list = new ArrayList(columns.Length);

			for (int Index = 0; Index < columns.Length; Index++)
			{
				ColumnInfo Col = columns[Index];
				RuleItem ColMapping = rule[Col.Field.Name];
				object value = null;
				try
				{
					if (ColMapping != null && ColMapping.FillType != FillTypes.None)
					{
						if ((Col.SupportedFillType & ColMapping.FillType) == ColMapping.FillType)
						{
							MdpImportWarning[] warnings = null;
							switch (ColMapping.FillType)
							{
								case FillTypes.CopyValue:
									value = row[ColMapping.SrcColumnName];
									if (value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()))
									{
										value = ConvertDataType(ColMapping.SrcColumnType, ColMapping.DestColumnType, value, ColMapping.DestColumnName, Col.Field.AllowNulls, rowIndex, out warnings);
									}
									else value = null;
									break;
								case FillTypes.Custom:
									value = ColMapping.CustomValue;
									value = ConvertDataType(typeof(string), ColMapping.DestColumnType, value, ColMapping.DestColumnName, Col.Field.AllowNulls, rowIndex, out warnings);
									break;
								case FillTypes.Default:
									value = null;
									break;
							}
							if (warnings != null)
								warningList.AddRange(warnings);

							if (Col.IsSystemDictionary && value != null)
								value = value.ToString();
						}
					}
				}
				catch (Exception e)
				{
					throw new MdpImportException(e.Message, row, rowIndex, ColMapping, Col.Field, value);
				}
				list.Add(value);
			}
			return list.ToArray();
		}


		public virtual FillResult FillData(FillDataMode mode, DataTable rawData, Rule rule)
		{
			return FillData(mode, rawData, rule, -1, DateTime.Now);
		}

		public virtual FillResult FillData(FillDataMode mode, DataTable rawData, Rule rule, int modifierId, DateTime modified)
		{
			if (rawData == null)
				throw new ArgumentNullException("rawData");

			FillResult retVal = new FillResult(rawData.Rows.Count);

			Validate(rule, rawData);

			int RowIndex = 0;
			foreach (DataRow Row in rawData.Rows)
			{
				try
				{
					int ObjectId = CreateSystemRow(mode, FillItemList(SystemColumnInfos, Row, RowIndex, rule, retVal.warningList));

					if (UserColumnInfos.Length != 0)
					{
						MetaObject Object = MetaObject.Load(ObjectId, _innerMetaClassName, modifierId, modified);
						if (Object == null)
						{
							Object = MetaObject.NewObject(ObjectId, _innerMetaClassName, modifierId, modified);
						}
						CreateUserRow(Object, rule, FillItemList(UserColumnInfos, Row, RowIndex, rule, retVal.warningList));

						Object.AcceptChanges();
					}
					else if (ObjectId == -1)
						throw new IdleOperationException();

					retVal.SuccessfulRow();
				}
				catch (AlreadyExistException)
				{
				}
				catch (MdpImportException ex)
				{
					retVal.ErrorRow();
					ex.setRowInfo(Row, RowIndex);
					retVal.ErrorException(ex);
				}
				catch (Exception ex)
				{
					retVal.ErrorRow();
					retVal.ErrorException(ex);
				}
				RowIndex++;
			}
			return retVal;
		}

		public virtual FillResult FillData(FillDataMode mode, DataTable rawData, Rule rule, SqlTransaction tran)
		{
			return FillData(mode, rawData, rule, -1, DateTime.Now, tran);
		}

		public virtual FillResult FillData(FillDataMode mode, DataTable rawData, Rule rule, int modifierId, DateTime modified, SqlTransaction tran)
		{
			MetaDataContext.Current.Transaction = tran;

			FillResult retVal = null;
			try
			{
				retVal = FillData(mode, rawData, rule, modifierId, modified);
			}
			finally
			{
				MetaDataContext.Current.Transaction = null;
			}

			return retVal;
		}
		public virtual FillResult FillData(FillDataMode mode, DataTable rawData, Rule rule, int modifierId, DateTime modified, int maximumErrors)
		{
			MetaDataContext.Current.BeginTransaction();
			try
			{
				FillResult retVal = FillData(mode, rawData, rule, modifierId, modified);
				if (maximumErrors == -1)
					MetaDataContext.Current.Commit();
				else if (retVal.ErrorRows <= maximumErrors)
					MetaDataContext.Current.Commit();
				else MetaDataContext.Current.Rollback();

				return retVal;
			}
			catch
			{
				MetaDataContext.Current.Rollback();
				throw;
			}

		}
		#endregion

		public virtual void Validate(Rule rule, DataTable rawData)
		{
			if (rule == null)
				throw new ArgumentNullException("rule");

			if (rule._innerClassName != _innerMetaClassName)
			{
				throw new NotMatchingRuleException();
			}
			/*			
						ArrayList missingColumns = new ArrayList();
						foreach(DataColumn col in RawData.Columns)
						{
							if (Mapping.GetBySrcColumnName(col.ColumnName)==null)
							{
								missingColumns.Add(col.ColumnName);
							}
						}
						if (missingColumns.Count > 0) 
						{
							throw new MissingMappingRule((string[])missingColumns.ToArray(typeof(string)));
						}
			*/
		}
	}
}
