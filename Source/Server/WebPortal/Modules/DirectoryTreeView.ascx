<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.DirectoryTreeView" Codebehind="DirectoryTreeView.ascx.cs" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<link href='<%= ResolveClientUrl("~/styles/IbnFramework/treeStyle.css") %>' type="text/css" rel="stylesheet" />
<script type="text/javascript">
function _dtSV(strName,strKey,strId)
{
    var elem1=document.getElementById('<%= txtFolder.ClientID %>');
    if(!elem1)
		return;
	var elem2=document.getElementById('<%= txtContainerKey.ClientID %>');
    if(!elem2)
		return;
	var elem3=document.getElementById('<%= txtFolderId.ClientID %>');
    if(!elem3)
		return;
    elem1.value=strName;
    elem2.value=strKey;
    elem3.value=strId;
}
</script>
<asp:TextBox CssClass="text" ID="txtFolder" ReadOnly="True" Runat=server Width="250px" /><asp:CustomValidator ID="cvFolder" CssClass="text" ErrorMessage="*" Runat="server" Display="Dynamic" Enabled="False" /><br>
<input id="txtContainerKey" type="hidden" runat="server" NAME="txtContainerKey">
<input id="txtFolderId" type="hidden" runat="server" NAME="txtFolderId">
<ComponentArt:TreeView id="TreeView1" Height="150px" Width="250px" 
EnableViewState="false"
DragAndDropEnabled="false" 
NodeEditingEnabled="false"
KeyboardEnabled="true"
CssClass="TreeView" 
NodeCssClass="TreeNode" 
SelectedNodeCssClass="SelectedTreeNode" 
MultipleSelectedNodeCssClass="MultipleSelectedTreeNode"
HoverNodeCssClass="HoverTreeNode"
NodeEditCssClass="NodeEdit"
CutNodeCssClass="CutTreeNode"
MarginWidth="24"
DefaultMarginImageWidth="24"
DefaultMarginImageHeight="20"
LineImageWidth="19" 
LineImageHeight="20"
DefaultImageWidth="16" 
DefaultImageHeight="16"
NodeLabelPadding="3"
ParentNodeImageUrl="~/layouts/images/folder2.gif" 
LeafNodeImageUrl="~/layouts/images/folder2.gif" 
ExpandCollapseImageWidth="19"
ExpandCollapseImageHeight="20"
ShowLines="true" 
ClientScriptLocation = "~/Scripts/componentart_webui_client/" 
LineImagesFolderUrl="~/layouts/images/lines/"
runat="server" >
<ServerTemplates>
<ComponentArt:NavigationCustomTemplate id="UnreadItemsTemplate">
	<Template>
	<div style="padding-left:1px;"><b><%# DataBinder.Eval(Container.DataItem, "Text") %></b>
		<font color="blue">(<%# Container.Attributes["UnreadItems"] %>)</font>
	</div>
	</Template>
</ComponentArt:NavigationCustomTemplate>

<ComponentArt:NavigationCustomTemplate id="InfoItemsTemplate">
	<Template>
	<b><%# DataBinder.Eval(Container.DataItem, "Text") %></b>
	<font color="green">[<%# Container.Attributes["InfoItems"] %>]</font>
	</Template>
</ComponentArt:NavigationCustomTemplate>
</ServerTemplates>
</ComponentArt:TreeView>
