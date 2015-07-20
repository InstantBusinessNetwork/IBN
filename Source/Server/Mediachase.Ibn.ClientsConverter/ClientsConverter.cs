using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Mediachase.Database;
using Mediachase.Ibn.Business.VCard;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using MD45 = Mediachase.MetaDataPlus;
using MD47 = Mediachase.Ibn.Data;


namespace Mediachase.Ibn.Converter
{
	public class ClientsConverter
	{
		private int _commandTimeout;

		public event EventHandler<ClientsConverterEventArgs> Warning;

		public ClientsConverter(int commandTimeout)
		{
			_commandTimeout = commandTimeout;
		}

		public void Convert(DBHelper source, DBHelper target)
		{
			using (MD47.DataContext dataContext47 = new MD47.DataContext(string.Empty))
			{
				// Initialize metadata
				MD47.DataContext.Current = dataContext47;
				MD47.DataContext.Current.SqlContext.CommandTimeout = _commandTimeout;

				SqlTransaction previous45Transaction = MD45.MetaDataContext.Current.Transaction;
				SqlTransaction previous47Transaction = MD47.DataContext.Current.SqlContext.Transaction;
				//SqlTransaction previousDatabaseTransaction = Mediachase.IBN.Database.DbContext.Current.Transaction;

				using (DBTransaction tranSource = source.BeginTransaction())
				using (DBTransaction tran = target.BeginTransaction())
				{
					DBHelper2.Init(source);
					MD45.MetaDataContext.Current.Transaction = tranSource.SqlTran;
					MD47.DataContext.Current.SqlContext.Transaction = tran.SqlTran;
					//Mediachase.IBN.Database.DbContext.Current.Transaction = tran.SqlTran;
					try
					{
						Dictionary<int, OrganizationEntity> organizationsById = new Dictionary<int, OrganizationEntity>();
						Dictionary<int, ContactEntity> contactsById = new Dictionary<int, ContactEntity>();

						ConvertOrganizations(organizationsById);

						string companyLocale = (string)target.RunTextScalar("SELECT [Locale] FROM [LANGUAGES] WHERE [IsDefault]=1");
						ConvertContacts(companyLocale, organizationsById, contactsById);

						UpdateObjects(target, organizationsById, contactsById);

						//Update email boxes
						UpdateXmlInDatabase(target, organizationsById, contactsById, new ProcessXmlDelegate(UpdateEmailBoxes), "EMailRouterPop3Box", "EMailRouterPop3BoxId", "Settings");

						// Update reports
						UpdateXmlInDatabase(target, organizationsById, contactsById, new ProcessXmlDelegate(UpdateReports), "Report", "ReportId", "IBNReportTemplate");

						tran.Commit();
					}
					finally
					{
						MD45.MetaDataContext.Current.Transaction = previous45Transaction;
						MD47.DataContext.Current.SqlContext.Transaction = previous47Transaction;
						//Mediachase.IBN.Database.DbContext.Current.Transaction = previousDatabaseTransaction;
					}
				}
			}
		}


		#region protected void SendWarning(string format, params object[] args)
		protected void SendWarning(string format, params object[] args)
		{
			if (Warning != null)
			{
				ClientsConverterEventArgs cea = new ClientsConverterEventArgs();
				cea.Message = string.Format(CultureInfo.CurrentUICulture, format, args);
				Warning(this, cea);
				if (cea.Cancel)
					throw new ClientsConverterException("Conversion has been canceled by user.");
			}
		}
		#endregion


