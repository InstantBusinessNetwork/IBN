<%@ Reference Control="~/Modules/DirectoryTreeView.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.PublishVersion" Codebehind="PublishVersion.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="DirTree" src="~/Modules/DirectoryTreeView.ascx" %>
<table class="ibn-stylebox-light text" style="HEIGHT: 100%;" height="100%" cellspacing="0" cellpadding="2" width="100%" border="0">
	<tr>
		<td valign=top class="text" style="PADDING-RIGHT:45px; PADDING-LEFT:45px; PADDING-BOTTOM:15px; PADDING-TOP:15px" align="center">
			<table cellspacing="0" cellpadding="0" border="0" class="ibn-propertysheet" width="100%">
				<tr>
					<td class="text" valign=top style="padding-top:3" colspan="2">
						<asp:Label ID="lblPublishTo" Runat="server" CssClass="text" Font-Bold="True"></asp:Label>
					</td>
				</tr>
				<tr>
					<td>
						<ibn:DirTree id="ctrlDirTree" runat=server height=330 width=300/>
					</td>
					<td valign="top" width="20px"><asp:Label ID="lblNotValid" Runat=server CssClass="ibn-error" Visible=False>*</asp:Label>	</td>
				</tr>
				<tr>
					<td vAlign="bottom" align="center" height="40">
						<btn:imbutton class="text" id="btnPublish" Runat="server" style="width:110px;"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true"></btn:imbutton>
					</td>
					<td></td>
				</tr>
				<tr runat="server" id="trError" style="padding-top:10px">
					<td>
						<asp:Label Runat="server" ID="lblErrorMessage" CssClass="ibn-error"></asp:Label>
					</td>
					<td width="20px"></td>
				</tr>
			</table>
		</td>
	</tr>
</table>