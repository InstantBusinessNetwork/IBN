using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events.McCalendar.DataTypes;
using Mediachase.Ibn.Events.McCalendar.Components;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.Handlers;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Events.Response;

namespace Mediachase.Ibn.Events.CustomMethods.ChangeTracking
{
	public class ChangeTrackingMethod
	{
		public const string METHOD_NAME = "ChangeTracking";

		public static ChangeTrackingResponse ChangeTracking(ChangeTrackingRequest request)
		{
			ChangeTrackingResponse retVal = new ChangeTrackingResponse();
			List<EntityObjectProperty> toRemove = new List<EntityObjectProperty>();
			foreach (EntityObjectProperty property in request.Target.Properties)
			{
				if(property.Name != CalendarEventResourceEntity.FieldEventId)
				{
					toRemove.Add(property);
				}
			}
			foreach (EntityObjectProperty property in toRemove)
			{
				request.Target.Properties.Remove(property);
			}
			
			request.Target.Properties.Add(CalendarEventResourceEntity.FieldStatus, (int)request.Status);
			UpdateRequest updateRequest = new UpdateRequest(request.Target);

			BusinessManager.Execute(updateRequest);
			
			if(request.SendNotify)
			{
				//TODO: Send notify impl
			}

			return retVal;
		}
		

		//private static McCalAddress FindAttendee(CalendarEventResourceEntity resourceEntity, McEvent mcEvent)
		//{
		//    McCalAddress retVal = null;
		//    //try first find by custom parameters
		//    foreach (McCalAddress calAddress in mcEvent.Attendee)
		//    {
		//        if (calAddress.MetaClassName == CalendarEventResourceEntity.ClassName)
		//        {
		//            if (calAddress.MetaObjectId != null)
		//            {
		//                PrimaryKeyId pk = PrimaryKeyId.Parse(calAddress.MetaObjectId);
		//                if (pk == resourceEntity.PrimaryKeyId)
		//                {
		//                    retVal = calAddress;
		//                    break;
		//                }
		//            }
		//        }

		//        //Find by email
		//        if(calAddress.EmailAddress.Equals(resourceEntity.Email, StringComparison.InvariantCultureIgnoreCase))
		//        {
		//            retVal = calAddress;
		//        }
		//    }

		//    return retVal;

		//}
	}
}
