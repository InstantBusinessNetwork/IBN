using System;
using System.Threading;

namespace Mediachase.Schedule.Service
{
	/// <summary>
	/// Represents a smpp command async result.
	/// </summary>
	public class ScheduleServiceAsyncResult: IAsyncResult
	{
		private AsyncCallback		_AsyncCallback;
		private object				_AsyncObject;
		private object				_AsyncState;
		private int					_CleanedUp;
		private bool				_CompletedSynchronously;
		private bool				_EndCalled;
		private Exception			_ErrorException;
		private object				_Event;
		private int					_IntCompleted;
		private int					_IntInvokedCallback;
		private object				_Result;
		private DateTime			_WaitResponse	=	DateTime.MinValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="SmppAsyncResult"/> class.
		/// </summary>
		/// <param name="requestCommand">The request command.</param>
		/// <param name="myCallBack">My call back.</param>
		/// <param name="myState">My state.</param>
		internal ScheduleServiceAsyncResult(object AsyncObject, AsyncCallback myCallBack, object myState)
		{
			this._AsyncObject			= AsyncObject;

			this._AsyncState			= myState;
			this._AsyncCallback			= myCallBack;

			this._IntInvokedCallback	= (myCallBack != null) ? 0 : 1;
		}

		/// <summary>
		/// Completes the specified completed synchronously.
		/// </summary>
		/// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
		private void Complete(bool completedSynchronously)
		{
			this._CompletedSynchronously = completedSynchronously;
			Interlocked.Increment(ref this._IntCompleted);
			if (this._Event != null)
			{
				((ManualResetEvent) this._Event).Set();
			}
			this.InternalCleanup();
		}

		/// <summary>
		/// Completes the specified completed synchronously.
		/// </summary>
		/// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
		/// <param name="result">The result.</param>
		private void Complete(bool completedSynchronously, object result)
		{
			this._Result = result;
			this.Complete(completedSynchronously);
		}

		/// <summary>
		/// Internals the wait for completion.
		/// </summary>
		/// <returns></returns>
		internal object InternalWaitForCompletion()
		{
			if (this._IntCompleted == 0)
			{
				ManualResetEvent event1 = (ManualResetEvent) this.AsyncWaitHandle;
				event1.WaitOne();
			}

			if(this.ErrorException!=null)
				throw this.ErrorException;

			return this._Result;
		}

		/// <summary>
		/// Invokes the callback.
		/// </summary>
		private void InvokeCallback()
		{
			if (Interlocked.Increment(ref this._IntInvokedCallback) == 1)
			{
				this._AsyncCallback(this);
			}
		}

		/// <summary>
		/// Invokes the callback.
		/// </summary>
		/// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
		internal void InvokeCallback(bool completedSynchronously)
		{
			this.Complete(completedSynchronously);
			this.InvokeCallback();
		}

		/// <summary>
		/// Invokes the callback.
		/// </summary>
		/// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
		/// <param name="result">The result.</param>
		internal void InvokeCallback(bool completedSynchronously, object result)
		{
			this.Complete(completedSynchronously, result);
			this.InvokeCallback();
		}


		/// <summary>
		/// Gets the async object.
		/// </summary>
		/// <value>The async object.</value>
		public object AsyncObject
		{
			get
			{
				return this._AsyncObject;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [end called].
		/// </summary>
		/// <value><c>true</c> if [end called]; otherwise, <c>false</c>.</value>
		internal bool EndCalled
		{
			get
			{
				return this._EndCalled;
			}
			set
			{
				this._EndCalled = value;
			}
		}

		/// <summary>
		/// Gets or sets the wait response.
		/// </summary>
		/// <value>The wait response.</value>
		internal DateTime WaitResponse
		{
			get
			{
				return this._WaitResponse;
			}
			set
			{
				this._WaitResponse = value;
			}
		}

		/// <summary>
		/// Gets or sets the error exception.
		/// </summary>
		/// <value>The error exception.</value>
		internal Exception ErrorException
		{
			get
			{
				return this._ErrorException;
			}
			set
			{
				this._ErrorException = value;
			}
		}

		/// <summary>
		/// Gets or sets the result.
		/// </summary>
		/// <value>The result.</value>
		public object Result
		{
			get
			{
				return this._Result;
			}
			set
			{
				this._Result	=	value;
			}
		}

		/// <summary>
		/// Internals the cleanup.
		/// </summary>
		private void InternalCleanup()
		{
			if (Interlocked.Increment(ref this._CleanedUp) == 1)
			{
				this.Cleanup();
			}
		}

		/// <summary>
		/// Cleanups this instance.
		/// </summary>
		internal virtual void Cleanup()
		{
		}

		#region IAsyncResult Members

		/// <summary>
		/// Gets a user-defined object that qualifies or contains information about
		/// an asynchronous operation.
		/// </summary>
		/// <value></value>
		public object AsyncState
		{
			get
			{
				return this._AsyncState;
			}
		}

		/// <summary>
		/// Gets an indication of whether the asynchronous operation completed synchronously.
		/// </summary>
		/// <value></value>
		public bool CompletedSynchronously
		{
			get
			{
				return this._CompletedSynchronously;
			}
		}

		/// <summary>
		/// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation
		/// to complete.
		/// </summary>
		/// <value></value>
		public System.Threading.WaitHandle AsyncWaitHandle
		{
			get
			{
				int num1 = this._IntCompleted;

				if (this._Event == null)
				{
					Interlocked.CompareExchange(ref this._Event, new ManualResetEvent(num1 != 0), (object) null);
				}

				ManualResetEvent event1 = (ManualResetEvent) this._Event;

				if ((num1 == 0) && (this._IntCompleted != 0))
				{
					event1.Set();
				}

				return event1;
			}
		}

		/// <summary>
		/// Gets an indication
		/// whether the asynchronous operation has completed.
		/// </summary>
		/// <value></value>
		public bool IsCompleted
		{
			get
			{
				return (this._IntCompleted != 0);
			}
		}

		#endregion
	}
}
