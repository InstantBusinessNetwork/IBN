
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
    public partial class DirectoryWorkGroupEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "DirectoryWorkGroup";
        #endregion
        
        #region Field's name const
        public const string FieldName = "Name";
        public const string FieldOrganizationalUnit = "OrganizationalUnit";
        public const string FieldOrganizationalUnitId = "OrganizationalUnitId";
        
		#endregion
        
        #region .Ctor
        public DirectoryWorkGroupEntity()
             : base("DirectoryWorkGroup")
        {
			InitializeProperties();
        }

        public DirectoryWorkGroupEntity(PrimaryKeyId primaryKeyId)
             : base("DirectoryWorkGroup", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DirectoryWorkGroupEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DirectoryWorkGroupEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Name", null);
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
                            case "DirectoryWorkGroupId": 
                            case "Name": 
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
