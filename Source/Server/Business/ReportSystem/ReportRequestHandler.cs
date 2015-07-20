using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus.Configurator;


namespace Mediachase.IBN.Business.ReportSystem
{
	/// <summary>
	/// Represents Report Request Handler.
	/// </summary>
	public class ReportRequestHandler: BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ReportRequestHandler"/> class.
		/// </summary>
		public ReportRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == ReportEntity.GetAssignedMetaClassName())
			{
				ReportEntity retVal = new ReportEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region ddd
		protected override void PreExecute(BusinessContext context)
		{
			base.PreExecute(context);

			if (context.Request!=null && 
				context.Request.Target!=null && 
				context.Request.Target.PrimaryKeyId.HasValue)
			{
				// OZ: 2008-11-26 Check Security
				if (!Security.IsUserInGroup(Security.CurrentUser.UserID, InternalSecureGroups.Administrator) &&
					!ReportManager.CanRead(context.Request.Target.PrimaryKeyId.Value, Security.CurrentUser.UserID))
					throw new AccessDeniedException();
			}
		}
		#endregion


		#region Create
		/// <summary>
		/// PreCreate
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCreate(BusinessContext context)
		{
			ReportEntity report = context.Request.Target as ReportEntity;
			if (report != null && !string.IsNullOrEmpty(report.RdlText))
			{
				SqlReport.DemandCustomSqlReportAccess();
			}

			base.PreCreate(context);
		} 
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			// Delete ACL
			foreach (mcweb_ReportAceRow row in mcweb_ReportAceRow.List(
				FilterElement.EqualElement(mcweb_ReportAceRow.Columns.ReportId, (Guid)context.Request.Target.PrimaryKeyId.Value)))
			{
				row.Delete();
			}
		}
		#endregion

		#region List
		protected override void PostListInsideTransaction(BusinessContext context)
		{
			base.PostListInsideTransaction(context);

			if (!Security.IsUserInGroup(Security.CurrentUser.UserID, InternalSecureGroups.Administrator))
			{
				ListResponse response = (ListResponse)context.Response;

				List<EntityObject> reports = new List<EntityObject>(response.EntityObjects);

				// Check User Can Read Report
				for (int index = reports.Count - 1; index >= 0; index--)
				{
					if (!ReportManager.CanRead(reports[index].PrimaryKeyId.Value, Security.CurrentUser.UserID))
					{
						reports.RemoveAt(index);
					}
				}

				// Update respionse
				if(response.EntityObjects.Length != reports.Count)
					response.EntityObjects = reports.ToArray();
			}
		}
		#endregion

		#region CustomMethod
		protected override void PreCustomMethod(BusinessContext context)
		{
			base.PreCustomMethod(context);
		}

		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PreCustomMethodInsideTransaction(context);
		}

		protected override void CustomMethod(BusinessContext context)
		{
			switch (context.Request.Method)
			{
				case ReportRequestMethod.GetReportData:
					context.SetResponse(GetReportData(context.Request));
					break;
				default:
					base.CustomMethod(context);
					break;
			}
		}

		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PostCustomMethodInsideTransaction(context);
		}

		protected override void PostCustomMethod(BusinessContext context)
		{
			base.PostCustomMethod(context);
		}
		#endregion

		#endregion

		private static ReportDataResponse GetReportData(Request request)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			// OZ: 2008-11-26 Check Security
			if (!Security.IsUserInGroup(Security.CurrentUser.UserID, InternalSecureGroups.Administrator) &&
				!ReportManager.CanRead(request.Target.PrimaryKeyId.Value, Security.CurrentUser.UserID))
				throw new AccessDeniedException();

			ReportDataResponse response = null;

			string filter = null;
			RequestParameter parameter = request.Parameters[ReportDataRequest.FilterParameterName];
			if (parameter != null)
				filter = parameter.Value as string;

			ReportEntity report = request.Target as ReportEntity;
			if (report != null)
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(report.RdlText);

				XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
				namespaceManager.AddNamespace("ns", "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition");

				List<DataTable> data = new List<DataTable>();

				foreach (XmlNode dataSetNode in doc.SelectNodes("/ns:Report/ns:DataSets/ns:DataSet", namespaceManager))
				{
					DataTable table = LoadDataSet(namespaceManager, dataSetNode, filter);
					data.Add(table);
				}

				response = new ReportDataResponse(data);
			}

			return response;
		}

		private static DataTable LoadDataSet(XmlNamespaceManager namespaceManager, XmlNode dataSetNode, string filter)
		{
			string dataSetName = dataSetNode.Attributes["Name"].Value;
			DataTable table = new DataTable(dataSetName);

			Dictionary<string, string> names = new Dictionary<string, string>();

			// Create columns
			foreach (XmlNode fieldNode in dataSetNode.SelectNodes("ns:Fields/ns:Field", namespaceManager))
			{
				string fieldName = fieldNode.Attributes["Name"].Value;
				string dataFieldName = fieldNode.SelectSingleNode("ns:DataField", namespaceManager).InnerText;

				names.Add(fieldName, dataFieldName);
				table.Columns.Add(fieldName);
			}

			string query = dataSetNode.SelectSingleNode("ns:Query/ns:CommandText", namespaceManager).InnerText;

			if (!string.IsNullOrEmpty(query))
			{
				// Apply filter
				if (!string.IsNullOrEmpty(filter) && dataSetName == "Filterable")
				{
					int i = query.IndexOf("] AS IBN_Filtered");
					if (i > 0)
					{
						int j = query.LastIndexOf('[', i - 1, i);
						if (j > 0)
						{
							query = string.Concat(query.Substring(0, j)
								, "(SELECT * FROM "
								, query.Substring(j, i - j + 1)
								, " WHERE ("
								, filter
								, ")) AS IBN_Filtered"
								);
						}
					}
				}

				using (SqlConnection connection = new SqlConnection(DbContext.Current.PortalConnectionString))
				{
					connection.Open();
					using (SqlTransaction transaction = connection.BeginTransaction())
					{
						try
						{
							// Write user id to SQL context
							UserLight currentUser = Security.CurrentUser;
							if (currentUser != null)
							{
								SetContextInfo(connection, transaction, currentUser.UserID);
							}

							// Execute query
							SqlCommand command = new SqlCommand(query, connection, transaction);
							command.CommandType = CommandType.Text;
							using (IDataReader reader = command.ExecuteReader(CommandBehavior.Default))
							{
								while (reader.Read())
								{
									DataRow row = table.NewRow();

									foreach (string fieldName in names.Keys)
									{
										string dataFieldName = names[fieldName];
										object value = reader[dataFieldName];

										if (value is DateTime)
										{
											if (currentUser != null)
											{
												// Convert time from UTC to local.
												DateTime dateValue = (DateTime)value;
												value = currentUser.CurrentTimeZone.ToLocalTime(dateValue);
											}
										}
										else
										{
											string stringValue = value as string;
											if (!string.IsNullOrEmpty(stringValue) && dataFieldName.EndsWith("Friendly"))
											{
												value = Common.GetWebResourceString(value as string);
											}
										}

										row[fieldName] = value;
									}

									table.Rows.Add(row);
								}
							}
						}
						finally
						{
							transaction.Rollback();
						}
					}
				}
			}

			return table;
		}

		private static void SetContextInfo(SqlConnection connection, SqlTransaction transaction, int userId)
		{
			SqlCommand command = new SqlCommand("DECLARE @BinaryId BINARY(128) SET @BinaryId = CAST(@UserId AS BINARY(128)) SET CONTEXT_INFO @BinaryId", connection, transaction);
			command.CommandType = CommandType.Text;
			command.Parameters.Add(new SqlParameter("@UserId", userId));
			command.ExecuteNonQuery();
		}
	}
}
