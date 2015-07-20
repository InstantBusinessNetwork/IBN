using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.IBN.Business.Documents.Configuration
{
		/// <summary>
	/// 
	/// </summary>
	[ConfigurationCollection(typeof(TemplateProcessorElement))]
	public class TemplateProcessorElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateProcessorElementCollection"/> class.
		/// </summary>
		public TemplateProcessorElementCollection()
		{

		}

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </summary>
		/// <returns>
		/// A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new TemplateProcessorElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			TemplateProcessorElement tElement = (TemplateProcessorElement)element;
			return tElement.Extension;
		}

		/// <summary>
		/// Gets or sets the <see cref="Mediachase.Ibn.Core.Business.Configuration.TemplateProcessorElement"/> at the specified index.
		/// </summary>
		/// <value></value>
		public TemplateProcessorElement this[int index]
		{
			get
			{
				return (TemplateProcessorElement)base.BaseGet(index);
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

		/// <summary>
		/// Adds the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Add(TemplateProcessorElement item)
		{
			this.BaseAdd(item);
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			base.BaseClear();
		}

		/// <summary>
		/// Finds the name of the type.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public string FindTypeName(string extension)
		{
			foreach (TemplateProcessorElement element in this)
			{
				if (string.Compare(extension, element.Extension, true) == 0)
					return element.TypeName;
			}

			return null;
		}
	}
}