		#region private static void ConvertOrganizations(Dictionary<int, OrganizationEntity> organizationsById)
		private static void ConvertOrganizations(Dictionary<int, OrganizationEntity> organizationsById)
		{
			Dictionary<int, Dictionary<string, object>> additionalFields = new Dictionary<int, Dictionary<string, object>>();

			// Load organizations list
			using (IDataReader reader = DBHelper2.DBHelper.RunTextDataReader("SELECT [OrgId],[OrgName] FROM [ORGANIZATIONS]"))
			{
				while (reader.Read())
				{
					int id = (int)reader["OrgId"];
					string name = (string)reader["OrgName"];

					Dictionary<string, object> additionalValues = new Dictionary<string, object>();
					additionalValues.Add("Name", name);

					additionalFields.Add(id, additionalValues);
				}
			}

			MD47.Meta.Management.MetaClass metaClass47 = MD47.DataContext.Current.MetaModel.MetaClasses[OrganizationEntity.GetAssignedMetaClassName()];
			MD45.Configurator.MetaClass metaClass45 = MD45.Configurator.MetaClass.Load("OrganizationsEx");

			MetadataPlusToMetadataConverter converter = new MetadataPlusToMetadataConverter(metaClass45, metaClass47, "old_", additionalFields);
			converter.CopyFields();
			converter.CopyEntities(organizationsById);
		}
		#endregion

		#region private void ConvertContacts(string companyLocale, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		private void ConvertContacts(string companyLocale, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		{
			Dictionary<string, OrganizationEntity> organizationsByName = new Dictionary<string, OrganizationEntity>();
			if (organizationsById != null && organizationsByName != null)
			{
				foreach (OrganizationEntity organization in organizationsById.Values)
				{
					organizationsByName.Add(organization.Name, organization);
				}
			}

			Regex urlValidator = new Regex("^[\\w-_./:\\?&=]+");

			foreach (VCard vcard in VCard.List())
			{
				int vcardId = vcard.VCardId;

				ContactEntity contact = BusinessManager.InitializeEntity<ContactEntity>(ContactEntity.GetAssignedMetaClassName());

				SafelySetStringProperty(contact, "FullName", vcard.FullName);
				SafelySetStringProperty(contact, "NickName", vcard.NickName);
				if (!string.IsNullOrEmpty(vcard.Url) && urlValidator.IsMatch(vcard.Url))
				{
					SafelySetStringProperty(contact, "WebSiteUrl", vcard.Url);
				}
				if (vcard.Birthday != DateTime.MinValue)
				{
					contact.BirthDate = vcard.Birthday;
				}
				SafelySetStringProperty(contact, "JobTitle", vcard.Title);
				SafelySetStringProperty(contact, "Role", vcard.Role);
				SafelySetStringProperty(contact, "Description", vcard.Description);
				contact.Gender = ConvertGender(vcard.Gender);

				ConvertVCardName(vcardId, contact);
				ConvertVCardAddress(vcardId, contact, companyLocale);
				ConvertVCardPhones(vcardId, contact);
				ConvertVCardEmails(vcardId, contact);
				ConvertVCardOrganization(vcard, contact, organizationsById, organizationsByName);

				// Save contact
				contact.PrimaryKeyId = BusinessManager.Create(contact);

				contactsById.Add(vcardId, contact);
			}
		}
		#endregion

