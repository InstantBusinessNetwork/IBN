using System;

namespace Mediachase.Schedule
{
	public interface IScheduleClient
	{
		void Invoke(string applicationName);
	}

	public delegate void ClientScheduleChangedEventHandler(IScheduleClient sender);

	//public interface IScheduleClient2
	//{
	//	void Invoke2(string applicationName);
	//}
}
