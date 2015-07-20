using System;
using System.Collections;

using Mediachase.Ibn.Database;
using Mediachase.Ibn.Database.VCard;


namespace Mediachase.Ibn.Business.VCard
{
	[Flags]
	internal enum VCardAddressTypes
	{
		None = 0,
		Home = 1,
		Work = 2
	}

	/// <summary>
	/// Summary description for VCardAddress.
	/// </summary>
	internal class VCardAddress
	{
		private VCardAddressRow _srcRow = null;

		public VCardAddress()
		{
			_srcRow = new VCardAddressRow();
		}

		private VCardAddress(VCardAddressRow row)
		{
			_srcRow = row;
		}

		public static VCardAddress[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			foreach (VCardAddressRow item in VCardAddressRow.List(VCardId))
			{
				retVal.Add(new VCardAddress(item));
			}

			return (VCardAddress[])retVal.ToArray(typeof(VCardAddress));
		}

		public static int Create(int VCardId, VCardAddressTypes type,
			string Country, string PostalCode, string Region, string Locality, string Street, string ExtraAddress,
			bool IsPrefered)
		{
			VCardAddressRow row = new VCardAddressRow();

			row.VCardId = VCardId;
			row.VCardAddressTypeId = (int)type;
			row.IsPrefered = IsPrefered;
			row.Country = Country;
			row.PostalCode = PostalCode;
			row.Region = Region;
			row.Locality = Locality;
			row.Street = Street;
			row.ExtraAddress = ExtraAddress;

			row.Update();

			return row.PrimaryKeyId;
		}

		public static void Update(VCardAddress item)
		{
			item._srcRow.Update();
		}

		public static void Delete(int VCardAdressId)
		{
			VCardAddressRow.Delete(VCardAdressId);
		}

		#region Public Properties

		public virtual int VCardAddressId
		{
			get
			{
				return _srcRow.VCardAddressId;
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

		public virtual VCardAddressTypes AddressType
		{
			get
			{
				return (VCardAddressTypes)_srcRow.VCardAddressTypeId;
			}

			set
			{
				_srcRow.VCardAddressTypeId = (int)value;
			}

		}

		public virtual bool IsPrefered
		{
			get
			{
				return _srcRow.IsPrefered;
			}

			set
			{
				_srcRow.IsPrefered = value;
			}

		}

		public virtual string ExtraAddress
		{
			get
			{
				return _srcRow.ExtraAddress;
			}

			set
			{
				_srcRow.ExtraAddress = value;
			}

		}

		public virtual string Street
		{
			get
			{
				return _srcRow.Street;
			}

			set
			{
				_srcRow.Street = value;
			}

		}

		public virtual string Locality
		{
			get
			{
				return _srcRow.Locality;
			}

			set
			{
				_srcRow.Locality = value;
			}

		}

		public virtual string Region
		{
			get
			{
				return _srcRow.Region;
			}

			set
			{
				_srcRow.Region = value;
			}

		}

		public virtual string PostalCode
		{
			get
			{
				return _srcRow.PostalCode;
			}

			set
			{
				_srcRow.PostalCode = value;
			}

		}

		public virtual string Country
		{
			get
			{
				return _srcRow.Country;
			}

			set
			{
				_srcRow.Country = value;
			}

		}

		#endregion
	}
}
