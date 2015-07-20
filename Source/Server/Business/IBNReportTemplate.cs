using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;

using Mediachase.Ibn;
using Mediachase.SQLQueryCreator;
using System.Text;


namespace Mediachase.IBN.Business.Reports
{
#region Report Template Classes
	public class FieldInfo
	{
		public FieldInfo()
		{
		}

		public FieldInfo(string Name, string DataType)
		{
			this.Name = Name;
			this.DataType = DataType;
		}

		public string Name	=	string.Empty;
		public string DataType	=	string.Empty;
	}

	public class FilterInfo
	{
		public FilterInfo()
		{
		}

		public FilterInfo(string FieldName, string DataType)
		{
			this.FieldName = FieldName;
			this.DataType = DataType;
		}

		public string FieldName	=	string.Empty;
		public string DataType	=	string.Empty;
		public StringCollection	Values	=	new StringCollection();
	}

	public enum SortDirection
	{
		ASC	=	1,
		DESC
	}

	public class SortInfo
	{
		public SortInfo()
		{
		}

		public SortInfo(string FieldName, string DataType, SortDirection SortDirection)
		{
			this.FieldName = FieldName;
			this.DataType = DataType;
			this.SortDirection = SortDirection;
		}

		public string FieldName	=	string.Empty;
		public string DataType	=	string.Empty;
		public SortDirection SortDirection = SortDirection.ASC;
	}
	#endregion


#region Report Template Collections
	public class FieldCollection: CollectionBase
	{
		public FieldCollection()
		{
		}

		public FieldInfo this[int Index]
		{
			get
			{
				return (FieldInfo)this.InnerList[Index];
			}
			set
			{
				if(value==null)
					throw new ArgumentNullException();
				this.InnerList[Index] = value;
			}
		}

		public FieldInfo this[string Name]
		{
			get
			{
				foreach(FieldInfo item in this.InnerList)
				{
					if(item.Name==Name)
						return item;
				}
				return null;
			}
		}

		public void Add(FieldInfo	field)
		{
			if(field==null)
				throw new ArgumentNullException();

			this.InnerList.Add(field);
		}

		public void Insert(int index, FieldInfo	field)
		{
			if(field==null)
				throw new ArgumentNullException();

			this.InnerList.Insert(index,field);
		}

		public int IndexOf(FieldInfo	field)
		{
			return this.InnerList.IndexOf(field);
		}
	}

	public class FilterCollection: CollectionBase
	{
		public FilterCollection()
		{
		}

		public FilterInfo this[string FieldName]
		{
			get
			{
				foreach(FilterInfo item in this.InnerList)
				{
					if(item.FieldName==FieldName)
						return item;
				}
				return null;
			}
		}

		public FilterInfo this[int Index]
		{
			get
			{
				return (FilterInfo)this.InnerList[Index];
			}
			set
			{
				if(value==null)
					throw new ArgumentNullException();
				this.InnerList[Index] = value;
			}
		}

		public void Add(FilterInfo	field)
		{
			if(field==null)
				throw new ArgumentNullException();

			this.InnerList.Add(field);
		}

		public void Insert(int index, FilterInfo	field)
		{
			if(field==null)
				throw new ArgumentNullException();

			this.InnerList.Insert(index,field);
		}

		public int IndexOf(FilterInfo	field)
		{
			return this.InnerList.IndexOf(field);
		}
	}

	public class SortCollection: CollectionBase
	{
		public SortCollection()
		{
		}

		public SortInfo this[string FieldName]
		{
			get
			{
				foreach(SortInfo item in this.InnerList)
				{
					if(item.FieldName==FieldName)
						return item;
				}
				return null;
			}
		}

		public SortInfo this[int Index]
		{
			get
			{
				return (SortInfo)this.InnerList[Index];
			}
			set
			{
				if(value==null)
					throw new ArgumentNullException();
				this.InnerList[Index] = value;
			}
		}

		public void Add(SortInfo	field)
		{
			if(field==null)
				throw new ArgumentNullException();

			this.InnerList.Add(field);
		}

		public void Insert(int index, SortInfo	field)
		{
			if(field==null)
				throw new ArgumentNullException();

			this.InnerList.Insert(index,field);
		}

		public int IndexOf(SortInfo	field)
		{
			return this.InnerList.IndexOf(field);
		}
	}
	#endregion


#region IBNReportTemplate
	/// <summary>
	/// Summary description for IBNReportTemplate.
	/// </summary>
	public class IBNReportTemplate
	{
		private string _Version = IbnConst.VersionMajorDotMinor;
		private string _Name	=	string.Empty;
		private string _ViewType=	string.Empty;
		private string _ObjectName=	string.Empty;
		private DateTime	_Created = DateTime.Now;
		private string _Author	=	string.Empty;
		private FieldCollection	_fields = new FieldCollection();
		private FilterCollection	_filters = new FilterCollection();
		private FieldCollection	_groups = new FieldCollection();
		private SortCollection	_sorts	=	new SortCollection();
		private bool	_ShowEmpty = true;

		public IBNReportTemplate():this(null,null)
		{
		}

		public IBNReportTemplate(QMaker	maker):this(maker,null)
		{
		}

