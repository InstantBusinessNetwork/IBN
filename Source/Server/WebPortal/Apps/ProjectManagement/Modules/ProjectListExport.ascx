<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectListExport.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.ProjectListExport" %>
<style type="text/css">
	.pad5 tbody tr td
	{
		padding: 5px;
	}
</style>
<table width="100%" border="0">
	<tr>
		<td style="padding:5px;" align="center">
			<table>
				<tr><td align="left">
					<asp:RadioButtonList ID="rbList" runat="server" CellPadding="5" CellSpacing="5" 
						CssClass="text pad5" RepeatDirection="Vertical">
					</asp:RadioButtonList>					
				</td></tr>
			</table>
		</td>
	</tr>
	<tr>
		<td align="center" style="padding:20px;">
			<asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Width="105px" />
		</td>
	</tr>
</table>