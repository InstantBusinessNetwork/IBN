using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbActiveDirectory
	{
		private DbActiveDirectory(){}

		#region LocalAddressRangeAdd()
		public static int LocalAddressRangeAdd(string startAddress, string endAddress)
		{
			return DbHelper2.RunSpInteger("LocalAddressRangeAdd",
				DbHelper2.mp("@StartAddress", SqlDbType.NVarChar, 32, startAddress),
				DbHelper2.mp("@EndAddress", SqlDbType.NVarChar, 32, endAddress));
		}
		#endregion

		#region LocalAddressRangeUpdate()
		public static void LocalAddressRangeUpdate(int rangeId, string startAddress, string endAddress)
		{
			DbHelper2.RunSp("LocalAddressRangeUpdate",
				DbHelper2.mp("@RangeId", SqlDbType.Int, rangeId),
				DbHelper2.mp("@StartAddress", SqlDbType.NVarChar, 32, startAddress),
				DbHelper2.mp("@EndAddress", SqlDbType.NVarChar, 32, endAddress));
		}
		#endregion

		#region LocalAddressRangeDelete()
		public static void LocalAddressRangeDelete(int rangeId)
		{
			DbHelper2.RunSp("LocalAddressRangeDelete", DbHelper2.mp("@RangeId",
				SqlDbType.Int, rangeId));
		}
		#endregion

		#region LocalAddressRangeGetList()
		public static IDataReader LocalAddressRangeGetList()
		{
			return DbHelper2.RunSpDataReader("LocalAddressRangeGetList");
		}
		#endregion
	}
}