		public IBNReportTemplate(QMaker	maker, SortCollection	coll)
		{
			if(maker!=null)
			{
				foreach(Mediachase.SQLQueryCreator.QField field in maker.Fields)
				{
					this.Fields.Add(new FieldInfo(field.Name,field.DataType.ToString()));
				}

//				foreach(Mediachase.SQLQueryCreator.FilterCondition filter in maker.Filters)
//				{
//					FilterInfo info = new FilterInfo(filter.OwnerField.Name,filter.OwnerField.DataType.ToString());
//
//					this.Filters.Add(info);
//				}

				foreach(Mediachase.SQLQueryCreator.QField group in maker.Groups)
				{
					this.Groups.Add(new FieldInfo(group.Name,group.DataType.ToString()));
				}
			}

			if(coll!=null)
			{
				foreach(SortInfo info in coll)
				{
					this.Sorting.Add(info);
				}
			}
		}

		public IBNReportTemplate(QMaker	maker, SortCollection	coll, string Name, string ObjectName, string ViewType, string Author, DateTime Created):this(maker,coll)
		{
			this.Name = Name;
			this.ObjectName = ObjectName;
			this.Author  = Author;
			this.ViewType = ViewType;
			this.Created = Created;
		}

		public static IBNReportTemplate Load(string	xml)
		{
			try 
			{
				XmlDocument	doc = new XmlDocument();
				doc.LoadXml(xml);
				return new IBNReportTemplate(doc);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex,"IBNReportTemplate");
				return null;
			}
		}

		public IBNReportTemplate(XmlDocument	xmlDoc)
		{
			this.Name = xmlDoc.SelectSingleNode("IBNReportTemplate/Name").InnerXml;
			this.Created = DateTime.Parse(xmlDoc.SelectSingleNode("IBNReportTemplate/Created").InnerXml);
			this.Author = xmlDoc.SelectSingleNode("IBNReportTemplate/Author").InnerXml;
			this.ViewType = xmlDoc.SelectSingleNode("IBNReportTemplate/ViewType").InnerXml;
			this.ObjectName = xmlDoc.SelectSingleNode("IBNReportTemplate/ObjectName").InnerXml;
			this.Version = xmlDoc.SelectSingleNode("IBNReportTemplate/Version").InnerXml;

			// Fields [12/14/2004]
			XmlNodeList xmlFields = xmlDoc.SelectNodes("IBNReportTemplate/Fields/Field");

			foreach(XmlNode xmlItem in xmlFields)
			{
				this.Fields.Add(new FieldInfo(xmlItem.Attributes["Name"].Value,xmlItem.Attributes["DataType"].Value));
			}

			// Group [12/14/2004]
			XmlNodeList xmlGroups = xmlDoc.SelectNodes("IBNReportTemplate/Groups/Field");

			this.ShowEmptyGroup = xmlDoc.SelectSingleNode("IBNReportTemplate/Groups").Attributes["ShowEmpty"].Value=="1";

			foreach(XmlNode xmlItem in xmlGroups)
			{
				this.Groups.Add(new FieldInfo(xmlItem.Attributes["Name"].Value,xmlItem.Attributes["DataType"].Value));
			}

			// Filters [12/14/2004]
			XmlNodeList xmlFilters = xmlDoc.SelectNodes("IBNReportTemplate/Filters/Filter");
			foreach(XmlNode xmlItem in xmlFilters)
			{
				FilterInfo	filter = new FilterInfo(xmlItem.Attributes["FieldName"].Value,xmlItem.Attributes["DataType"].Value);

				foreach(XmlNode xmlValue in xmlItem.SelectNodes("Value"))
				{
					filter.Values.Add(xmlValue.InnerXml);
				}

				this.Filters.Add(filter);
			}

			// Sorting [12/14/2004]
			XmlNodeList xmlSorts = xmlDoc.SelectNodes("IBNReportTemplate/Sorting/Sort");
			foreach(XmlNode xmlItem in xmlSorts)
			{
				this.Sorting.Add(new SortInfo(xmlItem.Attributes["FieldName"].Value,xmlItem.Attributes["DataType"].Value,xmlItem.Attributes["Asc"].Value=="1"?SortDirection.ASC:SortDirection.DESC ));
			}
		}

#region Properties
		public string Version
		{
			get
			{
				return _Version;
			}
			set
			{
				_Version = value;
			}
		}

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public string ViewType
		{
			get
			{
				return _ViewType;
			}
			set
			{
				_ViewType = value;
			}
		}

		public string ObjectName
		{
			get
			{
				return _ObjectName;
			}
			set
			{
				_ObjectName = value;
			}
		}

		public DateTime Created
		{
			get
			{
				return _Created;
			}
			set
			{
				_Created = value;
			}
		}

		public string Author
		{
			get
			{
				return _Author;
			}
			set
			{
				_Author = value;
			}
		}

		public FieldCollection Fields
		{
			get
			{
				return _fields;
			}
		}

		public bool ShowEmptyGroup
		{
			get
			{
				return _ShowEmpty;
			}
			set
			{
				_ShowEmpty = value;
			}
		}

