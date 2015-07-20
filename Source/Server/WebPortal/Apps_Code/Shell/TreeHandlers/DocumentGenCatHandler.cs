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
	public class DocumentGenCatHandler : IJsonHandler
	{
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			using (IDataReader reader = Document.GetListCategoriesAll())
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					JsonTreeNode node = new JsonTreeNode();

					node.iconCls = "iconNodeCls";

					node.text = reader["CategoryName"].ToString();
					node.cls = "nodeCls";

					node.href = "../../../Documents/default.aspx?GenCat=" + CategoryId.ToString();
					node.hrefTarget = "right";

					node.leaf = true;
					node.id = "MDocuments43_" + CategoryId.ToString();
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
