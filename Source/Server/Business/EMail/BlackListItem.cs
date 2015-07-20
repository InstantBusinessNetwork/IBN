using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for BlackListItem.
	/// </summary>
	public class BlackListItem
	{
		private EMailMessageAntiSpamItemRow _srcRow = null;

		private BlackListItem(EMailMessageAntiSpamItemRow row)
		{
			_srcRow = row;
		}

		public int Id
		{
			get 
			{
				return _srcRow.EMailMessageAntiSpamItemId;
			}
		}

		public string From
		{
			get 
			{
				return _srcRow.From;
			}
			set 
			{
				_srcRow.From = value;
			}
		}

		public static int Create(string From)
		{
			EMailMessageAntiSpamItemRow newRow = new EMailMessageAntiSpamItemRow();

			newRow.IsWhite = false;
			newRow.From = From;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		public static void Delete(int BlackListItemId)
		{
			EMailMessageAntiSpamItemRow.Delete(BlackListItemId);
		}

		public static void Delete(ArrayList ItemIds)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int ItemId in ItemIds)
				{
					Delete(ItemId);
				}

				tran.Commit();
			}
		}

		public static bool Contains(string From)
		{
            if (string.IsNullOrEmpty(From))
                return true;

			foreach(EMailMessageAntiSpamItemRow item in EMailMessageAntiSpamItemRow.List(From))
			{
				if(!item.IsWhite)
					return true;
			}

			return false;
		}

		public static BlackListItem[] List(string Keyword)
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailMessageAntiSpamItemRow row in EMailMessageAntiSpamItemRow.List(false, Keyword))
			{
				retVal.Add(new BlackListItem(row));
			}

			return (BlackListItem[])retVal.ToArray(typeof(BlackListItem));
		}

		public static BlackListItem[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailMessageAntiSpamItemRow row in EMailMessageAntiSpamItemRow.List(true))
			{
				retVal.Add(new BlackListItem(row));
			}

			return (BlackListItem[])retVal.ToArray(typeof(BlackListItem));
		}
	}
}
