using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.Ibn;


namespace Mediachase.IBN.Business
{
	public class IMManager
	{
		public static void LogOff(int userId)
		{
			if (PortalConfig.UseIM)
				IMHelper.LogOff(userId);
		}

		public static void UpdateGroup(int groupId)
		{
			if (PortalConfig.UseIM)
				IMHelper.UpdateGroup(groupId);
		}

		public static void UpdateUser(int userId)
		{
			if (PortalConfig.UseIM)
				IMHelper.UpdateUser(userId);
		}

		public static void UpdateUserWebStub(int userId)
		{
			if (PortalConfig.UseIM)
				IMHelper.UpdateUserWebStub(userId);
		}
	}
}
