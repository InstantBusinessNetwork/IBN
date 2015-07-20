
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
    public partial class AssignmentEntity: EntityObject
    {
        private EntityObjectProperty[] _exProperties;
        
        #region Util
        public const string ClassName = "Assignment";
        public const string PrimaryKeyName = "PrimaryKeyId";
        #endregion
        
        #region Field's name const
        public const string FieldActualAssignDate = "ActualAssignDate";
        public const string FieldActualFinishDate = "ActualFinishDate";
        public const string FieldActualStartDate = "ActualStartDate";
        public const string FieldActualWork = "ActualWork";
        public const string FieldAssignmentType = "AssignmentType";
        public const string FieldAutoComplete = "AutoComplete";
        public const string FieldClosedBy = "ClosedBy";
        public const string FieldComment = "Comment";
        public const string FieldCreated = "Created";
        public const string FieldCreatorId = "CreatorId";
        public const string FieldExecutionResult = "ExecutionResult";
        public const string FieldModified = "Modified";
        public const string FieldModifierId = "ModifierId";
        public const string FieldOwnerDocumentId = "OwnerDocumentId";
        public const string FieldParentAssignment = "ParentAssignment";
        public const string FieldParentAssignmentId = "ParentAssignmentId";
        public const string FieldPlanAssignDate = "PlanAssignDate";
        public const string FieldPlanFinishDate = "PlanFinishDate";
        public const string FieldPlanStartDate = "PlanStartDate";
        public const string FieldPlanWork = "PlanWork";
        public const string FieldPriority = "Priority";
        public const string FieldState = "State";
        public const string FieldSubject = "Subject";
        public const string FieldTaskTime = "TaskTime";
        public const string FieldTimeStatus = "TimeStatus";
        public const string FieldType = "Type";
        public const string FieldUserId = "UserId";
        public const string FieldUserStatus = "UserStatus";
        public const string FieldWorkflowActivityName = "WorkflowActivityName";
        public const string FieldWorkflowInstance = "WorkflowInstance";
        public const string FieldWorkflowInstanceId = "WorkflowInstanceId";
        public const string FieldWorkUnit = "WorkUnit";
        
		#endregion
        
        #region .Ctor
        public AssignmentEntity()
             : base("Assignment")
        {
			InitializeProperties();
        }

        public AssignmentEntity(PrimaryKeyId primaryKeyId)
             : base("Assignment", primaryKeyId)
        {
			InitializeProperties();
        }
        
        public AssignmentEntity(string metaClassName)
            : base(metaClassName)
        {
			InitializeProperties();
        }
        
        public AssignmentEntity(string metaClassName, PrimaryKeyId primaryKeyId)
            : base(metaClassName, primaryKeyId)
        {
			InitializeProperties();
        }
        #endregion
        
        #region Initialize Properties
        protected void InitializeProperties()
        {
			base.Properties.Add("ActualAssignDate", null);
            base.Properties.Add("ActualFinishDate", null);
            base.Properties.Add("ActualStartDate", null);
            base.Properties.Add("ActualWork", null);
            base.Properties.Add("AssignmentType", null);
            base.Properties.Add("AutoComplete", null);
            base.Properties.Add("ClosedBy", null);
            base.Properties.Add("Comment", null);
            base.Properties.Add("Created", null);
            base.Properties.Add("CreatorId", null);
            base.Properties.Add("ExecutionResult", null);
            base.Properties.Add("Modified", null);
            base.Properties.Add("ModifierId", null);
            base.Properties.Add("OwnerDocumentId", null);
            base.Properties.Add("ParentAssignment", null);
            base.Properties.Add("ParentAssignmentId", null);
            base.Properties.Add("PlanAssignDate", null);
            base.Properties.Add("PlanFinishDate", null);
            base.Properties.Add("PlanStartDate", null);
            base.Properties.Add("PlanWork", null);
            base.Properties.Add("Priority", null);
            base.Properties.Add("State", null);
            base.Properties.Add("Subject", null);
            base.Properties.Add("TaskTime", null);
            base.Properties.Add("TimeStatus", null);
            base.Properties.Add("Type", null);
            base.Properties.Add("UserId", null);
            base.Properties.Add("UserStatus", null);
            base.Properties.Add("WorkflowActivityName", null);
            base.Properties.Add("WorkflowInstance", null);
            base.Properties.Add("WorkflowInstanceId", null);
            base.Properties.Add("WorkUnit", null);
            
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
                            case "ActualAssignDate": 
                            case "ActualFinishDate": 
                            case "ActualStartDate": 
                            case "ActualWork": 
                            case "AssignmentId": 
                            case "AssignmentType": 
                            case "AutoComplete": 
                            case "ClosedBy": 
                            case "Comment": 
                            case "Created": 
                            case "CreatorId": 
                            case "ExecutionResult": 
                            case "Modified": 
                            case "ModifierId": 
                            case "OwnerDocumentId": 
                            case "ParentAssignment": 
                            case "ParentAssignmentId": 
                            case "PlanAssignDate": 
                            case "PlanFinishDate": 
                            case "PlanStartDate": 
                            case "PlanWork": 
                            case "Priority": 
                            case "State": 
                            case "Subject": 
                            case "TaskTime": 
                            case "TimeStatus": 
                            case "Type": 
                            case "UserId": 
                            case "UserStatus": 
                            case "WorkflowActivityName": 
                            case "WorkflowInstance": 
                            case "WorkflowInstanceId": 
                            case "WorkUnit": 
                            
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
        
        public Nullable<System.DateTime> ActualAssignDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["ActualAssignDate"].Value;
            }
            
            set
            {
                base.Properties["ActualAssignDate"].Value = value;
            }
            
        }
        
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
        
        public Nullable<System.Double> ActualWork
        {
            get
            {
                return (Nullable<System.Double>)base.Properties["ActualWork"].Value;
            }
            
            set
            {
                base.Properties["ActualWork"].Value = value;
            }
            
        }
        
        public System.String AssignmentType
        {
            get
            {
                return (System.String)base.Properties["AssignmentType"].Value;
            }
            
            set
            {
                base.Properties["AssignmentType"].Value = value;
            }
            
        }
        
        public System.Boolean AutoComplete
        {
            get
            {
                return (System.Boolean)base.Properties["AutoComplete"].Value;
            }
            
            set
            {
                base.Properties["AutoComplete"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ClosedBy
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ClosedBy"].Value;
            }
            
            set
            {
                base.Properties["ClosedBy"].Value = value;
            }
            
        }
        
        public System.String Comment
        {
            get
            {
                return (System.String)base.Properties["Comment"].Value;
            }
            
            set
            {
                base.Properties["Comment"].Value = value;
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
        
        public System.String ParentAssignment
        {
            get
            {
                return (System.String)base.Properties["ParentAssignment"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ParentAssignmentId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ParentAssignmentId"].Value;
            }
            
            set
            {
                base.Properties["ParentAssignmentId"].Value = value;
            }
            
        }
        
        public Nullable<System.DateTime> PlanAssignDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["PlanAssignDate"].Value;
            }
            
            set
            {
                base.Properties["PlanAssignDate"].Value = value;
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
        
        public Nullable<System.DateTime> PlanStartDate
        {
            get
            {
                return (Nullable<System.DateTime>)base.Properties["PlanStartDate"].Value;
            }
            
            set
            {
                base.Properties["PlanStartDate"].Value = value;
            }
            
        }
        
        public Nullable<System.Double> PlanWork
        {
            get
            {
                return (Nullable<System.Double>)base.Properties["PlanWork"].Value;
            }
            
            set
            {
                base.Properties["PlanWork"].Value = value;
            }
            
        }
        
        public System.Int32 Priority
        {
            get
            {
                return (System.Int32)base.Properties["Priority"].Value;
            }
            
            set
            {
                base.Properties["Priority"].Value = value;
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
        
        public System.String Subject
        {
            get
            {
                return (System.String)base.Properties["Subject"].Value;
            }
            
            set
            {
                base.Properties["Subject"].Value = value;
            }
            
        }
        
        public System.Int32 TaskTime
        {
            get
            {
                return (System.Int32)base.Properties["TaskTime"].Value;
            }
            
            set
            {
                base.Properties["TaskTime"].Value = value;
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
        
        public System.String Type
        {
            get
            {
                return (System.String)base.Properties["Type"].Value;
            }
            
            set
            {
                base.Properties["Type"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> UserId
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["UserId"].Value;
            }
            
            set
            {
                base.Properties["UserId"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> UserStatus
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["UserStatus"].Value;
            }
            
            set
            {
                base.Properties["UserStatus"].Value = value;
            }
            
        }
        
        public System.String WorkflowActivityName
        {
            get
            {
                return (System.String)base.Properties["WorkflowActivityName"].Value;
            }
            
            set
            {
                base.Properties["WorkflowActivityName"].Value = value;
            }
            
        }
        
        public System.String WorkflowInstance
        {
            get
            {
                return (System.String)base.Properties["WorkflowInstance"].Value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> WorkflowInstanceId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["WorkflowInstanceId"].Value;
            }
            
            set
            {
                base.Properties["WorkflowInstanceId"].Value = value;
            }
            
        }
        
        public System.String WorkUnit
        {
            get
            {
                return (System.String)base.Properties["WorkUnit"].Value;
            }
            
            set
            {
                base.Properties["WorkUnit"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
