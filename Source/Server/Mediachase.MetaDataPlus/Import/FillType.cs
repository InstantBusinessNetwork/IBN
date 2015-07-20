using System;

namespace Mediachase.MetaDataPlus.Import
{
	[Flags]
	public enum FillTypes
	{
		None = 0,
		CopyValue = 1,
		Custom = 2,
		Default = 4
	}
}