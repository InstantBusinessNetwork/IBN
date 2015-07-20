
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
    public partial class DirectoryOrganizationalUnitEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "DirectoryOrganizationalUnit";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldHasChildren = "HasChildren";
        public const string FieldIcon = "Icon";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldName = "Name";
        public const string FieldOrganizationalUnitScopeId = "OrganizationalUnitScopeId";
        public const string FieldOutlineLevel = "OutlineLevel";
        public const string FieldOutlineNumber = "OutlineNumber";
        public const string FieldParent = "Parent";
        public const string FieldParentId = "ParentId";
        public const string FieldPath = "Path";
        public const string FieldScopeIndex = "ScopeIndex";
        
		#endregion
        
        #region .Ctor
        public DirectoryOrganizationalUnitEntity()
             : base("DirectoryOrganizationalUnit")
        {
			InitializeProperties();
        }

        public DirectoryOrganizationalUnitEntity(PrimaryKeyId primaryKeyId)
             : base("DirectoryOrganizationalUnit", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DirectoryOrganizationalUnitEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DirectoryOrganizationalUnitEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("HasChildren", null);
            base.Properties.Add("Icon", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("OrganizationalUnitScopeId", null);
            base.Properties.Add("OutlineLevel", null);
            base.Properties.Add("OutlineNumber", null);
            base.Properties.Add("Parent", null);
            base.Properties.Add("ParentId", null);
            base.Properties.Add("Path", null);
            base.Properties.Add("ScopeIndex", null);
            
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
                            case "DirectoryOrganizationalUnitId": 
                            case "HasChildren": 
                            case "Icon": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "OrganizationalUnitScopeId": 
                            case "OutlineLevel": 
                            case "OutlineNumber": 
                            case "Parent": 
                            case "ParentId": 
                            case "Path": 
                            case "ScopeIndex": 
                            
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
        
        public System.Boolean HasChildren
        {
            get
            {
                return (System.Boolean)base.Properties["HasChildren"].Value;
            }
            
            set
            {
                base.Properties["HasChildren"].Value = value;
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
        
        public System.Guid OrganizationalUnitScopeId
        {
            get
            {
                return (System.Guid)base.Properties["OrganizationalUnitScopeId"].Value;
            }
            
            set
            {
                base.Properties["OrganizationalUnitScopeId"].Value = value;
            }
            
        }
        
        public System.Int32 OutlineLevel
        {
            get
            {
                return (System.Int32)base.Properties["OutlineLevel"].Value;
            }
            
            set
            {
                base.Properties["OutlineLevel"].Value = value;
            }
            
        }
        
        public System.String OutlineNumber
        {
            get
            {
                return (System.String)base.Properties["OutlineNumber"].Value;
            }
            
            set
            {
                base.Properties["OutlineNumber"].Value = value;
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
        
        public System.String Path
        {
            get
            {
                return (System.String)base.Properties["Path"].Value;
            }
            
        }
        
        public Nullable<System.Int32> ScopeIndex
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ScopeIndex"].Value;
            }
            
            set
            {
                base.Properties["ScopeIndex"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
