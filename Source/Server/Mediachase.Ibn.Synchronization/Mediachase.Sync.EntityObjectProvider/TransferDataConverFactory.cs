using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Sync.Core.Common;
using Mediachase.Ibn.Core.Business;
using Mediachase.Sync.Core;
using Mediachase.Ibn.Events;
using Mediachase.Sync.Core.TransferDataType;
using Mediachase.Ibn.Data;

namespace Mediachase.Sync.EntityObjectProvider
{
	public class TransferDataConverFactory : AbstractFactory, IFactoryMethod<EntityObjectHierarchy>, IFactoryMethod<SyncTransferData>
	{

		private static Dictionary<string, string> _transfer2entity = new Dictionary<string, string>();
		private static Dictionary<string, string> _entity2transfer = new Dictionary<string, string>();

		static TransferDataConverFactory()
		{
			RegisterMaping(AppointmentTransferData.DataName, CalendarEventEntity.ClassName);
			RegisterMaping(RecurrencePatternTransferData.DataName, CalendarEventRecurrenceEntity.ClassName);
			RegisterMaping(RecipientTransferData.DataName, CalendarEventResourceEntity.ClassName);
		}
		private static void RegisterMaping(string transferName, string entityName)
		{
			_transfer2entity.Add(transferName, entityName);
			_entity2transfer.Add(entityName, transferName);
		}

		private static EntityObject CreateEntityObject(SyncTransferData transferData)
		{
			EntityObject retVal = null;
			string entityName;
			if(_transfer2entity.TryGetValue(transferData.SyncDataName, out entityName))
			{
				switch(entityName)
				{
					case CalendarEventEntity.ClassName:
						retVal = new CalendarEventEntity();
						break;
					case CalendarEventRecurrenceEntity.ClassName:
						retVal = new CalendarEventRecurrenceEntity();
						break;
					case CalendarEventResourceEntity.ClassName:
						retVal = new CalendarEventResourceEntity();
						break;
				}
				if(retVal != null)
				{
					CopyProperies(transferData, retVal);
				}
			}

			return retVal;
		}

		private static SyncTransferData CreateTransferData(EntityObject entityObject)
		{
			SyncTransferData retVal = null;
			string transferName;
			if (_entity2transfer.TryGetValue(entityObject.MetaClassName, out transferName))
			{
				switch (transferName)
				{
					case AppointmentTransferData.DataName:
						retVal = new AppointmentTransferData();
						break;
					case RecurrencePatternTransferData.DataName:
						retVal = new RecurrencePatternTransferData();
						break;
					case RecipientTransferData.DataName:
						retVal = new RecipientTransferData();
						break;
				}
				if (retVal != null)
				{
					CopyProperies(entityObject, retVal);
				}
			}

			return retVal;

		}

		/// <summary>
		/// Копирует properties из SyncTRansferedData -> EntityObject
		/// </summary>
		/// <param name="src">The SRC.</param>
		/// <param name="dst">The DST.</param>
		private static void CopyProperies(SyncTransferData src, EntityObject dst)
		{
			foreach (KeyValuePair<string, object> pair in src.Properties)
			{
				dst[pair.Key] = pair.Value;
			}
		}
		private static void CopyProperies(EntityObject src, SyncTransferData dst)
		{
			foreach (string key in new List<string>(dst.Properties.Keys))
			{
				object srcValue = src[key];
				if (srcValue == null)
				{
					System.Reflection.PropertyInfo propInfo = dst.GetType().GetProperty(key);
					if (propInfo != null)
					{
						srcValue = propInfo.PropertyType.IsValueType ? Activator.CreateInstance(propInfo.PropertyType) : null;
					}
				}

				//Избавляемся от ibn framework зависимых типов.
				if (srcValue != null)
				{
					if (srcValue.GetType() == typeof(PrimaryKeyId))
					{
						srcValue = srcValue.ToString();
					}
					else if (srcValue.GetType() == typeof(PrimaryKeyId?))
					{
						srcValue = ((PrimaryKeyId?)srcValue).HasValue ? srcValue.ToString() : null;
					}

					dst.Properties[key] = srcValue;
				}
			}
		}

		#region IFactoryMethod<EntityObjectHierarchy> Members

		EntityObjectHierarchy IFactoryMethod<EntityObjectHierarchy>.Create(object obj)
		{
			EntityObjectHierarchy retVal = null;
			SyncTransferData data = obj as SyncTransferData;
			if (data != null)
			{
				EntityObject entity = CreateEntityObject(data);
				if (entity != null)
				{
					retVal = new EntityObjectHierarchy(entity);
					foreach (SyncTransferData child in data.Childrens)
					{
						retVal.Childrens.Add(Create<EntityObjectHierarchy>(child));
					}
				}
			}

			return retVal;
		}

		#endregion

		#region IFactoryMethod<SyncTransferData> Members

		SyncTransferData IFactoryMethod<SyncTransferData>.Create(object obj)
		{
			SyncTransferData retVal = null;
			EntityObjectHierarchy data = obj as EntityObjectHierarchy;
			if (data != null)
			{
				retVal = CreateTransferData(data.InnerEntity);
				foreach (EntityObjectHierarchy child in data.Childrens)
				{
					retVal.Childrens.Add(Create<SyncTransferData>(child));
				}
				
			}
			return retVal;
		}

		#endregion
	}
}
