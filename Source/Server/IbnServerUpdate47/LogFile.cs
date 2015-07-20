using System;
using System.Globalization;
using System.IO;

namespace IbnServerUpdate
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
			if(filePath != null)
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

		public static void WriteMessageFormat(string format, params object[] args)
		{
			WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		public static void WriteLine(string message)
		{
			try
			{
				if(_Writer != null)
				{
					_Writer.WriteLine(message);
					_Writer.Flush();
				}
				Console.WriteLine(message);
			}
			catch
			{
			}
		}

		public static void Show()
		{
			if(_FilePath != null)
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
