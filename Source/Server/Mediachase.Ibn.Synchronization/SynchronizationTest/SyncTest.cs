using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediachase.Ibn.Data;
using Microsoft.Synchronization;
using Mediachase.Sync.EntityObjectProvider;
using Mediachase.Ibn;
using Microsoft.Office.Interop.Outlook;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data.Services;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Events.Response;


namespace SynchronizationTest
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class SyncTest
	{
		private static string _connectionString = "Data source=S2;Initial catalog=ibn48portal;User ID=dev;Password=";
		private static DataContext _dataContext;

		//static string endpointConfigurationName = "WSHttpBinding_Sync101WebService";
		static Guid providerNameA = new Guid("11111111-1111-1111-1111-111111111111");
		static Guid providerNameB = new Guid("22222222-2222-2222-2222-222222222222");
		//static string providerNameC = "C";
		//static string folderPathForDataAndMetadata = Environment.CurrentDirectory;

		public SyncTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
		}

		//Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void MyClassCleanup() { }

		//Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize() 
		{
			//Configuration.Init();
			_dataContext = new DataContext(_connectionString);
			DataContext.Current = _dataContext;

			//string modulesVirtualPath = @"c:\Projects\IbnNextTfs\4.8\Server\WebPortal\Apps\";
			string modulesDirectoryPath = @"c:\Projects\IbnNextTfs\4.8\Server\WebPortal\Apps";
			GlobalContext context = GlobalContext.Current;
			if (context == null)
			{
				context = new GlobalContext(null);
				GlobalContext.Current = context;
			}
			context.ModulesDirectoryPath = modulesDirectoryPath;
			//context.ModulesVirtualPath = modulesVirtualPath;

			int usr = Mediachase.Ibn.Data.Services.Security.CurrentUserId;
			if (usr == -1)
			{
				//usr = Mediachase.IBN.Business.Security.UserLogin("et", "ibn");
				Mediachase.Ibn.Data.Services.Security.CurrentUserId = 329;
			}

		}

		//Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void MyTestCleanup() { }

		#endregion

		//Синхронизация событий без рекурсии
		//Синхронизация событий с рекурсиями
		//Синхронизация событий с exception
		//Конфликты
		//Чистка удалений
		//Устаревания реплик
		//Прогресс

		[TestMethod]
		public void Sync_Outlook_TO_IBN()
		{
			KnowledgeSyncProvider ibnProvider = GetProviderForSynchronization(providerNameA);
			KnowledgeSyncProvider outlookProvider = GetProviderForSynchronization(providerNameB);

			//Sync providers A and provider B
			DoSync(ibnProvider, outlookProvider, SyncDirectionOrder.Download);
		}

		[TestMethod]
		public void Sync_IBN_TO_Outlook()
		{
			KnowledgeSyncProvider ibnProvider = GetProviderForSynchronization(providerNameA);
			KnowledgeSyncProvider outlookProvider = GetProviderForSynchronization(providerNameB);

			//Sync providers A and provider B
			DoSync(ibnProvider, outlookProvider, SyncDirectionOrder.Upload);
		}

		[TestMethod]
		public void CreateIBN_Event()
		{
			CalendarEventEntity eventEntity = new CalendarEventEntity();
			eventEntity.Subject = "Test ibn";
			eventEntity.Start = DateTime.UtcNow;
			eventEntity.End = eventEntity.Start.AddHours(1);
			eventEntity.Body = "is ibn event";
			BusinessManager.Create(eventEntity);
		}

		[TestMethod]
		public void ChangeIBN_Event()
		{
			CalendarEventListRequest listRequest = new CalendarEventListRequest(CalendarEventEntity.ClassName, new FilterElement[] { });
			ListResponse listResp = (ListResponse)BusinessManager.Execute(listRequest);
			foreach (CalendarEventEntity eventEntity in listResp.EntityObjects)
			{
				eventEntity.PrimaryKeyId = (PrimaryKeyId)(VirtualEventId)eventEntity.PrimaryKeyId;
				eventEntity.Subject = "ibn subject";
				UpdateRequest updateRequest = new UpdateRequest(eventEntity);
				BusinessManager.Update(eventEntity);
				break;
			}

		}

		[TestMethod]
		public void BidirectionalSync_Outlook_IBN()
		{
			KnowledgeSyncProvider providerA = GetProviderForSynchronization(providerNameA);
			KnowledgeSyncProvider providerB = GetProviderForSynchronization(providerNameB);

			//Sync providers A and provider B
			DoSync(providerA, providerB, SyncDirectionOrder.DownloadAndUpload);

			//start clean
			//CleanUpProvider(providerA);
			//CleanUpProvider(providerB);
		}

		[TestMethod]
		public void CleanupProvider()
		{
			KnowledgeSyncProvider providerA = GetProviderForSynchronization(providerNameA);
			KnowledgeSyncProvider providerB = GetProviderForSynchronization(providerNameB);
			CleanUpProvider(providerA);
			CleanUpProvider(providerB);
		}
	
		static void DoSync(KnowledgeSyncProvider providerNameA, KnowledgeSyncProvider providerNameB, SyncDirectionOrder syncOrder)
		{
			SyncOperationStatistics stats;

			// Set the provider's conflict resolution policy since we are doing remote sync, we don't
			// want to see callbacks.
			providerNameA.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.DestinationWins;
			providerNameB.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.DestinationWins;

			//Sync providers
			Console.WriteLine("Sync A -{0} and B -{1}...", providerNameA.ToString(), providerNameB.ToString());
			SyncOrchestrator agent = new SyncOrchestrator();
			agent.Direction = syncOrder;
			agent.LocalProvider = providerNameA;
			agent.RemoteProvider = providerNameB;
			stats = agent.Synchronize();

			// Display the SyncOperationStatistics
			Console.WriteLine("Download Applied:\t {0}", stats.DownloadChangesApplied);
			Console.WriteLine("Download Failed:\t {0}", stats.DownloadChangesFailed);
			Console.WriteLine("Download Total:\t\t {0}", stats.DownloadChangesTotal);
			Console.WriteLine("Upload Total:\t\t {0}", stats.UploadChangesApplied);
			Console.WriteLine("Upload Total:\t\t {0}", stats.UploadChangesFailed);
			Console.WriteLine("Upload Total:\t\t {0}", stats.UploadChangesTotal);

			//Show the results of sync

		}

		static KnowledgeSyncProvider GetProviderForSynchronization(Guid replicaId)
		{
			KnowledgeSyncProvider retVal = null;
			// Return the real provider for endpoint A.
			if (replicaId == providerNameA)
			{
				retVal = CalendarEventSyncProvider.CreateInstance(Mediachase.Ibn.Data.Services.Security.CurrentUserId);
			}
			else if (replicaId == providerNameB)
			{
				retVal = new AppointmentSyncProvider();
			}
			else
			{
				throw new ArgumentOutOfRangeException("name");
			}

			return retVal;
		}

		static void CleanUpProvider(KnowledgeSyncProvider provider)
		{
			AppointmentSyncProvider outlookProvider = provider as AppointmentSyncProvider;
			CalendarEventSyncProvider ibnProvider = provider as CalendarEventSyncProvider;
			if (outlookProvider != null)
			{
				// Remove the data store file
				string metafile = outlookProvider.CurrentSetting.CurrentSyncAppSetting.metaDataFileName;
				if (System.IO.File.Exists(metafile))
				{
					System.IO.File.Delete(metafile);
				}
			}
			else if (ibnProvider != null)
			{
				FilterElement filterEl = new FilterElement(SynchronizationMetadataRow.ColumnReplicaId, FilterElementType.Equal, ibnProvider.ReplicaId.GetGuidId());
				foreach (SynchronizationMetadataRow row in SynchronizationMetadataRow.List(filterEl))
				{
					BusinessManager.Delete(new CalendarEventEntity((PrimaryKeyId)row.Uri));
					
					row.Delete();
				}
				SynchronizationReplicaRow.Delete(ibnProvider.ReplicaId.GetGuidId());
			}
		}

	}
}
