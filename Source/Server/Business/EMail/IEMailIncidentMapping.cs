using System;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for IEMailIncidentMapping.
	/// </summary>
	public interface IEMailIncidentMapping
	{
		IncidentInfo	Create(EMailRouterPop3Box box, Pop3Message msg);
	}
}
