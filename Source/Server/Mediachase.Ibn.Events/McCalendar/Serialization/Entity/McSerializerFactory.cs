using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Events.McCalendar.Common;
using Mediachase.Ibn.Core.Business;
using Mediachase.iCal.Components;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Events.McCalendar.Serialization.Entity.DataTypes;
using Mediachase.iCal.Serialization.iCalendar.DataTypes;
using System.Reflection;
using System.Collections;
using Mediachase.Ibn.Events.McCalendar.Components;
using Mediachase.Ibn.Events.McCalendar.DataTypes;


namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity
{
	public class McSerializerFactory : AbstractFactory, IFactoryMethod<IEntitySerializable>
	{
	
		#region IFactoryMethod<IEntitySerializable> Members

		IEntitySerializable IFactoryMethod<IEntitySerializable>.Create(object obj)
		{
			IEntitySerializable retVal = null;
			if (obj != null)
			{
				Type type = obj is Type ? obj as Type : obj.GetType();
				if (type.IsSubclassOf(typeof(iCalObject)))
				{
					if (type == typeof(McEvent))
					{
						retVal = new McEventSerializer(obj as McEvent);
					}
					else if (type == typeof(McRecurrencePattern))
					{
						retVal = new McRecurrencePatternSerializer(obj as McRecurrencePattern);
					}
					else if (type == typeof(RecurrencePattern))
					{
						retVal = new McRecurrencePatternSerializer(obj as RecurrencePattern);
					}
					else if (type == typeof(McCalAddress))
					{
						retVal = new McResourceSerializer(obj as McCalAddress);
					}
					else if (type == typeof(Cal_Address))
					{
						retVal = new McResourceSerializer(obj as Cal_Address);
					}
				}
			}

			return retVal;
		}

		#endregion
	
	}
}
