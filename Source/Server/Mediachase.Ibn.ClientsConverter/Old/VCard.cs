using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using Mediachase.Database;
using Mediachase.Ibn.Database.VCard;


namespace Mediachase.Ibn.Business.VCard
{
	/// <summary>
	/// Summary description for VCard.
	/// </summary>
	internal class VCard
	{
		private VCardRow _srcRow = null;

		//		private VCardName _nameInfo = null;
		//		//private VCardPhoto _photoInfo = null;
		//		private VCardOrganization _orgInfo = null;
		//		private VCardEMail[] _emails;
		//		private VCardAddress[] _adresses;
		//		private VCardTelephone[] _phones;

		public VCard()
		{
			_srcRow = new VCardRow();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VCard"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private VCard(VCardRow row)
		{
			_srcRow = row;

			//			// Load VCardName
			//			_nameInfo = VCardName.List(_srcRow.VCardId);
			//
			//			// Load VCardOrganization
			//			_orgInfo = VCardOrganization.List(_srcRow.VCardId);
			//
			//			// Load  VCardEMail[]
			//			_emails = VCardEMail.List(_srcRow.VCardId);
			//
			//			// Load VCardAddress[]
			//			_adresses = VCardAddress.List(_srcRow.VCardId);
			//
			//			// Load VCardTelephone[]
			//			_phones = VCardTelephone.List(_srcRow.VCardId);
		}

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns></returns>
		public static VCard Create()
		{
			return new VCard(new VCardRow());
		}

		/// <summary>
		/// Loads the specified V card id.
		/// </summary>
		/// <param name="VCardId">The V card id.</param>
		/// <returns></returns>
		public static VCard Load(int VCardId)
		{
			return new VCard(new VCardRow(VCardId));
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static VCard[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach (VCardRow row in VCardRow.List())
			{
				retVal.Add(new VCard(row));
			}

			return (VCard[])retVal.ToArray(typeof(VCard));
		}

		/// <summary>
		/// Lists the vcard by specified organization id.
		/// </summary>
		/// <param name="OrganizationId">The organization id.</param>
		/// <returns></returns>
		public static VCard[] List(int OrganizationId)
		{
			ArrayList retVal = new ArrayList();

			foreach (VCardRow row in VCardRow.List(OrganizationId))
			{
				retVal.Add(new VCard(row));
			}

			return (VCard[])retVal.ToArray(typeof(VCard));
		}

		/// <summary>
		/// Lists the specified substring.
		/// </summary>
		/// <param name="Substring">The substring.</param>
		/// <returns></returns>
		public static VCard[] List(string Substring)
		{
			ArrayList retVal = new ArrayList();

			foreach (VCardRow row in VCardRow.List(Substring))
			{
				retVal.Add(new VCard(row));
			}

			return (VCard[])retVal.ToArray(typeof(VCard));
		}


		#region Public Properties

		/// <summary>
		/// Gets the V card id.
		/// </summary>
		/// <value>The V card id.</value>
		public virtual int VCardId
		{
			get
			{
				return _srcRow.VCardId;
			}

		}

		/// <summary>
		/// Gets or sets the full name.
		/// </summary>
		/// <value>The full name.</value>
		public virtual string FullName
		{
			get
			{
				return _srcRow.FullName;
			}

			set
			{
				_srcRow.FullName = value;
			}

		}

		/// <summary>
		/// Gets or sets the name of the nick.
		/// </summary>
		/// <value>The name of the nick.</value>
		public virtual string NickName
		{
			get
			{
				return _srcRow.NickName;
			}

			set
			{
				_srcRow.NickName = value;
			}

		}

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public virtual string Url
		{
			get
			{
				return _srcRow.Url;
			}

			set
			{
				_srcRow.Url = value;
			}

		}

		/// <summary>
		/// Gets or sets the birthday.
		/// </summary>
		/// <value>The birthday.</value>
		public virtual DateTime Birthday
		{
			get
			{
				return _srcRow.Birthday;
			}

			set
			{
				_srcRow.Birthday = value;
			}

		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public virtual string Title
		{
			get
			{
				return _srcRow.Title;
			}

			set
			{
				_srcRow.Title = value;
			}

		}

		/// <summary>
		/// Gets or sets the role.
		/// </summary>
		/// <value>The role.</value>
		public virtual string Role
		{
			get
			{
				return _srcRow.Role;
			}

			set
			{
				_srcRow.Role = value;
			}

		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public virtual string Description
		{
			get
			{
				return _srcRow.Description;
			}

			set
			{
				_srcRow.Description = value;
			}

		}

		/// <summary>
		/// Gets or sets the gender.
		/// </summary>
		/// <value>The gender.</value>
		public virtual string Gender
		{
			get
			{
				return _srcRow.Gender;
			}

			set
			{
				_srcRow.Gender = value;
			}

		}

		public virtual int OrganizationId
		{
			get
			{
				return _srcRow.OrganizationId;
			}

			set
			{
				_srcRow.OrganizationId = value;
			}

		}

		#endregion

		#region Extended Properies
		//		public VCardName UserName
		//		{
		//			get 
		//			{
		//				return _nameInfo;
		//			}
		//		}

		//		public VCardPhoto Photo
		//		{
		//			get 
		//			{
		//				return _photoInfo;
		//			}
		//		}

		//		public VCardOrganization Organization
		//		{
		//			get 
		//			{
		//				return _orgInfo;
		//			}
		//		}

		//		public VCardTelephone[] Telephone
		//		{
		//			get 
		//			{
		//				return _phones;
		//			}
		//		}

		//		public VCardEMail[] EMails
		//		{
		//			get 
		//			{
		//				return _emails;
		//			}
		//		}
		//
		//		public VCardAddress[] Addresses
		//		{
		//			get 
		//			{
		//				return _adresses;
		//			}
		//		}

		#endregion
	}
}
