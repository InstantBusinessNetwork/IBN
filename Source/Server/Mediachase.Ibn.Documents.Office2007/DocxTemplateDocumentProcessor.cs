using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.IBN.Business.Documents;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace Mediachase.Ibn.Documents.Office2007
{
	/// <summary>
	/// Represents word document 2007 template document processor.
	/// </summary>
	/// <remarks>
	/// Supports .docx and extensions only.
	/// </remarks>
	public class DocxTemplateDocumentProcessor: DefaultDocumentTemplateProcessor
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DocxTemplateDocumentProcessor"/> class.
		/// </summary>
		public DocxTemplateDocumentProcessor()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		/// <summary>
		/// Converts the specified stream.
		/// </summary>
		/// <param name="extension"></param>
		/// <param name="stream">The stream.</param>
		/// <param name="keys">The keys.</param>
		/// <returns></returns>
		protected override System.IO.Stream Convert(string extension, System.IO.Stream stream, Dictionary<string, Mediachase.Ibn.Core.Business.EntityObject> keys)
		{
			// Create Temporary memory stream
			MemoryStream memoryStream = new MemoryStream();

			CopyStream(stream, memoryStream);

			memoryStream.Position = 0;

			using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(memoryStream, true))
			{
				string docText = null;
				using (StreamReader sr = new StreamReader(wordDocument.MainDocumentPart.GetStream()))
				{
					docText = sr.ReadToEnd();
				}

				string newDocText = ModifyText(docText);

				if (newDocText != docText)
				{
					using (StreamWriter sw = new StreamWriter(wordDocument.MainDocumentPart.GetStream(FileMode.Create)))
					{
						sw.Write(newDocText);
					}
				}
			}

			memoryStream.Position = 0;

			return memoryStream;
		}

		/// <summary>
		/// Copies the stream.
		/// </summary>
		/// <param name="srcStream">The SRC stream.</param>
		/// <param name="destStream">The dest stream.</param>
		private static void CopyStream(Stream srcStream, Stream destStream)
		{
			byte[] buffer = new byte[srcStream.Length];

			srcStream.Read(buffer, 0, buffer.Length);
			destStream.Write(buffer, 0, buffer.Length);
		}
		#endregion

		
	}
}
