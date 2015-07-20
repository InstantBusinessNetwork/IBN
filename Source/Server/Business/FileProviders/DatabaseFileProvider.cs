using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn.XmlTools;

namespace Mediachase.IBN.Business.FileProviders
{
	class DatabaseFileProvider : FileProvider
	{
		public override FileDescriptor[] GetFiles(string structureName, string searchPattern, Selector[] selectors)
		{
			// Table fields:
			//
			// ModuleName
			// StructureName
			// FileName
			// FileContent

			throw new NotImplementedException();
		}
	}
}
