<%@ Reference Page="~/Directory/ChangeZone.aspx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.ChangeZone" Codebehind="ChangeZone.ascx.cs" %>
<table class="ibn-stylebox text" cellspacing="0" cellpadding="0" border="0" width="100%" style="MARGIN-TOP: 0px; margin-left:2px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" title="Department 1" />
		</td>
	</tr>
	<tr>
		<td style="PADDING-RIGHT:15px; PADDING-LEFT:15px; PADDING-BOTTOM:15px; PADDING-TOP:15px">
			<table cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td>
						<asp:Label ID="lblText" Runat="server" CssClass="boldtext"></asp:Label><br><br>
						<asp:DropDownList ID="ddZones" Runat=server></asp:DropDownList>
					</td>
				</tr> 
				<tr>
					<td vAlign="center" align="right" height="60"><btn:imbutton style="width:110px;" class="text" id="btnSave" Runat="server" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" id="btnCancel" style="width:110px;" Runat="server" Text="" IsDecline="true" onserverclick="btnCancel_ServerClick"></btn:imbutton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
