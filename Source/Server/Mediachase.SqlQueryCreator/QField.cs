using System;
using System.Text;
using System.Data;

namespace Mediachase.SQLQueryCreator
{
	[Flags]
	public enum QFieldUsingType
	{
		Abstract = 0,
		Field = 1,
		Grouping = 2,
		Filter = 4,
		Sort = 8
	}	

	/// <summary>
	/// Summary description for QField.
	/// </summary>
	public class QField
	{
		private QFieldUsingType _UsingType	=	QFieldUsingType.Abstract;

		private string _Name		=	null;
		private string _FriendlyName		=	null;
		private string _DBName		=	null;

		private string _Value		=	null;

		//private QFieldJoinRelation	_Relation = null;
		private DbType				_DataType;

		private bool	_IsKey		=	false;

		private QFieldJoinRelation[]	_Relations = null;

		private	bool					_FieldValueIsKey	=	false;

		public QField(string Value)
		{
			_Value	=	Value;
		}

		public QField(string Name, string FriendlyName,	string Value)
		{
			_Name = Name;
			_FriendlyName = FriendlyName;
			_Value	=	Value;
		}

		public QField(string Name, string FriendlyName,	string DBName,DbType DataType, QFieldUsingType UsingType)
		{
			_Name = Name;
			_DBName = DBName;
			_DataType = DataType;
			_UsingType = UsingType;
			_FriendlyName = FriendlyName;
		}

		public QField(string Name, string FriendlyName,string DBName,DbType DataType, QFieldUsingType UsingType, bool IsKey)
		{
			_Name = Name;
			_DBName = DBName;
			_DataType = DataType;
			_UsingType = UsingType;
			_IsKey = IsKey;
			_FriendlyName = FriendlyName;
		}

		public QField(string Name, string FriendlyName,string DBName, DbType DataType, QFieldUsingType UsingType, QFieldJoinRelation Relation)
		{
			_Name = Name;
			_DBName = DBName;
			_DataType = DataType;
			_UsingType = UsingType;
			_Relations	=	new QFieldJoinRelation[]{Relation};
			_FriendlyName = FriendlyName;
		}

		public QField(string Name, string FriendlyName,string DBName, DbType DataType, QFieldUsingType UsingType, QFieldJoinRelation[] Relations)
		{
			_Name = Name;
			_DBName = DBName;
			_DataType = DataType;
			_UsingType = UsingType;
			_Relations = Relations;
			_FriendlyName = FriendlyName;
		}

		public QField(string Name, string FriendlyName,string DBName, DbType DataType, QFieldUsingType UsingType, QFieldJoinRelation Relation, bool FieldValueIsKey)
		{
			_Name = Name;
			_DBName = DBName;
			_DataType = DataType;
			_UsingType = UsingType;
			_Relations	=	new QFieldJoinRelation[]{Relation};
			_FriendlyName = FriendlyName;

			_FieldValueIsKey = FieldValueIsKey;
		}

		public QField(string Name, string FriendlyName,string DBName, DbType DataType, QFieldUsingType UsingType, QFieldJoinRelation[] Relations, bool FieldValueIsKey)
		{
			_Name = Name;
			_DBName = DBName;
			_DataType = DataType;
			_UsingType = UsingType;
			_Relations = Relations;
			_FriendlyName = FriendlyName;

			_FieldValueIsKey = FieldValueIsKey;
		}

		public DbType	DataType
		{
			get
			{
				return _DataType;
			}
		}

		public string	Value
		{
			get
			{
				return _Value;
			}
		}

		public bool IsKey
		{
			get
			{
				return _IsKey;
			}
		}

//		public QFieldJoinRelation	Relation
//		{
//			get{return null;}
//		}

		public QFieldJoinRelation[]	Relations
		{
			get
			{
				return _Relations;
			}
		}


		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public string FriendlyName
		{
			get
			{
				return _FriendlyName;
			}
		}

		public string DBName
		{
			get
			{
				return _DBName;
			}
		}

		public QFieldUsingType UsingType
		{
			get
			{
				return _UsingType;
			}
		}

		public bool FieldValueIsKey
		{
			get
			{
				return _FieldValueIsKey;
			}
		}

		public static bool operator==(QField field1,QField field2)
		{
			return Object.Equals(field1,field2);
		}

		public static bool operator!=(QField field1,QField field2)
		{
			return !(field1==field2);
		}

		public override bool Equals(object obj)
		{
			if(obj==null)
				return false;

			QField	srcField	=	obj as QField;

			if(srcField==null)
				return false;

			if(srcField.Name==this.Name /*&& 
				srcField.DBName == this.DBName*/ )
			{
				return true;
//				if(srcField.Relations!=null && 
//					this.Relations!=null &&
//					srcField.Relations.Length==this.Relations.Length)
//				{
//					foreach(QFieldJoinRelation srcRelation in srcField.Relations)
//					{
//						foreach(QFieldJoinRelation thisRelation in this.Relations)
//						{
//							if(thisRelation!=srcRelation)
//								return false;
//						}
//					}
//					return true;
//				}
//				else if(srcField.Relations==null && this.Relations==null)
//				{
//					return true;
//				}
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode()^DBName.GetHashCode()^(Relations!=null?Relations.GetHashCode():0);
		}
	}
}
