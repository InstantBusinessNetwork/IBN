using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.IBN.Business.Assignments
{
	/// <summary>
	/// Represents sync acl with assugnment plugin.
	/// </summary>
	public class SyncAclWithAssignmentPlugin: IPlugin
	{
		#region Const
		#endregion

		#region Properties
		public BusinessContext Context { get; set; }
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SyncAclWithAssugnmentPlugin"/> class.
		/// </summary>
		public SyncAclWithAssignmentPlugin()
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
				// Post Main Operations
				case AssignmentRequestMethod.Resume:
				case AssignmentRequestMethod.Activate:
				case AssignmentRequestMethod.AssignUser:
					AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
					Guid aclOwnerKey = (Guid)context.GetTargetPrimaryKeyId().Value;

					if (LoadFileLibraryAcl(entity, out fs, out acl))
					{
						// Remove All Assignment ACE
						for (int index = acl.Count - 1; index >= 0; index--)
						{
							if (acl[index].OwnerKey == aclOwnerKey)
								acl.RemoveAt(index);
						}

						// Add New
						if (entity.UserId.HasValue && CanUserWrite(entity))
							acl.Add(new AccessControlEntry(entity.UserId.Value, "Write", true, aclOwnerKey));

						// Save ACL
						AccessControlList.SetACL(fs, acl, false);
					}					
					break;
				// Pre Main Operations
				case AssignmentRequestMethod.Close:
				case AssignmentRequestMethod.Suspend:
				case RequestMethod.Delete:
					AssignmentEntity delEntity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
					Guid delAclOwnerKey = (Guid)context.GetTargetPrimaryKeyId().Value;

					if (LoadFileLibraryAcl(delEntity, out fs, out acl))
					{
						// Remove All Assignment ACE
						for (int index = acl.Count - 1; index >= 0; index--)
						{
							if (acl[index].OwnerKey == delAclOwnerKey)
								acl.RemoveAt(index);
						}

						// Save ACL
						AccessControlList.SetACL(fs, acl, false);
					}
					break;
			}
		}

		/// <summary>
		/// Users the can write.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		private bool CanUserWrite(AssignmentEntity entity)
		{
			if (entity.WorkflowInstanceId.HasValue && 
				!string.IsNullOrEmpty(entity.WorkflowActivityName))
			{
				PropertyValueCollection properties = WorkflowActivityWrapper.GetAssignmentProperties((Guid)entity.WorkflowInstanceId.Value, entity.WorkflowActivityName);

				if (properties.Contains(AssignmentCustomProperty.ReadOnlyLibraryAccess))
				{
					bool? value = properties[AssignmentCustomProperty.ReadOnlyLibraryAccess] as bool?;
					if (value.HasValue)
						return !value.Value;
				}
			}

			return true;
		}

		/// <summary>
		/// Loads the file library acl.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		private bool LoadFileLibraryAcl(AssignmentEntity entity, out FileStorage fs, out AccessControlList acl)
		{
			acl = null;
			fs = null;

			// Resolve ContainerKey
			string containerName = "FileLibrary";
			string containerKey = string.Empty;

			if (entity.OwnerDocumentId.HasValue)
				containerKey = UserRoleHelper.CreateDocumentContainerKey(entity.OwnerDocumentId.Value);
			//else 
			// TODO: Add Ace other owner here

			// Check ContainerKey
			if (string.IsNullOrEmpty(containerKey))
			    return false;

			// Open ACL
			BaseIbnContainer bic = BaseIbnContainer.Create(containerName, containerKey);
			fs = (FileStorage)bic.LoadControl("FileStorage");

			acl = AccessControlList.GetACL(fs.Root.Id);

			return true;
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
