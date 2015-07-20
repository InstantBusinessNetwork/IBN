using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database.Assignments;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.Assignments
{
	/// <summary>
	/// Represents sync acl with business process plugin.
	/// </summary>
	public class SyncAclWithBusinessProcessPlugin: IPlugin
	{
		#region Const
		#endregion

		#region Properties
		public BusinessContext Context { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncAclWithBusinessProcessPlugin"/> class.
		/// </summary>
		public SyncAclWithBusinessProcessPlugin()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public virtual void Execute(BusinessContext context)
		{
			// Load ACL
			FileStorage fs;
			AccessControlList acl;

			switch (context.GetMethod())
			{
				case RequestMethod.Create:
					WorkflowInstanceEntity newEntity = this.Context.Request.Target as WorkflowInstanceEntity;
					Guid newWorkflowInstanceId = (Guid)((CreateResponse)context.Response).PrimaryKeyId;

					if (LoadFileLibraryAcl(newEntity, out fs, out acl))
					{
						// Read Users From Activity And Create ACE
						foreach (int userId in GetUserList(newEntity))
						{
							// Modify ACL
							acl.Add(new AccessControlEntry(userId, "Read", true, newWorkflowInstanceId));

							// Create WorkflowParticipant
							CreateWorkflowParticipiant(newWorkflowInstanceId, userId, newEntity);
						}

						// Save ACL
						AccessControlList.SetACL(fs, acl, false);
					}
					break;
				case RequestMethod.Update:
					WorkflowInstanceEntity entity = this.Context.Request.Target as WorkflowInstanceEntity;
					Guid workflowInstanceId = (Guid)context.GetTargetPrimaryKeyId().Value;

					if (LoadFileLibraryAcl(entity, out fs, out acl))
					{
						// Remove All Workflow ACE
						for (int index = acl.Count - 1; index>=0; index--)
						{
							if (acl[index].OwnerKey == workflowInstanceId)
								acl.RemoveAt(index);
						}

						// Remove WorkflowParticipantRow
						RemoveAllWorkflowParticipants(workflowInstanceId);

						// Read Users From Activity And Create ACE
						foreach (int userId in GetUserList(entity))
						{
							acl.Add(new AccessControlEntry(userId, "Read", true, workflowInstanceId));

							// Create WorkflowParticipant
							CreateWorkflowParticipiant(workflowInstanceId, userId, entity);
						}

						// Save ACL
						AccessControlList.SetACL(fs, acl, false);
					}
					break;
				case RequestMethod.Delete:
					WorkflowInstanceEntity delEntity = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
					Guid delWorkflowInstanceId = (Guid)context.GetTargetPrimaryKeyId().Value;

					if (LoadFileLibraryAcl(delEntity, out fs, out acl))
					{
						// Remove All Workflow ACE
						for (int index = acl.Count - 1; index >= 0; index--)
						{
							if (acl[index].OwnerKey == delWorkflowInstanceId)
								acl.RemoveAt(index);
						}

						// Remove WorkflowParticipantRow
						RemoveAllWorkflowParticipants(delWorkflowInstanceId);

						// Save ACL
						AccessControlList.SetACL(fs, acl, false);
					}
					break;
			}
		}

		/// <summary>
		/// Removes all workflow participants.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		private void RemoveAllWorkflowParticipants(Guid workflowInstanceId)
		{
			WorkflowParticipantRow[] oldParticipants = WorkflowParticipantRow.List(FilterElement.EqualElement(WorkflowParticipantRow.ColumnWorkflowInstanceId, workflowInstanceId));
			foreach (WorkflowParticipantRow oldParticipant in oldParticipants)
			{
				oldParticipant.Delete();
			}
		}

		/// <summary>
		/// Creates the workflow participiant.
		/// </summary>
		/// <param name="workflowInstanceId">The workflow instance id.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="entity">The entity.</param>
		private void CreateWorkflowParticipiant(Guid workflowInstanceId, int userId, WorkflowInstanceEntity entity)
		{
			WorkflowParticipantRow row = new WorkflowParticipantRow();
			row.UserId = userId;
			row.WorkflowInstanceId = workflowInstanceId;

			// TODO: Extend Owner Processing
			row.ObjectId = entity.OwnerDocumentId.Value;
			row.ObjectType = (int)ObjectTypes.Document;

			row.Update();
		}

		/// <summary>
		/// Loads the file library acl.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		private bool LoadFileLibraryAcl(WorkflowInstanceEntity entity, out FileStorage fs, out AccessControlList acl)
		{
			acl = null;
			fs = null;

			// Resolve ContainerKey
			string containerName = "FileLibrary";
			string containerKey = string.Empty;

			if (entity.OwnerDocumentId.HasValue)
				containerKey = UserRoleHelper.CreateDocumentContainerKey(entity.OwnerDocumentId.Value);
			//else 
			// TODO: Extend Owner Processing

			// Check ContainerKey
			if (string.IsNullOrEmpty(containerKey))
			    return false;

			// Open ACL
			BaseIbnContainer bic = BaseIbnContainer.Create(containerName, containerKey);
			fs = (FileStorage)bic.LoadControl("FileStorage");

			acl = AccessControlList.GetACL(fs.Root.Id);

			return true;
		}

		/// <summary>
		/// Gets the user list.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		private int[] GetUserList(WorkflowInstanceEntity entity)
		{
			object workflowRoot = McWorkflowSerializer.GetObject(entity.Xaml);

			return WorkflowActivityWrapper.GetActivityUserList(workflowRoot);
		}
		#endregion

		#region IPlugin Members

		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void IPlugin.Execute(BusinessContext context)
		{
			this.Context = context;

			this.Execute(context);
		}

		#endregion
	}
}
