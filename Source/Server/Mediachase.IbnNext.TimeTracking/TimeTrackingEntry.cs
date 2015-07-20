using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IbnNext.TimeTracking
{
	partial class TimeTrackingEntry
	{
		public const string SecurityFilterId = "{2367EBF5-A976-42ee-AFC2-740EC97DD73E}";

		#region CreateSecurityFilterElement
		/// <summary>
		/// Creates the security filter element, that check security from TimeTrackingBlock
		/// </summary>
		/// <returns></returns>
		public static FilterElement CreateSecurityFilterElement()
		{
			FilterElement retVal = new FilterElement("ParentBlockId", FilterElementType.In, "{TimeTrackingSecurity:CanRead}", true);

			retVal.Uid = new Guid(SecurityFilterId);

			return retVal;
		}
		#endregion

		#region CreateStateFriendlyNameMetaField
		public static void CreateStateFriendlyNameMetaField()
		{
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass timeTrackingEntry = TimeTrackingEntry.GetAssignedMetaClass();

				AttributeCollection attr = new AttributeCollection();
				attr.Add(McDataTypeAttribute.StringMaxLength, 255);
				attr.Add(McDataTypeAttribute.StringIsUnique, false);
				attr.Add(McDataTypeAttribute.Expression,
						@"SELECT TOP 1 TTBS.FriendlyName FROM cls_TimeTrackingBlock_State TTBS" + Environment.NewLine +
						@"	JOIN cls_TimeTrackingBlock TTB ON " + Environment.NewLine +
						@"	TTB.mc_StateId = TTBS.TimeTrackingBlock_StateId" + Environment.NewLine +
						@"	AND" + Environment.NewLine +
						@"	TTB.[TimeTrackingBlockId] = AAA.[ParentBlockId]");

				MetaField retVal = timeTrackingEntry.CreateMetaField("StateFriendlyName",
					"State", MetaFieldType.Text, true, "''", attr);

				scope.SaveChanges();
			}
		}
		#endregion

		#region AddStateFriendlyNameMetaFieldToAllMetaView
		public static void AddStateFriendlyNameMetaFieldToAllMetaView()
		{
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				MetaClass timeTrackingEntry = TimeTrackingEntry.GetAssignedMetaClass();

				MetaField mf = timeTrackingEntry.Fields["StateFriendlyName"];

				foreach (MetaView mv in DataContext.Current.MetaModel.MetaViews.GetByMetaClass(timeTrackingEntry))
				{
					mv.AvailableFields.Add(mf);
				}

				scope.SaveChanges();
			}
		}
		#endregion

		#region OnSaving
		protected override void OnSaving()
		{
			int blockId = this.ParentBlockId;

			// O.R. [2008-07-29]
			if (!SkipSecurityCheckScope.IsActive)
			{
				// Check rights
				if (!TimeTrackingBlock.CheckUserRight(blockId, Security.RightWrite))
					throw new AccessDeniedException();
			}

			// O.R. [2008-08-04]: If finances are registered, set TotalApproved for new entries to zero.
			// So we can see the differences in the "Accounts" tab of info popup 
			TimeTrackingBlock block = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), blockId);
			if ((bool)block.Properties["AreFinancesRegistered"].Value && this.Properties["TotalApproved"].Value == null)
				this.Properties["TotalApproved"].Value = 0;

			base.OnSaving();
		}
		#endregion

		#region OnSaved
		protected override void OnSaved()
		{
			// Rebuild Time Tracking Block Day1, Day2, Day3, ... , Day7
			TimeTrackingBlock.RecalculateDays(this.ParentBlockId);

			base.OnSaved();
		}
		#endregion

		#region OnDeleting
		protected override void OnDeleting()
		{
			// O.R. [2008-07-29]
			if (!SkipSecurityCheckScope.IsActive)
			{
				// Check rights
				int blockId = this.ParentBlockId;
				if (!TimeTrackingBlock.CheckUserRight(blockId, Security.RightDelete))
					throw new AccessDeniedException();
			}

			base.OnDeleting();
		}
		#endregion

		#region OnDeleted
		protected override void OnDeleted()
		{
			// Rebuild Time Tracking Block Day1, Day2, Day3, ... , Day7
			TimeTrackingBlock.RecalculateDays(this.ParentBlockId);

			base.OnDeleted();
		}
		#endregion
	}
}
