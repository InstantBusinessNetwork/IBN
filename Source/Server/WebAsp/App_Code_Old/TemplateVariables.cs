using System;
using System.Collections.Specialized;

namespace Mediachase.Ibn.WebAsp
{
	/// <summary>
	/// Summary description for TemplateVariables.
	/// </summary>
	public class TemplateVariables
	{
		private NameValueCollection m_vars = new NameValueCollection();

		public TemplateVariables()
		{
		}

		public string this[string name]
		{
			get
			{
				if(name != null)
					name = name.ToLower();
				return m_vars[name];
			}
			set
			{
				if(name != null)
					name = name.ToLower();
				m_vars[name] = value;
			}
		}
	}
}
