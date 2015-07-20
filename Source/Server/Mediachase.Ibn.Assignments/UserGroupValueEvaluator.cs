using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents user group value evaluator.
	/// </summary>
	public sealed class UserGroupValueEvaluator: IValueEvaluator
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="UserGroupValueEvaluator"/> class.
		/// </summary>
		public UserGroupValueEvaluator()
		{
		}
		#endregion

		#region Methods
		#endregion


		#region IValueEvaluator Members

		/// <summary>
		/// Evals the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		object IValueEvaluator.Eval(object thisItem, string parameters)
		{
			List<int> retVal = new List<int>();

			foreach (string strVal in parameters.Split(',', ';', ' '))
			{
				int iVal;
				if (int.TryParse(strVal, out iVal))
				{
					retVal.Add(iVal);
				}
			}

			return retVal.ToArray();
		}

		#endregion
	} 
}
