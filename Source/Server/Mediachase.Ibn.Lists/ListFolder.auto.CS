
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
    public partial class ListFolder: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("ListFolder");
        }
        #endregion
        
        #region .Ctor
        public ListFolder()
             : base(ListFolder.GetAssignedMetaClass())
        {
        }

        public ListFolder(MetaObjectOptions options)
             : base(ListFolder.GetAssignedMetaClass(), options)
        {
        }
        
        public ListFolder(PrimaryKeyId primaryKeyId)
             : base(ListFolder.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public ListFolder(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(ListFolder.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public ListFolder(CustomTableRow row)
             : base(ListFolder.GetAssignedMetaClass(), row)
        {
        }
        
        public ListFolder(CustomTableRow row, MetaObjectOptions options)
             : base(ListFolder.GetAssignedMetaClass(), row, options)
        {
        }

        public ListFolder(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public ListFolder(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "HasChildren": 
                            case "ListFolderId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "OutlineLevel": 
                            case "OutlineNumber": 
                            case "Owner": 
                            case "Parent": 
                            case "ParentId": 
                            case "Project": 
                            case "ProjectId": 
                            case "ScopeIndex": 
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
        public static ListFolder[] List()
        {
            return MetaObject.List<ListFolder>(ListFolder.GetAssignedMetaClass());
        }
        
        public static ListFolder[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<ListFolder>(ListFolder.GetAssignedMetaClass(),filters);
        }

        public static ListFolder[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<ListFolder>(ListFolder.GetAssignedMetaClass(),sorting);
        }

        public static ListFolder[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<ListFolder>(ListFolder.GetAssignedMetaClass(),filters, sorting);
        }

        public static ListFolder[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<ListFolder>(ListFolder.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(ListFolder.GetAssignedMetaClass(), filters);
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
