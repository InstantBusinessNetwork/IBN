using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.Assignments
{
	public class StickedObjectCleanUpPlugin: IPlugin
	{
		public StickedObjectCleanUpPlugin()
		{

		}

		#region IPlugin Members
		/// <summary>
		/// Deletes sticked items if the method is "delete" or assignment state is not active
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(BusinessContext context)
		{
			if (context.GetMethod() == RequestMethod.Load || 
				context.GetMethod() == RequestMethod.List)
				return;

			PrimaryKeyId? pkId = context.GetTargetPrimaryKeyId();
			if (pkId.HasValue)
			{
				AssignmentEntity entity = (AssignmentEntity)BusinessManager.Load(AssignmentEntity.ClassName, pkId.Value);

				if (entity.OwnerDocumentId.HasValue)
				{
					if (context.GetMethod() == RequestMethod.Delete || 
						entity.State != (int)AssignmentState.Active)
					{
						if (entity.UserId.HasValue)
						{
							Calendar.DeleteStickedObject(entity.UserId.Value, entity.OwnerDocumentId.Value, (int)ObjectTypes.Document, (Guid)pkId.Value);
						}
					}
				}
			}
		}
		#endregion
	}
}
