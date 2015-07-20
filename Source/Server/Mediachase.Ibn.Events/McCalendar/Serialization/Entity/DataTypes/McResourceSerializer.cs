using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.McCalendar.DataTypes;
using Mediachase.iCal.Components;

namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity.DataTypes
{
	public class McResourceSerializer : IEntitySerializable
	{
		private Cal_Address _calAddress;

		public McResourceSerializer(Cal_Address calAddres)
		{
			_calAddress = calAddres;
		}

		public Cal_Address Resource
		{
			get
			{
				return _calAddress;
			}
		}
		#region IEntitySerializable Members

		public EntityObjectHierarchy Serialize<T>()
		{
			CalendarEventResourceEntity retVal = new CalendarEventResourceEntity();
			McCalAddress mcCalAddress = Resource as McCalAddress;
			if (mcCalAddress != null)
			{
				retVal.PrimaryKeyId = mcCalAddress.MetaObjectId;
			}
		
			//Copy Cal_Address parameters to entity object
			foreach (string paramName in Resource.Parameters.Keys)
			{
				iCal2EntityMapping.MappingResult mapRes = iCal2EntityMapping.iCalProp2EntityProp<T>(paramName);
				if (mapRes != null && Resource.Parameters.ContainsKey(paramName))
				{
					foreach (string paramValue in ((Parameter)Resource.Parameters[paramName]).Values)
					{
						retVal[mapRes.Name] =  EntityPropConverter.ToEntityProperty(mapRes.ValueType, paramValue);
					}
				}
			}

			retVal.Email = Resource.EmailAddress;

			return new EntityObjectHierarchy(retVal);
		}

		public object Deserialize<T>(Mediachase.Ibn.Core.Business.EntityObject entity)
		{
			if(entity.PrimaryKeyId != null)
			{
				McCalAddress mcCalAddress = Resource as McCalAddress;
				if (mcCalAddress != null)
				{
					mcCalAddress.MetaObjectId = entity.PrimaryKeyId.Value;
				}
			}

			foreach (EntityObjectProperty entityProp in entity.Properties)
			{
				iCal2EntityMapping.MappingResult mapRes = iCal2EntityMapping.EntityProp2iCalProp<T>(entityProp.Name);
				if (entityProp.Value == null || mapRes == null)
					continue;

				if (mapRes.Name.StartsWith("X-"))
				{
					Resource.AddParameter(mapRes.Name, entityProp.Value.ToString());
					continue;
				}
				switch (mapRes.Name)
				{
					case "Authority":
						Resource.Scheme = Uri.UriSchemeMailto;
						Resource.Authority = entityProp.Value.ToString();
						break;
				}

			}
			return Resource;
		}

		#endregion
	}
}
