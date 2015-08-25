using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookAddin.OutlookUI
{
	public enum eSyncStatus
	{
		Unknow,
		Ok,
		Ready,
		InProgress,
		ReadyProgress,
		Canceled,
		UnresolvedConflict,
		Failed,
		SkipedChangesDetected
	}

	public class SyncItemInfo
	{
		public DateTime LastSyncDate { get; set; }
		public string ErrorDescr { get; set; }
		public int SkippedCount { get; set; }
		

		public eSyncStatus Status { get; set; }
		public Dictionary<string, object> Properties { get; set; }

		public SyncItemInfo()
		{
			Properties = new Dictionary<string, object>();
		}

	}

	public enum eSettingItem_State
	{
		Normal,
		Selected,
		Hover,
		Disabled
	}

	public enum eMenuSettings_Icon
	{
		Connection,
		SyncItems
	}

	public enum eMenuSyncItemSettings_Icon
	{
		Calendar		= 1,
		Contact			= 2,
		Task			= 0,
		Note			= 3
	}

	public enum eSyncMenuItem_Icon
	{
		//Calendar images
		Calendar_ok				= 0,
		Calendar_ready			= 1,
		Calendar_sync			= 2,
		Calendar_sync_1			= 3,
		Calendar_sync_2			= 4,
		Calendar_sync_3			= 5,
		Calendar_conflict		= 6,
		Calendar_canceled		= 7,
		Calendar_failed			= 8,
		Calendar_unknow			= 1,

		//Contact images
		Contact_ok				= 18,
		Contact_ready			= 19,
		Contact_sync			= 20,
		Contact_sync_1			= 21,
		Contact_sync_2			= 22,
		Contact_sync_3			= 23,
		Contact_conflict		= 25,
		Contact_canceled		= 24,
		Contact_failed			= 26,
		Contact_unknow			= 19,


		//Task images
		Task_ok					= 9,
		Task_ready				= 10,
		Task_sync				= 11,
		Task_sync_1				= 12,
		Task_sync_2				= 13,
		Task_sync_3				= 14,
		Task_conflict			= 16,
		Task_canceled			= 15,
		Task_failed				= 17,
		Task_unknow				= 10,

		//Notes images
		Note_ok					= 27,
		Note_sync				= 27,
		Note_sync_1				= 27,
		Note_sync_2				= 27,
		Note_sync_3				= 27,
		Note_conflict			= 27,
		Note_canceled			= 27,
		Note_failed				= 27,
		Note_unknow				= 27

	}

	public class SyncItemEventArgs : EventArgs
	{
		public Outlook.OlItemType oItemType { get; set; }

		public SyncItemEventArgs(Outlook.OlItemType itemType)
		{
			oItemType = itemType;
		}
	}
}
