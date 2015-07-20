<%@ Control Language="c#" Inherits="Mediachase.Ibn.WebAsp.Modules.LoginUser" CodeBehind="LoginUser.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="..\Modules\BlockHeader.ascx" %>

<script type="text/javascript">
	//<![CDATA[
	function CreateTicket(principalId) {
		var req = window.XMLHttpRequest ?
		new XMLHttpRequest() :
		new ActiveXObject("Microsoft.XMLHTTP");
		req.onreadystatechange = function() {
			if (req.readyState != 4) return;
			if (req.readyState == 4) {
				if (req.status == 200) {
					var query = req.responseText;
					if (req.responseText != "")
						window.open(query, "_blank");
				}
				else {
					alert("Error: " + req.status + ". " + req.statusText);
				}
			}
		}

		var companyList = document.getElementById('<%= ddCompany.ClientID%>');
		if (companyList) {
			var companyUid = companyList.value;
			req.open("GET", 'CreateTicket.aspx?companyUid=' + companyUid + "&principalId=" + principalId, true);
			req.send(null);
		}
	}
	//]]>
</script>

<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox ibn-propertysheet">
	<tr>
		<td>
			<ibn:BlockHeader ID="secH" runat="server" Title="Login on behalf of User" />
		</td>
	</tr>
	<tr>
		<td class="ibn-navline" style="padding: 5px; background-color: #f0f0f0">
			Select Company:&nbsp;
			<asp:DropDownList ID="ddCompany" runat="server" CssClass="text">
			</asp:DropDownList>
			<asp:Button ID="btnApply" Text="Apply" CssClass="text" runat="server" OnClick="btnApply_Click"></asp:Button>
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid AllowSorting="True" Style="margin-top: 10px; margin-left: 5px; margin-right: 5px" ID="dgCompanyUser" Width="98%" AutoGenerateColumns="False" BorderWidth="0px" runat="server" GridLines="Horizontal" CellPadding="1" AllowPaging="True" PageSize="20" CssClass="text">
				<PagerStyle HorizontalAlign="Right" Mode="NumericPages"></PagerStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="User Name">
						<HeaderStyle Wrap="False" CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "LastName") + " " +
								DataBinder.Eval(Container.DataItem, "FirstName")%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:HyperLinkColumn DataNavigateUrlField="Email" DataNavigateUrlFormatString="mailto:{0}" DataTextField="Email" HeaderText="Email">
						<HeaderStyle Wrap="False" CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					</asp:HyperLinkColumn>
					<asp:TemplateColumn>
						<HeaderStyle Wrap="False" CssClass="ibn-vh2" Width="50"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="50"></ItemStyle>
						<ItemTemplate>
							<a href='javascript:CreateTicket(<%# Eval("PrincipalId") %>)'>Login</a>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
