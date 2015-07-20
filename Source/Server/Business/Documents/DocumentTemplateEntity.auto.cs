
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
    public partial class DocumentTemplateEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "DocumentTemplate";
        }
        #endregion
        
        #region .Ctor
        public DocumentTemplateEntity()
             : base("DocumentTemplate")
        {
			InitializeProperties();
        }

        public DocumentTemplateEntity(PrimaryKeyId primaryKeyId)
             : base("DocumentTemplate", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DocumentTemplateEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DocumentTemplateEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("DocumentType", null);
            base.Properties.Add("DocumentTypeId", null);
            base.Properties.Add("File", null);
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
                            case "DocumentTemplateId": 
                            case "DocumentType": 
                            case "DocumentTypeId": 
                            case "File": 
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
        
        public System.String DocumentType
        {
            get
            {
                return (System.String)base.Properties["DocumentType"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId DocumentTypeId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["DocumentTypeId"].Value;
            }
            
            set
            {
                base.Properties["DocumentTypeId"].Value = value;
            }
            
        }
        
        public Mediachase.Ibn.Data.Meta.FileInfo File
        {
            get
            {
                return (Mediachase.Ibn.Data.Meta.FileInfo)base.Properties["File"].Value;
            }
            
            set
            {
                base.Properties["File"].Value = value;
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
