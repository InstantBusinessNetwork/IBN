<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.TimeTracking.Primitives.MenuActions_Grid_TimeTrackingEntry" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.CustomColumnBaseType" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Hosting" %>
<%@ Import Namespace="System.Xml.XPath" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Import Namespace="Mediachase.Ibn.XmlTools" %>
<%@ Import Namespace="Mediachase.IbnNext.TimeTracking" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem)
	{
		if (DataItem != null)
			BindAction(DataItem);

		return string.Empty;
	}
	
	#region BindAction
	/// <summary>
	/// Binds the action.
	/// </summary>
	void BindAction(MetaObject DataItem)
	{
		string retVal = "var menu = new Ext.menu.Menu({ id: 'mainMenu', items: "; //[{text: 'main', menu: { items:
		string jsonVal = string.Empty ;
		
		IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, new Selector(DataItem.GetMetaType().Name, this.ViewName, this.Place));
		XPathNavigator actions = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid/CustomColumns");

		foreach (XPathNavigator gridItem in actions.SelectChildren("Column", string.Empty))
		{
			string type = gridItem.GetAttribute("type", string.Empty);
			string id = gridItem.GetAttribute("id", string.Empty);

			if (type == "MenuActions" && id == this.ColumnId)
			{
				foreach (XPathNavigator actionItem in gridItem.SelectChildren("Item", string.Empty))
				{
					string imageUrl = actionItem.GetAttribute("imageUrl", string.Empty);
					string commandName = actionItem.GetAttribute("commandName", string.Empty);
					string text = actionItem.GetAttribute("text", string.Empty);
					string tooltip = actionItem.GetAttribute("tooltip", string.Empty);

					text = CHelper.GetResFileString(text);
					Dictionary<string, string> dic = new Dictionary<string, string>();

					dic.Add("callFromGrid", "1");

					if (DataItem != null && DataItem.PrimaryKeyId == null && DataItem.Properties["ParentBlockId"].OriginalValue != null)	// Block
					{
						MetaObject ttb = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName("TimeTrackingBlock"), Convert.ToInt32(DataItem.Properties["ParentBlockId"].OriginalValue.ToString()));

						dic.Add("parentBlockId", DataItem.Properties["ParentBlockId"].OriginalValue.ToString());
						dic.Add("primaryKeyId", DataItem.Properties["ParentBlockId"].OriginalValue.ToString() + "::" + "TimeTrackingBlock");

						if (DataItem.Properties["BlockTypeInstanceId"].OriginalValue != null) // dla togo chtoby QuickAdd Control vybral nuzhnij block
							dic.Add("blockTypeInstanceId", DataItem.Properties["BlockTypeInstanceId"].OriginalValue.ToString());
					}
					else if (DataItem != null && DataItem.PrimaryKeyId != null)	// Entry
					{
						dic.Add("primaryKeyId", DataItem.PrimaryKeyId.ToString() + "::" + "TimeTrackingEntry");
					}

					CommandParameters cp = new CommandParameters(commandName, dic);
					bool isEnable = true;
					
					string clientScript = CommandManager.GetCurrent(this.Page).AddCommand(DataItem.GetMetaType().Name, this.ViewName, this.Place, cp, out isEnable);
					clientScript = clientScript.Replace("'", "\"");
					clientScript = clientScript.Remove(clientScript.Length - 1);
					
					imageUrl = this.ResolveClientUrl(imageUrl);					
					
					if (isEnable)
						jsonVal += String.Format("{{ text: '{0}', icon: '{1}', handler: menuItems_OnClickHandler,  tooltip: '{3}', disabledClass: '{2}||{4}' }},", text, imageUrl, commandName, CHelper.GetResFileString(tooltip), cp.ToString());
					
					
				}
			}
		}

		if (jsonVal.Length > 0)
			jsonVal = jsonVal.Remove(jsonVal.Length - 1);

		jsonVal = String.Format("[{0}]", jsonVal);

		
		retVal += jsonVal + "});"; //}}]
		HtmlGenericControl divContainer = (HtmlGenericControl)this.FindControl("menuContainer");
		if (divContainer != null)
		{
			if (jsonVal == "[]")
			{
				divContainer.Style.Add(HtmlTextWriterStyle.Display, "none");
				return;
			}

			retVal += string.Format(" menu.show($get('{0}'));", divContainer.ClientID);
			divContainer.Attributes.Add("onclick", retVal);
		}
		//return retVal;
	}
	#endregion	
</script>
<div runat="server" id="menuContainer" style="cursor: pointer; background-color: Transparent; height: 18px;" onmouseout="menuOnMouseOut(this)" onmousemove="menuOnMouseMove(this)" ><p style="float: left; display: none;">Μενώ</p>
	<img height="16" width="16" alt="menu" border="0" src='<%= this.Page.ResolveUrl("~/images/IbnFramework/menu.png") %>' />
	
	<p style="display: none;">
		<img style="padding-bottom: 5px;" height="5" width="7" border="0" alt="arrMenu" src='<%= this.Page.ResolveUrl("~/images/IbnFramework/arrdown.gif") %>' />
	</p>
</div>
<%# GetValue(DataItem) %>