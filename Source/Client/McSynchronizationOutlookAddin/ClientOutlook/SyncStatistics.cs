using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Synchronization;

namespace Mediachase.ClientOutlook
{
	/// <summary>
	/// Represent sync statistic
	/// </summary>
	public class SyncStatistics
	{
		private SyncOperationStatistics InnerSyncStatistics { get; set; }

		public SyncStatistics()
		{
		}

		public SyncStatistics(SyncOperationStatistics innerStats)
		{
			InnerSyncStatistics = innerStats;
		}

		
		public int DownloadChangesApplied 
		{
			get
			{
				int retVal = 0;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.DownloadChangesApplied;
				}
				return retVal;
			}
		}
		
		public int DownloadChangesFailed 
		{
			get
			{
				int retVal = 0;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.DownloadChangesFailed;
				}
				return retVal;
			}
		}
		
		public int DownloadChangesTotal 
		{
			get
			{
				int retVal = 0;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.DownloadChangesTotal;
				}
				return retVal;
			}
		}
	
		public DateTime SyncEndTime 
		{
			get
			{
				DateTime retVal = DateTime.MinValue;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.SyncEndTime;
				}
				return retVal;
			}
		}
		
		public DateTime SyncStartTime 
		{
			get
			{
				DateTime retVal = DateTime.MinValue;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.SyncStartTime;
				}
				return retVal;

			}
		}
		
		public int UploadChangesApplied 
		{
			get
			{
				int retVal = 0;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.UploadChangesApplied;
				}
				return retVal;

			}
		}
		
		public int UploadChangesFailed 
		{
			get
			{
				int retVal = 0;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.UploadChangesFailed;
				}
				return retVal;
			}
		}
		
		public int UploadChangesTotal 
		{
			get
			{
				int retVal = 0;
				if (InnerSyncStatistics != null)
				{
					retVal = InnerSyncStatistics.UploadChangesTotal;
				}
				return retVal;
			}
		}
	}

}
