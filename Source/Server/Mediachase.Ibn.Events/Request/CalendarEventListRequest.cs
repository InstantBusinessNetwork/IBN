using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events.CustomMethods;
using Mediachase.Ibn.Events.CustomMethods.List;

namespace Mediachase.Ibn.Events.Request
{
	public class CalendarEventListRequest : Mediachase.Ibn.Core.Business.Request
	{
		public const string FILTERS_PARAM = "Filters";
		public const string SORTING_PARAM = "Sorting";
		public const string DTSTART_TZID_PARAM = "DTStartTZID";
		public const string DTEND_TZID_PARAM = "DTEndTZID";
	

	
		public CalendarEventListRequest()
			:base(CalendarEventListMethod.METHOD_NAME,null)
		{
		}

		public CalendarEventListRequest(string metaClassName, FilterElement[] filters)
			: base(CalendarEventListMethod.METHOD_NAME, new EntityObject(metaClassName))
		{
			this.Filters = filters;
		}

		public CalendarEventListRequest(EntityObject target, FilterElement[] filters)
			: base(CalendarEventListMethod.METHOD_NAME, target)
		{
			this.Filters = filters;
		}

		public CalendarEventListRequest(string metaClassName, FilterElement[] filters, SortingElement[] sorting)
			: base(CalendarEventListMethod.METHOD_NAME, new EntityObject(metaClassName))
		{
			this.Filters = filters;
			this.Sorting = sorting;
		}

		public CalendarEventListRequest(EntityObject target, FilterElement[] filters, SortingElement[] sorting)
			: base(CalendarEventListMethod.METHOD_NAME, target)
		{
			this.Filters = filters;
			this.Sorting = sorting;
		}

		
		public CalendarEventListRequest(string metaClassName,SortingElement[] sorting)
			: base(CalendarEventListMethod.METHOD_NAME, new EntityObject(metaClassName))
		{
			this.Sorting = sorting;
		}

	
		public CalendarEventListRequest(EntityObject target, SortingElement[] sorting)
			: base(CalendarEventListMethod.METHOD_NAME, target)
		{
			this.Sorting = sorting;
		}


		public String DTStartTimeZoneId
		{
			get
			{
				return base.Parameters.GetValue<String>(DTSTART_TZID_PARAM, null);
			}

			set
			{
				base.Parameters.Add(DTSTART_TZID_PARAM, value);
			}
		}

		public String DTEndTimeZoneId
		{
			get
			{
				return base.Parameters.GetValue<String>(DTEND_TZID_PARAM, null);
			}

			set
			{
				base.Parameters.Add(DTEND_TZID_PARAM, value);
			}
		}

		public FilterElement[] Filters
		{
			get 
			{
				return base.Parameters.GetValue<FilterElement[]>(CalendarEventListRequest.FILTERS_PARAM, new FilterElement[]{});
			}
			set 
			{
				base.Parameters.Add(CalendarEventListRequest.FILTERS_PARAM, value);
			}
		}

		public SortingElement[] Sorting
		{
			get
			{
				return base.Parameters.GetValue<SortingElement[]>(CalendarEventListRequest.SORTING_PARAM, new SortingElement[] { });
			}
			set
			{
				base.Parameters.Add(CalendarEventListRequest.SORTING_PARAM, value);
			}
		}


	}
}
