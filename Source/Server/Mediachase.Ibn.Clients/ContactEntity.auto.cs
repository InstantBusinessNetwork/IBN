
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



namespace Mediachase.Ibn.Clients
{
    public partial class ContactEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperies = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "Contact";
        }
        #endregion
        
        #region .Ctor
        public ContactEntity()
             : base("Contact")
        {
			InitializeProperties();
        }

        public ContactEntity(PrimaryKeyId primaryKeyId)
             : base("Contact", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public ContactEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public ContactEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Address", null);
            base.Properties.Add("BirthDate", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("Description", null);
            base.Properties.Add("EMailAddress1", null);
            base.Properties.Add("EMailAddress2", null);
            base.Properties.Add("EMailAddress3", null);
            base.Properties.Add("Fax", null);
            base.Properties.Add("FirstName", null);
            base.Properties.Add("FullName", null);
            base.Properties.Add("Gender", null);
            base.Properties.Add("JobTitle", null);
            base.Properties.Add("LastName", null);
            base.Properties.Add("MiddleName", null);
            base.Properties.Add("MobilePhone", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("NickName", null);
            base.Properties.Add("Organization", null);
            base.Properties.Add("OrganizationId", null);
            base.Properties.Add("OrganizationUnit", null);
            base.Properties.Add("Role", null);
            base.Properties.Add("Telephone1", null);
            base.Properties.Add("Telephone2", null);
            base.Properties.Add("Telephone3", null);
            base.Properties.Add("WebSiteUrl", null);
            
        }
        #endregion

        #region Extended Properties
        public EntityObjectProperty[] ExtendedProperties
        {
            get
            {
                if(_exProperies==null)
                {
                    List<EntityObjectProperty> retVal = new List<EntityObjectProperty>();
                    
                    foreach(EntityObjectProperty property in base.Properties)
                    {
                        switch(property.Name)
                        {
                            case "Address": 
                            case "BirthDate": 
                            case "ContactId": 
                            case "Created": 
                            case "CreatorId": 
                            case "Description": 
                            case "EMailAddress1": 
                            case "EMailAddress2": 
                            case "EMailAddress3": 
                            case "Fax": 
                            case "FirstName": 
                            case "FullName": 
                            case "Gender": 
                            case "JobTitle": 
                            case "LastName": 
                            case "MiddleName": 
                            case "MobilePhone": 
                            case "Modified": 
                            case "ModifierId": 
                            case "NickName": 
                            case "Organization": 
                            case "OrganizationId": 
                            case "OrganizationUnit": 
                            case "Role": 
                            case "Telephone1": 
                            case "Telephone2": 
                            case "Telephone3": 
                            case "WebSiteUrl": 
                            
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
            set
            {
				_exProperies = value;
            }
        }
        #endregion
        
        #region Named Properties
        
        public EntityObject Address
        {
            get
            {
				return (EntityObject)base.Properties["Address"].Value;
            }
            
            set
            {
                base.Properties["Address"].Value = value;
            }
            
        }
        
        public Nullable<System.DateTime> BirthDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["BirthDate"].Value;
            }
            
            set
            {
                base.Properties["BirthDate"].Value = value;
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
        
        public System.String EMailAddress2
        {
            get
            {
                return (System.String)base.Properties["EMailAddress2"].Value;
            }
            
            set
            {
                base.Properties["EMailAddress2"].Value = value;
            }
            
        }
        
        public System.String EMailAddress3
        {
            get
            {
                return (System.String)base.Properties["EMailAddress3"].Value;
            }
            
            set
            {
                base.Properties["EMailAddress3"].Value = value;
            }
            
        }
        
        public System.String Fax
        {
            get
            {
                return (System.String)base.Properties["Fax"].Value;
            }
            
            set
            {
                base.Properties["Fax"].Value = value;
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
        
        public Nullable<System.Int32> Gender
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["Gender"].Value;
            }
            
            set
            {
                base.Properties["Gender"].Value = value;
            }
            
        }
        
        public System.String JobTitle
        {
            get
            {
                return (System.String)base.Properties["JobTitle"].Value;
            }
            
            set
            {
                base.Properties["JobTitle"].Value = value;
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
        
        public System.String MobilePhone
        {
            get
            {
                return (System.String)base.Properties["MobilePhone"].Value;
            }
            
            set
            {
                base.Properties["MobilePhone"].Value = value;
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
        
        public System.String NickName
        {
            get
            {
                return (System.String)base.Properties["NickName"].Value;
            }
            
            set
            {
                base.Properties["NickName"].Value = value;
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
        
        public System.String OrganizationUnit
        {
            get
            {
                return (System.String)base.Properties["OrganizationUnit"].Value;
            }
            
            set
            {
                base.Properties["OrganizationUnit"].Value = value;
            }
            
        }
        
        public System.String Role
        {
            get
            {
                return (System.String)base.Properties["Role"].Value;
            }
            
            set
            {
                base.Properties["Role"].Value = value;
            }
            
        }
        
        public System.String Telephone1
        {
            get
            {
                return (System.String)base.Properties["Telephone1"].Value;
            }
            
            set
            {
                base.Properties["Telephone1"].Value = value;
            }
            
        }
        
        public System.String Telephone2
        {
            get
            {
                return (System.String)base.Properties["Telephone2"].Value;
            }
            
            set
            {
                base.Properties["Telephone2"].Value = value;
            }
            
        }
        
        public System.String Telephone3
        {
            get
            {
                return (System.String)base.Properties["Telephone3"].Value;
            }
            
            set
            {
                base.Properties["Telephone3"].Value = value;
            }
            
        }
        
        public System.String WebSiteUrl
        {
            get
            {
                return (System.String)base.Properties["WebSiteUrl"].Value;
            }
            
            set
            {
                base.Properties["WebSiteUrl"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
