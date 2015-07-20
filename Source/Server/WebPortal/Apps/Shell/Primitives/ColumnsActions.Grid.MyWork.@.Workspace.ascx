<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.CustomColumnBaseType" ClassName="Mediachase.UI.Web.Shell.Primitives.ColumnsActions_Grid_MyWork_All_Workspace" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml.XPath" %>
<%@ Import Namespace="Mediachase.Ibn.XmlTools" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>

<script language="c#" runat="server">
	protected string GetValue(object dataItem)
	{
		string retVal = "";
		if (dataItem != null && dataItem is System.Data.DataRowView)
			retVal = BindAction(((System.Data.DataRowView)dataItem).Row);
		return retVal;
	}
	
	#region BindAction
	/// <summary>
	/// Binds the action.
	/// </summary>
	string BindAction(System.Data.DataRow DataItem)
	{	
		string retVal = string.Empty;
		if (this.ClassName == null || DataItem["type"] == DBNull.Value)
			return string.Empty;
		
		IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, new Selector(this.ClassName, this.ViewName, this.Place));
		XPathNavigator actions = navigable.CreateNavigator().SelectSingleNode("MetaView/Grid/CustomColumns");

		foreach (XPathNavigator gridItem in actions.SelectChildren("Column", string.Empty))
		{
			string type = gridItem.GetAttribute("type", string.Empty);
			string id = gridItem.GetAttribute("id", string.Empty);

			if (type == "ColumnsActions" && id == this.ColumnId)
			{
				foreach (XPathNavigator actionItem in gridItem.SelectChildren("Item", string.Empty))
				{
					string imageUrl = actionItem.GetAttribute("imageUrl", string.Empty);
					string commandName = actionItem.GetAttribute("commandName", string.Empty);
					string paddingLeft = actionItem.GetAttribute("paddingLeft", string.Empty);
					string paddingRight = actionItem.GetAttribute("paddingRight", string.Empty);
					string tooltip = actionItem.GetAttribute("tooltip", string.Empty);

					Dictionary<string, string> dic = new Dictionary<string, string>();

					if (DataItem["objectId"] != null && DataItem["objectTypeId"] != null && DataItem["userId"] != null)
					{
						dic.Add("objectId", DataItem["objectId"].ToString());
						dic.Add("objectTypeId", DataItem["objectTypeId"].ToString());
						
						if (DataItem["assignmentId"] != null)
							dic.Add("assignmentId", DataItem["assignmentId"].ToString());
						
						dic.Add("userId", DataItem["userId"].ToString());
					}
					else
					{
						dic.Add("objectId", "");
						dic.Add("objectTypeId", "");
						dic.Add("assignmentId", "");
						dic.Add("userId", "");
					}

					CommandParameters cp = new CommandParameters(commandName, dic);
					bool isEnable = true;
					string clientScript = CommandManager.GetCurrent(this.Page).AddCommand(this.ClassName, this.ViewName, this.Place, cp, out isEnable);
					imageUrl = this.ResolveClientUrl(imageUrl);


					//create ImageButton control			
					ImageButton img = new ImageButton();
					img.ImageUrl = imageUrl;

					if (tooltip != string.Empty)
					{
						tooltip = CHelper.GetResFileString(tooltip);
						img.ToolTip = tooltip;
					}

					img.OnClientClick = string.Format("{0}; return false;", clientScript);

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
<%# GetValue(DataBinder.GetDataItem(Container))%>