
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
    public partial class WorkflowInstanceEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName = "WorkflowInstance";
        public const string PrimaryKeyName = "PrimaryKeyId";
        #endregion
        
        #region Field's name const
        public const string FieldActualFinishDate = "ActualFinishDate";
        public const string FieldActualStartDate = "ActualStartDate";
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldDescription = "Description";
        public const string FieldExecutionResult = "ExecutionResult";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldName = "Name";
        public const string FieldOwnerDocumentId = "OwnerDocumentId";
        public const string FieldPlanDuration = "PlanDuration";
        public const string FieldPlanFinishDate = "PlanFinishDate";
        public const string FieldPlanFinishTimeType = "PlanFinishTimeType";
        public const string FieldSchemaId = "SchemaId";
        public const string FieldState = "State";
        public const string FieldTimeStatus = "TimeStatus";
        public const string FieldXaml = "Xaml";
        public const string FieldXSParameters = "XSParameters";
        
		#endregion
        
        #region .Ctor
        public WorkflowInstanceEntity()
             : base("WorkflowInstance")
        {
			InitializeProperties();
        }

        public WorkflowInstanceEntity(PrimaryKeyId primaryKeyId)
             : base("WorkflowInstance", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public WorkflowInstanceEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public WorkflowInstanceEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("ActualFinishDate", null);
            base.Properties.Add("ActualStartDate", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("Description", null);
            base.Properties.Add("ExecutionResult", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("Name", null);
            base.Properties.Add("OwnerDocumentId", null);
            base.Properties.Add("PlanDuration", null);
            base.Properties.Add("PlanFinishDate", null);
            base.Properties.Add("PlanFinishTimeType", null);
            base.Properties.Add("SchemaId", null);
            base.Properties.Add("State", null);
            base.Properties.Add("TimeStatus", null);
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
                            case "ActualFinishDate": 
                            case "ActualStartDate": 
                            case "Created": 
                            case "CreatorId": 
                            case "Description": 
                            case "ExecutionResult": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "OwnerDocumentId": 
                            case "PlanDuration": 
                            case "PlanFinishDate": 
                            case "PlanFinishTimeType": 
                            case "SchemaId": 
                            case "State": 
                            case "TimeStatus": 
                            case "WorkflowInstanceId": 
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
        
        public Nullable<System.DateTime> ActualFinishDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["ActualFinishDate"].Value;
            }
            
            set
            {
                base.Properties["ActualFinishDate"].Value = value;
            }
            
        }
        
        public Nullable<System.DateTime> ActualStartDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["ActualStartDate"].Value;
            }
            
            set
            {
                base.Properties["ActualStartDate"].Value = value;
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
        
        public Nullable<System.Int32> ExecutionResult
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ExecutionResult"].Value;
            }
            
            set
            {
                base.Properties["ExecutionResult"].Value = value;
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
        
        public Nullable<System.Int32> OwnerDocumentId
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["OwnerDocumentId"].Value;
            }
            
            set
            {
                base.Properties["OwnerDocumentId"].Value = value;
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
        
        public System.Int32 State
        {
            get
            {
                return (System.Int32)base.Properties["State"].Value;
            }
            
            set
            {
                base.Properties["State"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> TimeStatus
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["TimeStatus"].Value;
            }
            
            set
            {
                base.Properties["TimeStatus"].Value = value;
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
