<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.CustomColumnBaseType" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.ColumnsActions_Grid" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Hosting" %>
<%@ Import Namespace="System.Xml.XPath" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Import Namespace="Mediachase.Ibn.XmlTools" %>
<script language="c#" runat="server">
	protected string GetValue(object itemId)
	{
		string retVal = "";
		if (itemId != DBNull.Value)
			retVal = BindAction(itemId.ToString());
		else
			RegisterCommands();
		if (String.IsNullOrEmpty(retVal))
			retVal = "&nbsp;";
		return retVal;
	}

	void RegisterCommands()
	{
		if (String.IsNullOrEmpty(this.ClassName))
			return;
		IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, new Selector(this.ClassName, this.ViewName, this.Place));
		XPathNavigator actions = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid/CustomColumns");

		foreach (XPathNavigator gridItem in actions.SelectChildren("Column", string.Empty))
		{
			string type = gridItem.GetAttribute("type", string.Empty);
			string id = gridItem.GetAttribute("id", string.Empty);

			if (type == "ColumnsActions" && id == this.ColumnId)
			{
				XPathNodeIterator iterator = gridItem.SelectChildren("Item", string.Empty);
				foreach (XPathNavigator actionItem in iterator)
				{
					string commandName = actionItem.GetAttribute("commandName", string.Empty);
					CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.Place, commandName);
				}
			}
		}
	}
	
	#region BindAction
	/// <summary>
	/// Binds the action.
	/// </summary>
	string BindAction(string itemId)
	{	
		string retVal = string.Empty;
		
		IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, new Selector(this.ClassName, this.ViewName, this.Place));
		XPathNavigator actions = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid/CustomColumns");

		foreach (XPathNavigator gridItem in actions.SelectChildren("Column", string.Empty))
		{
			string type = gridItem.GetAttribute("type", string.Empty);
			string id = gridItem.GetAttribute("id", string.Empty);

			if (type == "ColumnsActions" && id == this.ColumnId)
			{
				XPathNodeIterator iterator = gridItem.SelectChildren("Item", string.Empty);
				foreach (XPathNavigator actionItem in iterator)
				{
					string imageUrl = actionItem.GetAttribute("imageUrl", string.Empty);
					string commandName = actionItem.GetAttribute("commandName", string.Empty);
					string paddingLeft = actionItem.GetAttribute("paddingLeft", string.Empty);
					string paddingRight = actionItem.GetAttribute("paddingRight", string.Empty);
					string tooltip = actionItem.GetAttribute("tooltip", string.Empty);

					Dictionary<string, string> dic = new Dictionary<string, string>();

					dic.Add("primaryKeyId", itemId);

					CommandParameters cp = new CommandParameters(commandName, dic);
					bool isEnable = true;
					string clientScript = "";
					if(!String.IsNullOrEmpty(commandName))
						clientScript = CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.Place, cp, out isEnable);
					imageUrl = this.ResolveClientUrl(imageUrl);

					//create ImageButton control
					ImageButton img = new ImageButton();
					img.ImageUrl = imageUrl;

					if (tooltip != string.Empty)
					{
						tooltip = CHelper.GetResFileString(tooltip);
						img.ToolTip = tooltip;
					}
					
					img.OnClientClick = string.Format("{0} return false;", clientScript);
					
					if (paddingRight != string.Empty)
						img.Style.Add(HtmlTextWriterStyle.PaddingRight, paddingRight + "px");
					
					if (paddingLeft != string.Empty)
						img.Style.Add(HtmlTextWriterStyle.PaddingLeft, paddingLeft + "px");

					//ImageButton control -> toString()
					StringBuilder sb = new StringBuilder();
					StringWriter tw = new StringWriter(sb);
					HtmlTextWriter hw = new HtmlTextWriter(tw);
					img.RenderControl(hw);
					//add image button
					if (isEnable)
						retVal += sb.ToString();
				}
			}
		}

		return retVal;
	}
	#endregion
</script>
<%# GetValue(String.IsNullOrEmpty(this.PrimaryKeyIdField) ? DBNull.Value : Eval(this.PrimaryKeyIdField))%>