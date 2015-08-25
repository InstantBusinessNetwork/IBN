// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Synchronization;

namespace Mediachase.ClientOutlook.TestProvider
{
    public class MyProviderFactory
    {
		public static MySimpleDataStore storeA = new MySimpleDataStore();
		public static MySimpleDataStore storeB = new MySimpleDataStore();
		public static MySimpleDataStore storeC = new MySimpleDataStore();
		public const string DATA = "data";
		public const string providerA = "A";
		public const string providerB = "B";
		public const string providerC = "C";

		static MyProviderFactory()
		{
			

			Guid[] itemIds = new Guid[9];
			itemIds[1] = new Guid("11111111-1111-1111-1111-111111111111");
			itemIds[2] = new Guid("22222222-2222-2222-2222-222222222222");
			itemIds[3] = new Guid("33333333-3333-3333-3333-333333333333");
			itemIds[4] = new Guid("44444444-4444-4444-4444-444444444444");
			itemIds[5] = new Guid("55555555-5555-5555-5555-555555555555");
			itemIds[6] = new Guid("66666666-6666-6666-6666-666666666666");
			itemIds[7] = new Guid("77777777-7777-7777-7777-777777777777");
			itemIds[8] = new Guid("88888888-8888-8888-8888-888888888888");


			storeA.CreateItem(new ItemData(DATA, "1"), itemIds[1]);
			storeA.CreateItem(new ItemData(DATA, "2"), itemIds[2]);
			storeA.CreateItem(new ItemData(DATA, "7"), itemIds[7]);
			storeA.CreateItem(new ItemData(DATA, "8"), itemIds[8]);
			storeA.CreateItem(new ItemData(DATA, "3"), itemIds[3]);
			storeA.CreateItem(new ItemData(DATA, "4"), itemIds[4]);
			storeB.CreateItem(new ItemData(DATA, "5"), itemIds[5]);
			storeB.CreateItem(new ItemData(DATA, "6"), itemIds[6]);

		}

		public static KnowledgeSyncProvider CreateAProvider()
		{
			return new MySyncProvider(providerA, storeA);

		}

		public static KnowledgeSyncProvider CreateBProvider()
		{
			return new MySyncProvider(providerB, storeB);
			
		}

	    /// <summary>
        /// This is an extension of an in-memory provider sample that is used to illustrate the responsibilites of a provider working on behalf 
        /// of a store. For instance, what do do with an item create/update/delete, how to get changes, how to apply changes, how to detect conflicts, 
        /// and how to resolve them using a custom action such as merge...
        /// 
        /// Please note that this sample is most useful with breakpoints in MyTestProgram.cs to find out HOW synchronization using the 
        /// Microsoft Sync Framework works. This sample is not designed to be a boot-strapper like the NTFS providers for native and managed...
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            const string DATA = "data";
            string providerA = "A";
            string providerB = "B";
            string providerC = "C";
            List<string> arguments = new List<string>(args);

            //start clean
            CleanUpProvider(providerA);
            CleanUpProvider(providerB);
            CleanUpProvider(providerC);

            //Initialize the stores
            storeA = new MySimpleDataStore();
            storeB = new MySimpleDataStore();
            storeC = new MySimpleDataStore();

            //Create items on each store using a global ID and the data of the item (Note, in order to verify results against 
            //a baseline we are using hardcoded global ids for the items. In a real application CreateItem should be called without
            //the Guid parameter, which would generate a new Guid id and return it.
            Guid[] itemIds = new Guid[9];
            itemIds[1] = new Guid("11111111-1111-1111-1111-111111111111");
            itemIds[2] = new Guid("22222222-2222-2222-2222-222222222222");
            itemIds[3] = new Guid("33333333-3333-3333-3333-333333333333");
            itemIds[4] = new Guid("44444444-4444-4444-4444-444444444444");
            itemIds[5] = new Guid("55555555-5555-5555-5555-555555555555");
            itemIds[6] = new Guid("66666666-6666-6666-6666-666666666666");
			itemIds[7] = new Guid("77777777-7777-7777-7777-777777777777");
			itemIds[8] = new Guid("88888888-8888-8888-8888-888888888888");




