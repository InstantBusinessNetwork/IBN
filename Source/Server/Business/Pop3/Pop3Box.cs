using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;

using Mediachase.IBN.Database.Pop3;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3Box.
	/// </summary>
	[Serializable]
	public class Pop3Box
	{
		private int _id;
		private string _name = string.Empty;
		private string _server = string.Empty;
		private int _port = 110;
		private string _login = string.Empty;
		private string _password = string.Empty;
		private int _interval = 0;
		private DateTime _lastRequest = DateTime.MinValue;
		private DateTime _lastSuccessfulRequest = DateTime.MinValue;
		private string _lastErrorText = String.Empty;
		private bool _active = false;
		private bool _AutoKillForRead = true;
		private Pop3Parameters _parameters = null;

		private Pop3MessageHandlerInfoList _handlers = new Pop3MessageHandlerInfoList();

		private bool _modified = false;

		public int Id { get { return _id; }}

		public string Name { 
			get 
			{ 
				return _name; 
			} 
			set
			{
				_modified = true;
				_name = value;
			}
		}

		public string Server 
		{ 
			get 
			{ 
				return _server; 
			} 
			set
			{
				_modified = true;
				_server = value;
			}
		}

		public int Port 
		{ 
			get 
			{ 
				return _port; 
			}
			set
			{
				_modified = true;
				_port = value;
			}
		}

		public string Login 
		{ 
			get 
			{ 
				return _login; 
			} 
			set
			{
				_modified = true;
				_login = value;
			}
		}

		public string Password 
		{ 
			get 
			{ 
				return _password; 
			} 
			set
			{
				_modified = true;
				_password = value;
			}
		}

		public int Interval 
		{ 
			get 
			{ 
				return _interval; 
			} 
			set
			{
				_modified = true;
				_interval = value;
			}
		}

		public DateTime LastRequest { get { return _lastRequest; } }

		public DateTime LastSuccessfulRequest { get { return _lastSuccessfulRequest; } }

		public string LastErrorText { get { return _lastErrorText; } }

		public Pop3Parameters Parameters
		{
			get
			{
				if (_parameters==null)
				{
					LoadParameters();
				}
				return _parameters;
			}
		}

		public Pop3MessageHandlerInfoList Handlers { get { return _handlers; } }

		public bool IsActive 
		{ 
			get 
			{ 
				return _active; 
			} 
			set
			{
				_modified = true;
				_active = value;
			}
		}
		
		public bool Modified { get { return _modified; }}

		public bool AutoKillForRead
		{
			get
			{
				return _AutoKillForRead;
			}
			set
			{
				_AutoKillForRead = value;
				_modified = true;
			}
		}

		public Pop3Box()
		{
			_id = -1;
			_modified = true;

		}
		public Pop3Box(IDataReader reader)
		{
			_id = (int)reader["Pop3BoxId"];
			if (reader["Name"]!=DBNull.Value)
			{
				_name = (string)reader["Name"];
			}
			_server = (string)reader["Server"];
			_port = (int)reader["Port"];
			_login = (string)reader["Login"];
			_password = (string)reader["Password"];
			_interval = (int)reader["Interval"];
			if (reader["LastRequest"]!=DBNull.Value)
			{
				_lastRequest = (DateTime)reader["LastRequest"];
			}
			if (reader["LastSuccessfulRequest"]!=DBNull.Value)
			{
				_lastSuccessfulRequest = (DateTime)reader["LastSuccessfulRequest"];
			}
			if (reader["LastErrorText"]!=DBNull.Value)
			{
				_lastErrorText = (string)reader["LastErrorText"];
			}
			_active = (bool)reader["Active"];
			_AutoKillForRead = (bool)reader["AutoKillForRead"];
		}

		public void ResetModified()
		{
			_modified = false;
		}

		private void LoadParameters()
		{
			if (_parameters==null)
			{
				_parameters = new Pop3Parameters(this);
			}
		}

		public void AddHandler(Pop3MessageHandlerInfo handlerInfo)
		{
			if (handlerInfo != null)
				_handlers.Add(handlerInfo);
		}

		public void Request()
		{
			System.Diagnostics.Trace.WriteLine("Pop3Box.Request()");

			_lastRequest = DateTime.Now;
			_lastErrorText = String.Empty;

			try
			{
				System.Diagnostics.Trace.WriteLine(string.Format("Dns.GetHostByName({0})", _server));

				//IPHostEntry hostInfo = Dns.GetHostEntry(_server);
				IPAddress[] addresses = Dns.GetHostAddresses(_server);

				if (addresses.Length > 0)
				{
					IPEndPoint pop3ServerEndPoint = new IPEndPoint(addresses[0], _port);

					Pop3Connection pop3Connection = new Pop3Connection();
					
					pop3Connection.Open(pop3ServerEndPoint);
					System.Diagnostics.Trace.WriteLine(string.Format("pop3Connection.Open - OK"));

					pop3Connection.User(_login);
					System.Diagnostics.Trace.WriteLine(string.Format("pop3Connection.Login - OK"));

					pop3Connection.Pass(_password);
					System.Diagnostics.Trace.WriteLine(string.Format("pop3Connection.Password - OK"));

					Pop3Stat stat = pop3Connection.Stat();
					System.Diagnostics.Trace.WriteLine(string.Format("Message Count = {0}", stat.MessageCout));

					Exception innerException = null;

					if(stat.MessageCout > 0)
					{
						Pop3UIDInfoList messageList = pop3Connection.Uidl();
						foreach(Pop3UIDInfo mi in messageList)
						{
							System.Diagnostics.Trace.WriteLine(string.Format("Message ID = {0}, UID = {1}", mi.ID, mi.UID));

							if (!_AutoKillForRead && DBPop3Box.CheckMessageUId(_id, mi.UID))
								continue;

							Pop3Message message = pop3Connection.Retr(mi.ID);

							System.Diagnostics.Trace.WriteLine(string.Format("pop3Connection.Retr - OK"));

							foreach(Pop3MessageHandlerInfo handlerInfo in _handlers)
							{
								try
								{
									handlerInfo.Handler.ProcessPop3Message(this, message);

									if (_AutoKillForRead)
									{
										pop3Connection.Dele(mi.ID);
									}
									else
									{
										DBPop3Box.AddMessageUId(_id, mi.UID);
									}

									System.Diagnostics.Trace.WriteLine(string.Format("Handler.ProcessPop3Message - OK"));
								}
								catch(Exception ex)
								{
									System.Diagnostics.Trace.WriteLine(ex);
									innerException = ex;
								}
							}
						}
					}

					pop3Connection.Quit();

					if(innerException!=null)
						throw innerException;

					System.Diagnostics.Trace.WriteLine("Quit - OK");
				}
				else
					throw new ArgumentException(string.Format("Could not find server {0}.", _server) );

				_lastSuccessfulRequest = _lastRequest;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				_lastErrorText = ex.Message;
			}

			System.Diagnostics.Trace.WriteLine(string.Format("DBPop3Box.UpdateTime(id = {0}, lastRequest = {1}, lastSuccessfulRequest = {2}, lastErrorText = {3})", _id, _lastRequest, _lastSuccessfulRequest, _lastErrorText));
			DBPop3Box.UpdateTime(_id, _lastRequest, _lastSuccessfulRequest, _lastErrorText);
		}

		public bool IsBoxModified()
		{
			return this.Modified || Handlers.IsModified || Parameters.IsModified;
		}
	}
}
