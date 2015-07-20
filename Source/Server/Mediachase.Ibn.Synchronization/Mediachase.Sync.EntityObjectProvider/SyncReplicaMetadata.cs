using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core;
using Microsoft.Synchronization.MetadataStorage;
using Mediachase.Sync.EntityObjectProvider.Sql;
using Microsoft.Synchronization;

namespace Mediachase.Sync.EntityObjectProvider
{
	/// <summary>
	/// 
	/// </summary>
	public class SyncReplicaMetadata : ReplicaMetadata
	{
		public const string URI_COLUMNNAME = "Uri";
		public const string TIMESTAMP_COLUMNNAME = "Timestamp";

		private ReplicaMetadataAdaptor Replica { get; set; }

		#region CalendarEventReplicaMetadata methods
		public SyncReplicaMetadata(SyncIdFormatGroup idFormats, SyncId replicaId, ReplicaMetadataAdaptor replica)
			: base(idFormats, replicaId)
		{
			Replica = replica;
		}

		#endregion
		#region ReplicaMetadata methods
		public override void SaveItemMetadata(ItemMetadata item)
		{
			ItemMetadataAdaptor entityItemMetaData =  ItemMetadataAdaptor.CreateInstance(item);
			entityItemMetaData.Save();
		}
			
		/// <summary>
		/// Этот метод помогает поставщику очищать удаленные элементы по расписанию. Временной интервал, указанный параметром timeInterval, 
		/// представляет собой минимальный возраст удаленных элементов, которые хранятся в хранилище метаданных. Очищаются только удаленные элементы, для которых истек срок, заданный в параметре timeInterval, 
		/// поэтому метод CleanupDeletedItems можно вызывать регулярно, например при каждом выполнении обнаружения изменения поставщиком. Если всегда сохранять удаленные элементы, помеченные как удаленные во время интервала timeInterval,
		/// то метод CleanupDeletedItems поможет поставщику избежать ресурсоемких полных перечислений.
		///Во время первого вызова метода CleanupDeletedItems он сохраняет текущее время как время последней успешной очистки и не предпринимает никаких других действий.
		///При каждом успешном вызове метода CleanupDeletedItems он выполняет следующие шаги.
		///  1. Сравнивает текущее время со временем последней успешной очистки. Если истекшее время меньше интервала timeInterval, то не предпринимается никаких других действий и возвращается значение false.
		///  2. Удаляет все удаленные элементы, которые старше последней успешной очистки.
		///  3. Сохраняет текущее время как время последней успешной очистки.
		///  4. Обновляет утраченный набор знаний таким образом, чтобы он отражал окончательно удаленные элементы.
		/// </summary>
		/// <param name="timeInterval">Минимальный возраст удаленных элементов, которые должны храниться в хранилище метаданных. Значение 0 означает, что производится моментальная очистка всех удаленных элементов.</param>
		/// <returns>Возвращает значение true, если очистка была выполнена. В противном случае возвращает значение false. </returns>
		public override bool CleanupDeletedItems(TimeSpan timeInterval)
		{
			bool retVal = false;
			//First call
			if (LastDeletedItemsCleanupTime != DateTime.MinValue)
			{
				TimeSpan elapsedTime = DateTime.UtcNow - LastDeletedItemsCleanupTime;
				if (elapsedTime >= timeInterval)
				{
					foreach (ItemMetadataAdaptor item in Replica.EntityMetadataItems.Where(x => x.IsDeleted && new DateTime(x.Timestamp.Value) < LastDeletedItemsCleanupTime))
					{
						Replica.ForgottenKnowledge.ForgetTo(Replica.CurrentKnowledge, item.ChangeVersion);
						item.Delete();
						retVal = true;
					}
				}
			}

			Replica.LastDeletedItemsCleanupTime = DateTime.UtcNow;
			return retVal;
		}

