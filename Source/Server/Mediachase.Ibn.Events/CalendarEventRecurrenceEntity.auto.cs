
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// An auto generated class. Don't modify it manually.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;



namespace Mediachase.Ibn.Events
{
    public partial class CalendarEventRecurrenceEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "CalendarEventRecurrence";
        #endregion
        
        #region Field's name const
        public const string FieldDayOfMonth = "DayOfMonth";
        public const string FieldDayOfWeekMask = "DayOfWeekMask";
        public const string FieldEvent = "Event";
        public const string FieldEventId = "EventId";
        public const string FieldInstance = "Instance";
        public const string FieldInterval = "Interval";
        public const string FieldMonthOfYear = "MonthOfYear";
        public const string FieldNoEndDate = "NoEndDate";
        public const string FieldOccurrences = "Occurrences";
        public const string FieldPatternEndDate = "PatternEndDate";
        public const string FieldRecurrenceType = "RecurrenceType";
        public const string FieldTitle = "Title";
        
		#endregion
        
        #region .Ctor
        public CalendarEventRecurrenceEntity()
             : base("CalendarEventRecurrence")
        {
			InitializeProperties();
        }

        public CalendarEventRecurrenceEntity(PrimaryKeyId primaryKeyId)
             : base("CalendarEventRecurrence", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CalendarEventRecurrenceEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CalendarEventRecurrenceEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("DayOfMonth", null);
            base.Properties.Add("DayOfWeekMask", null);
            base.Properties.Add("Event", null);
            base.Properties.Add("EventId", null);
            base.Properties.Add("Instance", null);
            base.Properties.Add("Interval", null);
            base.Properties.Add("MonthOfYear", null);
            base.Properties.Add("NoEndDate", null);
            base.Properties.Add("Occurrences", null);
            base.Properties.Add("PatternEndDate", null);
            base.Properties.Add("RecurrenceType", null);
            base.Properties.Add("Title", null);
            
        }
        #endregion

        #region Extended Properties
        public EntityObjectProperty[] ExtendedProperties
        {
            get
            {
                if(_exProperties==null)
                {
                    List<EntityObjectProperty> retVal = new List<EntityObjectProperty>();
                    
                    foreach(EntityObjectProperty property in base.Properties)
                    {
                        switch(property.Name)
                        {
                            case "CalendarEventRecurrenceId": 
                            case "DayOfMonth": 
                            case "DayOfWeekMask": 
                            case "Event": 
                            case "EventId": 
                            case "Instance": 
                            case "Interval": 
                            case "MonthOfYear": 
                            case "NoEndDate": 
                            case "Occurrences": 
                            case "PatternEndDate": 
                            case "RecurrenceType": 
                            case "Title": 
                            
                                 break;
                            default:
                                 retVal.Add(property);    
                                 break;
                        }
                    }
                    _exProperties = retVal.ToArray();
                }
                
                return _exProperties;
            }
            set
            {
                _exProperties = value;
            }
        }
        #endregion
        
        #region Named Properties
        
        public Nullable<System.Int32> DayOfMonth
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["DayOfMonth"].Value;
            }
            
            set
            {
                base.Properties["DayOfMonth"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> DayOfWeekMask
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["DayOfWeekMask"].Value;
            }
            
            set
            {
                base.Properties["DayOfWeekMask"].Value = value;
            }
            
        }
        
        public System.String Event
        {
            get
            {
                return (System.String)base.Properties["Event"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId EventId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["EventId"].Value;
            }
            
            set
            {
                base.Properties["EventId"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> Instance
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Instance"].Value;
            }
            
            set
            {
                base.Properties["Instance"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> Interval
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Interval"].Value;
            }
            
            set
            {
                base.Properties["Interval"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> MonthOfYear
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["MonthOfYear"].Value;
            }
            
            set
            {
                base.Properties["MonthOfYear"].Value = value;
            }
            
        }
        
        public Nullable<System.Boolean> NoEndDate
        {
            get
            {
                return (Nullable<System.Boolean>)base.Properties["NoEndDate"].Value;
            }
            
            set
            {
                base.Properties["NoEndDate"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> Occurrences
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Occurrences"].Value;
            }
            
            set
            {
                base.Properties["Occurrences"].Value = value;
            }
            
        }
        
        public Nullable<System.DateTime> PatternEndDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["PatternEndDate"].Value;
            }
            
            set
            {
                base.Properties["PatternEndDate"].Value = value;
            }
            
        }
        
        public System.Int32 RecurrenceType
        {
            get
            {
                return (System.Int32)base.Properties["RecurrenceType"].Value;
            }
            
            set
            {
                base.Properties["RecurrenceType"].Value = value;
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
