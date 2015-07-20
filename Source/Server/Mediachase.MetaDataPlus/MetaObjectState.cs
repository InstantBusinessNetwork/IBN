using System;

namespace Mediachase.MetaDataPlus
{
	/// <summary>
	/// Summary description for MetaObjectState.
	/// </summary>
	public enum MetaObjectState
	{
		/// <summary>
		/// The object has been added, and AcceptChanges has not been called.
		/// </summary>
		Added		=	0,
		/// <summary>
		/// The object was deleted using the Delete method of the MetaObject.
		/// </summary>
		Deleted,
		/// <summary>
		/// The object has been modified and AcceptChanges has not been called.
		/// </summary>
		Modified,
		/// <summary>
		/// The object has not changed since AcceptChanges was last called.
		/// </summary>
		Unchanged 
	}
}
