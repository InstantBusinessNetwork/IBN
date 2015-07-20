
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



namespace Mediachase.Ibn.Lists
{
    public partial class ListPublication: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("ListPublication");
        }
        #endregion
        
        #region .Ctor
        public ListPublication()
             : base(ListPublication.GetAssignedMetaClass())
        {
        }

        public ListPublication(MetaObjectOptions options)
             : base(ListPublication.GetAssignedMetaClass(), options)
        {
        }
        
        public ListPublication(PrimaryKeyId primaryKeyId)
             : base(ListPublication.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public ListPublication(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(ListPublication.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public ListPublication(CustomTableRow row)
             : base(ListPublication.GetAssignedMetaClass(), row)
        {
        }
        
        public ListPublication(CustomTableRow row, MetaObjectOptions options)
             : base(ListPublication.GetAssignedMetaClass(), row, options)
        {
        }

        public ListPublication(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public ListPublication(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "Folder": 
                            case "FolderId": 
                            case "ListInfo": 
                            case "ListInfoId": 
                            case "ListPublicationId": 
                            
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
        public static ListPublication[] List()
        {
            return MetaObject.List<ListPublication>(ListPublication.GetAssignedMetaClass());
        }
        
        public static ListPublication[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<ListPublication>(ListPublication.GetAssignedMetaClass(),filters);
        }

        public static ListPublication[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<ListPublication>(ListPublication.GetAssignedMetaClass(),sorting);
        }

        public static ListPublication[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<ListPublication>(ListPublication.GetAssignedMetaClass(),filters, sorting);
        }

        public static ListPublication[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<ListPublication>(ListPublication.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(ListPublication.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.String Folder
        {
            get
            {
                return (System.String)base.Properties["Folder"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId FolderId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["FolderId"].Value;
            }
            
            set
            {
                base.Properties["FolderId"].Value = value;
            }
            
        }
        
        public System.String ListInfo
        {
            get
            {
                return (System.String)base.Properties["ListInfo"].Value;
            }
            
        }
        
        public Mediachase.Ibn.Data.PrimaryKeyId ListInfoId
        {
            get
            {
                return (Mediachase.Ibn.Data.PrimaryKeyId)base.Properties["ListInfoId"].Value;
            }
            
            set
            {
                base.Properties["ListInfoId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
