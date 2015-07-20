
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
    public partial class CalendarEventResourceEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "CalendarEventResource";
        #endregion
        
        #region Field's name const
        public const string FieldContact = "Contact";
        public const string FieldContactId = "ContactId";
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldEmail = "Email";
        public const string FieldEvent = "Event";
        public const string FieldEventId = "EventId";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldName = "Name";
        public const string FieldOrganization = "Organization";
        public const string FieldOrganizationId = "OrganizationId";
        public const string FieldPrincipal = "Principal";
        public const string FieldPrincipalId = "PrincipalId";
        public const string FieldResourceEventOrganizator = "ResourceEventOrganizator";
        public const string FieldRsvp = "Rsvp";
        public const string FieldStatus = "Status";
        
		#endregion
        
        #region .Ctor
        public CalendarEventResourceEntity()
             : base("CalendarEventResource")
        {
			InitializeProperties();
        }

        public CalendarEventResourceEntity(PrimaryKeyId primaryKeyId)
             : base("CalendarEventResource", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CalendarEventResourceEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CalendarEventResourceEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Contact", null);
            base.Properties.Add("ContactId", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("Email", null);
            base.Properties.Add("Event", null);
            base.Properties.Add("EventId", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("Organization", null);
            base.Properties.Add("OrganizationId", null);
            base.Properties.Add("Principal", null);
            base.Properties.Add("PrincipalId", null);
            base.Properties.Add("ResourceEventOrganizator", null);
            base.Properties.Add("Rsvp", null);
            base.Properties.Add("Status", null);
            
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
                            case "CalendarEventResourceId": 
                            case "Contact": 
                            case "ContactId": 
                            case "Created": 
                            case "CreatorId": 
                            case "Email": 
                            case "Event": 
                            case "EventId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "Organization": 
                            case "OrganizationId": 
                            case "Principal": 
                            case "PrincipalId": 
                            case "ResourceEventOrganizator": 
                            case "Rsvp": 
                            case "Status": 
                            
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
        
        public System.String Contact
        {
            get
            {
                return (System.String)base.Properties["Contact"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ContactId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ContactId"].Value;
            }
            
            set
            {
                base.Properties["ContactId"].Value = value;
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
        
        public System.String Email
        {
            get
            {
                return (System.String)base.Properties["Email"].Value;
            }
            
            set
            {
                base.Properties["Email"].Value = value;
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
        
        public System.String Name
        {
            get
            {
                return (System.String)base.Properties["Name"].Value;
            }
            
            set
            {
                base.Properties["Name"].Value = value;
            }
            
        }
        
        public System.String Organization
        {
            get
            {
                return (System.String)base.Properties["Organization"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> OrganizationId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["OrganizationId"].Value;
            }
            
            set
            {
                base.Properties["OrganizationId"].Value = value;
            }
            
        }
        
        public System.String Principal
        {
            get
            {
                return (System.String)base.Properties["Principal"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> PrincipalId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["PrincipalId"].Value;
            }
            
            set
            {
                base.Properties["PrincipalId"].Value = value;
            }
            
        }
        
        public System.Boolean ResourceEventOrganizator
        {
            get
            {
                return (System.Boolean)base.Properties["ResourceEventOrganizator"].Value;
            }
            
            set
            {
                base.Properties["ResourceEventOrganizator"].Value = value;
            }
            
        }
        
        public System.Boolean Rsvp
        {
            get
            {
                return (System.Boolean)base.Properties["Rsvp"].Value;
            }
            
            set
            {
                base.Properties["Rsvp"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> Status
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Status"].Value;
            }
            
            set
            {
                base.Properties["Status"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
