using System;

namespace Mediachase.IBN.Business.EMail
{
	public delegate void ExceptionAbortEventHandler(object sender, ExceptionAbortEventArgs args);

	public class ExceptionAbortEventArgs: EventArgs
	{
		private Exception _ex = null;
		private bool _bAbort = false;

		public ExceptionAbortEventArgs(Exception ex)
		{
			_ex = ex;
		}

		public Exception Exception
		{
			get { return _ex; }
		}
        
		public bool Abort
		{
			get { return _bAbort; }
			set { _bAbort = value; }
		}
	
	}
}
