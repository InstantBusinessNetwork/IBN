
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
    public partial class CustomizationItemArgumentEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName = "CustomizationItemArgument";
        public const string PrimaryKeyName = "PrimaryKeyId";
        #endregion
        
        #region Field's name const
        public const string FieldItem = "Item";
        public const string FieldItemId = "ItemId";
        public const string FieldName = "Name";
        public const string FieldValue = "Value";
        
		#endregion
        
        #region .Ctor
        public CustomizationItemArgumentEntity()
             : base("CustomizationItemArgument")
        {
			InitializeProperties();
        }

        public CustomizationItemArgumentEntity(PrimaryKeyId primaryKeyId)
             : base("CustomizationItemArgument", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public CustomizationItemArgumentEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public CustomizationItemArgumentEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Item", null);
            base.Properties.Add("ItemId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("Value", null);
            
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
                            case "CustomizationItemArgumentId": 
                            case "Item": 
                            case "ItemId": 
                            case "Name": 
                            case "Value": 
                            
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
        
        public System.String Item
        {
            get
            {
                return (System.String)base.Properties["Item"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId ItemId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["ItemId"].Value;
            }
            
            set
            {
                base.Properties["ItemId"].Value = value;
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
        
        public System.String Value
        {
            get
            {
                return (System.String)base.Properties["Value"].Value;
            }
            
            set
            {
                base.Properties["Value"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
