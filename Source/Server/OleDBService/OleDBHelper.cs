using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Mediachase.Ibn.Service
{
	/// <summary>
	/// Summary description for McOleDbHelper.
	/// </summary>
	internal class OleDBHelper : MarshalByRefObject, IMCOleDBHelper
	{
		internal OleDBHelper()
		{
		}

		private static string[] GetSheetNames(OleDbConnection cnn)
		{
			ArrayList list = new ArrayList();

			DataTable tbl = new DataTable();
			tbl = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "Table"});

			foreach(DataRow row in tbl.Rows)
			{
				list.Add(row["TABLE_NAME"]);
			}
			return (string[])list.ToArray(typeof(string));
		}

		private static void RemoveEmptyRows(DataTable table)
		{
			for (int Index = table.Rows.Count - 1 ; Index >= 0 ; Index--)
			{
				DataRow row = table.Rows[Index];
				bool	empty = true;
				for (int FieldIndex = 0 ; FieldIndex < table.Columns.Count ; FieldIndex++)
				{
					if (row[FieldIndex]!=null && DBNull.Value != row[FieldIndex])
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

		private static DataSet Parse(string FileName)
		{
			DataSet ds = new DataSet();

			OleDbConnection cnn = null;

			// OZ [2008-03-26] Excell 2007 Addon 
			// http://support.microsoft.com/kb/247412
			// Should be installed 2007 Office System Driver: Data Connectivity Components http://www.microsoft.com/downloads/details.aspx?FamilyID=7554F536-8C28-4598-9B72-EF94E038C891&displaylang=en
			if (string.Compare(Path.GetExtension(FileName), ".xlsx",true)==0)
			{
				cnn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties=\"Excel 12.0;HDR=Yes\"");
			}
			//
			else
			{
				cnn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName + ";Extended Properties=\"Excel 8.0;HDR=Yes\"");
			}

			cnn.Open();
			try
			{
				string[] sheetNames = GetSheetNames(cnn);
				foreach (string sheetName in sheetNames)
				{
					/*
					if (SheetName.EndsWith("$Print_Titles") || SheetName.EndsWith("$Print_Area")
						|| SheetName.EndsWith("$Database") || SheetName.EndsWith("$Criteria")
						|| SheetName.EndsWith("$Data_form") || SheetName.EndsWith("$Sheet_Title"))
					{
					}
					else
					*/
					if (sheetName.EndsWith("$") || sheetName.EndsWith("$'") || sheetName.EndsWith("$\""))
					{
						DataTable	table = new DataTable();

						OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + sheetName + "]",	cnn);
						da.Fill(table);
						table.TableName = sheetName;

						RemoveEmptyRows(table);

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

		#region Implementation of IMcOleDbHelper
		public DataSet ConvertExcelToDataSet(string FileName)
		{
			return Parse(FileName);
		}
		#endregion
	}
}
