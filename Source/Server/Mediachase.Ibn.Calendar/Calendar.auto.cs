
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
    public partial class Calendar: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("Calendar");
        }
        #endregion
        
        #region .Ctor
        public Calendar()
             : base(Calendar.GetAssignedMetaClass())
        {
        }

        public Calendar(MetaObjectOptions options)
             : base(Calendar.GetAssignedMetaClass(), options)
        {
        }
        
        public Calendar(int primaryKeyId)
             : base(Calendar.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public Calendar(int primaryKeyId, MetaObjectOptions options)
             : base(Calendar.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public Calendar(CustomTableRow row)
             : base(Calendar.GetAssignedMetaClass(), row)
        {
        }
        
        public Calendar(CustomTableRow row, MetaObjectOptions options)
             : base(Calendar.GetAssignedMetaClass(), row, options)
        {
        }

        public Calendar(MetaClass metaType, int primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public Calendar(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "CalendarFolder": 
                            case "CalendarFolderId": 
                            case "Card": 
                            case "Description": 
                            case "Owner": 
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
        public static Calendar[] List()
        {
            return MetaObject.List<Calendar>(Calendar.GetAssignedMetaClass());
        }
        
        public static Calendar[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<Calendar>(Calendar.GetAssignedMetaClass(),filters);
        }

        public static Calendar[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<Calendar>(Calendar.GetAssignedMetaClass(),sorting);
        }

        public static Calendar[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<Calendar>(Calendar.GetAssignedMetaClass(),filters, sorting);
        }

        public static Calendar[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<Calendar>(Calendar.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(Calendar.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.String CalendarFolder
        {
            get
            {
                return (System.String)base.Properties["CalendarFolder"].Value;
            }
            
        }
        
        public Nullable<PrimaryKeyId> CalendarFolderId
        {
            get
            {
				return (Nullable<PrimaryKeyId>)base.Properties["CalendarFolderId"].Value;
            }
            
            set
            {
                base.Properties["CalendarFolderId"].Value = value;
            }
            
        }
        
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
