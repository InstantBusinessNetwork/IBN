using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Mediachase.MetaDataPlus.Import.Parser
{
	public enum ExcelVersion
	{
		None = 0,
		Excel50 = 5,
		Excel60 = 6,
		Excel70 = 7,
		Excel80 = 8
	}

	/// <summary>
	/// Summary description for OleDbIncomingDataParser.
	/// </summary>
	public class OleDBIncomingDataParser : IIncomingDataParser
	{
		private string _connectionString = null;

		public string Name
		{
			get
			{
				return "OleDbIncomingDataParser";
			}
		}

		public string Description
		{
			get
			{
				return "OleDbIncomingDataParser";
			}
		}

		private static void RemoveEmptyRows(DataTable table)
		{
			for (int Index = table.Rows.Count - 1; Index >= 0; Index--)
			{
				DataRow row = table.Rows[Index];
				bool empty = true;
				for (int FieldIndex = 0; FieldIndex < table.Columns.Count; FieldIndex++)
				{
					if (row[FieldIndex] != null && DBNull.Value != row[FieldIndex])
					{
						empty = false;
						break;
					}
				}
				if (empty)
					row.Delete();
			}
			table.AcceptChanges();
		}

		private static string[] GetSheetNames(OleDbConnection con)
		{
			ArrayList list = new ArrayList();

			DataTable tbl = new DataTable();
			tbl = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

			foreach (DataRow row in tbl.Rows)
			{
				list.Add(row["TABLE_NAME"]);
			}
			return (string[])list.ToArray(typeof(string));
		}

		private static DataTable Parse(OleDbConnection con, string sheetName, bool tryParse)
		{
			DataTable table = new DataTable();

			OleDbDataAdapter da = new OleDbDataAdapter((tryParse ? "SELECT TOP 5 " : "SELECT ") + "* FROM [" + sheetName + "]", con);
			da.Fill(table);
			table.TableName = sheetName;

			RemoveEmptyRows(table);
			return table;
		}

		public virtual DataSet Parse(string fileName, Stream stream)
		{
			if (stream != null)
			{
				throw new NotSupportedException();
			}
			DataSet ds = new DataSet();

			OleDbConnection cnn = new OleDbConnection(_connectionString.Replace("$1", fileName));
			cnn.Open();
			try
			{
				string[] SheetNames = GetSheetNames(cnn);
				foreach (string SheetName in SheetNames)
				{
					/*
					if (SheetName.EndsWith("$Print_Titles") || SheetName.EndsWith("$Print_Area")
						|| SheetName.EndsWith("$Database") || SheetName.EndsWith("$Criteria")
						|| SheetName.EndsWith("$Data_form") || SheetName.EndsWith("$Sheet_Title"))
					{
					}
					else
					*/
					if (SheetName.EndsWith("$") || SheetName.IndexOf("$") == -1)
					{
						DataTable table = Parse(cnn, SheetName, false);

						if (table.Rows.Count != 0)
							ds.Tables.Add(table);
					}
				}
			}
			finally
			{
				cnn.Close();
			}
			return ds;
		}

		public virtual bool CanParse(string fileName, Stream stream)
		{
			if (stream != null)
			{
				return false;
			}
			try
			{
				OleDbConnection cnn = new OleDbConnection(_connectionString.Replace("$1", fileName));
				cnn.Open();
				try
				{
					string[] SheetNames = GetSheetNames(cnn);
					foreach (string SheetName in SheetNames)
					{
						Parse(cnn, SheetName, false);
					}
				}
				finally
				{
					cnn.Close();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public OleDBIncomingDataParser(string connectionString)
		{
			_connectionString = connectionString;
		}

		public OleDBIncomingDataParser(ExcelVersion excelVersion)
			: this(excelVersion, true)
		{
		}

		public OleDBIncomingDataParser(ExcelVersion excelVersion, bool withHeader)
			: this("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=$1;Extended Properties=\"Excel " +
					((int)excelVersion).ToString(CultureInfo.InvariantCulture) + ".0;HDR=" + (withHeader ? "Yes" : "No") + "\"")
		{
		}
	}
}
