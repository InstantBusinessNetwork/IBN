<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="upTemplate.ascx.cs" Inherits="Mediachase.UI.Web.Modules.upTemplate1" %>
<div style="padding:4px; background-image: url(<%=ResolveClientUrl("~/Layouts/images/up_bg.gif") %>); background-repeat: repeat-y; background-color: #B4CAF4">
	<table id="BannerTable" cellspacing="0" cellpadding="0" style="border:0; width:100%">
		<tr>
			<td>
				<a href='<%=ResolveClientUrl("~/Workspace/default.aspx?Btab=Workspace")%>'><img alt="" src='<%=Mediachase.UI.Web.Util.CommonHelper.GetCompanyLogoUrl(Page)%>' /></a>
			</td>
			<td style="white-space:nowrap; width:100%; padding-left: 15px">
				<div><asp:Label ID="lblExpirationDate" runat="server" ForeColor="#ff0000" CssClass="text" Font-Size=".9em"></asp:Label></div>
				<div><span id="onetidPageTitle" class="ibn-pagetitle"></span></div>
			</td>
			<td style="white-space:nowrap; text-align: right; vertical-align: bottom">
				<asp:label runat="server" ID="lblUser" CssClass="ibn-propertysheet"></asp:label><br />
				<span id="timeSpan" class="ibn-propertysheet"></span>
				<asp:Label ID="lblTime" runat="server" CssClass="ibn-propertysheet"></asp:Label>
			</td>
		</tr>
	</table>
</div>

<input type="hidden" id="TimeOffSet" runat="server" value="0"/>
<input type="hidden" id="Is24Hours" runat="server" value="0"/>
<script type="text/javascript">
	//<![CDATA[
	Is24Hours = false;
	TimeOffset = 0;

	TimeOffset = document.getElementById('<%= TimeOffSet.ClientID%>').value;
	if(document.getElementById('<%= Is24Hours.ClientID %>').value=='0')
		Is24Hours = false;
	else
		Is24Hours = true;
		
	if (typeof(showTheTime) != 'undefined')
		showTheTime();
	//]]>
</script>