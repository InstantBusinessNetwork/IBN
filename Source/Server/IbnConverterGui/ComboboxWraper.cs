using System;

namespace IbnConverterGui
{
	/// <summary>
	/// Summary description for ComboboxWraper.
	/// </summary>
	internal class ComboboxWraper
	{
		public string Text	=	string.Empty;
		public object Value;

		public ComboboxWraper()
		{
		}

		public ComboboxWraper(string Text, object Value)
		{
			this.Text	=  Text;
			this.Value = Value;
		}
		
		public override string ToString()
		{
			return Text;
		}
	}
}
