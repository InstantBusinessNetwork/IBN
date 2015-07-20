using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Threading;

using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	#region enum SpecialMessageType
	public enum SpecialMessageType
	{
		BatchAlert = 1,
		BatchItem,
		BatchItemDeleted,
		BroadcastMessage,
		FileDelivery,
		ForgottenPassword,
		Issue_ResponseSignature
	}
	#endregion

	/// <summary>
	/// Summary description for SpecialMessage.
	/// </summary>
	internal class SpecialMessage
	{
		private static ResourceManager rmTemplates = new ResourceManager(AlertTemplate.sSpecialTemplates, typeof(SpecialMessage).Assembly);
		private static ResourceManager rmTypes = new ResourceManager("Mediachase.IBN.Business.Resources.SpecialMessageTypes", typeof(SpecialMessage).Assembly);

		internal static ResourceManager TemplatesRM{ get{ return rmTemplates; } }
		internal static ResourceManager TypesRM{ get{ return rmTypes; } }

		private SpecialMessage()
		{
		}

		#region GetVariables()
		internal static AlertVariable[] GetVariables(SpecialMessageType type)
		{
			ArrayList vars = new ArrayList();

			switch(type)
			{
				case SpecialMessageType.BatchAlert:
					vars.Add(new AlertVariable(Variable.End));
					vars.Add(new AlertVariable(Variable.Start));
					vars.Add(new AlertVariable(Variable.Text));
					break;
				case SpecialMessageType.BatchItem:
					vars.Add(new AlertVariable(Variable.EventDate));
					vars.Add(new AlertVariable(Variable.EventName));
					vars.Add(new AlertVariable(Variable.InitiatedBy));
					vars.Add(new AlertVariable(Variable.Link, true, true));
					vars.Add(new AlertVariable(Variable.Link, true, false, true)); // RelLink
					vars.Add(new AlertVariable(Variable.Title));
					vars.Add(new AlertVariable(Variable.Title, false, true, true)); // RelTitle
					break;
				case SpecialMessageType.BatchItemDeleted:
					vars.Add(new AlertVariable(Variable.EventDate));
					vars.Add(new AlertVariable(Variable.EventName));
					vars.Add(new AlertVariable(Variable.InitiatedBy));
					vars.Add(new AlertVariable(Variable.Title));
					break;
				case SpecialMessageType.BroadcastMessage:
					vars.Add(new AlertVariable(Variable.InitiatedBy));
					vars.Add(new AlertVariable(Variable.Text));
					break;
				case SpecialMessageType.FileDelivery:
					vars.Add(new AlertVariable(Variable.InitiatedBy));
					break;
				case SpecialMessageType.ForgottenPassword:
					vars.Add(new AlertVariable(Variable.LogonLink, true, false));
					break;
				case SpecialMessageType.Issue_ResponseSignature:
					vars.Add(new AlertVariable(Variable.Company));
					vars.Add(new AlertVariable(Variable.Department));
					vars.Add(new AlertVariable(Variable.Email));
					vars.Add(new AlertVariable(Variable.Fax));
					vars.Add(new AlertVariable(Variable.FirstName));
					vars.Add(new AlertVariable(Variable.LastName));
					vars.Add(new AlertVariable(Variable.Link, true, false));
					vars.Add(new AlertVariable(Variable.Location));
					vars.Add(new AlertVariable(Variable.Mobile));
					vars.Add(new AlertVariable(Variable.Phone));
					vars.Add(new AlertVariable(Variable.Position));
					vars.Add(new AlertVariable(Variable.Text));
					vars.Add(new AlertVariable(Variable.Ticket));
					vars.Add(new AlertVariable(Variable.Title));
					break;
			}

			return vars.ToArray(typeof(AlertVariable)) as AlertVariable[];
		}
		#endregion
	}
}
