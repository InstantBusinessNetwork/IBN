<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListInfoEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.ListInfoEdit" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>

<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:BlockHeader id="MainBlockHeader" runat="server" Title="<%$Resources : IbnFramework.ListInfo, NewList%>" />
		</td>
	</tr>
	<tr>
		<td>
			<table cellpadding="7" cellspacing="0" width="100%" border="0">
				<tr>
					<td class="ibn-label" style="width:120px;">
						<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Name%>" />:
					</td>
					<td>
						<asp:TextBox Runat="server" ID="ListNameTextBox" Width="99%" MaxLength="100" CssClass="text"></asp:TextBox>
					</td>
					<td style="width:16px;">
						<asp:RequiredFieldValidator id="ListNameRequiredFieldValidator" runat="server" ErrorMessage="*" ControlToValidate="ListNameTextBox" Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" style="width:120px;" valign="top">
						<asp:Literal ID="Literal3" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Description%>" />:
					</td>
					<td valign="top">
						<asp:TextBox Runat="server" ID="txtDescription" Width="99%" TextMode="MultiLine" Rows="4" CssClass="text"></asp:TextBox>
					</td>
					<td></td>
				</tr>
				<tr>
					<td class="ibn-label" style="width:120px;" valign="top">
						<asp:Literal ID="Literal4" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Type%>" />:
					</td>
					<td valign="top">
						<asp:DropDownList ID="ddType" runat="server" Width="99%" CssClass="text"></asp:DropDownList>
					</td>
					<td></td>
				</tr>
				<tr>
					<td class="ibn-label" style="width:120px;" valign="top">
						<asp:Literal ID="Literal5" runat="server" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Status%>" />:
					</td>
					<td valign="top">
						<asp:DropDownList ID="ddStatus" runat="server" Width="99%" CssClass="text"></asp:DropDownList>
					</td>
					<td></td>
				</tr>

				<tr>
					<td></td>
					<td style="padding:10px 10px 10px 0px;">
						<btn:IMButton runat="server" ID="SaveButton" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Save%>" class="text" style="width:110px" OnServerClick="SaveButton_ServerClick"></btn:IMButton>&nbsp;&nbsp;
						<btn:IMButton runat="server" ID="CancelButton" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Cancel%>" CausesValidation="false" class="text" style="width:110px" IsDecline="True"></btn:IMButton>
					</td>
					<td></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
