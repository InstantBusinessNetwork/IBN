using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Xml.Serialization.GeneratedAssembly;

using Mediachase.Ibn.Core;					// MetaDataWrapper
using Mediachase.Ibn.Data;					// FilterElement
using Mediachase.Ibn.Data.Meta;				// MetaObject
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.IbnNext.TimeTracking
{
	public static class TimeTrackingManager
	{
		#region const
		public const string BlockMetaClassName = "TimeTrackingBlock";
		public const string EntryMetaClassName = "TimeTrackingEntry";
		public const string BlockTypeMetaClassName = "TimeTrackingBlockType";
		public const string BlockTypeInstanceMetaClassName = "TimeTrackingBlockTypeInstance";

		public const string BlockTypeFieldName_Id = "TimeTrackingBlockTypeId";
		public const string BlockTypeFieldName_Title = "Title";
		public const string BlockTypeFieldName_BlockCard = "BlockCard";
		public const string BlockTypeFieldName_EntryCard = "EntryCard";
		public const string BlockTypeFieldName_StateMachine = "StateMachineId";
		public const string BlockTypeFieldName_IsProject = "IsProject";

		public const string BlockTypeInstanceFieldName_BlockType = "BlockTypeId";
		public const string BlockTypeInstanceFieldName_Title = "Title";
		public const string BlockTypeInstanceFieldName_Project = "ProjectId";
		public const string BlockTypeInstanceFieldName_Status = "StatusId";

		public const string Right_AddMyTTBlock = "AddMyTTBlock";
		public const string Right_AddAnyTTBlock = "AddAnyTTBlock";
		public const string Right_Write = "Write";
		public const string Right_Delete = "Delete";
		public const string Right_RegFinances = "RegFinances";
		public const string Right_UnRegFinances = "UnRegFinances";
		public const string Right_ViewFinances = "ViewFinances";

		public const int OwnerRoleId = 6;
		#endregion


		#region GetBlockMetaClass
		/// <summary>
		/// Gets the TimeTrackingBlock MetaClass 
		/// </summary>
		/// <returns>MetaClass</returns>
		public static MetaClass GetBlockMetaClass()
		{
			return MetaDataWrapper.GetMetaClassByName(BlockMetaClassName);
		}
		#endregion

		#region GetEntryMetaClass
		/// <summary>
		/// Gets the TimeTrackingEntry MetaClass 
		/// </summary>
		/// <returns>MetaClass</returns>
		public static MetaClass GetEntryMetaClass()
		{
			return MetaDataWrapper.GetMetaClassByName(EntryMetaClassName);
		}
		#endregion

		#region GetBlockTypeMetaClass
		/// <summary>
		/// Gets the TimeTrackingBlockType MetaClass 
		/// </summary>
		/// <returns>MetaClass</returns>
		public static MetaClass GetBlockTypeMetaClass()
		{
			return MetaDataWrapper.GetMetaClassByName(BlockTypeMetaClassName);
		}
		#endregion

		#region GetBlockTypeInstanceMetaClass
		/// <summary>
		/// Gets the TimeTrackingBlockTypeInstance MetaClass 
		/// </summary>
		/// <returns>MetaClass</returns>
		public static MetaClass GetBlockTypeInstanceMetaClass()
		{
			return MetaDataWrapper.GetMetaClassByName(BlockTypeInstanceMetaClassName);
		}
		#endregion

		#region GetBlockTypeList
		/// <summary>
		/// TimeTrackingBlockTypeId, Title, BlockCard, EntryCard, StateMachineId, IsProject
		/// </summary>
		/// <returns>MetaObject[]</returns>
		public static MetaObject[] GetBlockTypeList()
		{
			return MetaObject.List(GetBlockTypeMetaClass());
		}
		#endregion

		#region GetBlockTypeItem
		/// <summary>
		/// TimeTrackingBlockTypeId, Title, BlockCard, EntryCard, StateMachineId, IsProject
		/// </summary>
		/// <param name="id">TimeTrackingBlockTypeId.</param>
		/// <returns>MetaObject</returns>
		public static MetaObject GetBlockTypeItem(int id)
		{
			return MetaObjectActivator.CreateInstance(GetBlockTypeMetaClass(), id);
		}
		#endregion

		#region DeleteBlockTypeItem
		/// <summary>
		/// Deletes the BlockType Item
		/// </summary>
		/// <param name="id">TimeTrackingBlockTypeId.</param>
		public static void DeleteBlockTypeItem(int id)
		{
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				MetaObject mo = GetBlockTypeItem(id);
				bool isProject = (bool)mo.Properties[BlockTypeFieldName_IsProject].Value;

				if (!isProject)	// global
				{
					MetaObject[] instances = MetaObject.List(GetBlockTypeInstanceMetaClass(), FilterElement.EqualElement(BlockTypeInstanceFieldName_BlockType, id));
					foreach (MetaObject bti in instances)
						bti.Delete();
				}

				mo.Delete();

				tran.Commit();
			}
		}
		#endregion

		#region AddBlockTypeItem
		/// <summary>
		/// Adds the BlockType Item
		/// </summary>
		/// <param name="title">BlockType Title</param>
		/// <param name="blockCard">Block Card Name</param>
		/// <param name="entryCard">Entry Card Name</param>
		/// <param name="stateMachineId">StateMachineId</param>
		/// <param name="isProject">Is the BlockType the Project SuperType or Global otherwise</param>
		/// <returns>MetaObject</returns>
		public static MetaObject AddBlockTypeItem(string title, string blockCard, string entryCard, int stateMachineId, bool isProject)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				MetaObject mo = MetaObjectActivator.CreateInstance(GetBlockTypeMetaClass());
				mo.Properties[BlockTypeFieldName_Title].Value = title;
				mo.Properties[BlockTypeFieldName_BlockCard].Value = blockCard;
				mo.Properties[BlockTypeFieldName_EntryCard].Value = entryCard;
				mo.Properties[BlockTypeFieldName_StateMachine].Value = stateMachineId;
				mo.Properties[BlockTypeFieldName_IsProject].Value = isProject;
				mo.Save();

				// Create Instance for Global SuperType 
				if (!isProject)
				{
					// OZ 2010-04-12 Fixed Object TimeTrackingBlockTypeInstance.PrimaryKeyId = '#' not found
					using (SkipSecurityCheckScope skipSecurity = Security.SkipSecurityCheck())
					{
						MetaClass mcInst = GetBlockTypeInstanceMetaClass();
						MetaObject inst = MetaObjectActivator.CreateInstance(mcInst);
						inst.Properties[BlockTypeInstanceFieldName_Title].Value = title;
						inst.Properties[BlockTypeInstanceFieldName_BlockType].Value = mo.PrimaryKeyId;
						inst.Save();

						// Exception for Everyone - he can read and
						MetaObject ex = MetaObjectActivator.CreateInstance(Security.GetExceptionAclMetaClass(mcInst.Name));
						ex.Properties["ObjectId"].Value = inst.PrimaryKeyId;
						ex.Properties["PrincipalId"].Value = 1;	// Everyone
						ex.Properties["Read"].Value = 2;			// Can Read
						ex.Properties["AddMyTTBlock"].Value = 2;	// Can Add TimeTrackingBlock for himself
						ex.Save();
					}
				}

				tran.Commit();
				return mo;
			}
		}
		#endregion

		#region AddOwner
		/// <summary>
		/// Adds the Owner to TimeTrackingBlock
		/// </summary>
		/// <param name="objectId">TimeTrackingBlock Id</param>
		/// <param name="principalId">UserId</param>
		/// <returns>MetaObject</returns>
		public static MetaObject AddOwner(int objectId, int principalId)
		{
			using (SkipSecurityCheckScope skipSecurity = Security.SkipSecurityCheck())
			{
				MetaObject mo = MetaObjectActivator.CreateInstance(RoleManager.GetRolePrincipalMetaClass(BlockMetaClassName));
				mo.Properties["ObjectId"].Value = objectId;
				mo.Properties["PrincipalId"].Value = principalId;
				mo.Properties["RoleId"].Value = OwnerRoleId;
				mo.Save();
				return mo;
			}

		}
		#endregion

		#region AddEntries
		/// <summary>
		/// Adds the TimeTracking entries
		/// </summary>
		/// <param name="blockTypeInstanceId"></param>
		/// <param name="startDate"></param>
		/// <param name="ownerId"></param>
		/// <param name="objects"></param>
		/// <param name="objectTypes"></param>
		/// <param name="titles"></param>
		public static void AddEntries(int blockTypeInstanceId, DateTime startDate, int ownerId, List<int> objects, List<int> objectTypes, List<string> titles)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			startDate = startDate.Date;

			// Get the list of existing entries
			TimeTrackingEntry[] existingEntries = TimeTrackingEntry.List(
				FilterElement.EqualElement("StartDate", startDate),
				FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.EqualElement("BlockTypeInstanceId", blockTypeInstanceId),
				FilterElement.IsNotNullElement("ObjectId"));

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Get the block
				TimeTrackingBlock block = GetTimeTrackingBlock(blockTypeInstanceId, startDate, ownerId, true);
				if (!Security.CanWrite(block))
					throw new AccessDeniedException();

				string entryCard = GetEntryCard(blockTypeInstanceId);

				// Loop by entries
				for (int i = 0; i < objects.Count; i++)
				{
					// Check for duplicates
					bool entryExists = false;
					foreach (TimeTrackingEntry existingEntry in existingEntries)
					{
						if (objects[i] == (int)existingEntry.Properties["ObjectId"].Value
							&& objectTypes[i] == (int)existingEntry.Properties["ObjectTypeId"].Value)
						{
							entryExists = true;
							break;
						}
					}
					if (entryExists)
						continue;

					// Add the entry
					TimeTrackingEntry entry = new TimeTrackingEntry();
					entry.ParentBlockId = block.PrimaryKeyId ?? -1;
					entry.Title = titles[i];

					entry.BlockTypeInstanceId = blockTypeInstanceId;
					entry.OwnerId = ownerId;

					entry.Properties["Card"].Value = entryCard;

					entry.Properties["ObjectId"].Value = objects[i];
					entry.Properties["ObjectTypeId"].Value = objectTypes[i];

					entry.Save();
				}

				tran.Commit();
			}
		}

		public static void AddEntries(DateTime startDate, int ownerId, List<int> objects, List<int> objectTypes, List<string> titles, List<int> blockTypeInstances)
		{
			startDate = startDate.Date;

			// We use the dictionaries for optimization
			//	whiteList contains the blocks for which we have the "Write" right
			//	blackList contains the blocks for which we don't have the "Write" right
			//	entryCardList contains card names by BlockTypeInstanceId
			Dictionary<int, TimeTrackingBlock> whiteList = new Dictionary<int, TimeTrackingBlock>();
			Dictionary<int, TimeTrackingBlock> blackList = new Dictionary<int, TimeTrackingBlock>();
			Dictionary<int, string> entryCardList = new Dictionary<int, string>();

			// Get the list of existing entries
			TimeTrackingEntry[] existingEntries = TimeTrackingEntry.List(
				FilterElement.EqualElement("StartDate", startDate),
				FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.IsNotNullElement("ObjectId"));

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				for (int i = 0; i < objects.Count; i++)
				{
					int blockTypeInstanceId = blockTypeInstances[i];

					// Go to the next item if we don't have the "Write" right
					if (blackList.ContainsKey(blockTypeInstanceId))
						continue;

					// Get the block
					TimeTrackingBlock block;
					if (whiteList.ContainsKey(blockTypeInstanceId))
					{
						block = whiteList[blockTypeInstanceId];
					}
					else  // new block
					{
						block = GetTimeTrackingBlock(blockTypeInstanceId, startDate, ownerId, true);

						if (Security.CanWrite(block))
						{
							whiteList.Add(blockTypeInstanceId, block);
						}
						else
						{
							blackList.Add(blockTypeInstanceId, block);
							continue;
						}
					}

					// Check for duplicates
					bool entryExists = false;
					foreach (TimeTrackingEntry existingEntry in existingEntries)
					{
						if (objects[i] == (int)existingEntry.Properties["ObjectId"].Value
							&& objectTypes[i] == (int)existingEntry.Properties["ObjectTypeId"].Value)
						{
							entryExists = true;
							break;
						}
					}
					if (entryExists)
						continue;

					// Get the entry card
					string entryCard;
					if (entryCardList.ContainsKey(blockTypeInstanceId))
					{
						entryCard = entryCardList[blockTypeInstanceId];
					}
					else
					{
						entryCard = GetEntryCard(blockTypeInstanceId);
						entryCardList.Add(blockTypeInstanceId, entryCard);
					}

					// Adding
					TimeTrackingEntry entry = new TimeTrackingEntry();
					entry.ParentBlockId = block.PrimaryKeyId ?? -1;
					entry.Title = titles[i];

					entry.BlockTypeInstanceId = blockTypeInstanceId;
					entry.OwnerId = ownerId;

					entry.Properties["Card"].Value = entryCard;

					entry.Properties["ObjectId"].Value = objects[i];
					entry.Properties["ObjectTypeId"].Value = objectTypes[i];

					entry.Save();
				}

				tran.Commit();
			}
		}

		public static void AddEntries(DateTime startDate, int ownerId, List<int> entryIdList)
		{
			startDate = startDate.Date;

			// We use the dictionaries for optimization
			//	whiteList contains the blocks for which we have the "Write" right
			//	blackList contains the blocks for which we don't have the "Write" right
			Dictionary<int, TimeTrackingBlock> whiteList = new Dictionary<int, TimeTrackingBlock>();
			Dictionary<int, TimeTrackingBlock> blackList = new Dictionary<int, TimeTrackingBlock>();

			TimeTrackingEntry[] existingEntries = TimeTrackingEntry.List(
				FilterElement.EqualElement("StartDate", startDate),
				FilterElement.EqualElement("OwnerId", ownerId));

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				TimeTrackingEntry[] entryList = TimeTrackingEntry.List(new FilterElement("PrimaryKeyId", FilterElementType.In, entryIdList));
				foreach (TimeTrackingEntry srcEntry in entryList)
				{
					int blockTypeInstanceId = srcEntry.BlockTypeInstanceId;

					// Go to the next item if we don't have the "Write" right
					if (blackList.ContainsKey(blockTypeInstanceId))
						continue;

					// Get the block
					TimeTrackingBlock block;
					if (whiteList.ContainsKey(blockTypeInstanceId))
					{
						block = whiteList[blockTypeInstanceId];
					}
					else  // the block wasn't processed yet
					{
						block = GetTimeTrackingBlock(blockTypeInstanceId, startDate, ownerId, true);

						if (Security.CanWrite(block))
						{
							whiteList.Add(blockTypeInstanceId, block);
						}
						else
						{
							blackList.Add(blockTypeInstanceId, block);
							continue;
						}
					}

					// Check for duplicates
					bool entryExists = false;
					foreach (TimeTrackingEntry existingEntry in existingEntries)
					{
						if (srcEntry.Properties["ObjectId"].Value != null
							&& existingEntry.Properties["ObjectId"].Value != null
							&& (int)srcEntry.Properties["ObjectId"].Value == (int)existingEntry.Properties["ObjectId"].Value
							&& (int)srcEntry.Properties["ObjectTypeId"].Value == (int)existingEntry.Properties["ObjectTypeId"].Value)
						{
							entryExists = true;
							break;
						}
						else if (srcEntry.Properties["ObjectId"].Value == null
							&& existingEntry.Properties["ObjectId"].Value == null
							&& srcEntry.Title == existingEntry.Title)
						{
							entryExists = true;
							break;
						}
					}

					if (entryExists)
						continue;

					// Add the entry
					string entryCard = GetEntryCard(blockTypeInstanceId);

					TimeTrackingEntry entry = new TimeTrackingEntry();
					entry.ParentBlockId = block.PrimaryKeyId ?? -1;
					entry.Title = srcEntry.Title;

					entry.BlockTypeInstanceId = blockTypeInstanceId;
					entry.OwnerId = ownerId;

					entry.Properties["Card"].Value = entryCard;

					if (srcEntry.Properties["ObjectId"].Value != null && srcEntry.Properties["ObjectTypeId"].Value != null)
					{
						entry.Properties["ObjectId"].Value = srcEntry.Properties["ObjectId"].Value;
						entry.Properties["ObjectTypeId"].Value = srcEntry.Properties["ObjectTypeId"].Value;
					}

					entry.Save();
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetTimeTrackingBlock
		public static TimeTrackingBlock GetTimeTrackingBlock(int blockTypeInstanceId, DateTime startDate, int ownerId)
		{
			return GetTimeTrackingBlock(blockTypeInstanceId, startDate, ownerId, false);
		}

		private static TimeTrackingBlock GetTimeTrackingBlock(int blockTypeInstanceId, DateTime startDate, int ownerId, bool createNew)
		{
			startDate = startDate.Date;

			TimeTrackingBlock block = null;
			TimeTrackingBlock[] mas = TimeTrackingBlock.List(FilterElement.EqualElement("OwnerId", ownerId), FilterElement.EqualElement("BlockTypeInstanceId", blockTypeInstanceId), FilterElement.EqualElement("StartDate", startDate));
			if (mas.Length > 0)
			{
				block = mas[0];
			}
			else if (createNew)
			{
				block = new TimeTrackingBlock();

				MetaObject blockInstance = MetaObjectActivator.CreateInstance<MetaObject>(GetBlockTypeInstanceMetaClass(), blockTypeInstanceId);
				int blockTypeId = (PrimaryKeyId)blockInstance.Properties["BlockTypeId"].Value;
				MetaObject blockType = MetaObjectActivator.CreateInstance<MetaObject>(GetBlockTypeMetaClass(), blockTypeId);

				MetaClass mcBlock = GetBlockMetaClass();
				int stateMachineId = (PrimaryKeyId)blockType.Properties["StateMachineId"].Value;
				string stateName = StateMachineManager.GetStateMachine(mcBlock, stateMachineId).States[0].Name;

				MetaObject stateInstance = StateMachineManager.GetState(mcBlock, stateName);

				block.mc_StateMachineId = stateMachineId;
				block.mc_StateId = stateInstance.PrimaryKeyId ?? -1;
				block.BlockTypeInstanceId = blockTypeInstanceId;
				block.OwnerId = ownerId;
				block.StartDate = startDate;
				block.Title = blockInstance.Properties["Title"].Value.ToString();
				if (blockInstance.Properties["ProjectId"].Value != null)
					block.ProjectId = (PrimaryKeyId)blockInstance.Properties["ProjectId"].Value;
				if (!String.IsNullOrEmpty(blockType.Properties["BlockCard"].Value.ToString()))
					block.Properties["Card"].Value = blockType.Properties["BlockCard"].Value;
				block.Save();

				// Add Owner to Block
				AddOwner(block.PrimaryKeyId.Value, ownerId);
			}

			return block;
		}
		#endregion

		#region GetEntryCard
		private static string GetEntryCard(int blockTypeInstanceId)
		{
			MetaObject blockInstance = MetaObjectActivator.CreateInstance<MetaObject>(GetBlockTypeInstanceMetaClass(), blockTypeInstanceId);
			int blockTypeId = (PrimaryKeyId)blockInstance.Properties["BlockTypeId"].Value;
			MetaObject blockType = MetaObjectActivator.CreateInstance<MetaObject>(GetBlockTypeMetaClass(), blockTypeId);

			if (blockType.Properties["EntryCard"].Value != null)
				return blockType.Properties["EntryCard"].Value.ToString();
			else
				return String.Empty;
		}
		#endregion

		#region AddHoursForEntryByObject
		public static void AddHoursForEntryByObject(int objectId, int objectTypeId, string title, int projectId, int ownerId, DateTime startDate, int dayNum, int minutes, bool summarize)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			startDate = startDate.Date;


			// O.R. [2008-07-25]
			TimeTrackingBlockTypeInstance inst = null;
			using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
			{
				inst = GetBlockTypeInstanceByProject(projectId);
			}
			if (inst == null)
				throw new ArgumentException("ProjectId is wrong or corresponding BlockTypeInstance does not exists.");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// O.R. [2008-07-28]
				TimeTrackingBlock block = null;
				using (SkipSecurityCheckScope scope = Mediachase.Ibn.Data.Services.Security.SkipSecurityCheck())
				{
					block = GetTimeTrackingBlock(inst.PrimaryKeyId.Value, startDate, ownerId, true);

					if (block == null || !Security.CanWrite(block))
						throw new AccessDeniedException();

					// Get the Entry
					TimeTrackingEntry entry = null;
					TimeTrackingEntry[] entryList = TimeTrackingEntry.List(
						FilterElement.EqualElement("BlockTypeInstanceId", inst.PrimaryKeyId),
						FilterElement.EqualElement("ObjectId", objectId),
						FilterElement.EqualElement("ObjectTypeId", objectTypeId),
						FilterElement.EqualElement("OwnerId", ownerId),
						FilterElement.EqualElement("StartDate", startDate)
						);

					if (entryList.Length > 0)
					{
						entry = entryList[0];
					}
					else
					{
						string entryCard = GetEntryCard(inst.PrimaryKeyId.Value);

						entry = new TimeTrackingEntry();
						entry.ParentBlockId = block.PrimaryKeyId ?? -1;
						entry.Title = title;
						entry.BlockTypeInstanceId = inst.PrimaryKeyId.Value;
						entry.OwnerId = ownerId;
						entry.Properties["Card"].Value = entryCard;
						entry.Properties["ObjectId"].Value = objectId;
						entry.Properties["ObjectTypeId"].Value = objectTypeId;
					}

					switch (dayNum)
					{
						case 1:
							if (summarize)
								entry.Day1 = (double)Math.Min(entry.Day1 + minutes, 1440);
							else
								entry.Day1 = (double)Math.Min(minutes, 1440);
							break;
						case 2:
							if (summarize)
								entry.Day2 = (double)Math.Min(entry.Day2 + minutes, 1440);
							else
								entry.Day2 = (double)Math.Min(minutes, 1440);
							break;
						case 3:
							if (summarize)
								entry.Day3 = (double)Math.Min(entry.Day3 + minutes, 1440);
							else
								entry.Day3 = (double)Math.Min(minutes, 1440);
							break;
						case 4:
							if (summarize)
								entry.Day4 = (double)Math.Min(entry.Day4 + minutes, 1440);
							else
								entry.Day4 = (double)Math.Min(minutes, 1440);
							break;
						case 5:
							if (summarize)
								entry.Day5 = (double)Math.Min(entry.Day5 + minutes, 1440);
							else
								entry.Day5 = (double)Math.Min(minutes, 1440);
							break;
						case 6:
							if (summarize)
								entry.Day6 = (double)Math.Min(entry.Day6 + minutes, 1440);
							else
								entry.Day6 = (double)Math.Min(minutes, 1440);
							break;
						case 7:
							if (summarize)
								entry.Day7 = (double)Math.Min(entry.Day7 + minutes, 1440);
							else
								entry.Day7 = (double)Math.Min(minutes, 1440);
							break;
					}

					entry.Save();
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetBlockTypeInstanceByProject
		public static TimeTrackingBlockTypeInstance GetBlockTypeInstanceByProject(int projectId)
		{
			TimeTrackingBlockTypeInstance[] instList;
			if (projectId > 0)
				instList = TimeTrackingBlockTypeInstance.List(FilterElement.EqualElement("ProjectId", projectId));
			else
				instList = TimeTrackingBlockTypeInstance.List(new FilterElement("ProjectId", FilterElementType.IsNull, null));

			if (instList.Length == 0)
				return null;
			else
				return instList[0];
		}
		#endregion

		#region GetProjectByBlockTypeInstance
		public static int GetProjectByBlockTypeInstance(int blockTypeInstanceId)
		{
			TimeTrackingBlockTypeInstance inst = new TimeTrackingBlockTypeInstance(blockTypeInstanceId);
			if (inst.ProjectId.HasValue)
				return inst.ProjectId.Value;
			return -1;
		}
		#endregion

		#region AddEntry
		/// <summary>
		/// Add the TTEntry Item
		/// </summary>
		/// <returns></returns>
		public static TimeTrackingEntry AddEntry(int blockTypeInstanceId, DateTime startDate, int ownerId, string title)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			startDate = startDate.Date;

			TimeTrackingEntry retVal;
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				MetaObject blockInstance = MetaObjectActivator.CreateInstance<MetaObject>(BlockTypeInstanceMetaClassName, blockTypeInstanceId);
				int blockTypeId = (PrimaryKeyId)blockInstance.Properties[BlockTypeInstanceFieldName_BlockType].Value;
				MetaObject blockType = MetaObjectActivator.CreateInstance<MetaObject>(BlockTypeMetaClassName, blockTypeId);

				TimeTrackingBlock block = null;
				TimeTrackingBlock[] mas = TimeTrackingBlock.List(FilterElement.EqualElement("OwnerId", ownerId), FilterElement.EqualElement("BlockTypeInstanceId", blockTypeInstanceId), FilterElement.EqualElement("StartDate", startDate));
				if (mas.Length > 0)
				{
					block = mas[0];

					if (!Security.CanWrite(block))
						throw new AccessDeniedException();
				}
				else
				{
					block = new TimeTrackingBlock();
					int stateMachineId = (PrimaryKeyId)blockType.Properties[BlockTypeFieldName_StateMachine].Value;
					string stateName = StateMachineManager.GetStateMachine(BlockMetaClassName, stateMachineId).States[0].Name;
					MetaObject stateInstance = StateMachineManager.GetState(BlockMetaClassName, stateName);

					block.mc_StateMachineId = stateMachineId;
					block.mc_StateId = stateInstance.PrimaryKeyId ?? -1;
					block.BlockTypeInstanceId = blockTypeInstanceId;
					block.OwnerId = ownerId;
					block.StartDate = startDate;
					block.Title = blockInstance.Properties[BlockTypeFieldName_Title].Value.ToString();
					if (blockInstance.Properties[BlockTypeInstanceFieldName_Project].Value != null)
						block.ProjectId = (PrimaryKeyId)blockInstance.Properties[BlockTypeInstanceFieldName_Project].Value;
					if (!String.IsNullOrEmpty(blockType.Properties[BlockTypeFieldName_BlockCard].Value.ToString()))
						block.Properties["Card"].Value = blockType.Properties[BlockTypeFieldName_BlockCard].Value;
					block.Save();

					// Add Owner to Block
					TimeTrackingManager.AddOwner(block.PrimaryKeyId.Value, ownerId);
				}

				TimeTrackingEntry entry = new TimeTrackingEntry();
				entry.ParentBlockId = block.PrimaryKeyId ?? -1;
				entry.Title = title;

				entry.BlockTypeInstanceId = blockTypeInstanceId;
				entry.OwnerId = ownerId;

				if (!String.IsNullOrEmpty(blockType.Properties[BlockTypeFieldName_EntryCard].Value.ToString()))
					entry.Properties["Card"].Value = blockType.Properties[BlockTypeFieldName_EntryCard].Value;

				entry.Save();

				retVal = entry;

				tran.Commit();
			}
			return retVal;
		}
		#endregion

		#region AddEntryWithData
		/// <summary>
		/// Adds the entry with data.
		/// </summary>
		/// <param name="blockTypeInstanceId">The block type instance id.</param>
		/// <param name="startDate">The start date.</param>
		/// <param name="ownerId">The owner id.</param>
		/// <param name="title">The title.</param>
		/// <param name="day1">The day1.</param>
		/// <param name="day2">The day2.</param>
		/// <param name="day3">The day3.</param>
		/// <param name="day4">The day4.</param>
		/// <param name="day5">The day5.</param>
		/// <param name="day6">The day6.</param>
		/// <param name="day7">The day7.</param>
		/// <returns></returns>
		public static TimeTrackingEntry AddEntryWithData(int blockTypeInstanceId, DateTime startDate, int ownerId, string title, double day1, double day2, double day3, double day4, double day5, double day6, double day7)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			TimeTrackingEntry entry = null;
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				entry = AddEntry(blockTypeInstanceId, startDate, ownerId, title);
				entry.Day1 = day1;
				entry.Day2 = day2;
				entry.Day3 = day3;
				entry.Day4 = day4;
				entry.Day5 = day5;
				entry.Day6 = day6;
				entry.Day7 = day7;

				entry.Save();

				tran.Commit();
			}
			return entry;
		}
		#endregion

		#region DeleteEntry
		/// <summary>
		/// Delete the TTEntry Item
		/// </summary>
		/// <returns></returns>
		public static bool DeleteEntry(TimeTrackingEntry entry)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			bool retval = false;
			TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), entry.ParentBlockId);
			if (ttb != null && Security.CanWrite(ttb))
			{
				entry.Delete();
				retval = true;
			}
			return retval;
		}
		#endregion

		#region GetBlockTypeInstances
		/// <summary>
		/// Gets the list of TimeTrackingBlockTypeInstances
		/// </summary>
		/// <returns></returns>
		public static DataTable GetBlockTypeInstances()
		{
			MetaClass mc = GetBlockTypeInstanceMetaClass();

			FilterElement orBlock = new OrBlockFilterElement();
			orBlock.ChildElements.Add(new FilterElement(BlockTypeInstanceFieldName_Project, FilterElementType.IsNull, null));
			orBlock.ChildElements.Add(FilterElement.EqualElement(BlockTypeInstanceFieldName_Status, 1));
			orBlock.ChildElements.Add(FilterElement.EqualElement(BlockTypeInstanceFieldName_Status, 5));

			TimeTrackingBlockTypeInstance[] mas = MetaObject.List<TimeTrackingBlockTypeInstance>(mc, new FilterElementCollection(orBlock), new SortingElementCollection(new SortingElement(BlockTypeFieldName_IsProject, SortingElementType.Asc), new SortingElement(BlockTypeFieldName_Title, SortingElementType.Asc)));
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("PrimaryKeyId", typeof(string)));
			dt.Columns.Add(new DataColumn("IsProject", typeof(bool)));
			DataRow dr;
			foreach (MetaObject mo in mas)
			{
				dr = dt.NewRow();
				dr["Title"] = mo.Properties[mc.TitleFieldName].Value.ToString();
				dr["PrimaryKeyId"] = mo.PrimaryKeyId.ToString();
				dr["IsProject"] = (((TimeTrackingBlockTypeInstance)mo).ProjectId != null);
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region GetNonProjectBlockTypeInstances
		/// <summary>
		/// Gets the list of non-project TimeTrackingBlockTypeInstances
		/// </summary>
		/// <returns></returns>
		public static TimeTrackingBlockTypeInstance[] GetNonProjectBlockTypeInstances()
		{
			MetaClass mc = TimeTrackingManager.GetBlockTypeInstanceMetaClass();

			return MetaObject.List<TimeTrackingBlockTypeInstance>(mc,
				new FilterElementCollection(new FilterElement("ProjectId", FilterElementType.IsNull, null)),
				new SortingElementCollection(new SortingElement("Title", SortingElementType.Asc)));
		}
		#endregion

		#region GetProjectBlockTypeInstances
		/// <summary>
		/// Gets the list of project TimeTrackingBlockTypeInstances
		/// </summary>
		/// <returns></returns>
		public static TimeTrackingBlockTypeInstance[] GetProjectBlockTypeInstances()
		{
			MetaClass mc = TimeTrackingManager.GetBlockTypeInstanceMetaClass();

			FilterElement orBlock = new OrBlockFilterElement();
			orBlock.ChildElements.Add(FilterElement.EqualElement("StatusId", 1));
			orBlock.ChildElements.Add(FilterElement.EqualElement("StatusId", 5));

			return MetaObject.List<TimeTrackingBlockTypeInstance>(mc,
				new FilterElementCollection(orBlock),
				new SortingElementCollection(new SortingElement("Title", SortingElementType.Asc)));
		}
		#endregion

		#region UpdateEntry
		/// <summary>
		/// Updates the TTEntry Item
		/// </summary>
		/// <returns></returns>
		public static void UpdateEntry(int entryId, int totalApproved, decimal rate)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				TimeTrackingEntry entry = new TimeTrackingEntry(entryId);

				TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), entry.ParentBlockId);
				if (!Security.CheckObjectRight(ttb, Right_RegFinances))
					throw new AccessDeniedException();

				using (SkipSecurityCheckScope skipSecurity = Security.SkipSecurityCheck())
				{
					entry.Properties["TotalApproved"].Value = (double)totalApproved;
					entry.Properties["Rate"].Value = rate;

					entry.Save();
				}

				tran.Commit();
			}
		}
		#endregion

		#region Clone
		/// <summary>
		/// Clones the week.
		/// </summary>
		/// <param name="ownerId">The owner id.</param>
		/// <param name="srcStartDate">The SRC start date.</param>
		/// <param name="destStartDate">The dest start date.</param>
		public static void CloneWeek(int ownerId, DateTime srcStartDate, DateTime destStartDate)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			srcStartDate = srcStartDate.Date;
			destStartDate = destStartDate.Date;

			TimeTrackingBlock[] srcBlocks = TimeTrackingBlock.List(FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.EqualElement("StartDate", srcStartDate));

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				foreach (TimeTrackingBlock srcBlock in srcBlocks)
				{
					TimeTrackingBlock destBlock = new TimeTrackingBlock();

					#region Copy TimeTrackingBlock Properties
					// Set Start Date
					destBlock.StartDate = destStartDate;

					// Copy References
					destBlock.BlockTypeInstanceId = srcBlock.BlockTypeInstanceId;
					destBlock.mc_StateMachineId = srcBlock.mc_StateMachineId;
					destBlock.OwnerId = srcBlock.OwnerId;
					destBlock.ProjectId = srcBlock.ProjectId;

					// Copy Card
					destBlock.Card = srcBlock.Card;

					// Copy Extended properties 
					foreach (MetaObjectProperty exProperty in srcBlock.ExtendedProperties)
					{
						if (!exProperty.IsReadOnly)
						{
							destBlock.Properties[exProperty.Name].Value = exProperty.Value;
						}
					}

					// Copy Fields
					destBlock.Title = srcBlock.Title;

					destBlock.Day1 = 0;
					destBlock.Day2 = 0;
					destBlock.Day3 = 0;
					destBlock.Day4 = 0;
					destBlock.Day5 = 0;
					destBlock.Day6 = 0;
					destBlock.Day7 = 0;
					#endregion

					destBlock.Save();

					TimeTrackingEntry[] srcEntries = TimeTrackingEntry.List(FilterElement.EqualElement("ParentBlockId", srcBlock.PrimaryKeyId.Value));

					foreach (TimeTrackingEntry srcEntry in srcEntries)
					{
						TimeTrackingEntry destEntry = new TimeTrackingEntry();

						#region Copy TimeTrackingEntry Properties
						// Set Start Date
						//destEntry.StartDate = destStartDate; // Referienced Field From TimeTrackingBlock

						// Copy References
						destEntry.ParentBlockId = destBlock.PrimaryKeyId.Value;
						destEntry.BlockTypeInstanceId = srcEntry.BlockTypeInstanceId;
						destEntry.OwnerId = srcEntry.OwnerId;

						// Copy Card
						destEntry.Card = srcEntry.Card;

						// Copy Extended properties 
						foreach (MetaObjectProperty exProperty in srcEntry.ExtendedProperties)
						{
							if (!exProperty.IsReadOnly)
							{
								destEntry.Properties[exProperty.Name].Value = exProperty.Value;
							}
						}

						// Copy Fields
						destEntry.Title = srcEntry.Title;

						destEntry.Day1 = 0;
						destEntry.Day2 = 0;
						destEntry.Day3 = 0;
						destEntry.Day4 = 0;
						destEntry.Day5 = 0;
						destEntry.Day6 = 0;
						destEntry.Day7 = 0;

						#endregion

						destEntry.Save();
					}
				}

				tran.Commit();
			}
		}
		#endregion

		#region GetTotalHoursByObject
		public static int GetTotalHoursByObject(int objectId, int objectTypeId)
		{
			double hours = 0;
			TimeTrackingEntry[] entryList = TimeTrackingEntry.List(
				FilterElement.EqualElement("ObjectId", objectId),
				FilterElement.EqualElement("ObjectTypeId", objectTypeId),
				FilterElement.EqualElement("OwnerId", Security.CurrentUserId)
				);

			foreach (TimeTrackingEntry entry in entryList)
			{
				hours += entry.Day1;
				hours += entry.Day2;
				hours += entry.Day3;
				hours += entry.Day4;
				hours += entry.Day5;
				hours += entry.Day6;
				hours += entry.Day7;
			}

			return (int)hours;
		}
		#endregion

		#region GetApprovedHoursByObject [Obsolete]
		// We shouldn't use these methods bacause of instead of approved time we should use registered time
		[Obsolete]
		public static int GetApprovedHoursByObject(int objectId, int objectTypeId)
		{
			return GetApprovedHoursByObject(objectId, objectTypeId, true);
		}

		[Obsolete]
		public static int GetApprovedHoursByObject(int objectId, int objectTypeId, bool currentUser)
		{
			double hours = 0;

			TimeTrackingEntry[] entryList;

			if (currentUser)
				entryList = TimeTrackingEntry.List(
					FilterElement.EqualElement("ObjectId", objectId),
					FilterElement.EqualElement("ObjectTypeId", objectTypeId),
					FilterElement.EqualElement("OwnerId", Security.CurrentUserId));
			else
				entryList = TimeTrackingEntry.List(
					FilterElement.EqualElement("ObjectId", objectId),
					FilterElement.EqualElement("ObjectTypeId", objectTypeId));


			MetaClass mc = GetBlockMetaClass();
			Dictionary<int, bool> blockList = new Dictionary<int, bool>();

			foreach (TimeTrackingEntry entry in entryList)
			{
				int blockId = entry.ParentBlockId;

				if (!blockList.ContainsKey(blockId))
				{
					TimeTrackingBlock block = new TimeTrackingBlock(blockId);

					bool isInEndState = false;
					if (StateMachineManager.GetFinalStateId(mc, block.mc_StateMachineId) == block.mc_StateId)
						isInEndState = true;

					blockList.Add(blockId, isInEndState);
				}

				if (blockList[blockId])
				{
					hours += entry.Day1;
					hours += entry.Day2;
					hours += entry.Day3;
					hours += entry.Day4;
					hours += entry.Day5;
					hours += entry.Day6;
					hours += entry.Day7;
				}
			}

			return (int)hours;
		}
		#endregion

		#region GetEntriesForClone
		/// <summary>
		/// Gets the list of entries for clone
		/// </summary>
		/// <returns></returns>
		public static TimeTrackingEntry[] GetEntriesForClone(int ownerId, DateTime srcStartDate, DateTime destStartDate)
		{
			srcStartDate = srcStartDate.Date;
			destStartDate = destStartDate.Date;

			// Get the existing entries
			TimeTrackingEntry[] existingEntries = TimeTrackingEntry.List(
				FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.EqualElement("StartDate", destStartDate));

			// Get the source entries
			TimeTrackingEntry[] srcEntries = TimeTrackingEntry.List(
				FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.EqualElement("StartDate", srcStartDate));

			// Compare source and existing entries
			List<TimeTrackingEntry> destEntries = new List<TimeTrackingEntry>();
			foreach (TimeTrackingEntry srcEntry in srcEntries)
			{
				bool entryExists = false;
				foreach (TimeTrackingEntry existingEntry in existingEntries)
				{
					if (srcEntry.Properties["ObjectId"].Value != null
						&& existingEntry.Properties["ObjectId"].Value != null
						&& (int)srcEntry.Properties["ObjectId"].Value == (int)existingEntry.Properties["ObjectId"].Value
						&& (int)srcEntry.Properties["ObjectTypeId"].Value == (int)existingEntry.Properties["ObjectTypeId"].Value)
					{
						entryExists = true;
						break;
					}
					else if (srcEntry.Properties["ObjectId"].Value == null
						&& existingEntry.Properties["ObjectId"].Value == null
						&& srcEntry.Title == existingEntry.Title)
					{
						entryExists = true;
						break;
					}
				}

				if (!entryExists)
				{
					destEntries.Add(srcEntry);
				}
			}

			return destEntries.ToArray();
		}
		#endregion

		#region MakeTransitionWithComment
		public static void MakeTransitionWithComment(int blockId, Guid transitionUid, string comment)
		{
			if (!Mediachase.Ibn.License.TimeTrackingModule)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				TimeTrackingBlock block = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), blockId);

				CommentService cs = block.GetService<CommentService>();

				if (cs != null && !String.IsNullOrEmpty(comment))
				{
					cs.AddComment("", comment.Replace("\n", "<br />"));
				}

				StateMachineService sms = block.GetService<StateMachineService>();
				if (sms != null)
					sms.MakeTransition(transitionUid);

				block.Save();

				tran.Commit();
			}
		}
		#endregion

		#region Init
		/// <summary>
		/// Initialize Time Tracking Mananger
		/// </summary>
		public static void Init()
		{
			DataContext.MetaModelLoad += new MetaModelLoadEventHandler(DataContext_MetaModelLoad);
		}

		/// <summary>
		/// Handles the MetaModelLoad event of the DataContext control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="Mediachase.Ibn.Data.MetaModelLoadEventArgs"/> instance containing the event data.</param>
		static void DataContext_MetaModelLoad(object sender, MetaModelLoadEventArgs args)
		{
			// Remove Meta View
			args.MetaModel.MetaViews.Remove("TT_ManagerGroupByProjectUser");
			args.MetaModel.MetaViews.Remove("TT_ManagerGroupByUserProject");
			args.MetaModel.MetaViews.Remove("TT_MyGroupByWeekProject");
			args.MetaModel.MetaViews.Remove("TT_MyRejectedGroupByWeekProject");

			// Load From Configuration File
			args.MetaModel.MetaViews.Add(LoadMetaViewFromConfigurationFile("TimeTrackingEntry", "TT_ManagerGroupByProjectUser"));
			args.MetaModel.MetaViews.Add(LoadMetaViewFromConfigurationFile("TimeTrackingEntry", "TT_ManagerGroupByUserProject"));
			args.MetaModel.MetaViews.Add(LoadMetaViewFromConfigurationFile("TimeTrackingEntry", "TT_MyGroupByWeekProject"));
			args.MetaModel.MetaViews.Add(LoadMetaViewFromConfigurationFile("TimeTrackingEntry", "TT_MyRejectedGroupByWeekProject"));
		}

		private static MetaView LoadMetaViewFromConfigurationFile(string metaClassName, string metaViewName)
		{
			IXPathNavigable navigable = XmlBuilder.GetXml(StructureType.MetaView, new Selector(metaClassName, metaViewName));
			XPathNavigator root = navigable.CreateNavigator();

			XPathNavigator schemaNode = root.SelectSingleNode("MetaView/Schema");
			if (schemaNode == null)
				throw new ArgumentException("MetaView's Schema is skiped for '" + metaViewName + "' meta view.");

			string metaViewXml = schemaNode.OuterXml.Replace("Schema>", "MetaView>"); // Allows to use deserializer

			return McXmlSerializer.GetObject<MetaViewSerializer, MetaView>(metaViewXml);
		}
		#endregion

		#region GetPostedTimeByObject
		public static int GetPostedTimeByObject(int objectId, int objectTypeId)
		{
			double minutes = 0;

			TimeTrackingEntry[] entryList = TimeTrackingEntry.List(
					FilterElement.EqualElement("ObjectId", objectId),
					FilterElement.EqualElement("ObjectTypeId", objectTypeId));

			MetaClass mc = GetBlockMetaClass();
			Dictionary<int, bool> blockList = new Dictionary<int, bool>();

			foreach (TimeTrackingEntry entry in entryList)
			{
				if ((bool)entry.Properties["AreFinancesRegistered"].Value)
				{
					if (entry.Properties["TotalApproved"].Value != null)
						minutes += (double)entry.Properties["TotalApproved"].Value;
				}
			}

			return (int)minutes;
		}
		#endregion
	}
}
