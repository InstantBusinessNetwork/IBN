
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
    public partial class DirectoryPrincipalEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "DirectoryPrincipal";
        #endregion
        
        #region Field's name const
        public const string FieldName = "Name";
        public const string FieldType = "Type";
        
		#endregion
        
        #region .Ctor
        public DirectoryPrincipalEntity()
             : base("DirectoryPrincipal")
        {
			InitializeProperties();
        }

        public DirectoryPrincipalEntity(PrimaryKeyId primaryKeyId)
             : base("DirectoryPrincipal", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public DirectoryPrincipalEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public DirectoryPrincipalEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Name", null);
            base.Properties.Add("Type", null);
            
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
                            case "DirectoryPrincipalId": 
                            case "Name": 
                            case "Type": 
                            
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
        
        public System.Int32 Type
        {
            get
            {
                return (System.Int32)base.Properties["Type"].Value;
            }
            
            set
            {
                base.Properties["Type"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
