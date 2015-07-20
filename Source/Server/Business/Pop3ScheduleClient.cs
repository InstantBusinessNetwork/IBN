using System;
using System.Collections;
using System.Data;
using System.Diagnostics;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.Schedule;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Pop3ScheduleClient.
	/// </summary>
	public class Pop3ScheduleClient: MarshalByRefObject, IScheduleClient
	{
		public Pop3ScheduleClient()
		{
		}

		/// <summary>
		/// Invokes the specified application name.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		public void InternalInvoke(string applicationName)
		{
			if(applicationName == IbnConst.ProductName)
			{
				UserLight prevUser, curUser;
				prevUser = Security.SetCurrentUser("alert", out curUser);

				try
				{
					Pop3.Pop3Manager.SingleRun();
				}
				finally
				{
					Security.SetCurrentUser(prevUser);
				}
			}
		}

		#region IScheduleClient Members

		/// <summary>
		/// Invoke2s the specified application name.
		/// </summary>
		/// <param name="applicationName">Name of the application.</param>
		public void Invoke(string applicationName)
		{
			if(applicationName == IbnConst.ProductName)
			{
				DbHelper2.Init();

				string[] list = Company.GetApplicationIdsForScheduleService();

				foreach(string appId in list)
				{
					try
					{
						System.Diagnostics.Trace.WriteLine("Invoke2: " + appId);

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
					finally
					{
					}
				}
			}
		}

		#endregion
	}
}
