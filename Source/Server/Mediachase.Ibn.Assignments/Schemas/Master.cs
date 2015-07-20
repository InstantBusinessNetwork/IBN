using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Assignments.Schemas
{
	/// <summary>
	/// Represents master.
	/// </summary>
	public abstract class Master
	{
		#region Const	
		#endregion 

		#region Fields	
		#endregion 

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="Master"/> class.
		/// </summary>
		public Master()
		{
		}
		#endregion
	
		#region Properties
		/// <summary>
		/// Gets or sets the type of the owner.
		/// </summary>
		/// <value>The type of the owner.</value>
		public string TypeName { get; set; }

		/// <summary>
		/// Gets or sets the instance factory.
		/// </summary>
		/// <value>The instance factory.</value>
		public ObjectInstanceFactory InstanceFactory { get; set; }
		#endregion 

		#region Methods
		#endregion 
	}
}
