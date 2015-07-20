using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Allows access to the value evaluator
	/// </summary>
	public interface IValueEvaluator
	{
		/// <summary>
		/// Evals the specified entity.
		/// </summary>
		/// <param name="thisItem">The this item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		object Eval(object thisItem, string parameters);
	}
}
