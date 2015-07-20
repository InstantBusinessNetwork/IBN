using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Alert.Service;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents Inner Alert Service.
	/// </summary>
	internal sealed class InnerAlertService : IAlertService5
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="InnerAlertService"/> class.
		/// </summary>
		public InnerAlertService()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		#endregion

		#region IAlertService5 Members

		void IAlertService5.BeginSend(IAlertMessage message)
		{
			message.BeginSend();
		}

		IEmailAlertMessage6 IAlertService5.CreateEmailAlert()
		{
			return new InnerEmailAlertMessage();
		}

		IIbnAlertMessage IAlertService5.CreateIbnAlert()
		{
			return new InnerIbnAlertMessage();
		}

		#endregion
	}
}