		/// <summary>
		/// Создает новый объект метаданных элемента, который может быть использован для добавления записи метаданных нового элемента в хранилище метаданных.
		/// </summary>
		/// <param name="globalId">The global id.</param>
		/// <param name="creationVersion">The creation version.</param>
		/// <returns></returns>
		public override ItemMetadata CreateItemMetadata(SyncId globalId, SyncVersion creationVersion)
		{
			if (globalId == null || creationVersion == null)
			{
				throw new ArgumentNullException();
			}
			if (globalId.IsVariableLength != IdFormats.ItemIdFormat.IsVariableLength
			|| globalId.RawId.Length != IdFormats.ItemIdFormat.Length)
			{
				throw new SyncIdFormatMismatchException("globalId");
			}

			ItemMetadataAdaptor retVal = ItemMetadataAdaptor.CreateInstance(globalId, ReplicaId);
			retVal.CreationVersion = creationVersion;
			retVal.ChangeVersion = creationVersion;
			return retVal;
		}


		/// <summary>
		/// Возвращает метаданные элемента, имеющего указанный глобальный идентификатор элемента.
		/// </summary>
		/// <param name="globalId">The global id.</param>
		/// <returns></returns>
		public override ItemMetadata FindItemMetadataById(SyncId globalId)
		{
			if(globalId == null)
			{
				throw new ArgumentNullException("globalId");
			}
			if(globalId.IsVariableLength != IdFormats.ItemIdFormat.IsVariableLength 
			    || globalId.RawId.Length != IdFormats.ItemIdFormat.Length)
			{
				throw new SyncIdFormatMismatchException("globalId");
			}

			ItemMetadataAdaptor retVal = Replica.FindItemMetadataById(globalId);
		
			return retVal;
		}

		/// <summary>
		/// Возвращает пакет изменений, содержащий метаданные элементов, которые отсутствовали в указанном наборе знаний от поставщика назначения
		/// </summary>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="destinationKnowledge">The destination knowledge.</param>
		/// <returns></returns>
		public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge)
		{
			ChangeBatch retVal = null;
			ulong tickCount = GetNextTickCount();


			List<ItemChange> changes = DetectChanges(destinationKnowledge, batchSize);

			retVal = new ChangeBatch(IdFormats, destinationKnowledge, Replica.ForgottenKnowledge);
			
			// Add the changes to the ChangeBatch with our made with knowledge
			// (Made width knowledge is the knowledge the other side will "learn" if they apply these
			// changes successfully)
			retVal.BeginUnorderedGroup();
			retVal.AddChanges(changes);
			// If last change batch, mark accordingly
			// (We always enumerate full batches, so if our batch is less than the batch size we
			// must be at the last batch. The second condition is spurious.)
			bool isLastBatch = false;
			if ((changes.Count < batchSize) || (changes.Count == 0))
			{
				retVal.SetLastBatch();
				isLastBatch = true;
			}

			retVal.EndUnorderedGroup(Replica.CurrentKnowledge, isLastBatch);

			return retVal;
		}

