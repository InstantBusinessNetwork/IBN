
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
    public partial class CalendarEventRecurrence: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("CalendarEventRecurrence");
        }
        #endregion
        
        #region .Ctor
        public CalendarEventRecurrence()
             : base(CalendarEventRecurrence.GetAssignedMetaClass())
        {
        }

        public CalendarEventRecurrence(MetaObjectOptions options)
             : base(CalendarEventRecurrence.GetAssignedMetaClass(), options)
        {
        }
        
        public CalendarEventRecurrence(int primaryKeyId)
             : base(CalendarEventRecurrence.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public CalendarEventRecurrence(int primaryKeyId, MetaObjectOptions options)
             : base(CalendarEventRecurrence.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public CalendarEventRecurrence(CustomTableRow row)
             : base(CalendarEventRecurrence.GetAssignedMetaClass(), row)
        {
        }
        
        public CalendarEventRecurrence(CustomTableRow row, MetaObjectOptions options)
             : base(CalendarEventRecurrence.GetAssignedMetaClass(), row, options)
        {
        }

        public CalendarEventRecurrence(MetaClass metaType, int primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public CalendarEventRecurrence(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "CalendarEventRecurrenceId": 
                            case "DtEnd": 
                            case "DtStart": 
                            case "Event": 
                            case "EventId": 
                            case "Exdate": 
                            case "Exrule": 
                            case "Rdate": 
                            case "Rrule": 
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
        public static CalendarEventRecurrence[] List()
        {
            return MetaObject.List<CalendarEventRecurrence>(CalendarEventRecurrence.GetAssignedMetaClass());
        }
        
        public static CalendarEventRecurrence[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<CalendarEventRecurrence>(CalendarEventRecurrence.GetAssignedMetaClass(),filters);
        }

        public static CalendarEventRecurrence[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<CalendarEventRecurrence>(CalendarEventRecurrence.GetAssignedMetaClass(),sorting);
        }

        public static CalendarEventRecurrence[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<CalendarEventRecurrence>(CalendarEventRecurrence.GetAssignedMetaClass(),filters, sorting);
        }

        public static CalendarEventRecurrence[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<CalendarEventRecurrence>(CalendarEventRecurrence.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(CalendarEventRecurrence.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public Nullable<System.DateTime> DtEnd
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["DtEnd"].Value;
            }
            
            set
            {
                base.Properties["DtEnd"].Value = value;
            }
            
        }
        
        public Nullable<System.DateTime> DtStart
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["DtStart"].Value;
            }
            
            set
            {
                base.Properties["DtStart"].Value = value;
            }
            
        }
        
        public System.String Event
        {
            get
            {
                return (System.String)base.Properties["Event"].Value;
            }
            
        }

		public Nullable<PrimaryKeyId> EventId
        {
            get
            {
				return (Nullable<PrimaryKeyId>)base.Properties["EventId"].Value;
            }
            
            set
            {
                base.Properties["EventId"].Value = value;
            }
            
        }
        
        public System.String Exdate
        {
            get
            {
                return (System.String)base.Properties["Exdate"].Value;
            }
            
            set
            {
                base.Properties["Exdate"].Value = value;
            }
            
        }
        
        public System.String Exrule
        {
            get
            {
                return (System.String)base.Properties["Exrule"].Value;
            }
            
            set
            {
                base.Properties["Exrule"].Value = value;
            }
            
        }
        
        public System.String Rdate
        {
            get
            {
                return (System.String)base.Properties["Rdate"].Value;
            }
            
            set
            {
                base.Properties["Rdate"].Value = value;
            }
            
        }
        
        public System.String Rrule
        {
            get
            {
                return (System.String)base.Properties["Rrule"].Value;
            }
            
            set
            {
                base.Properties["Rrule"].Value = value;
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
