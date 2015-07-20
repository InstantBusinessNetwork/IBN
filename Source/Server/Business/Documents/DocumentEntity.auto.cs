
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
    public partial class DocumentEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "Document";
        }
        #endregion
        
        #region .Ctor
        public DocumentEntity()
             : base("Document")
        {
			InitializeProperties();
        }

        public DocumentEntity(PrimaryKeyId primaryKeyId)
             : base("Document", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DocumentEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DocumentEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("ActiveVersion", null);
            base.Properties.Add("ActiveVersionId", null);
            base.Properties.Add("Card", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("Description", null);
            base.Properties.Add("DocumentType", null);
            base.Properties.Add("DocumentTypeId", null);
            base.Properties.Add("Initiator", null);
            base.Properties.Add("InitiatorAuthor", null);
            base.Properties.Add("InitiatorAuthorId", null);
            base.Properties.Add("InitiatorContact", null);
            base.Properties.Add("InitiatorContactId", null);
            base.Properties.Add("InitiatorOrganization", null);
            base.Properties.Add("InitiatorOrganizationId", null);
            base.Properties.Add("IsOriginal", null);
            base.Properties.Add("MasterDocument", null);
            base.Properties.Add("MasterDocumentId", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("RegistrationNumber", null);
            base.Properties.Add("Title", null);
            base.Properties.Add("TotalPageCount", null);
            
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
                            case "ActiveVersion": 
                            case "ActiveVersionId": 
                            case "Card": 
                            case "Created": 
                            case "CreatorId": 
                            case "Description": 
                            case "DocumentId": 
                            case "DocumentType": 
                            case "DocumentTypeId": 
                            case "Initiator": 
                            case "InitiatorAuthor": 
                            case "InitiatorAuthorId": 
                            case "InitiatorContact": 
                            case "InitiatorContactId": 
                            case "InitiatorOrganization": 
                            case "InitiatorOrganizationId": 
                            case "IsOriginal": 
                            case "MasterDocument": 
                            case "MasterDocumentId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "RegistrationNumber": 
                            case "Title": 
                            case "TotalPageCount": 
                            
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
        
        public System.String ActiveVersion
        {
            get
            {
                return (System.String)base.Properties["ActiveVersion"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ActiveVersionId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ActiveVersionId"].Value;
            }
            
            set
            {
                base.Properties["ActiveVersionId"].Value = value;
            }
            
        }
        
        public System.String Card
        {
            get
            {
                return (System.String)base.Properties["Card"].Value;
            }
            
            set
            {
                base.Properties["Card"].Value = value;
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
        
        public Mediachase.Ibn.Data.Meta.MultiReferenceObject Initiator
        {
            get
            {
                return (Mediachase.Ibn.Data.Meta.MultiReferenceObject)base.Properties["Initiator"].Value;
            }
            
            set
            {
                base.Properties["Initiator"].Value = value;
            }
            
        }
        
        public System.String InitiatorAuthor
        {
            get
            {
                return (System.String)base.Properties["InitiatorAuthor"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> InitiatorAuthorId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["InitiatorAuthorId"].Value;
            }
            
            set
            {
                base.Properties["InitiatorAuthorId"].Value = value;
            }
            
        }
        
        public System.String InitiatorContact
        {
            get
            {
                return (System.String)base.Properties["InitiatorContact"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> InitiatorContactId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["InitiatorContactId"].Value;
            }
            
            set
            {
                base.Properties["InitiatorContactId"].Value = value;
            }
            
        }
        
        public System.String InitiatorOrganization
        {
            get
            {
                return (System.String)base.Properties["InitiatorOrganization"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> InitiatorOrganizationId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["InitiatorOrganizationId"].Value;
            }
            
            set
            {
                base.Properties["InitiatorOrganizationId"].Value = value;
            }
            
        }
        
        public System.Boolean IsOriginal
        {
            get
            {
                return (System.Boolean)base.Properties["IsOriginal"].Value;
            }
            
            set
            {
                base.Properties["IsOriginal"].Value = value;
            }
            
        }
        
        public System.String MasterDocument
        {
            get
            {
                return (System.String)base.Properties["MasterDocument"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> MasterDocumentId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["MasterDocumentId"].Value;
            }
            
            set
            {
                base.Properties["MasterDocumentId"].Value = value;
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
        
        public System.String RegistrationNumber
        {
            get
            {
                return (System.String)base.Properties["RegistrationNumber"].Value;
            }
            
            set
            {
                base.Properties["RegistrationNumber"].Value = value;
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
        
        public System.Int32 TotalPageCount
        {
            get
            {
                return (System.Int32)base.Properties["TotalPageCount"].Value;
            }
            
            set
            {
                base.Properties["TotalPageCount"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
