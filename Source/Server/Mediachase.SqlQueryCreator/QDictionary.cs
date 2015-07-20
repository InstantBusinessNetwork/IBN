using System;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for QDictionary.
	/// </summary>
	public class QDictionary
	{
		private QField	_fieldId	=	null;
		private QField	_fieldValue	=	null;
		private string	_sqlQuery	=	string.Empty;

		public QDictionary()
		{
		}

		public QDictionary(QField	Id,	QField	Value,	string SQLQuery)
		{
			_fieldId = Id;
			_fieldValue = Value;
			_sqlQuery = SQLQuery;
		}

		public QField FieldId
		{
			get
			{
				return _fieldId;
			}
		}

		public QField FieldValue
		{
			get
			{
				return _fieldValue;
			}
		}

		public string SQLQuery
		{
			get
			{
				return _sqlQuery;
			}
		}

		public string GetSQLQuery()
		{
			return _sqlQuery;
		}

		public string GetSQLQuery(params object[] args)
		{
			return string.Format(_sqlQuery,args);
		}
	}
}
