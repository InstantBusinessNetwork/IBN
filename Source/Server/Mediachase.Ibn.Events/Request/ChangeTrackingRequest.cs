using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Events.CustomMethods.ChangeTracking;

namespace Mediachase.Ibn.Events.Request
{
	public class ChangeTrackingRequest : CalendarRequest
	{
		public const string STATUS_PARAM = "Status";
		public const string SENDNOTIFY_PARAM = "Sendnotify";

		public ChangeTrackingRequest(EntityObject target)
			:base(ChangeTrackingMethod.METHOD_NAME, target)
		{
		}
		public ChangeTrackingRequest(EntityObject target, string recurrenceId)
			: base(ChangeTrackingMethod.METHOD_NAME, target, recurrenceId)
		{
		}

		public bool SendNotify
		{
			get
			{
				return base.Parameters.GetValue<bool>(SENDNOTIFY_PARAM, true);
			}

			set
			{
				base.Parameters.Add(SENDNOTIFY_PARAM, value);
			}
		}

		public eResourceStatus Status
		{
			get 
			{
				return base.Parameters.GetValue<eResourceStatus>(STATUS_PARAM, eResourceStatus.NotResponded);
			}

			set
			{
				base.Parameters.Add(STATUS_PARAM, value);
			}
		}
	}
}
