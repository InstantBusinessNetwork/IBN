<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Miscellaneous.ascx.cs" Inherits="Mediachase.UI.Web.Admin.Modules.Miscellaneous" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<table border="0" cellpadding="0" cellspacing="0" width="100%" class="ibn-stylebox2 text">
	<tr>
		<td colspan="2">
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td style="width:50%; padding:5px;" valign="top">
			<ibn:BlockHeaderLight2 ID="AuditHeader" runat="server" HeaderCssClass="ibn-toolbar-light" Title="<%$Resources : IbnFramework.Admin, Audit%>" />
			<table class="ibn-stylebox-light" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td style="padding:5px;" class="text info" valign="top" >
						<div class="ibn-padding5">
							<asp:CheckBox runat="server" ID="AuditWebLogin" Text="<%$Resources : IbnFramework.Admin, AuditWebLogin%>" />
						</div>
						<div class="ibn-padding5">
							<asp:CheckBox runat="server" ID="AuditIbnClientLogin" />
						</div>
						<div class="ibn-padding5" style="text-align:center;">
							<ibn:ImButton ID="SaveButton" Runat="server" class="text" style="width:110px;" Text="<%$Resources : IbnFramework.Common, tSave%>" onserverclick="SaveButton_ServerClick"></ibn:ImButton>
						</div>
						<div style="text-align:center">
							<asp:Label runat="server" ID="SavedLabel" Text="<%$Resources : IbnFramework.Admin, SettingsSaved%>" ForeColor="#4682B4"></asp:Label>
						</div>
					</td>
				</tr>
			</table>
			
			<ibn:BlockHeaderLight2 ID="ProjectViewHeader" runat="server" HeaderCssClass="ibn-toolbar-light" Title="<%$Resources : IbnFramework.Admin, ProjectView%>" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr runat="server" id="ProjectWithTabsRow">
					<td class="info">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<asp:Literal runat="server" ID="Literal3" Text="<%$Resources : IbnFramework.Admin, ProjectWithTabs%>"></asp:Literal>
							<div class="text" style="padding:10px; text-align:center">
								<asp:LinkButton runat="server" ID="SwitchToProjectWithLeftMenuButton" 
									Text="<%$ Resources: IbnFramework.Admin, SwitchToProjectWithLeftMenu %>" 
									Font-Bold="true" Font-Underline="true" ForeColor="Red" 
									onclick="SwitchToProjectWithLeftMenuButton_Click"></asp:LinkButton>
							</div>
						</blockquote>
					</td>
				</tr>
				<tr runat="server" id="ProjectWithLeftMenuRow">
					<td class="info">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<asp:Literal runat="server" ID="Literal4" Text="<%$Resources : IbnFramework.Admin, ProjectWithLeftMenu%>"></asp:Literal>
							<div class="text" style="padding:10px; text-align:center">
								<asp:LinkButton runat="server" ID="SwitchToProjectWithTabsButton" 
									Text="<%$ Resources: IbnFramework.Admin, SwitchToProjectWithTabs %>" 
									Font-Bold="true" Font-Underline="true" ForeColor="Red" 
									onclick="SwitchToProjectWithTabsButton_Click"></asp:LinkButton>
							</div>
						</blockquote>
					</td>
				</tr>
			</table>
		</td>
		<td style="width:50%; padding:5px;" valign="top">
			<ibn:BlockHeaderLight2 ID="ModeHeader" runat="server" HeaderCssClass="ibn-toolbar-light" Title="<%$Resources : IbnFramework.Admin, OperationMode%>"/>
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr runat="server" id="DevModeRow">
					<td  class="info">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<asp:Literal runat="server" ID="Literal6" Text="<%$Resources : IbnFramework.Admin, DevMode%>"></asp:Literal>
							<div class="text" style="padding:10px; text-align:center">
								<asp:LinkButton runat="server" ID="ButtonSwitchToAdmin" Text="<%$ Resources: IbnFramework.Admin, SwitchToAdminMode %>" Font-Bold="true" Font-Underline="true" ForeColor="Red" OnClick="ButtonSwitchToAdmin_Click" ></asp:LinkButton>
							</div>
						</blockquote>
					</td>
				</tr>
				<tr runat="server" id="AdminModeRow">
					<td class="info">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<asp:Literal runat="server" ID="Literal7" Text="<%$Resources : IbnFramework.Admin, AdminMode%>"></asp:Literal>
							<div class="text" style="padding:10px; text-align:center">
								<asp:LinkButton runat="server" ID="ButtonSwitchToDev" Text="<%$ Resources: IbnFramework.Admin, SwitchToDevMode %>" Font-Bold="true" Font-Underline="true" ForeColor="Red" OnClick="ButtonSwitchToDev_Click" ></asp:LinkButton>
							</div>
						</blockquote>
					</td>
				</tr>
			</table>
			
			<ibn:BlockHeaderLight2 ID="RssHeader" runat="server" HeaderCssClass="ibn-toolbar-light" Title="" />
			<table class="ibn-stylebox-light text" cellspacing="0" cellpadding="7" width="100%" border="0">
				<tr runat="server" id="RssEnableRow">
					<td class="info">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<asp:Literal runat="server" ID="Literal1" Text="<%$Resources : IbnFramework.Admin, RssEnableDescription%>"></asp:Literal>
							<div class="text" style="padding:10px; text-align:center">
								<asp:LinkButton runat="server" ID="lblEnableRss" Text="<%$ Resources: IbnFramework.Admin, RssEnableText %>" Font-Bold="true" Font-Underline="true" ForeColor="Red"></asp:LinkButton>
							</div>
						</blockquote>
					</td>
				</tr>
				<tr runat="server" id="RssDisableRow">
					<td class="info">
						<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
							<asp:Literal runat="server" ID="Literal2" Text="<%$Resources : IbnFramework.Admin, RssDisableDescription%>"></asp:Literal>
							<div class="text" style="padding:10px; text-align:center">
								<asp:LinkButton runat="server" ID="lblDisableRss" Text="<%$ Resources: IbnFramework.Admin, RssDisableText %>" Font-Bold="true" Font-Underline="true" ForeColor="Red"></asp:LinkButton>
							</div>
						</blockquote>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>