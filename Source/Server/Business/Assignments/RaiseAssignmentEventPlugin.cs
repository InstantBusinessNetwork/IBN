using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Assignments;

namespace Mediachase.IBN.Business.Assignments
{
	/// <summary>
	/// Represents raise assignment event plugin.
	/// </summary>
	public sealed class RaiseAssignmentEventPlugin: IPlugin
	{
		#region Const
		private const string OldAssignmentEntity = "AssignmentEventPlugin_OldAssignmentEntity";
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="RaiseAssignmentEventPlugin"/> class.
		/// </summary>
		public RaiseAssignmentEventPlugin()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Saves the old assignment entity.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="entity">The entity.</param>
		private void SaveOldAssignmentEntity(BusinessContext context, AssignmentEntity entity)
		{
			context.Items[OldAssignmentEntity] = entity;
		}

		/// <summary>
		/// Loads the old assignment entity.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private AssignmentEntity LoadOldAssignmentEntity(BusinessContext context)
		{
			return (AssignmentEntity)context.Items[OldAssignmentEntity];
		}
		#endregion

		#region IPlugin Members

		/// <summary>
		/// Executes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(BusinessContext context)
		{
			if (context.PluginStage == EventPipeLineStage.PreMainOperationInsideTranasaction)
			{
				switch (context.GetMethod())
				{
					case RequestMethod.Delete:
						// TODO: Pre Method
						OnAssignmentDeleting(context.GetTargetPrimaryKeyId().Value);
						break;
					case AssignmentRequestMethod.AssignUser:
					case RequestMethod.Update:
					case AssignmentRequestMethod.Activate:
					case AssignmentRequestMethod.Close:
					case AssignmentRequestMethod.Resume:
					case AssignmentRequestMethod.Suspend:
						// Save Old AssignmentEntity
						SaveOldAssignmentEntity(context, (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value));
						break;
				}
			}
			else if (context.PluginStage == EventPipeLineStage.PostMainOperationInsideTranasaction)
			{
				switch (context.GetMethod())
				{
					case RequestMethod.Create:
						OnAssignmentCreated(((CreateResponse)context.Response).PrimaryKeyId);
						break;
					case AssignmentRequestMethod.AssignUser:
					case RequestMethod.Update:
					case AssignmentRequestMethod.Activate:
					case AssignmentRequestMethod.Close:
					case AssignmentRequestMethod.Resume:
					case AssignmentRequestMethod.Suspend:
						// TODO: Pre Method
						OnAssignmentUpdated(LoadOldAssignmentEntity(context), (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, context.GetTargetPrimaryKeyId().Value));
						break;
				}
			}

		}


		/// <summary>
		/// Called when [assignment updated].
		/// </summary>
		/// <param name="primaryKey">The primary key.</param>
		private void OnAssignmentUpdated(AssignmentEntity oldEntity, AssignmentEntity newEntity)
		{
			PrimaryKeyId primaryKey = oldEntity.PrimaryKeyId.Value;

			// Check User
			if (newEntity.UserId != oldEntity.UserId)
				OnAssignmentUserAssigned(primaryKey, oldEntity.UserId, newEntity.UserId);

			// Subject
			if (newEntity.Subject != oldEntity.Subject )
				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_GeneralInfo, (Guid)primaryKey);

			// Priority
			if (newEntity.Priority != oldEntity.Priority)
				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_Priority, (Guid)primaryKey);

			// State
			if (newEntity.State != oldEntity.State)
			{
				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_Status, (Guid)primaryKey);

				// delete DateTypeValue for closed assignment (we notify about active only)
				if (newEntity.State == (int)AssignmentState.Closed)
					Schedule.DeleteDateTypeValue(DateTypes.AssignmentFinishDate, (Guid)primaryKey);
			}

			// Finish Date
			if (newEntity.PlanFinishDate != oldEntity.PlanFinishDate)
			{
				if (oldEntity.PlanFinishDate.HasValue)
				{
					Schedule.DeleteDateTypeValue(DateTypes.AssignmentFinishDate, (Guid)primaryKey);
				}

				DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);

				if (newEntity.PlanFinishDate.HasValue && newEntity.PlanFinishDate.Value > dateTimeNow)
				{
					Schedule.UpdateDateTypeValue(DateTypes.AssignmentFinishDate, (Guid)primaryKey, DataContext.Current.CurrentUserTimeZone.ToUniversalTime(newEntity.PlanFinishDate.Value));
				}

				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_FinishDate, (Guid)primaryKey);
			}
		}

		/// <summary>
		/// Called when [assignment user assigned].
		/// </summary>
		/// <param name="primaryKey">The primary key.</param>
		/// <param name="oldUserId">The old user id.</param>
		/// <param name="newUserId">The new user id.</param>
		private void OnAssignmentUserAssigned(PrimaryKeyId primaryKey, int? oldUserId, int? newUserId)
		{
			if(oldUserId.HasValue)
				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_Participant_Resigned, (Guid)primaryKey, oldUserId.Value);

			if (newUserId.HasValue)
				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_Participant_Assigned, (Guid)primaryKey, newUserId.Value);
		}

		/// <summary>
		/// Called when [assignment created].
		/// </summary>
		/// <param name="primaryKey">The primary key.</param>
		private void OnAssignmentCreated(PrimaryKeyId primaryKey)
		{
			DateTime dateTimeNow = DataContext.Current.CurrentUserTimeZone.ToLocalTime(DateTime.UtcNow);
			AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, primaryKey);

			SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Created, (Guid)primaryKey);

			if (entity.PlanFinishDate.HasValue && entity.PlanFinishDate.Value > dateTimeNow)
			{
				Schedule.UpdateDateTypeValue(DateTypes.AssignmentFinishDate, (Guid)primaryKey, DataContext.Current.CurrentUserTimeZone.ToUniversalTime(entity.PlanFinishDate.Value));
			}

			if(entity.UserId.HasValue)
				SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Updated_Participant_Assigned, (Guid)primaryKey, entity.UserId.Value);
		}

		/// <summary>
		/// Called when [assignment deleting].
		/// </summary>
		/// <param name="primaryKey">The primary key.</param>
		private void OnAssignmentDeleting(PrimaryKeyId primaryKey)
		{
			SystemEvents.AddSystemEvents(SystemEventTypes.Assignment_Deleted, (Guid)primaryKey);
		}


		#endregion
	} 
}
