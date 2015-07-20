using System;
using System.Collections;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailIssueExternalRecipient.
	/// </summary>
	public class EMailIssueExternalRecipient
	{
		private EMailIssueExternalRecipientRow _srcRow = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="EMailIssueExternalRecipient"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		private EMailIssueExternalRecipient(EMailIssueExternalRecipientRow row)
		{
			_srcRow = row;
		}

		public static int Create(int IssueId, string EMail)
		{
			EMailIssueExternalRecipientRow newRow = new EMailIssueExternalRecipientRow();

			newRow.IssueId = IssueId;
			newRow.EMail = EMail;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		/// <summary>
		/// Loads the specified E mail issue external recipient id.
		/// </summary>
		/// <param name="EMailIssueExternalRecipientId">The E mail issue external recipient id.</param>
		/// <returns></returns>
		public static EMailIssueExternalRecipient Load(int EMailIssueExternalRecipientId)
		{
			return new EMailIssueExternalRecipient(new EMailIssueExternalRecipientRow(EMailIssueExternalRecipientId));
		}


		/// <summary>
		/// Determines whether [has E mail recipient] [the specified issue id].
		/// </summary>
		/// <param name="IssueId">The issue id.</param>
		/// <returns>
		/// 	<c>true</c> if [has E mail recipient] [the specified issue id]; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasEMailRecipient(int IssueId)
		{
			return List(IssueId).Length>0;
		}

		/// <summary>
		/// Determines whether [contains] [the specified issue id].
		/// </summary>
		/// <param name="IssueId">The issue id.</param>
		/// <param name="EMail">The E mail.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified issue id]; otherwise, <c>false</c>.
		/// </returns>
		public static bool Contains(int IssueId, string EMail)
		{
			foreach(EMailIssueExternalRecipient item in EMailIssueExternalRecipient.List(IssueId))
			{
				if(string.Compare(EMail, item.EMail, true)==0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Lists the specified issue id.
		/// </summary>
		/// <param name="IssueId">The issue id.</param>
		/// <returns></returns>
		public static EMailIssueExternalRecipient[]	List(int IssueId)
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailIssueExternalRecipientRow row in EMailIssueExternalRecipientRow.List(IssueId))
			{
				retVal.Add(new EMailIssueExternalRecipient(row));
			}

			return (EMailIssueExternalRecipient[])retVal.ToArray(typeof(EMailIssueExternalRecipient));
		}

		/// <summary>
		/// Strings the list.
		/// </summary>
		/// <param name="IssueId">The issue id.</param>
		/// <returns></returns>
		public static string[] StringList(int IssueId)
		{
			ArrayList retVal = new ArrayList();

			foreach (EMailIssueExternalRecipientRow row in EMailIssueExternalRecipientRow.List(IssueId))
			{
				retVal.Add(row.EMail);
			}

			return (string[])retVal.ToArray(typeof(string));
		}


		/// <summary>
		/// Updates the specified recipient.
		/// </summary>
		/// <param name="recipient">The recipient.</param>
		public static void Update(int IssueId, Hashtable recipientHash)
		{
			if(!Incident.CanUpdateExternalRecipients(IssueId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				EMailIssueExternalRecipientRow[] list = EMailIssueExternalRecipientRow.List(IssueId);

				Hashtable alreadyAddedItems = new Hashtable();

				foreach(EMailIssueExternalRecipientRow row in list)
				{
					if(!alreadyAddedItems.ContainsKey(row.EMail.ToLower()))
						alreadyAddedItems.Add(row.EMail.ToLower(),null);
				}

				// Delete 
				foreach(EMailIssueExternalRecipientRow row in list)
				{
					if(!recipientHash.ContainsKey(row.EMailIssueExternalRecipientId))
					{
						alreadyAddedItems.Remove(row.EMail.ToLower());
						row.Delete();
					}
				}

				// Insert
				foreach(int Key in recipientHash.Keys)
				{
					string Value = (string)recipientHash[Key];

					if(Key<0 && !alreadyAddedItems.ContainsKey(Value.ToLower()))
					{
						EMailIssueExternalRecipientRow newRow = new EMailIssueExternalRecipientRow();
						newRow.IssueId = IssueId;
						newRow.EMail = Value;

						newRow.Update();

						alreadyAddedItems.Add(Value.ToLower(),null);
					}
				}

				tran.Commit();
			}
		}

		#region Public Properties
		
		/// <summary>
		/// Gets the E mail issue external recipient id.
		/// </summary>
		/// <value>The E mail issue external recipient id.</value>
		public virtual int EMailIssueExternalRecipientId
		{
			get
			{
				return _srcRow.EMailIssueExternalRecipientId;
			}
			
		}
		
		/// <summary>
		/// Gets the issue id.
		/// </summary>
		/// <value>The issue id.</value>
		public virtual int IssueId
		{
			get
			{
				return _srcRow.IssueId;
			}
		}
		
		/// <summary>
		/// Gets or sets the E mail.
		/// </summary>
		/// <value>The E mail.</value>
		public virtual string EMail
		{
			get
			{
				return _srcRow.EMail;
			}
			
			set
			{
				_srcRow.EMail = value;
			}	
		}
		
		#endregion
	}
}
