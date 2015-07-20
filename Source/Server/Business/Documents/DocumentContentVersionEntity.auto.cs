
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
    public partial class DocumentContentVersionEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "DocumentContentVersion";
        }
        #endregion
        
        #region .Ctor
        public DocumentContentVersionEntity()
             : base("DocumentContentVersion")
        {
			InitializeProperties();
        }

        public DocumentContentVersionEntity(PrimaryKeyId primaryKeyId)
             : base("DocumentContentVersion", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DocumentContentVersionEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DocumentContentVersionEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Card", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("File", null);
            base.Properties.Add("Index", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("Note", null);
            base.Properties.Add("OwnerDocument", null);
            base.Properties.Add("OwnerDocumentId", null);
            base.Properties.Add("State", null);
            
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
                            case "Card": 
                            case "Created": 
                            case "CreatorId": 
                            case "DocumentContentVersionId": 
                            case "File": 
                            case "Index": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "Note": 
                            case "OwnerDocument": 
                            case "OwnerDocumentId": 
                            case "State": 
                            
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
        
        public System.Int32 Index
        {
            get
            {
                return (System.Int32)base.Properties["Index"].Value;
            }
            
            set
            {
                base.Properties["Index"].Value = value;
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
        
        public System.String Note
        {
            get
            {
                return (System.String)base.Properties["Note"].Value;
            }
            
            set
            {
                base.Properties["Note"].Value = value;
            }
            
        }
        
        public System.String OwnerDocument
        {
            get
            {
                return (System.String)base.Properties["OwnerDocument"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId OwnerDocumentId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["OwnerDocumentId"].Value;
            }
            
            set
            {
                base.Properties["OwnerDocumentId"].Value = value;
            }
            
        }
        
        public System.Int32 State
        {
            get
            {
                return (System.Int32)base.Properties["State"].Value;
            }
            
            set
            {
                base.Properties["State"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