            storeA.CreateItem(new ItemData(DATA, "1"), itemIds[1]);
            storeA.CreateItem(new ItemData(DATA, "2"), itemIds[2]);
			storeA.CreateItem(new ItemData(DATA, "7"), itemIds[7]);
			storeA.CreateItem(new ItemData(DATA, "8"), itemIds[8]);
            storeB.CreateItem(new ItemData(DATA, "3"), itemIds[3]);
            storeB.CreateItem(new ItemData(DATA, "4"), itemIds[4]);
            storeC.CreateItem(new ItemData(DATA, "5"), itemIds[5]);
            storeC.CreateItem(new ItemData(DATA, "6"), itemIds[6]);

            //Show the contents of the stores, prior to ANY any synchronization...
            Console.WriteLine("Show the contents of the stores, prior to any synchronization...");
            Console.WriteLine(new MySyncProvider(providerA, storeA).ToString());
            Console.WriteLine(storeA.ToString());
            Console.WriteLine(new MySyncProvider(providerB, storeB).ToString());
            Console.WriteLine(storeB.ToString());
            Console.WriteLine(new MySyncProvider(providerC, storeC).ToString());
            Console.WriteLine(storeC.ToString());

            //Sync providers A and provider B
            DoBidirectionalSync(providerA, storeA, providerB, storeB);

			if (arguments.Contains("-q") != true)
			{
				Console.WriteLine("Sync has finished...");
				Console.ReadLine();
			}
		    //Create an update-update conflict on item 1 and show merge... (Note - this sample handles update-update conflicts and simply merges the data)
            Console.WriteLine("A and B are going to create a conflict on item 1. A writes 'Did Merging' and B writes 'Work?'");

            //Create an update-update conflict to merge...
            storeA.UpdateItem(itemIds[1], new ItemData(DATA, "Did Merging"));
            storeB.UpdateItem(itemIds[1], new ItemData(DATA, " Work?"));

            //Sync providers A and provider B
            DoBidirectionalSync(providerA, storeA, providerB, storeB);

            //Delete an item on B and show that the delete propagates
            Console.WriteLine("Deleting item '4' on B");
            storeB.DeleteItem(itemIds[4]);

            //Sync B and C
            DoBidirectionalSync(providerB, storeB, providerC, storeC);

            //Close the sync loop by syncing A and C
            Console.WriteLine("Closing the \"sync loop\" by syncing A and C...");
            DoBidirectionalSync(providerA, storeA, providerC, storeC);

            //Delete item 2 on B
            Console.WriteLine("{0}Deleting item '2' on B.", Environment.NewLine);
            storeB.DeleteItem(itemIds[2]);

            // Sync B and C
            DoBidirectionalSync(providerB, storeB, providerC, storeC);

            //Cleanup Tombstones on B
            Console.WriteLine("{0}Clean up Tombstones on B.", Environment.NewLine);
            new MySyncProvider(providerB, storeB).CleanupTombstones(TimeSpan.Zero);

            // Sync B and C
            DoBidirectionalSync(providerB, storeB, providerC, storeC);

            //Close the sync loop by syncing A and C
            DoBidirectionalSync(providerA, storeA, providerC, storeC);

            if (arguments.Contains("-q") != true)
            {
                Console.WriteLine("Sync has finished...");
                Console.ReadLine();
            }
        }

        static void RegisterCallbacks(MySyncProvider provider)
        {
            provider.DestinationCallbacks.FullEnumerationNeeded += new EventHandler<FullEnumerationNeededEventArgs>(DestinationCallbacks_FullEnumerationNeeded);
            provider.DestinationCallbacks.ItemChangeSkipped += new EventHandler<ItemChangeSkippedEventArgs>(DestinationCallbacks_ItemChangeSkipped);
            provider.DestinationCallbacks.ItemChanging += new EventHandler<ItemChangingEventArgs>(DestinationCallbacks_ItemChanging);
            provider.DestinationCallbacks.ItemConflicting += new EventHandler<ItemConflictingEventArgs>(DestinationCallbacks_ItemConflicting);
            provider.DestinationCallbacks.ProgressChanged += new EventHandler<SyncStagedProgressEventArgs>(DestinationCallbacks_ProgressChanged);
        }

