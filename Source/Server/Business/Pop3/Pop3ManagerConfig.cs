using System;
using System.Collections;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3ManagerConfig.
	/// </summary>
	public class Pop3ManagerConfig
	{
		public Pop3MessageHandlerInfoList _handlers = new Pop3MessageHandlerInfoList();

		public Pop3ManagerConfig()
		{
		}

		public Pop3MessageHandlerInfoList Handlers
		{
			get
			{
				return _handlers;
			}
		}
	}
}
