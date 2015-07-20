using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data;
using System.Data;

namespace Mediachase.Ibn.Clients
{
	internal static  class PartnerUtil
	{
		/// <summary>
		/// Gets the client info.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <param name="orgUid">The org uid.</param>
		/// <param name="contactUid">The contact uid.</param>
		/// <returns></returns>
		public static bool GetClientInfo(int principalId, out Guid orgUid, out Guid contactUid)
		{
			orgUid = Guid.Empty;
			contactUid = Guid.Empty;

			//PartnerGetClientInfo
			using(IDataReader reader = SqlHelper.ExecuteReader(SqlContext.Current, System.Data.CommandType.StoredProcedure, "PartnerGetClientInfo",
				SqlHelper.SqlParameter("@PrincipalId", System.Data.SqlDbType.Int, principalId)))
			{
				if (reader.Read())
				{
					orgUid = (Guid)SqlHelper.DBNull2Null(reader["OrgUid"], Guid.Empty);
					contactUid = (Guid)SqlHelper.DBNull2Null(reader["ContactUid"], Guid.Empty);

					return true;
				}
			}

			return false;
		}
	}
}
