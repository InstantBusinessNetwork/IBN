using System;
using System.Collections;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for IncidentInfo.
	/// </summary>
	public class IncidentInfo
	{
		public IncidentInfo()
		{
		}

		public int EMailBox = -1;

		public int CreatorId = -1;

		public string MailSenderEmail = string.Empty;
		public string Title = string.Empty;
		public string Description = string.Empty;

		public string EMailBody = string.Empty;

		public int TypeId = -1;
		public int PriorityId = -1;
		public int SeverityId = -1;

		public ArrayList GeneralCategories = new ArrayList();
		public ArrayList IncidentCategories = new ArrayList();

		public int ProjectId = -1;
		public int IncidentBoxId = -1;

		public PrimaryKeyId OrgUid = PrimaryKeyId.Empty;
		public PrimaryKeyId ContactUid = PrimaryKeyId.Empty;

		/// <summary>
		/// Gets the title or description or E mail body.
		/// </summary>
		/// <value>The title or description or E mail body.</value>
		public string TitleOrDescriptionOrEMailBody
		{
			get
			{
				return Title + "\r\n" +
					Description + "\r\n" +
					EMailBody;
			}
		}

		// TODO: Meta Data Fields
	}
}
