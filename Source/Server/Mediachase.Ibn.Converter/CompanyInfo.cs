using System;
using System.Collections.Generic;
using System.Data;

using Mediachase.Database;


namespace Mediachase.Ibn.Converter
{
	/// <summary>
	/// Summary description for CompanyInfo.
	/// </summary>
	public class CompanyInfo
	{
		private string _database;
		private string _domain;
		private int _id;
		private string _title;

		public string Database
		{
			get { return _database; }
		}
		public string Domain
		{
			get { return _domain; }
		}
		public int Id
		{
			get { return _id; }
		}
		public string Title
		{
			get { return _title; }
		}

		private CompanyInfo()
		{
		}

		#region LoadList()
		internal static IList<CompanyInfo> LoadList(DBHelper source)
		{
			List<CompanyInfo> ret = new List<CompanyInfo>();
			using (IDataReader reader = source.RunTextDataReader("SELECT [company_id], [domain], [company_name], [db_name] FROM [COMPANIES]"))
			{
				while (reader.Read())
				{
					CompanyInfo item = Load(reader);
					ret.Add(item);
				}
			}
			return ret;
		}
		#endregion

		#region Load()
		internal static CompanyInfo Load(IDataRecord record)
		{
			CompanyInfo ret = new CompanyInfo();

			if (record != null)
			{
				ret._database = record["db_name"].ToString();
				ret._domain = record["domain"].ToString();
				ret._id = (int)record["company_id"];
				ret._title = record["company_name"].ToString();
			}

			return ret;
		}
		#endregion
	}
}
