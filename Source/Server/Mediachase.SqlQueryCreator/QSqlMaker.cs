using System;
using System.Text;
using System.Collections;

namespace Mediachase.SQLQueryCreator
{
	public class QSqlSelectFieldMaker
	{
		public QField	Field		=	null;
		public bool		IsNULL		=	false;
		public string	Alias		=	null;

		public bool		IsHidden		=	false;

		public QSqlSelectFieldMaker(QField	Field, bool	IsNULL, string	Alias)
		{
			this.Field = Field;
			this.IsNULL = IsNULL;
			this.Alias = Alias;
		}

		public QSqlSelectFieldMaker(QField	Field, bool	IsNULL, string	Alias, bool IsHidden)
		{
			this.Field = Field;
			this.IsNULL = IsNULL;
			this.Alias = Alias;
			this.IsHidden = IsHidden;
		}

	}
	/// <summary>
	/// Summary description for QSqlSelectMaker.
	/// </summary>
	public class QSqlSelectMaker
	{
		private string	_Tag;
		private string	_Parent;
		private string	_OwnerTable;
		private string	_TimeOffset;
		private string	_LanguageId;

		private	bool	_AddDistinct	=	false;

		private	ArrayList	_Fields	=	new ArrayList();
		private	ArrayList	_WhereConditions	=	new ArrayList();

		private	Hashtable	_TableAlias = new Hashtable();

		public QSqlSelectMaker(string Tag, string Parent, string OwnerTable, string TimeOffset, string LanguageId)
		{
			_Tag = Tag;
			_Parent = Parent;
			_OwnerTable = OwnerTable;

			_TimeOffset = TimeOffset;
			_LanguageId = LanguageId;
		}

		public QSqlSelectMaker(string Tag, string Parent, string OwnerTable, string TimeOffset, string LanguageId, bool AddDistinct)
		{
			_Tag = Tag;
			_Parent = Parent;
			_OwnerTable = OwnerTable;

			_TimeOffset = TimeOffset;
			_LanguageId = LanguageId;

			_AddDistinct	=	AddDistinct;
		}

		public ArrayList Fields
		{
			get
			{
				return _Fields;
			}
		}

		public void AddField(QSqlSelectFieldMaker FieldMaker)
		{
			if(!FieldMaker.IsNULL && FieldMaker.Field.Relations!=null)
			{
				foreach(QFieldJoinRelation relation in  FieldMaker.Field.Relations)
				{
					if(relation.LanguageField!=null)
						_WhereConditions.Add( new SimpleFilterCondition(new QField(relation.LanguageField,relation.LanguageField,relation.LanguageField,System.Data.DbType.Int32,QFieldUsingType.Abstract,FieldMaker.Field.Relations),_LanguageId,SimpleFilterType.Equal));
				}
			}

			_Fields.Add(FieldMaker);
		}

		public void AddFilter(FilterCondition Filter)
		{
			_WhereConditions.Add(Filter);
		}

		protected string AddAlias(string Object, QFieldJoinRelation	relation)
		{
			if(relation==null||relation.DestTable==this._OwnerTable)
				return AddAlias(Object);

			Object = string.Format("{0}{1}",Object,(uint)relation.GetHashCode());

			if(_TableAlias.ContainsKey(Object))
				return (string)_TableAlias[Object];

			_TableAlias.Add(Object,string.Format("AL{0}",(uint)relation.GetHashCode()));
			return (string)_TableAlias[Object];
		}

		protected string AddAlias(string Object)
		{
			if(_TableAlias.ContainsKey(Object))
				return (string)_TableAlias[Object];

			_TableAlias.Add(Object,string.Format("AL{0}",_TableAlias.Count+1));
			return (string)_TableAlias[Object];
		}

