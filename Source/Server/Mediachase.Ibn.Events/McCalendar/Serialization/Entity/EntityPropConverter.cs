using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity
{
	public static class EntityPropConverter
	{
		public static object ToEntityProperty(Type valueType, string value)
		{
			TypeCode typeCode = Type.GetTypeCode(valueType);
			object retVal = value;
			switch (typeCode)
			{
				case TypeCode.Boolean:
					retVal = Convert.ToBoolean(value);
					break;
				case TypeCode.Int32:
					retVal = Convert.ToInt32(value);
					break;
				case TypeCode.String:
					break;
				default:
					if (valueType == typeof(PrimaryKeyId))
					{
						retVal = PrimaryKeyId.Parse(value);
					}
					else if (valueType.IsEnum)
					{
						retVal = (int)Enum.Parse(valueType, value);
					}
					break;
			}

			return retVal;
		}
	}
}
