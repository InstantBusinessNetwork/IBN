
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
    public partial class TimeTrackingBlock: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("TimeTrackingBlock");
        }
        #endregion
        
        #region .Ctor
        public TimeTrackingBlock()
             : base(TimeTrackingBlock.GetAssignedMetaClass())
        {
        }

        public TimeTrackingBlock(MetaObjectOptions options)
             : base(TimeTrackingBlock.GetAssignedMetaClass(), options)
        {
        }
        
        public TimeTrackingBlock(PrimaryKeyId primaryKeyId)
             : base(TimeTrackingBlock.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public TimeTrackingBlock(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(TimeTrackingBlock.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public TimeTrackingBlock(CustomTableRow row)
             : base(TimeTrackingBlock.GetAssignedMetaClass(), row)
        {
        }
        
        public TimeTrackingBlock(CustomTableRow row, MetaObjectOptions options)
             : base(TimeTrackingBlock.GetAssignedMetaClass(), row, options)
        {
        }

        public TimeTrackingBlock(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public TimeTrackingBlock(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "AreFinancesRegistered": 
                            case "BlockTypeInstance": 
                            case "BlockTypeInstanceId": 
                            case "Card": 
                            case "Created": 
                            case "CreatorId": 
                            case "Day1": 
                            case "Day2": 
                            case "Day3": 
                            case "Day4": 
                            case "Day5": 
                            case "Day6": 
                            case "Day7": 
                            case "DayT": 
                            case "IsRejected": 
                            case "mc_State": 
                            case "mc_StateId": 
                            case "mc_StateMachine": 
                            case "mc_StateMachineId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Owner": 
                            case "OwnerId": 
                            case "Project": 
                            case "ProjectId": 
                            case "StartDate": 
                            case "StateFriendlyName": 
                            case "TimeTrackingBlockId": 
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
        public static TimeTrackingBlock[] List()
        {
            return MetaObject.List<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass());
        }
        
        public static TimeTrackingBlock[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(),filters);
        }

        public static TimeTrackingBlock[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(),sorting);
        }

        public static TimeTrackingBlock[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(),filters, sorting);
        }

        public static TimeTrackingBlock[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(TimeTrackingBlock.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.Boolean AreFinancesRegistered
        {
            get
            {
                return (System.Boolean)base.Properties["AreFinancesRegistered"].Value;
            }
            
            set
            {
                base.Properties["AreFinancesRegistered"].Value = value;
            }
            
        }
        
        public System.String BlockTypeInstance
        {
            get
            {
                return (System.String)base.Properties["BlockTypeInstance"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId BlockTypeInstanceId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["BlockTypeInstanceId"].Value;
            }
            
            set
            {
                base.Properties["BlockTypeInstanceId"].Value = value;
            }
            
        }
        
        public System.String Card
        {
            get
            {
                return (System.String)base.Properties["Card"].Value;
            }
            
            set
            {
                base.Properties["Card"].Value = value;
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
        
        public System.Double Day1
        {
            get
            {
                return (System.Double)base.Properties["Day1"].Value;
            }
            
            set
            {
                base.Properties["Day1"].Value = value;
            }
            
        }
        
        public System.Double Day2
        {
            get
            {
                return (System.Double)base.Properties["Day2"].Value;
            }
            
            set
            {
                base.Properties["Day2"].Value = value;
            }
            
        }
        
        public System.Double Day3
        {
            get
            {
                return (System.Double)base.Properties["Day3"].Value;
            }
            
            set
            {
                base.Properties["Day3"].Value = value;
            }
            
        }
        
        public System.Double Day4
        {
            get
            {
                return (System.Double)base.Properties["Day4"].Value;
            }
            
            set
            {
                base.Properties["Day4"].Value = value;
            }
            
        }
        
        public System.Double Day5
        {
            get
            {
                return (System.Double)base.Properties["Day5"].Value;
            }
            
            set
            {
                base.Properties["Day5"].Value = value;
            }
            
        }
        
        public System.Double Day6
        {
            get
            {
                return (System.Double)base.Properties["Day6"].Value;
            }
            
            set
            {
                base.Properties["Day6"].Value = value;
            }
            
        }
        
        public System.Double Day7
        {
            get
            {
                return (System.Double)base.Properties["Day7"].Value;
            }
            
            set
            {
                base.Properties["Day7"].Value = value;
            }
            
        }
        
        public System.Double DayT
        {
            get
            {
                return (System.Double)base.Properties["DayT"].Value;
            }
            
        }
        
        public System.Boolean IsRejected
        {
            get
            {
                return (System.Boolean)base.Properties["IsRejected"].Value;
            }
            
            set
            {
                base.Properties["IsRejected"].Value = value;
            }
            
        }
        
        public System.String mc_State
        {
            get
            {
                return (System.String)base.Properties["mc_State"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId mc_StateId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["mc_StateId"].Value;
            }
            
            set
            {
                base.Properties["mc_StateId"].Value = value;
            }
            
        }
        
        public System.String mc_StateMachine
        {
            get
            {
                return (System.String)base.Properties["mc_StateMachine"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId mc_StateMachineId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["mc_StateMachineId"].Value;
            }
            
            set
            {
                base.Properties["mc_StateMachineId"].Value = value;
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
        
        public System.String Owner
        {
            get
            {
                return (System.String)base.Properties["Owner"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId OwnerId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["OwnerId"].Value;
            }
            
            set
            {
                base.Properties["OwnerId"].Value = value;
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
        
        public System.DateTime StartDate
        {
            get
            {
                return (System.DateTime)base.Properties["StartDate"].Value;
            }
            
            set
            {
                base.Properties["StartDate"].Value = value;
            }
            
        }
        
        public System.String StateFriendlyName
        {
            get
            {
                return (System.String)base.Properties["StateFriendlyName"].Value;
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
