using System;
using System.Xml;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;

using Mediachase.MetaDataPlus.Common;


namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaInstaller.
	/// </summary>
	public sealed class MetaInstaller
	{
		private static CultureInfo _culture = CultureInfo.InvariantCulture;

		private MetaInstaller()
		{
		}

		#region -- CleanupLockalStorage --
		public static void CleanupLocalDiskStorage()
		{
			if (MetaDataContext.Current.MetaFileDataStorageType == MetaFileDataStorageType.DataBase)
				return;

			string[] FileList = Directory.GetFiles(MetaDataContext.Current.LocalDiskStorage, "*.mdpdata");

			foreach (string fileName in FileList)
			{
				bool bGoodFile = false;

				try
				{
					int MetaKey = int.Parse(System.IO.Path.GetFileNameWithoutExtension(fileName), _culture);

					using (IDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_GetMetaKeyInfo"),
							  new SqlParameter("@MetaKey", MetaKey)))
					{
						if (reader.Read())
							bGoodFile = true;
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
				}

				if (!bGoodFile)
				{
					try
					{
						File.Delete(fileName);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Trace.WriteLine(ex);
					}
				}
			}
		}
		#endregion

		#region -- MetaFileCopy --
		public static void MetaFileCopyToDataBase(string path)
		{
			foreach (string fileName in Directory.GetFiles(path, "*.mdpdata"))
			{
				try
				{
					int MetaKey = int.Parse(System.IO.Path.GetFileNameWithoutExtension(fileName), _culture);

					string FilePath = System.IO.Path.Combine(path, fileName);

					using (FileStream stream = File.OpenRead(FilePath))
					{
						using (IDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_GetMetaKeyInfo"),
								  new SqlParameter("@MetaKey", MetaKey)))
						{
							if (reader.Read())
							{
								byte[] Buffer = new byte[stream.Length];
								stream.Read(Buffer, 0, Buffer.Length);

								//int MetaClassId = (int)reader["MetaClassId"];
								//int MetaFieldId = (int)reader["MetaFieldId"];
								//int MetaObjectId = (int)reader["MetaObjectId"];

								SqlParameter spData = new SqlParameter("@Data", SqlDbType.Image);
								spData.Value = SqlHelper.Null2DBNull(Buffer);

								// Step 3. Save new values [11/30/2004]
								SqlHelper.ExecuteNonQuery(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("SP_UpdateMetaFile"),
									new SqlParameter("@MetaKey", MetaKey),
									new SqlParameter("@FileName", fileName),
									new SqlParameter("@ContentType", string.Empty),
									new SqlParameter("@Size", stream.Length),
									spData,
									new SqlParameter("@CreationTime", File.GetCreationTime(FilePath)),
									new SqlParameter("@LastReadTime", File.GetLastAccessTime(FilePath)),
									new SqlParameter("@LastWriteTime", File.GetLastWriteTime(FilePath)));
							}
						}
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex);
				}
			}
		}

		public static void MetaFileCopyToLocalDisk(string path)
		{
			using (IDataReader reader = SqlHelper.ExecuteReader(MetaDataContext.Current, CommandType.StoredProcedure, AsyncResources.GetConstantValue("mdpsp_sys_LoadMetaFileList")))
			{
				while (reader.Read())
				{
					byte[] Data = (byte[])(SqlHelper.DBNull2Null(reader["Data"]));

					using (FileStream stream = File.OpenWrite(System.IO.Path.Combine(path, string.Format(_culture, "{0}.mdpdata", (int)reader["MetaKey"]))))
					{
						stream.Write(Data, 0, Data.Length);
						stream.Flush();
					}
				}
			}
		}
		#endregion

		#region -- Restore --

		public static void RestoreFromFile(SqlTransaction tran, string fileName)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(fileName);
			Restore(tran, xmlDoc.SelectSingleNode("MetaDataPlusBackup"));
		}

		public static void Restore(SqlTransaction tran, string xml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);
			Restore(tran, xmlDoc.SelectSingleNode("MetaDataPlusBackup"));
		}


		public static void Restore(SqlTransaction tran, XmlNode root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			XmlAttribute version = root.Attributes["version"];

			try
			{
				MetaDataContext.Current.Transaction = tran;

				switch (version.Value)
				{
					case "1.0":
						RestoreFromVersion10(root);
						break;
					default:
						throw new MetaException("Unsupported backup version {0}.", version.Value);

				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				throw;
			}
			finally
			{
				MetaDataContext.Current.Transaction = null;
			}
		}
		public static void RestoreFromFile(string fileName)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(fileName);
			Restore(xmlDoc.SelectSingleNode("MetaDataPlusBackup"));
		}

		public static void Restore(string xml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);
			Restore(xmlDoc.SelectSingleNode("MetaDataPlusBackup"));
		}


		public static void Restore(XmlNode root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			XmlAttribute version = root.Attributes["version"];

			try
			{
				MetaDataContext.Current.BeginTransaction();

				switch (version.Value)
				{
					case "1.0":
						RestoreFromVersion10(root);
						break;
					default:
						throw new MetaException("Unsupported backup version {0}.", version.Value);

				}

				MetaDataContext.Current.Commit();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				MetaDataContext.Current.Rollback();
				throw;
			}
		}

		#region -- Restore Version 1.0

		private static MetaClass LoadMetaClassVersion10(XmlNode xmlMetaClass, MetaClass parent)
		{
			string Name = xmlMetaClass.SelectSingleNode("Name").InnerXml;

			MetaClass mc = MetaClass.Load(Name);

			if (mc == null)
			{
				mc = MetaClass.Create(xmlMetaClass.SelectSingleNode("Namespace").InnerXml,
					Name,
					xmlMetaClass.SelectSingleNode("FriendlyName").InnerXml,
					xmlMetaClass.SelectSingleNode("TableName").InnerXml,
					parent,
					(MetaClassType)Enum.Parse(typeof(MetaClassType), xmlMetaClass.SelectSingleNode("MetaClassType").InnerXml),
					xmlMetaClass.SelectSingleNode("Description").InnerXml);
			}
			else
			{
				if (mc.FriendlyName != xmlMetaClass.SelectSingleNode("FriendlyName").InnerXml)
					mc.FriendlyName = xmlMetaClass.SelectSingleNode("FriendlyName").InnerXml;

				if (mc.Description != xmlMetaClass.SelectSingleNode("Description").InnerXml)
					mc.Description = xmlMetaClass.SelectSingleNode("Description").InnerXml;
			}

			string FieldListChangedSqlScript = xmlMetaClass.SelectSingleNode("FieldListChangedSqlScript").InnerXml;

			if (!string.IsNullOrEmpty(FieldListChangedSqlScript) && mc.FieldListChangedSqlScript != FieldListChangedSqlScript)
			{
				mc.FieldListChangedSqlScript = FieldListChangedSqlScript;
			}
			else
			{
				if (mc.FieldListChangedSqlScript != null)
					mc.FieldListChangedSqlScript = null;
			}

			string strTag = xmlMetaClass.SelectSingleNode("Tag").InnerXml;
			if (!string.IsNullOrEmpty(strTag))
			{
				mc.Tag = Convert.FromBase64String(strTag);
			}
			else
			{
				if (mc.Tag != null)
					mc.Tag = null;
			}

			return mc;
		}

		private static MetaField LoadMetaFieldVersion10(XmlNode xmlMetaField)
		{
			string Name = xmlMetaField.SelectSingleNode("Name").InnerXml;

			MetaField mf = MetaField.Load(Name);

			if (mf == null)
			{
				mf = MetaField.Create(xmlMetaField.SelectSingleNode("Namespace").InnerXml,
					Name,
					xmlMetaField.SelectSingleNode("FriendlyName").InnerXml,
					xmlMetaField.SelectSingleNode("Description").InnerXml,
					(MetaDataType)Enum.Parse(typeof(MetaDataType), xmlMetaField.SelectSingleNode("DataType").InnerXml),
					Int32.Parse(xmlMetaField.SelectSingleNode("Length").InnerXml, _culture),
					bool.Parse(xmlMetaField.SelectSingleNode("AllowNulls").InnerXml),
					bool.Parse(xmlMetaField.SelectSingleNode("SaveHistory").InnerXml),
					bool.Parse(xmlMetaField.SelectSingleNode("AllowSearch").InnerXml),
					xmlMetaField.SelectSingleNode("MultiLanguageValue")!=null?
						bool.Parse(xmlMetaField.SelectSingleNode("MultiLanguageValue").InnerXml):
						false
					);
			}
			else
			{
				if (mf.FriendlyName != xmlMetaField.SelectSingleNode("FriendlyName").InnerXml)
					mf.FriendlyName = xmlMetaField.SelectSingleNode("FriendlyName").InnerXml;

				if (mf.Description != xmlMetaField.SelectSingleNode("Description").InnerXml)
					mf.Description = xmlMetaField.SelectSingleNode("Description").InnerXml;

				if (mf.SaveHistory != bool.Parse(xmlMetaField.SelectSingleNode("SaveHistory").InnerXml))
					mf.SaveHistory = bool.Parse(xmlMetaField.SelectSingleNode("SaveHistory").InnerXml);

				if (mf.AllowSearch != bool.Parse(xmlMetaField.SelectSingleNode("AllowSearch").InnerXml))
					mf.AllowSearch = bool.Parse(xmlMetaField.SelectSingleNode("AllowSearch").InnerXml);
			}

			string strTag = xmlMetaField.SelectSingleNode("Tag").InnerXml;
			if (!string.IsNullOrEmpty(strTag))
				mf.Tag = Convert.FromBase64String(strTag);
			else
				mf.Tag = null;

			// Dictionary
			if (mf.Dictionary != null)
			{
				foreach (XmlNode dicItem in xmlMetaField.SelectNodes("Dictionary"))
				{
					if (!mf.Dictionary.Contains(dicItem.InnerXml))
						mf.Dictionary.Add(dicItem.InnerXml);
				}
			}

			return mf;
		}


		private static void RestoreFromVersion10(XmlNode root)
		{
			#region -- Restore Meta Classes --
			XmlNodeList xmlMetaClassList = root.SelectNodes("MetaClass");

			foreach (XmlNode xmlMetaClass in xmlMetaClassList)
			{
				string ParentClass = xmlMetaClass.SelectSingleNode("ParentClass").InnerXml;

				MetaClass parent = null;

				if (!string.IsNullOrEmpty(ParentClass))
				{
					parent = MetaClass.Load(ParentClass);

					if (parent == null)
					{
						XmlNode xmlParentMetaClass = root.SelectSingleNode(string.Format(_culture, "MetaClass[Name='{0}']", ParentClass));
						parent = LoadMetaClassVersion10(xmlParentMetaClass, null);
					}
				}

				LoadMetaClassVersion10(xmlMetaClass, parent);
			}

			#endregion

			#region -- Restore Meta Fields --
			XmlNodeList xmlMetaFieldList = root.SelectNodes("MetaField");

			foreach (XmlNode xmlMetaField in xmlMetaFieldList)
			{
				MetaField mf = LoadMetaFieldVersion10(xmlMetaField);

				XmlNodeList xmlOwnerMetaClassList = xmlMetaField.SelectNodes("OwnerMetaClass");

				foreach (XmlNode xmlOwnerMetaClass in xmlOwnerMetaClassList)
				{
					MetaClass ownerMC = MetaClass.Load(xmlOwnerMetaClass.InnerXml);
					if (!ownerMC.MetaFields.Contains(mf))
						ownerMC.AddField(mf);
				}
			}
			#endregion
		}


		#endregion

		#endregion

		#region -- Backup --
		public static void Backup(string destFileName, params object[] selected)
		{
			XmlDocument xmlDocument = new XmlDocument();
			Backup(xmlDocument, selected);
			xmlDocument.Save(destFileName);
		}

		public static string Backup(params object[] selected)
		{
			XmlDocument xmlDocument = new XmlDocument();
			Backup(xmlDocument, selected);
			return xmlDocument.InnerXml;
		}

		public static void Backup(XmlNode destNode, params object[] selected)
		{
			if (destNode == null)
				throw new ArgumentNullException("destNode");
			if (selected == null)
				throw new ArgumentNullException("selected");

			XmlDocument ownerDocument = destNode as XmlDocument;
			if (ownerDocument == null)
				ownerDocument = destNode.OwnerDocument;

			XmlNode xmlMetaDataPlusBackup = ownerDocument.CreateElement("MetaDataPlusBackup");

			XmlAttribute xmlMDPBVersion = ownerDocument.CreateAttribute("version");
			xmlMDPBVersion.Value = "1.0";
			xmlMetaDataPlusBackup.Attributes.Append(xmlMDPBVersion);

			#region -- Backup Meta Classes --
			foreach (object metaElement in selected)
			{
				MetaClass mc = metaElement as MetaClass;
				if (mc != null)
				{
					if (mc.Parent != null && xmlMetaDataPlusBackup.SelectSingleNode(string.Format(_culture, "MetaClass[Name='{0}']", mc.Parent.Name)) == null)
					{
						BackupMetaClass(xmlMetaDataPlusBackup, mc.Parent);
					}
					BackupMetaClass(xmlMetaDataPlusBackup, mc);
				}
			}
			#endregion

			#region -- Backup Meta Fields --
			foreach (object metaElement in selected)
			{
				MetaField mf = metaElement as MetaField;
				if (mf != null)
				{
					BackupMetaField(xmlMetaDataPlusBackup, mf);
				}
			}
			#endregion

			destNode.AppendChild(xmlMetaDataPlusBackup);
		}

		private static void BackupMetaClass(XmlNode root, MetaClass mc)
		{
			XmlDocumentFragment xmlMetaClass = root.OwnerDocument.CreateDocumentFragment();
			xmlMetaClass.InnerXml = "<MetaClass><Namespace></Namespace><Name></Name><FriendlyName></FriendlyName><MetaClassType></MetaClassType><ParentClass></ParentClass><TableName></TableName><Description></Description><FieldListChangedSqlScript></FieldListChangedSqlScript><Tag></Tag></MetaClass>";

			xmlMetaClass.SelectSingleNode("MetaClass/Namespace").InnerXml = mc.Namespace;
			xmlMetaClass.SelectSingleNode("MetaClass/Name").InnerXml = mc.Name;
			xmlMetaClass.SelectSingleNode("MetaClass/FriendlyName").InnerXml = mc.FriendlyName;
			xmlMetaClass.SelectSingleNode("MetaClass/MetaClassType").InnerXml = mc.MetaClassType.ToString();

			if (mc.Parent != null)
				xmlMetaClass.SelectSingleNode("MetaClass/ParentClass").InnerXml = mc.Parent.Name;

			xmlMetaClass.SelectSingleNode("MetaClass/TableName").InnerXml = mc.TableName;
			xmlMetaClass.SelectSingleNode("MetaClass/Description").InnerXml = mc.Description;

			if (mc.FieldListChangedSqlScript != null)
				xmlMetaClass.SelectSingleNode("MetaClass/FieldListChangedSqlScript").InnerXml = mc.FieldListChangedSqlScript;

			if (mc.Tag != null)
				xmlMetaClass.SelectSingleNode("MetaClass/Tag").InnerXml = Convert.ToBase64String((byte[])mc.Tag);

			root.AppendChild((XmlNode)xmlMetaClass);
		}

		private static void BackupMetaField(XmlNode root, MetaField mf)
		{
			if (root == null)
				throw new ArgumentNullException("root");
			if (mf == null)
				throw new ArgumentNullException("mf");

			XmlDocumentFragment xmlMetaField = root.OwnerDocument.CreateDocumentFragment();
			xmlMetaField.InnerXml = "<MetaField><Namespace></Namespace><Name></Name><FriendlyName></FriendlyName><Description></Description><DataType></DataType><Length></Length><AllowNulls></AllowNulls><SaveHistory></SaveHistory><AllowSearch></AllowSearch><MultiLanguageValue></MultiLanguageValue><Tag></Tag></MetaField>";

			xmlMetaField.SelectSingleNode("MetaField/Namespace").InnerXml = mf.Namespace;
			xmlMetaField.SelectSingleNode("MetaField/Name").InnerXml = mf.Name;
			xmlMetaField.SelectSingleNode("MetaField/FriendlyName").InnerXml = mf.FriendlyName;
			xmlMetaField.SelectSingleNode("MetaField/Description").InnerXml = mf.Description;
			xmlMetaField.SelectSingleNode("MetaField/DataType").InnerXml = mf.DataType.ToString();
			xmlMetaField.SelectSingleNode("MetaField/Length").InnerXml = mf.Length.ToString(_culture);
			xmlMetaField.SelectSingleNode("MetaField/AllowNulls").InnerXml = mf.AllowNulls.ToString();
			xmlMetaField.SelectSingleNode("MetaField/SaveHistory").InnerXml = mf.SaveHistory.ToString();
			xmlMetaField.SelectSingleNode("MetaField/AllowSearch").InnerXml = mf.AllowSearch.ToString();
			xmlMetaField.SelectSingleNode("MetaField/MultiLanguageValue").InnerXml = mf.MultilanguageValue.ToString();

			if (mf.Tag != null)
				xmlMetaField.SelectSingleNode("MetaField/Tag").InnerXml = Convert.ToBase64String((byte[])mf.Tag);

			if (mf.OwnerMetaClassIdList.Count > 0)
			{
				foreach (int MetaClassId in mf.OwnerMetaClassIdList)
				{
					MetaClass mc = MetaClass.Load(MetaClassId);

					XmlNode xmlOwnerMetaClass = (XmlNode)root.OwnerDocument.CreateElement("OwnerMetaClass");
					xmlOwnerMetaClass.InnerXml = mc.Name;

					xmlMetaField.SelectSingleNode("MetaField").AppendChild(xmlOwnerMetaClass);
				}
			}

			// Dictionary
			if (mf.Dictionary != null)
			{
				foreach (MetaDictionaryItem dicItem in mf.Dictionary)
				{
					XmlNode xmlDictionaryItem = (XmlNode)root.OwnerDocument.CreateElement("Dictionary");
					xmlDictionaryItem.InnerXml = dicItem.Value;
					xmlMetaField.SelectSingleNode("MetaField").AppendChild(xmlDictionaryItem);
				}
			}

			root.AppendChild((XmlNode)xmlMetaField);
		}

		#endregion

	}
}
