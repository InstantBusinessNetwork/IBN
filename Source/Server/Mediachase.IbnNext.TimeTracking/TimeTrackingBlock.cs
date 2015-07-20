using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Data;
using System.Globalization;
using System.Web;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Sql;
using System.Data.SqlClient;
using Mediachase.Ibn.Data.Services;

namespace Mediachase.IbnNext.TimeTracking
{
	partial class TimeTrackingBlock
	{
		protected override void OnDeleting()
		{
			// Remove Assigned Entry
			TimeTrackingEntry[] entryList = TimeTrackingEntry.List(FilterElement.EqualElement("ParentBlockId", this.PrimaryKeyId.Value));

			foreach (TimeTrackingEntry entry in entryList)
			{
				entry.Delete();
			}

			base.OnDeleting();
		}

		protected override void OnSaving()
		{
			// Call Base Method => Call Service PreSaveMethod
			base.OnSaving();

			if (this.Properties.Contains("IsRejected") &&
				this.IsServiceActivated<StateMachineService>())
			{
				StateMachineService stateMachine = this.GetService<StateMachineService>();

				if (stateMachine != null && 
					this.Properties["mc_StateId"].IsChanged)
				{
					// Get Name
					string oldState = (string)this.Properties["mc_State"].OriginalValue;
					string newState = (string)this.Properties["mc_State"].Value;

					// Get State Machine Index
					int oldStateIndex = stateMachine.StateMachine.GetStateIndex(oldState);
					int newStateIndex = stateMachine.StateMachine.GetStateIndex(newState);

					// Update IsRejectedState state field.
					this.Properties["IsRejected"].Value = (oldStateIndex > newStateIndex);
				}
			}

			// OZ: Clean up from HttpContext
			if (this.PrimaryKeyId.HasValue)
			{
				ClearUserRight(this.PrimaryKeyId.Value, Security.RightRead);
				ClearUserRight(this.PrimaryKeyId.Value, Security.RightWrite);
			}

			string cacheKey = this.ToString() + "_" + Security.CurrentUserId.ToString() + "_AllowedRights";
			DataContext.Current.Cache.Remove(cacheKey);
		}

		#region CheckUserRight
		protected static void ClearUserRight(int timeTrackingBlockId, string rightName)
		{
			if (Mediachase.Ibn.License.TimeTrackingModule)
			{
				string key = string.Format(CultureInfo.InvariantCulture, "_ttb_{0}_right_{1}",timeTrackingBlockId, rightName);

				if (HttpContext.Current != null)
					HttpContext.Current.Items[key] = null;
			}
		}

		public static bool CheckUserRight(int timeTrackingBlockId, string rightName)
		{
			if (rightName == null)
				throw new ArgumentNullException("rightName");

			bool bRetVal = false;
			if (Mediachase.Ibn.License.TimeTrackingModule)
			{

				string key = string.Format(CultureInfo.InvariantCulture, "_ttb_{0}_right_{1}",
					timeTrackingBlockId, rightName);

				if (HttpContext.Current == null || HttpContext.Current.Items[key] == null)
				{
					TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), timeTrackingBlockId);
					SecurityService ss = ttb.GetService<SecurityService>();

					if (ss != null)
						bRetVal = ss.CheckUserRight(rightName);

					if (HttpContext.Current != null)
						HttpContext.Current.Items[key] = bRetVal;
				}
				else
				{
					bRetVal = (bool)HttpContext.Current.Items[key];
				}
			}

			return bRetVal;
		}

		public static bool CheckUserRight(TimeTrackingBlock ttb, string rightName)
		{
			if (rightName == null)
				throw new ArgumentNullException("rightName");

			if (ttb == null)
				throw new ArgumentNullException("ttb");

			bool bRetVal = false;
			if (Mediachase.Ibn.License.TimeTrackingModule)
			{
				string key = string.Format(CultureInfo.InvariantCulture, "_ttb_{0}_right_{1}",
					ttb.PrimaryKeyId.ToString(), rightName);

				if (HttpContext.Current == null || HttpContext.Current.Items[key] == null)
				{
					SecurityService ss = ttb.GetService<SecurityService>();

					if (ss != null)
						bRetVal = ss.CheckUserRight(rightName);

				    if (HttpContext.Current != null)
				        HttpContext.Current.Items[key] = bRetVal;
				}
				else
				{
				    bRetVal = (bool)HttpContext.Current.Items[key];
				}
			}

			return bRetVal;
		}
		#endregion

		#region RecalculateDays
		public static void RecalculateDays(int timeTrackingBlockId)
		{
			SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
				"TimeTrackingBlockRecalculateDay", new SqlParameter("@TimeTrackingBlockId", timeTrackingBlockId));
		}
		#endregion
	}
}
