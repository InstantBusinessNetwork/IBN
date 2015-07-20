<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.GlobalSubscription" Codebehind="GlobalSubscription.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="MARGIN-TOP:0px">
	<tr>
		<td><ibn:blockheader id="secHeader" title="" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td class="ibn-navline ibn-alternating text" style="PADDING-RIGHT:14px;PADDING-LEFT:14px;PADDING-BOTTOM:14px;PADDING-TOP:14px">
			<span class="ibn-label"><%=LocRM.GetString("ObjectType") %>:</span>&nbsp;
			<asp:dropdownlist id="ddType" Width="200px" CssClass="text" Runat="server" AutoPostBack="True"></asp:dropdownlist>
		</td>
	</tr>
	<tr>
		<td class="ibn-navline" style="padding:7">
			<asp:DataGrid Runat="server" ID="grdMain" AllowSorting="False" AllowPaging="False" 
				Width="100%" AutoGenerateColumns="False" BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="text">
				<Columns>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
	<tr>
		<td align="right" height="50" style="PADDING-RIGHT:20"> 	
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" id="btnCancel" Runat="server" style="width:110px;" CausesValidation="false"></btn:imbutton>
		</td>
	</tr>
</table>