		public FieldCollection Groups
		{
			get
			{
				return _groups;
			}
		}

		public FilterCollection Filters
		{
			get
			{
				return _filters;
			}
		}

		public SortCollection Sorting
		{
			get
			{
				return _sorts;
			}
		}
		#endregion

#region XML Methods 
		public XmlDocument	CreateXMLTemplate()
		{
			XmlDocument	xmlDoc	=	new XmlDocument();

			xmlDoc.LoadXml("<IBNReportTemplate><Version>" + IbnConst.VersionMajorDotMinor + "</Version><Name/><Created/><Author/><ViewType/><ObjectName/><Fields/><Groups/><Filters/><Sorting/></IBNReportTemplate>");

			xmlDoc.SelectSingleNode("IBNReportTemplate/Name").InnerText = this.Name;
			xmlDoc.SelectSingleNode("IBNReportTemplate/Created").InnerText = this.Created.ToString("s");
			xmlDoc.SelectSingleNode("IBNReportTemplate/Author").InnerText = this.Author;
			xmlDoc.SelectSingleNode("IBNReportTemplate/ViewType").InnerText = this.ViewType;
			xmlDoc.SelectSingleNode("IBNReportTemplate/ObjectName").InnerText = this.ObjectName;
			xmlDoc.SelectSingleNode("IBNReportTemplate/Version").InnerText = this.Version;

			// Fields [12/14/2004]
			XmlNode xmlFields = xmlDoc.SelectSingleNode("IBNReportTemplate/Fields");
			foreach(FieldInfo item in this.Fields)
			{
				XmlNode xmlItem = xmlDoc.CreateElement("Field");

				XmlAttribute	atrName = xmlDoc.CreateAttribute("Name");
				atrName.Value = item.Name;

				XmlAttribute	atrDataType = xmlDoc.CreateAttribute("DataType");
				atrDataType.Value = item.DataType;

				xmlItem.Attributes.Append(atrName);
				xmlItem.Attributes.Append(atrDataType);

				xmlFields.AppendChild(xmlItem);
			}

			// Group [12/14/2004]
			XmlNode xmlGroups = xmlDoc.SelectSingleNode("IBNReportTemplate/Groups");

			XmlAttribute	atrShowEmpty = xmlDoc.CreateAttribute("ShowEmpty");
			atrShowEmpty.Value = this.ShowEmptyGroup?"1":"0";
			xmlGroups.Attributes.Append(atrShowEmpty);

			foreach(FieldInfo item in this.Groups)
			{
				XmlNode xmlItem = xmlDoc.CreateElement("Field");

				XmlAttribute	atrName = xmlDoc.CreateAttribute("Name");
				atrName.Value = item.Name;

				XmlAttribute	atrDataType = xmlDoc.CreateAttribute("DataType");
				atrDataType.Value = item.DataType;

				xmlItem.Attributes.Append(atrName);
				xmlItem.Attributes.Append(atrDataType);

				xmlGroups.AppendChild(xmlItem);
			}

			// Filters [12/14/2004]
			XmlNode xmlFilters = xmlDoc.SelectSingleNode("IBNReportTemplate/Filters");
			foreach(FilterInfo item in this.Filters)
			{
				XmlNode xmlItem = xmlDoc.CreateElement("Filter");

				XmlAttribute	atrName = xmlDoc.CreateAttribute("FieldName");
				atrName.Value = item.FieldName;

				XmlAttribute	atrDataType = xmlDoc.CreateAttribute("DataType");
				atrDataType.Value = item.DataType;

				xmlItem.Attributes.Append(atrName);
				xmlItem.Attributes.Append(atrDataType);

				foreach(string Value in item.Values)
				{
					XmlNode xmlValue = xmlDoc.CreateElement("Value");
					xmlValue.InnerXml = Value;

					xmlItem.AppendChild(xmlValue);
				}

				xmlFilters.AppendChild(xmlItem);
			}

			// Sorting [12/14/2004]
			XmlNode xmlSorts = xmlDoc.SelectSingleNode("IBNReportTemplate/Sorting");
			foreach(SortInfo item in this.Sorting)
			{
				XmlNode xmlItem = xmlDoc.CreateElement("Sort");

				XmlAttribute	atrName = xmlDoc.CreateAttribute("FieldName");
				atrName.Value = item.FieldName;

				XmlAttribute	atrDataType = xmlDoc.CreateAttribute("DataType");
				atrDataType.Value = item.DataType;

				XmlAttribute	atrAsc = xmlDoc.CreateAttribute("Asc");
				atrAsc.Value = (item.SortDirection==SortDirection.ASC?"1":"0");

				xmlItem.Attributes.Append(atrName);
				xmlItem.Attributes.Append(atrDataType);
				xmlItem.Attributes.Append(atrAsc);

				xmlSorts.AppendChild(xmlItem);
			}
            
			return xmlDoc;
		}

//		public string	CreateSortXML()
//		{
//			return string.Empty;
//		}
#endregion

#region QMaker Methods 
//		public QMaker	CreateQMaker(QObject	owerObject)
//		{
//			return null;
//		}

#endregion
	}
#endregion
}
