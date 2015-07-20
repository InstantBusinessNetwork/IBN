using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Clients
{
	/// <summary>
	/// Represents VCard Utility.
	/// </summary>
	public static class VCardUtil
	{
		#region Const
		private const string FullName = "FullName";
		private const string NickName = "NickName";
		private const string WebSiteUrl = "WebSiteUrl";
		private const string BirthDate = "BirthDate";
		private const string JobTitle = "JobTitle";
		private const string Role = "Role";
		private const string Description = "Description";
		private const string Gender = "Gender";
		private const string Organization = "Organization";
		private const string OrganizationUnit = "OrganizationUnit";

		// By VCardName
		private const string LastName = "LastName";
		private const string FirstName = "FirstName";
		private const string MiddleName = "MiddleName";

		// VCardEMail
		private const string EMailAddress1 = "EMailAddress1";
		private const string EMailAddress2 = "EMailAddress2";
		private const string EMailAddress3 = "EMailAddress3";

		// VCardTelephone
		private const string MobilePhone = "MobilePhone";
		private const string Telephone1 = "Telephone1";
		private const string Telephone2 = "Telephone2";
		private const string Telephone3 = "Telephone3";
		private const string Fax = "Fax";

		// VCardAddress
		private const string Address_Country = "Country";
		private const string Address_PostalCode = "PostalCode";
		private const string Address_Region = "Region";
		private const string Address_Line1 = "Line1";
		private const string Address_Line2 = "Line2";
		private const string Address_Line3 = "Line3";
		private const string Address_City = "City";

		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="VCardUtil"/> class.
		/// </summary>
		static VCardUtil()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		public static DataSet ConvertFile(string filePath)
		{
			string streamText = String.Empty;
			using (StreamReader sr = new StreamReader(filePath, Encoding.Default, true))
			{
				streamText = sr.ReadToEnd();
			}
			return Convert(streamText);
		}

		public static DataSet Convert(string vcardText)
		{
			if (vcardText == null)
				throw new ArgumentNullException("vcardText");

			// Initialize
			DataSet retVal = new DataSet();
			DataTable table = retVal.Tables.Add("VCard");

			#region Initialize table
			// By VCard
			table.Columns.Add(VCardUtil.FullName, typeof(string)); // FullName
			table.Columns.Add(VCardUtil.NickName, typeof(string)); // NickName
			table.Columns.Add(VCardUtil.WebSiteUrl, typeof(string)); // Url
			table.Columns.Add(VCardUtil.BirthDate, typeof(DateTime)); // Birthday
			table.Columns.Add(VCardUtil.JobTitle, typeof(string)); // Title
			table.Columns.Add(VCardUtil.Role, typeof(string)); // Role
			table.Columns.Add(VCardUtil.Description, typeof(string)); // Description
			table.Columns.Add(VCardUtil.Gender, typeof(string)); // Gender
			table.Columns.Add(VCardUtil.Organization, typeof(string)); // Organization/Name
			table.Columns.Add(VCardUtil.OrganizationUnit, typeof(string)); // Organization/Unit

			// By VCardName
			table.Columns.Add(VCardUtil.LastName, typeof(string)); // Family
			table.Columns.Add(VCardUtil.FirstName, typeof(string)); // Given
			table.Columns.Add(VCardUtil.MiddleName, typeof(string)); // Middle

			// VCardEMail
			table.Columns.Add(VCardUtil.EMailAddress1, typeof(string)); // 
			table.Columns.Add(VCardUtil.EMailAddress2, typeof(string)); // 
			table.Columns.Add(VCardUtil.EMailAddress3, typeof(string)); // 

			// VCardTelephone
			table.Columns.Add(VCardUtil.MobilePhone, typeof(string)); // 
			table.Columns.Add(VCardUtil.Telephone1, typeof(string)); // 
			table.Columns.Add(VCardUtil.Telephone2, typeof(string)); // 
			table.Columns.Add(VCardUtil.Telephone3, typeof(string)); // 
			table.Columns.Add(VCardUtil.Fax, typeof(string)); // 

			// VCardAddress
			table.Columns.Add(VCardUtil.Address_Country, typeof(string)); // Country
			table.Columns.Add(VCardUtil.Address_PostalCode, typeof(string)); // PostalCode
			table.Columns.Add(VCardUtil.Address_Region, typeof(string)); // Region
			table.Columns.Add(VCardUtil.Address_Line1, typeof(string)); // Street + ExtraAddress
			table.Columns.Add(VCardUtil.Address_Line2, typeof(string)); // 
			table.Columns.Add(VCardUtil.Address_Line3, typeof(string)); // 
			table.Columns.Add(VCardUtil.Address_City, typeof(string)); // Locality

			#endregion

			// Parser vcardText
			MatchCollection matches = Regex.Matches(vcardText,
				@"(?<contentline>(?<name>[\x30-\x39\x41-\x5A\x61-\x7A\-]+)(;(?<param>[^:]+))?:(?<value>[^\r]+)(\r\n)+)",
				RegexOptions.Compiled);

			string Name, Param, Value;

			for (int index = 0; index < matches.Count; index++)
			{
				DataRow row = table.NewRow();
				table.Rows.Add(row);

				Name = matches[index].Groups["name"].Value;
				Param = matches[index].Groups["param"].Value;
				Value = matches[index].Groups["value"].Value;

				int BeginVCardIndex = index;
				int EndVCardIndex = -1;

				if (Name == "BEGIN" && Value == "VCARD")
				{
					for (; index < matches.Count; index++)
					{
						Name = matches[index].Groups["name"].Value;
						Param = matches[index].Groups["param"].Value;
						Value = matches[index].Groups["value"].Value;

						if (Param.IndexOf("CHARSET=UTF-7") != -1)
						{
							Value = System.Text.Encoding.UTF7.GetString(Encoding.UTF8.GetBytes(Value));
						}

						if (Name == "END" && Value == "VCARD")
						{
							EndVCardIndex = index;
							break;
						}

						#region Fill Params
						switch (Name)
						{
							#region VCard
							case "FN":
								row[VCardUtil.FullName] = Value;
								break;
							case "NICKNAME":
								row[VCardUtil.NickName] = Value;
								break;
							case "BDAY":
								row[VCardUtil.BirthDate] = DateTime.Parse(Value);
								break;
							case "X-GENDER":
								row[VCardUtil.Gender] = Value;
								break;
							case "ROLE":
								row[VCardUtil.Role] = Value;
								break;
							case "TITLE":
								row[VCardUtil.JobTitle] = Value;
								break;
							case "URL":
								row[VCardUtil.WebSiteUrl] = Value;
								break;
							case "NOTE":
								row[VCardUtil.Description] = string.Empty;
								break;
							#endregion
							#region VCardName
							case "N":
								string[] splitNItems = Value.Split(';');

								if (splitNItems.Length > 0)
									row[VCardUtil.LastName] = splitNItems[0];

								if (splitNItems.Length > 1)
									row[VCardUtil.FirstName] = splitNItems[1];

								if (splitNItems.Length > 2)
									row[VCardUtil.MiddleName] = splitNItems[2];
								break;
							#endregion
							#region VCardOrganization
							case "ORG":
								string[] splitOrgItems = Value.Split(';');

								if (splitOrgItems.Length > 0)
									row[VCardUtil.Organization] = splitOrgItems[0];

								if (splitOrgItems.Length > 1)
									row[VCardUtil.OrganizationUnit] = splitOrgItems[1];
								break;
							#endregion
							#region EMail
							case "EMAIL":
								if (Param.IndexOf("PREF") != -1)
									row[VCardUtil.EMailAddress1] = Value;
								else if (Param.IndexOf("WORK") != -1)
									row[VCardUtil.EMailAddress1] = Value;
								else if (Param.IndexOf("HOME") != -1)
									row[VCardUtil.EMailAddress2] = Value;
								else
									row[VCardUtil.EMailAddress3] = Value;
								break;
							#endregion
							#region Adress
							case "ADR":
								string[] splitAdrItems = Value.Split(';');

								if (splitAdrItems.Length > 0)
									row[VCardUtil.Address_Line2] = splitAdrItems[0];
								if (splitAdrItems.Length > 1)
									row[VCardUtil.Address_Line1] = splitAdrItems[1];
								if (splitAdrItems.Length > 2)
									row[VCardUtil.Address_Line1] = splitAdrItems[2] + row[VCardUtil.Address_Line1];
								if (splitAdrItems.Length > 3)
									row[VCardUtil.Address_City] = splitAdrItems[3];
								if (splitAdrItems.Length > 4)
									row[VCardUtil.Address_Region] = splitAdrItems[4];
								if (splitAdrItems.Length > 5)
									row[VCardUtil.Address_PostalCode] = splitAdrItems[5];
								if (splitAdrItems.Length > 6)
									row[VCardUtil.Address_Country] = splitAdrItems[6];

								//if (Param.IndexOf("PREF") != -1)
								//    newVCardAdr.IsPrefered = true;

								//if (Param.IndexOf("WORK") != -1)
								//    newVCardAdr.VCardAddressTypeId = (int)VCardAddressType.Work;
								//else if (Param.IndexOf("HOME") != -1)
								//    newVCardAdr.VCardAddressTypeId = (int)VCardAddressType.Home;
								break;
							#endregion
							#region Phone
							case "TEL":
								if (Param.IndexOf("PREF") != -1)
									row[VCardUtil.Telephone1] = Value;
								else if (Param.IndexOf("WORK") != -1)
									row[VCardUtil.Telephone1] = Value;
								else if (Param.IndexOf("HOME") != -1)
									row[VCardUtil.Telephone2] = Value;
								else if (Param.IndexOf("FAX") != -1)
									row[VCardUtil.Fax] = Value;
								else if (Param.IndexOf("CELL") != -1)
									row[VCardUtil.MobilePhone] = Value;
								else
									row[VCardUtil.Telephone3] = Value;
								break;
							#endregion
						}

						#endregion
					}
				}
			}

			// Return Data Set
			return retVal;
		}
		#endregion

		#region Export
		/// <summary>
		/// Exports the specified contacts to VCard format.
		/// </summary>
		/// <param name="pkItems">The pk items.</param>
		/// <returns></returns>
		public static string Export(params PrimaryKeyId[] contactPrimaryKeyItems)
		{
			StringBuilder sb = new StringBuilder(256 * (contactPrimaryKeyItems.Length+1));

			foreach (PrimaryKeyId primaryKey in contactPrimaryKeyItems)
			{
				sb.Append(Export(primaryKey));
			}

			return sb.ToString();

		}

		/// <summary>
		/// Returns the empty if null.
		/// </summary>
		/// <param name="str">The STR.</param>
		/// <returns></returns>
		private static string ReturnEmptyIfNull(string str)
		{
			if (str == null)
				return string.Empty;
			return str;
		}

		/// <summary>
		/// Exports the specified contact to VCard format.
		/// </summary>
		/// <param name="contactPrimaryKey">The contact primary key.</param>
		/// <returns></returns>
		public static string Export(PrimaryKeyId contactPrimaryKey)
		{
			StringBuilder sb = new StringBuilder(256); 

			ContactEntity entity = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), contactPrimaryKey);

			sb.Append("BEGIN:VCARD\r\n");
			sb.Append("VERSION:2.1\r\n");

			#region Export Name Info
			// Export Name Info
			sb.AppendFormat("N:{0};{1};{2}\r\n",
				ReturnEmptyIfNull(entity.LastName),
				ReturnEmptyIfNull(entity.FirstName),
				ReturnEmptyIfNull(entity.MiddleName));
			#endregion

			#region Export Common Info
			// Export Common Info
			if (!string.IsNullOrEmpty(entity.FullName))
				sb.AppendFormat("FN:{0}\r\n", entity.FullName);

			if (!string.IsNullOrEmpty(entity.NickName))
				sb.AppendFormat("NICKNAME:{0}\r\n", entity.NickName);

			if (entity.BirthDate.HasValue && entity.BirthDate != DateTime.MinValue)
				sb.AppendFormat("BDAY:{0}\r\n", entity.BirthDate.Value.ToString("yyyy-MM-dd"));

			if (entity.Gender.HasValue)
				sb.AppendFormat("X-GENDER:{0}\r\n", GlobalResource.GetString(MetaEnum.GetFriendlyName(DataContext.Current.GetMetaClass(ContactEntity.GetAssignedMetaClassName()).Fields["Gender"].GetMetaType(), entity.Gender.Value)));

			if (!string.IsNullOrEmpty(entity.Role))
				sb.AppendFormat("ROLE:{0}\r\n", entity.Role);

			if (!string.IsNullOrEmpty(entity.JobTitle))
				sb.AppendFormat("TITLE:{0}\r\n", entity.JobTitle);

			if (!string.IsNullOrEmpty(entity.WebSiteUrl))
				sb.AppendFormat("URL:{0}\r\n", entity.WebSiteUrl);

			if (!string.IsNullOrEmpty(entity.Description))
				sb.AppendFormat("NOTE:{0}\r\n", entity.Description); 
			#endregion

			#region Export Organization Info
			// Export Organization Info
			if (entity.OrganizationId.HasValue)
			{
				sb.AppendFormat("ORG:{0};{1}\r\n",
					entity.Organization,
					entity.OrganizationUnit);
			} 
			#endregion

			#region Export EMails
			// Export EMails
			foreach (string emailFieldName in GetEmailFieldNames())
			{
				string email = (string)entity.Properties[emailFieldName].Value;

				if (string.IsNullOrEmpty(email))
					continue;

			    string emailTypeString = string.Empty;

			    if (emailFieldName == "EMailAddress1")
			        emailTypeString += ";PREF";

			    if (emailFieldName == "EMailAddress1")
			        emailTypeString += ";WORK";
			    else if (emailFieldName == "EMailAddress2")
			        emailTypeString += ";HOME";

			    emailTypeString += ";INTERNET";

				sb.AppendFormat("EMAIL{1}:{0}\r\n",
					email,
					emailTypeString);
			}
			
			#endregion

			#region Export Phones
			//// Export Phones
			foreach (string phoneFieldName in new string[] { "Fax", "MobilePhone", "Telephone1", "Telephone2", "Telephone3" })
			{
				string phone = (string)entity.Properties[phoneFieldName].Value;
				if (string.IsNullOrEmpty(phone))
					continue;

				string phoneTypeString = string.Empty;

				if (phoneFieldName == "Telephone1")
					phoneTypeString += ";PREF";
				else if (phoneFieldName == "Telephone1")
					phoneTypeString += ";WORK";
				else if (phoneFieldName == "Telephone2")
					phoneTypeString += ";HOME";
				else if (phoneFieldName == "Fax")
					phoneTypeString += ";FAX";
				else if (phoneFieldName == "MobilePhone")
					phoneTypeString += ";CELL";

				sb.AppendFormat("TEL{1}:{0}\r\n",
					phone,
					phoneTypeString);
			}
			
			#endregion

			#region Export Addresses
			// Export Addresses
			foreach (AddressEntity address in BusinessManager.List(AddressEntity.GetAssignedMetaClassName(), 
				new FilterElement[]{ FilterElement.EqualElement("ContactId", contactPrimaryKey)}))
			{
				string adrTypeString = string.Empty;

				if (address.IsDefaultContactElement)
					adrTypeString += ";PREF";

				//if (address.AddressType == Add)
				//    adrTypeString += ";WORK";
				//else if (vCardAdrr.VCardAddressTypeId == (int)VCardTelephoneType.Home)
				//    adrTypeString += ";HOME";

				sb.AppendFormat("ADR{0}:;{1};{2};{3};{4};{5};{6}\r\n",
					adrTypeString,
					string.Empty,
					address.Line1,
					address.City,
					address.Region,
					address.PostalCode,
					address.Country
					);
			}
			
			#endregion

			sb.Append("END:VCARD\r\n");

			return sb.ToString();
		}

		/// <summary>
		/// Gets the email field names.
		/// </summary>
		/// <returns></returns>
		private static string[] GetEmailFieldNames()
		{
			List<string> retVal = new List<string>();

			foreach (MetaField mf in DataContext.Current.GetMetaClass(ContactEntity.GetAssignedMetaClassName()).Fields)
			{
				if (mf.GetMetaType().Name == "EMail")
					retVal.Add(mf.Name);
			}

			return retVal.ToArray();
		}

		#endregion

	}
}
