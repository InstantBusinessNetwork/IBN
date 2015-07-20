using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.CustomMethods;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Events.CustomMethods.List;
using Mediachase.Ibn.Events.CustomMethods.UpdateResources;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Sql;
using System.Data;

namespace Mediachase.Ibn.Events.Handlers
{
	/// <summary>
	/// Represents CalendarEvent request handler.
	/// </summary>
	public class CalendarEventRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarEventRequestHandler"/> class.
		/// </summary>
		public CalendarEventRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == CalendarEventEntity.ClassName)
			{
				CalendarEventEntity retVal = new CalendarEventEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#endregion

		#region Custom Methods

		protected override void PreCustomMethod(BusinessContext context)
		{
			base.PreCustomMethod(context);
		}

		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PreCustomMethodInsideTransaction(context);
		}

		protected override void CustomMethod(BusinessContext context)
		{
			switch (context.Request.Method)
			{
				case CalendarEventListMethod.METHOD_NAME:
					context.SetResponse(CalendarEventListMethod.ListEvent(context.Request as CalendarEventListRequest));
					break;
				case CalendarResourcesUpdateMethod.METHOD_NAME:
					CalendarResourcesUpdateMethod.UpdateResources(context.Request as CalendarEventUpdateResourcesRequest);
					break;
				default:
					base.CustomMethod(context);
					break;
			}
			base.CustomMethod(context);
		}

		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PostCustomMethodInsideTransaction(context);
		}

		protected override void PostCustomMethod(BusinessContext context)
		{
			base.PostCustomMethod(context);
		}
		#endregion

		#region Overrides

		protected override void Update(BusinessContext context)
		{
			CalendarEventEntity eventEntity = context.Request.Target as CalendarEventEntity;
			UpdateRequest updateRequest = context.Request as UpdateRequest;
			bool forceBase = updateRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				Mediachase.Ibn.Events.CustomMethods.EventHelper.PrepareUpdateEventRequest(updateRequest);
			}
			else
			{
				base.Update(context);
			}

			
		}

		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			#region Remove references from IBN 4.7 objects
			SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
				"bus_cls_CalendarEventDelete",
				SqlHelper.SqlParameter("@eventId", SqlDbType.UniqueIdentifier, context.GetTargetPrimaryKeyId().Value));
			#endregion
		}
		protected override void Delete(BusinessContext context)
		{
			DeleteRequest deleteRequest = context.Request as DeleteRequest;
			bool forceBase = deleteRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				Mediachase.Ibn.Events.CustomMethods.EventHelper.DeleteEventRequestHandle(deleteRequest);
			}
			else
			{
				base.Delete(context);
			}
		}

		protected override void Create(BusinessContext context)
		{
			//EntityObject target = context.Request.Target;
			//VirtualEventId vEventId = VirtualEventId.CreateInstance();
			////≈сли в target primaryKeyId будет виртуальным то значит генерировать новый не нужно
			//if (target.PrimaryKeyId != null)
			//{
			//    vEventId = (VirtualEventId)target.PrimaryKeyId.Value;
			//    //записываем как есть
			//    target["CalendarEventId"] = PrimaryKeyId.Parse(vEventId.ToString());
			//}
			//else
			//{
			//    //записываем только базовую часть без рекурсии
			//    target["CalendarEventId"] = (PrimaryKeyId)vEventId;
			//}
			CreateRequest createRequest = context.Request as CreateRequest;
			bool forceBase = createRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				CreateResponse response = Mediachase.Ibn.Events.CustomMethods.EventHelper.CreateEventRequestHandle(createRequest);
				context.SetResponse(response);
			}
			else
			{
				base.Create(context);
			}
		}

		protected override void List(BusinessContext context)
		{
			ListRequest listRequest = context.Request as ListRequest;
			bool forceBase = listRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if(!forceBase)
			{
				#region Test area
				//FilterElementCollection filterCollection = new FilterElementCollection();
				//filterCollection.Add(new FilterElement(CalendarEventEntity.FieldStart,
				//                                       FilterElementType.Greater, new DateTime(2009, 1, 15)));
				//OrBlockFilterElement orBlock = new OrBlockFilterElement();
				//orBlock.ChildElements.Add(new FilterElement(CalendarEventEntity.FieldStart, FilterElementType.Less, new DateTime(2009, 1, 16)));
				//orBlock.ChildElements.Add(new FilterElement(CalendarEventEntity.FieldStart, FilterElementType.Less, new DateTime(2009, 1, 17)));
				//filterCollection.Add(orBlock);

				//CalendarEventListRequest calListRequest = new CalendarEventListRequest(listRequest.Target, filterCollection.ToArray(),
				//                                                                       listRequest.Sorting); 
				#endregion

				CalendarEventListRequest calListRequest = new CalendarEventListRequest(listRequest.Target, listRequest.Filters,
				                                                                       listRequest.Sorting);
				ListResponse response = CalendarEventListMethod.ListEvent(calListRequest);
				context.SetResponse(response);
			}
			else
			{
				base.List(context);
			}
		}

		protected override void Load(BusinessContext context)
		{
			LoadRequest loadRequest = context.Request as LoadRequest;
			bool forceBase = loadRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				LoadResponse response = new LoadResponse();
				EntityObject entity = EventHelper.PrerpareLoadEventRequest(loadRequest);
				response.EntityObject = entity;
				context.SetResponse(response);
			}
			else
			{
				base.Load(context);
			}
		}


		protected override void PostCreate(BusinessContext context)
		{
			CreateRequest createRequest = context.Request as CreateRequest;
			bool forceBase = createRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				int principalId = Security.CurrentUserId;
				if (principalId != -1)
				{
					//—оздаем организатора эвента
					CalendarEventResourceEntity organizator = new CalendarEventResourceEntity();
					organizator.PrincipalId = principalId;
					organizator.EventId = ((CreateResponse)context.Response).PrimaryKeyId;
					organizator.ResourceEventOrganizator = true;
					organizator.Name = GetResourceTitle(principalId);
					BusinessManager.Create(organizator);
				}

			}
			base.PostCreate(context);

		} 
		#endregion

		private static string GetResourceTitle(int principalId)
		{
			string retVal = String.Empty;
			EntityObject entityObject = BusinessManager.Load(Principal.GetAssignedMetaClass().Name, principalId);
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(Principal.GetAssignedMetaClass().Name);
			if (entityObject != null && entityObject.Properties[mc.TitleFieldName] != null 
				&& entityObject[mc.TitleFieldName] != null)
			{
				retVal = entityObject[mc.TitleFieldName].ToString();
			}
			return retVal;
		}
	}
}
