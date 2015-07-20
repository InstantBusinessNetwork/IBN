using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents ResetDeliveryAttemptsRequest.
	/// </summary>
	public class ResetDeliveryAttemptsRequest: Request
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ResetDeliveryAttemptsRequest"/> class.
		/// </summary>
		public ResetDeliveryAttemptsRequest(PrimaryKeyId primaryKey):
			base(OutgoingMessageQueueMethod.ResetDeliveryAttempts, new OutgoingMessageQueueEntity(primaryKey))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResetDeliveryAttemptsRequest"/> class.
		/// </summary>
		/// <param name="target">The target.</param>
		public ResetDeliveryAttemptsRequest(EntityObject target) :
			base(OutgoingMessageQueueMethod.ResetDeliveryAttempts, target)
		{
		}

		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion


	}
}
