using System;
using Mediachase.IBN.Database;
using System.Data;

namespace Mediachase.IBN.Business
{
	public enum FullTextSearchPopulateStatus
	{
		Idle = 0,
		FullPopulationInProgress = 1,
		Paused = 2,
		Throttled = 3,
		Recovering =4,
		Shutdown = 5,
		IncrementalPopulationInProgress  = 6,
		BuildingIndex = 7,
		DiskIsFullPaused = 8,
		ChangeTracking = 9
	}

	public class FullTextSearchInfo
	{
		/// <summary>
		/// Gets size of the full-text index in megabytes.
		/// </summary>
		public int IndexSize;

		/// <summary>
		/// Gets populate status.
		/// </summary>
		public FullTextSearchPopulateStatus PopulateStatus;
	}

	/// <summary>
	/// Summary description for FullTextSearch.
	/// </summary>
	public class FullTextSearch
	{
		private FullTextSearch()
		{
		}

		/// <summary>
		/// Verifies that Microsoft Search Service is installed.
		/// </summary>
		/// <returns></returns>
		public static bool IsInstalled()
		{
			using(IDataReader reader = DbFullTextSearch.IsInstalled())
			{
				if(reader.Read())
				{
					if(((int)reader[0])==1)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Activates this instance.
		/// </summary>
		public static void Activate()
		{
			DbFullTextSearch.Activate();
		}

		/// <summary>
		/// Deactivates this instance.
		/// </summary>
		public static void Deactivate()
		{
			DbFullTextSearch.Deactivate();
		}

		/// <summary>
		/// Determines whether this instance is active.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsActive()
		{
			using(IDataReader reader = DbFullTextSearch.GetActive())
			{
				if(reader.Read())
				{
					if(((int)reader[0])==1)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the information.
		/// </summary>
		/// <returns></returns>
		public static FullTextSearchInfo GetInformation()
		{
			if(!IsActive())
				return null;

			FullTextSearchInfo retVal = new FullTextSearchInfo(); 

			using(IDataReader reader = DbFullTextSearch.GetInfo())
			{
				if(reader.Read())
				{
					retVal.PopulateStatus = (FullTextSearchPopulateStatus)(int)reader[0];
					retVal.IndexSize = (int)reader[1];
				}
			}

			return retVal;
		}
	}
}
