
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
    public partial class CalendarEventLink: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("CalendarEventLink");
        }
        #endregion
        
        #region .Ctor
        public CalendarEventLink()
             : base(CalendarEventLink.GetAssignedMetaClass())
        {
        }

        public CalendarEventLink(MetaObjectOptions options)
             : base(CalendarEventLink.GetAssignedMetaClass(), options)
        {
        }
        
        public CalendarEventLink(int primaryKeyId)
             : base(CalendarEventLink.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public CalendarEventLink(int primaryKeyId, MetaObjectOptions options)
             : base(CalendarEventLink.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public CalendarEventLink(CustomTableRow row)
             : base(CalendarEventLink.GetAssignedMetaClass(), row)
        {
        }
        
        public CalendarEventLink(CustomTableRow row, MetaObjectOptions options)
             : base(CalendarEventLink.GetAssignedMetaClass(), row, options)
        {
        }

        public CalendarEventLink(MetaClass metaType, int primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public CalendarEventLink(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "CalendarEventLinkId": 
                            case "CalendarId": 
                            case "Event": 
                            case "EventId": 
                            
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
        public static CalendarEventLink[] List()
        {
            return MetaObject.List<CalendarEventLink>(CalendarEventLink.GetAssignedMetaClass());
        }
        
        public static CalendarEventLink[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<CalendarEventLink>(CalendarEventLink.GetAssignedMetaClass(),filters);
        }

        public static CalendarEventLink[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<CalendarEventLink>(CalendarEventLink.GetAssignedMetaClass(),sorting);
        }

        public static CalendarEventLink[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<CalendarEventLink>(CalendarEventLink.GetAssignedMetaClass(),filters, sorting);
        }

        public static CalendarEventLink[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<CalendarEventLink>(CalendarEventLink.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(CalendarEventLink.GetAssignedMetaClass(), filters);
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

		public PrimaryKeyId CalendarId
        {
            get
            {
				return (PrimaryKeyId)base.Properties["CalendarId"].Value;
            }
            
            set
            {
                base.Properties["CalendarId"].Value = value;
            }
            
        }
        
        public System.String Event
        {
            get
            {
                return (System.String)base.Properties["Event"].Value;
            }
            
        }

		public PrimaryKeyId EventId
        {
            get
            {
				return (PrimaryKeyId)base.Properties["EventId"].Value;
            }
            
            set
            {
                base.Properties["EventId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
