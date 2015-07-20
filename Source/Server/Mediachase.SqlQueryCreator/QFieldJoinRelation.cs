using System;

namespace Mediachase.SQLQueryCreator
{
	
	/// <summary>
	/// Summary description for QFieldJoinRelation.
	/// </summary>
	public class QFieldJoinRelation
	{
		private string _SrcTable	=	null;
		private string _DestTable	=	null;

		private string _SrcKey		=	null;
		private string _DestKey		=	null;

		private string	_LanguageField	=	null; 
		private FilterCondition _Filter	=	null;

		public QFieldJoinRelation(string  SrcTable, string  DestTable, string SrcKey, string  DestKey)
		{
			_SrcTable	=	SrcTable;
			_DestTable	=	DestTable;

			_SrcKey		=	SrcKey;
			_DestKey		=	DestKey;
		}

		public QFieldJoinRelation(string  SrcTable, string  DestTable, string SrcKey, string  DestKey, FilterCondition filter)
		{
			_SrcTable	=	SrcTable;
			_DestTable	=	DestTable;

			_SrcKey		=	SrcKey;
			_DestKey		=	DestKey;

			_Filter	=	filter;
		}

		public QFieldJoinRelation(string  SrcTable, string  DestTable, string SrcKey, string  DestKey, string LanguageField)
		{
			_SrcTable	=	SrcTable;
			_DestTable	=	DestTable;

			_SrcKey		=	SrcKey;
			_DestKey		=	DestKey;

			_LanguageField	=	LanguageField;
		}

		public string SourceTable
		{
			get
			{
				return _SrcTable;
			}
		}

		public string  DestTable
		{
			get
			{
				return _DestTable;
			}
		}


		public string SourceKey
		{
			get
			{
				return _SrcKey;
			}
		}
		
		public string  DestKey
		{
			get
			{
				return _DestKey;
			}
		}

		public string LanguageField
		{
			get
			{
				return _LanguageField;
			}
		}

		public FilterCondition Filter
		{
			get
			{
				return _Filter;
			}
		}

		public static bool operator==(QFieldJoinRelation item1,QFieldJoinRelation item2)
		{
			return Object.Equals(item1,item2);
		}

		public static bool operator!=(QFieldJoinRelation item1,QFieldJoinRelation item2)
		{
			return !(item1==item2);
		}

		public override bool Equals(object obj)
		{
			if(obj==null)
				return false;

			QFieldJoinRelation	srcItem	=	obj as QFieldJoinRelation;

			if(srcItem==null)
				return false;

			if(String.Compare(srcItem.SourceTable,SourceTable,true)==0&&
				String.Compare(srcItem.DestTable,DestTable,true)==0&&
				String.Compare(srcItem.DestKey,DestKey,true)==0 &&
				String.Compare(srcItem.SourceKey,SourceKey,true)==0)
			{
				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _SrcTable.GetHashCode()^_DestTable.GetHashCode()^_SrcKey.GetHashCode()^_DestKey.GetHashCode();
		}
	}

}
