using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml.XPath;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Business.Customization;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class DefaultAdmin : System.Web.UI.UserControl
	{
		#region _nodeId
		private string _nodeId
		{
			get
			{
				if (Request["NodeId"] != null)
					return Request["NodeId"];
				return String.Empty;
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(_nodeId))
			{
				IXPathNavigable navigable;
				// Selector: ClassName.ViewName.PlaceName.ProfileId.UserId
				Selector selector = new Selector(string.Empty, string.Empty, string.Empty, ProfileManager.GetProfileIdByUser().ToString(), Mediachase.IBN.Business.Security.UserID.ToString());

				// don't hide items for administrator
				if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.Administrator))
					navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetCustomizationXml(null, StructureType.Navigation, selector);
				else
					navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.Navigation, selector);

				XPathNavigator link = navigable.CreateNavigator().SelectSingleNode(String.Format("//Link[@id='{0}']", _nodeId));
				if (link != null)
				{
					string mainTitle = UtilHelper.GetResFileString(link.GetAttribute("text", string.Empty));
					lblParentName.Text = mainTitle;

					foreach (XPathNavigator subItem in link.SelectChildren(string.Empty, string.Empty))
					{
						string title = UtilHelper.GetResFileString(subItem.GetAttribute("text", string.Empty));
						string command = subItem.GetAttribute("command", string.Empty);
						string cmd = String.Empty;
						if (!String.IsNullOrEmpty(command))
						{
							if (!CommandManager.IsEnableCommand("", "", "", "", "", command))
								continue;
							cmd = CommandManager.GetCurrent(this.Page).AddCommand("", "", "", command, null);
							cmd = cmd.Replace("\"", "&quot;");
							//node.href = String.Format("javascript:{0}", cmd);
						}
						HtmlTableRow tr = new HtmlTableRow();
						HtmlTableCell tc = new HtmlTableCell();
						tc.InnerHtml = String.Format("<img alt='' src='{0}' /> <a href=\"{1}\">{2}</a>",
							Page.ResolveUrl("~/layouts/images/rect.gif"),
							String.Format("javascript:{{{0}}}", cmd), title);
						tr.Cells.Add(tc);
						mainTable.Rows.Add(tr);
					}
				}
			}
		}
	}
}