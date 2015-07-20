using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mediachase.Ibn.Core.Business;
using System.Text.RegularExpressions;

namespace Mediachase.IBN.Business.Documents
{
	/// <summary>
	/// Represents default document template processor.
	/// </summary>
	/// <remarks>
	/// Supports .txt, .html and .xml extension only.
	/// </remarks>
	public class DefaultDocumentTemplateProcessor: IDocumentTemplateProcessor
	{
		#region Const
		public const string RegexKeyWordPattern = @"{(?<KeyName>\w+):(?<KeyValue>\w+)(:(?<OutputFormat>[^}]+))?}";
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultDocumentTemplateProcessor"/> class.
		/// </summary>
		public DefaultDocumentTemplateProcessor()
		{
		}
		#endregion

		#region Properties
		private Dictionary<string, Mediachase.Ibn.Core.Business.EntityObject> _keys;

		protected Dictionary<string, Mediachase.Ibn.Core.Business.EntityObject> InnerKeys
		{
			get { return _keys; }
			private set { _keys = value; }
		}
	
		#endregion

		#region Methods
		/// <summary>
		/// Converts the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="keys">The keys.</param>
		/// <returns></returns>
		protected virtual Stream Convert(string extension, Stream stream, Dictionary<string, Mediachase.Ibn.Core.Business.EntityObject> keys)
		{
			// Read Text From Stream
			string docText = null;
			Encoding encdoing = null;

			using (StreamReader sr = new StreamReader(stream, true))
			{
				encdoing = sr.CurrentEncoding;
				docText = sr.ReadToEnd();
			}

			// Modify Text
			docText = ModifyText(docText);

			// Create New Output stream
			MemoryStream outputStream = new MemoryStream();

			byte[] buffer = encdoing.GetBytes(docText);
			outputStream.Write(buffer, 0, buffer.Length);
			outputStream.Position = 0;

			return outputStream;
		}

		/// <summary>
		/// Modifies the text.
		/// </summary>
		/// <param name="docText">The doc text.</param>
		/// <returns></returns>
		protected virtual string ModifyText(string docText)
		{
			if (string.IsNullOrEmpty(docText))
				return docText;

			return Regex.Replace(docText, RegexKeyWordPattern, ReplaceKeyValue);
		}

		/// <summary>
		/// Replaces the key value.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		protected string ReplaceKeyValue(Match match)
		{
			string keyName = match.Groups["KeyName"].Value;
			string keyValue = match.Groups["KeyValue"].Value;
			string outputFormat = match.Groups["OutputFormat"].Value;

			try
			{
				if (this.InnerKeys.ContainsKey(keyName))
				{
					EntityObject entity = this.InnerKeys[keyName];

					if (entity != null)
					{
						object value = null;

						if (entity.Properties.Contains(keyValue))
							value = entity[keyValue];
						else if (keyValue == "PrimaryKeyId")
							value = entity.PrimaryKeyId.Value;

						if (value != null)
						{
							if (string.IsNullOrEmpty(outputFormat))
								return string.Format("{0}", value);
							else
								return string.Format("{0:" + outputFormat + "}", value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				return match.Value + " Replace Error: " + ex.ToString();
			}

			return match.Value;
		}
		#endregion



		#region IDocumentTemplateProcessor Members

		/// <summary>
		/// Converts the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="keys">The keys.</param>
		/// <returns></returns>
		System.IO.Stream IDocumentTemplateProcessor.Convert(string extension, System.IO.Stream stream, Dictionary<string, Mediachase.Ibn.Core.Business.EntityObject> keys)
		{
			this.InnerKeys = keys;

			return this.Convert(extension, stream, keys);
		}

		#endregion
	}
}
