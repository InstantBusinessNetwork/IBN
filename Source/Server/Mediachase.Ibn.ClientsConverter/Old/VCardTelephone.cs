using System;
using System.Collections;

using Mediachase.Ibn.Database;
using Mediachase.Ibn.Database.VCard;


namespace Mediachase.Ibn.Business.VCard
{
	[Flags]
	internal enum VCardTelephoneTypes
	{
		None = 0,
		Home = 1,
		Work = 2,
		Pager = 4,
		Msg = 8,
		Cell = 16,
		Video = 32,
		Bbs = 64,
		Modem = 128,
		Isdn = 256,
		Pcs = 512,
		Pref = 1024,
		Voice = 2048,
		Fax = 4096,
	}

	/// <summary>
	/// Summary description for VCardTelephone.
	/// </summary>
	internal class VCardTelephone
	{
		private VCardTelephoneRow _srcRow = null;

		public VCardTelephone()
		{
			_srcRow = new VCardTelephoneRow();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VCardTelephone"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private VCardTelephone(VCardTelephoneRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Lists the specified V card id.
		/// </summary>
		/// <param name="VCardId">The V card id.</param>
		/// <returns></returns>
		public static VCardTelephone[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			foreach (VCardTelephoneRow item in VCardTelephoneRow.List(VCardId))
			{
				retVal.Add(new VCardTelephone(item));
			}

			return (VCardTelephone[])retVal.ToArray(typeof(VCardTelephone));
		}

		/// <summary>
		/// Creates the specified V card id.
		/// </summary>
		/// <param name="VCardId">The V card id.</param>
		/// <param name="Type">The type.</param>
		/// <param name="Number">The number.</param>
		/// <returns></returns>
		public static int Create(int VCardId, VCardTelephoneTypes Type, string Number)
		{
			VCardTelephoneRow row = new VCardTelephoneRow();

			row.VCardId = VCardId;
			row.TelephoneTypeId = (int)Type;
			row.Number = Number;

			row.Update();

			return row.PrimaryKeyId;
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public static void Update(VCardTelephone item)
		{
			item._srcRow.Update();
		}

		/// <summary>
		/// Deletes the specified V card telephone id.
		/// </summary>
		/// <param name="VCardTelephoneId">The V card telephone id.</param>
		public static void Delete(int VCardTelephoneId)
		{
			VCardTelephoneRow.Delete(VCardTelephoneId);
		}

		#region Public Properties


		public virtual int VCardTelephoneId
		{
			get
			{
				return _srcRow.VCardTelephoneId;
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

		public virtual string Number
		{
			get
			{
				return _srcRow.Number;
			}

			set
			{
				_srcRow.Number = value;
			}

		}

		public virtual VCardTelephoneTypes TelephoneType
		{
			get
			{
				return (VCardTelephoneTypes)_srcRow.TelephoneTypeId;
			}

			set
			{
				_srcRow.TelephoneTypeId = (int)value;
			}

		}

		#endregion
	}
}
