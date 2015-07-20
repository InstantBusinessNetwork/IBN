
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
    public partial class TimeTrackingEntry: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("TimeTrackingEntry");
        }
        #endregion
        
        #region .Ctor
        public TimeTrackingEntry()
             : base(TimeTrackingEntry.GetAssignedMetaClass())
        {
        }

        public TimeTrackingEntry(MetaObjectOptions options)
             : base(TimeTrackingEntry.GetAssignedMetaClass(), options)
        {
        }
        
        public TimeTrackingEntry(PrimaryKeyId primaryKeyId)
             : base(TimeTrackingEntry.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public TimeTrackingEntry(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(TimeTrackingEntry.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public TimeTrackingEntry(CustomTableRow row)
             : base(TimeTrackingEntry.GetAssignedMetaClass(), row)
        {
        }
        
        public TimeTrackingEntry(CustomTableRow row, MetaObjectOptions options)
             : base(TimeTrackingEntry.GetAssignedMetaClass(), row, options)
        {
        }

        public TimeTrackingEntry(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public TimeTrackingEntry(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "Cost": 
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
                            case "Modified": 
                            case "ModifierId": 
                            case "ObjectId": 
                            case "ObjectTypeId": 
                            case "Owner": 
                            case "OwnerId": 
                            case "ParentBlock": 
                            case "ParentBlockId": 
                            case "Rate": 
                            case "StartDate": 
                            case "StateFriendlyName": 
                            case "TimeTrackingEntryId": 
                            case "Title": 
                            case "TotalApproved": 
                            
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
        public static TimeTrackingEntry[] List()
        {
            return MetaObject.List<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass());
        }
        
        public static TimeTrackingEntry[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(),filters);
        }

        public static TimeTrackingEntry[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(),sorting);
        }

        public static TimeTrackingEntry[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(),filters, sorting);
        }

        public static TimeTrackingEntry[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(TimeTrackingEntry.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.Boolean AreFinancesRegistered
        {
            get
            {
                return (System.Boolean)base.Properties["AreFinancesRegistered"].Value;
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
        
        public Nullable<System.Double> Cost
        {
            get
            {
                return (Nullable<System.Double>)base.Properties["Cost"].Value;
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
        
        public Nullable<System.Int32> ObjectId
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ObjectId"].Value;
            }
            
            set
            {
                base.Properties["ObjectId"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ObjectTypeId
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ObjectTypeId"].Value;
            }
            
            set
            {
                base.Properties["ObjectTypeId"].Value = value;
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
        
        public System.String ParentBlock
        {
            get
            {
                return (System.String)base.Properties["ParentBlock"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId ParentBlockId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["ParentBlockId"].Value;
            }
            
            set
            {
                base.Properties["ParentBlockId"].Value = value;
            }
            
        }
        
        public Nullable<System.Decimal> Rate
        {
            get
            {
                return (Nullable<System.Decimal>)base.Properties["Rate"].Value;
            }
            
            set
            {
                base.Properties["Rate"].Value = value;
            }
            
        }
        
        public System.DateTime StartDate
        {
            get
            {
                return (System.DateTime)base.Properties["StartDate"].Value;
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
        
        public Nullable<System.Double> TotalApproved
        {
            get
            {
                return (Nullable<System.Double>)base.Properties["TotalApproved"].Value;
            }
            
            set
            {
                base.Properties["TotalApproved"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
