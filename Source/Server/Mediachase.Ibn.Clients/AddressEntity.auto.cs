
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
    public partial class AddressEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperies = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "Address";
        }
        #endregion
        
        #region .Ctor
        public AddressEntity()
             : base("Address")
        {
			InitializeProperties();
        }

        public AddressEntity(PrimaryKeyId primaryKeyId)
             : base("Address", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public AddressEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public AddressEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("AddressType", null);
            base.Properties.Add("City", null);
            base.Properties.Add("Contact", null);
            base.Properties.Add("ContactId", null);
            base.Properties.Add("Country", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("Fax", null);
            base.Properties.Add("IsDefaultContactElement", null);
            base.Properties.Add("IsDefaultOrganizationElement", null);
            base.Properties.Add("Line1", null);
            base.Properties.Add("Line2", null);
            base.Properties.Add("Line3", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("Organization", null);
            base.Properties.Add("OrganizationId", null);
            base.Properties.Add("PostalCode", null);
            base.Properties.Add("PostOfficeBox", null);
            base.Properties.Add("Region", null);
            base.Properties.Add("Telephone1", null);
            base.Properties.Add("Telephone2", null);
            base.Properties.Add("Telephone3", null);
            
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
                            case "AddressId": 
                            case "AddressType": 
                            case "City": 
                            case "Contact": 
                            case "ContactId": 
                            case "Country": 
                            case "Created": 
                            case "CreatorId": 
                            case "Fax": 
                            case "IsDefaultContactElement": 
                            case "IsDefaultOrganizationElement": 
                            case "Line1": 
                            case "Line2": 
                            case "Line3": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "Organization": 
                            case "OrganizationId": 
                            case "PostalCode": 
                            case "PostOfficeBox": 
                            case "Region": 
                            case "Telephone1": 
                            case "Telephone2": 
                            case "Telephone3": 
                            
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
        
        public Nullable<System.Int32> AddressType
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["AddressType"].Value;
            }
            
            set
            {
                base.Properties["AddressType"].Value = value;
            }
            
        }
        
        public System.String City
        {
            get
            {
                return (System.String)base.Properties["City"].Value;
            }
            
            set
            {
                base.Properties["City"].Value = value;
            }
            
        }
        
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
        
        public System.String Country
        {
            get
            {
                return (System.String)base.Properties["Country"].Value;
            }
            
            set
            {
                base.Properties["Country"].Value = value;
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
        
        public System.Boolean IsDefaultContactElement
        {
            get
            {
                return (System.Boolean)base.Properties["IsDefaultContactElement"].Value;
            }
            
            set
            {
                base.Properties["IsDefaultContactElement"].Value = value;
            }
            
        }
        
        public System.Boolean IsDefaultOrganizationElement
        {
            get
            {
                return (System.Boolean)base.Properties["IsDefaultOrganizationElement"].Value;
            }
            
            set
            {
                base.Properties["IsDefaultOrganizationElement"].Value = value;
            }
            
        }
        
        public System.String Line1
        {
            get
            {
                return (System.String)base.Properties["Line1"].Value;
            }
            
            set
            {
                base.Properties["Line1"].Value = value;
            }
            
        }
        
        public System.String Line2
        {
            get
            {
                return (System.String)base.Properties["Line2"].Value;
            }
            
            set
            {
                base.Properties["Line2"].Value = value;
            }
            
        }
        
        public System.String Line3
        {
            get
            {
                return (System.String)base.Properties["Line3"].Value;
            }
            
            set
            {
                base.Properties["Line3"].Value = value;
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
        
        public System.String PostalCode
        {
            get
            {
                return (System.String)base.Properties["PostalCode"].Value;
            }
            
            set
            {
                base.Properties["PostalCode"].Value = value;
            }
            
        }
        
        public System.String PostOfficeBox
        {
            get
            {
                return (System.String)base.Properties["PostOfficeBox"].Value;
            }
            
            set
            {
                base.Properties["PostOfficeBox"].Value = value;
            }
            
        }
        
        public System.String Region
        {
            get
            {
                return (System.String)base.Properties["Region"].Value;
            }
            
            set
            {
                base.Properties["Region"].Value = value;
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
        
        #endregion
        
        
    }
}