		#region private static void ConvertVCardName(int vcardId, ContactEntity contact)
		private void ConvertVCardName(int vcardId, ContactEntity contact)
		{
			VCardName name = VCardName.List(vcardId);
			if (name != null)
			{
				SafelySetStringProperty(contact, "LastName", name.Family);
				SafelySetStringProperty(contact, "FirstName", name.Given);
				SafelySetStringProperty(contact, "MiddleName", name.Middle);
			}
		}
		#endregion
		#region private void ConvertVCardAddress(int vcardId, ContactEntity contact, string companyLocale)
		private void ConvertVCardAddress(int vcardId, ContactEntity contact, string companyLocale)
		{
			AddressEntity address = contact.Address as AddressEntity;
			VCardAddress[] addresses = VCardAddress.List(vcardId);
			if (addresses.Length > 0)
			{
				VCardAddress addressCard = addresses[0];

				address.AddressType = 1; // Primary
				SafelySetStringProperty(address, "City", addressCard.Locality);
				SafelySetStringProperty(address, "Country", addressCard.Country);
				SafelySetStringProperty(address, "Name", GetAddressName(addressCard.AddressType, companyLocale));
				SafelySetStringProperty(address, "PostalCode", addressCard.PostalCode);
				SafelySetStringProperty(address, "Region", addressCard.Region);

				// Line1 = "Street, Extra"
				string street = addressCard.Street;
				if (street != null)
				{
					street = street.Trim();
				}
				string extra = addressCard.ExtraAddress;
				if (extra != null)
				{
					extra = extra.Trim();
				}

				string line1 = street;
				if (!string.IsNullOrEmpty(line1))
				{
					if (!string.IsNullOrEmpty(extra))
					{
						line1 += ", ";
						line1 += extra;
					}
				}
				else
				{
					line1 = extra;
				}

				if (!string.IsNullOrEmpty(line1))
				{
					SafelySetStringProperty(address, "Line1", line1);
				}
			}
		}
		#endregion
		#region private void ConvertVCardPhones(int vcardId, ContactEntity contact)
		private void ConvertVCardPhones(int vcardId, ContactEntity contact)
		{
			VCardTelephone[] phones = VCardTelephone.List(vcardId);
			foreach (VCardTelephone phone in phones)
			{
				switch (phone.TelephoneType)
				{
					case VCardTelephoneTypes.Cell:
						SafelySetStringProperty(contact, "MobilePhone", phone.Number);
						break;
					case VCardTelephoneTypes.Fax:
						SafelySetStringProperty(contact, "Fax", phone.Number);
						break;
					case VCardTelephoneTypes.Pref:
						SafelySetStringProperty(contact, "Telephone1", phone.Number);
						break;
					case VCardTelephoneTypes.Work:
						SafelySetStringProperty(contact, "Telephone2", phone.Number);
						break;
					case VCardTelephoneTypes.Home:
						SafelySetStringProperty(contact, "Telephone3", phone.Number);
						break;
				}
			}
		}
		#endregion
		#region private void ConvertVCardEmails(int vcardId, ContactEntity contact)
		private void ConvertVCardEmails(int vcardId, ContactEntity contact)
		{
			VCardEmail[] emails = VCardEmail.List(vcardId);
			foreach (VCardEmail email in emails)
			{
				switch (email.EmailType)
				{
					case VCardEmailTypes.Work:
						SafelySetStringProperty(contact, "EMailAddress1", email.Value);
						break;
					case VCardEmailTypes.Home:
						SafelySetStringProperty(contact, "EMailAddress2", email.Value);
						break;
					case VCardEmailTypes.None:
						SafelySetStringProperty(contact, "EMailAddress3", email.Value);
						break;
				}
			}
		}
		#endregion
		#region private void ConvertVCardOrganization(VCard vcard, ContactEntity contact, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<string, OrganizationEntity> organizationsByName)
		private void ConvertVCardOrganization(VCard vcard, ContactEntity contact, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<string, OrganizationEntity> organizationsByName)
		{
			OrganizationEntity organization = null;

			int organizationId = vcard.OrganizationId;
			if (organizationId < 0)
			{
				VCardOrganization organizationCard = VCardOrganization.List(vcard.VCardId);
				if (organizationCard != null)
				{
					if (!string.IsNullOrEmpty(organizationCard.Unit))
					{
						SafelySetStringProperty(contact, "OrganizationUnit", organizationCard.Unit);
					}

					string organizationName = organizationCard.Name;
					if (!string.IsNullOrEmpty(organizationName))
					{
						if (organizationName.Length > 255)
						{
							organizationName = organizationName.Substring(0, 255);
						}

						// Find or create organization
						if (organizationsByName.ContainsKey(organizationName))
						{
							organization = organizationsByName[organizationName];
						}
						else
						{
							organization = BusinessManager.InitializeEntity<OrganizationEntity>(OrganizationEntity.GetAssignedMetaClassName());
							SafelySetStringProperty(organization, "Name", organizationName);
							organization.PrimaryKeyId = BusinessManager.Create(organization);
						}
					}
				}
			}
			else
			{
				if (organizationsById.ContainsKey(organizationId))
				{
					organization = organizationsById[organizationId];
				}
			}

			if (organization != null)
			{
				contact.OrganizationId = organization.PrimaryKeyId;
			}
		}
		#endregion

