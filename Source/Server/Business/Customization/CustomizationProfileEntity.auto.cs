
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



namespace Mediachase.Ibn.Business.Customization
{
    public partial class CustomizationProfileEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "CustomizationProfile";
        #endregion
        
        #region Field's name const
        public const string FieldName = "Name";
        public const string FieldWorkspacePersonalization = "WorkspacePersonalization";
        
		#endregion
        
        #region .Ctor
        public CustomizationProfileEntity()
             : base("CustomizationProfile")
        {
			InitializeProperties();
        }

        public CustomizationProfileEntity(PrimaryKeyId primaryKeyId)
             : base("CustomizationProfile", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CustomizationProfileEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CustomizationProfileEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Name", null);
            base.Properties.Add("WorkspacePersonalization", null);
            
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
                            case "CustomizationProfileId": 
                            case "Name": 
                            case "WorkspacePersonalization": 
                            
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
        
        public System.Boolean WorkspacePersonalization
        {
            get
            {
                return (System.Boolean)base.Properties["WorkspacePersonalization"].Value;
            }
            
            set
            {
                base.Properties["WorkspacePersonalization"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
