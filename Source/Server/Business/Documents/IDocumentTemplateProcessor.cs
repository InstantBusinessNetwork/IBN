using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.IBN.Business.Documents
{
	public interface IDocumentTemplateProcessor
	{
		Stream Convert(string extension, Stream stream, Dictionary<string, EntityObject> keys);
	}
}