
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



namespace Mediachase.IBN.Business.ReportSystem
{
    public partial class ReportEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties = null;
        
        #region Util
        public static string GetAssignedMetaClassName()
        {
             return "Report";
        }
        #endregion
        
        #region .Ctor
        public ReportEntity()
             : base("Report")
        {
			InitializeProperties();
        }

        public ReportEntity(PrimaryKeyId primaryKeyId)
             : base("Report", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public ReportEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public ReportEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("DefaultFilterSql", null);
            base.Properties.Add("DefaultFilterXml", null);
            base.Properties.Add("Description", null);
            base.Properties.Add("FileName", null);
            base.Properties.Add("FilterControl", null);
            base.Properties.Add("IsCustom", null);
            base.Properties.Add("IsFilterable", null);
            base.Properties.Add("IsPersonal", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Owner", null);
            base.Properties.Add("RdlText", null);
            base.Properties.Add("TimeZoneId", null);
            base.Properties.Add("Title", null);
            
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
                            case "Created": 
                            case "CreatorId": 
                            case "DefaultFilterSql": 
                            case "DefaultFilterXml": 
                            case "Description": 
                            case "FileName": 
                            case "FilterControl": 
                            case "IsCustom": 
                            case "IsFilterable": 
                            case "IsPersonal": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Owner": 
                            case "RdlText": 
                            case "ReportId": 
                            case "TimeZoneId": 
                            case "Title": 
                            
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
        
        public System.String DefaultFilterSql
        {
            get
            {
                return (System.String)base.Properties["DefaultFilterSql"].Value;
            }
            
            set
            {
                base.Properties["DefaultFilterSql"].Value = value;
            }
            
        }
        
        public System.String DefaultFilterXml
        {
            get
            {
                return (System.String)base.Properties["DefaultFilterXml"].Value;
            }
            
            set
            {
                base.Properties["DefaultFilterXml"].Value = value;
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
        
        public System.String FileName
        {
            get
            {
                return (System.String)base.Properties["FileName"].Value;
            }
            
            set
            {
                base.Properties["FileName"].Value = value;
            }
            
        }
        
        public System.String FilterControl
        {
            get
            {
                return (System.String)base.Properties["FilterControl"].Value;
            }
            
            set
            {
                base.Properties["FilterControl"].Value = value;
            }
            
        }
        
        public System.Boolean IsCustom
        {
            get
            {
                return (System.Boolean)base.Properties["IsCustom"].Value;
            }
            
            set
            {
                base.Properties["IsCustom"].Value = value;
            }
            
        }
        
        public System.Boolean IsFilterable
        {
            get
            {
                return (System.Boolean)base.Properties["IsFilterable"].Value;
            }
            
            set
            {
                base.Properties["IsFilterable"].Value = value;
            }
            
        }
        
        public System.Boolean IsPersonal
        {
            get
            {
                return (System.Boolean)base.Properties["IsPersonal"].Value;
            }
            
            set
            {
                base.Properties["IsPersonal"].Value = value;
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
        
        public System.String RdlText
        {
            get
            {
                return (System.String)base.Properties["RdlText"].Value;
            }
            
            set
            {
                base.Properties["RdlText"].Value = value;
            }
            
        }
        
        public System.Int32 TimeZoneId
        {
            get
            {
                return (System.Int32)base.Properties["TimeZoneId"].Value;
            }
            
            set
            {
                base.Properties["TimeZoneId"].Value = value;
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
