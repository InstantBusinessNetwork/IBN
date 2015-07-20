using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.Documents;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Web.UI.DocumentManagement.CommandHandlers
{
	public class CloneVersionHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				PrimaryKeyId parentKey = PrimaryKeyId.Parse(cp.CommandArguments["primaryKeyId"]);
				PrimaryKeyId docId = PrimaryKeyId.Parse(((Control)Sender).Page.Request["ObjectId"]);

				DocumentContentVersionEntity docVersion = (DocumentContentVersionEntity)BusinessManager.Load(DocumentContentVersionEntity.GetAssignedMetaClassName(), parentKey);

				DocumentContentVersionEntity newVersion = (DocumentContentVersionEntity)BusinessManager.InitializeEntity(DocumentContentVersionEntity.GetAssignedMetaClassName());
				newVersion.Name = docVersion.Name;
				newVersion.OwnerDocumentId = docId;

				CreateRequest request = new CreateRequest(newVersion);
				request.Parameters.Add(DocumentContentVersionRequestParameters.Create_SourceVersionId, parentKey);

				BusinessManager.Execute(request);
				//CreateResponse response = (CreateResponse)BusinessManager.Execute(request);
				//objectId = response.PrimaryKeyId;

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
