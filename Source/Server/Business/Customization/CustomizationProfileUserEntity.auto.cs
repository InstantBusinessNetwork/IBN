
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
    public partial class CustomizationProfileUserEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "CustomizationProfileUser";
        #endregion
        
        #region Field's name const
        public const string FieldPrincipal = "Principal";
        public const string FieldPrincipalId = "PrincipalId";
        public const string FieldProfile = "Profile";
        public const string FieldProfileId = "ProfileId";
        
		#endregion
        
        #region .Ctor
        public CustomizationProfileUserEntity()
             : base("CustomizationProfileUser")
        {
			InitializeProperties();
        }

        public CustomizationProfileUserEntity(PrimaryKeyId primaryKeyId)
             : base("CustomizationProfileUser", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CustomizationProfileUserEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CustomizationProfileUserEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Principal", null);
            base.Properties.Add("PrincipalId", null);
            base.Properties.Add("Profile", null);
            base.Properties.Add("ProfileId", null);
            
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
                            case "CustomizationProfileUserId": 
                            case "Principal": 
                            case "PrincipalId": 
                            case "Profile": 
                            case "ProfileId": 
                            
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
        
        public System.String Principal
        {
            get
            {
                return (System.String)base.Properties["Principal"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId PrincipalId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["PrincipalId"].Value;
            }
            
            set
            {
                base.Properties["PrincipalId"].Value = value;
            }
            
        }
        
        public System.String Profile
        {
            get
            {
                return (System.String)base.Properties["Profile"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId ProfileId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["ProfileId"].Value;
            }
            
            set
            {
                base.Properties["ProfileId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
