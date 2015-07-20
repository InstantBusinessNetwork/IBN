using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents complete assignment request.
	/// </summary>
    public class CloseAssignmentRequest : Request
	{
        #region Const
		#endregion

		#region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteRequest"/> class.
        /// </summary>
		public CloseAssignmentRequest():base(AssignmentRequestMethod.Close, null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteRequest"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
		public CloseAssignmentRequest(EntityObject entity)
			: base(AssignmentRequestMethod.Close, entity)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteRequest"/> class.
        /// </summary>
        /// <param name="primaryKeyId">The primary key id.</param>
        public CloseAssignmentRequest(PrimaryKeyId primaryKeyId)
			: base(AssignmentRequestMethod.Close, new AssignmentEntity(primaryKeyId))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompleteAssignmentRequest"/> class.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <param name="executionResult">The execution result.</param>
		public CloseAssignmentRequest(PrimaryKeyId primaryKeyId, int executionResult)
			: base(AssignmentRequestMethod.Close, new AssignmentEntity(primaryKeyId))
		{
			this.ExecutionResult = executionResult;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the execution result.
		/// </summary>
		/// <value>The execution result.</value>
		public int? ExecutionResult 
		{
			get
			{
				return this.Target[AssignmentEntity.FieldExecutionResult] as int?;
			}
			set
			{
				this.Target[AssignmentEntity.FieldExecutionResult] = value;
			}
		}

		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		/// <value>The comment.</value>
		public string Comment
		{
			get
			{
				return (string)this.Target[AssignmentEntity.FieldComment];
			}
			set
			{
				this.Target[AssignmentEntity.FieldComment] = value;
			}
		}
		#endregion

		#region Methods
		#endregion

	}
}
