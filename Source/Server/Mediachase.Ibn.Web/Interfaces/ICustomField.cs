using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Web.Interfaces
{
	/// <summary>
	/// Интерфейс используется для контролов-примитивов старой метадаты MetaDataPlus
	/// </summary>
	public interface ICustomField
	{
		object Value { get; set; }
		string FieldName { get; set; }
		bool AllowEmptyValues { get; set; }
	}
}
