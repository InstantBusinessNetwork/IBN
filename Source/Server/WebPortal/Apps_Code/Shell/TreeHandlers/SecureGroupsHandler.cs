using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
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
	public class SecureGroupsHandler : IJsonHandler
	{
		private const string _rootId = "MGroups21";
		#region IJsonHandler Members

		public string GetJsonDataSource(string nodeId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			int parentId;
			if (!int.TryParse(nodeId, out parentId) || parentId < 1)
			{
				parentId = 1;
			}

			if (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Partner) && (parentId == 1 || parentId == 6))
			{
				int iCurGroupId = -1;
				ArrayList VisiblePartnerGroups = new ArrayList();
				ArrayList VisibleUnpartnerGroups = new ArrayList();
				using (IDataReader reader = Mediachase.IBN.Business.User.GetListSecureGroup(Mediachase.IBN.Business.Security.CurrentUser.UserID))
				{
					if (reader.Read())
					{
						iCurGroupId = (int)reader["GroupId"];
					}
				}
				VisiblePartnerGroups.Clear();
				VisibleUnpartnerGroups.Clear();
				VisiblePartnerGroups.Add(iCurGroupId);
				using (IDataReader reader = SecureGroup.GetListGroupsByPartner(iCurGroupId))
				{
					while (reader.Read())
					{
						int iGroupId = (int)reader["GroupId"];
						if (SecureGroup.IsPartner(iGroupId))
							VisiblePartnerGroups.Add(iGroupId);
						else
							VisibleUnpartnerGroups.Add(iGroupId);
					}
				}
				ClearUnpartnerGroups(VisibleUnpartnerGroups);
				if (VisibleUnpartnerGroups.Contains(1))
				{
					return BindOrdinaryTree(parentId);
				}

				ArrayList children = new ArrayList();
				if (parentId == 1)
				{
					children.AddRange(VisibleUnpartnerGroups);
					children.Add(6);
				}
				else if (parentId == 6)
				{
					children.AddRange(VisiblePartnerGroups);
				}
				children.Sort();
				foreach (int iGroupId in children)
				{
					JsonTreeNode node = new JsonTreeNode();
					node.icon = "../../../Layouts/Images/icons/regular.gif";
					if (iGroupId < 9 && iGroupId != 6)
						node.icon = "../../../Layouts/Images/icons/Admins.gif";
					if (SecureGroup.IsPartner(iGroupId) || iGroupId == 6)
						node.icon = "../../../Layouts/Images/icons/Partners.gif";
					node.iconCls = "iconStdCls";

					using (IDataReader rdr = SecureGroup.GetGroup(iGroupId))
					{
						if (rdr.Read())
							node.text = CHelper.GetResFileString(rdr["GroupName"].ToString());
					}
					node.cls = "nodeCls";

					node.href = "../../../Directory/Directory.aspx?Tab=0&SGroupID=" + iGroupId.ToString();
					node.hrefTarget = "right";

					node.id = Guid.NewGuid().ToString("N");
					node.itemid = iGroupId.ToString();
					node.staticParentId = _rootId;

					node.leaf = true;
					if (iGroupId == 6)
					{
						node.leaf = false;
					}
					else
						using (IDataReader rdr = SecureGroup.GetListChildGroups(iGroupId))
						{
							if (rdr.Read())
								node.leaf = false;
						}

					nodes.Add(node);
				}
			}
			else
				return BindOrdinaryTree(parentId);

			if (nodes.Count > 0)
				return UtilHelper.JsonSerialize(nodes);
			else
				return String.Empty;
		}

		private string BindOrdinaryTree(int parentId)
		{
			List<JsonTreeNode> nodes = new List<JsonTreeNode>();

			using (IDataReader reader = SecureGroup.GetListChildGroups(parentId))
			{
				while (reader.Read())
				{
					int iGroupId = (int)reader["GroupId"];
					JsonTreeNode node = new JsonTreeNode();
					node.icon = "../../../Layouts/Images/icons/regular.gif";
					if (iGroupId < 9 && iGroupId != 6)
						node.icon = "../../../Layouts/Images/icons/Admins.gif";
					if (SecureGroup.IsPartner(iGroupId) || iGroupId == 6)
						node.icon = "../../../Layouts/Images/icons/Partners.gif";
					node.iconCls = "iconStdCls";

					node.leaf = true;
					using (IDataReader rdr = SecureGroup.GetListChildGroups(iGroupId))
					{
						if (rdr.Read())
							node.leaf = false;
					}
					node.text = CHelper.GetResFileString(reader["GroupName"].ToString());
					node.cls = "nodeCls";

					node.href = "../../../Directory/Directory.aspx?Tab=0&SGroupID=" + iGroupId.ToString(CultureInfo.InvariantCulture);
					node.hrefTarget = "right";

					node.id = Guid.NewGuid().ToString("N");
					node.itemid = iGroupId.ToString();
					node.staticParentId = _rootId;

					nodes.Add(node);
				}
			}

			if (nodes.Count > 0)
				return UtilHelper.JsonSerialize(nodes);
			else
				return String.Empty;
		}

		#endregion

		#region ClearUnpartnerGroups
		private void ClearUnpartnerGroups(ArrayList VisibleUnpartnerGroups)
		{
			ArrayList alDelete = new ArrayList();
			for (int i = 0; i < VisibleUnpartnerGroups.Count - 1; i++)
			{
				for (int j = i + 1; j < VisibleUnpartnerGroups.Count; j++)
				{
					int a1 = (int)VisibleUnpartnerGroups[i];
					int a2 = (int)VisibleUnpartnerGroups[j];
					if (IsChild(a1, a2) && !alDelete.Contains(a2))
						alDelete.Add(a2);
				}
			}
			foreach (int k in alDelete)
			{
				VisibleUnpartnerGroups.Remove(k);
			}
		}

		private bool IsChild(int ParentId, int ChildId)
		{
			bool retVal = false;
			int CurParent = ChildId;
			while (CurParent > 1)
			{
				CurParent = SecureGroup.GetParentGroup(CurParent);
				if (CurParent == ParentId)
				{
					retVal = true;
					break;
				}
			}

			return retVal;
		}
		#endregion
	}
}
