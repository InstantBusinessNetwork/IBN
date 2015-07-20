using System;
using System.Collections;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for FilterCondition.
	/// </summary>
	public abstract class FilterCondition
	{
		private	QField	_Field	=	null;

		public FilterCondition(QField	Field)
		{
			_Field = Field;
		}

		public QField OwnerField
		{
			get
			{
				return _Field;
			}
		}

		public virtual string Create(string DefaultTable, System.Collections.Hashtable tableAlias, string LanguageId, string TimeOffset)
		{
			return "";
		}

		protected virtual string GetSqlValue(QField field, string value)
		{
			if (field != null && field.DataType == System.Data.DbType.Guid)
				return "'" + value + "'";

			return value;
		}
	}

	public enum SimpleFilterType
	{
		Equal = 1,
		NotEqual,
		Less,
		LessOrEqual,
		Great,
		GreatOrEqual,
		IsNull,
		IsNotNull,
		In,
		Like
	}

	

	public class SimpleFilterCondition: FilterCondition
	{
		private string _Value				=	null;
		private SimpleFilterType _Type				=	SimpleFilterType.Equal;

		public SimpleFilterCondition(QField	Field, string Value, SimpleFilterType Type):base(Field)
		{
			if(Value!=null)
				_Value = "{0} {1} " + base.GetSqlValue(Field, Value);
			_Type = Type;
		}

		public SimpleFilterCondition(QField	Field, string[] Values, SimpleFilterType Type):base(Field)
		{
			bool bFirst = true;

			_Value = "";

			if(Values!=null)
			{
				foreach(string Value in Values)
				{
					if(bFirst)
						bFirst = false;
					else
						_Value += " OR ";
					if(Value=="NULL"||Value=="NOT NULL")
						_Value += "{0} IS " + base.GetSqlValue(Field, Value);
					else
						_Value += "{0} {1} " + base.GetSqlValue(Field, Value);
				}
			}
			
			_Type = Type;
		}

		internal string InternalValue
		{
			get
			{
				return _Value;
			}
			set 
			{
				_Value  = value;
			}
		}

		public string Value
		{
			set
			{
				_Value += "{0} {1} " + value;
			}
		}

		public SimpleFilterType FilterType
		{
			get
			{
				return _Type;
			}
			set
			{
				_Type = value;
			}
		}

		public override string Create(string DefaultTable, System.Collections.Hashtable tableAlias,string LanguageId, string TimeOffset)
		{
			string TableAlias = null;

			if(this.OwnerField.Relations!=null)
			{
				string TableName = string.Format("{0}{1}",this.OwnerField.Relations[this.OwnerField.Relations.Length-1].DestTable,(uint)this.OwnerField.Relations[this.OwnerField.Relations.Length-1].GetHashCode());
				TableAlias = (string)tableAlias[TableName];

				if(TableAlias==null)
					TableAlias = (string)tableAlias[DefaultTable];
			}
			else
				TableAlias = (string)tableAlias[DefaultTable];

			string strFieldDbName = this.OwnerField.DBName;
			if(strFieldDbName==null)
				strFieldDbName = this.OwnerField.Value;

			string TmpFieldFormat	=	null;

			if(strFieldDbName.IndexOf("{0}")!=-1)
			{
				//strBuilder.Append(" ");
				TmpFieldFormat = string.Format(strFieldDbName,TableAlias, LanguageId, TimeOffset);
			}
			else
				TmpFieldFormat = string.Format("{0}.[{1}]",TableAlias,strFieldDbName);

			switch(this.FilterType) 
			{
			case SimpleFilterType.Equal:
				return string.Format(_Value, TmpFieldFormat,"=");
			case SimpleFilterType.Great:
				return string.Format(_Value, TmpFieldFormat,">");
			case SimpleFilterType.GreatOrEqual:
				return string.Format(_Value, TmpFieldFormat,">=");
			case SimpleFilterType.Less:
				return string.Format(_Value, TmpFieldFormat,"<");
			case SimpleFilterType.LessOrEqual:
				return string.Format(_Value, TmpFieldFormat,"<=");
			case SimpleFilterType.NotEqual:
				return string.Format(_Value, TmpFieldFormat,"<>");
			case SimpleFilterType.IsNull:
				return string.Format("{0} IS NULL",TmpFieldFormat);
			case SimpleFilterType.IsNotNull:
				return string.Format("{0} IS NOT NULL",TmpFieldFormat);
			case SimpleFilterType.In:
				return string.Format(_Value, TmpFieldFormat,"IN");
			case SimpleFilterType.Like:
				return string.Format(_Value, TmpFieldFormat,"LIKE");
			}
			return string.Format("");
		}

	}

	public class IntervalFilterCondition: FilterCondition
	{
		private string	_ValueLess	=	null;
		private string	_ValueGreat	=	null;

		public IntervalFilterCondition(QField	Field, string ValueLess, string ValueGreat):base(Field)
		{
			_ValueLess = ValueLess;
			_ValueGreat = ValueGreat;
		}

		public string ValueLess
		{
			get
			{
				return _ValueLess;
			}
			set
			{
				_ValueLess  = value;
			}
		}

		public string ValueGreat
		{
			get
			{
				return _ValueGreat;
			}
			set
			{
				_ValueGreat  = value;
			}
		}

		public override string Create(string DefaultTable, System.Collections.Hashtable tableAlias,string LanguageId, string TimeOffset)
		{
			string TableAlias = null;

			if(this.OwnerField.Relations!=null)
			{
				string TableName = string.Format("{0}{1}",this.OwnerField.Relations[this.OwnerField.Relations.Length-1].DestTable,(uint)this.OwnerField.Relations[this.OwnerField.Relations.Length-1].GetHashCode());
				TableAlias = (string)tableAlias[TableName];
			}
			else
				TableAlias = (string)tableAlias[DefaultTable];

			// OZ [2008-04-28] Fix Error
			string strFieldDbName = this.OwnerField.DBName;
			if (strFieldDbName == null)
				strFieldDbName = this.OwnerField.Value;

			string TmpFieldFormat = null;

			if (strFieldDbName.IndexOf("{0}") != -1)
			{
				//strBuilder.Append(" ");
				TmpFieldFormat = string.Format(strFieldDbName, TableAlias, LanguageId, TimeOffset);
			}
			else
				TmpFieldFormat = string.Format("{0}.[{1}]", TableAlias, strFieldDbName);


			return string.Format("{0} >= {1} AND {0} <= {2}", TmpFieldFormat, this.ValueLess, this.ValueGreat);
		}

	}

	public class ExtendedFilterCondition: FilterCondition
	{
		private string _Query;
		private string _Value;

		public ExtendedFilterCondition(string Query,string Value):base(null)
		{
			_Query = Query;
			_Value = Value;
		}

		public override string Create(string DefaultTable, System.Collections.Hashtable tableAlias, string LanguageId, string TimeOffset)
		{
			return string.Format(this.Query,tableAlias[DefaultTable],LanguageId, TimeOffset, this.Value);
		}

		public string Query
		{
			get
			{
				return _Query;
			}
			set
			{
				_Query  = value;
			}
		}

		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value  = value;
			}
		}

	}

}
