using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.AccessControl;
using Mediachase.Ibn;
using System.Security.Principal;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Represents .
	/// </summary>
	internal sealed class CompanyWebSiteSemaphore
	{
		#region Const
		private const string Uid = "38BEB100-0566-49f7-ABC0-84BC6AA355A5";
		#endregion

		#region Properties
		private static CompanyWebSiteSemaphore CurrentElement { get; set; }

		private Semaphore InnerSemaphore { get; set; }
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyWebSiteSemaphore"/> class.
		/// </summary>
		/// <param name="semaphore">The semaphore.</param>
		private CompanyWebSiteSemaphore(Semaphore semaphore)
		{
			this.InnerSemaphore = semaphore;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="CompanyWebSiteSemaphore"/> is reclaimed by garbage collection.
		/// </summary>
		~CompanyWebSiteSemaphore()
		{
			try
			{
				if (this.InnerSemaphore != null)
				{
					this.InnerSemaphore.Release();
				}
			}
			catch (ObjectDisposedException)
			{
			}

			this.InnerSemaphore = null;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns></returns>
		public static bool Initialize()
		{
			bool bResult = false;

			if (CurrentElement == null)
			{
				// lock
				lock (typeof(CompanyWebSiteSemaphore))
				{
					if (CurrentElement == null)
					{
						int maxPortalsCount = License.PortalsCount;

						Semaphore semaphore = null;

						if (maxPortalsCount > 0)
						{
							// Step 1. Try open Semaphore

							try
							{
								semaphore = Semaphore.OpenExisting(Uid);
							}
							catch (WaitHandleCannotBeOpenedException)
							{
								// The named semaphore does not exist. 
								SemaphoreSecurity security = new SemaphoreSecurity();
								
								// OZ 2009-03-10/Fix: System.Security.Principal.IdentityNotMappedException
								//SemaphoreAccessRule accessRule = new SemaphoreAccessRule("Everyone", SemaphoreRights.TakeOwnership | SemaphoreRights.FullControl, AccessControlType.Allow);
								SecurityIdentifier everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
								SemaphoreAccessRule accessRule = new SemaphoreAccessRule(everyoneSid, SemaphoreRights.TakeOwnership | SemaphoreRights.FullControl, AccessControlType.Allow);

								security.AddAccessRule(accessRule);

								bool createdNew;
								semaphore = new Semaphore(maxPortalsCount, maxPortalsCount, Uid, out createdNew, security);
							}

							// Wait one with timeout
							if (!semaphore.WaitOne(3000, false))
							{
								throw new LicenseRestrictionException("License.PortalsCount");
							}

							CurrentElement = new CompanyWebSiteSemaphore(semaphore);
						}
						else if (maxPortalsCount==-1)
						{
							// Unlimited
							CurrentElement = new CompanyWebSiteSemaphore(null);
						}
						else
							throw new LicenseRestrictionException("Wrong License.PortalsCount = " + maxPortalsCount.ToString());

						bResult = true;
					}
				}
			}

			return bResult;
		}

		/// <summary>
		/// Uninitializes this instance.
		/// </summary>
		public static void Uninitialize()
		{
			if (CurrentElement != null)
			{
				lock (typeof(CompanyWebSiteSemaphore))
				{
					if (CurrentElement != null)
					{
						if (CurrentElement.InnerSemaphore != null)
						{
							CurrentElement.InnerSemaphore.Release();
							CurrentElement.InnerSemaphore.Close();
							CurrentElement.InnerSemaphore = null;
						}
						CurrentElement = null;
					}
				}
			}
		}
		#endregion


	}
}
