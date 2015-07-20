using System;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for EMailMessageLog.
	/// </summary>
	public class EMailMessageLog
	{
		private EMailMessageLogRow _srcRow = null;

		private EMailMessageLog(EMailMessageLogRow row)
		{
			_srcRow = row;
		}

		/// <summary>
		/// Adds the specified from.
		/// </summary>
		/// <param name="From">From.</param>
		/// <param name="To">To.</param>
		/// <param name="Subject">The subject.</param>
		/// <param name="EMailBoxId">The E mail box id.</param>
		/// <param name="Result">The result.</param>
		internal static void Add(string From, string To, string Subject, 
			int EMailBoxId, EMailMessageAntiSpamRuleRusult Result)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				EMailMessageLogRow row = new EMailMessageLogRow();

				row.Incoming = true;
				row.Created = DateTime.UtcNow;

				row.From = From;
				row.To = To;
				row.Subject = Subject;
				row.AntiSpamResult = (int)Result;
				row.EMailBoxId = EMailBoxId;

				row.Update();
				tran.Commit();
			}
		}

		/// <summary>
		/// Adds the specified from.
		/// </summary>
		/// <param name="From">From.</param>
		/// <param name="To">To.</param>
		/// <param name="Subject">The subject.</param>
		internal static void Add(string From, string To, string Subject)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				EMailMessageLogRow row = new EMailMessageLogRow();

				row.Incoming = false;
				row.Created = DateTime.UtcNow;

				row.From = From;
				row.To = To;
				row.Subject = Subject;
				row.AntiSpamResult = (int)EMailMessageAntiSpamRuleRusult.Accept;
				row.EMailBoxId = -1;

				row.Update();
				tran.Commit();
			}

		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static EMailMessageLog[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailMessageLogRow row in EMailMessageLogRow.List())
			{
				retVal.Add(new EMailMessageLog(row));
			}

			return (EMailMessageLog[])retVal.ToArray(typeof(EMailMessageLog));
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public static void Clear()
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				EMailMessageLogRow.Clear();

				tran.Commit();
			}
		}

		/// <summary>
		/// Cleans the up.
		/// </summary>
		/// <param name="Period">The period.</param>
		public static void CleanUp(int Period)
		{
			EMailMessageLogRow.CleanUp(Period);
		}

		#region Public Properties
		
		/// <summary>
		/// Gets the E mail message log id.
		/// </summary>
		/// <value>The E mail message log id.</value>
		public virtual int EMailMessageLogId
	    
		{
			get
			{
				return _srcRow.EMailMessageLogId;
			}
			
		}
		
		/// <summary>
		/// Gets the created.
		/// </summary>
		/// <value>The created.</value>
		public virtual DateTime Created
	    
		{
			get
			{
				return Database.DBCommon.GetLocalDate(Security.CurrentUser.TimeZoneId,_srcRow.Created);
			}
		}
		
		/// <summary>
		/// Gets from.
		/// </summary>
		/// <value>From.</value>
		public virtual string From
	    
		{
			get
			{
				return _srcRow.From;
			}
			
		}
		
		/// <summary>
		/// Gets to.
		/// </summary>
		/// <value>To.</value>
		public virtual string To
	    
		{
			get
			{
				return _srcRow.To;
			}
			
		}
		
		/// <summary>
		/// Gets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public virtual string Subject
	    
		{
			get
			{
				return _srcRow.Subject;
			}
			
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="EMailMessageLog"/> is incoming.
		/// </summary>
		/// <value><c>true</c> if incoming; otherwise, <c>false</c>.</value>
		public virtual bool Incoming
	    
		{
			get
			{
				return _srcRow.Incoming;
			}
			
		}
		
		/// <summary>
		/// Gets the E mai box id.
		/// </summary>
		/// <value>The E mai box id.</value>
		public virtual int EMailBoxId
	    
		{
			get
			{
				return _srcRow.EMailBoxId;
			}
		}
		
		/// <summary>
		/// Gets the anti spam result.
		/// </summary>
		/// <value>The anti spam result.</value>
		public virtual EMailMessageAntiSpamRuleRusult AntiSpamResult
	    
		{
			get
			{
				return (EMailMessageAntiSpamRuleRusult)_srcRow.AntiSpamResult;
			}
			
		}
		
		#endregion
	}
}
