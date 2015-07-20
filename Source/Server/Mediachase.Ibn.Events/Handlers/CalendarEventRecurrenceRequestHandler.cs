using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.CustomMethods;
using Mediachase.Ibn.Events.Request;
using Mediachase.Ibn.Events.McCalendar.Components;

namespace Mediachase.Ibn.Events.Handlers
{
	/// <summary>
	/// Represents CalendarEventRecurrence request handler.
	/// </summary>
	public class CalendarEventRecurrenceRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarEventRecurrenceRequestHandler"/> class.
		/// </summary>
		public CalendarEventRecurrenceRequestHandler()
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
			if (metaClassName == CalendarEventRecurrenceEntity.ClassName)
			{
				CalendarEventRecurrenceEntity retVal = new CalendarEventRecurrenceEntity();
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
		protected override void PreUpdate(BusinessContext context)
		{
			CalendarEventRecurrenceEntity recurrenceEntity = context.Request.Target as CalendarEventRecurrenceEntity;
			UpdateRequest updateRequest = context.Request as UpdateRequest;
			bool forceBase = updateRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				PrimaryKeyId pk = EventHelper.PrepareUpdateRecurrenceRequest(updateRequest);
				context.Request.Target.PrimaryKeyId = pk;
			}

			base.PreUpdate(context);
		}

		protected override void Delete(BusinessContext context)
		{
			CalendarEventRecurrenceEntity recurrenceEntity = context.Request.Target as CalendarEventRecurrenceEntity;
			DeleteRequest deleteRequest = context.Request as DeleteRequest;
			bool forceBase = deleteRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if(!forceBase)
			{
				Mediachase.Ibn.Events.CustomMethods.EventHelper.DeleteRecurrenceRequestHandle(deleteRequest);
			}
			else
			{
				base.Delete(context);
			}
		}

		protected override void PreList(BusinessContext context)
		{
			ListRequest listRequest = context.Request as ListRequest;
			bool forceBase = listRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				object filterValue = EventHelper.RecursiveFindFilterElementValue(listRequest.Filters,
																				CalendarEventRecurrenceEntity.FieldEventId);
				if (filterValue != null)
				{
					//try first load exception
					PrimaryKeyId eventId = PrimaryKeyId.Parse(filterValue.ToString());
					if (EventHelper.LoadCalEvent(eventId) == null)
					{
						//convert him to series id)
						eventId = (PrimaryKeyId)(VirtualEventId)eventId;
					}
					listRequest.Filters = EventHelper.RecursiveReplaceFilterElementValue(listRequest.Filters,
																						CalendarEventRecurrenceEntity.FieldEventId,
																						eventId);
				}
			}
			base.PreList(context);
		}

		protected override void Create(BusinessContext context)
		{
			CreateRequest createRequest = context.Request as CreateRequest;
			bool forceBase = createRequest.Parameters.GetValue<bool>(EventHelper.FORCE_BASE_PARAM, false);
			if (!forceBase)
			{
				CreateResponse response = Mediachase.Ibn.Events.CustomMethods.EventHelper.CreateRecurrenceRequestHandle(createRequest);
				context.SetResponse(response);
			}
			else
			{
				base.Create(context);
			}
		}
		#endregion
	}
}
