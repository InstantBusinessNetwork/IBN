using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Mediachase.IBN.Business.Documents;

namespace Mediachase.Ibn.Documents.Office2007
{
	/// <summary>
	/// Represents excell work sheet 2007 template document processor.
	/// </summary>
	/// <remarks>
	/// Supports .xlsx and extensions only.
	/// </remarks>
	public class XlsxTemplateDocumentProcessor : DefaultDocumentTemplateProcessor
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="XlsxTemplateDocumentProcessor"/> class.
		/// </summary>
		public XlsxTemplateDocumentProcessor()
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

			using (SpreadsheetDocument sheetDocument = SpreadsheetDocument.Open(memoryStream, true))
			{
				foreach (IdPartPair item in sheetDocument.WorkbookPart.Parts)
				{
					string docText = null;
					using (StreamReader sr = new StreamReader(item.OpenXmlPart.GetStream()))
					{
						docText = sr.ReadToEnd();
					}

					string newDocText = ModifyText(docText);

					if (newDocText != docText)
					{
						using (StreamWriter sw = new StreamWriter(item.OpenXmlPart.GetStream(FileMode.Create)))
						{
							sw.Write(newDocText);
						}
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
