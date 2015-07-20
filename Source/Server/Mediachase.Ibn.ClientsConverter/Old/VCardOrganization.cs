using System;
using System.Collections;

using Mediachase.Ibn.Database;
using Mediachase.Ibn.Database.VCard;


namespace Mediachase.Ibn.Business.VCard
{
	/// <summary>
	/// Summary description for VCardOrganization.
	/// </summary>
	internal class VCardOrganization
	{
		private VCardOrganizationRow _srcRow = null;

		public VCardOrganization()
		{
			_srcRow = new VCardOrganizationRow();
		}

		private VCardOrganization(VCardOrganizationRow row)
		{
			_srcRow = row;
		}

		public static VCardOrganization List(int VCardId)
		{
			VCardOrganizationRow[] items = VCardOrganizationRow.List(VCardId);
			if (items.Length > 0)
				return new VCardOrganization(items[0]);
			return null;
		}

		#region Public Properties

		public virtual int VCardOrganizationId
		{
			get
			{
				return _srcRow.VCardOrganizationId;
			}

		}

		public virtual int VCardId
		{
			get
			{
				return _srcRow.VCardId;
			}

			set
			{
				_srcRow.VCardId = value;
			}

		}

		public virtual string Name
		{
			get
			{
				return _srcRow.Name;
			}

			set
			{
				_srcRow.Name = value;
			}

		}

		public virtual string Unit
		{
			get
			{
				return _srcRow.Unit;
			}

			set
			{
				_srcRow.Unit = value;
			}

		}

		#endregion
	}
}
