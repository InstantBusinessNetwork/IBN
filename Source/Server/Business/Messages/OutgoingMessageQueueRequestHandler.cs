using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents OutgoingMessageQueue request handler.
	/// </summary>
	public class OutgoingMessageQueueRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="OutgoingMessageQueueRequestHandler"/> class.
		/// </summary>
		public OutgoingMessageQueueRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == OutgoingMessageQueueEntity.ClassName)
			{
				OutgoingMessageQueueEntity retVal = new OutgoingMessageQueueEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		/// <summary>
		/// Pres the create inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);
		}
		#endregion

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void CustomMethod(BusinessContext context)
		{
			base.CustomMethod(context);

			switch (context.GetMethod())
			{
				case OutgoingMessageQueueMethod.ResetDeliveryAttempts:
					ResetDeliveryAttempts(context);
					break;
			}
		}

		#endregion

		#region ResetDeliveryAttempts
		/// <summary>
		/// Resets the delivery attempts.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void ResetDeliveryAttempts(BusinessContext context)
		{
			MetaObject metaObject = MetaObjectActivator.CreateInstance(context.GetTargetMetaClassName(), context.GetTargetPrimaryKeyId().Value);

			metaObject[OutgoingMessageQueueEntity.FieldDeliveryAttempts] = 0;

			metaObject.Save();
		}
		#endregion
	}
}
