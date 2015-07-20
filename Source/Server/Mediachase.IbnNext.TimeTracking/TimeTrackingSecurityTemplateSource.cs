using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using System.Data;
using System.Data.SqlClient;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IbnNext.TimeTracking
{
	public class TimeTrackingSecurityTemplateSource : ITemplateSource
	{
		public virtual object GetValue(string key)
		{
			switch (key)
			{
				case "CanRead":
					if (DataHelper.UseSpOptimization)
					{
						string currentUserAllGroup = Security.GetPrincipalGroups(Security.CurrentUserId);

						SelectCommandBuilderParameter value = new SelectCommandBuilderParameter();
						// Declare SP Prefix @Table
						// Declare SP Parameter

						value.Parameters.Add(new SqlParameter("@PrincipalGroups",currentUserAllGroup));
						value.Prefix = "DECLARE @PrincipalGroupsTable TABLE (PrincipalId int)" + "\r\n" +
							"INSERT INTO @PrincipalGroupsTable (PrincipalId) SELECT Item FROM Split(@PrincipalGroups)\r\n\r\n";


						StringBuilder sbFilterValue = new StringBuilder();

						sbFilterValue.AppendFormat(@"SELECT [ttb].TimeTrackingBlockId{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"FROM [cls_TimeTrackingBlock] AS [ttb]{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"WHERE ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            ttb.TimeTrackingBlockId {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"	  IN{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                SELECT ObjectId FROM cls_TimeTrackingBlock_ReadACL{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                  WHERE PrincipalId IN ({1}) AND [Read] = 2{0}", Environment.NewLine, "SELECT PrincipalId FROM @PrincipalGroupsTable");
						sbFilterValue.AppendFormat(@"            ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            OR{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            EXISTS {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                SELECT * {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                  FROM cls_TimeTrackingBlock_GlobalACL G{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    LEFT JOIN cls_TimeTrackingBlock_GlobalACL_State S ON ");
						sbFilterValue.AppendFormat(@"(G.TimeTrackingBlock_GlobalACLId = S.GlobalACLId AND S.StateMachineId = [ttb].mc_StateMachineId AND ");
						sbFilterValue.AppendFormat(@"S.StateId = [ttb].mc_StateId){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                  WHERE ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                            (S.[Read] IS NULL AND G.[Read] = 2){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                            OR {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                            S.[Read] = 2{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    AND PrincipalId IN ({1}){0}", Environment.NewLine, "SELECT PrincipalId FROM @PrincipalGroupsTable");
						sbFilterValue.AppendFormat(@"            ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        AND ttb.TimeTrackingBlockId NOT IN{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            SELECT ObjectId FROM cls_TimeTrackingBlock_ReadACL{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                WHERE {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    PrincipalId IN ({1}) AND [Read] = 3{0}", Environment.NewLine, "SELECT PrincipalId FROM @PrincipalGroupsTable");
						sbFilterValue.AppendFormat(@"        ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        AND NOT EXISTS {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            SELECT *{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"              FROM cls_TimeTrackingBlock_GlobalACL G{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    LEFT JOIN cls_TimeTrackingBlock_GlobalACL_State S ON ");
						sbFilterValue.AppendFormat(@"(G.TimeTrackingBlock_GlobalACLId = S.GlobalACLId AND S.StateMachineId = [ttb].mc_StateMachineId AND ");
						sbFilterValue.AppendFormat(@"S.StateId = [ttb].mc_StateId){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"              WHERE ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        (S.[Read] IS NULL AND G.[Read] = 3){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        OR {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        S.[Read] = 3{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    ) {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                AND PrincipalId IN ({1}){0}", Environment.NewLine, "SELECT PrincipalId FROM @PrincipalGroupsTable");
						sbFilterValue.AppendFormat(@"        ){0}", Environment.NewLine);

						value.Query = sbFilterValue.ToString();

						return value;
					}
					else
					{
						string currentUserAllGroup = Security.GetPrincipalGroups(Security.CurrentUserId);

						StringBuilder sbFilterValue = new StringBuilder();

						sbFilterValue.AppendFormat(@"SELECT [ttb].TimeTrackingBlockId{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"FROM [cls_TimeTrackingBlock] AS [ttb]{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"WHERE ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            ttb.TimeTrackingBlockId {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"	  IN{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                SELECT ObjectId FROM cls_TimeTrackingBlock_ReadACL{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                  WHERE PrincipalId IN ({1}) AND [Read] = 2{0}", Environment.NewLine, currentUserAllGroup);
						sbFilterValue.AppendFormat(@"            ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            OR{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            EXISTS {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                SELECT * {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                  FROM cls_TimeTrackingBlock_GlobalACL G{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    LEFT JOIN cls_TimeTrackingBlock_GlobalACL_State S ON ");
						sbFilterValue.AppendFormat(@"(G.TimeTrackingBlock_GlobalACLId = S.GlobalACLId AND S.StateMachineId = [ttb].mc_StateMachineId AND ");
						sbFilterValue.AppendFormat(@"S.StateId = [ttb].mc_StateId){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                  WHERE ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                            (S.[Read] IS NULL AND G.[Read] = 2){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                            OR {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                            S.[Read] = 2{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    AND PrincipalId IN ({1}){0}", Environment.NewLine, currentUserAllGroup);
						sbFilterValue.AppendFormat(@"            ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        AND ttb.TimeTrackingBlockId NOT IN{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            SELECT ObjectId FROM cls_TimeTrackingBlock_ReadACL{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                WHERE {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    PrincipalId IN ({1}) AND [Read] = 3{0}", Environment.NewLine, currentUserAllGroup);
						sbFilterValue.AppendFormat(@"        ){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        AND NOT EXISTS {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"        ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"            SELECT *{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"              FROM cls_TimeTrackingBlock_GlobalACL G{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    LEFT JOIN cls_TimeTrackingBlock_GlobalACL_State S ON ");
						sbFilterValue.AppendFormat(@"(G.TimeTrackingBlock_GlobalACLId = S.GlobalACLId AND S.StateMachineId = [ttb].mc_StateMachineId AND ");
						sbFilterValue.AppendFormat(@"S.StateId = [ttb].mc_StateId){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"              WHERE ({0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        (S.[Read] IS NULL AND G.[Read] = 3){0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        OR {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                        S.[Read] = 3{0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                    ) {0}", Environment.NewLine);
						sbFilterValue.AppendFormat(@"                AND PrincipalId IN ({1}){0}", Environment.NewLine, currentUserAllGroup);
						sbFilterValue.AppendFormat(@"        ){0}", Environment.NewLine);

						return sbFilterValue.ToString();
					}
			}

			return string.Empty;
		}

		#region ITemplateSource Members

		object ITemplateSource.GetValue(string key)
		{
			return this.GetValue(key);
		}

		#endregion
	}

}
