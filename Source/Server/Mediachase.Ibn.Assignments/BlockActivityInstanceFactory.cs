using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Assignments
{
	/// <summary>
	/// Represents block activity instance factory.
	/// </summary>
	public class BlockActivityInstanceFactory : ObjectInstanceFactory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BlockActivityInstanceFactory"/> class.
		/// </summary>
		public BlockActivityInstanceFactory()
		{
			this.CompletionType = BlockActivityType.All;
		}

		#region Properties
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public BlockActivityType CompletionType
		{
			get;
			set;
		}
		#endregion

		#region ObjectInstanceFactory Members
		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="master">The master.</param>
		/// <returns></returns>
		public override object CreateInstance()
		{
			BlockActivity retVal = new BlockActivity();
			retVal.Name = "blockActivity_" + Guid.NewGuid().ToString("N");
			retVal.Type = this.CompletionType;

			return retVal;
		}

		#endregion


	}
}
