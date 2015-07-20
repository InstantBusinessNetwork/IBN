using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents activate assignment request.
	/// </summary>
    public class ActivateAssignmentRequest : Request
	{
        #region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivateAssignmentRequest"/> class.
		/// </summary>
		public ActivateAssignmentRequest():base(AssignmentRequestMethod.Activate, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivateAssignmentRequest"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public ActivateAssignmentRequest(EntityObject entity)
			: base(AssignmentRequestMethod.Activate, entity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivateAssignmentRequest"/> class.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
        public ActivateAssignmentRequest(PrimaryKeyId primaryKeyId)
			: base(AssignmentRequestMethod.Activate, new AssignmentEntity(primaryKeyId))
		{
		}
		#endregion

		#region Properties

		#endregion

		#region Methods
		#endregion

	}
}
