using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml;

using Mediachase.Database;

namespace IbnServerUpdate
{
	internal class CompanyInfo
	{
		internal string Id { get; set; }
		internal int OldId { get; set; }
		internal bool IsActive { get; set; }
		internal string Host { get; set; }
		internal string Port { get; set; }
		internal string Database { get; set; }
		internal DateTime Created { get; set; }
		internal int CompanyType { get; set; }

		internal string CompanyLogo { get; set; }
		internal string HomePageImage { get; set; }

		private CompanyInfo()
		{
		}

		#region internal static CompanyInfo[] ListCompanies(XmlDocument doc)
		internal static CompanyInfo[] ListCompanies(DBHelper dbHelper)
		{
			List<CompanyInfo> list = new List<CompanyInfo>();

			string previousDatabase = dbHelper.Database;
			try
			{
				dbHelper.Database = Settings.SqlDatabase;

				using (IDataReader reader = dbHelper.RunTextDataReader("SELECT [company_id],[is_active],[domain],[port],[db_name],[creation_date],[company_type],[company_logo],[hp_image] FROM [COMPANIES]"))
				{
					while (reader.Read())
					{
						CompanyInfo company = new CompanyInfo();

						company.OldId = (int)reader["company_id"];
						company.IsActive = (bool)reader["is_active"];
						company.Host = reader["domain"].ToString();
						company.Port = reader["port"].ToString();
						company.Database = reader["db_name"].ToString();
						company.Created = (DateTime)reader["creation_date"];
						company.CompanyType = Convert.ToInt32((byte)reader["company_type"]);
						company.CompanyLogo = EncodeData(reader["company_logo"]);
						company.HomePageImage = EncodeData(reader["hp_image"]);

						list.Add(company);
					}
				}
			}
			finally
			{
				dbHelper.Database = previousDatabase;
			}

			return list.ToArray();
		}
		#endregion


		#region private static string EncodeData(byte[] data)
		private static string EncodeData(object data)
		{
			string result = null;

			if (data != null && data != DBNull.Value)
				result = Convert.ToBase64String((byte[])data);

			return result;
		}
		#endregion
	}
}
