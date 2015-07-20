using System;
using System.Diagnostics;
using System.Globalization;
using System.Web;

namespace Mediachase.Ibn
{
	/// <summary>
	/// Represents log.
	/// </summary>
	internal static class Log
	{
		const string sLogName = IbnConst.EventLogName;
		const string sSourceName = Constants.Name + " " + IbnConst.VersionMajorDotMinor;

		[ThreadStatic]
		private static EventLog _eventLog;
		private static object _lockObject = new object();

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <returns></returns>
		private static EventLog GetLog()
		{
			if (_eventLog == null)
			{
				_eventLog = new EventLog();
				((System.ComponentModel.ISupportInitialize)(_eventLog)).BeginInit();
				_eventLog.Log = sLogName;
				_eventLog.Source = sSourceName;
				((System.ComponentModel.ISupportInitialize)(_eventLog)).EndInit();
			}
			return _eventLog;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		internal static string Name { get { return sLogName; } }

		/// <summary>
		/// Gets the source.
		/// </summary>
		/// <value>The source.</value>
		internal static string Source { get { return sSourceName; } }

		/// <summary>
		/// Writes the entry.
		/// </summary>
		/// <param name="message">The message.</param>
		internal static void WriteEntry(string message)
		{
			WriteEntry(message, EventLogEntryType.Information);
		}

		/// <summary>
		/// Writes the entry.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="type">The type.</param>
		internal static void WriteEntry(string message, EventLogEntryType type)
		{
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				message = string.Format(CultureInfo.InvariantCulture, "Host: {0}\r\n\r\n{1}", HttpContext.Current.Request.Url.Host, message);
			}

			try
			{
				EventLog log = GetLog();
				if (log != null)
				{
					lock (_lockObject)
					{
						log.WriteEntry(message, type);
					}
				}
			}
			catch { }
		}

		/// <summary>
		/// Writes the error.
		/// </summary>
		/// <param name="message">The message.</param>
		internal static void WriteError(string message)
		{
			WriteEntry(message, EventLogEntryType.Error);
		}

		/// <summary>
		/// Writes the exception.
		/// </summary>
		/// <param name="ex">The ex.</param>
		internal static void WriteException(Exception ex)
		{
			WriteEntry(string.Format(CultureInfo.InvariantCulture, "Exception:\r\n{0}\r\n\r\n ", ex), EventLogEntryType.Error);
		}

		/// <summary>
		/// Writes the exception.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="ex">The ex.</param>
		internal static void WriteException(string location, Exception ex)
		{
			WriteEntry(string.Format(CultureInfo.InvariantCulture, "Location: {0}\r\n\r\nException:\r\n{1}\r\n\r\n ", location, ex), EventLogEntryType.Error);
		}


		/// <summary>
		/// Writes the information.
		/// </summary>
		/// <param name="message">The message.</param>
		internal static void WriteInformation(string message)
		{
			WriteEntry(message, EventLogEntryType.Information);
		}

		/// <summary>
		/// Writes the warning.
		/// </summary>
		/// <param name="message">The message.</param>
		internal static void WriteWarning(string message)
		{
			WriteEntry(message, EventLogEntryType.Warning);
		}
	}
}
