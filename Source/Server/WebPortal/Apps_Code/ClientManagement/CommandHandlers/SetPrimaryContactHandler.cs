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
	public class SetPrimaryContactHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters && !String.IsNullOrEmpty(((Control)Sender).Page.Request["ObjectId"]))
			{
				CommandParameters cp = (CommandParameters)Element;

				if (cp.CommandArguments["primaryKeyId"] == null)
					throw new ArgumentException("PrimaryKeyId is null for SetPrimaryContactEnableHandler");

				PrimaryKeyId contactPk = PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]);
				PrimaryKeyId orgPk = PrimaryKeyId.Parse(((Control)Sender).Page.Request["ObjectId"]);

				EntityObject entity = BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), orgPk);
				entity.Properties["PrimaryContactId"].Value = contactPk;
				BusinessManager.Update(entity);

				CHelper.RequireDataBind();
			}
		}
		#endregion
	}
}
