using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Events.CustomMethods.UpdateResources;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Events.Request
{
	public class CalendarEventUpdateResourcesRequest :  Mediachase.Ibn.Core.Business.Request
	{
		private EntityObject[] _entityObjects;

		public CalendarEventUpdateResourcesRequest(CalendarEventEntity target)
			: base(CalendarResourcesUpdateMethod.METHOD_NAME, target)
		{
		}

		public CalendarEventUpdateResourcesRequest(CalendarEventEntity target, EntityObject[] entityObjects)
			: base(CalendarResourcesUpdateMethod.METHOD_NAME, target)
		{
			_entityObjects = entityObjects;
		}


		public EntityObject[] EntityObjects
		{
			get { return _entityObjects; }
			set { _entityObjects = value; }
		}
	
	}
}