        static void DestinationCallbacks_ProgressChanged(object sender, SyncStagedProgressEventArgs e)
        {
            Console.Write("Event Progress Changed: provider - {0}, ", e.ReportingProvider.ToString());
            Console.Write("stage - {0}, ", e.Stage.ToString());
            Console.WriteLine("work - {0} of {1}", e.CompletedWork, e.TotalWork);
        }

        static void DestinationCallbacks_ItemConflicting(object sender, ItemConflictingEventArgs e)
        {
            Console.Write("Event Item conflicting: source data - {0}, ", e.SourceChangeData != null ? e.SourceChangeData.ToString() : null);
            Console.WriteLine("destination data - {0}", e.DestinationChangeData != null ? e.DestinationChangeData.ToString() : null);
            e.SetResolutionAction(ConflictResolutionAction.Merge);
        }

        static void DestinationCallbacks_ItemChanging(object sender, ItemChangingEventArgs e)
        {
            Console.Write("Event Item changing: item - {0}, ", e.Item.ItemId.ToString());
            Console.WriteLine("change kind - {0}", e.Item.ChangeKind.ToString());
        }

        static void DestinationCallbacks_ItemChangeSkipped(object sender, ItemChangeSkippedEventArgs e)
        {
            Console.Write("Event Item Change Skipped: provider {0}, ", e.ReportingProvider.ToString());
            Console.Write("stage - {0}, ", e.Stage.ToString());
            Console.WriteLine("item  - {0} ", e.ItemChange.ItemId.ToString());
        }

        static void DestinationCallbacks_FullEnumerationNeeded(object sender, FullEnumerationNeededEventArgs e)
        {
            FullEnumerationAction action = FullEnumerationAction.Full;  // This can be changed by the application to control if full enumeration is FZull, Partial, or Aborted.
            Console.Write("Event Full Enumeration Needed: old action {0}, ", e.Action.ToString());
            Console.WriteLine("new action - {0} ", action.ToString());
            e.Action = action;
        }

        public static void CleanUpProvider(string name)
        {
            string providerMetadata = Environment.CurrentDirectory + "\\" + name.ToString() + ".Metadata";
            string providerReplicaid = Environment.CurrentDirectory + "\\" + name.ToString() + ".Replicaid";

            //Remove the metadata file
            if (System.IO.File.Exists(providerMetadata))
            {
                System.IO.File.Delete(providerMetadata);
            }

            //Remove the ReplicaId file
            if (System.IO.File.Exists(providerReplicaid))
            {
                System.IO.File.Delete(providerReplicaid);
            }
        }

        static void DoBidirectionalSync(string nameA, MySimpleDataStore storeA, string nameB, MySimpleDataStore storeB)
        {
            SyncOperationStatistics stats;
            MySyncProvider providerA = new MySyncProvider(nameA, storeA);
            MySyncProvider providerB = new MySyncProvider(nameB, storeB);

            //Set the provider's conflict resolution policy to custom (in order to show how to do complex resolution actions)
            providerA.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;
            providerB.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;

            //Register callbacks so we can handle conflicts in the event that they're detected... And other things.
            RegisterCallbacks(providerA);
            RegisterCallbacks(providerB);

            //Sync providers
            Console.WriteLine("Sync {0} and {1}...", nameA, nameB);
            SyncOrchestrator agent = new SyncOrchestrator();
            agent.Direction = SyncDirectionOrder.DownloadAndUpload;
            agent.LocalProvider = providerA;
            agent.RemoteProvider = providerB;
            stats = agent.Synchronize();

            // Display the SyncOperationStatistics
            Console.WriteLine("Download Applied:\t {0}", stats.DownloadChangesApplied);
            Console.WriteLine("Download Failed:\t {0}", stats.DownloadChangesFailed);
            Console.WriteLine("Download Total:\t\t {0}", stats.DownloadChangesTotal);
            Console.WriteLine("Upload Total:\t\t {0}", stats.UploadChangesApplied);
            Console.WriteLine("Upload Total:\t\t {0}", stats.UploadChangesFailed);
            Console.WriteLine("Upload Total:\t\t {0}", stats.UploadChangesTotal);

            //Show the results of sync
            Console.WriteLine(providerA.ToString());
            Console.WriteLine(storeA.ToString());
            Console.WriteLine(providerB.ToString());
            Console.WriteLine(storeB.ToString());
        }

      
    }
}
