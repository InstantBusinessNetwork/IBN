using System;
using System.Collections;
using System.Collections.Specialized;

namespace Mediachase.Ibn.ControlSystem
{
	/// <summary>
	/// Summary description for IbnControlInfo.
	/// </summary>
	public class IbnControlInfo
	{
		private string _name;
		private string _type;
		private NameValueCollection _parameters = new NameValueCollection();
		private DefaultAccessControlList _dacl = new DefaultAccessControlList();

		public IbnControlInfo(string name, string type)
		{
			_name = name;
			_type = type;
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Type
		{
			get
			{
				return _type;
			}
		}

		public NameValueCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public DefaultAccessControlList DefaultAccessControlList
		{
			get
			{
				return _dacl;
			}
		}
	}
}
