<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowInstanceEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.WorkFlowInstanceEdit" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/Apps/BusinessProcess/Styles/Schema.css")%>' />
<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/styles/IbnFramework/calendar.css")%>' />

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:1px">
	<tr>
		<td><ibn:blockheader id="MainHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="padding:5px" valign="top">
			<table class="text" cellspacing="0" cellpadding="5" border="0" style="table-layout:fixed;">
				<tr>
					<td class="ibn-label" style="width:150px;">
						<asp:Literal runat="server" ID="NameLabel" Text="<%$ Resources: IbnFramework.BusinessProcess, Name %>"></asp:Literal>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="NameText" Width="480"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="NameTextValidator" ControlToValidate="NameText" ErrorMessage="*"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" valign="top">
						<asp:Literal runat="server" ID="DescriptionLabel" Text="<%$ Resources: IbnFramework.BusinessProcess, Description %>"></asp:Literal>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="DescriptionText" Width="480" TextMode="MultiLine" Rows="5"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="SchemaMasterLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, SchemaMaster %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="SchemaMasterList" Width="480" AutoPostBack="true" 
							onselectedindexchanged="SchemaMasterList_SelectedIndexChanged"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="Literal1" Text="<%$ Resources: IbnFramework.BusinessProcess, OverdueAssignments %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="AssignmentOverdueActionList" Width="480"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td>
						<asp:CheckBox runat="server" ID="LockLibraryCheckBox" Text="<%$Resources: IbnFramework.BusinessProcess, LockLibrary %>" />
					</td>
				</tr>
			</table>
			<asp:UpdatePanel runat="server" ID="MainUpdatePanel" RenderMode="Block" UpdateMode="Conditional">
				<ContentTemplate>
					<asp:PlaceHolder runat="server" ID="MainPlaceHolder"></asp:PlaceHolder>
					<div style="padding-top:10px; padding-left:170px;">
						<btn:imbutton class="text" id="SaveButton" style="width:110px;" 
							Runat="server" onserverclick="SaveButton_ServerClick" />&nbsp;&nbsp;
						<btn:imbutton class="text" id="CancelButton" style="width:110px;" 
							Runat="server" IsDecline="true" CausesValidation="false" 
							onserverclick="CancelButton_ServerClick" />
					</div>
				</ContentTemplate>
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="SchemaMasterList" />
				</Triggers>
			</asp:UpdatePanel>
		</td>
	</tr>
</table>
