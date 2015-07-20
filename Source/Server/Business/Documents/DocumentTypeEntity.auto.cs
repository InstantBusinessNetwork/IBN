
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



namespace Mediachase.IBN.Business.Documents
{
    public partial class DocumentTypeEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "DocumentType";
        }
        #endregion
        
        #region .Ctor
        public DocumentTypeEntity()
             : base("DocumentType")
        {
			InitializeProperties();
        }

        public DocumentTypeEntity(PrimaryKeyId primaryKeyId)
             : base("DocumentType", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DocumentTypeEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DocumentTypeEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("FriendlyName", null);
            base.Properties.Add("Name", null);
            
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
                            case "DocumentTypeId": 
                            case "FriendlyName": 
                            case "Name": 
                            
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
        
        public System.String FriendlyName
        {
            get
            {
                return (System.String)base.Properties["FriendlyName"].Value;
            }
            
            set
            {
                base.Properties["FriendlyName"].Value = value;
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
        
        #endregion
        
        
    }
}
