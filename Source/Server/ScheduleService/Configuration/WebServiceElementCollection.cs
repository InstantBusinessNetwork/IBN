using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.Schedule.Service.Configuration
{
	/// <summary>
	/// Represents WebServiceElementCollection.
	/// </summary>
	[ConfigurationCollection(typeof(WebServiceElement))]
	public class WebServiceElementCollection : ConfigurationElementCollection
	{
		#region Const
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the <see cref="Mediachase.Schedule.Service.Configuration.WebServiceElement"/> at the specified index.
		/// </summary>
		/// <value></value>
		public WebServiceElement this[int index]
		{
			get
			{
				return (WebServiceElement)base.BaseGet(index);
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceElement"/> class.
		/// </summary>
		public WebServiceElementCollection()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new WebServiceElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
		/// <returns>
		/// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			WebServiceElement ruleElement = (WebServiceElement)element;
			return ruleElement.Url;
		}



		#endregion
	}
}
