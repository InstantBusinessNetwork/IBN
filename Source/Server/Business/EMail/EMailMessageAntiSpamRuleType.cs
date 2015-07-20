using System;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailMessageAntiSpamRuleType.
	/// </summary>
	public enum EMailMessageAntiSpamRuleType
	{
		Contains = 0,
		RegexMatch = 1,
		IsEqual = 2,
		Service = 3, // BlackList, WhiteList, Ticket
	}
}
