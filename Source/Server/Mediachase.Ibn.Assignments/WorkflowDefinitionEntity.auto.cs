
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



namespace Mediachase.Ibn.Assignments
{
    public partial class WorkflowDefinitionEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName = "WorkflowDefinition";
        public const string PrimaryKeyName = "PrimaryKeyId";
        #endregion
        
        #region Field's name const
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldDescription = "Description";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldName = "Name";
        public const string FieldPlanDuration = "PlanDuration";
        public const string FieldPlanFinishDate = "PlanFinishDate";
        public const string FieldPlanFinishTimeType = "PlanFinishTimeType";
        public const string FieldProject = "Project";
        public const string FieldProjectId = "ProjectId";
        public const string FieldSchemaId = "SchemaId";
        public const string FieldSupportedIbnObjectTypes = "SupportedIbnObjectTypes";
        public const string FieldTemplateGroup = "TemplateGroup";
        public const string FieldXaml = "Xaml";
        public const string FieldXSParameters = "XSParameters";
        
		#endregion
        
        #region .Ctor
        public WorkflowDefinitionEntity()
             : base("WorkflowDefinition")
        {
			InitializeProperties();
        }

        public WorkflowDefinitionEntity(PrimaryKeyId primaryKeyId)
             : base("WorkflowDefinition", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public WorkflowDefinitionEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public WorkflowDefinitionEntity(string metaClassName, PrimaryKeyId primaryKeyId)
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
            base.Properties.Add("Description", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("PlanDuration", null);
            base.Properties.Add("PlanFinishDate", null);
            base.Properties.Add("PlanFinishTimeType", null);
            base.Properties.Add("Project", null);
            base.Properties.Add("ProjectId", null);
            base.Properties.Add("SchemaId", null);
            base.Properties.Add("SupportedIbnObjectTypes", null);
            base.Properties.Add("TemplateGroup", null);
            base.Properties.Add("Xaml", null);
            base.Properties.Add("XSParameters", null);
            
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
                            case "Description": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "PlanDuration": 
                            case "PlanFinishDate": 
                            case "PlanFinishTimeType": 
                            case "Project": 
                            case "ProjectId": 
                            case "SchemaId": 
                            case "SupportedIbnObjectTypes": 
                            case "TemplateGroup": 
                            case "WorkflowDefinitionId": 
                            case "Xaml": 
                            case "XSParameters": 
                            
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
        
        public Nullable<System.Int32> PlanDuration
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["PlanDuration"].Value;
            }
            
            set
            {
                base.Properties["PlanDuration"].Value = value;
            }
            
        }
        
        public Nullable<System.DateTime> PlanFinishDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["PlanFinishDate"].Value;
            }
            
            set
            {
                base.Properties["PlanFinishDate"].Value = value;
            }
            
        }
        
        public System.Int32 PlanFinishTimeType
        {
            get
            {
                return (System.Int32)base.Properties["PlanFinishTimeType"].Value;
            }
            
            set
            {
                base.Properties["PlanFinishTimeType"].Value = value;
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
        
        public System.Guid SchemaId
        {
            get
            {
                return (System.Guid)base.Properties["SchemaId"].Value;
            }
            
            set
            {
                base.Properties["SchemaId"].Value = value;
            }
            
        }
        
        public System.Int32[] SupportedIbnObjectTypes
        {
            get
            {
                return (System.Int32[])base.Properties["SupportedIbnObjectTypes"].Value;
            }
            
            set
            {
                base.Properties["SupportedIbnObjectTypes"].Value = value;
            }
            
        }
        
        public System.Int32 TemplateGroup
        {
            get
            {
                return (System.Int32)base.Properties["TemplateGroup"].Value;
            }
            
            set
            {
                base.Properties["TemplateGroup"].Value = value;
            }
            
        }
        
        public System.String Xaml
        {
            get
            {
                return (System.String)base.Properties["Xaml"].Value;
            }
            
            set
            {
                base.Properties["Xaml"].Value = value;
            }
            
        }
        
        public System.String XSParameters
        {
            get
            {
                return (System.String)base.Properties["XSParameters"].Value;
            }
            
            set
            {
                base.Properties["XSParameters"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
