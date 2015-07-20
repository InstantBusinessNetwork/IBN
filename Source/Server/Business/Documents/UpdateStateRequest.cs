using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.IBN.Business.Documents
{
	public class UpdateStateRequest: UpdateRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateStateRequest"/> class.
		/// </summary>
		public UpdateStateRequest(): base()
		{
			this.Method = DocumentContentVersionRequestMethod.UpdateState;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateStateRequest"/> class.
		/// </summary>
		/// <param name="target">The target.</param>
		public UpdateStateRequest(EntityObject target)
			: base(target)
		{
			this.Method = DocumentContentVersionRequestMethod.UpdateState;
		}

	}
}
