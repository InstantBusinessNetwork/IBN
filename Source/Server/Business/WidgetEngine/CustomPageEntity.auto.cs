
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
    public partial class CustomPageEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName = "CustomPage";
        public const string PrimaryKeyName = "PrimaryKeyId";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldDescription = "Description";
        public const string FieldIcon = "Icon";
        public const string FieldJsonData = "JsonData";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldProfile = "Profile";
        public const string FieldProfileId = "ProfileId";
        public const string FieldPropertyJsonData = "PropertyJsonData";
        public const string FieldTemplateId = "TemplateId";
        public const string FieldTitle = "Title";
        public const string FieldUid = "Uid";
        public const string FieldUser = "User";
        public const string FieldUserId = "UserId";
        
		#endregion
        
        #region .Ctor
        public CustomPageEntity()
             : base("CustomPage")
        {
			InitializeProperties();
        }

        public CustomPageEntity(PrimaryKeyId primaryKeyId)
             : base("CustomPage", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CustomPageEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CustomPageEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("Icon", null);
            base.Properties.Add("JsonData", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Profile", null);
            base.Properties.Add("ProfileId", null);
            base.Properties.Add("PropertyJsonData", null);
            base.Properties.Add("TemplateId", null);
            base.Properties.Add("Title", null);
            base.Properties.Add("Uid", null);
            base.Properties.Add("User", null);
            base.Properties.Add("UserId", null);
            
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
                            case "CustomPageId": 
                            case "Description": 
                            case "Icon": 
                            case "JsonData": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Profile": 
                            case "ProfileId": 
                            case "PropertyJsonData": 
                            case "TemplateId": 
                            case "Title": 
                            case "Uid": 
                            case "User": 
                            case "UserId": 
                            
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
        
        public Mediachase.Ibn.Data.Meta.FileInfo Icon
        {
            get
            {
                return (Mediachase.Ibn.Data.Meta.FileInfo)base.Properties["Icon"].Value;
            }
            
            set
            {
                base.Properties["Icon"].Value = value;
            }
            
        }
        
        public System.String JsonData
        {
            get
            {
                return (System.String)base.Properties["JsonData"].Value;
            }
            
            set
            {
                base.Properties["JsonData"].Value = value;
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
        
        public System.String Profile
        {
            get
            {
                return (System.String)base.Properties["Profile"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ProfileId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ProfileId"].Value;
            }
            
            set
            {
                base.Properties["ProfileId"].Value = value;
            }
            
        }
        
        public System.String PropertyJsonData
        {
            get
            {
                return (System.String)base.Properties["PropertyJsonData"].Value;
            }
            
            set
            {
                base.Properties["PropertyJsonData"].Value = value;
            }
            
        }
        
        public System.Guid TemplateId
        {
            get
            {
                return (System.Guid)base.Properties["TemplateId"].Value;
            }
            
            set
            {
                base.Properties["TemplateId"].Value = value;
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
        
        public System.Guid Uid
        {
            get
            {
                return (System.Guid)base.Properties["Uid"].Value;
            }
            
            set
            {
                base.Properties["Uid"].Value = value;
            }
            
        }
        
        public System.String User
        {
            get
            {
                return (System.String)base.Properties["User"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> UserId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["UserId"].Value;
            }
            
            set
            {
                base.Properties["UserId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
