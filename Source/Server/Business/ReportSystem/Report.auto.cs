
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// An auto generated class. Don't modify it manually.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;



namespace Mediachase.IBN.Business.ReportSystem
{
    internal partial class Report: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("Report");
        }
        #endregion
        
        #region .Ctor
        public Report()
             : base(Report.GetAssignedMetaClass())
        {
        }

        public Report(MetaObjectOptions options)
             : base(Report.GetAssignedMetaClass(), options)
        {
        }
        
        public Report(PrimaryKeyId primaryKeyId)
             : base(Report.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public Report(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(Report.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public Report(CustomTableRow row)
             : base(Report.GetAssignedMetaClass(), row)
        {
        }
        
        public Report(CustomTableRow row, MetaObjectOptions options)
             : base(Report.GetAssignedMetaClass(), row, options)
        {
        }

        public Report(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public Report(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
            : base(metaType, row, options)
        {
        }
        #endregion

        #region Extended Properties
        public MetaObjectProperty[] ExtendedProperties
        {
            get
            {
                if(_exProperies==null)
                {
                    List<MetaObjectProperty> retVal = new List<MetaObjectProperty>();
                    
                    foreach(MetaObjectProperty property in base.Properties)
                    {
                        switch(property.Name)
                        {
                            case "Created": 
                            case "CreatorId": 
                            case "DefaultFilterSql": 
                            case "DefaultFilterXml": 
                            case "Description": 
                            case "FileName": 
                            case "FilterControl": 
                            case "IsCustom": 
                            case "IsFilterable": 
                            case "IsPersonal": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Owner": 
                            case "RdlText": 
                            case "ReportId": 
                            case "TimeZoneId": 
                            case "Title": 
                            
                                 break;
                            default:
                                 retVal.Add(property);    
                                 break;
                        }
                    }
                    _exProperies = retVal.ToArray();
                }
                
                return _exProperies;
            }
        }
        #endregion
        
        #region Static Methods (List + GetTotalCount)
        public static Report[] List()
        {
            return MetaObject.List<Report>(Report.GetAssignedMetaClass());
        }
        
        public static Report[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<Report>(Report.GetAssignedMetaClass(),filters);
        }

        public static Report[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<Report>(Report.GetAssignedMetaClass(),sorting);
        }

        public static Report[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<Report>(Report.GetAssignedMetaClass(),filters, sorting);
        }

        public static Report[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<Report>(Report.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(Report.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.DateTime Created
        {
            get
            {
                return (System.DateTime)base.Properties["Created"].Value;
            }
            
            set
            {
                base.Properties["Created"].Value = value;
            }
            
        }
        
        public System.Int32 CreatorId
        {
            get
            {
                return (System.Int32)base.Properties["CreatorId"].Value;
            }
            
            set
            {
                base.Properties["CreatorId"].Value = value;
            }
            
        }
        
        public System.String DefaultFilterSql
        {
            get
            {
                return (System.String)base.Properties["DefaultFilterSql"].Value;
            }
            
            set
            {
                base.Properties["DefaultFilterSql"].Value = value;
            }
            
        }
        
        public System.String DefaultFilterXml
        {
            get
            {
                return (System.String)base.Properties["DefaultFilterXml"].Value;
            }
            
            set
            {
                base.Properties["DefaultFilterXml"].Value = value;
            }
            
        }
        
        public System.String Description
        {
            get
            {
                return (System.String)base.Properties["Description"].Value;
            }
            
            set
            {
                base.Properties["Description"].Value = value;
            }
            
        }
        
        public System.String FileName
        {
            get
            {
                return (System.String)base.Properties["FileName"].Value;
            }
            
            set
            {
                base.Properties["FileName"].Value = value;
            }
            
        }
        
        public System.String FilterControl
        {
            get
            {
                return (System.String)base.Properties["FilterControl"].Value;
            }
            
            set
            {
                base.Properties["FilterControl"].Value = value;
            }
            
        }
        
        public System.Boolean IsCustom
        {
            get
            {
                return (System.Boolean)base.Properties["IsCustom"].Value;
            }
            
            set
            {
                base.Properties["IsCustom"].Value = value;
            }
            
        }
        
        public System.Boolean IsFilterable
        {
            get
            {
                return (System.Boolean)base.Properties["IsFilterable"].Value;
            }
            
            set
            {
                base.Properties["IsFilterable"].Value = value;
            }
            
        }
        
        public System.Boolean IsPersonal
        {
            get
            {
                return (System.Boolean)base.Properties["IsPersonal"].Value;
            }
            
            set
            {
                base.Properties["IsPersonal"].Value = value;
            }
            
        }
        
        public System.DateTime Modified
        {
            get
            {
                return (System.DateTime)base.Properties["Modified"].Value;
            }
            
            set
            {
                base.Properties["Modified"].Value = value;
            }
            
        }
        
        public System.Int32 ModifierId
        {
            get
            {
                return (System.Int32)base.Properties["ModifierId"].Value;
            }
            
            set
            {
                base.Properties["ModifierId"].Value = value;
            }
            
        }
        
        public System.Object Owner
        {
            get
            {
                return (System.Object)base.Properties["Owner"].Value;
            }
            
            set
            {
                base.Properties["Owner"].Value = value;
            }
            
        }
        
        public System.String RdlText
        {
            get
            {
                return (System.String)base.Properties["RdlText"].Value;
            }
            
            set
            {
                base.Properties["RdlText"].Value = value;
            }
            
        }
        
        public System.Int32 TimeZoneId
        {
            get
            {
                return (System.Int32)base.Properties["TimeZoneId"].Value;
            }
            
            set
            {
                base.Properties["TimeZoneId"].Value = value;
            }
            
        }
        
        public System.String Title
        {
            get
            {
                return (System.String)base.Properties["Title"].Value;
            }
            
            set
            {
                base.Properties["Title"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
