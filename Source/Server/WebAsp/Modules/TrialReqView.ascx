<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.TrialReqView" Codebehind="TrialReqView.ascx.cs" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secH" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table style="MARGIN-TOP: 10px; MARGIN-LEFT: 20px" width="100%" >
				<tr>
					<td width="55%">
						<table style="table-layout:fixed" cellspacing="4" cellpadding="4" width="100%" class="text">
							<tr>
								<td width="140"><strong><asp:label id="lblCompName" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtCompName" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td vAlign="center" width="140"><strong><asp:label id="lblDescr" Runat="server" CssClass="text"></asp:label></strong></td>
								<td colSpan="3"><asp:label id="txtDescr" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblDomain" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtDomain" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblContactName" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtContactName" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblEMail" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtEMail" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblPhone" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtPhone" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblLogin" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtLogin" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblPassword" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtPassword" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td width="140"><strong><asp:label id="lblCountry" Runat="server" CssClass="text"></asp:label></strong></td>
								<td><asp:label id="txtCountry" Runat="server" CssClass="text"></asp:label></td>
							</tr>
							<tr>
								<td height="5"><STRONG>Default Language:</STRONG></td>
								<td>
									<asp:Label id=lblDefaultLanguage runat="server"></asp:Label></td>
							</tr>
						</table>
					</td>
					<td width="45%" valign="top">
						<div align="left" class="text" id="divXML" runat="server"><%=sXML%></div>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<asp:linkbutton id="lbDelete" runat="server" Visible="False" onclick="lbDelete_Click">lb</asp:linkbutton>
<script language="javascript">
	function DeleteRequest()
	{
		if (confirm('<%=LocRM.GetString("Warning") %>' ))
			<%=Page.GetPostBackClientEvent(lbDelete,"") %>
	}
</script>
