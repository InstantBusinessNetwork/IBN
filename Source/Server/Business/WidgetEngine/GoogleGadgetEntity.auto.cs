
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



namespace Mediachase.IBN.Business.WidgetEngine
{
    public partial class GoogleGadgetEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "GoogleGadget";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldDescription = "Description";
        public const string FieldLink = "Link";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldTitle = "Title";
        
		#endregion
        
        #region .Ctor
        public GoogleGadgetEntity()
             : base("GoogleGadget")
        {
			InitializeProperties();
        }

        public GoogleGadgetEntity(PrimaryKeyId primaryKeyId)
             : base("GoogleGadget", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public GoogleGadgetEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public GoogleGadgetEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("Description", null);
            base.Properties.Add("Link", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
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
                            case "Created": 
                            case "CreatorId": 
                            case "Description": 
                            case "GoogleGadgetId": 
                            case "Link": 
                            case "Modified": 
                            case "ModifierId": 
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
        
        public System.String Link
        {
            get
            {
                return (System.String)base.Properties["Link"].Value;
            }
            
            set
            {
                base.Properties["Link"].Value = value;
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
