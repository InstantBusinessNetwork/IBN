using System;
using System.Globalization;
using System.IO;

namespace Update
{
	internal class LogFile
	{
		static string _FilePath;
		static StreamWriter _Writer;

		private LogFile()
		{
		}

		public static void Open(string filePath)
		{
			_FilePath = filePath;
			if (!string.IsNullOrEmpty(filePath))
				_Writer = new StreamWriter(filePath, true);
		}

		public static void Close()
		{
			if(_Writer != null)
			{
				_Writer.Flush();
				_Writer.Close();
				_Writer = null;
			}
		}

		public static void WriteFormatted(string format, params object[] args)
		{
			WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		public static void WriteLine(params string[] messageParts)
		{
			try
			{
				foreach (string part in messageParts)
				{
					if (_Writer != null)
						_Writer.Write(part);
					Console.Write(part);
				}

				if (_Writer != null)
				{
					_Writer.WriteLine();
					_Writer.Flush();
				}
				Console.WriteLine();
			}
			catch
			{
			}
		}

		public static void Show()
		{
			if (!string.IsNullOrEmpty(_FilePath))
			{
				using (System.Diagnostics.Process process = new System.Diagnostics.Process())
				{
					process.StartInfo.FileName = "notepad.exe";
					process.StartInfo.Arguments = _FilePath;
					process.Start();
				}
			}
		}
	}
}
