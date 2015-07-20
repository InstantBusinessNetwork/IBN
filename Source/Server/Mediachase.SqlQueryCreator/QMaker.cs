using System;
using System.Collections;
using System.Text;

namespace Mediachase.SQLQueryCreator
{
	/// <summary>
	/// Summary description for QMaker.
	/// </summary>
	public class QMaker
	{
		private string		_ReportName	=	null;
		private string		_KeyUniquePrefix = null;

		private QObject				_OwnerObject	=	null;
		private FieldCollection		_Fields			=	new FieldCollection();
		private	FilterCollection	_Filters		=	new FilterCollection();
		private	GroupCollection		_Groups			=	new GroupCollection();

		private	string				_Language		=	"1"	;
		private	string				_TimeOffset		=	"0"	;

		private	ArrayList	QSqlSelectList = new ArrayList();
		private	ArrayList	OrderByFields	=	new ArrayList();
		private	long		ProcessedGroupItems	=	0;
		private	int			CurrentStepTagLevel	=	0;

		private bool		_AddExObjects	=	true;

		private	ArrayList	_PrevSqlQuery = new ArrayList();

		public QMaker(QObject OwnerObject)
		{
			_OwnerObject = OwnerObject;

			_ReportName = OwnerObject.OwnerTable;
		}

		public QMaker(QObject OwnerObject, bool AddExObjects)
		{
			_OwnerObject = OwnerObject;

			_ReportName = OwnerObject.OwnerTable;

			_AddExObjects = AddExObjects;
		}

		internal QMaker(QObject OwnerObject, string ReportName, string KeyUniquePrefix)
		{
			_OwnerObject = OwnerObject;

			_ReportName = ReportName;
			_KeyUniquePrefix = KeyUniquePrefix;
		}

		public QObject OwnerObject
		{
			get
			{
				return _OwnerObject;
			}
		}

		public FieldCollection	Fields
		{
			get
			{
				return _Fields;
			}
		}

		public ArrayList	PrevSqlQuery
		{
			get
			{
				return _PrevSqlQuery;
			}
		}

		public FilterCollection	Filters
		{
			get
			{
				return _Filters;
			}
		}

		public GroupCollection	Groups
		{
			get
			{
				return _Groups;
			}
		}

		public string	Language
		{
			get
			{
				return _Language;
			}
			set
			{
				_Language = value;
			}
		}

		public string	TimeOffset
		{
			get
			{
				return _TimeOffset;
			}
			set
			{	
				_TimeOffset	=	value;
			}	
		}

		protected void AddItemKeyToSelect(QSqlSelectMaker	select)
		{
			bool bShowAlias	=	false;

			// Add Root Static Field [4/21/2004]
			select.AddField(new QSqlSelectFieldMaker(new QField(_ReportName),false,bShowAlias?"Report!1!Name":null));

			// Add Group Fields [4/20/2004]
			foreach(QField tmpGroupField in this.Groups)
			{
				// Step 1. Add Groups Item [4/21/2004]

				select.AddField(new QSqlSelectFieldMaker(new QField(tmpGroupField.FriendlyName),false,null));

				select.AddField(new QSqlSelectFieldMaker(tmpGroupField,false,null));

				select.AddField(new QSqlSelectFieldMaker(tmpGroupField.Relations==null?tmpGroupField:(new QField(tmpGroupField.Name,tmpGroupField.FriendlyName,tmpGroupField.FieldValueIsKey?tmpGroupField.DBName:tmpGroupField.Relations[tmpGroupField.Relations.Length-1].DestKey,tmpGroupField.DataType,tmpGroupField.UsingType,tmpGroupField.Relations)),false,null));
			}

			// Add Key Field [4/21/2004]
			if(this.OwnerObject.KeyField!=null)
			{
				QField	keyField = this.OwnerObject.KeyField;
				if(_KeyUniquePrefix!=null)
				{
					keyField = new QField(this.OwnerObject.KeyField.Name, this.OwnerObject.KeyField.FriendlyName, 
						"'" + _KeyUniquePrefix+ "' + CONVERT(NVARCHAR(3000),{0}.["+this.OwnerObject.KeyField.DBName+"],20)",
						this.OwnerObject.KeyField.DataType,this.OwnerObject.KeyField.UsingType,true);
				}
				select.AddField(new QSqlSelectFieldMaker(keyField,false,null));
			}

			// [FieldName!Tag!Id] [4/22/2004]
			select.AddField(new QSqlSelectFieldMaker(null,true,null));
			// [Field!Tag!Name] [4/22/2004]
			select.AddField(new QSqlSelectFieldMaker(null,true,null));
			// [Field!Tag!Description] [4/22/2004]
			select.AddField(new QSqlSelectFieldMaker(null,true,null));
			// [FieldName!Tag!Type] [4/22/2004]
			select.AddField(new QSqlSelectFieldMaker(null,true,null));
			// [FieldName!Tag!DataType] [4/22/2004]
			select.AddField(new QSqlSelectFieldMaker(null,true,null));

			// New Addon [4/22/2004]
			// [Values!Tag!Value] [4/22/2004]
			select.AddField(new QSqlSelectFieldMaker(null,true,null));
			// End New Addon [4/22/2004]
		}

