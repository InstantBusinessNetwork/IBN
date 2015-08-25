using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Mediachase.Sync.Core;

namespace Mediachase.ClientOutlook
{
	internal class ThreadedWorkManager<T> : IDisposable
	{
		EventWaitHandle _wh = new AutoResetEvent(false);
		Thread _worker;
		object _locker = new object();
		Queue<T> _tasks = new Queue<T>();
		bool _finalize = false;

		public delegate void VoidFunc(T param);
		private VoidFunc _workProc = null;

		public ThreadedWorkManager(VoidFunc workProc)
		{
			_workProc = workProc;

			_worker = new Thread(WorkerProcess);
			_worker.Start();
		}
		
		public void SheduleTask(T task)
		{
			lock (_locker)
			{
				_tasks.Enqueue(task);
			}

			_wh.Set();
		}

		private bool NeedTerminate
		{
			get
			{
				return _finalize;
			}
			set
			{
				lock (_locker)
				{
					_finalize = value;
				}
				_wh.Set();
			}
		}

		private void WorkerProcess()
		{
			while (true)
			{
				DebugAssistant.Log("ThreadedWorkManager: Worker process begin main loop");

				if (NeedTerminate)//Завершаем поток если установлен флаг завершения
					break;

				if (_tasks.Count > 0)
				{
					T task = _tasks.Dequeue(); 
					DebugAssistant.Log("ThreadedWorkManager: Worker process processed job task " + task);
					if (_workProc != null)
					{
						_workProc(task);
					}
				}
				else
				{
					DebugAssistant.Log("ThreadedWorkManager: Worker process sleeping....");
					_wh.WaitOne();       // Больше задач нет, ждем сигнала...
				}
			}

			DebugAssistant.Log("ThreadedWorkManager: Worker process finished");

		}

		#region IDisposable Members

		public void Dispose()
		{
			NeedTerminate = true;
		}

		#endregion
	}
}
