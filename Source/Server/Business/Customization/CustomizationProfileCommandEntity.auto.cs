
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
    public partial class CustomizationProfileCommandEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName =  "CustomizationProfileCommand";
        #endregion
        
        #region Field's name const
        public const string FieldArgument = "Argument";
        public const string FieldCommandType = "CommandType";
        public const string FieldProfile = "Profile";
        public const string FieldProfileId = "ProfileId";
        public const string FieldStructureType = "StructureType";
        public const string FieldXmlFullId = "XmlFullId";
        
		#endregion
        
        #region .Ctor
        public CustomizationProfileCommandEntity()
             : base("CustomizationProfileCommand")
        {
			InitializeProperties();
        }

        public CustomizationProfileCommandEntity(PrimaryKeyId primaryKeyId)
             : base("CustomizationProfileCommand", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CustomizationProfileCommandEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CustomizationProfileCommandEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Argument", null);
            base.Properties.Add("CommandType", null);
            base.Properties.Add("Profile", null);
            base.Properties.Add("ProfileId", null);
            base.Properties.Add("StructureType", null);
            base.Properties.Add("XmlFullId", null);
            
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
                            case "Argument": 
                            case "CommandType": 
                            case "CustomizationProfileCommandId": 
                            case "Profile": 
                            case "ProfileId": 
                            case "StructureType": 
                            case "XmlFullId": 
                            
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
        
        public System.String Argument
        {
            get
            {
                return (System.String)base.Properties["Argument"].Value;
            }
            
            set
            {
                base.Properties["Argument"].Value = value;
            }
            
        }
        
        public System.Int32 CommandType
        {
            get
            {
                return (System.Int32)base.Properties["CommandType"].Value;
            }
            
            set
            {
                base.Properties["CommandType"].Value = value;
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
        
        public System.Int32 StructureType
        {
            get
            {
                return (System.Int32)base.Properties["StructureType"].Value;
            }
            
            set
            {
                base.Properties["StructureType"].Value = value;
            }
            
        }
        
        public System.String XmlFullId
        {
            get
            {
                return (System.String)base.Properties["XmlFullId"].Value;
            }
            
            set
            {
                base.Properties["XmlFullId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
