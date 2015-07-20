
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
    public partial class CalendarEventEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "CalendarEvent";
        #endregion
        
        #region Field's name const
        public const string FieldAllDayEvent = "AllDayEvent";
        public const string FieldBody = "Body";
        public const string FieldBusyStatus = "BusyStatus";
        public const string FieldCalendarEventException = "CalendarEventException";
        public const string FieldCalendarEventExceptionId = "CalendarEventExceptionId";
        public const string FieldCategories = "Categories";
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldDeletedException = "DeletedException";
        public const string FieldDuration = "Duration";
        public const string FieldEnd = "End";
        public const string FieldEndTimeZoneOffset = "EndTimeZoneOffset";
        public const string FieldImportance = "Importance";
        public const string FieldIsRecurring = "IsRecurring";
        public const string FieldLocation = "Location";
        public const string FieldMeetingStatus = "MeetingStatus";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldProject = "Project";
        public const string FieldProjectId = "ProjectId";
        public const string FieldSensitivy = "Sensitivy";
        public const string FieldSequence = "Sequence";
        public const string FieldStart = "Start";
        public const string FieldStartTimeZoneOffset = "StartTimeZoneOffset";
        public const string FieldSubject = "Subject";
        
		#endregion
        
        #region .Ctor
        public CalendarEventEntity()
             : base("CalendarEvent")
        {
			InitializeProperties();
        }

        public CalendarEventEntity(PrimaryKeyId primaryKeyId)
             : base("CalendarEvent", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CalendarEventEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CalendarEventEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("AllDayEvent", null);
            base.Properties.Add("Body", null);
            base.Properties.Add("BusyStatus", null);
            base.Properties.Add("CalendarEventException", null);
            base.Properties.Add("CalendarEventExceptionId", null);
            base.Properties.Add("Categories", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("DeletedException", null);
            base.Properties.Add("Duration", null);
            base.Properties.Add("End", null);
            base.Properties.Add("EndTimeZoneOffset", null);
            base.Properties.Add("Importance", null);
            base.Properties.Add("IsRecurring", null);
            base.Properties.Add("Location", null);
            base.Properties.Add("MeetingStatus", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Project", null);
            base.Properties.Add("ProjectId", null);
            base.Properties.Add("Sensitivy", null);
            base.Properties.Add("Sequence", null);
            base.Properties.Add("Start", null);
            base.Properties.Add("StartTimeZoneOffset", null);
            base.Properties.Add("Subject", null);
            
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
                            case "AllDayEvent": 
                            case "Body": 
                            case "BusyStatus": 
                            case "CalendarEventException": 
                            case "CalendarEventExceptionId": 
                            case "CalendarEventId": 
                            case "Categories": 
                            case "Created": 
                            case "CreatorId": 
                            case "DeletedException": 
                            case "Duration": 
                            case "End": 
                            case "EndTimeZoneOffset": 
                            case "Importance": 
                            case "IsRecurring": 
                            case "Location": 
                            case "MeetingStatus": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Project": 
                            case "ProjectId": 
                            case "Sensitivy": 
                            case "Sequence": 
                            case "Start": 
                            case "StartTimeZoneOffset": 
                            case "Subject": 
                            
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
        
        public System.Boolean AllDayEvent
        {
            get
            {
                return (System.Boolean)base.Properties["AllDayEvent"].Value;
            }
            
            set
            {
                base.Properties["AllDayEvent"].Value = value;
            }
            
        }
        
        public System.String Body
        {
            get
            {
                return (System.String)base.Properties["Body"].Value;
            }
            
            set
            {
                base.Properties["Body"].Value = value;
            }
            
        }
        
        public System.Boolean BusyStatus
        {
            get
            {
                return (System.Boolean)base.Properties["BusyStatus"].Value;
            }
            
            set
            {
                base.Properties["BusyStatus"].Value = value;
            }
            
        }
        
        public System.String CalendarEventException
        {
            get
            {
                return (System.String)base.Properties["CalendarEventException"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> CalendarEventExceptionId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["CalendarEventExceptionId"].Value;
            }
            
            set
            {
                base.Properties["CalendarEventExceptionId"].Value = value;
            }
            
        }
        
        public System.String Categories
        {
            get
            {
                return (System.String)base.Properties["Categories"].Value;
            }
            
            set
            {
                base.Properties["Categories"].Value = value;
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
        
        public System.Boolean DeletedException
        {
            get
            {
                return (System.Boolean)base.Properties["DeletedException"].Value;
            }
            
            set
            {
                base.Properties["DeletedException"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> Duration
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Duration"].Value;
            }
            
            set
            {
                base.Properties["Duration"].Value = value;
            }
            
        }
        
        public System.DateTime End
        {
            get
            {
                return (System.DateTime)base.Properties["End"].Value;
            }
            
            set
            {
                base.Properties["End"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> EndTimeZoneOffset
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["EndTimeZoneOffset"].Value;
            }
            
            set
            {
                base.Properties["EndTimeZoneOffset"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> Importance
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Importance"].Value;
            }
            
            set
            {
                base.Properties["Importance"].Value = value;
            }
            
        }
        
        public System.Boolean IsRecurring
        {
            get
            {
                return (System.Boolean)base.Properties["IsRecurring"].Value;
            }
            
            set
            {
                base.Properties["IsRecurring"].Value = value;
            }
            
        }
        
        public System.String Location
        {
            get
            {
                return (System.String)base.Properties["Location"].Value;
            }
            
            set
            {
                base.Properties["Location"].Value = value;
            }
            
        }
        
        public System.Int32 MeetingStatus
        {
            get
            {
                return (System.Int32)base.Properties["MeetingStatus"].Value;
            }
            
            set
            {
                base.Properties["MeetingStatus"].Value = value;
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
        
        public System.Int32 Sensitivy
        {
            get
            {
                return (System.Int32)base.Properties["Sensitivy"].Value;
            }
            
            set
            {
                base.Properties["Sensitivy"].Value = value;
            }
            
        }
        
        public System.Int32 Sequence
        {
            get
            {
                return (System.Int32)base.Properties["Sequence"].Value;
            }
            
            set
            {
                base.Properties["Sequence"].Value = value;
            }
            
        }
        
        public System.DateTime Start
        {
            get
            {
                return (System.DateTime)base.Properties["Start"].Value;
            }
            
            set
            {
                base.Properties["Start"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> StartTimeZoneOffset
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["StartTimeZoneOffset"].Value;
            }
            
            set
            {
                base.Properties["StartTimeZoneOffset"].Value = value;
            }
            
        }
        
        public System.String Subject
        {
            get
            {
                return (System.String)base.Properties["Subject"].Value;
            }
            
            set
            {
                base.Properties["Subject"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
