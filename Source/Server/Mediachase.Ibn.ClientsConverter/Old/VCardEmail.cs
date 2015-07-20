using System;
using System.Collections;

using Mediachase.Ibn.Database;
using Mediachase.Ibn.Database.VCard;


namespace Mediachase.Ibn.Business.VCard
{
	[Flags]
	internal enum VCardEmailTypes
	{
		None = 0,
		Home = 1,
		Work = 2,
		Internet = 4,
		X400 = 8
	}

	/// <summary>
	/// Summary description for VCardEMail.
	/// </summary>
	internal class VCardEmail
	{
		private VCardEmailRow _srcRow = null;

		public VCardEmail()
		{
			_srcRow = new VCardEmailRow();
		}

		private VCardEmail(VCardEmailRow row)
		{
			_srcRow = row;
		}

		public static VCardEmail[] List(int VCardId)
		{
			ArrayList retVal = new ArrayList();

			foreach (VCardEmailRow item in VCardEmailRow.List(VCardId))
			{
				retVal.Add(new VCardEmail(item));
			}

			return (VCardEmail[])retVal.ToArray(typeof(VCardEmail));
		}

		public static int Create(int VCardId, VCardAddressTypes type, string UserId, bool IsPrefered)
		{
			VCardEmailRow row = new VCardEmailRow();

			row.VCardId = VCardId;
			row.IsPrefered = IsPrefered;
			row.EmailTypeId = (int)type;
			row.UserId = UserId;

			row.Update();

			return row.PrimaryKeyId;
		}

		public static void Update(VCardEmail item)
		{
			item._srcRow.Update();
		}

		public static void Delete(int VCardEMailId)
		{
			VCardEmailRow.Delete(VCardEMailId);
		}

		#region Public Properties

		public virtual int VCardEMailId
		{
			get
			{
				return _srcRow.VCardEMailId;
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

		public virtual string Value
		{
			get
			{
				return _srcRow.UserId;
			}

			set
			{
				_srcRow.UserId = value;
			}

		}

		public virtual VCardEmailTypes EmailType
		{
			get
			{
				return (VCardEmailTypes)_srcRow.EmailTypeId;
			}

			set
			{
				_srcRow.EmailTypeId = (int)value;
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

		#endregion
	}
}