		/// <summary>
		/// Возвращает пакет изменений, который содержит метаданные элементов, имеющих значение идентификатора выше 
		/// или равное указанной нижней границы, как часть полного перечисления.
		/// </summary>
		/// <param name="batchSize">Size of the batch.</param>
		/// <param name="lowerEnumerationBound">The lower enumeration bound.</param>
		/// <param name="destinationKnowledge">The destination knowledge.</param>
		/// <returns></returns>
		public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge destinationKnowledge)
		{
			// Increment the tick count.
			ulong tickCount = GetNextTickCount();

			// Update the knowledge with an updated local tick count.
			Replica.CurrentKnowledge.SetLocalTickCount(tickCount);

			// Copy all in range item metadata entries (as changes) in our batch
			// We use a sorted list as the engine requires we enumerate all in range changes
			// in id sorted order.
			List<ItemChange> changes = DetectChanges(destinationKnowledge, lowerEnumerationBound, batchSize);
			changes.Sort((x,y) => x.ItemId.CompareTo(y.ItemId));

			// Construct a ChangeBatch we can use to create a ChangeBatch instance
			FullEnumerationChangeBatch changeBatch = new FullEnumerationChangeBatch(IdFormats, destinationKnowledge,
																					Replica.ForgottenKnowledge, lowerEnumerationBound);
			// Add the changes
			if (changes.Count > 0)
			{
				changeBatch.BeginOrderedGroup(lowerEnumerationBound);
				changeBatch.AddChanges(changes);
				changeBatch.EndOrderedGroup(changes[changes.Count - 1].ItemId, Replica.CurrentKnowledge);
			}
			// Return the batch

			return changeBatch;
		}

		/// <summary>
		/// Возвращает пакет изменений, содержащий версии элементов и базовые единицы, которые хранятся в данной реплике. 
		/// Они соответствуют элементам и базовым единицам, на которые были ссылки в пакете изменений, полученном от другого поставщика.
		/// </summary>
		/// <param name="sourceChanges">The source changes.</param>
		/// <returns></returns>
		public override IEnumerable<ItemChange> GetLocalVersions(ChangeBatch sourceChanges)
		{
			if (sourceChanges == null)
			{
				throw new ArgumentNullException("sourceChanges");
			}
			ulong tickCount = GetNextTickCount();

			Replica.CurrentKnowledge.SetLocalTickCount(tickCount);
			// Iterate over changes in the source ChangeBatch
			foreach (ItemChange itemChange in sourceChanges)
			{
				ItemChange change = null;
				ItemMetadata item = Replica.FindItemMetadataById(itemChange.ItemId);
				
				// Iterate through each item to get the corresponding version in the local store
				if (item != null)
				{
					// Found the corresponding item in the local metadata
					// Get the local creation version and change (update) version from the metadata 
					change = new ItemChange(IdFormats, ReplicaId, item.GlobalId,
						item.IsDeleted ? ChangeKind.Deleted : ChangeKind.Update,  // If local item is a tombstone, mark it accordingly
						item.CreationVersion, item.ChangeVersion);
				}
				else
				{
					// Remote item has no local counterpart
					// This item is unknown to us
					change = new ItemChange(IdFormats, ReplicaId, itemChange.ItemId,
						ChangeKind.UnknownItem,   // Mark the change as unknown
						SyncVersion.UnknownVersion, SyncVersion.UnknownVersion);
				}

				// Add our change to the change list
				yield return change;
			}

		}

		public override IEnumerable<ItemChange> GetFullEnumerationLocalVersions(FullEnumerationChangeBatch sourceChanges)
		{

			if (sourceChanges == null)
			{
				throw new ArgumentNullException("sourceChanges");
			}
			foreach (ItemMetadata item in Replica.EntityMetadataItems)
			{
				ItemChange change = null;

				if ((item.GlobalId == sourceChanges.DestinationVersionEnumerationRangeLowerBound || item.GlobalId > sourceChanges.DestinationVersionEnumerationRangeLowerBound)
				   && (item.GlobalId == sourceChanges.DestinationVersionEnumerationRangeUpperBound || item.GlobalId < sourceChanges.DestinationVersionEnumerationRangeUpperBound))
				{
					// Found the corresponding item in the local metadata
					// Get the local creation version and change (update) version from the metadata 
					change = new ItemChange(IdFormats, ReplicaId, item.GlobalId,
						item.IsDeleted ? ChangeKind.Deleted : ChangeKind.Update,  // If local item is a tombstone, mark it accordingly
						item.CreationVersion, item.ChangeVersion);
				}
				else
				{
					// Remote item has no local counterpart
					// This item is unknown to us
					change = new ItemChange(IdFormats, ReplicaId, null,
						ChangeKind.UnknownItem,   // Mark the change as unknown
						SyncVersion.UnknownVersion, SyncVersion.UnknownVersion);
				}
				
				yield return change;
			}
		}

		/// <summary>
		/// Возвращает полное число элементов в хранилище метаданных и число элементов, удаленных из хранилища метаданных. 
		/// </summary>
		/// <param name="totalItemCount">The total item count.</param>
		/// <param name="deletedItemCount">The deleted item count.</param>
		public override void GetItemCount(out uint totalItemCount, out uint deletedItemCount)
		{
			totalItemCount = (uint)Replica.EntityMetadataItems.Where(x => x.IsDeleted == false).Count();
			deletedItemCount = (uint)Replica.EntityMetadataItems.Where(x => x.IsDeleted).Count();
		}

		/// <summary>
		/// Увеличивает ранее сохраненное значение счетчика тактов для данной реплики, 
		/// сохраняет новое значение в хранилище и возвращает новое значение.
		/// </summary>
		/// <returns></returns>
		public override ulong GetNextTickCount()
		{
			return (ulong)Replica.GetNextTickCount();
		}

		/// <summary>
		/// Возвращает утраченный набор знаний для данной реплики. 
		/// </summary>
		/// <returns></returns>
		public override ForgottenKnowledge GetForgottenKnowledge()
		{
			return new ForgottenKnowledge(base.IdFormats, Replica.CurrentKnowledge);
		}

		/// <summary>
		/// Возвращает копию текущего набора знаний для данной реплики. 
		/// </summary>
		/// <returns></returns>
		public override SyncKnowledge GetKnowledge()
		{
			return Replica.CurrentKnowledge.Clone();
		}
		/// <summary>
		/// Removes the item metadata.
		/// </summary>
		/// <param name="globalIds">The global ids.</param>
		public override void RemoveItemMetadata(IEnumerable<SyncId> globalIds)
		{
			foreach (SyncId syncId in globalIds)
			{
				ItemMetadataAdaptor entityItemMetadata = Replica.FindItemMetadataById(syncId);
				if (entityItemMetadata == null)
				{
					throw new Exception("item not found");
				}
				Replica.ForgottenKnowledge.ForgetTo(Replica.CurrentKnowledge, entityItemMetadata.ChangeVersion);
				entityItemMetadata.Delete();
			}
		}

	

		public override void SaveReplicaMetadata()
		{
			Replica.Save();
		}

		public override void SetForgottenKnowledge(ForgottenKnowledge forgottenKnowledge)
		{
			Replica.ForgottenKnowledge = forgottenKnowledge;
		}

		public override void SetKnowledge(SyncKnowledge knowledge)
		{
			Replica.CurrentKnowledge = knowledge;
		}  

	

		public override DateTime LastDeletedItemsCleanupTime
		{
			get { return Replica.LastDeletedItemsCleanupTime ?? DateTime.MinValue; }
		}

		/// <summary>
		/// Возвращает список записей метаданных элемента, имеющих указанные значения для набора индексированных полей.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="fieldValue">The field value.</param>
		/// <returns></returns>
		public override IEnumerable<ItemMetadata> FindItemMetadataByIndexedField(string fieldName, object fieldValue)
		{
			if (fieldName == null || fieldValue == null)
			{
				throw new ArgumentNullException("fieldName or fieldValue");
			}
			if (fieldName.Length == 0)
			{
				throw new ArgumentException("fieldName");
			}

			return ItemMetadataAdaptor.FindMetadataItemByCustomField(fieldName, fieldValue);
		}


		/// <summary>
		/// Возвращает уникальную запись метаданных элемента, имеющую указанное значение индексированного поля.
		/// </summary>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="fieldValue">The field value.</param>
		/// <returns></returns>
		public override ItemMetadata FindItemMetadataByUniqueIndexedField(string fieldName, object fieldValue)
		{
			if (fieldName == null || fieldValue == null)
			{
				throw new ArgumentNullException("fieldName or fieldValue");
			}
			if (fieldName.Length == 0)
			{
				throw new ArgumentException("fieldName");
			}
			ItemMetadata retVal = null;
			foreach(ItemMetadata item in ItemMetadataAdaptor.FindMetadataItemByCustomField(fieldName, fieldValue))
			{
				retVal = item;
				break;
			}
			return retVal;
		}

		#region Not implement
		public override void ExcludeItemFromAllKnowledgeByGlobalId(Microsoft.Synchronization.SyncId globalId)
		{
			throw new NotImplementedException();
		}

		public override byte[] CustomReplicaMetadata
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override DeleteDetector DeleteDetector
		{
			get { throw new NotImplementedException(); }
		}

	
		public override IEnumerable<ItemMetadata> FindItemMetadataByIndexedFields(IEnumerable<KeyValuePair<string, object>> fields)
		{
			throw new NotImplementedException();
		}


		public override ItemMetadata FindItemMetadataByUniqueIndexedFields(IEnumerable<KeyValuePair<string, object>> fields)
		{
			throw new NotImplementedException();
		}

	
		public override Microsoft.Synchronization.ChangeBatch GetFilteredChangeBatch(uint batchSize, Microsoft.Synchronization.SyncKnowledge destinationKnowledge, Microsoft.Synchronization.FilterInfo filterInfo, ReplicaMetadata.ItemFilterCallback filterCallback)
		{
			throw new NotImplementedException();
		}

		public override Microsoft.Synchronization.FullEnumerationChangeBatch GetFilteredFullEnumerationChangeBatch(uint batchSize, Microsoft.Synchronization.FilterInfo filterInfo, ReplicaMetadata.ItemFilterCallback filterCallback, Microsoft.Synchronization.SyncId lowerEnumerationBound, Microsoft.Synchronization.SyncKnowledge destinationKnowledge)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ItemMetadata> FindItemMetadataByMergeWinnerId(SyncId globalId)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ItemMetadata> GetAllItems(bool shouldIncludeDeletedItems)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ItemChange> GetFilteredFullEnumerationLocalVersions(FullEnumerationChangeBatch sourceChanges, ReplicaMetadata.ItemFilterCallback filterCallback)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<ItemChange> GetFilteredLocalVersions(ChangeBatch sourceChanges, ReplicaMetadata.ItemFilterCallback filterCallback)
		{
			throw new NotImplementedException();
		}

		public override uint ProviderVersion
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		
		#endregion
		#endregion

		private List<ItemChange> DetectChanges(SyncKnowledge destinationKnowledge, uint batchSize)
		{
			return DetectChanges(destinationKnowledge, IdFormats.ItemIdFormat.Zero, batchSize);
		}
		/// <summary>
		/// Detects changes not known to the destination and returns a change batch
		/// </summary>
		/// <param name="destinationKnowledge">Requester Knowledge</param>
		/// <param name="batchSize">Maximum number of changes to return</param>
		/// <returns>List of changes</returns>
		private List<ItemChange> DetectChanges(SyncKnowledge destinationKnowledge, SyncId lowerEnumerationBound, uint batchSize)
		{
			List<ItemChange> retVal = new List<ItemChange>();

			if (destinationKnowledge == null)
				throw new ArgumentNullException("destinationKnowledge");
			if (batchSize < 0)
				throw new ArgumentOutOfRangeException("batchSize");

			ulong currentLocalTickCount = Replica.TickCount;

			// Update local knowledge with the current local tick count
			Replica.CurrentKnowledge.SetLocalTickCount(currentLocalTickCount);

			// Map the destination knowledge
			// This maps the knowledge from the remote replica key map (where the destination is replicaKey 0)
			// to the local replica key map (where the source is replicaKey)
			//
			// We do this because our metadata is relative to the local store (and local key map)
			// (This is typical of most sync providers)
			SyncKnowledge mappedKnowledge = Replica.CurrentKnowledge.MapRemoteKnowledgeToLocal(destinationKnowledge);

			foreach (ItemMetadata item in Replica.EntityMetadataItems.Where(x => x.GlobalId.CompareTo(lowerEnumerationBound) >= 0))
			{
				// Check if the current version of the item is known to the destination
				// We simply check if the update version is contained in his knowledge

				// If the metadata is for a tombstone, the change is a delete

				if (!mappedKnowledge.Contains(Replica.ReplicaId, item.GlobalId, item.ChangeVersion))
				{
					ItemChange itemChange = new ItemChange(IdFormats, Replica.ReplicaId, item.GlobalId,
						item.IsDeleted ? ChangeKind.Deleted : ChangeKind.Update,
						item.CreationVersion, item.ChangeVersion);

					// Change isn't known to the remote store, so add it to the batch

					retVal.Add(itemChange);
				}

				// If the batch is full, break
				//
				// N.B. Rest of changes will be detected in next batch. Current batch will not be
				// reenumerated (except in case destination doesn't successfully apply them) as
				// when destination applies the changes in this batch, they will add them to their
				// knowledge.

				if (retVal.Count == batchSize)
					break;
			}

			return retVal;
		}
	
		public static string KToString(SyncKnowledge knowlege)
		{
			String retVal = "";

			System.IO.MemoryStream memStream = new System.IO.MemoryStream();
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(memStream, Encoding.UTF8);
			writer.Formatting = System.Xml.Formatting.Indented;
			knowlege.WriteXml(writer);
			writer.Flush();
			memStream.Seek(0, System.IO.SeekOrigin.Begin);
			System.IO.StreamReader reader = new System.IO.StreamReader(memStream);
			retVal = reader.ReadToEnd();

			return retVal;

		}

		
	}
}
