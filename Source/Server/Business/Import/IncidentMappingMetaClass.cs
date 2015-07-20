using System;
using System.Collections;
using System.Data;

using Mediachase.IBN.Business;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus.Import;


namespace Mediachase.IBN.Business.Import
{
	public class EmptyTitleException : Exception
	{
		public EmptyTitleException()
			: base("Title cannot be empty")
		{
		}
	}

	/// <summary>
	/// Summary description for IncidentMappingMetaClass.
	/// </summary>
	public class IncidentMappingMetaClass : MappingMetaClass
	{
		protected override void FillSystemColumnInfo(ArrayList array)
		{
			MetaClass metaClass = MetaClass.Load(InnerMetaClassName);

			array.Add(new ColumnInfo(metaClass.SystemMetaFields["Title"], FillTypes.CopyValue | FillTypes.Custom));
			array.Add(new ColumnInfo(metaClass.SystemMetaFields["Description"], FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default));
			array.Add(new ColumnInfo(metaClass.SystemMetaFields["CreationDate"], FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default));

			array.Add(new ColumnInfo(MetaField.CreateVirtual(metaClass.Namespace, "Priority", "Priority", "Priority",
										MetaDataType.ShortString, 255,
										true, false, false, false), FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default, true));

			array.Add(new ColumnInfo(MetaField.CreateVirtual(metaClass.Namespace, "Type", metaClass.SystemMetaFields["TypeId"].FriendlyName, "Type",
										MetaDataType.ShortString, 255,
										true, false, false, false), FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default, true));

			array.Add(new ColumnInfo(MetaField.CreateVirtual(metaClass.Namespace, "Severity", "SeverityId", "SeverityId",
										MetaDataType.ShortString, 255,
										true, false, false, false), FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default, true));

			array.Add(new ColumnInfo(metaClass.SystemMetaFields["Resolution"], FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default));
			array.Add(new ColumnInfo(metaClass.SystemMetaFields["Workaround"], FillTypes.CopyValue | FillTypes.Custom | FillTypes.Default));
		}

		private int GetPriorityId(string name)
		{
			using (IDataReader reader = DBCommon.GetListPriorities(0))
			{
				while (reader.Read())
				{
					if (String.Compare(name, (string)reader["PriorityName"], true) == 0)
					{
						return (int)reader["PriorityId"];
					}
				}
			}
			throw new Mediachase.MetaDataPlus.Import.InvalidValue(name);
		}

		private int GetTypeId(string name)
		{
			using (IDataReader reader = Incident.GetListIncidentTypes())
			{
				while (reader.Read())
				{
					if (String.Compare(name, (string)reader["TypeName"], true) == 0)
					{
						return (int)reader["TypeId"];
					}
				}
			}
			throw new Mediachase.MetaDataPlus.Import.InvalidValue(name);
		}

		private int GetSeverityId(string name)
		{
			using (IDataReader reader = Incident.GetListIncidentSeverity())
			{
				while (reader.Read())
				{
					if (String.Compare(name, (string)reader["SeverityName"], true) == 0)
					{
						return (int)reader["SeverityId"];
					}
				}
			}
			throw new Mediachase.MetaDataPlus.Import.InvalidValue(name);
		}

		protected override int CreateSystemRow(FillDataMode mode, params object[] item)
		{
			if (mode == FillDataMode.Update)
			{
				throw new NotSupportedException("Update is not supported for Incident import");
			}
			string Description = "";
			string Resolution = "";
			string Workaround = "";
			string Title = "Imported incident " + DateTime.Now.ToString("d");
			DateTime CreationDate = DateTime.UtcNow;
			int PriorityId = int.Parse(PortalConfig.IncidentDefaultValuePriorityField);
			int TypeId = int.Parse(PortalConfig.IncidentDefaultValueTypeField);
			int SeverityId = int.Parse(PortalConfig.IncidentDefaultValueSeverityField); ;

			if (item[0] != null) Title = (string)item[0];

			if (item[2] != null) CreationDate = DBCommon.GetUTCDate(Security.CurrentUser.TimeZoneId, (DateTime)item[2]);
			if (item[1] != null) Description = (string)item[1];
			if (item[6] != null) Resolution = (string)item[6];
			if (item[7] != null) Workaround = item[7].ToString();
			if (item[3] != null) PriorityId = GetPriorityId((string)item[3]);
			if (item[4] != null) TypeId = GetTypeId((string)item[4]);
			if (item[5] != null) SeverityId = GetSeverityId((string)item[5]);

			int IncidentId = Incident.Create(Title, Description, TypeId, PriorityId, SeverityId, Security.CurrentUser.UserID, CreationDate);
			//Issue2.UpdateResolutionInfo(IncidentId, Resolution, Workaround);

			return IncidentId;
		}

		public FillResult FillData(FillDataMode mode, DataTable rawData, Mediachase.MetaDataPlus.Import.Rule rule, int maximumErrors)
		{
			using (DbTransaction tran = DbTransaction.Begin())
			{
				FillResult retVal = FillData(mode, rawData, rule, Security.CurrentUser.UserID, DateTime.UtcNow, DbContext.Current.Transaction);
				if (maximumErrors == -1 || retVal.ErrorRows <= maximumErrors)
					tran.Commit();
				else tran.Rollback();

				return retVal;
			}
		}

		public IncidentMappingMetaClass()
		{
			InnerMetaClassName = "IncidentsEx";
		}
	}
}
