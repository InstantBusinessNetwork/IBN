
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



namespace Mediachase.Ibn.Business.Messages
{
    public partial class OutgoingMessageQueueEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "OutgoingMessageQueue";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldDeliveryAttempts = "DeliveryAttempts";
        public const string FieldEmail = "Email";
        public const string FieldEmailId = "EmailId";
        public const string FieldError = "Error";
        public const string FieldIbnClientMessage = "IbnClientMessage";
        public const string FieldIbnClientMessageId = "IbnClientMessageId";
        public const string FieldIsDelivered = "IsDelivered";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldSource = "Source";
        
		#endregion
        
        #region .Ctor
        public OutgoingMessageQueueEntity()
             : base("OutgoingMessageQueue")
        {
			InitializeProperties();
        }

        public OutgoingMessageQueueEntity(PrimaryKeyId primaryKeyId)
             : base("OutgoingMessageQueue", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public OutgoingMessageQueueEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public OutgoingMessageQueueEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("DeliveryAttempts", null);
            base.Properties.Add("Email", null);
            base.Properties.Add("EmailId", null);
            base.Properties.Add("Error", null);
            base.Properties.Add("IbnClientMessage", null);
            base.Properties.Add("IbnClientMessageId", null);
            base.Properties.Add("IsDelivered", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Source", null);
            
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
                            case "Created": 
                            case "CreatorId": 
                            case "DeliveryAttempts": 
                            case "Email": 
                            case "EmailId": 
                            case "Error": 
                            case "IbnClientMessage": 
                            case "IbnClientMessageId": 
                            case "IsDelivered": 
                            case "Modified": 
                            case "ModifierId": 
                            case "OutgoingMessageQueueId": 
                            case "Source": 
                            
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
        
        public System.Int32 DeliveryAttempts
        {
            get
            {
                return (System.Int32)base.Properties["DeliveryAttempts"].Value;
            }
            
            set
            {
                base.Properties["DeliveryAttempts"].Value = value;
            }
            
        }
        
        public System.String Email
        {
            get
            {
                return (System.String)base.Properties["Email"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> EmailId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["EmailId"].Value;
            }
            
            set
            {
                base.Properties["EmailId"].Value = value;
            }
            
        }
        
        public System.String Error
        {
            get
            {
                return (System.String)base.Properties["Error"].Value;
            }
            
            set
            {
                base.Properties["Error"].Value = value;
            }
            
        }
        
        public System.String IbnClientMessage
        {
            get
            {
                return (System.String)base.Properties["IbnClientMessage"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> IbnClientMessageId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["IbnClientMessageId"].Value;
            }
            
            set
            {
                base.Properties["IbnClientMessageId"].Value = value;
            }
            
        }
        
        public System.Boolean IsDelivered
        {
            get
            {
                return (System.Boolean)base.Properties["IsDelivered"].Value;
            }
            
            set
            {
                base.Properties["IsDelivered"].Value = value;
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
        
        public System.String Source
        {
            get
            {
                return (System.String)base.Properties["Source"].Value;
            }
            
            set
            {
                base.Properties["Source"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
