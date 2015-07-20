
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



namespace Mediachase.Ibn.Calendar
{
    public partial class CalendarFolderLink: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("CalendarFolderLink");
        }
        #endregion
        
        #region .Ctor
        public CalendarFolderLink()
             : base(CalendarFolderLink.GetAssignedMetaClass())
        {
        }

        public CalendarFolderLink(MetaObjectOptions options)
             : base(CalendarFolderLink.GetAssignedMetaClass(), options)
        {
        }
        
        public CalendarFolderLink(int primaryKeyId)
             : base(CalendarFolderLink.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public CalendarFolderLink(int primaryKeyId, MetaObjectOptions options)
             : base(CalendarFolderLink.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public CalendarFolderLink(CustomTableRow row)
             : base(CalendarFolderLink.GetAssignedMetaClass(), row)
        {
        }
        
        public CalendarFolderLink(CustomTableRow row, MetaObjectOptions options)
             : base(CalendarFolderLink.GetAssignedMetaClass(), row, options)
        {
        }

        public CalendarFolderLink(MetaClass metaType, int primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public CalendarFolderLink(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "Calendar": 
                            case "CalendarFolderLinkId": 
                            case "CalendarId": 
                            case "Folder": 
                            case "FolderId": 
                            
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
        public static CalendarFolderLink[] List()
        {
            return MetaObject.List<CalendarFolderLink>(CalendarFolderLink.GetAssignedMetaClass());
        }
        
        public static CalendarFolderLink[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<CalendarFolderLink>(CalendarFolderLink.GetAssignedMetaClass(),filters);
        }

        public static CalendarFolderLink[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<CalendarFolderLink>(CalendarFolderLink.GetAssignedMetaClass(),sorting);
        }

        public static CalendarFolderLink[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<CalendarFolderLink>(CalendarFolderLink.GetAssignedMetaClass(),filters, sorting);
        }

        public static CalendarFolderLink[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<CalendarFolderLink>(CalendarFolderLink.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(CalendarFolderLink.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.String Calendar
        {
            get
            {
                return (System.String)base.Properties["Calendar"].Value;
            }
            
        }

		public PrimaryKeyId CalendarId
        {
            get
            {
				return (PrimaryKeyId)base.Properties["CalendarId"].Value;
            }
            
            set
            {
                base.Properties["CalendarId"].Value = value;
            }
            
        }
        
        public System.String Folder
        {
            get
            {
                return (System.String)base.Properties["Folder"].Value;
            }
            
        }

		public PrimaryKeyId FolderId
        {
            get
            {
				return (PrimaryKeyId)base.Properties["FolderId"].Value;
            }
            
            set
            {
                base.Properties["FolderId"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
