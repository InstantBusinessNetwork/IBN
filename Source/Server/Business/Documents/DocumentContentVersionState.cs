using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.IBN.Business.Documents
{
	/// <summary>
	/// Describies document content version state.
	/// </summary>
	public enum DocumentContentVersionState
	{
		/// <summary>
		/// The version is draft
		/// </summary>
		Draft = 1,

		/// <summary>
		/// The version is active.
		/// </summary>
		Active = 2,

		/// <summary>
		/// The version is obsolete.
		/// </summary>
		Obsolete = 3,
	}
}
