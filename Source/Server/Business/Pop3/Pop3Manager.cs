using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.Pop3;

using System.Net;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for Pop3Manager.
	/// </summary>
	public class Pop3Manager
	{
		private static Pop3Manager	_current = null;

		private Pop3ManagerConfig	_config  = null;
		private Thread				_thread = null;
		private AutoResetEvent		_stopEvent = new AutoResetEvent(false);

		[ThreadStatic]
		private static Pop3Box	_selectedPop3Box = null;

		/// <summary>
		/// Initializes the <see cref="Pop3Manager"/> class.
		/// </summary>
		static Pop3Manager()
		{
			Pop3ManagerConfig config = (Pop3ManagerConfig)ConfigurationManager.GetSection("pop3Modules");
			_current = new Pop3Manager(config);

			// Fixed: Remove Handler.Init from constructor !!! _current ==  null
			_current.Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Pop3Manager"/> class.
		/// </summary>
		/// <param name="config">The config.</param>
		private Pop3Manager(Pop3ManagerConfig config)
		{
			Pop3Manager._current = this;
			_config = config;

		}

		/// <summary>
		/// Inits this instance.
		/// </summary>
		private void Init()
		{
			Pop3MessageHandlerInfoList infos = this.Config.Handlers;

			foreach(Pop3MessageHandlerInfo info in infos)
			{
				try
				{
					info.Handler.Init(this);
				}
				catch(Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(ex,"Pop3Manager");
				}
				
			}
		}

		/// <summary>
		/// Gets the current Pop3Manager.
		/// </summary>
		/// <value>The current.</value>
		public static Pop3Manager Current 
		{ 
			get 
			{ 
				return _current; 
			} 
		}

		/// <summary>
		/// Gets the config.
		/// </summary>
		/// <value>The config.</value>
		public Pop3ManagerConfig Config 
		{ 
			get 
			{ 
				return _config; 
			} 
		}

		public Pop3Box SelectedPop3Box
		{
			get
			{
				return _selectedPop3Box;
			}
			set
			{
				_selectedPop3Box = value;
			}
		}

		/// <summary>
		/// Gets the POP3 box.
		/// </summary>
		/// <param name="pop3BoxId">The POP3 box id.</param>
		/// <returns></returns>
		public Pop3Box GetPop3Box(int pop3BoxId)
		{
			foreach(Pop3Box box in this.GetPop3BoxList())
			{
				if(box.Id == pop3BoxId)
					return box;
			}
			return null;
		}

		/// <summary>
		/// Gets the POP3 box.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public Pop3Box GetPop3Box(string name)
		{
			foreach(Pop3Box box in this.GetPop3BoxList())
			{
				if(box.Name == name)
					return box;
			}
			return null;
		}

		/// <summary>
		/// Gets the POP3 box list.
		/// </summary>
		/// <returns></returns>
		public Pop3Box[] GetPop3BoxList()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DBPop3Box.GetList())
			{
				Pop3Box box = null;
				int     boxId = -1;
				while (reader.Read())
				{
					if (boxId!=(int)reader["Pop3BoxId"])
					{
						if(box!=null)
							box.Handlers.ResetModified();

						box = new Pop3Box(reader);
						list.Add(box);
					}
					if (reader["HandlerName"]!=DBNull.Value)
					{
						box.AddHandler(_config.Handlers[(string)reader["HandlerName"]]);
					}
				}
			}
			return (Pop3Box[])list.ToArray(typeof(Pop3Box));
		}

		/// <summary>
		/// Removes the POP3 box.
		/// </summary>
		/// <param name="box">The box.</param>
		public void RemovePop3Box(Pop3Box box)
		{
			RemovePop3Box(box.Id);
		}

		/// <summary>
		/// Removes the POP3 box.
		/// </summary>
		/// <param name="Pop3BoxId">The POP3 box id.</param>
		public void RemovePop3Box(int Pop3BoxId)
		{
			DBPop3Box.Delete(Pop3BoxId);
		}

		/// <summary>
		/// Updates the POP3 box.
		/// </summary>
		/// <param name="box">The box.</param>
		/// <returns></returns>
		public Pop3Box UpdatePop3Box(Pop3Box box)
		{
			if (box.IsBoxModified())
			{
				using (DbTransaction tra = DbTransaction.Begin())
				{
					int BoxId = box.Id;

					if (box.Modified)
					{
						if (BoxId == -1)
						{
							using (IDataReader reader = DBPop3Box.Create(box.Name, box.Server,
										box.Port, box.Login, box.Password, box.IsActive, 
										box.Interval, box.LastRequest, box.LastSuccessfulRequest,
										box.LastErrorText, box.AutoKillForRead))
							{
								if (reader.Read())
								{
									BoxId = (int)reader["Pop3BoxId"];
								}
							}
						}
						else
						{
							DBPop3Box.Update(BoxId, box.Name, box.Server,
								box.Port, box.Login, box.Password, box.IsActive, 
								box.Interval, box.LastRequest, box.LastSuccessfulRequest,
								box.LastErrorText, box.AutoKillForRead);
						}

						box.ResetModified();
					}

					if (box.Handlers.IsModified)
					{
						DBPop3Box.DeleteHandlers(BoxId);

						foreach(Pop3MessageHandlerInfo info in box.Handlers)
						{
							DBPop3Box.CreateHandler(BoxId, info.Name);
						}
						box.Handlers.ResetModified();
					}
					if (box.Parameters.IsModified)
					{
						DBPop3Box.DeleteParameters(BoxId);

						foreach(string name in box.Parameters.Keys)
						{
							DBPop3Box.AddParameter(BoxId, name, box.Parameters[name]);
						}
					}

					tra.Commit();
				}
			}
			return box;
		}

		/// <summary>
		/// Singles the run.
		/// </summary>
		public static void SingleRun()
		{
			_current.Request(_current.GetPop3BoxList());
		}

		/// <summary>
		/// Runs this instance.
		/// </summary>
		public static void Run()
		{
			lock (_current)
			{
				if (_current._thread != null)
					return;
			}
			_current._thread = new Thread(new ThreadStart(ThreadProc));
			_current._thread.Start();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public static void Stop()
		{
			lock (_current)
			{
				if (_current._thread != null)
				{
					_current._stopEvent.Set();
				}
			}
		}

		/// <summary>
		/// Requests the specified boxes.
		/// </summary>
		/// <param name="boxes">The boxes.</param>
		private void Request(Pop3Box[] boxes)
		{
			DateTime current = DateTime.Now;
			foreach(Pop3Box box in boxes)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("Request mails from: {0}. IsActive = {1}. LastRequest = {2}. Interval = {3}", box.Name, box.IsActive, box.LastRequest, box.Interval));

				// OZ [2009-05-05]: Skip Read Maindler if handler not set
				if (box.Handlers.Count >0 && 
					box.IsActive && 
					current >= box.LastRequest.AddMinutes(box.Interval))
				{
					box.Request();
				}
			}
		}

		/// <summary>
		/// Threads the proc.
		/// </summary>
		private static void ThreadProc()
		{
			try
			{
				Pop3Box[] boxes = Pop3Manager.Current.GetPop3BoxList();
				while (true)
				{
					Pop3Manager.Current.Request(boxes);
						
					if (_current._stopEvent.WaitOne(30000, false))
						break;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
			}

			lock (_current)
			{
				Pop3Manager.Current._thread = null;
				_current._stopEvent.Reset();
			}
		}
	}
}
