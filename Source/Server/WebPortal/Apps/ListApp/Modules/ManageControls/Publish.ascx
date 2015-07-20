<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Publish.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.Publish" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="ibn" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<style type="text/css">
#DataTable td
{
	padding:5px;
}
#DataTable td span.rightColumn
{
	display: none;
}
.rightColumn
{
	position: absolute;
	right: 15px;
	color: #666666!important;
}
.nodeCls, .x-panel-bwrap .x-panel-body
{
	position: relative;
}
* 
{
	zoom: 1;	
}
.MenuTree
{
	display: none;
}
</style>
<script type="text/javascript">
function SelectNode(node, args)
{
	var path = node.id;
	var text = node.text;
	while (node.parentNode && node.parentNode.id && node.parentNode.id != "FL_root_id")
	{
		node = node.parentNode;
		path = node.id + "/" + path;
	}
	
	var hiddenField = document.getElementById('<%= NodeIdField.ClientID%>');
	if (hiddenField)
		hiddenField.value = path;
		
	var selectedItemLabel = document.getElementById('<%= SelectedItemLabel.ClientID %>');
	if (selectedItemLabel)
	{
		selectedItemLabel.innerHTML = text;
		selectedItemLabel.className = "";
	}
}

function InitMenuTree()
{
	var menuTree = $find('<%=MenuTree.ClientID%>');
	if (menuTree && menuTree.extTreePanel)
		menuTree.extTreePanel.on('click', SelectNode);
}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td colspan="2">
			<ibn:BlockHeader id="MainHeader" runat="server" Title="<%$ Resources: IbnFramework.ListInfo, Publication %>"></ibn:BlockHeader>
		</td>
	</tr>
	<tr>
		<td style="width:300px; padding-left:8px;">
			<div class="ibn-stylebox2" style="height:415px; position:relative; top:1px;bottom: 1px;">
				<ibn:JsTreePanel FillToContainer="true" AutoScroll="true" id="MenuTree" runat="server" IconCss="iconStdCls" NodeTextCss="nodeCls" CssClass="MenuTree"></ibn:JsTreePanel>				
			</div>
			
		</td>
		<td valign="top" style="padding-right:8px; padding-left:8px;">
			<table cellpadding="0" cellspacing="3" width="100%" id="DataTable">
				<tr>
					<td class="ibn-label" style="width:160px;">
						<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: IbnFramework.GlobalMetaInfo, DisplayRegion %>"></asp:Literal>:
					</td>
					<td> 
						<asp:Label runat="server" ID="SelectedItemLabel" Text="<%$ Resources: IbnShell.Navigation, ItemNotSelected %>" CssClass="ibn-error"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: IbnFramework.GlobalMetaInfo, DisplayText %>"></asp:Literal>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="ItemText" Width="200"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="ItemTextRequiredValidator" ControlToValidate="ItemText" Display="static" ErrorMessage="*" Font-Bold="true"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal ID="Literal3" runat="server" Text="<%$ Resources: IbnFramework.GlobalMetaInfo, DisplayOrder %>"></asp:Literal>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="ItemOrder" Width="200" Text="10000"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="ItemOrderRequiredValidator" ControlToValidate="ItemOrder" Display="dynamic" ErrorMessage="*" Font-Bold="true">
						</asp:RequiredFieldValidator><asp:RangeValidator runat="server" ID="ItemOrderRangeValidator" ControlToValidate="ItemOrder" CultureInvariantValues="true" Type="Integer" MinimumValue="0" MaximumValue="999999" ErrorMessage="*" Display="dynamic" Font-Bold="true"></asp:RangeValidator>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<br /><br /><br /><br />
						<ibn:IMButton ID="PublishButton" runat="server" class="text" Text="<%$ Resources: IbnShell.Navigation, Publish %>" OnServerClick="PublishButton_ServerClick" width="150px"/>
						<br /><br />
						<ibn:IMButton ID="CloseButton" runat="server" class="text" Text="<%$ Resources: IbnFramework.ListInfo, tClose %>" IsDecline="true" width="150px"/>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:HiddenField runat="server" ID="NodeIdField" />