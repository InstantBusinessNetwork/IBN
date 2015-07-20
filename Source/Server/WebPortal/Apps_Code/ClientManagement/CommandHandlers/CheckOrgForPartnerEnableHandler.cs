using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ClientManagement.CommandHandlers
{
	public class CheckOrgForPartnerEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = true;

			if (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				retval = false;

				// get the partner groupId
				int groupId = User.GetGroupForPartnerUser(Mediachase.IBN.Business.Security.CurrentUser.UserID);

				// Check that partner group has OrgUid
				using (IDataReader reader = SecureGroup.GetGroup(groupId))
				{
					if (reader.Read())
					{
						if (reader["OrgUid"] != DBNull.Value)
							retval = true;
					}
				}
			}

			return retval;
		}
		#endregion
	}
}
