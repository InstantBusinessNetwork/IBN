using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Web.UI.WebControls
{

	/// <summary>
	/// Summary description for Owner.
	/// </summary>
	[ParseChildren(false)]
	[ToolboxItem(false)]	
	public class Owner : Control, INamingContainer
	{
		private string _text;
		private string _key;
		private object _value;

		public Owner(object val, string text)
		{
			_value = val;
			_text = text;
		}

		public Owner()
		{
		}

		/// <summary>
		/// Gets or sets the value to compare with the appointments
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets or sets the text displayed in the Owner. 
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Gets or sets a string that identifies this Owner in its parent collection. 
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}
	}
}
