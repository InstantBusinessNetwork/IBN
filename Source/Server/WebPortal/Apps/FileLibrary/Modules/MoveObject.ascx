<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MoveObject.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.FileLibrary.Modules.MoveObject" %>
<%@ Reference Control="~/Modules/DirectoryTreeView.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="DirTree" src="~/Modules/DirectoryTreeView.ascx" %>
<link href='<%=ResolveClientUrl("~/styles/IbnFramework/mcCalendClient.css")%>' rel="stylesheet" type="text/css" />
<table id="mainTable" cellspacing="0" cellpadding="2" width="100%" border="0" style="margin:0px;">
	<tr>
		<td valign="top" class="text" style="PADDING-LEFT:15px; PADDING-TOP:15px">
			<table cellspacing="0" cellpadding="0" border="0" class="ibn-propertysheet">
				<tr>
					<td class="text">
						<b><asp:Label ID="lblMoveTo" Runat="server" CssClass="text"></asp:Label>:&nbsp;</b>
					</td>
				</tr>
				<tr>
					<td>
						<ibn:DirTree id="ctrlDirTree" runat="server" Width="400px" Height="250px" />
						<asp:Label ID="lblNotValid" Runat="server" CssClass="ibn-error" Visible="False">*</asp:Label>	
					</td>
				</tr>
				<tr>
					<td valign="bottom" align="right" height="35px" colspan="2">
						<btn:imbutton class="text" id="btnMove" Runat="server" style="width:125px;"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true"></btn:imbutton></td>
				</tr>
			</table>
		</td>
	</tr>
</table>