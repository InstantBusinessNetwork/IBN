using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;


namespace Mediachase.Ibn.Web.UI.Shell.TreeHandlers
{
	public class DocumentStatusHandler : IJsonHandler
	{
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			using (IDataReader reader = Document.GetListDocumentStatus())
			{
				while (reader.Read())
				{
					int StatusId = (int)reader["StatusId"];
					JsonTreeNode node = new JsonTreeNode();

					node.iconCls = "iconNodeCls";

					node.text = reader["StatusName"].ToString();
					node.cls = "nodeCls";

					node.href = "../../../Documents/default.aspx?DocStatus=" + StatusId.ToString();
					node.hrefTarget = "right";

					node.leaf = true;
					node.id = "MDocuments41_" + StatusId.ToString();
					nodes.Add(node);
				}
			}

			if (nodes.Count > 0)
				return UtilHelper.JsonSerialize(nodes);
			else
				return String.Empty;
		}

		#endregion
	}
}
