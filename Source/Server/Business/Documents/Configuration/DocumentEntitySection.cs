using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Mediachase.IBN.Business.Documents.Configuration
{
	/// <summary>
	/// Represents document entity section.
	/// </summary>
	public class DocumentEntitySection : ConfigurationSection
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentEntitySection"/> class.
		/// </summary>
		public DocumentEntitySection()
		{
		}
		#endregion

		#region Properties
		[ConfigurationProperty("templateProcessors", IsDefaultCollection = true)]
		public TemplateProcessorElementCollection TemplateProcessors
		{
			get
			{
				return (TemplateProcessorElementCollection)base["templateProcessors"];
			}
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
