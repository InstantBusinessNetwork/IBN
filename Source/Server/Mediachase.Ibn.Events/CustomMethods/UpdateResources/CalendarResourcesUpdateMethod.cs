using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Events.CustomMethods.UpdateResources
{
	/// <summary>
	/// Позволяет синхронизировать ресурсы события с заданным множеством
	/// </summary>
	public static class CalendarResourcesUpdateMethod
	{
		public const string METHOD_NAME = "UpdateResources";

	
		private class ResourceActionWrapper
		{
			public Mediachase.Ibn.Core.Business.Request ResourceActionRequest;

			public EntityObject ResourceEntity
			{
				get
				{
					return ResourceActionRequest.Target;
				}
			}

			public ResourceActionWrapper(EntityObject entity, Mediachase.Ibn.Core.Business.Request request)
			{
				ResourceActionRequest = request;
				ResourceActionRequest.Target = entity;
			}

		
			
			//public static EntityObject ConvertToEntityObejct( ResourceActionWrapper actionWrapper)
			//{
			//    return actionWrapper._request.Target;
			//}
		}

		/// <summary>
		/// Updates the resources.
		/// </summary>
		/// <param name="request">The request.</param>
		public static void UpdateResources(CalendarEventUpdateResourcesRequest request)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			ListRequest listRequest = null;
			ListResponse listResponse = null;
			//Get the event primary key from target
			PrimaryKeyId eventId = request.Target.PrimaryKeyId.Value;
			//Возможные новые pk
			Dictionary<PrimaryKeyId, PrimaryKeyId> primaryKeyMap = null;

			List<ResourceActionWrapper> resourceActions = new List<ResourceActionWrapper>();
			//Determine update or create actions
			foreach (EntityObject entityObject in request.EntityObjects)
			{
				if(entityObject.MetaClassName == CalendarEventResourceEntity.ClassName)
				{
					//Assign eventId properties 
					entityObject[CalendarEventResourceEntity.FieldEventId] = eventId;

					listRequest = (ListRequest)EventHelper.ConstructRequest<ListRequest>(entityObject, false);
					listRequest.Filters = ConstructFilterByEntity(entityObject);
					listResponse =  (ListResponse)BusinessManager.Execute(listRequest);
						//BusinessManager.List(entityObject.MetaClassName, ConstructFilterByEntity(entityObject));
					if(listResponse.EntityObjects.Length != 0)
					{
						//Update disable
						//foreach (EntityObject existEntity in listResponse.EntityObjects)
						//{
						//    entityObject.PrimaryKeyId = existEntity.PrimaryKeyId;
						//    resourceActions.Add(new ResourceActionWrapper(entityObject, new UpdateRequest()));
						//}
					}
					else
					{
						resourceActions.Add(new ResourceActionWrapper(entityObject, new CreateRequest()));
					}
				}
			}
			//Determine delete action
			listRequest = (ListRequest)EventHelper.ConstructRequest<ListRequest>(new CalendarEventResourceEntity(), false);
			listRequest.Filters = new FilterElement[] { new FilterElement(CalendarEventResourceEntity.FieldEventId, FilterElementType.Equal, eventId) };
			listResponse = (ListResponse)BusinessManager.Execute(listRequest);
			foreach (CalendarEventResourceEntity resourceEntity in listResponse.EntityObjects)
			{
				EntityComparer entityComp = new EntityComparer(resourceEntity, CalendarEventResourceEntity.ComparedProperties);
				if(Array.Find<EntityObject>(request.EntityObjects, entityComp.CompareEntity) == null)
				{
					resourceActions.Add(new ResourceActionWrapper(resourceEntity, new DeleteRequest()));
				}
			}
			//Пытаемся разбить event на exception
			if(resourceActions.Count != 0)
			{
				bool exceptionCreated;
				primaryKeyMap = EventHelper.PrepareChangeData((VirtualEventId)eventId, out exceptionCreated);
			}
			//Execute action
			foreach (ResourceActionWrapper resourceAction in resourceActions)
			{
				//получаем возможно новый pk event
				eventId = EventHelper.FindDestiantionId(primaryKeyMap, eventId);
				//получаем возможно новый pk resource
				if (resourceAction.ResourceEntity.PrimaryKeyId != null)
				{
					resourceAction.ResourceEntity.PrimaryKeyId = EventHelper.FindDestiantionId(primaryKeyMap, 
																							   resourceAction.ResourceEntity.PrimaryKeyId.Value);
				}
				//записываем возможно изменившийся pk event-a
				resourceAction.ResourceEntity[CalendarEventResourceEntity.FieldEventId] = eventId;
				//вызываем запросы на модификацию
				BusinessManager.Execute(resourceAction.ResourceActionRequest);
			}
		}

		private static FilterElement[] ConstructFilterByEntity(EntityObject entity)
		{
			AndBlockFilterElement retVal = new AndBlockFilterElement();
			foreach (string propName in CalendarEventResourceEntity.ComparedProperties)
			{
				EntityObjectProperty property = entity.Properties[propName];
				if (property != null && property.Value != null)
				{
					retVal.ChildElements.Add(new FilterElement(propName, FilterElementType.Equal, property.Value));
				}
			}
			return new FilterElement[] { retVal };
		}

	}
}
