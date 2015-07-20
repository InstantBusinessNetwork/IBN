using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Converter
{
	public class ConverterEventArgs : EventArgs
	{
		private string _message;
		private bool _cancel;

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
}
