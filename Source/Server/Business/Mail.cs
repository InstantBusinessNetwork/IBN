using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using Mediachase.IBN.Database;
using Mediachase.Net.Mail;


namespace Mediachase.IBN.Business
{
	public enum MailboxType
	{
		Issue,
		Folder
	}
}
