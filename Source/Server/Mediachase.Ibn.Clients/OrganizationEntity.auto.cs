
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
    public partial class OrganizationEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "Organization";
        }
        #endregion
        
        #region .Ctor
        public OrganizationEntity()
             : base("Organization")
        {
			InitializeProperties();
        }

        public OrganizationEntity(PrimaryKeyId primaryKeyId)
             : base("Organization", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public OrganizationEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public OrganizationEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Address", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("Description", null);
            base.Properties.Add("EMailAddress1", null);
            base.Properties.Add("EMailAddress2", null);
            base.Properties.Add("EMailAddress3", null);
            base.Properties.Add("Fax", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("Parent", null);
            base.Properties.Add("ParentId", null);
            base.Properties.Add("PrimaryContact", null);
            base.Properties.Add("PrimaryContactId", null);
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
                if(_exProperties==null)
                {
                    List<EntityObjectProperty> retVal = new List<EntityObjectProperty>();
                    
                    foreach(EntityObjectProperty property in base.Properties)
                    {
                        switch(property.Name)
                        {
                            case "Address": 
                            case "Created": 
                            case "CreatorId": 
                            case "Description": 
                            case "EMailAddress1": 
                            case "EMailAddress2": 
                            case "EMailAddress3": 
                            case "Fax": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "OrganizationId": 
                            case "Parent": 
                            case "ParentId": 
                            case "PrimaryContact": 
                            case "PrimaryContactId": 
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
        
        public AddressEntity Address
        {
            get
            {
				return (AddressEntity)base.Properties["Address"].Value;
            }
            
            set
            {
                base.Properties["Address"].Value = value;
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
        
        public System.String Parent
        {
            get
            {
                return (System.String)base.Properties["Parent"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ParentId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ParentId"].Value;
            }
            
            set
            {
                base.Properties["ParentId"].Value = value;
            }
            
        }
        
        public System.String PrimaryContact
        {
            get
            {
                return (System.String)base.Properties["PrimaryContact"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> PrimaryContactId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["PrimaryContactId"].Value;
            }
            
            set
            {
                base.Properties["PrimaryContactId"].Value = value;
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
