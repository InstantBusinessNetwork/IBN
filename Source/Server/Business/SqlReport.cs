using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using System.Data.SqlClient;


namespace Mediachase.IBN.Business
{

	class HexDump
	{
		/// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <returns></returns>
		public static string ToString(byte[] array)
		{
			return HexDump.ToString(array, array.Length, 8, 4 * 8, false);
		}

		/// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="count">The count.</param>
		/// <returns></returns>
		public static string ToString(byte[] array, int count)
		{
			return HexDump.ToString(array, count, 8, 4 * 8, false);
		}

		/// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="bytesSeparator">The bytes separator.</param>
		/// <param name="bytesNewLine">The bytes new line.</param>
		/// <param name="printChar">if set to <c>true</c> [print char].</param>
		/// <returns></returns>
		public static string ToString(byte[] array, int bytesSeparator, int bytesNewLine, bool printChar)
		{
			return HexDump.ToString(array, array.Length, bytesSeparator, bytesNewLine, printChar);
		}

		/// <summary>
		/// Obtains the String representation of this instance.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="count">The count.</param>
		/// <param name="bytesSeparator">The bytes separator.</param>
		/// <param name="bytesNewLine">The bytes new line.</param>
		/// <param name="printChar">if set to <c>true</c> [print char].</param>
		/// <returns></returns>
		public static string ToString(byte[] array, int count, int bytesSeparator, int bytesNewLine, bool printChar)
		{
			int len = count;
			StringBuilder sb = new StringBuilder(len * 4);
			for (int i = 0; i < len; i++)
			{
				// Переводим байт в шестнадцатиричное строковое предствление.
				sb.Append(array[i].ToString("X2"));
				sb.Append(" ");

				int i1 = i + 1;

				if (i1 % bytesSeparator == 0)
					sb.Append(" ");

				if (i1 % bytesNewLine == 0 || i1 == len)
				{
					if (printChar)
						PrintChars(array, i, bytesSeparator, bytesNewLine, sb);
					sb.Append("\r\n");
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Prints the chars.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="index">The index.</param>
		/// <param name="bytesSeparator">The bytes separator.</param>
		/// <param name="bytesNewLine">The bytes new line.</param>
		/// <param name="sb">The sb.</param>
		private static void PrintChars(byte[] array, int index, int bytesSeparator,
			int bytesNewLine, StringBuilder sb)
		{
			int len = array.Length;
			int i1 = index + 1;
			if (i1 == len)
			{
				// Дорисовываем пробелы для выравнивания строки 
				// в конце дампа.
				int bytesToEnd = bytesNewLine - len % bytesNewLine;
				int separatorsToEnd = bytesToEnd / bytesSeparator;
				sb.Append(' ', bytesToEnd * 3 + separatorsToEnd + 1);
			}

			// Выводим символьное представление дампа
			sb.Append(' ');
			for (int k = index / bytesNewLine * bytesNewLine; k <= index; k++)
			{
				char ch = (char)array[k];
				switch (ch)
				{
					case '\n': sb.Append(@"\n"); break;
					case '\r': sb.Append(@"\r"); break;
					case '\v': sb.Append(@"\v"); break;
					case '\\': sb.Append(@"\\"); break;
					case '\t': sb.Append(@"\t"); break;
					default: sb.Append(ch); break;
				}
			}
		}
	}

	/// <summary>
	/// Summary description for SqlReport.
	/// </summary>
	public class SqlReport
	{
		public static bool AskCustomSqlReportAccess()
		{
			return CheckCustomSqlReportAccess(false);
		}

		public static void DemandCustomSqlReportAccess()
		{
			CheckCustomSqlReportAccess(true);
		}

		private static bool CheckCustomSqlReportAccess(bool throwExceptions)
		{
			bool result = false;

			if (!Configuration.DisableCustomSqlReport)
			{
				if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
				{
					result = true;
				}
				else if (throwExceptions)
				{
					throw new AccessDeniedException();
				}
			}
			else if (throwExceptions)
			{
				throw new AccessDeniedException("It is prohibited to create custom reports on this server.");
			}

			return result;
		}

		private static void AppendText(StringBuilder output, string text)
		{
			output.Append(HttpUtility.HtmlEncode(text).Replace("\r\n", "<br />"));
		}

		private static void AppendElement(StringBuilder output, string name, string value)
		{
			AppendElement(output, name, value, null);
		}

		private static void AppendElement(StringBuilder output, string name, string value, string className)
		{
			output.Append("<");
			output.Append(name);

			if (!string.IsNullOrEmpty(className))
			{
				output.Append(" class=\"");
				output.Append(className);
				output.Append("\"");
			}

			if (value != null)
			{
				output.Append(">");
				AppendText(output, value);
				output.Append("</");
				output.Append(name);
			}
			else
			{
				output.Append(" /");
			}

			output.Append(">");
		}

		private static void AppendReportTable(StringBuilder output, IDataReader reader)
		{
			output.Append("<table>");
			output.Append("<tr>");
			for (int i = 0; i < reader.FieldCount; i++)
			{
				AppendElement(output, "th", reader.GetName(i).ToString());
			}
			output.Append("</tr>");

			while (reader.Read())
			{
				output.Append("<tr>");
				for (int i = 0; i < reader.FieldCount; i++)
				{
					output.Append("<td>");

					if (reader.IsDBNull(i))
					{
						AppendElement(output, "span", "NULL", "null");
					}
					else if (reader[i] is byte[])
					{
						AppendText(output, HexDump.ToString((byte[])reader[i]));
					}
					else
					{
						string value = reader[i].ToString();
						if (!string.IsNullOrEmpty(value))
						{
							AppendText(output, value);
						}
					}

					output.Append("</td>");
				}
				output.Append("</tr>");
			}
			output.Append("</table>");
		}

		private static bool ReadQuery(System.IO.StringReader reader, StringBuilder sqlCommandText)
		{
			while (true)
			{
				string line = reader.ReadLine();

				if (line == null)
				{
					return false;
				}

				string trimLine = line.Trim();

				if (trimLine != string.Empty)
				{
					if (string.Compare(trimLine, "GO", true) == 0)
						return true;
					else
					{
						sqlCommandText.Append(line);
						sqlCommandText.Append("\r\n");
					}
				}
			}
		}

		public static string ExecuteScript(string script)
		{
			DemandCustomSqlReportAccess();

			StringBuilder output = new StringBuilder();

			output.Append("<?xml version='1.0' encoding='utf-8'?>");
			output.Append("<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.1//EN' 'http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd'>");
			output.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
			output.Append("<head>");
			output.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
			AppendElement(output, "title", "Custom SQL Report");
			output.Append("<style type='text/css'>");
			output.Append("table { border-collapse: collapse; border: 1px solid #000000; }");
			output.Append("th, td { vertical-align: top; border: 1px solid; padding: 4px; }");
			output.Append("span.null { font-style: italic; }");
			output.Append("p.error { color: red; }");
			output.Append("</style>");
			output.Append("</head>");

			output.Append("<body>");

			output.Append("<p>");
			AppendText(output, Configuration.Domain);
			output.Append("<br />");
			AppendText(output, DateTime.Now.ToString(CultureInfo.CurrentUICulture));
			output.Append("</p>");

			DbTransaction tran = DbTransaction.Begin();
			try
			{
				
				// O.R. [2009-10-13]: Set context info
				DbHelper2.RunText("DECLARE @BinaryId BINARY(128) SET @BinaryId = CAST(@UserId AS BINARY(128)) SET CONTEXT_INFO @BinaryId",
					DbHelper2.mp("@UserId", SqlDbType.Int, Security.CurrentUser.UserID));

				using (System.IO.StringReader reader = new System.IO.StringReader(script))
				{
					bool bContueRead = true;
					StringBuilder sqlCommandText = new StringBuilder();

					while (bContueRead)
					{
						bContueRead = ReadQuery(reader, sqlCommandText);
						if (sqlCommandText.Length > 0)
						{
							string query = sqlCommandText.ToString();
							AppendElement(output, "p", query);

							try
							{
								using (IDataReader rd = DbHelper2.RunTextDataReader(query))
								{
									do
									{
										AppendReportTable(output, rd);
									}
									while (rd.NextResult());
								}
							}
							catch (Exception e)
							{
								Trace.WriteLine(e.ToString());
								AppendElement(output, "p", e.ToString(), "error");
							}
							sqlCommandText = new StringBuilder();
						}
					}
				}
			}
			catch
			{
			}
			output.Append("</body>");
			output.Append("</html>");

			tran.Rollback();

			return output.ToString();
		}
	}
}
