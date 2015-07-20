<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomPageEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.CustomPageEdit" %>
<%@ Reference VirtualPath="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="mc" Src="~/Apps/Common/Modules/ResourceEditor.ascx" TagName="ResourceEditor" %>

<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:1px">
	<tr>
		<td colspan="2"><ibn:blockheader id="MainHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td valign="top">
			<table class="text" cellspacing="7" cellpadding="0" border="0" style="table-layout:fixed;">
				<tr>
					<td class="ibn-label" style="width:150px;">
						<asp:Literal runat="server" ID="TitleLiteral" Text="<%$ Resources: IbnFramework.Global, _mc_Title %>"></asp:Literal>:
					</td>
					<td>
						<mc:ResourceEditor runat="server" ID="ctrlTitleText" WebServiceUrl="~/Apps/Common/WebServices/WsResourceEditor.asmx" CheckValueTimeout="1000" CloseTextBoxTimeout="0" Width="200" CssClassLabel="labelClass" CssClassSuccess="successLabel" CssClassFailed="failedLabel"/>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" valign="top">
						<asp:Literal runat="server" ID="DescriptionLiteral" Text="<%$ Resources: IbnFramework.Global, _mc_Description %>"></asp:Literal>:
					</td>
					<td>
						<asp:TextBox runat="server" ID="DescriptionText" Width="480" TextMode="MultiLine" Rows="5"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td class="ibn-label">
						<asp:Literal runat="server" ID="TemplateLiteral" Text="<%$ Resources: IbnFramework.WidgetEngine, PageTemplate %>"></asp:Literal>:
					</td>
					<td>
						<asp:DropDownList runat="server" ID="TemplateList" Width="480"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td style="padding-top:10px;">
						<btn:imbutton class="text" id="SaveButton" style="width:110px;" 
							Runat="server" onserverclick="SaveButton_ServerClick" />&nbsp;&nbsp;
						<btn:imbutton class="text" id="CancelButton" style="width:110px;" 
							Runat="server" IsDecline="true" CausesValidation="false" onserverclick="CancelButton_ServerClick"/>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