		#region private static void UpdateObjects(DBHelper target, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		private static void UpdateObjects(DBHelper target, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		{
			string[] tableNames = { "DOCUMENTS", "EVENTS", "GROUPS", "INCIDENTS", "PROJECTS", "TODO" };

			UpdateTables(target, tableNames, "OrgId", "OrgUid", organizationsById);
			UpdateTables(target, tableNames, "VCardId", "ContactUid", contactsById);
		}
		#endregion

		#region private static void UpdateXmlInDatabase(DBHelper target, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById, ProcessXmlDelegate processXml, string tableName, string keyColumnName, string valueColumnName)
		private static void UpdateXmlInDatabase(DBHelper target, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById, ProcessXmlDelegate processXml, string tableName, string keyColumnName, string valueColumnName)
		{
			Dictionary<int, string> stringsById = new Dictionary<int, string>();

			using (IDataReader reader = target.RunTextDataReader("SELECT [" + keyColumnName + "],[" + valueColumnName + "] FROM [" + tableName + "]"))
			{
				while (reader.Read())
				{
					string value = reader[valueColumnName].ToString();
					if (!string.IsNullOrEmpty(value))
					{
						int key = (int)reader[keyColumnName];
						stringsById.Add(key, value);
					}
				}
			}

			foreach (int key in stringsById.Keys)
			{
				string xml = stringsById[key];

				XmlDataDocument doc = new XmlDataDocument();
				try
				{
					doc.LoadXml(xml);
					bool isModified = processXml(doc, organizationsById, contactsById);
					if (isModified)
					{
						target.RunText("UPDATE [" + tableName + "] SET [" + valueColumnName + "]=@value WHERE [" + keyColumnName + "]=@key"
							, DBHelper.MP("@key", SqlDbType.Int, key)
							, DBHelper.MP("@value", SqlDbType.NText, doc.OuterXml)
							);
					}
				}
				catch (XmlException)
				{
				}
			}
		}
		#endregion


		#region private void SafelySetStringProperty(EntityObject entity, string name, string value)
		private void SafelySetStringProperty(EntityObject entity, string name, string value)
		{
			string result = value;

			MD47.Meta.Management.MetaClass metaClass = MD47.DataContext.Current.MetaModel.MetaClasses[entity.MetaClassName];
			MD47.Meta.Management.MetaField metaField = metaClass.Fields[name];
			int maximumLength = metaField.Attributes.GetValue<int>(MD47.Meta.Management.McDataTypeAttribute.StringMaxLength, int.MaxValue);

			if (value != null && value.Length > maximumLength)
			{
				result = value.Substring(0, maximumLength);
				SendWarning("Maximum string length for {0}.{1} is {2} characters. The string '{3}' will be truncated to '{4}'.", entity.MetaClassName, name, maximumLength, value, result);
			}

			entity[name] = result;
		}
		#endregion

		#region private static int? ConvertGender(string gender)
		private static int? ConvertGender(string gender)
		{
			int? result = null;

			if (!string.IsNullOrEmpty(gender))
			{
				switch (gender[0])
				{
					case 'M':
					case 'm':
					case 'М':
					case 'м':
						result = 2; // Male
						break;
					case 'F':
					case 'f':
					case 'Ж':
					case 'ж':
						result = 1; // Female
						break;
				}
			}

			return result;
		}
		#endregion

		#region private static string GetAddressName(VCardAddressType type, string locale)
		private static string GetAddressName(VCardAddressTypes type, string locale)
		{
			string name;

			if (locale == "ru-RU")
			{
				switch (type)
				{
					case VCardAddressTypes.Home:
						name = "Домашний";
						break;
					case VCardAddressTypes.Work:
						name = "Рабочий";
						break;
					case VCardAddressTypes.None:
					default:
						name = "Основной";
						break;
				}
			}
			else
			{
				switch (type)
				{
					case VCardAddressTypes.Home:
						name = "Home";
						break;
					case VCardAddressTypes.Work:
						name = "Work";
						break;
					case VCardAddressTypes.None:
					default:
						name = "General";
						break;
				}
			}

			return name;
		}
		#endregion


		#region private static void UpdateTables<T>(DBHelper target, string[] tableNames, string keyColumnName, string valueColumnName, Dictionary<int, T> entitiesById) where T : EntityObject
		private static void UpdateTables<T>(DBHelper target, string[] tableNames, string keyColumnName, string valueColumnName, Dictionary<int, T> entitiesById) where T : EntityObject
		{
			const string keyParameterName = "@key";
			const string valueParameterName = "@value";

			string query = BuildUpdateQuery(tableNames, keyColumnName, valueColumnName, keyParameterName, valueParameterName);

			// Get only those ids, which are referenced by other objects
			int[] referencedIds = SelectIds(tableNames, keyColumnName);

			foreach (int id in referencedIds)
			{
				if (entitiesById.ContainsKey(id))
				{
					T entity = entitiesById[id];

					target.RunText(query
						, DBHelper.MP(keyParameterName, SqlDbType.Int, id)
						, DBHelper.MP(valueParameterName, SqlDbType.UniqueIdentifier, entity.PrimaryKeyId.Value)
					);
				}
			}
		}
		#endregion

		#region private static int[] SelectIds(string[] tableNames, string columnName)
		private static int[] SelectIds(string[] tableNames, string columnName)
		{
			List<int> list = new List<int>();

			string query = BuildSelectQuery(tableNames, columnName);
			using (IDataReader reader = DBHelper2.DBHelper.RunTextDataReader(query))
			{
				while (reader.Read())
				{
					object value = reader[columnName];
					if (value != null && value != DBNull.Value)
					{
						list.Add((int)value);
					}
				}
			}

			return list.ToArray();
		}
		#endregion

		#region private static string BuildSelectQuery(string[] tableNames, string columnName)
		private static string BuildSelectQuery(string[] tableNames, string columnName)
		{
			StringBuilder builder = new StringBuilder();

			foreach (string tableName in tableNames)
			{
				if (builder.Length > 0)
				{
					builder.Append("\r\nUNION ");
				}

				builder.Append("SELECT [");
				builder.Append(columnName);
				builder.Append("] FROM [");
				builder.Append(tableName);
				builder.Append("]");
			}

			return builder.ToString();
		}
		#endregion

		#region private static string BuildUpdateQuery(string[] tableNames, string keyColumnName, string valueColumnName, string keyParameterName, string valueParameterName)
		private static string BuildUpdateQuery(string[] tableNames, string keyColumnName, string valueColumnName, string keyParameterName, string valueParameterName)
		{
			StringBuilder builder = new StringBuilder();

			foreach (string tableName in tableNames)
			{
				if (builder.Length > 0)
				{
					builder.Append("\r\n");
				}

				builder.Append("UPDATE [");
				builder.Append(tableName);
				builder.Append("] SET [");
				builder.Append(valueColumnName);
				builder.Append("]=");
				builder.Append(valueParameterName);
				builder.Append(" WHERE [");
				builder.Append(keyColumnName);
				builder.Append("]=");
				builder.Append(keyParameterName);
			}

			return builder.ToString();
		}
		#endregion


		private delegate bool ProcessXmlDelegate(XmlDataDocument doc, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById);

		#region private bool UpdateEmailBoxes(XmlDataDocument doc, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		private bool UpdateEmailBoxes(XmlDataDocument doc, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		{
			bool isModified = false;

			ReplaceIdWithUid(doc, ref isModified, "OrgId", "OrgUid", organizationsById);
			ReplaceIdWithUid(doc, ref isModified, "VCardId", "ContactUid", contactsById);

			return isModified;
		}
		#endregion

		#region private bool UpdateReports(XmlDataDocument doc, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		private bool UpdateReports(XmlDataDocument doc, Dictionary<int, OrganizationEntity> organizationsById, Dictionary<int, ContactEntity> contactsById)
		{
			bool isModified = false;

			List<XmlNode> nodesToRemove = new List<XmlNode>();

			string[] fieldNames = { "EventClient", "DocClient", "IncClient", "PrjClient", "ToDoClient" };

			foreach (string fieldName in fieldNames)
			{
				XmlNode filterNode = doc.SelectSingleNode("/IBNReportTemplate/Filters/Filter[@FieldName='" + fieldName + "']");
				if (filterNode != null)
				{
					foreach (XmlNode valueNode in filterNode.SelectNodes("Value"))
					{
						EntityObject entity = null;

						string value = valueNode.InnerText;
						if (!string.IsNullOrEmpty(value))
						{
							int id;
							if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
							{
								if (id > 0) // Organization
								{
									if (organizationsById.ContainsKey(id))
									{
										entity = organizationsById[id];
									}
								}
								else // Contact
								{
									id = -id;
									if (contactsById.ContainsKey(id))
									{
										entity = contactsById[id];
									}
								}
							}
						}

						if (entity != null)
						{
							valueNode.InnerText = entity.PrimaryKeyId.ToString();
						}
						else
						{
							nodesToRemove.Add(valueNode);
						}

						isModified = true;
					}
				}
			}

			foreach (XmlNode node in nodesToRemove)
			{
				node.ParentNode.RemoveChild(node);
			}

			return isModified;
		}
		#endregion

		#region private static void ReplaceIdWithUid<T>(XmlDataDocument doc, ref bool isModified, string oldName, string newName, Dictionary<int, T> entitiesById) where T : EntityObject
		private static void ReplaceIdWithUid<T>(XmlDataDocument doc, ref bool isModified, string oldName, string newName, Dictionary<int, T> entitiesById) where T : EntityObject
		{
			XmlNode organization = doc.SelectSingleNode("/IncidentBoxDocument/Block/Param[@Name='" + oldName + "']");
			if (organization != null)
			{
				string newValue = Guid.Empty.ToString("D");

				XmlAttribute valueAttribute = organization.Attributes["Value"];
				if (valueAttribute != null)
				{
					string oldValue = valueAttribute.Value;
					if (!string.IsNullOrEmpty(oldValue))
					{
						XmlDocument docValue = new XmlDocument();
						docValue.LoadXml(oldValue);
						XmlNode valueNode = docValue.SelectSingleNode("/int");
						if (valueNode != null)
						{
							string valueString = valueNode.InnerText;
							if (!string.IsNullOrEmpty(valueString))
							{
								int id;
								if (int.TryParse(valueString, out id))
								{
									if (id > 0 && entitiesById.ContainsKey(id))
									{
										newValue = entitiesById[id].PrimaryKeyId.Value.ToString();
									}
								}
							}
						}
					}
				}

				XmlNode parentNode = organization.ParentNode;
				parentNode.RemoveChild(organization);

				XmlNode newParamNode = parentNode.AppendChild(doc.CreateElement("Param"));
				newParamNode.Attributes.Append(doc.CreateAttribute("Name")).Value = newName;
				newParamNode.Attributes.Append(doc.CreateAttribute("Value")).Value = newValue;

				isModified = true;
			}
		}
		#endregion
	}
}
