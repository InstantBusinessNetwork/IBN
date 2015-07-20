<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.Modules.ArticleView" Codebehind="ArticleView.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox text" style="MARGIN-TOP: 0px; margin-left:2px" cellspacing="0" cellpadding="0" width="100%" border=0>
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td style="PADDING: 15px" class="ibn-light ibn-navline">
			<asp:Label runat="server" ID="lblQuestion" CssClass="boldText"></asp:Label>
		</td>
	</tr>
	<tr>
		<td style="PADDING: 15px" class="ibn-navline">
			<asp:Label runat="server" ID="lblAnswer"></asp:Label>
		</td>
	</tr>
	<tr>
		<td style="PADDING: 10px" class="ibn-light">
			<table cellpadding="3" cellspacing="0" width="100%" border="0" class="ibn-propertysheet">
				<tr runat="server" id="trFiles">
					<td class="ibn-label" width="80px" align="right" style="padding-right:10px;" valign="top">
						<%= LocRM.GetString("atclFiles") %>:
					</td>
					<td class="ibn-value">
						<asp:Repeater runat="server" ID="rptFiles" OnItemCommand="rptFiles_ItemCommand">
							<ItemTemplate>
								<%# Eval("FileLink")%> <asp:ImageButton ID="ibDelete" Runat=server CommandName="Delete" ImageUrl="~/layouts/Images/Delete.gif" CommandArgument='<%# Eval("FileId")%>' ImageAlign="AbsMiddle"></asp:ImageButton><br />
							</ItemTemplate>
						</asp:Repeater>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" width="80px" align="right" style="padding-right:10px;">
						<%= LocRM.GetString("atclTags") %>:
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="lblTags"></asp:Label>
					</td>
				</tr>
				<tr>
					<td class="ibn-label" width="80px" align="right" style="padding-right:10px;">
						<%= LocRM.GetString("atclCreated")%>:
					</td>
					<td class="ibn-value">
						<asp:Label runat="server" ID="lblCreated"></asp:Label>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
