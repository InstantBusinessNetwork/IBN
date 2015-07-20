
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
    public partial class EmailEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "Email";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldFrom = "From";
        public const string FieldHtmlBody = "HtmlBody";
        public const string FieldMessageContext = "MessageContext";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldSubject = "Subject";
        public const string FieldTo = "To";
        
		#endregion
        
        #region .Ctor
        public EmailEntity()
             : base("Email")
        {
			InitializeProperties();
        }

        public EmailEntity(PrimaryKeyId primaryKeyId)
             : base("Email", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public EmailEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public EmailEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("From", null);
            base.Properties.Add("HtmlBody", null);
            base.Properties.Add("MessageContext", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Subject", null);
            base.Properties.Add("To", null);
            
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
                            case "EmailId": 
                            case "From": 
                            case "HtmlBody": 
                            case "MessageContext": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Subject": 
                            case "To": 
                            
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
        
        public System.String From
        {
            get
            {
                return (System.String)base.Properties["From"].Value;
            }
            
            set
            {
                base.Properties["From"].Value = value;
            }
            
        }
        
        public System.String HtmlBody
        {
            get
            {
                return (System.String)base.Properties["HtmlBody"].Value;
            }
            
            set
            {
                base.Properties["HtmlBody"].Value = value;
            }
            
        }
        
        public System.String MessageContext
        {
            get
            {
                return (System.String)base.Properties["MessageContext"].Value;
            }
            
            set
            {
                base.Properties["MessageContext"].Value = value;
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
        
        public System.String To
        {
            get
            {
                return (System.String)base.Properties["To"].Value;
            }
            
            set
            {
                base.Properties["To"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
