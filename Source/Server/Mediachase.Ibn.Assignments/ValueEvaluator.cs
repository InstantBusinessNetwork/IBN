using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents function evaluator.
	/// </summary>
	public static class ValueEvaluator
	{
		#region Const	
		#endregion 

		#region Methods
		/// <summary>
		/// Evals the specified value evaluator type name.
		/// </summary>
		/// <param name="valueEvaluatorTypeName">Name of the value evaluator type.</param>
		/// <param name="thisItem">The this item.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns></returns>
		public static object Eval(string valueEvaluatorTypeName, object thisItem, string parameter)
		{
			if (valueEvaluatorTypeName == null)
				throw new ArgumentNullException("valueEvaluatorTypeName");

			return Eval(AssemblyUtil.LoadType(valueEvaluatorTypeName), thisItem, parameter);
		}

		/// <summary>
		/// Evals the specified value evaluator type.
		/// </summary>
		/// <param name="valueEvaluatorType">Type of the value evaluator.</param>
		/// <param name="thisItem">The this item.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns></returns>
		public static object Eval(Type valueEvaluatorType, object thisItem, string parameter)
		{
			if (valueEvaluatorType == null)
				throw new ArgumentNullException("valueEvaluatorType");

			return Eval((IValueEvaluator)Activator.CreateInstance(valueEvaluatorType), thisItem, parameter);
		}

		/// <summary>
		/// Evals the specified value evaluator.
		/// </summary>
		/// <param name="valueEvaluator">The value evaluator.</param>
		/// <param name="thisItem">The this item.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public static object Eval(IValueEvaluator valueEvaluator, object thisItem, string parameters)
		{
			if (valueEvaluator == null)
				throw new ArgumentNullException("valueEvaluator");

			return valueEvaluator.Eval(thisItem, parameters);
		}
		#endregion 
	}
}
