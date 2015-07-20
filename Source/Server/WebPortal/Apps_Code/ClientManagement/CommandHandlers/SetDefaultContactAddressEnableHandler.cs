using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Clients;

namespace Mediachase.Ibn.Web.UI.ClientManagement.CommandHandlers
{
	public class SetDefaultContactAddressEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = true;

			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (cp.CommandArguments["primaryKeyId"] == null)
					throw new ArgumentException("PrimaryKeyId is null for SetDefaultContactAddressEnableHandler");

				PrimaryKeyId pk = PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]);
				EntityObject entity = BusinessManager.Load(AddressEntity.GetAssignedMetaClassName(), pk);
				retval = !(bool)entity.Properties["IsDefaultContactElement"].Value;
			}

			return retval;
		}
		#endregion
	}
}
