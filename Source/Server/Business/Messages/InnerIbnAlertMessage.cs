using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Alert.Service;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents InnerIbnAlertMessage.
	/// </summary>
	internal class InnerIbnAlertMessage : IIbnAlertMessage
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="InnerIbnAlertMessage"/> class.
		/// </summary>
		public InnerIbnAlertMessage()
		{
			this.MessageEntity = BusinessManager.InitializeEntity<IbnClientMessageEntity>(IbnClientMessageEntity.ClassName);
		}
		#endregion

		#region Properties
		protected IbnClientMessageEntity MessageEntity { get; set; }
		#endregion

		#region Methods
		#endregion

		#region IIbnAlertMessage Members

		/// <summary>
		/// Gets or sets the sender.
		/// </summary>
		/// <value>The sender.</value>
		string IIbnAlertMessage.Sender
		{
			get
			{
				return this.MessageEntity.FromId.ToString();
			}
			set
			{
				this.MessageEntity.FromId = int.Parse(value);
			}
		}

		#endregion

		#region IAlertMessage Members

		/// <summary>
		/// Begins the send.
		/// </summary>
		void IAlertMessage.BeginSend()
		{
			this.MessageEntity.FromId = ConvertOriginalIdToPrincipalId(this.MessageEntity.FromId);
			this.MessageEntity.ToId = ConvertOriginalIdToPrincipalId(this.MessageEntity.ToId);

			CreateRequest request = new CreateRequest(this.MessageEntity);
			request.Parameters.Add(OutgoingMessageQueuePlugin.AddToQueue, true);
			request.Parameters.Add(OutgoingMessageQueuePlugin.SourceName, OutgoingEmailServiceType.AlertService.ToString());

			BusinessManager.Execute(request);
		}

		/// <summary>
		/// Converts the original id to principal id.
		/// </summary>
		/// <param name="originalId">The original id.</param>
		/// <returns></returns>
		private PrimaryKeyId ConvertOriginalIdToPrincipalId(PrimaryKeyId originalId)
		{
			return DBUser.GetPrincipalId((int)originalId);
		}

		/// <summary>
		/// Converts the principal id to original id.
		/// </summary>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		private PrimaryKeyId ConvertPrincipalIdToOriginalId(PrimaryKeyId principalId)
		{
			return DBUser.GetOriginalId((int)principalId);
		}

		/// <summary>
		/// Gets or sets the body.
		/// </summary>
		/// <value>The body.</value>
		string IAlertMessage.Body
		{
			get
			{
				return this.MessageEntity.HtmlBody;
			}
			set
			{
				this.MessageEntity.HtmlBody = value;
			}
		}

		/// <summary>
		/// Gets or sets the recipient.
		/// </summary>
		/// <value>The recipient.</value>
		string IAlertMessage.Recipient
		{
			get
			{
				return this.MessageEntity.ToId.ToString();
			}
			set
			{
				this.MessageEntity.ToId = int.Parse(value);
			}
		}

		/// <summary>
		/// Sends this instance.
		/// </summary>
		void IAlertMessage.Send()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
