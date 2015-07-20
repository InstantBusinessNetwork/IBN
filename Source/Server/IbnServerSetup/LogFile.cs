using System;
using System.Globalization;
using System.IO;

namespace IbnServer
{
	internal sealed class LogFile
	{
		static string _filePath;
		static StreamWriter _writer;

		private LogFile()
		{
		}

		public static void Open(string FilePath)
		{
			_filePath = FilePath;
			if(FilePath != null)
				_writer = new StreamWriter(FilePath, true);
		}

		public static void Close()
		{
			try
			{
				if(_writer != null)
				{
					_writer.Flush();
					_writer.Close();
					_writer = null;
				}
			}
			catch
			{
			}
		}

		public static void WriteMessageFormat(string format, params object[] args)
		{
			WriteMessage(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		public static void WriteMessage(string message)
		{
			try
			{
				if(_writer != null)
				{
					_writer.WriteLine(message);
					_writer.Flush();
				}
				Console.WriteLine(message);
			}
			catch{}
		}

		public static void Show()
		{
			if(_filePath != null)
			{
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo.FileName = "notepad.exe";
				proc.StartInfo.Arguments = _filePath;
				proc.Start();
			}
		}
	}
}
