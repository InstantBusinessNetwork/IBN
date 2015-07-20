using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mediachase.Ibn.Data.Sql;
using System.Data;
using Mediachase.Ibn.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Business.ReportSystem
{
	public class ReportManager
	{
		/// <summary>
		/// Gets the filter controls.
		/// </summary>
		/// <param name="directoryPath">The directory path.</param>
		/// <returns></returns>
		public static string[] GetFilterControls(string directoryPath)
		{
			List<string> list = new List<string>();

			if (Directory.Exists(directoryPath))
			{
				foreach (string filePath in Directory.GetFiles(directoryPath, "*.ascx"))
				{
					string name = Path.GetFileNameWithoutExtension(filePath);
					if (!list.Contains(name))
					{
						list.Add(name);
					}
				}
			}

			return list.ToArray();
		}

		/// <summary>
		/// Generates the RDL.
		/// </summary>
		/// <param name="reportTemplateXml">The report template XML.</param>
		/// <returns></returns>
		public static string GenerateRdl(string reportTemplateXml)
		{
			return null;
		}

		#region CanRead
		/// <summary>
		/// Determines whether this instance can read the specified report id.
		/// </summary>
		/// <param name="reportId">The report id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can read the specified report id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanRead(PrimaryKeyId reportId)
		{
			return CanRead(Security.CurrentUser.UserID);
		}

		/// <summary>
		/// Determines whether this instance can read the specified report id.
		/// </summary>
		/// <param name="reportId">The report id.</param>
		/// <param name="userId">The user id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can read the specified report id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanRead(PrimaryKeyId reportId, int userId)
		{
			SqlParameter retVal = SqlHelper.CreateRetvalSqlParameter();

			SqlHelper.ExecuteNonQuery(SqlContext.Current,
				CommandType.StoredProcedure, "mc_mcweb_ReportAceCanUserRead",
				SqlHelper.SqlParameter("@UserId", SqlDbType.Int, userId),
				SqlHelper.SqlParameter("@ReportId", SqlDbType.UniqueIdentifier, reportId),
				retVal);

			return ((int)retVal.Value) == 1;
		}
		#endregion

	}
}
