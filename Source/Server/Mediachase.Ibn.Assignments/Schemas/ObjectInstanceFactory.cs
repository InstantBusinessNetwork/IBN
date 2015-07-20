using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Allows access to the activity instance factory
	/// </summary>
	public abstract class ObjectInstanceFactory
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type { get; set; }

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="master">The master.</param>
		/// <returns></returns>
		public abstract object CreateInstance();
	}
}
