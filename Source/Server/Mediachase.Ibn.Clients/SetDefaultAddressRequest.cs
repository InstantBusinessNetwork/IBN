using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Clients
{
	/// <summary>
	/// Represents set default address request.
	/// </summary>
	public class SetDefaultAddressRequest: UpdateRequest
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="SetDefaultAddressRequest"/> class.
		/// </summary>
		public SetDefaultAddressRequest()
		{
			this.Method = AddressRequestMethod.SetDefaultAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SetDefaultAddressRequest"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public SetDefaultAddressRequest(EntityObject entity): base(entity)
		{
			this.Method = AddressRequestMethod.SetDefaultAddress;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SetDefaultAddressRequest"/> class.
		/// </summary>
		/// <param name="primaryKeyId">The primary key id.</param>
		public SetDefaultAddressRequest(PrimaryKeyId primaryKeyId)
			: base(new AddressEntity(primaryKeyId))
		{
			this.Method = AddressRequestMethod.SetDefaultAddress;
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion

		
	}
}
