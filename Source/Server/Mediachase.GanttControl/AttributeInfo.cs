using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Gantt
{
	public class AttributeInfo
	{
		private string _name;
		private string _value;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public AttributeInfo(string name, string value)
		{
			_name = name;
			_value = value;
		}
	}
}
