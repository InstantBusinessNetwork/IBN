using System;
using System.Net;

namespace Mediachase.Alert.Service
{
	public interface IAlertService5
	{
		void BeginSend(IAlertMessage message);
		IIbnAlertMessage CreateIbnAlert();
		IEmailAlertMessage6 CreateEmailAlert();
	}

	public interface IAlertMessage
	{
		string Recipient { get;set;}
		string Body { get;set;}
		void BeginSend();
		void Send();
	}

	public interface IIbnAlertMessage : IAlertMessage
	{
		string Sender { get;set;}
	}

	public interface IEmailAlertMessage6 : IAlertMessage
	{
		string Sender { get;set;}
		string Subject { get;set;}

		void AddFile(string fileName);
		void RemoveFile(string fileName);
		string[] GetFiles();

		IEmailAlertAttachment CreateAttachment(string name);

		string CC { get;set;}
		string Bcc { get;set;}
		string SmtpServer { get;set;}
		long SmtpPort { get;set;}
		string SmtpSecureConnection { get;set;}
		bool SmtpAuthenticate { get;set;}
		string SmtpUser { get;set;}
		string SmtpPassword { get;set;}

		bool IsBodyHtml { get;set;}
	}

	public interface IEmailAlertAttachment
	{
		void Write(byte[] data, int count);
		void Complete();
		bool IsComplete { get;}
	}

	public enum SecureConnectionType
	{
		None,
		Ssl3,
		Tls
	}
}
