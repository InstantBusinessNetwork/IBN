
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



namespace Mediachase.Ibn.Calendar
{
    public partial class CalendarFolder: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("CalendarFolder");
        }
        #endregion
        
        #region .Ctor
        public CalendarFolder()
             : base(CalendarFolder.GetAssignedMetaClass())
        {
        }

        public CalendarFolder(MetaObjectOptions options)
             : base(CalendarFolder.GetAssignedMetaClass(), options)
        {
        }
        
        public CalendarFolder(int primaryKeyId)
             : base(CalendarFolder.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public CalendarFolder(int primaryKeyId, MetaObjectOptions options)
             : base(CalendarFolder.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public CalendarFolder(CustomTableRow row)
             : base(CalendarFolder.GetAssignedMetaClass(), row)
        {
        }
        
        public CalendarFolder(CustomTableRow row, MetaObjectOptions options)
             : base(CalendarFolder.GetAssignedMetaClass(), row, options)
        {
        }

        public CalendarFolder(MetaClass metaType, int primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public CalendarFolder(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "Calendar": 
                            case "CalendarId": 
                            case "HasChildren": 
                            case "OutlineLevel": 
                            case "OutlineNumber": 
                            case "Owner": 
                            case "Parent": 
                            case "ParentId": 
                            case "Project": 
                            case "ProjectId": 
                            case "ScopeIndex": 
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
        public static CalendarFolder[] List()
        {
            return MetaObject.List<CalendarFolder>(CalendarFolder.GetAssignedMetaClass());
        }
        
        public static CalendarFolder[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<CalendarFolder>(CalendarFolder.GetAssignedMetaClass(),filters);
        }

        public static CalendarFolder[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<CalendarFolder>(CalendarFolder.GetAssignedMetaClass(),sorting);
        }

        public static CalendarFolder[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<CalendarFolder>(CalendarFolder.GetAssignedMetaClass(),filters, sorting);
        }

        public static CalendarFolder[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<CalendarFolder>(CalendarFolder.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(CalendarFolder.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.String Calendar
        {
            get
            {
                return (System.String)base.Properties["Calendar"].Value;
            }
            
        }

		public Nullable<PrimaryKeyId> CalendarId
        {
            get
            {
				return (Nullable<PrimaryKeyId>)base.Properties["CalendarId"].Value;
            }
            
            set
            {
                base.Properties["CalendarId"].Value = value;
            }
            
        }
        
        public System.Boolean HasChildren
        {
            get
            {
                return (System.Boolean)base.Properties["HasChildren"].Value;
            }
            
            set
            {
                base.Properties["HasChildren"].Value = value;
            }
            
        }
        
        public System.Int32 OutlineLevel
        {
            get
            {
                return (System.Int32)base.Properties["OutlineLevel"].Value;
            }
            
            set
            {
                base.Properties["OutlineLevel"].Value = value;
            }
            
        }
        
        public System.String OutlineNumber
        {
            get
            {
                return (System.String)base.Properties["OutlineNumber"].Value;
            }
            
            set
            {
                base.Properties["OutlineNumber"].Value = value;
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
        
        public System.String Parent
        {
            get
            {
                return (System.String)base.Properties["Parent"].Value;
            }
            
        }

		public Nullable<PrimaryKeyId> ParentId
        {
            get
            {
				return (Nullable<PrimaryKeyId>)base.Properties["ParentId"].Value;
            }
            
            set
            {
                base.Properties["ParentId"].Value = value;
            }
            
        }
        
        public System.String Project
        {
            get
            {
                return (System.String)base.Properties["Project"].Value;
            }
            
        }

		public Nullable<PrimaryKeyId> ProjectId
        {
            get
            {
				return (Nullable<PrimaryKeyId>)base.Properties["ProjectId"].Value;
            }
            
            set
            {
                base.Properties["ProjectId"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ScopeIndex
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ScopeIndex"].Value;
            }
            
            set
            {
                base.Properties["ScopeIndex"].Value = value;
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
