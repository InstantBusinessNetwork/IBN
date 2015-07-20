using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.SQLQueryCreator;


namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QMetaLoader.
	/// </summary>
	public class QMetaLoader
	{
		protected static DbType MetaDataType2DbType(MetaDataType type)
		{
			switch (type)
			{
				case MetaDataType.BigInt:
					return DbType.Int64;
				case MetaDataType.Binary:
					return DbType.Binary;
				case MetaDataType.Bit:
					return DbType.Boolean;
				case MetaDataType.Char:
					return DbType.String;
				case MetaDataType.DateTime:
					return DbType.DateTime;
				case MetaDataType.Decimal:
					return DbType.Decimal;
				case MetaDataType.Float:
					return DbType.Double;
				case MetaDataType.Image:
					return DbType.Binary;
				case MetaDataType.Int:
					return DbType.Int32;
				case MetaDataType.Money:
					return DbType.Decimal;
				case MetaDataType.NChar:
					return DbType.String;
				case MetaDataType.NText:
					return DbType.String;
				case MetaDataType.NVarChar:
					return DbType.String;
				case MetaDataType.Real:
					return DbType.Single;
				case MetaDataType.UniqueIdentifier:
					return DbType.Guid;
				case MetaDataType.SmallDateTime:
					return DbType.Date;
				case MetaDataType.SmallInt:
					return DbType.Int16;
				case MetaDataType.SmallMoney:
					return DbType.Decimal;
				case MetaDataType.Text:
					return DbType.String;
				case MetaDataType.Timestamp:
					return DbType.Binary;
				case MetaDataType.TinyInt:
					return DbType.Byte;
				case MetaDataType.VarBinary:
					return DbType.Binary;
				case MetaDataType.VarChar:
					return DbType.String;
				case MetaDataType.Variant:
					return DbType.VarNumeric;
				case MetaDataType.Numeric:
					return DbType.VarNumeric;
				case MetaDataType.Sysname:
					return DbType.String;
				// MetaData Types [11/16/2004]
				case MetaDataType.Integer:
					return DbType.Int32;
				case MetaDataType.Boolean:
					return DbType.Boolean;
				case MetaDataType.Date:
					return DbType.DateTime;
				case MetaDataType.Email:
					return DbType.String;
				case MetaDataType.Url:
					return DbType.String;
				case MetaDataType.ShortString:
					return DbType.String;
				case MetaDataType.LongString:
					return DbType.String;
				case MetaDataType.LongHtmlString:
					return DbType.String;
//				case MetaDataType.DictionarySingleValue:
//					return DbType.StringFixedLength;
//				case MetaDataType.DictionaryMultivalue:
//					return DbType.StringFixedLength;
//				case MetaDataType.EnumSingleValue:
//					return DbType.StringFixedLength;
//				case MetaDataType.EnumMultivalue:
//					return DbType.StringFixedLength;
//					//MultiStringValue = 38,
//				case MetaDataType.File:
//					return DbType.StringFixedLength;
//				case MetaDataType.ImageFile:
//					return DbType.StringFixedLength;
				default:
					throw new NotSupportedException("Unsuported MetaDataType");
			}
		}

		private QMetaLoader()
		{
		}

		#region LoadMetaField
		public static void LoadMetaField(QObject qObject, string metaClassName)
		{
			LoadMetaField(qObject, MetaClass.Load(metaClassName));
		}

		public static void LoadMetaField(QObject qObject, int metaClassId)
		{
			LoadMetaField(qObject, MetaClass.Load(metaClassId));
		}

		public static void LoadMetaField(QObject qObject, MetaClass currentClass)
		{
			foreach (MetaField field in currentClass.UserMetaFields)
			{
				if (qObject.Fields[string.Format("{1}", qObject.OwnerTable, field.Name)] != null)
					continue;

				switch (field.DataType)
				{
					case MetaDataType.DictionaryMultivalue:
					case MetaDataType.EnumMultivalue:
						QField MFieldValue = new QField(string.Format("{1}", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							"Value",
							DbType.String,
							QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
							new QFieldJoinRelation[]{
														new QFieldJoinRelation(qObject.OwnerTable,currentClass.TableName,qObject.KeyField.DBName,"ObjectId"),
														new QFieldJoinRelation(currentClass.TableName,"MetaMultivalueDictionary",field.Name,"MetaKey"),
														new QFieldJoinRelation("MetaMultivalueDictionary","MetaDictionary","MetaDictionaryId","MetaDictionaryId",new SimpleFilterCondition(new QField("MetaFieldId"),field.Id.ToString(),SimpleFilterType.Equal))
													}
							);

						QField MFieldId = new QField(string.Format("{1}Id", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							"MetaDictionaryId",
							DbType.String,
							QFieldUsingType.Abstract,
							new QFieldJoinRelation[]{
														new QFieldJoinRelation(qObject.OwnerTable,currentClass.TableName,qObject.KeyField.DBName,"ObjectId"),
														new QFieldJoinRelation(currentClass.TableName,"MetaMultivalueDictionary",field.Name,"MetaKey"),
														new QFieldJoinRelation("MetaMultivalueDictionary","MetaDictionary","MetaDictionaryId","MetaDictionaryId",new SimpleFilterCondition(new QField("MetaFieldId"),field.Id.ToString(),SimpleFilterType.Equal))
													}
							);

						qObject.Fields.Add(MFieldValue);
						qObject.Fields.Add(MFieldId);

						qObject.Dictionary.Add(new QDictionary(MFieldId, MFieldValue, string.Format("SELECT MetaDictionaryId as Id, Value FROM MetaDictionary WHERE MetaFieldId = {0}", field.Id)));
						break;
					case MetaDataType.DictionarySingleValue:
					case MetaDataType.EnumSingleValue:
						QField SFieldValue = new QField(string.Format("{1}", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							"Value",
							DbType.String,
							QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
							new QFieldJoinRelation[]{
														new QFieldJoinRelation(qObject.OwnerTable,currentClass.TableName,qObject.KeyField.DBName,"ObjectId"),
														new QFieldJoinRelation(currentClass.TableName,"MetaDictionary",field.Name,"MetaDictionaryId",new SimpleFilterCondition(new QField("MetaFieldId"),field.Id.ToString(),SimpleFilterType.Equal) )
													}
							);

						QField SFieldId = new QField(string.Format("{1}Id", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							"MetaDictionaryId",
							DbType.String,
							QFieldUsingType.Abstract,
							new QFieldJoinRelation[]{
														new QFieldJoinRelation(qObject.OwnerTable,currentClass.TableName,qObject.KeyField.DBName,"ObjectId"),
														new QFieldJoinRelation(currentClass.TableName,"MetaDictionary",field.Name,"MetaDictionaryId",new SimpleFilterCondition(new QField("MetaFieldId"),field.Id.ToString(),SimpleFilterType.Equal) )
													}
							);

						qObject.Fields.Add(SFieldValue);
						qObject.Fields.Add(SFieldId);

						qObject.Dictionary.Add(new QDictionary(SFieldId, SFieldValue, string.Format("SELECT MetaDictionaryId as Id, Value FROM MetaDictionary WHERE MetaFieldId = {0}", field.Id)));
						break;
					case MetaDataType.Image:
					case MetaDataType.Binary:
						// Ignory [12/8/2004]
						break;
					case MetaDataType.File:
					case MetaDataType.ImageFile:
						qObject.Fields.Add(new QField(string.Format("{1}", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							"FileName",
							DbType.String,
							QFieldUsingType.Field | QFieldUsingType.Grouping,
							new QFieldJoinRelation[]{
														new QFieldJoinRelation(qObject.OwnerTable,currentClass.TableName,qObject.KeyField.DBName,"ObjectId"),
														new QFieldJoinRelation(currentClass.TableName,"MetaFileValue",field.Name,"MetaKey")
													}
							));

						break;
					case MetaDataType.LongHtmlString:
					case MetaDataType.LongString:
						qObject.Fields.Add(new QField(string.Format("{1}", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							field.Name,
							MetaDataType2DbType(field.DataType),
							QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
							new QFieldJoinRelation(qObject.OwnerTable, currentClass.TableName, qObject.KeyField.DBName, "ObjectId"))
							);
						break;
					default:
						qObject.Fields.Add(new QField(string.Format("{1}", qObject.OwnerTable, field.Name),
							field.FriendlyName,
							field.Name,
							MetaDataType2DbType(field.DataType),
							QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
							new QFieldJoinRelation(qObject.OwnerTable, currentClass.TableName, qObject.KeyField.DBName, "ObjectId"))
							);
						break;
				}
			}
		}
		#endregion

		#region AddHeaderInformation
		public static void AddHeaderInformation(XmlDocument xmlDoc, QMaker qMaker)
		{
			StringBuilder sbH = new StringBuilder();
			StringWriter tempWriterH = new StringWriter(sbH);
			XmlTextWriter writerH = new XmlTextWriter(tempWriterH);

			writerH.WriteStartElement("Headers");

			foreach (QField gField in qMaker.Groups)
			{
				writerH.WriteStartElement("Header");
				writerH.WriteAttributeString("Name", gField.Name);
				writerH.WriteAttributeString("Description", gField.FriendlyName);
				writerH.WriteEndElement();//Header
			}
			foreach (QField fField in qMaker.Fields)
			{
				writerH.WriteStartElement("Header");
				writerH.WriteAttributeString("Name", fField.Name);
				writerH.WriteAttributeString("Description", fField.FriendlyName);
				writerH.WriteEndElement();//Header
			}

			writerH.WriteEndElement();//Headers
			writerH.Flush();

			XmlDocumentFragment xmlHeaders = xmlDoc.CreateDocumentFragment();
			xmlHeaders.InnerXml = sbH.ToString();

			xmlDoc.SelectSingleNode("Report").AppendChild(xmlHeaders);
		}

		#endregion

		#region AddSortInformation
		public static void AddSortInformation(XmlDocument xmlDoc, IBNReportTemplate template)
		{
			XmlDocument xmlTemplateDoc = template.CreateXMLTemplate();

			XmlNode xmlSorting = xmlTemplateDoc.SelectSingleNode("IBNReportTemplate/Sorting");

			XmlNode xmlSortingR = xmlDoc.ImportNode(xmlSorting, true);
			xmlDoc.SelectSingleNode("Report").AppendChild(xmlSortingR);
		}

		#endregion

		#region AddFilterInformation
		public static void AddFilterInformation(XmlDocument xmlDoc, IBNReportTemplate template)
		{
			XmlDocument xmlTemplateDoc = template.CreateXMLTemplate();

			XmlNode xmlFilters = xmlTemplateDoc.SelectSingleNode("IBNReportTemplate/Filters");

			XmlNode xmlFiltersR = xmlDoc.ImportNode(xmlFilters, true);
			xmlDoc.SelectSingleNode("Report").AppendChild(xmlFiltersR);
		}

		#endregion

	}
}
