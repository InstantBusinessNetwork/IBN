using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.IBN.Business.Documents.Configuration
{
	/// <summary>
	/// Represents template processor element.
	/// </summary>
	public class TemplateProcessorElement : ConfigurationElement
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateProcessorElement"/> class.
		/// </summary>
		public TemplateProcessorElement()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateProcessorElement"/> class.
		/// </summary>
		/// <param name="extension">The extension.</param>
		/// <param name="typeName">Name of the type.</param>
		public TemplateProcessorElement(string extension, string typeName)
		{
			this.Extension = extension;
			this.TypeName = typeName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the extension.
		/// </summary>
		/// <value>The extension.</value>
		[ConfigurationProperty("extension", IsRequired = true, IsKey = true)]
		public string Extension
		{
			get { return (string)base["extension"]; }
			set { base["Extension"] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the type.
		/// </summary>
		/// <value>The name of the type.</value>
		[ConfigurationProperty("type")]
		public string TypeName
		{
			get { return (string)base["type"]; }
			set { base["type"] = value; }
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
