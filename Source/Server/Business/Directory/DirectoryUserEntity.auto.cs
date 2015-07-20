
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



namespace Mediachase.Ibn.Business.Directory
{
    public partial class DirectoryUserEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "DirectoryUser";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldEMailAddress1 = "EMailAddress1";
        public const string FieldFirstName = "FirstName";
        public const string FieldFullName = "FullName";
        public const string FieldLastName = "LastName";
        public const string FieldLogin = "Login";
        public const string FieldMiddleName = "MiddleName";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldOrganizationalUnit = "OrganizationalUnit";
        public const string FieldOrganizationalUnitId = "OrganizationalUnitId";
        
		#endregion
        
        #region .Ctor
        public DirectoryUserEntity()
             : base("DirectoryUser")
        {
			InitializeProperties();
        }

        public DirectoryUserEntity(PrimaryKeyId primaryKeyId)
             : base("DirectoryUser", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DirectoryUserEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DirectoryUserEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("EMailAddress1", null);
            base.Properties.Add("FirstName", null);
            base.Properties.Add("FullName", null);
            base.Properties.Add("LastName", null);
            base.Properties.Add("Login", null);
            base.Properties.Add("MiddleName", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("OrganizationalUnit", null);
            base.Properties.Add("OrganizationalUnitId", null);
            
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
                            case "DirectoryUserId": 
                            case "EMailAddress1": 
                            case "FirstName": 
                            case "FullName": 
                            case "LastName": 
                            case "Login": 
                            case "MiddleName": 
                            case "Modified": 
                            case "ModifierId": 
                            case "OrganizationalUnit": 
                            case "OrganizationalUnitId": 
                            
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
        
        public System.String EMailAddress1
        {
            get
            {
                return (System.String)base.Properties["EMailAddress1"].Value;
            }
            
            set
            {
                base.Properties["EMailAddress1"].Value = value;
            }
            
        }
        
        public System.String FirstName
        {
            get
            {
                return (System.String)base.Properties["FirstName"].Value;
            }
            
            set
            {
                base.Properties["FirstName"].Value = value;
            }
            
        }
        
        public System.String FullName
        {
            get
            {
                return (System.String)base.Properties["FullName"].Value;
            }
            
            set
            {
                base.Properties["FullName"].Value = value;
            }
            
        }
        
        public System.String LastName
        {
            get
            {
                return (System.String)base.Properties["LastName"].Value;
            }
            
            set
            {
                base.Properties["LastName"].Value = value;
            }
            
        }
        
        public System.String Login
        {
            get
            {
                return (System.String)base.Properties["Login"].Value;
            }
            
            set
            {
                base.Properties["Login"].Value = value;
            }
            
        }
        
        public System.String MiddleName
        {
            get
            {
                return (System.String)base.Properties["MiddleName"].Value;
            }
            
            set
            {
                base.Properties["MiddleName"].Value = value;
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
        
        public System.String OrganizationalUnit
        {
            get
            {
                return (System.String)base.Properties["OrganizationalUnit"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId OrganizationalUnitId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["OrganizationalUnitId"].Value;
            }
            
            set
            {
                base.Properties["OrganizationalUnitId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
