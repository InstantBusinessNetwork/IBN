using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;

using Mediachase.MetaDataPlus.Common;
using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.MetaDataPlus
{
	/// <summary>
	/// Summary description for MetaObject.
	/// </summary>
	public class MetaObject
	{
		private int _objectId = -1;
		private MetaObjectState _state = MetaObjectState.Added;
		private MetaClass _metaClass;

		private int _creatorId = -1;
		private DateTime _created = DateTime.Now;

		private int _modifierId = -1;
		private DateTime _modified = DateTime.Now;

		private Hashtable _fieldStorage = new Hashtable();

		#region Private Methods
		void LoadMetaFields()
		{
			foreach (MetaField field in this.MetaClass.MetaFields)
			{
				if (field.IsSystem)
					continue;
				_fieldStorage[field.Name] = null;
			}
		}

		void LoadMetaFields2()
		{
			foreach (MetaField field in this.MetaClass.MetaFields)
			{
				if (field.IsSystem)
					continue;

				object FieldValue = this[field.Name];

				if (FieldValue != null)
				{
					if (field.DataType == MetaDataType.DictionarySingleValue ||
						field.DataType == MetaDataType.EnumSingleValue)
					{
						//bool bFound = false;
						//foreach (MetaDictionaryItem DicItem in field.Dictionary)
						//{
						//    if (DicItem.Id == (int)FieldValue)
						//    {
						//        bFound = true;
						//        FieldValue = DicItem;
						//        break;
						//    }
						//}

						//if (!bFound)
						//{
						//    System.Diagnostics.Trace.WriteLine(string.Format("Couldn't find the MetaDictionaryItem (Id = {0}).", (int)FieldValue), "Mediachase.MetaDataPlus.MetaObject");
						//    FieldValue = null;
						//}
					}
					else if (field.DataType == MetaDataType.DictionaryMultivalue ||
						field.DataType == MetaDataType.EnumMultivalue)
					{
						int MetaKey = (int)FieldValue;

						// TODO: Move it in to the current transaction [12/1/2004]
						using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMultivalueDictionary"),
								  new SqlParameter("@MetaKey", MetaKey)))
						{
							ArrayList MultuValues = new ArrayList();

							while (readerValue.Read())
							{
								foreach (MetaDictionaryItem DicItem in field.Dictionary)
								{
									if (DicItem.Id == (int)readerValue["MetaDictionaryId"])
									{
										MultuValues.Add(DicItem);
										break;
									}
								}
							}
							readerValue.Close();

							FieldValue = (MetaDictionaryItem[])MultuValues.ToArray(typeof(MetaDictionaryItem));
						}
					}
					else if (field.DataType == MetaDataType.File || field.DataType == MetaDataType.ImageFile)
					{
						int MetaKey = (int)FieldValue;

						// TODO: Move it in to the current transaction [12/1/2004]
						using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFile"),
								  new SqlParameter("@MetaKey", MetaKey)))
						{
							if (readerValue.Read())
							{
								MetaFile file = new MetaFile(readerValue);

								FieldValue = file;
							}
							else
								FieldValue = null;

							readerValue.Close();
						}

					}
					else if (field.DataType == MetaDataType.StringDictionary)
					{
						int MetaKey = (int)FieldValue;

						// TODO: Move it in to the current transaction [12/1/2004]
						using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaStringDictionary"),
								  new SqlParameter("@MetaKey", MetaKey)))
						{
							if (readerValue.Read())
							{
								MetaStringDictionary dic = new MetaStringDictionary();
								dic.LoadDictionary(readerValue);
								FieldValue = dic;
							}
							else
								FieldValue = null;

							readerValue.Close();
						}
					}
					else if (field.DataType == MetaDataType.MetaObject)
					{
						using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaObjectValue"),
								  new SqlParameter("@MetaKey", (int)FieldValue)))
						{
							int MetaClassId = -1;
							int MetaObjectId = -1;

							if (readerValue.Read())
							{
								MetaClassId = (int)readerValue["MetaClassId"];
								MetaObjectId = (int)readerValue["MetaObjectId"];

								readerValue.Close();

								try
								{
									FieldValue = MetaObject.Load(MetaObjectId, MetaClassId);
								}
								catch (Exception ex)
								{
									FieldValue = null;
									System.Diagnostics.Trace.WriteLine(ex, "Load MetaDataType.MetaObject");
								}
							}
							else
							{
								FieldValue = null;
								readerValue.Close();
							}
						}

					}
				}

				_fieldStorage[field.Name] = FieldValue;
			}

			this._state = MetaObjectState.Unchanged;
		}


		void LoadMetaFields(SqlDataReader reader)
		{
			_creatorId = (int)SqlHelper.DBNull2Null(reader["CreatorId"], -1);
			_created = (DateTime)SqlHelper.DBNull2Null(reader["Created"], DateTime.Now);

			_modifierId = (int)SqlHelper.DBNull2Null(reader["ModifierId"], -1);
			_modified = (DateTime)SqlHelper.DBNull2Null(reader["Modified"], DateTime.Now);

			foreach (MetaField field in this.MetaClass.MetaFields)
			{
				if (field.IsSystem)
					continue;

				object FieldValue = SqlHelper.DBNull2Null(reader[field.Name]);

				if (FieldValue != null)
				{
					if (field.DataType == MetaDataType.DictionarySingleValue ||
						field.DataType == MetaDataType.EnumSingleValue)
					{
						bool bFound = false;
						foreach (MetaDictionaryItem DicItem in field.Dictionary)
						{
							if (DicItem.Id == (int)FieldValue)
							{
								bFound = true;
								FieldValue = DicItem;
								break;
							}
						}

						if (!bFound)
						{
							System.Diagnostics.Trace.WriteLine(string.Format("Couldn't find the MetaDictionaryItem (Id = {0}).", (int)FieldValue), "Mediachase.MetaDataPlus.MetaObject");
							FieldValue = null;
						}
					}
					else if (field.DataType == MetaDataType.DictionaryMultivalue ||
						field.DataType == MetaDataType.EnumMultivalue)
					{
						//int MetaKey = (int)FieldValue;

						//// TODO: Move it in to the current transaction [12/1/2004]
						//using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMultivalueDictionary"),
						//          new SqlParameter("@MetaKey", MetaKey)))
						//{
						//    ArrayList MultuValues = new ArrayList();

						//    while (readerValue.Read())
						//    {
						//        foreach (MetaDictionaryItem DicItem in field.Dictionary)
						//        {
						//            if (DicItem.Id == (int)readerValue["MetaDictionaryId"])
						//            {
						//                MultuValues.Add(DicItem);
						//                break;
						//            }
						//        }
						//    }
						//    readerValue.Close();

						//    FieldValue = (MetaDictionaryItem[])MultuValues.ToArray(typeof(MetaDictionaryItem));
						//}
					}
					else if (field.DataType == MetaDataType.File || field.DataType == MetaDataType.ImageFile)
					{
						//int MetaKey = (int)FieldValue;

						//// TODO: Move it in to the current transaction [12/1/2004]
						//using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current.ConnectionString, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaFile"),
						//          new SqlParameter("@MetaKey", MetaKey)))
						//{
						//    if (readerValue.Read())
						//    {
						//        MetaFile file = new MetaFile(readerValue);

						//        FieldValue = file;
						//    }
						//    else
						//        FieldValue = null;

						//    readerValue.Close();
						//}

					}
					else if (field.DataType == MetaDataType.StringDictionary)
					{
						//int MetaKey = (int)FieldValue;

						//// TODO: Move it in to the current transaction [12/1/2004]
						//using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current.ConnectionString, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaStringDictionary"),
						//          new SqlParameter("@MetaKey", MetaKey)))
						//{
						//    if (readerValue.Read())
						//    {
						//        MetaStringDictionary dic = new MetaStringDictionary();
						//        dic.LoadDictionary(readerValue);
						//        FieldValue = dic;
						//    }
						//    else
						//        FieldValue = null;

						//    readerValue.Close();
						//}
					}
					else if (field.DataType == MetaDataType.MetaObject)
					{
						//using (SqlDataReader readerValue = SqlHelper.ExecuteReader(MetaDataContext.Current.ConnectionString, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_LoadMetaObjectValue"),
						//          new SqlParameter("@MetaKey", (int)FieldValue)))
						//{
						//    int MetaClassId = -1;
						//    int MetaObjectId = -1;

						//    if (readerValue.Read())
						//    {
						//        MetaClassId = (int)readerValue["MetaClassId"];
						//        MetaObjectId = (int)readerValue["MetaObjectId"];

						//        readerValue.Close();

						//        try
						//        {
						//            FieldValue = MetaObject.Load(MetaObjectId, MetaClassId);
						//        }
						//        catch (Exception ex)
						//        {
						//            FieldValue = null;
						//            System.Diagnostics.Trace.WriteLine(ex, "Load MetaDataType.MetaObject");
						//        }
						//    }
						//    else
						//    {
						//        FieldValue = null;
						//        readerValue.Close();
						//    }
						//}

					}
				}

				_fieldStorage[field.Name] = FieldValue;
			}

			this._state = MetaObjectState.Unchanged;
		}

		static MetaObject Load(MetaClass type, SqlDataReader reader)
		{
			MetaObject retVal = new MetaObject();

			retVal._objectId = (int)reader["ObjectId"];
			retVal._metaClass = type;

			retVal.LoadMetaFields(reader);

			return retVal;
		}

		void ReloadMetaFields()
		{
			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, string.Format(AsyncResources.GetConstantValue("SPAT_TABLE_Get"), this.MetaClass.TableName),
					  new SqlParameter("@ObjectId", this.Id)))
			{
				if (reader.Read())
				{
					LoadMetaFields(reader);
				}
				reader.Close();
			}

			LoadMetaFields2();
		}
		#endregion

		#region Static Method Delete
		public static void Delete(int objectId, string metaClassName)
		{
			MetaObject obj = MetaObject.Load(objectId, metaClassName);
			if (obj != null)
			{
				obj.Delete();
				obj.AcceptChanges();
			}
		}

		public static void Delete(int objectId, int metaClassId)
		{
			MetaObject obj = MetaObject.Load(objectId, metaClassId);
			if (obj != null)
			{
				obj.Delete();
				obj.AcceptChanges();
			}
		}

		public static void Delete(int objectId, MetaClass type)
		{
			MetaObject obj = MetaObject.Load(objectId, type);
			if (obj != null)
			{
				obj.Delete();
				obj.AcceptChanges();
			}
		}
		#endregion

		#region Static Methods Get List
		public static MetaObject[] GetList(string metaClassName)
		{
			return MetaObject.GetList(MetaClass.Load(metaClassName));
		}

		public static MetaObject[] GetList(int metaClassId)
		{
			return MetaObject.GetList(MetaClass.Load(metaClassId));
		}

		public static MetaObject[] GetList(MetaClass type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			ArrayList retVal = new ArrayList();

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.Text, string.Format(AsyncResources.GetConstantValue("SPAT_TABLE_List"), type.TableName)))
			{
				while (reader.Read())
				{
					retVal.Add(MetaObject.Load(type, reader));
				}
				reader.Close();
			}

			foreach (MetaObject obj in retVal)
			{
				obj.LoadMetaFields2();
			}

			return (MetaObject[])retVal.ToArray(typeof(MetaObject));
		}

		public static MetaObject[] GetList(string metaClassName, int modifierId, DateTime modified)
		{
			return MetaObject.GetList(MetaClass.Load(metaClassName), modifierId, modified);
		}

		public static MetaObject[] GetList(int metaClassId, int modifierId, DateTime modified)
		{
			return MetaObject.GetList(MetaClass.Load(metaClassId), modifierId, modified);
		}

		public static MetaObject[] GetList(MetaClass type, int modifierId, DateTime modified)
		{
			MetaObject[] retVal = null;

			retVal = MetaObject.GetList(type);

			if (retVal != null)
			{
				foreach (MetaObject item in retVal)
				{
					item.ModifierId = modifierId;
					item.Modified = modified;
				}
			}

			return retVal;
		}
		#endregion

		#region Static Methods Load

		public static MetaObject Load(int objectId, string metaClassName)
		{
			MetaClass mc = MetaClass.Load(metaClassName);
			if (mc != null)
				return MetaObject.Load(objectId, mc);
			else
				return null;
		}

		public static MetaObject Load(int objectId, int metaClassId)
		{
			MetaClass mc = MetaClass.Load(metaClassId);
			if (mc != null)
				return MetaObject.Load(objectId, mc);
			else
				return null;
		}

		public static MetaObject Load(int objectId, MetaClass type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			MetaObject retVal = null;

			using (SqlDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, string.Format(AsyncResources.GetConstantValue("SPAT_TABLE_Get"), type.TableName),
					  new SqlParameter("@ObjectId", objectId)/*,
					  new SqlParameter("@Language", MetaDataContext.Current.Language)*/
																					   ))
			{
				if (reader.Read())
				{
					retVal = MetaObject.Load(type, reader);
				}
				reader.Close();
			}

			if(retVal!=null)
				retVal.LoadMetaFields2();

			return retVal;
		}

		public static MetaObject Load(int objectId, string metaClassName, int modifierId, DateTime modified)
		{
			MetaClass mc = MetaClass.Load(metaClassName);
			if (mc != null)
				return MetaObject.Load(objectId, mc, modifierId, modified);
			else
				return null;
		}

		public static MetaObject Load(int objectId, int metaClassId, int modifierId, DateTime modified)
		{
			MetaClass mc = MetaClass.Load(metaClassId);
			if (mc != null)
				return MetaObject.Load(objectId, mc, modifierId, modified);
			else
				return null;
		}

		public static MetaObject Load(int objectId, MetaClass type, int modifierId, DateTime modified)
		{
			MetaObject retVal = null;

			retVal = MetaObject.Load(objectId, type);

			if (retVal != null)
			{
				retVal.ModifierId = modifierId;
				retVal.Modified = modified;
			}

			return retVal;
		}

		#endregion

		#region  Static Methods NewObject

		public static MetaObject NewObject(int objectId, string metaClassName)
		{
			return MetaObject.NewObject(objectId, MetaClass.Load(metaClassName), -1, DateTime.Now);
		}

		public static MetaObject NewObject(int objectId, int metaClassId)
		{
			return MetaObject.NewObject(objectId, MetaClass.Load(metaClassId), -1, DateTime.Now);
		}

		public static MetaObject NewObject(int objectId, MetaClass type)
		{
			return MetaObject.NewObject(objectId, type, -1, DateTime.Now);
		}

		public static MetaObject NewObject(int objectId, string metaClassName, int creatorId)
		{
			return MetaObject.NewObject(objectId, MetaClass.Load(metaClassName), creatorId, DateTime.Now);
		}

		public static MetaObject NewObject(int objectId, int metaClassId, int creatorId)
		{
			return MetaObject.NewObject(objectId, MetaClass.Load(metaClassId), creatorId, DateTime.Now);
		}

		public static MetaObject NewObject(int objectId, MetaClass type, int creatorId)
		{
			return MetaObject.NewObject(objectId, type, creatorId, DateTime.Now);
		}

		public static MetaObject NewObject(int objectId, string metaClassName, int creatorId, DateTime created)
		{
			return MetaObject.NewObject(objectId, MetaClass.Load(metaClassName), creatorId, created);
		}

		public static MetaObject NewObject(int objectId, int metaClassId, int creatorId, DateTime created)
		{
			return MetaObject.NewObject(objectId, MetaClass.Load(metaClassId), creatorId, created);
		}

		public static MetaObject NewObject(int objectId, MetaClass type, int creatorId, DateTime created)
		{
			MetaObject retVal = new MetaObject();

			retVal._objectId = objectId;
			retVal._metaClass = type;

			retVal._creatorId = creatorId;
			retVal._created = created;

			retVal._modifierId = creatorId;
			retVal._modified = created;

			retVal.LoadMetaFields();

			return retVal;
		}

		#endregion

		#region Common Properties
		protected Hashtable FieldStorage
		{
			get
			{
				return _fieldStorage;
			}
		}

		public Hashtable GetValues()
		{
			Hashtable retVal = new Hashtable(this.FieldStorage);
			return retVal;
		}

		public Hashtable GetComplexFieldValues()
		{
			Hashtable retVal = new Hashtable(this.FieldStorage.Count);

			foreach (string Key in this.FieldStorage.Keys)
			{
				object FieldValue = this.FieldStorage[Key];

				MetaField metaField = this.MetaClass.MetaFields[Key];

				switch (metaField.DataType)
				{
					case MetaDataType.DictionarySingleValue:
					case MetaDataType.DictionaryMultivalue:
					case MetaDataType.EnumSingleValue:
					case MetaDataType.EnumMultivalue:
					case MetaDataType.StringDictionary:
					case MetaDataType.File:
					case MetaDataType.ImageFile:
					case MetaDataType.MetaObject:
						retVal.Add(Key, FieldValue);
						break;
					default:
						//retVal.Add(Key,FieldValue);
						break;
				}
			}

			return retVal;
		}

		public Hashtable GetElementaryFieldValues()
		{
			Hashtable retVal = new Hashtable(this.FieldStorage.Count);

			foreach (string Key in this.FieldStorage.Keys)
			{
				object FieldValue = this.FieldStorage[Key];

				MetaField metaField = this.MetaClass.MetaFields[Key];

				switch (metaField.DataType)
				{
					case MetaDataType.DictionarySingleValue:
					case MetaDataType.DictionaryMultivalue:
					case MetaDataType.EnumSingleValue:
					case MetaDataType.EnumMultivalue:
					case MetaDataType.StringDictionary:
					case MetaDataType.File:
					case MetaDataType.ImageFile:
					case MetaDataType.MetaObject:
						//retVal.Add(Key,FieldValue);
						break;
					default:
						retVal.Add(Key, FieldValue);
						break;
				}
			}

			return retVal;
		}

		public int Id
		{
			get
			{
				return _objectId;
			}
		}

		public MetaDataContext Context
		{
			get
			{
				return MetaDataContext.Current;
			}
		}

		public int CreatorId
		{
			get
			{
				return _creatorId;
			}
			set
			{
				_creatorId = value;
			}
		}

		public DateTime Created
		{
			get
			{
				return _created;
			}
			set
			{
				_created = value;
			}
		}


		public int ModifierId
		{
			get
			{
				return _modifierId;
			}
			set
			{
				_modifierId = value;
			}
		}

		public DateTime Modified
		{
			get
			{
				return _modified;
			}
			set
			{
				_modified = value;
			}
		}

		public MetaObjectState ObjectState
		{
			get
			{
				return _state;
			}
		}

		public MetaClass MetaClass
		{
			get
			{
				return _metaClass;
			}
		}
		#endregion

		#region Manage Methods
		public void AcceptChanges()
		{
			switch (this.ObjectState)
			{
				case MetaObjectState.Added:
				case MetaObjectState.Modified:
					// Insert or Update object [11/25/2004]
					SqlParameter[] sqlParams = new SqlParameter[6 + 1 + this.FieldStorage.Keys.Count];

					int Index = 0;
					sqlParams[Index++] = new SqlParameter("@ObjectId", this.Id);

					sqlParams[Index++] = new SqlParameter("@CreatorId", this.CreatorId);
					sqlParams[Index++] = new SqlParameter("@Created", this.Created);

					sqlParams[Index++] = new SqlParameter("@ModifierId", this.ModifierId);
					sqlParams[Index++] = new SqlParameter("@Modified", this.Modified);

					//if (MetaDataContext.Current.Language != null)
					//{
					//	sqlParams[Index++] = new SqlParameter("@Language", MetaDataContext.Current.Language);
					//}
					//else sqlParams[Index++] = new SqlParameter("@Language", DBNull.Value);

					SqlParameter RetvalParam = new SqlParameter("@Retval", SqlDbType.Int);
					RetvalParam.Direction = ParameterDirection.Output;
					sqlParams[Index++] = RetvalParam;

					foreach (string Key in this.FieldStorage.Keys)
					{
						object FieldValue = this.FieldStorage[Key] == null ? DBNull.Value : this.FieldStorage[Key];

						// OZ: I changed lines to fix System and User field with same named problem.
						//MetaField	metaField = this.MetaClass.MetaFields[Key];
						MetaField metaField = this.MetaClass.UserMetaFields[Key];

						//if(metaField.IsSystem)
						//	continue;

						if (FieldValue != DBNull.Value)
						{
							#region FieldValue!=DBNull.Value Section
							if (metaField.DataType == MetaDataType.DictionarySingleValue ||
								metaField.DataType == MetaDataType.EnumSingleValue)
							{
								FieldValue = ((MetaDictionaryItem)FieldValue).Id;
							}
							else if (metaField.DataType == MetaDataType.DictionaryMultivalue ||
								metaField.DataType == MetaDataType.EnumMultivalue)
							{
								if (this.Id != -1)
								{
									// Step 1. Resived Meta Key Value [11/30/2004]
									int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

									// Step 2. Clear old value [11/30/2004]
									SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_ClearMultivalueDictionary"),
										new SqlParameter("@MetaKey", MetaKey));

									// Step 3. Save new values [11/30/2004]
									foreach (MetaDictionaryItem DicItem in (MetaDictionaryItem[])FieldValue)
									{
										SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_AddMultivalueDictionary"),
											new SqlParameter("@MetaKey", MetaKey),
											new SqlParameter("@MetaDictionaryId", DicItem.Id));
									}

									FieldValue = MetaKey;
								}
								else
								{
									FieldValue = (int)-1;
								}
							}
							else if (metaField.DataType == MetaDataType.File || metaField.DataType == MetaDataType.ImageFile)
							{
								if (this.Id != -1)
								{
									MetaFile file = (MetaFile)FieldValue;

									// Step 1. Resived Meta Key Value [11/30/2004]
									int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

									if (file.HasChanges)
									{
										SqlParameter spData = new SqlParameter("@Data", SqlDbType.Image);
										spData.Value = SqlHelper.Null2DBNull(MetaDataContext.Current.MetaFileDataStorageType == MetaFileDataStorageType.DataBase ? file.Buffer : null);

										// Step 3. Save new values [11/30/2004]
										SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaFile"),
											new SqlParameter("@MetaKey", MetaKey),
											new SqlParameter("@FileName", file.Name),
											new SqlParameter("@ContentType", file.ContentType),
											new SqlParameter("@Size", file.Size),
											spData,
											new SqlParameter("@CreationTime", file.CreationTime),
											new SqlParameter("@LastReadTime", file.LastReadTime),
											new SqlParameter("@LastWriteTime", file.LastWriteTime));

										if (MetaDataContext.Current.MetaFileDataStorageType == MetaFileDataStorageType.LocalDisk)
										{
											using (FileStream stream = File.OpenWrite(Path.Combine(MetaDataContext.Current.LocalDiskStorage, string.Format("{0}.mdpdata", MetaKey))))
											{
												if (file.Buffer != null)
													stream.Write(file.Buffer, 0, file.Size);
												stream.SetLength(file.Size);
												stream.Flush();
											}
										}

										file.HasChanges = false;
										file.SetId(MetaKey);
									}

									FieldValue = MetaKey;
								}
								else
								{
									FieldValue = (int)-1;
								}

							}
							else if (metaField.DataType == MetaDataType.StringDictionary)
							{
								if (this.Id != -1)
								{
									MetaStringDictionary dic = (MetaStringDictionary)FieldValue;

									// Step 1. Resived Meta Key Value [11/30/2004]
									int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

									// Step 2. Clear old value [11/30/2004]
									SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_ClearStringDictionary"),
										new SqlParameter("@MetaKey", MetaKey));

									// Step 3. Add new value [12/23/2004]
									foreach (string KeyValue in dic.Keys)
									{
										SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_AddMetaStringDictionary"),
											new SqlParameter("@MetaKey", MetaKey),
											new SqlParameter("@Key", KeyValue),
											new SqlParameter("@Value", dic[KeyValue]));
									}

									FieldValue = MetaKey;
								}
								else
								{
									FieldValue = (int)-1;
								}

							}
							else if (metaField.DataType == MetaDataType.MetaObject)
							{
								if (this.Id != -1)
								{
									MetaObject mObject = (MetaObject)FieldValue;

									// Step 1. Resived Meta Key Value [11/30/2004]
									int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

									// Step 2. Clear old value [11/30/2004]
									SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaObjectValue"),
										new SqlParameter("@MetaKey", MetaKey));

									// Step 3. Add new value [12/23/2004]
									SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaObjectValue"),
										new SqlParameter("@MetaKey", MetaKey),
										new SqlParameter("@MetaClassId", mObject.MetaClass.Id),
										new SqlParameter("@MetaObjectId", mObject.Id));
								}
								else
								{
									FieldValue = (int)-1;
								}
							}
							#endregion
						}
						else
						{
							#region DBNull.Value Section
							if (metaField.DataType == MetaDataType.DictionaryMultivalue ||
								metaField.DataType == MetaDataType.EnumMultivalue)
							{
								// Step 1. Resived Meta Key Value [11/30/2004]
								int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

								// Step 2. Clear old value [11/30/2004]
								SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_ClearMultivalueDictionary"),
									new SqlParameter("@MetaKey", MetaKey));
							}
							else if (metaField.DataType == MetaDataType.File || metaField.DataType == MetaDataType.ImageFile)
							{
								// Step 1. Resived Meta Key Value [11/30/2004]
								int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);
								// Step 2. Clear old value [11/30/2004]
								SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaFile"),
									new SqlParameter("@MetaKey", MetaKey));
							}
							else if (metaField.DataType == MetaDataType.StringDictionary)
							{
								// Step 1. Resived Meta Key Value [11/30/2004]
								int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

								// Step 2. Clear old value [11/30/2004]
								SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_ClearStringDictionary"),
									new SqlParameter("@MetaKey", MetaKey));
							}
							else if (metaField.DataType == MetaDataType.MetaObject)
							{
								// Step 1. Resived Meta Key Value [11/30/2004]
								int MetaKey = SqlHelper.GetMetaKey(this.Id, this.MetaClass.Id, metaField.Id);

								// Step 2. Clear old value [11/30/2004]
								SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_DeleteMetaObjectValue"),
									new SqlParameter("@MetaKey", MetaKey));
							}

							#endregion

							//if (!metaField.AllowNulls && this.ObjectState == MetaObjectState.Added)
							//{
							//	FieldValue = MetaObject.GetDefaultValue(metaField.DataType);
							//}
						}


						// Fix Problem: DbNull.Value in an image column [2/17/2005]
						if (FieldValue == DBNull.Value && metaField.DataType == MetaDataType.Image)
						{
							SqlParameter tmpSqlPrm = new SqlParameter("@" + Key, SqlDbType.Image);
							tmpSqlPrm.Value = DBNull.Value;
							sqlParams[Index++] = tmpSqlPrm;
						}
						else if (FieldValue == DBNull.Value && metaField.DataType == MetaDataType.Binary)
						{
							SqlParameter tmpSqlPrm = new SqlParameter("@" + Key, SqlDbType.Binary);
							tmpSqlPrm.Value = DBNull.Value;
							sqlParams[Index++] = tmpSqlPrm;
						}
						else if (FieldValue == DBNull.Value && metaField.DataType == MetaDataType.Money)
						{
							SqlParameter tmpSqlPrm = new SqlParameter("@" + Key, SqlDbType.Money);
							tmpSqlPrm.Value = DBNull.Value;
							sqlParams[Index++] = tmpSqlPrm;
						}
						else if (metaField.DataType == MetaDataType.NText ||
							metaField.DataType == MetaDataType.LongHtmlString ||
							metaField.DataType == MetaDataType.LongString)
						{
							SqlParameter tmpSqlPrm = new SqlParameter("@" + Key, SqlDbType.NText);
							tmpSqlPrm.Value = FieldValue;
							sqlParams[Index++] = tmpSqlPrm;
						}
						else
							sqlParams[Index++] = new SqlParameter("@" + Key, FieldValue);

					}

					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, string.Format(AsyncResources.GetConstantValue("SPAT_TABLE_Update"), this.MetaClass.TableName), sqlParams);

					if (this.Id == -1)
					{
						this._objectId = (int)RetvalParam.Value;
						this.AcceptChanges();
					}

					this._state = MetaObjectState.Unchanged;
					break;
				case MetaObjectState.Deleted:
					// Delete File Item if MetaFileDataStorageType.LocalDisk
					if (this.Context.MetaFileDataStorageType == MetaFileDataStorageType.LocalDisk)
					{
						foreach (object o in this.FieldStorage)
						{
							MetaFile mf = o as MetaFile;
							if (mf != null)
							{
								try
								{
									mf.Delete();
								}
								catch (Exception ex)
								{
									System.Diagnostics.Trace.WriteLine(ex);
								}
							}
						}
					}

					// Delete Object
					SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, string.Format(AsyncResources.GetConstantValue("SPAT_TABLE_Delete"), this.MetaClass.TableName),
						new SqlParameter("@ObjectId", this.Id));
					break;
				case MetaObjectState.Unchanged:
					break;
			}
		}

		public void RejectChanges()
		{
			switch (this.ObjectState)
			{
				case MetaObjectState.Added:
					foreach (string FieldName in this.FieldStorage.Keys)
					{
						this.FieldStorage[FieldName] = null;
					}
					break;
				case MetaObjectState.Deleted:
				case MetaObjectState.Modified:
					this._state = MetaObjectState.Unchanged;
					// Reload Settings [11/25/2004]
					ReloadMetaFields();
					break;
				case MetaObjectState.Unchanged:
					break;
			}
		}

		public void Delete()
		{
			this._state = MetaObjectState.Deleted;
		}

		protected void ChangeState()
		{
			switch (this.ObjectState)
			{
				case MetaObjectState.Added:
					break;
				case MetaObjectState.Deleted:
					throw new DeletedObjectInaccessibleException();
				case MetaObjectState.Modified:
					break;
				case MetaObjectState.Unchanged:
					this._state = MetaObjectState.Modified;
					break;
			}
		}

		protected void ValidateMetaField(string metaFieldName, object value)
		{
			MetaField field = this.MetaClass.MetaFields[metaFieldName];

			if (field == null)
				throw new ArgumentException("");

			if (value != null)
			{
				if (field.DataType == MetaDataType.DictionarySingleValue ||
					field.DataType == MetaDataType.EnumSingleValue)
				{
					if (value.GetType() != typeof(MetaDictionaryItem))
						throw new IncorectValueTypeException();

					foreach (MetaDictionaryItem dicItem in field.Dictionary)
					{
						if (((MetaDictionaryItem)value).Id == dicItem.Id)
							return;
					}

					throw new IncorectDictionaryItemException();
				}
				else if (field.DataType == MetaDataType.DictionarySingleValue ||
					field.DataType == MetaDataType.EnumSingleValue)
				{
					if (value.GetType() != typeof(MetaDictionaryItem[]))
						throw new IncorectValueTypeException();

					MetaDictionaryItem[] items = value as MetaDictionaryItem[];

					int validItemCount = 0;
					foreach (MetaDictionaryItem dicSrcItem in items)
					{
						foreach (MetaDictionaryItem dicItem in field.Dictionary)
						{
							if (dicSrcItem.Id == dicItem.Id)
							{
								validItemCount++;
								break;
							}
						}
					}

					if (validItemCount != items.Length)
						throw new IncorectDictionaryItemException();
				}
				else if (field.DataType == MetaDataType.File ||
					field.DataType == MetaDataType.ImageFile)
				{
					if (value.GetType() != typeof(MetaFile))
						throw new IncorectValueTypeException();

				}
				else if (field.DataType == MetaDataType.StringDictionary)
				{
					if (value.GetType() != typeof(MetaStringDictionary))
						throw new IncorectValueTypeException();
				}
				else if (field.DataType == MetaDataType.MetaObject)
				{
					if (value.GetType() != typeof(MetaObject))
						throw new IncorectValueTypeException();
				}
			}
		}
		#endregion

		#region Data Methods

		public object this[MetaField field]
		{
			get
			{
				if (field == null)
					throw new ArgumentNullException("field");

				return this[field.Name];
			}
			set
			{
				if (field == null)
					throw new ArgumentNullException("field");

				this[field.Name] = value;
			}
		}

		public object this[string fieldName]
		{
			get
			{
				return this.FieldStorage[fieldName];
			}
			set
			{
				ValidateMetaField(fieldName, value);
				ChangeState();

				this.FieldStorage[fieldName] = value;
			}
		}

		public object this[int fieldId]
		{
			get
			{
				return this[MetaField.Load(fieldId).Name];
			}
			set
			{
				this[MetaField.Load(fieldId).Name] = value;
			}
		}

		public object[] ItemArray
		{
			get
			{
				int Count = this.FieldStorage.Count;

				object[] retVal = new object[Count];

				int Index = 0;

				foreach (MetaField field in this.MetaClass.MetaFields)
				{
					if (field.IsSystem)
						continue;
					retVal[Index++] = this.FieldStorage[field.Name];
				}

				return retVal;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (value.Length != this.FieldStorage.Count)
					throw new ArgumentException("Invalid array length.");

				int Index = 0;
				foreach (MetaField field in this.MetaClass.MetaFields)
				{
					if (field.IsSystem)
						continue;
					this.FieldStorage[field.Name] = value[Index++];
				}
			}
		}

		#endregion

		#region GetDictionaryItem Methods
		public MetaDictionaryItem GetDictionaryItem(int fieldId)
		{
			return (MetaDictionaryItem)this[fieldId];
		}

		public MetaDictionaryItem GetDictionaryItem(string fieldName)
		{
			return (MetaDictionaryItem)this[fieldName];
		}

		public MetaDictionaryItem GetDictionaryItem(MetaField field)
		{
			return (MetaDictionaryItem)this[field];
		}
		#endregion

		#region GetDictionaryItems Methods
		public MetaDictionaryItem[] GetDictionaryItems(int fieldId)
		{
			return (MetaDictionaryItem[])this[fieldId];
		}

		public MetaDictionaryItem[] GetDictionaryItems(string fieldName)
		{
			return (MetaDictionaryItem[])this[fieldName];
		}

		public MetaDictionaryItem[] GetDictionaryItems(MetaField field)
		{
			return (MetaDictionaryItem[])this[field];
		}
		#endregion

		#region GetFile Methods
		public MetaFile GetFile(int fieldId)
		{
			return (MetaFile)this[fieldId];
		}

		public MetaFile GetFile(string fieldName)
		{
			return (MetaFile)this[fieldName];
		}

		public MetaFile GetFile(MetaField field)
		{
			return (MetaFile)this[field];
		}
		#endregion

		#region GetStringDictionary Methods
		public MetaStringDictionary GetStringDictionary(int fieldId)
		{
			return (MetaStringDictionary)this[fieldId];
		}

		public MetaStringDictionary GetStringDictionary(string fieldName)
		{
			return (MetaStringDictionary)this[fieldName];
		}

		public MetaStringDictionary GetStringDictionary(MetaField field)
		{
			return (MetaStringDictionary)this[field];
		}

		#endregion

		#region GetMetaObject Methods
		public MetaObject GetMetaObject(int fieldId)
		{
			return (MetaObject)this[fieldId];
		}

		public MetaObject GetMetaObject(string fieldName)
		{
			return (MetaObject)this[fieldName];
		}

		public MetaObject GetMetaObject(MetaField field)
		{
			return (MetaObject)this[field];
		}
		#endregion

		#region GetString Methods
		public string GetString(int fieldId)
		{
			return (string)this[fieldId];
		}

		public string GetString(string fieldName)
		{
			return (string)this[fieldName];
		}

		public string GetString(MetaField field)
		{
			return (string)this[field];
		}
		#endregion

		#region GetInt32 Methods
		public Int32 GetInt32(int fieldId)
		{
			return (Int32)this[fieldId];
		}

		public Int32 GetInt32(string fieldName)
		{
			return (Int32)this[fieldName];
		}

		public Int32 GetInt32(MetaField field)
		{
			return (Int32)this[field];
		}
		#endregion

		#region GetDateTime Methods
		public DateTime GetDateTime(int fieldId)
		{
			return (DateTime)this[fieldId];
		}

		public DateTime GetDateTime(string fieldName)
		{
			return (DateTime)this[fieldName];
		}

		public DateTime GetDateTime(MetaField field)
		{
			return (DateTime)this[field];
		}
		#endregion

		#region GetDefaultValue
		public static object GetDefaultValue(MetaDataType type)
		{
			// SqlDbType Enumeration 
			//ms-help://MS.MSDNQTR.2004JAN.1033/cpref/html/frlrfSystemDataSqlDbTypeClassTopic.htm
			switch (type)
			{
				case MetaDataType.BigInt:
					return (Int64)0;
				case MetaDataType.Binary:
					return new byte[0];
				case MetaDataType.Bit:
					return (Boolean)false;
				case MetaDataType.Char:
					return String.Empty;
				case MetaDataType.DateTime:
					return DateTime.Now;
				case MetaDataType.Decimal:
					return (Decimal)0;
				case MetaDataType.Float:
					return (Double)0;
				case MetaDataType.Image:
					return new byte[0];
				case MetaDataType.Int:
					return (Int32)0;
				case MetaDataType.Money:
					return (Decimal)0;
				case MetaDataType.NChar:
					return string.Empty;
				case MetaDataType.NText:
					return string.Empty;
				case MetaDataType.NVarChar:
					return string.Empty;
				case MetaDataType.Real:
					return (Single)0;
				case MetaDataType.UniqueIdentifier:
					return Guid.NewGuid();
				case MetaDataType.SmallDateTime:
					return DateTime.Now;
				case MetaDataType.SmallInt:
					return (Int16)0;
				case MetaDataType.SmallMoney:
					return (Decimal)0;
				case MetaDataType.Text:
					return string.Empty;
				case MetaDataType.Timestamp:
					return new byte[0];
				case MetaDataType.TinyInt:
					return (Byte)0;
				case MetaDataType.VarBinary:
					return new byte[0];
				case MetaDataType.VarChar:
					return string.Empty;
				case MetaDataType.Variant:
					return (int)0;
				case MetaDataType.Numeric:
					return (Decimal)0;
				case MetaDataType.Sysname:
					return string.Empty;

				// MetaData Types [11/16/2004]
				case MetaDataType.Integer:
					return (Int32)0;
				case MetaDataType.Boolean:
					return (Boolean)false;
				case MetaDataType.Date:
					return DateTime.Now;
				case MetaDataType.Email:
					return string.Empty;
				case MetaDataType.Url:
					return string.Empty;
				case MetaDataType.ShortString:
					return string.Empty;
				case MetaDataType.LongString:
					return string.Empty;
				case MetaDataType.LongHtmlString:
					return string.Empty;
				case MetaDataType.DictionarySingleValue:
					return (Int32)(-1);
				case MetaDataType.DictionaryMultivalue:
					return (Int32)(-1);
				case MetaDataType.EnumSingleValue:
					return (Int32)(-1);
				case MetaDataType.EnumMultivalue:
					return (Int32)(-1);
				case MetaDataType.StringDictionary:
					return (Int32)(-1);
				case MetaDataType.File:
					return (Int32)(-1);
				case MetaDataType.ImageFile:
					return (Int32)(-1);
				case MetaDataType.MetaObject:
					return (Int32)(-1);
				default:
					return null;
			}
		}
		#endregion

		#region GetHistory
		public static IDataReader GetHistoryDataReader(int metaClassId, int metaObjectId)
		{
			return GetHistoryDataReader(MetaClass.Load(metaClassId), metaObjectId);
		}

		public static IDataReader GetHistoryDataReader(MetaClass metaClass, int metaObjectId)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			return SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, string.Format(AsyncResources.GetConstantValue("SPAT_TABLE_History"), metaClass.TableName), new SqlParameter("@ObjectId", metaObjectId));
		}

		public IDataReader GetHistoryDataReader()
		{
			return GetHistoryDataReader(this.MetaClass, this.Id);
		}

		#endregion
	}
}
