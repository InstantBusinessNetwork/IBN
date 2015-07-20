
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
    public partial class ListInfo: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("ListInfo");
        }
        #endregion
        
        #region .Ctor
        public ListInfo()
             : base(ListInfo.GetAssignedMetaClass())
        {
        }

        public ListInfo(MetaObjectOptions options)
             : base(ListInfo.GetAssignedMetaClass(), options)
        {
        }
        
        public ListInfo(PrimaryKeyId primaryKeyId)
             : base(ListInfo.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public ListInfo(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(ListInfo.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public ListInfo(CustomTableRow row)
             : base(ListInfo.GetAssignedMetaClass(), row)
        {
        }
        
        public ListInfo(CustomTableRow row, MetaObjectOptions options)
             : base(ListInfo.GetAssignedMetaClass(), row, options)
        {
        }

        public ListInfo(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public ListInfo(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "Created": 
                            case "CreatorId": 
                            case "Description": 
                            case "Folder": 
                            case "FolderId": 
                            case "IsTemplate": 
                            case "ListInfoId": 
                            case "ListType": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Owner": 
                            case "Project": 
                            case "ProjectId": 
                            case "Status": 
                            case "Title": 
                            
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
        public static ListInfo[] List()
        {
            return MetaObject.List<ListInfo>(ListInfo.GetAssignedMetaClass());
        }
        
        public static ListInfo[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<ListInfo>(ListInfo.GetAssignedMetaClass(),filters);
        }

        public static ListInfo[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<ListInfo>(ListInfo.GetAssignedMetaClass(),sorting);
        }

        public static ListInfo[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<ListInfo>(ListInfo.GetAssignedMetaClass(),filters, sorting);
        }

        public static ListInfo[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<ListInfo>(ListInfo.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(ListInfo.GetAssignedMetaClass(), filters);
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
        
        public System.String Description
        {
            get
            {
                return (System.String)base.Properties["Description"].Value;
            }
            
            set
            {
                base.Properties["Description"].Value = value;
            }
            
        }
        
        public System.String Folder
        {
            get
            {
                return (System.String)base.Properties["Folder"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> FolderId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["FolderId"].Value;
            }
            
            set
            {
                base.Properties["FolderId"].Value = value;
            }
            
        }
        
        public System.Boolean IsTemplate
        {
            get
            {
                return (System.Boolean)base.Properties["IsTemplate"].Value;
            }
            
            set
            {
                base.Properties["IsTemplate"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ListType
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ListType"].Value;
            }
            
            set
            {
                base.Properties["ListType"].Value = value;
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
        
        public System.Object Owner
        {
            get
            {
                return (System.Object)base.Properties["Owner"].Value;
            }
            
            set
            {
                base.Properties["Owner"].Value = value;
            }
            
        }
        
        public System.String Project
        {
            get
            {
                return (System.String)base.Properties["Project"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ProjectId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ProjectId"].Value;
            }
            
            set
            {
                base.Properties["ProjectId"].Value = value;
            }
            
        }
        
        public System.Int32 Status
        {
            get
            {
                return (System.Int32)base.Properties["Status"].Value;
            }
            
            set
            {
                base.Properties["Status"].Value = value;
            }
            
        }
        
        public System.String Title
        {
            get
            {
                return (System.String)base.Properties["Title"].Value;
            }
            
            set
            {
                base.Properties["Title"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
