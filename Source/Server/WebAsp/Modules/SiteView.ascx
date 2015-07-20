<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.SiteView" Codebehind="SiteView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\Modules\BlockHeader.ascx" %>
<table class="ibn-stylebox" cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:0px;">
	<tr>
		<td><ibn:blockheader id="secH" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="ibn-propertysheet" cellspacing="4" cellpadding="4" style="PADDING-TOP: 10px">
				<tbody>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("Company")%>:</strong></td>
						<td><asp:Label id="lblCompanyName" Runat="server" Width="300" cssclass="text"></asp:Label></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px" vAlign="top"><strong><%=LocRM.GetString("Type")%>:</strong></td>
						<td colspan="3"><asp:Label id="lblType" Runat="server" Width="300" cssclass="text"></asp:Label></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("Portal")%>:</strong></td>
						<td><asp:HyperLink runat="server" ID="PortalLink" Target="_blank"></asp:HyperLink></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px" vAlign="top"><strong><%=LocRM.GetString("Database")%>:</strong></td>
						<td colspan="3"><asp:Label id="DatabaseLabel" Runat="server" Width="300" cssclass="text"></asp:Label></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px">
							<strong><%=LocRM.GetString("IsActive")%>:</strong>
						</td>
						<td>
							<asp:Label id="lblIsActive" runat="server" cssclass="text" Width="300"></asp:Label>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("ContactName")%>:</strong></td>
						<td><asp:Label id="ContactNameLabel" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("ContactPhone")%>:</strong></td>
						<td><asp:Label id="ContactPhoneLabel" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("ContactEmail")%>:</strong></td>
						<td><asp:Label id="ContactEmailLabel" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("SendSpam")%>:</strong></td>
						<td><asp:Label id="SendSpamLabel" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					
					<tr runat="server" id="TariffDelimiterRow">
						<td colspan="2" style="height:7px;"></td>
					</tr>
					<tr runat="server" id="TariffRow">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("Tariff")%>:</strong></td>
						<td><asp:HyperLink id="TariffValue" Runat="server" cssclass="text"></asp:HyperLink></td>
					</tr>
					<tr runat="server" id="BalanceRow">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("Balance")%>:</strong></td>
						<td><asp:Label id="BalanceValue" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					<tr runat="server" id="AlertThresholdRow">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("AlertThreshold")%>:</strong></td>
						<td><asp:Label id="AlertThresholdValue" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					<tr runat="server" id="CreditLimitRow">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("CreditLimit")%>:</strong></td>
						<td><asp:Label id="CreditLimitValue" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					<tr runat="server" id="DiscountRow">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("Discount")%>:</strong></td>
						<td><asp:Label id="DiscountValue" Runat="server" cssclass="text"></asp:Label> %</td>
					</tr>
					<tr runat="server" id="PaidTillRow">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("PaidTill")%>:</strong></td>
						<td><asp:Label id="PaidTillValue" Runat="server" cssclass="text"></asp:Label></td>
					</tr>
					
					<tr>
						<td colspan="2" style="height:7px;"></td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("DiskSpace")%>:</strong></td>
						<td>
							<asp:Label id="lblOccupiedDiskSpace" Runat="server" cssclass="text"></asp:Label> /
							<asp:Label id="lblMaxDiskSpace" Runat="server" cssclass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("InternalUsers")%>:</strong></td>
						<td>
							<asp:Label id="lblActiveUsers" Runat="server" cssclass="text"></asp:Label> /
							<asp:Label id="lblAllowedUsers" Runat="server" cssclass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("ExternalUsers")%>:</strong></td>
						<td>
							<asp:Label id="lblActiveExternalUsers" Runat="server" cssclass="text"></asp:Label> /
							<asp:Label id="lblAllowedExternalUsers" Runat="server" cssclass="text"></asp:Label>
						</td>
					</tr>
					
					
					<tr id="trStart" runat="server">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("StartDate")%>:</strong></td>
						<td><asp:Label id="lblDateFrom" runat="server" width="97" CssClass="text"></asp:Label></td>
					</tr>
					<tr id="trEnd" runat="server">
						<td style="WIDTH: 176px"><strong><%=LocRM.GetString("TrialEndDate")%>:</strong></td>
						<td><asp:Label id="lblDateTo" runat="server" width="97" CssClass="text"></asp:Label></td>
					</tr>
				</tbody>
			</table>
		</td>
	</tr>
</table>
<asp:LinkButton id="lbDelete" runat="server" Visible="False" onclick="lbDelete_Click">lb</asp:LinkButton>
<script type="text/javascript">
	function DeleteSite()
	{
		if (confirm('<%=LocRM.GetString("Warning") %>' ))
			<%=Page.GetPostBackClientEvent(lbDelete,"") %>
	}
</script>
