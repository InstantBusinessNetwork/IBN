<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowDefinitionSelect.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.WorkflowDefinitionSelect" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="10" width="100%">
	<tr>
		<td class="ibn-label" style="width:130px;">
			<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: IbnFramework.BusinessProcess, TemplateGroup %>"></asp:Literal>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="TeplateGroupList" Width="100%" 
				AutoPostBack="true" 
				onselectedindexchanged="TeplateGroupList_SelectedIndexChanged"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" style="width:130px;">
			<asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: IbnFramework.BusinessProcess, SelectTemplate %>"></asp:Literal>:
		</td>
		<td>
			<asp:DropDownList runat="server" ID="TemplateList" Width="100%"></asp:DropDownList>
			<asp:Label runat="server" ID="NoTemplatesLabel" Text="<%$Resources: IbnFramework.BusinessProcess, NoTemplates %>" CssClass="ibn-error"></asp:Label>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center">
			<br />
			<ibn:IMButton ID="SaveButton" runat="server" class="text" Text="<%$ Resources: IbnFramework.Common, btnOk %>" OnServerClick="SaveButton_ServerClick" style="width:100px"/>
			<br /><br />
			<ibn:IMButton ID="CancelButton" runat="server" class="text" Text="<%$ Resources: IbnFramework.Common, btnCancel %>" IsDecline="true" style="width:100px"/>
		</td>
	</tr>
</table>