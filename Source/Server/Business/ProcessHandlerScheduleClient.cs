using System;
using System.Collections;
using System.Data;
using System.Diagnostics;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;
using Mediachase.Schedule;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ScheduleProcessHandler.
	/// </summary>
	public class ProcessHandlerScheduleClient : MarshalByRefObject, IScheduleClient
	{
		private class ProcessHandlerTempItems
		{
			public int Id;
			public int HandlerId;
			public string Params;
			public int HookId;
			public int? ObjectId;
			public Guid? ObjectUid;
			public DateTime Dt;
			public DateTime DateValue;
			public int Lag;

			public ProcessHandlerTempItems(int Id, int HandlerId, string Params, int HookId, int? ObjectId, Guid? ObjectUid, DateTime Dt, DateTime DateValue, int Lag)
			{
				this.Id = Id;
				this.HandlerId = HandlerId;
				this.Params = Params;
				this.HookId = HookId;
				this.ObjectId = ObjectId;
				this.ObjectUid = ObjectUid;
				this.Dt = Dt;
				this.DateValue = DateValue;
				this.Lag = Lag;
			}
		}

		public ProcessHandlerScheduleClient()
		{
		}

		public void InternalInvoke(string applicationName)
		{
			if (applicationName == IbnConst.ProductName)
			{
				UserLight prevUser, curUser;
				prevUser = Security.SetCurrentUser("alert", out curUser);

				try
				{
					//DateTime from = DateTime.UtcNow.AddHours(-1);
					//DateTime to = DateTime.UtcNow;

					ArrayList items = new ArrayList();

					using (IDataReader reader = DbSchedule2.FillScheduleProcessHandlerTempItems())
					{
						// ValueId, DateTypeName, ObjectTypeId, ObjectId, Dt, HookId, HandlerId, Params, DateValue, Lag
						while (reader.Read())
						{
							items.Add(new ProcessHandlerTempItems(
								(int)reader["ValueId"],
								(int)reader["HandlerId"],
								(string)reader["Params"],
								(int)reader["HookId"],
								reader["ObjectId"] == DBNull.Value ? null : (int?)reader["ObjectId"],
								reader["ObjectUid"] == DBNull.Value ? null : (Guid?)reader["ObjectUid"],
								(DateTime)reader["Dt"],
								(DateTime)reader["DateValue"],
								(int)reader["Lag"]));
						}
					}

					foreach (ProcessHandlerTempItems item in items)
					{
						try
						{
							Schedule.ProcessHandler(item.HandlerId, item.Params, item.HookId, item.ObjectId, item.ObjectUid, item.DateValue);
							DbSchedule2.AddProcessedDateType(item.Id, item.HookId, item.DateValue, item.Lag);
							//DbSchedule2.DeleteDateTypeValue(item.Id);
						}
						catch (Exception ex)
						{
							System.Diagnostics.Trace.WriteLine(ex);
							Log.WriteError(ex.ToString());
						}
					}
				}
				finally
				{
					Security.SetCurrentUser(prevUser);
				}
			}
		}

		#region IScheduleClient Members

		public void Invoke(string applicationName)
		{
			if (applicationName == IbnConst.ProductName)
			{
				DbHelper2.Init();

				string[] list = Company.GetApplicationIdsForScheduleService();

				foreach (string appId in list)
				{
					try
					{
						GlobalContext.Current = new GlobalContext(System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"..\Apps"));
						//Configuration.Init2(appId);
						DataContext.Current = new DataContext(Mediachase.IBN.Database.DbContext.Current.PortalConnectionString);

						InternalInvoke(applicationName);
					}
					catch (Exception e)
					{
						string msg = string.Format("Invoke() failed.\nAPP_ID: {0}\nException: {1}", appId, e.ToString());
						Log.WriteError(msg);
					}
				}
			}
		}

		#endregion
	}
}
