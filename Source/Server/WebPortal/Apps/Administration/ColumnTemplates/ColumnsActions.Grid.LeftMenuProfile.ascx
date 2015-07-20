<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.Administration.ColumnTemplates.ColumnsActions_Grid_LeftMenuProfile" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.CustomColumnBaseType" %>
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
	protected string GetValue(object itemId, 
		object hiddenObj, object hiddenParentObj, object hiddenLayerObj, 
		object addedObj, object addedLayerObj,
		object changedObj, object changedLayerObj)
	{
		bool hidden = false;
		if (hiddenObj != DBNull.Value && hiddenObj is bool)
			hidden = (bool)hiddenObj;

		bool hiddenParent = false;
		if (hiddenParentObj != DBNull.Value && hiddenParentObj is bool)
			hiddenParent = (bool)hiddenParentObj;

		string hiddenLayer = String.Empty;
		if (hiddenLayerObj != DBNull.Value)
			hiddenLayer = hiddenLayerObj.ToString();

		bool added = false;
		if (addedObj != DBNull.Value && addedObj is bool)
			added = (bool)addedObj;

		string addedLayer = String.Empty;
		if (addedLayerObj != DBNull.Value)
			addedLayer = addedLayerObj.ToString();

		bool changed = false;
		if (changedObj != DBNull.Value && changedObj is bool)
			changed = (bool)changedObj;

		string changedLayer = String.Empty;
		if (changedLayerObj != DBNull.Value)
			changedLayer = changedLayerObj.ToString();
		
		string retVal = "";
		if (itemId != DBNull.Value)
			retVal = BindAction(itemId.ToString(), hidden, hiddenParent, hiddenLayer, added, addedLayer, changed, changedLayer);
		if (String.IsNullOrEmpty(retVal))
			retVal = "&nbsp;";
		return retVal;
	}
	
	#region BindAction
	/// <summary>
	/// Binds the action.
	/// </summary>
	string BindAction(string itemId, bool hidden, bool hiddenParent, string hiddenLayer,
		bool added, string addedLayer, 
		bool changed, string changedLayer)
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
					dic.Add("hidden", hidden.ToString(CultureInfo.InvariantCulture));
					dic.Add("hiddenParent", hiddenParent.ToString(CultureInfo.InvariantCulture));
					dic.Add("hiddenLayer", hiddenLayer);
					dic.Add("added", added.ToString(CultureInfo.InvariantCulture));
					dic.Add("addedLayer", addedLayer);
					dic.Add("changed", changed.ToString(CultureInfo.InvariantCulture));
					dic.Add("changedLayer", changedLayer);

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
<%# GetValue
	(
		String.IsNullOrEmpty(this.PrimaryKeyIdField) ? DBNull.Value : Eval(this.PrimaryKeyIdField),
		Eval("Hidden"),
		Eval("HiddenParent"),
		Eval("HiddenLayer"),
		Eval("Added"),
		Eval("AddedLayer"),
		Eval("Changed"),
		Eval("ChangedLayer")
	)
%>