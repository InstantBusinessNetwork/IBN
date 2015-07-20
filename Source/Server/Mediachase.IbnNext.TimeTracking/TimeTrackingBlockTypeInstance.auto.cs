
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



namespace Mediachase.IbnNext.TimeTracking
{
    public partial class TimeTrackingBlockTypeInstance: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("TimeTrackingBlockTypeInstance");
        }
        #endregion
        
        #region .Ctor
        public TimeTrackingBlockTypeInstance()
             : base(TimeTrackingBlockTypeInstance.GetAssignedMetaClass())
        {
        }

        public TimeTrackingBlockTypeInstance(MetaObjectOptions options)
             : base(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), options)
        {
        }
        
        public TimeTrackingBlockTypeInstance(PrimaryKeyId primaryKeyId)
             : base(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public TimeTrackingBlockTypeInstance(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public TimeTrackingBlockTypeInstance(CustomTableRow row)
             : base(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), row)
        {
        }
        
        public TimeTrackingBlockTypeInstance(CustomTableRow row, MetaObjectOptions options)
             : base(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), row, options)
        {
        }

        public TimeTrackingBlockTypeInstance(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public TimeTrackingBlockTypeInstance(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "BlockType": 
                            case "BlockTypeId": 
                            case "Created": 
                            case "CreatorId": 
                            case "IsProject": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Project": 
                            case "ProjectId": 
                            case "StatusId": 
                            case "TimeTrackingBlockTypeInstanceId": 
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
        public static TimeTrackingBlockTypeInstance[] List()
        {
            return MetaObject.List<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass());
        }
        
        public static TimeTrackingBlockTypeInstance[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(),filters);
        }

        public static TimeTrackingBlockTypeInstance[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(),sorting);
        }

        public static TimeTrackingBlockTypeInstance[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(),filters, sorting);
        }

        public static TimeTrackingBlockTypeInstance[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.String BlockType
        {
            get
            {
                return (System.String)base.Properties["BlockType"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId BlockTypeId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["BlockTypeId"].Value;
            }
            
            set
            {
                base.Properties["BlockTypeId"].Value = value;
            }
            
        }
        
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
        
        public System.Boolean IsProject
        {
            get
            {
                return (System.Boolean)base.Properties["IsProject"].Value;
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
        
        public System.String Project
        {
            get
            {
                return (System.String)base.Properties["Project"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ProjectId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ProjectId"].Value;
            }
            
            set
            {
                base.Properties["ProjectId"].Value = value;
            }
            
        }
        
        public System.Int32 StatusId
        {
            get
            {
                return (System.Int32)base.Properties["StatusId"].Value;
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
