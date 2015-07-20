using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents suspend assignment request.
	/// </summary>
    public class SuspendAssignmentRequest : Request
	{
        #region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendAssignmentRequest"/> class.
		/// </summary>
		public SuspendAssignmentRequest()
			: base(AssignmentRequestMethod.Suspend, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendAssignmentRequest"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public SuspendAssignmentRequest(EntityObject entity)
			: base(AssignmentRequestMethod.Suspend, entity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SuspendAssignmentRequest"/> class.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
        public SuspendAssignmentRequest(PrimaryKeyId primaryKeyId)
			: base(AssignmentRequestMethod.Suspend, new AssignmentEntity(primaryKeyId))
		{
		}
		#endregion

		#region Properties

		#endregion

		#region Methods
		#endregion

	}
}
