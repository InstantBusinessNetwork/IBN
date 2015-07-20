using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.iCal.Components;
using Mediachase.Ibn.Core.Business;
using Mediachase.iCal.DataTypes;
using Mediachase.Ibn.Events.McCalendar.Serialization.Algoritm;


namespace Mediachase.Ibn.Events.McCalendar.Serialization.Entity
{
	public interface IEntitySerializable
	{
		EntityObjectHierarchy Serialize<T>();
		object Deserialize<T>(EntityObject entity);
	}
}
