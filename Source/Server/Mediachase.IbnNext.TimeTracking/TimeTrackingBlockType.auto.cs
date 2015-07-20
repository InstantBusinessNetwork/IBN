
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
    public partial class TimeTrackingBlockType: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("TimeTrackingBlockType");
        }
        #endregion
        
        #region .Ctor
        public TimeTrackingBlockType()
             : base(TimeTrackingBlockType.GetAssignedMetaClass())
        {
        }

        public TimeTrackingBlockType(MetaObjectOptions options)
             : base(TimeTrackingBlockType.GetAssignedMetaClass(), options)
        {
        }
        
        public TimeTrackingBlockType(PrimaryKeyId primaryKeyId)
             : base(TimeTrackingBlockType.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public TimeTrackingBlockType(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(TimeTrackingBlockType.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public TimeTrackingBlockType(CustomTableRow row)
             : base(TimeTrackingBlockType.GetAssignedMetaClass(), row)
        {
        }
        
        public TimeTrackingBlockType(CustomTableRow row, MetaObjectOptions options)
             : base(TimeTrackingBlockType.GetAssignedMetaClass(), row, options)
        {
        }

        public TimeTrackingBlockType(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public TimeTrackingBlockType(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "BlockCard": 
                            case "Created": 
                            case "CreatorId": 
                            case "EntryCard": 
                            case "IsProject": 
                            case "Modified": 
                            case "ModifierId": 
                            case "StateMachine": 
                            case "StateMachineId": 
                            case "TimeTrackingBlockTypeId": 
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
        public static TimeTrackingBlockType[] List()
        {
            return MetaObject.List<TimeTrackingBlockType>(TimeTrackingBlockType.GetAssignedMetaClass());
        }
        
        public static TimeTrackingBlockType[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<TimeTrackingBlockType>(TimeTrackingBlockType.GetAssignedMetaClass(),filters);
        }

        public static TimeTrackingBlockType[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<TimeTrackingBlockType>(TimeTrackingBlockType.GetAssignedMetaClass(),sorting);
        }

        public static TimeTrackingBlockType[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<TimeTrackingBlockType>(TimeTrackingBlockType.GetAssignedMetaClass(),filters, sorting);
        }

        public static TimeTrackingBlockType[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<TimeTrackingBlockType>(TimeTrackingBlockType.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(TimeTrackingBlockType.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.String BlockCard
        {
            get
            {
                return (System.String)base.Properties["BlockCard"].Value;
            }
            
            set
            {
                base.Properties["BlockCard"].Value = value;
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
        
        public System.String EntryCard
        {
            get
            {
                return (System.String)base.Properties["EntryCard"].Value;
            }
            
            set
            {
                base.Properties["EntryCard"].Value = value;
            }
            
        }
        
        public System.Boolean IsProject
        {
            get
            {
                return (System.Boolean)base.Properties["IsProject"].Value;
            }
            
            set
            {
                base.Properties["IsProject"].Value = value;
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
        
        public System.String StateMachine
        {
            get
            {
                return (System.String)base.Properties["StateMachine"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId StateMachineId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["StateMachineId"].Value;
            }
            
            set
            {
                base.Properties["StateMachineId"].Value = value;
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
