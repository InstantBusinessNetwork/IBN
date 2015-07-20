
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// An auto generated class. Don't modify it manually.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;



namespace Mediachase.IbnNext.TimeTracking
{
    public partial class Principal: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("Principal");
        }
        #endregion
        
        #region .Ctor
        public Principal()
             : base(Principal.GetAssignedMetaClass())
        {
        }

        public Principal(MetaObjectOptions options)
             : base(Principal.GetAssignedMetaClass(), options)
        {
        }
        
        public Principal(PrimaryKeyId primaryKeyId)
             : base(Principal.GetAssignedMetaClass(), primaryKeyId)
        {
        }

		public Principal(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(Principal.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public Principal(CustomTableRow row)
             : base(Principal.GetAssignedMetaClass(), row)
        {
        }
        
        public Principal(CustomTableRow row, MetaObjectOptions options)
             : base(Principal.GetAssignedMetaClass(), row, options)
        {
        }

		public Principal(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public Principal(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
            : base(metaType, row, options)
        {
        }
        #endregion

        #region Extended Properties
        public MetaObjectProperty[] ExtendedProperties
        {
            get
            {
                if(_exProperies==null)
                {
                    List<MetaObjectProperty> retVal = new List<MetaObjectProperty>();
                    
                    foreach(MetaObjectProperty property in base.Properties)
                    {
                        switch(property.Name)
                        {
                            case "Card": 
                            case "Created": 
                            case "CreatorId": 
                            case "EMail": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "PrincipalId": 
                            
                                 break;
                            default:
                                 retVal.Add(property);    
                                 break;
                        }
                    }
                    _exProperies = retVal.ToArray();
                }
                
                return _exProperies;
            }
        }
        #endregion
        
        #region Static Methods (List + GetTotalCount)
        public static Principal[] List()
        {
            return MetaObject.List<Principal>(Principal.GetAssignedMetaClass());
        }
        
        public static Principal[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<Principal>(Principal.GetAssignedMetaClass(),filters);
        }

        public static Principal[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<Principal>(Principal.GetAssignedMetaClass(),sorting);
        }

        public static Principal[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<Principal>(Principal.GetAssignedMetaClass(),filters, sorting);
        }

        public static Principal[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<Principal>(Principal.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(Principal.GetAssignedMetaClass(), filters);
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
        
        public System.String EMail
        {
            get
            {
                return (System.String)base.Properties["EMail"].Value;
            }
            
            set
            {
                base.Properties["EMail"].Value = value;
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
        
        #endregion
        
        
    }
}
