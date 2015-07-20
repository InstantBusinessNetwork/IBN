using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;


namespace Mediachase.Ibn.Web.UI.Shell.TreeHandlers
{
	public class DocumentPriorityHandler : IJsonHandler
	{
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			using (IDataReader reader = Document.GetListPriorities())
			{
				while (reader.Read())
				{
					int PriorityId = (int)reader["PriorityId"];
					JsonTreeNode node = new JsonTreeNode();

					node.iconCls = "iconNodeCls";

					node.text = reader["PriorityName"].ToString();
					node.cls = "nodeCls";

					node.href = "../../../Documents/default.aspx?DocPrty=" + PriorityId.ToString();
					node.hrefTarget = "right";

					node.leaf = true;
					node.id = "MDocuments42_" + PriorityId.ToString();
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
