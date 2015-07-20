using System;
using System.Collections;
using Mediachase.IBN.Database.EMail;
using Mediachase.IBN.Database;
using System.Data;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for WhiteListItem.
	/// </summary>
	public class WhiteListItem
	{
		private EMailMessageAntiSpamItemRow _srcRow = null;

		private WhiteListItem(EMailMessageAntiSpamItemRow row)
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

			newRow.IsWhite = true;
			newRow.From = From;

			newRow.Update();

			return newRow.PrimaryKeyId;
		}

		public static void Delete(int WhiteListItemId)
		{
			EMailMessageAntiSpamItemRow.Delete(WhiteListItemId);
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


		public static bool Contains(string from)
		{
			// All IBN User are included into white list (Automatically)
			#region Auto Users
			int emailUser = DBUser.GetUserByEmail(from,false);
			if(emailUser>0)
				return true;

			Client client = Common.GetClient(from);
			if (client != null)
				return true;
			#endregion 

			foreach (EMailMessageAntiSpamItemRow item in EMailMessageAntiSpamItemRow.List(from))
			{
				if(item.IsWhite)
					return true;
			}

			return false;
		}

		public static WhiteListItem[] List(string Keyword)
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailMessageAntiSpamItemRow row in EMailMessageAntiSpamItemRow.List(true, Keyword))
			{
				retVal.Add(new WhiteListItem(row));
			}

			return (WhiteListItem[])retVal.ToArray(typeof(WhiteListItem));
		}

		public static WhiteListItem[] List()
		{
			ArrayList retVal = new ArrayList();

			foreach(EMailMessageAntiSpamItemRow row in EMailMessageAntiSpamItemRow.List(true))
			{
				retVal.Add(new WhiteListItem(row));
			}

			return (WhiteListItem[])retVal.ToArray(typeof(WhiteListItem));
		}
	}
}
