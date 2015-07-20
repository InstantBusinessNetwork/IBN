<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.CustomSQLReport" Codebehind="CustomSQLReport.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="padding:8px">
			<table class="text" cellspacing="0" cellpadding="0" width="600px" border="0">
				<tr>
					<td valign="top">
						<asp:TextBox CssClass="text" TextMode="MultiLine" ID="txtSQLRequest" Runat="server" Height="255px" Width="600px"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td align="right" style="padding-top:10px;">
						<btn:IMButton class="text" style="WIDTH: 110px" id="btnProcess" Runat="server" Text="Process"></btn:IMButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>

