<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceUtilGraphControl.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.ResourceUtilGraphControl" %>

<script type="text/javascript">
	//<![CDATA[
	var _linkToGraph = "";
	var _imageHeight = 0;
	var _savedParent = null;
	function f_onload(lx, portionWidth, imageHeight) {
		_imageHeight = imageHeight;
		var _obj = document.getElementById('<%=LinkToGraph.ClientID%>');
		_linkToGraph = _obj.value;
		BindDiv('<%=divImg.ClientID%>', _linkToGraph, -1, '', '', lx, portionWidth, imageHeight);
	}
	function ShowLegend() {
		menu = document.getElementById('<%= divLegendMenu.ClientID%>');
		menu.style.display = "";

		_savedParent = menu.parentNode;
		document.body.appendChild(menu);
	}
	function closeLegendMenu() {
		menu = document.getElementById('<%= divLegendMenu.ClientID%>');
		menu.style.display = "none";

		if (_savedParent)
			_savedParent.appendChild(menu);
	}
	//]]>
</script>

<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td valign="top" runat="server" id="UsersCell">
			<asp:DataGrid runat="server" ID="UsersGrid" AllowPaging="False" AllowSorting="False" CellPadding="0" GridLines="None" CellSpacing="0" BorderWidth="0px" AutoGenerateColumns="False" Width="100%">
				<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="UserId" HeaderText="ID" ReadOnly="True" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn HeaderStyle-CssClass="pp" HeaderStyle-Width="105px" ItemStyle-CssClass="pp" ItemStyle-Width="105px">
						<HeaderTemplate>
							<div style="height: <%=Mediachase.IBN.Business.ResourceChart.HeaderHeight-1%>px"><%=LocRM.GetString("tUser")%></div>
						</headertemplate>
						<ItemTemplate>
							<div class="IconAndText" style="white-space: nowrap; height: <%=Mediachase.IBN.Business.ResourceChart.ItemHeight-1%>px">
							<%# UsersAsLinks
								? Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)Eval("UserId"))
								: Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName((int)Eval("UserId"))
							%>
							<%# Eval("Details") %>
							</div>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
		<td valign="top" width="80%">
			<div id="divLegendMenu" runat="server" style="position: absolute; width: 330px; border: solid 1px #333333; display: none; right: 25px; z-index: 10000;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle" onclick="closeLegendMenu()">
				<img alt="" src='<%= ResolveClientUrl("~/Layouts/images/deny_black.gif")%>' width="15" height="15" border="0" align="right" hspace="0" vspace="0" style="cursor: pointer;" class="borderWhite" onclick="closeLegendMenu()" />
				<asp:Table ID="tblLegend" runat="server" CellPadding="4" CellSpacing="0" CssClass="text">
				</asp:Table>
			</div>
			<div id="divImg" onresize="_resizing(this.id, _imageHeight)" runat="server" style="background-color: #e5e3df; overflow: hidden; position: relative; cursor: pointer;">
			</div>
		</td>
	</tr>
</table>
<input id="LinkToGraph" type="hidden" runat="server" />
