using System;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Mediachase.Sync.Core
{
    [Flags]
    public enum DebugSeverity
    {
		Debug		= 0x00,
        Info		= 0x01,
        Trace		= 0x03,
        Error		= 0x07,
        MessageBox	= 0x0f
    }

	/// <summary>
	/// The debugAssistant class. 
	/// </summary>
	public class DebugAssistant
	{
		public static string LogFilePath { get; set; }

        /// <summary>
		/// The static constructor.
		/// </summary>
		static DebugAssistant()
		{
        }

		/// <summary>
		/// Saftey dance.
		/// </summary>
		protected DebugAssistant()
		{
			// just so no one goes creating an instance of DebugAssistant
		}

        private static bool LogInFile(string msg)
        {

            if (!string.IsNullOrEmpty(LogFilePath))
            {
                lock(typeof(DebugAssistant))
                {
                    using(StreamWriter stream = File.AppendText(LogFilePath))
                    {
                        stream.WriteLine(msg);
                    }
                }

            }

            return true;
        }

        private static bool LogTrace(string msg, DebugSeverity severity)
        {
            Trace.WriteLine(msg);
            return true;
        }

        private static bool LogInDialog(string msg, DebugSeverity severity)
        {
            MessageBox.Show(msg);
            return true;
        }


        public static void Log(string msg)
        {
            Log(DebugSeverity.Trace, msg);
        }

        public static void Log(DebugSeverity severity, string msg, params object[] param)
        {
            Func<String, DebugSeverity, bool> logFunction = null;

            if((severity & DebugSeverity.MessageBox) == DebugSeverity.MessageBox)
            {
                logFunction = LogInDialog;
            }
            else if((severity & DebugSeverity.Trace) == DebugSeverity.Trace)
            {
                logFunction = LogTrace;
            }

            if(logFunction != null)
            {
				msg = FormatMsg(severity, msg, param);
                logFunction(msg, severity);
				LogInFile(msg);
            }
        }

		private static string FormatMsg(DebugSeverity severity, string msg, object[] param)
		{
			string retVal = msg;
			if (param != null)
			{
				for (int i = 0; i < param.Length; i++)
				{
					msg.Replace("{" + i + "}", param[i].ToString());
				}
			}

            return String.Format("{2} [{3}] :{0} #{1}", msg, Thread.CurrentThread.ManagedThreadId, 
                                                                DateTime.Now.ToString("g"), severity.ToString());
           
		}
	}
}