		protected void AddLastFieldToSelect(QSqlSelectMaker	select, QField availableField, QField exField,bool bFinalStep)
		{
			bool bShowAlias	=	false;

			// Add Root Static Field [4/21/2004]
			select.AddField(new QSqlSelectFieldMaker(new QField(_ReportName),false,bShowAlias?"Report!1!Name":null));

			// Add Group Fields [4/20/2004]
			foreach(QField tmpGroupField in this.Groups)
			{
				// Step 1. Add Groups Item [4/21/2004]
				select.AddField(new QSqlSelectFieldMaker(new QField(tmpGroupField.FriendlyName),false,null));

				select.AddField(new QSqlSelectFieldMaker(tmpGroupField,false,null));

				select.AddField(new QSqlSelectFieldMaker(tmpGroupField.Relations==null?tmpGroupField:(new QField(tmpGroupField.FriendlyName,tmpGroupField.FriendlyName,tmpGroupField.FieldValueIsKey?tmpGroupField.DBName:tmpGroupField.Relations[tmpGroupField.Relations.Length-1].DestKey,tmpGroupField.DataType,tmpGroupField.UsingType,tmpGroupField.Relations)),false,null));
			}

			// Add Key Field [4/21/2004]
			if(this.OwnerObject.KeyField!=null)
			{
				QField	keyField = this.OwnerObject.KeyField;
				if(_KeyUniquePrefix!=null)
				{
					keyField = new QField(this.OwnerObject.KeyField.Name, this.OwnerObject.KeyField.FriendlyName, 
						"'" + _KeyUniquePrefix+ "' + CONVERT(NVARCHAR(3000),{0}.["+this.OwnerObject.KeyField.DBName+"],20)",
						this.OwnerObject.KeyField.DataType,this.OwnerObject.KeyField.UsingType,true);
				}
				select.AddField(new QSqlSelectFieldMaker(keyField,false,null));
			}

			if(bFinalStep)
			{
				// [FieldName!Tag!Id] [4/22/2004]
                select.AddField(new QSqlSelectFieldMaker(new QField(this.Fields.IndexOf(availableField).ToString("000")), false, null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.Name),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.FriendlyName),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.Relations==null?"1":(availableField.Relations.Length==1?"2":"3") ),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.DataType.ToString()),false,null));
				if(exField!=null)
					select.AddField(new QSqlSelectFieldMaker(exField,false,null));
				else
					select.AddField(new QSqlSelectFieldMaker(availableField,false,null));
			}
			else
			{
				// Add Fields [4/20/2004]
				//foreach(QField availableFields in this.Fields)
				//{
				select.AddField(new QSqlSelectFieldMaker(new QField(this.Fields.IndexOf(availableField).ToString("000")),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.Name),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.FriendlyName),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.Relations==null?"1":(availableField.Relations.Length==1?"2":"3") ),false,null));
				select.AddField(new QSqlSelectFieldMaker(new QField(availableField.DataType.ToString()),false,null));
				select.AddField(new QSqlSelectFieldMaker(null,true,null));
				//}
			}
		}

		protected void AddFieldToSelect(QSqlSelectMaker	select)
		{
			bool bShowAlias	=	QSqlSelectList.Count==0;

			// Add Root Static Field [4/21/2004]
			select.AddField(new QSqlSelectFieldMaker(new QField(_ReportName),false,bShowAlias?"[Report!1!Name]":null));

			// Add Group Fields [4/20/2004]
			int	tmpGroupTagNumber	=	2;

			foreach(QField tmpGroupField in this.Groups)
			{
				// Step 1. Add Groups Item [4/21/2004]
				string FieldAlias	=	null;

				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Groups",tmpGroupTagNumber.ToString(),"Name");

					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}

				bool FieldIsNull	=	false;

				if((tmpGroupTagNumber-2)>=ProcessedGroupItems)
					FieldIsNull = true;

				select.AddField(new QSqlSelectFieldMaker(new QField(tmpGroupField.FriendlyName),FieldIsNull,FieldAlias));

				tmpGroupTagNumber++;

				// Step 2. Add Group Item [4/21/2004]
				//////////////////////////////////////////////////////////////////////////
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Group",tmpGroupTagNumber.ToString(),"Name");

					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}

				if((tmpGroupTagNumber-2)>=ProcessedGroupItems)
					FieldIsNull = true;

				select.AddField(new QSqlSelectFieldMaker(tmpGroupField,FieldIsNull,FieldAlias));
				//////////////////////////////////////////////////////////////////////////
				
				
				//////////////////////////////////////////////////////////////////////////
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Group",tmpGroupTagNumber.ToString(),"Id");

					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}

				if((tmpGroupTagNumber-2)>=ProcessedGroupItems)
					FieldIsNull = true;

				select.AddField(new QSqlSelectFieldMaker(tmpGroupField.Relations==null?tmpGroupField:(new QField(tmpGroupField.Name,tmpGroupField.FriendlyName,tmpGroupField.FieldValueIsKey?tmpGroupField.DBName:tmpGroupField.Relations[tmpGroupField.Relations.Length-1].DestKey,tmpGroupField.DataType,tmpGroupField.UsingType,tmpGroupField.Relations)),FieldIsNull,FieldAlias));
				//////////////////////////////////////////////////////////////////////////


				tmpGroupTagNumber++;
			}

			// Add Key Field [4/21/2004]
			if(this.OwnerObject.KeyField!=null)
			{
				string KeyFieldAlias	=	null;

				if(bShowAlias)
				{
					KeyFieldAlias	=	string.Format("[{0}!{1}!Key]","Item",tmpGroupTagNumber);

					if(!OrderByFields.Contains(KeyFieldAlias))
						OrderByFields.Add(KeyFieldAlias);
				}

				select.AddField(new QSqlSelectFieldMaker(this.OwnerObject.KeyField,true,KeyFieldAlias));
			}

			tmpGroupTagNumber++;

			// Add Fields [4/20/2004]
			//foreach(QField availableFields in this.Fields)
			{
				string FieldAlias	=	null;

				// [FieldName!Tag!Id] [4/22/2004]
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Field",tmpGroupTagNumber,"Id");
					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}
				select.AddField(new QSqlSelectFieldMaker(null,true,FieldAlias));

				// [Field!Tag!Name] [4/22/2004]
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Field",tmpGroupTagNumber,"Name");
					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}
				select.AddField(new QSqlSelectFieldMaker(null,true,FieldAlias));

				// [Field!Tag!Description] [4/22/2004]
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Field",tmpGroupTagNumber,"Description");
					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}
				select.AddField(new QSqlSelectFieldMaker(null,true,FieldAlias));

				// [FieldName!Tag!Type] [4/22/2004]
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Field",tmpGroupTagNumber,"Type");
					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}
				select.AddField(new QSqlSelectFieldMaker(null,true,FieldAlias));

				// [FieldName!Tag!DataType] [4/22/2004]
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}]","Field",tmpGroupTagNumber,"DataType");
					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}
				select.AddField(new QSqlSelectFieldMaker(null,true,FieldAlias));

				// New Addon [4/22/2004]
				tmpGroupTagNumber++;

				// [Values!Tag!Value] [4/22/2004]
				if(bShowAlias)
				{
					FieldAlias	=	string.Format("[{0}!{1}!{2}!element]","Values",tmpGroupTagNumber,"Value");
					if(!OrderByFields.Contains(FieldAlias))
						OrderByFields.Add(FieldAlias);
				}
				select.AddField(new QSqlSelectFieldMaker(null,true,FieldAlias));
				// End New Addon [4/22/2004]
			}
		}

		protected QSqlSelectMaker	CreateRootSelect()
		{
			//Create Repoort Root
			QSqlSelectMaker	retVal	=	null;

			// Create Root [4/21/2004]
			string strTag = CurrentStepTagLevel.ToString();
			string strParent = CurrentStepTagLevel==1?"NULL":(CurrentStepTagLevel-1).ToString();

			retVal	=	new QSqlSelectMaker(strTag, strParent, null,this.TimeOffset,this.Language);

			AddFieldToSelect(retVal);

			return retVal;
		}

		protected QSqlSelectMaker	CreateGroupsSelect(QField	group)
		{
			QSqlSelectMaker	retVal	=	null;

			// Create Root [4/21/2004]
			string GroupOwnerTable = group.Relations==null?this.OwnerObject.OwnerTable:group.Relations[group.Relations.Length-1].DestTable;

			string strTag = CurrentStepTagLevel.ToString();
			string strParent = CurrentStepTagLevel==1?"NULL":(CurrentStepTagLevel-1).ToString();

			retVal	=	new QSqlSelectMaker(strTag, strParent, GroupOwnerTable,this.TimeOffset,this.Language);

			AddFieldToSelect(retVal);

			return retVal;
		}

		protected QSqlSelectMaker	CreateGroupSelect(QField	group)
		{
			QSqlSelectMaker	retVal	=	null;

			// Create Root [4/21/2004]
			string GroupOwnerTable = group.Relations==null?this.OwnerObject.OwnerTable:group.Relations[group.Relations.Length-1].DestTable;

			string strTag = CurrentStepTagLevel.ToString();
			string strParent = CurrentStepTagLevel==1?"NULL":(CurrentStepTagLevel-1).ToString();

			retVal	=	new QSqlSelectMaker(strTag, strParent, GroupOwnerTable,this.TimeOffset,this.Language,group.Relations==null);

			AddFieldToSelect(retVal);

			// Where Extension Addon [4/30/2004]
			if(this.OwnerObject.GetWhereExtensions()!=null && GroupOwnerTable==this.OwnerObject.OwnerTable)
			{
				foreach(FilterCondition	exFilter in this.OwnerObject.GetWhereExtensions())
				{
					retVal.AddFilter(exFilter);
				}
			}
			// end [4/30/2004]
	
//			foreach(FilterCondition filter in this.Filters)
//			{
//				if(group==filter.OwnerField)
//					retVal.AddFilter(filter);
//			}

			return retVal;
		}

		protected QSqlSelectMaker	CreateKeySelect()
		{
			QSqlSelectMaker	retVal	=	null;

			string strTag = CurrentStepTagLevel.ToString();
			string strParent = CurrentStepTagLevel==1?"NULL":(CurrentStepTagLevel-1).ToString();

			retVal	=	new QSqlSelectMaker(strTag, strParent, this.OwnerObject.OwnerTable,this.TimeOffset,this.Language);

			AddItemKeyToSelect(retVal);

			// Add Filter [4/20/2004]
			foreach(FilterCondition filter in this.Filters)
			{
				retVal.AddFilter(filter);
			}

			// Where Extension Addon [4/30/2004]
			if(this.OwnerObject.GetWhereExtensions()!=null)
			{
				foreach(FilterCondition	exFilter in this.OwnerObject.GetWhereExtensions())
				{
					retVal.AddFilter(exFilter);
				}
			}
			// end [4/30/2004]


			return retVal;
		}

		protected QSqlSelectMaker	CreateLastSelect(int TagLevel, QField field, QField exField, bool bFinalStep)
		{
			QSqlSelectMaker	retVal	=	null;

			string strTag = TagLevel.ToString();
			string strParent = TagLevel==1?"NULL":(TagLevel-1).ToString();

			retVal	=	new QSqlSelectMaker(strTag, strParent, this.OwnerObject.OwnerTable,this.TimeOffset,this.Language);

			AddLastFieldToSelect(retVal,field,exField,bFinalStep);

			// Add Filter [4/20/2004]
			//if(bFinalStep)
			{
				foreach(FilterCondition filter in this.Filters)
				{
					retVal.AddFilter(filter);
				}
			}

			// Where Extension Addon [4/30/2004]
			if(this.OwnerObject.GetWhereExtensions()!=null)
			{
				foreach(FilterCondition	exFilter in this.OwnerObject.GetWhereExtensions())
				{
					retVal.AddFilter(exFilter);
				}
			}
			// end [4/30/2004]

			return retVal;
		}

		internal ArrayList CreateSQLQueryList()
		{
			// Clear All Tempery Variables [4/21/2004]
			QSqlSelectList.Clear();
			OrderByFields.Clear();
			ProcessedGroupItems = 0;
			CurrentStepTagLevel = 1;

			//Create Repoort Root
			QSqlSelectList.Add(CreateRootSelect());
			CurrentStepTagLevel++;

			foreach(QField groupField in this.Groups)
			{
				ProcessedGroupItems++;
				QSqlSelectList.Add(CreateGroupsSelect(groupField));
				CurrentStepTagLevel++;

				ProcessedGroupItems++;
				QSqlSelectList.Add(CreateGroupSelect(groupField));
				CurrentStepTagLevel++;
			}

			QSqlSelectList.Add(CreateKeySelect());

			CurrentStepTagLevel++;
			foreach(QField	availabeField in this.Fields)
			{
				QSqlSelectList.Add(CreateLastSelect(CurrentStepTagLevel, availabeField,null,false));
			}

			CurrentStepTagLevel++;
			foreach(QField	availabeField in this.Fields)
			{
				QSqlSelectList.Add(CreateLastSelect(CurrentStepTagLevel, availabeField,null,true));

				if(this.OwnerObject.GetFieldExtensions(availabeField)!=null)
				{
					foreach(QField	extField in this.OwnerObject.GetFieldExtensions(availabeField))
					{
						QSqlSelectList.Add(CreateLastSelect(CurrentStepTagLevel, availabeField, extField,true));
					}
				}
			}

			return QSqlSelectList;
		}

		public string Create()
		{
			StringBuilder	strBuilder = new StringBuilder();

			if(this.OwnerObject.GetExtensions()!=null&&_AddExObjects)
			{
				_KeyUniquePrefix	=	this.OwnerObject.OwnerTable;
			}

			ArrayList	arrayExObjectList = new ArrayList();

			// Step 1. Create Common SQL Query [4/29/2004]
			CreateSQLQueryList();

			if(this.OwnerObject.GetExtensions()!=null&&_AddExObjects)
			{
				foreach(QObject	exObject in this.OwnerObject.GetExtensions())
				{
					QMaker	exMaker	=	new QMaker(exObject, this.OwnerObject.OwnerTable,exObject.OwnerTable);

					// 2006-12-12: Fix Language Problem
					exMaker.Language = this.Language;

					foreach(QField	exField	in this.Fields)
					{
						exMaker.Fields.Add(exObject.Fields[exField.Name]);
					}

					foreach(QField	exGroup	in this.Groups)
					{
						exMaker.Groups.Add(exObject.Fields[exGroup.Name]);
					}

					foreach(FilterCondition	exFilter	in this.Filters)
					{
						if(exFilter is SimpleFilterCondition)
						{
							SimpleFilterCondition tmpFilter = (SimpleFilterCondition)exFilter;
							SimpleFilterCondition newFilter	=	new SimpleFilterCondition(exObject.Fields[tmpFilter.OwnerField.Name],"", tmpFilter.FilterType);
							newFilter.InternalValue = tmpFilter.InternalValue;

							if(tmpFilter.OwnerField.DBName!=null)
							{
								int iIndexOfKey = newFilter.InternalValue.IndexOf("@"+tmpFilter.OwnerField.DBName);
								if(iIndexOfKey!=-1)
								{
									newFilter.InternalValue = newFilter.InternalValue.Replace("@"+tmpFilter.OwnerField.DBName,"@"+newFilter.OwnerField.DBName);
								}
							}

							exMaker.Filters.Add(newFilter);
						}
						else if(exFilter is IntervalFilterCondition)
						{
							IntervalFilterCondition tmpFilter = (IntervalFilterCondition)exFilter;
							IntervalFilterCondition newFilter	=	new IntervalFilterCondition(exObject.Fields[tmpFilter.OwnerField.Name],tmpFilter.ValueLess, tmpFilter.ValueGreat);

							exMaker.Filters.Add(newFilter);
						}
						else if(exFilter is ExtendedFilterCondition)
						{
							ExtendedFilterCondition tmpFilter = (ExtendedFilterCondition)exFilter;
							ExtendedFilterCondition newFilter	=	new ExtendedFilterCondition(tmpFilter.Query, tmpFilter.Value);

							exMaker.Filters.Add(newFilter);
						}
					}

					arrayExObjectList.Add(exMaker);

//					ArrayList	addonQuery = exMaker.CreateSQLQueryList();
//
//					for(int iIndex=1;iIndex<addonQuery.Count;iIndex++)
//					{
//						QSqlSelectList.Add(addonQuery[iIndex]);
//					}
				}
			}

			// Add prev SQL Commands [5/7/2004]
			foreach(string PrevCommand in this.PrevSqlQuery)
			{
				strBuilder.Append(PrevCommand);
				strBuilder.Append("\r\n");
			}

			// Create SQL Command
			bool bRunFirst = true;

			foreach(QSqlSelectMaker sqlMaker in QSqlSelectList)
			{
				if(bRunFirst)
					bRunFirst = false;
				else
					//strBuilder.Append("\r\nUNION ALL\r\n");
					strBuilder.Append("\r\nUNION\r\n");
				strBuilder.Append( sqlMaker.Create(this.OwnerObject.OwnerTable,this.Groups));
			}

			foreach(QMaker	exMaker in arrayExObjectList)
			{
				ArrayList	addonQuery = exMaker.CreateSQLQueryList();

				for(int iIndex=1;iIndex<addonQuery.Count;iIndex++)
				{
					if(bRunFirst)
						bRunFirst = false;
					else
						//strBuilder.Append("\r\nUNION ALL\r\n");
						strBuilder.Append("\r\nUNION\r\n");

					strBuilder.Append( ((QSqlSelectMaker)addonQuery[iIndex]).Create(exMaker.OwnerObject.OwnerTable,this.Groups));
				}
			}

			// Add order By Command [4/20/2004]
			if(OrderByFields.Count>0)
			{
				strBuilder.Append("\r\nORDER BY");

				bRunFirst = true;
				foreach(string OrderAlias in OrderByFields)
				{
					if(bRunFirst)
					{
						bRunFirst = false;
						strBuilder.Append(" ");
					}
					else
						strBuilder.Append(" , ");

					strBuilder.Append(OrderAlias);
				}
			}

			// Final step [4/20/2004]
			strBuilder.Append("\r\nFOR XML EXPLICIT");

#if DEBUG
			System.Diagnostics.Trace.WriteLine(strBuilder.ToString()); 
#endif
			
			return strBuilder.ToString();
		}
	}
}
