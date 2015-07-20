<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListInfoMove.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.ListInfoMove" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" src="~/Modules/ObjectDropDownNew.ascx" %>
<link href='<%= this.ResolveClientUrl("~/styles/IbnFramework/treeStyle.css")%>' type="text/css" rel="stylesheet" />
<script type="text/javascript">
	function ChangeProject()
	{
		<%= Page.ClientScript.GetPostBackEventReference(lbChangeProject, "")%>
	}	
	
	function onNodeClick(node)
	{
		var obj = document.getElementById('<%=btnMove.ClientID%>');
		var obj1 = document.getElementById('<%=btnMoveAssign.ClientID%>');
		if(node.Value==-1)
		{
			obj.disabled = true;
			obj1.disabled = true;
			obj.style.color = "#aeaeae";
			obj1.style.color = "#aeaeae";
		}
		else
		{
			obj.disabled = false;
			obj1.disabled = false;
			obj.style.color = "#000000";
			obj1.style.color = "#000000";
			document.forms[0].<%= destFolderId.ClientID%>.value = node.Value;
		}
	}		
</script>
<style type="text/css">
	.SelTrNode 
	{ 
		font-family: tahoma; 
		font-size: 11px; 
		background-color: gray; 
		color:white; 
		padding-top:2px;
		padding-bottom:1px;
		padding-left: 3px; 
		padding-right: 3px; 
		cursor: pointer;
	}
</style>
<asp:UpdatePanel ID="upPanel" runat="server">
	<ContentTemplate>
	
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td>
			<ibn:BlockHeader id="secHeader" runat="server"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td class="ibn-alternating ibn-navline">
			<asp:RadioButtonList ID="rbList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rbList_SelectedIndexChanged" RepeatDirection="Horizontal"></asp:RadioButtonList>
		</td>
	</tr>
	<tr id="trProject" runat="server">
		<td class="ibn-navline">
			<table cellpadding="7" cellspacing="0" border="0" width="100%">
				<tr>
					<td width="150px"><b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.ListInfo:tSelectProject}") %>:</b></td>
					<td>
						<ibn:ObjectDD ID="ucProject" OnChange="ChangeProject()" ObjectTypes="3" runat="server" Width="99%" ItemCount="4" ClassName="" ViewName="" PlaceName="ListInfoList" CommandName="MC_ListApp_PM_ObjectDD" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trTree" runat="server">
		<td class="ibn-navline">
			<ComponentArt:TreeView id="MoveTree" Width="99%" Height="320px" 
				AutoScroll = "True"
				BorderWidth = "0"
				DragAndDropEnabled="false"
				NodeEditingEnabled="false" 
				CssClass="TreeView" 
				NodeCssClass="TreeNode" 
				SelectedNodeCssClass="SelTrNode"
				HoverNodeCssClass="HoverTreeNode"
				NodeEditCssClass="NodeEdit"
				DefaultImageWidth="16" 
				DefaultImageHeight="16"
				NodeLabelPadding="2"
				ParentNodeImageUrl="~/layouts/images/folder.gif" 
				ExpandedParentNodeImageUrl="~/layouts/images/folder_open.gif"
				LeafNodeImageUrl="~/layouts/images/folder.gif" 
				ShowLines="true" 
				ClientScriptLocation = "~/Scripts/componentart_webui_client/"
				EnableViewState="False"
				LineImagesFolderUrl="~/layouts/images/lines/"
				runat="server">
			</ComponentArt:TreeView>
		</td>
	</tr>
	<tr>
		<td align="right" valign="middle" style="padding: 3px 10px 3px 0px">
			<btn:IMButton class="text" id="btnMoveAssign" Runat="server" />&nbsp;&nbsp;
			<btn:IMButton ID="btnMove" runat="server" class="text" />
		</td>
	</tr>
</table>
<input type="hidden" id="destFolderId" name="destFolderId" runat="server" />
<asp:LinkButton ID="lbChangeProject" runat="server" OnClick="lbChangeProject_Click" style="display:none;"></asp:LinkButton>

</ContentTemplate>
</asp:UpdatePanel>