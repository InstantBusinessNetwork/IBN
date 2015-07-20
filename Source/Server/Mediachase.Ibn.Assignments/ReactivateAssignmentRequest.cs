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
    public class ReactivateAssignmentRequest : Request
	{
        #region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ReactivateAssignmentRequest"/> class.
		/// </summary>
		public ReactivateAssignmentRequest():base(AssignmentRequestMethod.Reactivate, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactivateAssignmentRequest"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public ReactivateAssignmentRequest(EntityObject entity)
			: base(AssignmentRequestMethod.Reactivate, entity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactivateAssignmentRequest"/> class.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
        public ReactivateAssignmentRequest(PrimaryKeyId primaryKeyId)
			: base(AssignmentRequestMethod.Reactivate, new AssignmentEntity(primaryKeyId))
		{
		}
		#endregion

		#region Properties

		#endregion

		#region Methods
		#endregion

	}
}
