using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Alert.Service;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.Ibn.Business.Messages
{
	/// <summary>
	/// Represents InnerEmailAlertMessage.
	/// </summary>
	internal sealed class InnerEmailAlertMessage : IEmailAlertMessage6
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="InnerEmailAlertMessage"/> class.
		/// </summary>
		public InnerEmailAlertMessage()
		{
			this.MessageEntity = BusinessManager.InitializeEntity<EmailEntity>(EmailEntity.ClassName);
			//this.Files = new List<InnerEmailAlertAttachment>();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the message entity.
		/// </summary>
		/// <value>The message entity.</value>
		private EmailEntity MessageEntity { get; set; }

		/// <summary>
		/// Gets or sets the files.
		/// </summary>
		/// <value>The files.</value>
		//protected List<InnerEmailAlertAttachment> Files { get; private set; }
		#endregion

		#region Methods
		#endregion

		#region IEmailAlertMessage6 Members

		/// <summary>
		/// Adds the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		void IEmailAlertMessage6.AddFile(string fileName)
		{
			throw new NotSupportedException();
			//InnerEmailAlertAttachment file = new InnerEmailAlertAttachment(fileName);

			//this.Files.Add(file);
		}

		/// <summary>
		/// Gets or sets the BCC.
		/// </summary>
		/// <value>The BCC.</value>
		string IEmailAlertMessage6.Bcc
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets the CC.
		/// </summary>
		/// <value>The CC.</value>
		string IEmailAlertMessage6.CC
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Creates the attachment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		IEmailAlertAttachment IEmailAlertMessage6.CreateAttachment(string name)
		{
			throw new NotSupportedException();
			//InnerEmailAlertAttachment file = new InnerEmailAlertAttachment(name);

			//this.Files.Add(file);

			//return file;
		}

		string[] IEmailAlertMessage6.GetFiles()
		{
			throw new NotSupportedException();
			//List<string> fileNames = new List<string>();

			//foreach(InnerEmailAlertAttachment item in this.Files)
			//{
			//    fileNames.Add(item.FileName);
			//}

			//return fileNames.ToArray();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is body HTML.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is body HTML; otherwise, <c>false</c>.
		/// </value>
		bool IEmailAlertMessage6.IsBodyHtml
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Removes the file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		void IEmailAlertMessage6.RemoveFile(string fileName)
		{
			throw new NotSupportedException();
			//for (int index = this.Files.Count - 1; index >= 0; index--)
			//{
			//    InnerEmailAlertAttachment file = this.Files[index];

			//    if (file.FileName == fileName)
			//    {
			//        this.Files.RemoveAt(index);
			//    }
			//}
		}

		/// <summary>
		/// Gets or sets the sender.
		/// </summary>
		/// <value>The sender.</value>
		string IEmailAlertMessage6.Sender
		{
			get
			{
				return this.MessageEntity.From;
			}
			set
			{
				this.MessageEntity.From = value;
			}
		}

		#region SmtpAuthenticate
		bool IEmailAlertMessage6.SmtpAuthenticate
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		string IEmailAlertMessage6.SmtpPassword
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		long IEmailAlertMessage6.SmtpPort
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		string IEmailAlertMessage6.SmtpSecureConnection
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		string IEmailAlertMessage6.SmtpServer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		string IEmailAlertMessage6.SmtpUser
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		} 
		#endregion

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>The subject.</value>
		string IEmailAlertMessage6.Subject
		{
			get
			{
				return this.MessageEntity.Subject;
			}
			set
			{
				this.MessageEntity.Subject = value;
			}
		}

		#endregion

		#region IAlertMessage Members

		/// <summary>
		/// Begins the send.
		/// </summary>
		void IAlertMessage.BeginSend()
		{
			// Check File
			//if (this.Files.Count > 0)
			//{
			//    // Create Single EML
			//    this.MessageEntity.MessageContext = CreateMessageContent();
			//}

			// Create Message
			CreateRequest request = new CreateRequest(this.MessageEntity);
			request.Parameters.Add(OutgoingMessageQueuePlugin.AddToQueue, true);
			request.Parameters.Add(OutgoingMessageQueuePlugin.SourceName, OutgoingEmailServiceType.AlertService.ToString());

			BusinessManager.Execute(request);
		}



		/// <summary>
		/// Creates the content of the message.
		/// </summary>
		/// <returns></returns>
		//private string CreateMessageContent()
		//{
		//    MailMessage message = new MailMessage();

		//    message.From = CreateMailAddress(this.MessageEntity.From);
		//    message.To.Add(CreateMailAddress(this.MessageEntity.To));
		//    message.Subject = this.MessageEntity.Subject;
		//    message.IsBodyHtml = true;
		//    message.Body = this.MessageEntity.HtmlBody;
			
		//    foreach(InnerEmailAlertAttachment item in this.Files)
		//    {
		//        string contentType = "";
		//        if (File.Exists(item.FileName))
		//        {
		//            message.Attachments.Add(new Attachment(contentType, item.FileName));
		//        }
		//        else
		//        {
		//            item.InnerStream.Position = 0;
		//            message.Attachments.Add(new Attachment(contentType, Path.GetFileName(item.FileName), item.InnerStream));
		//        }
		//    }


		//    return message.MessageContent;
		//}

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
				return this.MessageEntity.To;
			}
			set
			{
				this.MessageEntity.To = value;
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
