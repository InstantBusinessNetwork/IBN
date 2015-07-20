using System;
using System.Data;
using System.Globalization;

using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.MetaDataPlus.Import
{
	/// <summary>
	/// Summary description for ImportExceptions.
	/// </summary>

	public class MdpImportException : Exception
	{
		private DataRow _row;
		private int _rowIndex = -1;
		private RuleItem _colMapping;
		private MetaField _destField;
		private string _errorValue;

		private bool _errorInfo;

		public DataRow Row { get { return _row; } }
		public int RowIndex { get { return _rowIndex; } }
		public RuleItem ColMapping { get { return _colMapping; } }
		public MetaField DestField { get { return _destField; } }
		public string ErrorValue { get { return _errorValue; } }

		public MdpImportException(string message, DataRow row, int rowIndex, RuleItem colMapping, MetaField destField, object errorValue)
			: base(message)
		{
			_row = row;
			_rowIndex = rowIndex;
			_colMapping = colMapping;
			_destField = destField;

			if (errorValue != null)
				_errorValue = errorValue.ToString();
			else _errorValue = "<null>";

			_errorInfo = true;
		}

		public MdpImportException(string message)
			: base(message)
		{
		}

		internal void setRowInfo(DataRow row, int rowIndex)
		{
			_row = row;
			_rowIndex = rowIndex;
		}

		public override string ToString()
		{
			if (_errorInfo)
			{
				string msg = Message + "\r\n";

				msg += "Row number: " + (_rowIndex + 1).ToString() + "\r\n";
				if (_colMapping != null)
				{
					if (_destField == null)
					{
						msg += "Destination field is absent\r\n";
					}
					else
					{
						switch (_colMapping.FillType)
						{
							case FillTypes.CopyValue:
								msg += "Import from " + _colMapping.SrcColumnName + " To " + _destField.FriendlyName + "\r\n";
								msg += "Value: " + _errorValue + "\r\n";
								break;
							case FillTypes.Custom:
								msg += "Import To " + _destField.FriendlyName + "\r\n";
								msg += "Value: " + _errorValue + "\r\n";
								break;
							case FillTypes.Default:
								msg += "Import To " + _destField.FriendlyName + "\r\n";
								msg += "Default value\r\n";
								break;
						}
					}
				}
				else msg += "No mapping information\r\n";

				return msg;
			}
			else return base.ToString();
		}

	}

	public class AlreadyExistException : MdpImportException
	{
		public AlreadyExistException()
			: base("Meta Object already exists")
		{
		}
	}

	public class NotExistException : MdpImportException
	{
		public NotExistException()
			: base("Meta Object doesn't exists")
		{
		}
	}

	public class AbsentValue : MdpImportException
	{
		public AbsentValue(string fieldName)
			: base(String.Format("Absent value for {0}", fieldName))
		{
		}
	}

	public class InvalidValue : MdpImportException
	{
		public InvalidValue(string value)
			: base(String.Format("Invalid value {0}", value))
		{
		}
	}

	public class InvalidCastException : MdpImportException
	{
		public InvalidCastException(string dest, string sour)
			: base(String.Format("Cannot convert {0} to {1}", dest, sour))
		{
		}
	}

	public class NotMatchingRuleException : MdpImportException
	{
		public NotMatchingRuleException()
			: base("Rule is not matching MappingMetaClass")
		{
		}
	}

	public class MissingMappingRule : MdpImportException
	{
		private string[] _columns;

		public string[] Columns { get { return _columns; } }

		public MissingMappingRule(string[] columns)
			: base(String.Format("Missing mapping rules for {0} column", columns == null ? "null" : columns.Length.ToString(CultureInfo.InvariantCulture)))
		{
			_columns = columns;
		}
	}

	public class IdleOperationException : MdpImportException
	{
		public IdleOperationException()
			: base("No information was added")
		{
		}
	}
}
