using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.IbnNext.TimeTracking
{
	partial class Principal
	{
		public const string PrincipalTypeEnum = "PrincipalType";

		#region CanAddChildPrincipal
		/// <summary>
		/// Determines whether this instance [can add child principal] the specified principal.
		/// </summary>
		/// <param name="principal">The principal.</param>
		/// <returns></returns>
		public static string[] CanAddChildPrincipal(Principal principal)
		{
			switch (principal.Card)
			{
				case PrincipalCardName.User:
					return new string[] { };
				case PrincipalCardName.Everyone:
					return new string[] { };
				case PrincipalCardName.MainCompany:
					return new string[] { };
				case PrincipalCardName.Partners:
					return new string[] { };
				case PrincipalCardName.PartnerCompany:
					return new string[] { };
				case PrincipalCardName.Department:
					return new string[] { };
				case PrincipalCardName.Roles:
					return new string[] { };
				case PrincipalCardName.Role:
					return new string[] { };
				case PrincipalCardName.Teams:
					return new string[] { };
				case PrincipalCardName.Team:
					return new string[] { };
				case PrincipalCardName.Restricted:
					return new string[] { };
			}

			return new string[] { };
		}

		/// <summary>
		/// Determines whether this instance [can add child principal] the specified principal id.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		public static string[] CanAddChildPrincipal(int principalId)
		{
			return CanAddChildPrincipal(new Principal(principalId));
		}
		#endregion

		#region GetPrincipalTypes
		public static MetaEnumItem[] GetPrincipalTypes()
		{
			return MetaEnum.GetItems(DataContext.Current.MetaModel.RegisteredTypes[PrincipalTypeEnum]);
		}
		#endregion

		#region GetPrincipalTypeFriendlyName
		public static string GetPrincipalTypeFriendlyName(int id)
		{
			MetaFieldType type = DataContext.Current.MetaModel.RegisteredTypes[PrincipalTypeEnum];
			return MetaEnum.GetFriendlyName(type, id);
		}
		#endregion
	}
}
