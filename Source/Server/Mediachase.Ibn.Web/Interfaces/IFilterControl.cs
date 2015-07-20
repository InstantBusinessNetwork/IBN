using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Web.Interfaces
{
	/// <summary>
	/// Интерфейс фильтр-примитовов для custom(xml) отчетов
	/// </summary>
	public interface IFilterControl
	{
		object Value { get; set; }
		string FilterTitle { get; set; }
		string FilterField { get; set; }
		string FilterType { get; }

		void InitControl(object reader);
	}

	public class IntFilterValue
	{
		public string TypeValue { get; set; }
		public string FirstValue { get; set; }
		public string SecondValue { get; set; }

		public IntFilterValue()
		{
		}

		public IntFilterValue(string type, string firstValue, string secondValue)
		{
			TypeValue = type;
			FirstValue = firstValue;
			SecondValue = secondValue;
		}
	}

	public class DateFilterValue
	{
		public string TypeValue { get; set; }
		public string FirstValue { get; set; }
		public string SecondValue { get; set; }

		public DateFilterValue()
		{
		}

		public DateFilterValue(string type, string firstValue, string secondValue)
		{
			TypeValue = type;
			FirstValue = firstValue;
			SecondValue = secondValue;
		}
	}

	public class TimeFilterValue
	{
		public string TypeValue { get; set; }
		public string FirstValue { get; set; }
		public string SecondValue { get; set; }

		public TimeFilterValue()
		{
		}

		public TimeFilterValue(string type, string firstValue, string secondValue)
		{
			TypeValue = type;
			FirstValue = firstValue;
			SecondValue = secondValue;
		}
	}
}
