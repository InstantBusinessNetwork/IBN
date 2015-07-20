using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents Delivery Provider Config.
	/// </summary>
	[Serializable]
	public class DeliveryProviderConfig
	{
		#region Const
		#endregion

		#region Fields
		private int _maxDeliveryAttempts = 100;
		private bool _enable = true;
		private int _messageExpiration = 7 * 24 * 60;
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="OutgoingMessageQueueConfig"/> class.
		/// </summary>
		public DeliveryProviderConfig()
		{
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DeliveryProviderConfig"/> is enable.
		/// </summary>
		/// <value><c>true</c> if enable; otherwise, <c>false</c>.</value>
		public bool Enable
		{
			get { return _enable; }
			set { _enable = value; }
		}

		/// <summary>
		/// Gets or sets the max delivery attempts.
		/// </summary>
		/// <value>The max delivery attempts.</value>
		public int MaxDeliveryAttempts
		{
			get { return _maxDeliveryAttempts; }
			set { _maxDeliveryAttempts = value; }
		}

		/// <summary>
		/// Gets or sets the message expiration (in minutes).
		/// </summary>
		/// <value>The message expiration.</value>
		/// <remarks>
		/// The default value is 7 days (10080 minutes).
		/// </remarks>
		public int MessageExpiration
		{
			get { return _messageExpiration; }
			set { _messageExpiration = value; }
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
