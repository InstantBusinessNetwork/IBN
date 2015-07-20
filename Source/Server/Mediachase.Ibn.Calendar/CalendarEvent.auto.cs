
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
    public partial class CalendarEvent: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("CalendarEvent");
        }
        #endregion
        
        #region .Ctor
        public CalendarEvent()
             : base(CalendarEvent.GetAssignedMetaClass())
        {
        }

        public CalendarEvent(MetaObjectOptions options)
             : base(CalendarEvent.GetAssignedMetaClass(), options)
        {
        }
        
        public CalendarEvent(int primaryKeyId)
             : base(CalendarEvent.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public CalendarEvent(int primaryKeyId, MetaObjectOptions options)
             : base(CalendarEvent.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public CalendarEvent(CustomTableRow row)
             : base(CalendarEvent.GetAssignedMetaClass(), row)
        {
        }
        
        public CalendarEvent(CustomTableRow row, MetaObjectOptions options)
             : base(CalendarEvent.GetAssignedMetaClass(), row, options)
        {
        }

        public CalendarEvent(MetaClass metaType, int primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public CalendarEvent(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "Card": 
                            case "Description": 
                            case "DtEnd": 
                            case "DtStart": 
                            case "PrimaryCalendar": 
                            case "PrimaryCalendarId": 
                            case "Project": 
                            case "ProjectId": 
                            case "Summary": 
                            case "Title": 
                            case "Transp": 
                            
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
        public static CalendarEvent[] List()
        {
            return MetaObject.List<CalendarEvent>(CalendarEvent.GetAssignedMetaClass());
        }
        
        public static CalendarEvent[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<CalendarEvent>(CalendarEvent.GetAssignedMetaClass(),filters);
        }

        public static CalendarEvent[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<CalendarEvent>(CalendarEvent.GetAssignedMetaClass(),sorting);
        }

        public static CalendarEvent[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<CalendarEvent>(CalendarEvent.GetAssignedMetaClass(),filters, sorting);
        }

        public static CalendarEvent[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<CalendarEvent>(CalendarEvent.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(CalendarEvent.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
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
        
        public System.DateTime DtEnd
        {
            get
            {
                return (System.DateTime)base.Properties["DtEnd"].Value;
            }
            
            set
            {
                base.Properties["DtEnd"].Value = value;
            }
            
        }
        
        public System.DateTime DtStart
        {
            get
            {
                return (System.DateTime)base.Properties["DtStart"].Value;
            }
            
            set
            {
                base.Properties["DtStart"].Value = value;
            }
            
        }
        
        public System.String PrimaryCalendar
        {
            get
            {
                return (System.String)base.Properties["PrimaryCalendar"].Value;
            }
            
        }

		public Nullable<PrimaryKeyId> PrimaryCalendarId
        {
            get
            {
				return (Nullable<PrimaryKeyId>)base.Properties["PrimaryCalendarId"].Value;
            }
            
            set
            {
                base.Properties["PrimaryCalendarId"].Value = value;
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
        
        public System.String Summary
        {
            get
            {
                return (System.String)base.Properties["Summary"].Value;
            }
            
            set
            {
                base.Properties["Summary"].Value = value;
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
        
        public System.Boolean Transp
        {
            get
            {
                return (System.Boolean)base.Properties["Transp"].Value;
            }
            
            set
            {
                base.Properties["Transp"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
