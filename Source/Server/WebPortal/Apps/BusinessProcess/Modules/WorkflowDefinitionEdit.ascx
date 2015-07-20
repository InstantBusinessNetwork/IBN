<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkflowDefinitionEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.WorkflowDefinitionEdit" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ObjectDD" Src="~/Modules/ObjectDropDownNew.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/Apps/BusinessProcess/Styles/Schema.css")%>' />

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
						<asp:Literal runat="server" ID="TemplateGroupLiteral" Text="<%$ Resources: IbnFramework.BusinessProcess, TemplateGroup %>"></asp:Literal>:
					</td>
					<td>
						<asp:UpdatePanel runat="server" ID="TemplateGroupUpdatePanel" RenderMode="Block" UpdateMode="Conditional">
							<ContentTemplate>
								<asp:DropDownList runat="server" ID="TemplateGroupList" Width="460"></asp:DropDownList>
								<button id="EditItemsButton" runat="server" style="border:0px;padding:0px;height:20px;width:22px;background-color:transparent" type="button"><img 
									height="20" title='<%=HttpContext.GetGlobalResourceObject("IbnFramework.Common", "Edit") %>' src='<%=ResolveClientUrl("~/Images/IbnFramework/dictionary_edit.gif")%>' width="22" border="0" /></button>
								<asp:Button id="RefreshButton" runat="server" CausesValidation="False" style="display:none;" OnClick="RefreshButton_Click"></asp:Button>
							</ContentTemplate>
						</asp:UpdatePanel>
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
						<asp:Literal runat="server" ID="Literal2" Text="<%$ Resources: IbnFramework.BusinessProcess, OverdueAssignments %>"></asp:Literal>:
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
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="ProjectLiteral" Text="<%$ Resources: IbnFramework.Global, _mc_Project %>"></asp:Literal>:
					</td>
					<td>
						<ibn:ObjectDD ID="ProjectControl" ObjectTypes="3" ObjectTypeId="3" runat="server" Width="480" />
					</td>
				</tr>
			</table>
			<asp:UpdatePanel runat="server" ID="MainUpdatePanel" RenderMode="Block" UpdateMode="Conditional">
				<ContentTemplate>
					<table class="text" cellspacing="0" cellpadding="5" border="0" style="table-layout:fixed;">
						<tr>
							<td class="ibn-label" style="width:150px;">
								<asp:Literal runat="server" ID="Literal1" Text="<%$ Resources: IbnFramework.BusinessProcess, ObjectTypes %>"></asp:Literal>:
							</td>
							<td>
								<asp:CheckBox runat="server" ID="DocumentCheckBox" Text="<%$ Resources: IbnFramework.Admin, ObjectType_Document %>" Checked="true" />&nbsp;
								<asp:CheckBox runat="server" ID="TodoCheckBox" Text="<%$ Resources: IbnFramework.Admin, ObjectType_ToDo %>" Checked="true" />&nbsp;
								<asp:CheckBox runat="server" ID="TaskCheckBox" Text="<%$ Resources: IbnFramework.Admin, ObjectType_Task %>" Checked="true" />&nbsp;
								<asp:CheckBox runat="server" ID="IncidentCheckBox" Text="<%$ Resources: IbnFramework.Admin, ObjectType_Incident %>" Checked="true" />&nbsp;
							</td>
						</tr>
					</table>
					
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
