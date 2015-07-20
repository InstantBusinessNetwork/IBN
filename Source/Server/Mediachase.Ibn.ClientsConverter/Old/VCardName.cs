using System;
using System.Collections;

using Mediachase.Ibn.Database;
using Mediachase.Ibn.Database.VCard;


namespace Mediachase.Ibn.Business.VCard
{
	/// <summary>
	/// Summary description for VCardName.
	/// </summary>
	internal class VCardName
	{
		private VCardNameRow _srcRow = null;

		public VCardName()
		{
			_srcRow = new VCardNameRow();
		}

		private VCardName(VCardNameRow row)
		{
			_srcRow = row;
		}

		public static VCardName List(int VCardId)
		{
			VCardNameRow[] items = VCardNameRow.List(VCardId);
			if (items.Length > 0)
				return new VCardName(items[0]);
			return null;
		}

		#region Public Properties

		public virtual int VCardNameId
		{
			get
			{
				return _srcRow.VCardNameId;
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

		public virtual string Family
		{
			get
			{
				return _srcRow.Family;
			}

			set
			{
				_srcRow.Family = value;
			}

		}

		public virtual string Given
		{
			get
			{
				return _srcRow.Given;
			}
			set
			{
				_srcRow.Given = value;
			}

		}

		public virtual string Middle
		{
			get
			{
				return _srcRow.Middle;
			}

			set
			{
				_srcRow.Middle = value;
			}

		}

		#endregion
	}
}