		public string Create(string OwnerTable, GroupCollection	GroupByList)
		{
			StringBuilder	strBuilder	=	new StringBuilder();

			// 1. Create Select [4/20/2004]
			if(_AddDistinct)
				strBuilder.AppendFormat("SELECT DISTINCT {0} AS Tag, {1} AS Parent", _Tag,_Parent);
			else
				strBuilder.AppendFormat("SELECT {0} AS Tag, {1} AS Parent", _Tag,_Parent);

			ArrayList	_AdditionalWhereList	=	new ArrayList();

			ArrayList	fromTables	=	new ArrayList();

			if(_OwnerTable!=null)
			{
				fromTables.Add(_OwnerTable);
			}

			// 1. Create Field List [4/20/2004]
			foreach(QSqlSelectFieldMaker FieldMaker in _Fields)
			{
				strBuilder.Append(" ,");

				// Add Filed Name [4/20/2004]
				if(FieldMaker.IsNULL)
					strBuilder.Append(" NULL");
				else if(FieldMaker.Field.Value!=null)
				{
					strBuilder.AppendFormat(" N'{0}'",FieldMaker.Field.Value);
				}
				else
				{
					string TableAlias = null;

					if(FieldMaker.Field.Relations==null)
					{
//						if(_OwnerTable!=null)
//							TableAlias = AddAlias(_OwnerTable);
						TableAlias = AddAlias(OwnerTable);
						if(!fromTables.Contains(OwnerTable))
							fromTables.Add(OwnerTable);
					}
					else
					{
						if(!FieldMaker.IsNULL && FieldMaker.Field.Relations[0].SourceTable!=_OwnerTable && !fromTables.Contains(FieldMaker.Field.Relations[FieldMaker.Field.Relations.Length-1].DestTable)
							&& FieldMaker.Field.Relations[0].SourceTable!=_OwnerTable && !fromTables.Contains(FieldMaker.Field.Relations[FieldMaker.Field.Relations.Length-1]))
						{
							fromTables.Add(FieldMaker.Field.Relations[FieldMaker.Field.Relations.Length-1]);
						}

						QFieldJoinRelation prevRelation	=	null;

						foreach(QFieldJoinRelation relation in FieldMaker.Field.Relations)
						{
							if(relation.Filter!=null)
							{
								if(!_AdditionalWhereList.Contains(FieldMaker.Field))
									_AdditionalWhereList.Add(FieldMaker.Field);
							}

							AddAlias(relation.SourceTable, prevRelation);
							TableAlias = AddAlias(relation.DestTable,relation);

							prevRelation = relation;
						}
					}

					string TmpFieldFormat;

					if(FieldMaker.Field.DataType==System.Data.DbType.DateTime)
						TmpFieldFormat = string.Format("[dbo].GetLocalDate({2}, {0}.[{1}])",TableAlias,FieldMaker.Field.DBName,_TimeOffset);
					else
					{
						if(FieldMaker.Field.DBName.IndexOf("{0}")!=-1)
						{
							//strBuilder.Append(" ");
							TmpFieldFormat = string.Format(FieldMaker.Field.DBName,TableAlias, this._LanguageId, this._TimeOffset);
						}
						else
							TmpFieldFormat = string.Format("{0}.[{1}]",TableAlias,FieldMaker.Field.DBName);
					}

					//strBuilder.AppendFormat(" (CASE WHEN ({0}) IS NULL THEN '' ELSE ({0}) END)",TmpFieldFormat);

					// 2008-03-13 Fix Int Ordering Problem
					//if (FieldMaker.Field.DataType == System.Data.DbType.Int32)
					//    strBuilder.AppendFormat(TmpFieldFormat);
					//else
					strBuilder.AppendFormat(" CONVERT(NVARCHAR(3000),{0},20)", TmpFieldFormat);

				}

				// Add Filed Alias [4/20/2004]
				if(FieldMaker.Alias!=null)
				{
					strBuilder.Append(" AS");
					strBuilder.AppendFormat(" {0}", FieldMaker.Alias);
				}
			}

			if(fromTables.Count>0)
			{
				// 3. Create From  [4/20/2004]
				strBuilder.Append(" FROM");
				bool bFirstRun = true;
				foreach(object FromTableName in fromTables)
				{
					if(bFirstRun)
						bFirstRun = false;
					else
						strBuilder.Append(" ,");

					if(FromTableName is string)
					{
						string FromTableAlias = AddAlias((string)FromTableName);

						strBuilder.AppendFormat(" {0} AS {1}",FromTableName, FromTableAlias);
					}
					else if(FromTableName is QFieldJoinRelation)
					{
						string FromTableAlias = AddAlias(((QFieldJoinRelation)FromTableName).DestTable, (QFieldJoinRelation)FromTableName);

						strBuilder.AppendFormat(" {0} AS {1}",((QFieldJoinRelation)FromTableName).DestTable, FromTableAlias);
					}
				
				}

				// 4. Add Inner Join Section [4/20/2004]
				ArrayList	listSetJoin	=	new ArrayList();

				if(_OwnerTable!=null)
				{
					foreach(QSqlSelectFieldMaker FieldMaker in _Fields)
					{
						if(FieldMaker.Field!=null&&FieldMaker.Field.Relations!=null && !FieldMaker.IsNULL 
							&& FieldMaker.Field.Relations[FieldMaker.Field.Relations.Length-1].DestTable!=_OwnerTable &&
							FieldMaker.Field.Relations[0].SourceTable==_OwnerTable)
						{
							QFieldJoinRelation prevRelation	=	null;

							foreach(QFieldJoinRelation relation in FieldMaker.Field.Relations)
							{
								if(!listSetJoin.Contains(relation))
								{
									listSetJoin.Add(relation);

									bool bFound =	false;
									foreach(QField gropFiled in GroupByList)
									{
										if(gropFiled==FieldMaker.Field)
										{
											bFound = true;
										}
									}

									if(bFound)
										strBuilder.Append(" JOIN");
									else
										strBuilder.Append(" LEFT JOIN");

									string DestTableAlias = null;
									string SourceTableAlias = null; 

									SourceTableAlias = AddAlias(relation.SourceTable, prevRelation);
									DestTableAlias = AddAlias(relation.DestTable, relation);
									
									strBuilder.AppendFormat(" {0} AS {1} ON {2}.[{3}] = {4}.[{5}]",
										relation.DestTable,DestTableAlias,
										SourceTableAlias,relation.SourceKey, 
										DestTableAlias, relation.DestKey);

									if (FieldMaker.Field != null)
									{
										if (!_AdditionalWhereList.Contains(FieldMaker.Field))
											_AdditionalWhereList.Add(FieldMaker.Field);
									}
								}		
								prevRelation = relation;
							}
						}
					}

					foreach(FilterCondition filter in _WhereConditions)
					{
						if(!_Fields.Contains(filter.OwnerField))
						{
							if(filter.OwnerField!=null&&
								filter.OwnerField.Relations!=null && 
								filter.OwnerField.Relations[filter.OwnerField.Relations.Length-1].DestTable!=_OwnerTable &&
								filter.OwnerField.Relations[0].SourceTable==_OwnerTable)
							{
								QFieldJoinRelation prevRelation	=	null;

								foreach(QFieldJoinRelation relation in filter.OwnerField.Relations)
								{
									if(!listSetJoin.Contains(relation))
									{
										listSetJoin.Add(relation);

										bool bFound =	false;
										foreach(QField gropFiled in GroupByList)
										{
											if(gropFiled==filter.OwnerField)
											{
												bFound = true;
											}
										}

										if(bFound)
											strBuilder.Append(" JOIN");
										else
											strBuilder.Append(" LEFT JOIN");

										string DestTableAlias = null;
										string SourceTableAlias = null; 

										SourceTableAlias = AddAlias(relation.SourceTable, prevRelation);
										DestTableAlias = AddAlias(relation.DestTable, relation);
									
										strBuilder.AppendFormat(" {0} AS {1} ON {2}.[{3}] = {4}.[{5}]",
											relation.DestTable,DestTableAlias,
											SourceTableAlias,relation.SourceKey, 
											DestTableAlias, relation.DestKey);

										if (relation.Filter != null)
										{
											if (!_AdditionalWhereList.Contains(filter.OwnerField))
												_AdditionalWhereList.Add(filter.OwnerField);
										}
									}			
									prevRelation = relation;
								}
							}
						}
					}
				}

				// 4. Add Where section [4/20/2004]
				if(_WhereConditions.Count>0 || _AdditionalWhereList.Count>0)
				{
					foreach(FilterCondition filter in _WhereConditions)
					{
						if(filter.OwnerField!=null&&filter.OwnerField.Relations!=null)
						{
							QFieldJoinRelation prevRelation	=	null;

							foreach(QFieldJoinRelation relation in filter.OwnerField.Relations)
							{
								if(relation.LanguageField!=null)
									AddAlias(relation.DestTable);

								AddAlias(relation.DestTable, relation);
								AddAlias(relation.SourceTable, prevRelation);

								prevRelation = relation;
							}
						}
					}

					foreach(QField AddFieldWhere in _AdditionalWhereList)
					{
						QFieldJoinRelation prevRelation	=	null;
						foreach(QFieldJoinRelation	AddRelationWhere in AddFieldWhere.Relations)
						{
							if(AddRelationWhere.Filter!=null)
							{
								AddAlias(AddRelationWhere.DestTable, AddRelationWhere);
								AddAlias(AddRelationWhere.SourceTable, prevRelation);
							}
							prevRelation = AddRelationWhere;
						}
					}

					bool bFirstTime = true;

					foreach(FilterCondition filter in _WhereConditions)
					{
						if(bFirstTime)
						{
							bFirstTime = false;
							strBuilder.Append(" WHERE");
						}
						else
						{
							strBuilder.Append(" AND");
						}

						strBuilder.AppendFormat(" ({0})",filter.Create(_OwnerTable,_TableAlias,_LanguageId, _TimeOffset));
					}

					foreach(QField AddFieldWhere in _AdditionalWhereList)
					{
						foreach(QFieldJoinRelation	relation in AddFieldWhere.Relations)
						{
							if(relation.Filter!=null && (fromTables.Contains(relation.DestTable) || listSetJoin.Contains(relation)))
							{
								if(bFirstTime)
								{
									bFirstTime = false;
									strBuilder.Append(" WHERE");
								}
								else
								{
									strBuilder.Append(" AND");
								}

								string tmpOwnerTableObject;
								if(relation.DestTable==this._OwnerTable)
									tmpOwnerTableObject = relation.DestTable;
								else
									tmpOwnerTableObject = string.Format("{0}{1}",relation.DestTable,(uint)relation.GetHashCode());

								strBuilder.AppendFormat(" ({0})",relation.Filter.Create(tmpOwnerTableObject,_TableAlias,_LanguageId, _TimeOffset));
							}
						}
					}
				}
			}

			return strBuilder.ToString();
		}

	}
}
