using System;
using System.Collections;

namespace Mediachase.Schedule.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class InvocationTimer
	{
		private int _interval = 60000;
		//private Hashtable _clients = new Hashtable();

		public InvocationTimer()
		{
		}

		public void Reset()
		{
			//_clients.Clear();
		}

		//public void AddClient(string applicationName, string assembly, string type, string interval, string timeout, string objectUri, string authentication, string domain, string user, string password)
		//{
		//    string key = string.Format("{0}_{1}_{2}_{3}", applicationName, assembly, type, objectUri);
		//    if(!_clients.ContainsKey(key))
		//        _clients[key] = new ClientInfo(applicationName, assembly, type, interval, timeout,  objectUri, authentication, domain, user, password);
		//}

		public int Interval
		{
			get { return _interval; }
			set { _interval = value; }
		}

		//public ClientInfo[] Clients
		//{
		//    get
		//    {
		//        ClientInfo[] clients = new ClientInfo[_clients.Values.Count];
		//        _clients.Values.CopyTo(clients, 0);
		//        return clients;
		//    }
		//}
	}
}
