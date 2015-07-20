using System;
using Mediachase.Net.Mail;

namespace Mediachase.IBN.Business.Pop3
{
	/// <summary>
	/// Summary description for IPop3MessageHandler.
	/// </summary>
	public interface IPop3MessageHandler
	{
		void Init(Pop3Manager manager);
		void ProcessPop3Message(Pop3Box box, Pop3Message message);

		string Name{get;}
		string Description{get;}
		//System.Web.UI.Control PropertyPage{get;}
	}
}
