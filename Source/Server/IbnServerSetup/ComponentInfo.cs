using System;
using System.Collections.Generic;
using System.Text;

namespace IbnServer
{
	internal class ComponentInfo
	{
		internal string FilePath { get; set; }
		internal bool Supports64Bit { get; set; }

		private ComponentInfo()
		{
		}

		internal ComponentInfo(string filePath, bool supports64Bit)
		{
			this.FilePath = filePath;
			this.Supports64Bit = supports64Bit;
		}
	}
}
