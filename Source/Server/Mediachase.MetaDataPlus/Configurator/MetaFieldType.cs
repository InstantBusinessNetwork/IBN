using System;

namespace Mediachase.MetaDataPlus.Configurator
{
	/// <summary>
	/// Summary description for MetaFieldType.
	/// </summary>
	public enum MetaDataType
	{
		// SQL Data Types [11/16/2004]
		BigInt			= 0,
		Binary			= 1,
		Bit				= 2,
		Char			= 3,
		DateTime		= 4,
		Decimal			= 5,
		Float			= 6,
		Image			= 7,
		Int				= 8,
		Money			= 9,
		NChar			= 10,
		NText			= 11,
		NVarChar		= 12,
		Real			= 13,
		UniqueIdentifier= 14,
		SmallDateTime	= 15,
		SmallInt		= 16,
		SmallMoney		= 17,
		Text			= 18,
		Timestamp		= 19,
		TinyInt			= 20,
		VarBinary		= 21,
		VarChar			= 22,
		Variant			= 23,
		Numeric			= 24,
		Sysname			= 25,

		// MetaData Types [11/16/2004]
		Integer			= 26,
		Boolean			= 27,
		Date			= 28,
		Email			= 29,
		Url				= 30,
		ShortString		= 31,
		LongString		= 32,
		LongHtmlString	= 33,

		DictionarySingleValue = 34,
		DictionaryMultivalue = 35,
		EnumSingleValue		= 36,
		EnumMultivalue		= 37,
		StringDictionary	= 38,
		File				=	39,
		ImageFile			=	40,

		MetaObject			=	41
	}
}
