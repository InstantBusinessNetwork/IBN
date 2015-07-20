using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mediachase.MetaDataPlus.Import.Parser
{
	/// <summary>
	/// Summary description for CsvIncomingDataParser.
	/// </summary>
	public class CsvIncomingDataParser : IIncomingDataParser
	{

		private string _sourceFolder;
		private bool _withHeader = true;
		private bool _multiline = true;
		private char _delimiter = ';';
		private char _textQualifier;
		private Encoding _encoding = Encoding.UTF8;

		public string Name
		{
			get
			{
				return "CsvIncomingDataParser";
			}
		}

		public string Description
		{
			get
			{
				return "CsvIncomingDataParser";
			}
		}

		protected void FillTableMetaData(DataTable table, string line)
		{
			if (table == null)
				throw new ArgumentNullException("table");
			if (line == null)
				throw new ArgumentNullException("line");

			CultureInfo culture = CultureInfo.InvariantCulture;
			string[] columns = line.Split(new char[] { _delimiter });

			for (int index = 0; index < columns.Length; index++)
			{
				if (_withHeader)
				{
					if (String.Empty.CompareTo(columns[index]) == 0)
					{
						int i = index;
						do
						{
							columns[index] = "F" + i.ToString(culture);
						}
						while (table.Columns.IndexOf(columns[index]) != -1);
					}
					table.Columns.Add(columns[index]);
				}
				else table.Columns.Add("F" + index.ToString(culture));
			}
		}

		protected void FillTable(string fullPath, DataTable table)
		{
			if (table == null)
				throw new ArgumentNullException("table");

			using (StreamReader reader = new StreamReader(fullPath, _encoding))
			{
				bool first = true;

				while (reader.Peek() >= 0)
				{
					string s = reader.ReadLine();
					StringBuilder value = new StringBuilder();
					int col_begin = 0, col_end = 0;

					if (first)
					{
						FillTableMetaData(table, s);
						first = false;
						if (_withHeader)
							continue;
					}
					DataRow row = table.NewRow();

					for (int index = 0; index < table.Columns.Count && col_end != -1; index++)
					{
						while (s.Length > col_begin && s[col_begin] == ' ')
						{
							col_begin++;
						}
						value.Length = 0;
						if (_textQualifier != '\0' && s.Length > col_begin && s[col_begin] == _textQualifier)
						{
							do
							{
								col_begin++;
								col_end = s.IndexOf(_textQualifier, col_begin);
								while (col_end == -1 && _multiline)
								{
									value.Append(s.Substring(col_begin));
									value.Append("\n");
									if (reader.Peek() == 0)
										throw new CsvException("Invalid file structure");
									s = reader.ReadLine();
									col_begin = 0;
									col_end = s.IndexOf(_textQualifier, col_begin);
								}

								if (col_end == -1)
									throw new CsvException("Invalid file structure");

								value.Append(s.Substring(col_begin, col_end - col_begin + 1));
								col_begin = col_end + 1;
							}
							while (s.Length > col_begin && s[col_begin] == _textQualifier);

							value.Length--;
						}
						col_end = s.IndexOf(_delimiter, col_begin);

						if (col_end != -1)
						{
							value.Append(s.Substring(col_begin, col_end - col_begin).TrimEnd(null));
							col_begin = col_end + 1;
						}
						else
							value.Append(s.Substring(col_begin).TrimEnd(null));

						row[index] = value.ToString();
					}
					table.Rows.Add(row);
				}
			}
		}

		public virtual DataSet Parse(string fileName, Stream stream)
		{
			if (stream != null)
			{
				throw new NotSupportedException();
			}
			DataSet ds = new DataSet();
			/*
						OleDbConnection cnn = new OleDbConnection(_connectionString);
						cnn.Open();
						try
						{
			*/
			DataTable table = new DataTable();

			FillTable(Path.Combine(_sourceFolder, fileName), table);
			/*
							OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [" + FileName + "]", cnn);
							da.Fill(table);
			*/
			table.TableName = fileName;

			ds.Tables.Add(table);
			/*
						}
						finally
						{
							cnn.Close();
						}
			*/
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
				Parse(fileName, stream);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public CsvIncomingDataParser (string sourceFolder, bool withHeader, char delimiter, char textQualifier, bool multiline, Encoding textEncoding)
			: this(sourceFolder, withHeader, delimiter, textQualifier, multiline)
		{
			_encoding = textEncoding;
		}

		public CsvIncomingDataParser(string sourceFolder, bool withHeader, char delimiter, char textQualifier, bool multiline)
			: this(sourceFolder, withHeader, delimiter, textQualifier) {
			_multiline = multiline;
		}

		public CsvIncomingDataParser(string sourceFolder, bool withHeader, char delimiter, char textQualifier)
			: this(sourceFolder, withHeader, delimiter)
		{
			_textQualifier = textQualifier;
		}

		public CsvIncomingDataParser(string sourceFolder, bool withHeader, char delimiter)
		{
			_sourceFolder = sourceFolder;
			_withHeader = withHeader;
			_delimiter = delimiter;
		}
	}
}
