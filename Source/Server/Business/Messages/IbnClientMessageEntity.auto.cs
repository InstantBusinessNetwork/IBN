
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
    public partial class IbnClientMessageEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "IbnClientMessage";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldFrom = "From";
        public const string FieldFromId = "FromId";
        public const string FieldHtmlBody = "HtmlBody";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldSubject = "Subject";
        public const string FieldTo = "To";
        public const string FieldToId = "ToId";
        
		#endregion
        
        #region .Ctor
        public IbnClientMessageEntity()
             : base("IbnClientMessage")
        {
			InitializeProperties();
        }

        public IbnClientMessageEntity(PrimaryKeyId primaryKeyId)
             : base("IbnClientMessage", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public IbnClientMessageEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public IbnClientMessageEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("FromId", null);
            base.Properties.Add("HtmlBody", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Subject", null);
            base.Properties.Add("To", null);
            base.Properties.Add("ToId", null);
            
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
                            case "From": 
                            case "FromId": 
                            case "HtmlBody": 
                            case "IbnClientMessageId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Subject": 
                            case "To": 
                            case "ToId": 
                            
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
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId FromId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["FromId"].Value;
            }
            
            set
            {
                base.Properties["FromId"].Value = value;
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
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId ToId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["ToId"].Value;
            }
            
            set
            {
                base.Properties["ToId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
