using System;
using System.Collections;
using Mediachase.IBN.Business;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for DefaultEMailIncidentMapping.
	/// </summary>
	public class DefaultEMailIncidentMapping: IEMailIncidentMapping
	{
		public DefaultEMailIncidentMapping()
		{
		}

		#region IEMailIncidentMapping Members

		public IncidentInfo Create(EMailRouterPop3Box box, Mediachase.Net.Mail.Pop3Message msg)
		{
			IncidentInfo retVal = new IncidentInfo();

			retVal.CreatorId = box.Settings.DefaultEMailIncidentMappingBlock.DefaultCreator;

			retVal.EMailBox = box.EMailRouterPop3BoxId;

			retVal.Title = msg.Subject==null?string.Empty:msg.Subject; 

			if(retVal.Title==string.Empty)
			{
				// Set Default Inicdent Title if subject is empty
				string SenderID = EMailMessage.GetSenderName(msg);
				if(SenderID==string.Empty)
					SenderID = EMailMessage.GetSenderEmail(msg);
				retVal.Title = string.Format("{0} ({1})", SenderID, DateTime.Now.ToString("d"));
			}

			// OZ: 2008-07-30 Added description defenition rules
			if (box.Settings.DefaultEMailIncidentMappingBlock.DescriptionId == -1)
			{
				retVal.Description = string.Empty;
				retVal.EMailBody = EMailMessageInfo.ExtractTextFromHtml(msg);

			}
			else
			{
				retVal.Description = EMailMessageInfo.ExtractTextFromHtml(msg);
				retVal.EMailBody = retVal.Description;
			}
			//

			int priorityId = GetPriorityId(box, msg);

			if (priorityId < 0) // <0 From Email
			{
				retVal.PriorityId = 500;

				string importance = msg.Importance;
				if (importance != null)
				{
					switch (importance.ToLower())
					{
						case "low":
							retVal.PriorityId = 0;
							break;
						case "normal":
							retVal.PriorityId = 500;
							break;
						case "high":
							retVal.PriorityId = 750;
							break;
					}
				}
			}
			else
			{
				retVal.PriorityId = priorityId;
			}


			retVal.MailSenderEmail = EMailMessage.GetSenderEmail(msg);

			retVal.SeverityId = GetSeverityId(box, msg);
			retVal.TypeId = GetTypeId(box, msg);
			retVal.GeneralCategories.AddRange(GetGeneralCategories(box, msg));
			retVal.IncidentCategories.AddRange(GetIncidentCategories(box, msg));

			// OZ: 2007-01-11
			retVal.ProjectId = box.Settings.DefaultEMailIncidentMappingBlock.ProjectId;
			retVal.IncidentBoxId = box.Settings.DefaultEMailIncidentMappingBlock.IncidentBoxId;

			// OZ: 2007-01-22
			retVal.OrgUid = box.Settings.DefaultEMailIncidentMappingBlock.OrgUid;
			retVal.ContactUid = box.Settings.DefaultEMailIncidentMappingBlock.ContactUid;

			return retVal;
		}

		#endregion

		protected virtual int GetSeverityId(EMailRouterPop3Box box, Mediachase.Net.Mail.Pop3Message msg)
		{
			return box.Settings.DefaultEMailIncidentMappingBlock.SeverityId;
		}

		protected virtual int GetPriorityId(EMailRouterPop3Box box, Mediachase.Net.Mail.Pop3Message msg)
		{
			return box.Settings.DefaultEMailIncidentMappingBlock.PriorityId;
		}

		protected virtual int GetTypeId(EMailRouterPop3Box box, Mediachase.Net.Mail.Pop3Message msg)
		{
			return box.Settings.DefaultEMailIncidentMappingBlock.TypeId;
		}

		protected virtual int[] GetGeneralCategories(EMailRouterPop3Box box, Mediachase.Net.Mail.Pop3Message msg)
		{
			return (int[])box.Settings.DefaultEMailIncidentMappingBlock.GeneralCategories.ToArray(typeof(int));
		}

		protected virtual int[] GetIncidentCategories(EMailRouterPop3Box box, Mediachase.Net.Mail.Pop3Message msg)
		{
			return (int[])box.Settings.DefaultEMailIncidentMappingBlock.IncidentCategories.ToArray(typeof(int));
		}
	}
}
