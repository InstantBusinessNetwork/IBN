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
	public class ContactGroupsHandler : IJsonHandler
	{
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			int userIMGroupId = 0;
			DataTable dt = null;
			JsonTreeNode node;

			if (!IMGroup.CanCreate())
			{
				using (IDataReader rdr = Mediachase.IBN.Business.User.GetUserInfo(Mediachase.IBN.Business.Security.CurrentUser.UserID))
				{
					rdr.Read();
					userIMGroupId = (int)rdr["IMGroupId"];
				}

				dt = IMGroup.GetListIMGroupsYouCanSee(userIMGroupId);

				string imGroupName = IMGroup.GetIMGroupName(userIMGroupId, null);
				if (imGroupName != null)
				{
					DataRow dr = dt.NewRow();

					dr["IMGroupId"] = userIMGroupId;
					dr["IMGroupName"] = imGroupName;

					dt.Rows.InsertAt(dr, 0);
				}
			}
			else
			{
				dt = IMGroup.GetListIMGroup();
			}

			foreach (DataRow dr in dt.Rows)
			{
				int imGroupId = (int)dr["IMGroupId"];
				node = new JsonTreeNode();
				node.icon = "../../../Layouts/Images/icons/ibngroup.gif";
				node.iconCls = "iconStdCls";
				node.text = dr["IMGroupName"].ToString();
				node.cls = "nodeCls";
				node.href = "../../../Directory/Directory.aspx?Tab=1&IMGroupID=" + imGroupId.ToString();
				node.hrefTarget = "right";
				node.leaf = true;
				nodes.Add(node);
			}

			if (nodes.Count > 0)
				return UtilHelper.JsonSerialize(nodes);
			else
				return String.Empty;
		}

		#endregion
	}
}
