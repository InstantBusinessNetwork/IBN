using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents resume assignment request.
	/// </summary>
    public class ResumeAssignmentRequest : Request
	{
        #region Const
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ResumeAssignmentRequest"/> class.
		/// </summary>
		public ResumeAssignmentRequest()
			: base(AssignmentRequestMethod.Resume, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResumeAssignmentRequest"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public ResumeAssignmentRequest(EntityObject entity)
			: base(AssignmentRequestMethod.Resume, entity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResumeAssignmentRequest"/> class.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
        public ResumeAssignmentRequest(PrimaryKeyId primaryKeyId)
			: base(AssignmentRequestMethod.Resume, new AssignmentEntity(primaryKeyId))
		{
		}
		#endregion

		#region Properties

		#endregion

		#region Methods
		#endregion

	}
}
