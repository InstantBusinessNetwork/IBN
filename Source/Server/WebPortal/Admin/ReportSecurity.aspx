<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.ReportSecurity" CodeBehind="ReportSecurity.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tSecurity") %></title>
</head>
<body class="UserBackground" id="pT_body">

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
				try {
					if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
						window.event.keyCode = 0;
				}
				catch (e) {}
			}
			function FuncSave() {
				<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>
			}
		//]]>
	</script>

	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<ibn:BlockHeader ID="secHeader" runat="server" />
	<table id="mainTable" class="ibn-stylebox-light text" style="height: 100%;" cellspacing="0" cellpadding="2" width="100%" border="0">
		<tr>
			<td class="boldtext" width="290" height="22">
				<%=LocRM.GetString("Available") %>
				:
			</td>
			<td class="ibn-navframe boldtext">
				<%=LocRM.GetString("Selected") %>
				:
			</td>
		</tr>
		<tr style="height: 100%">
			<td valign="top" width="290">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Group") %>:
						</td>
						<td width="91%">
							<asp:DropDownList ID="ddGroups" runat="server" Width="190px" CssClass="text" AutoPostBack="True">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td valign="top">
							<%=LocRM.GetString("User") %>:
						</td>
						<td valign="top">
							<asp:DropDownList ID="ddUsers" runat="server" Width="190px" CssClass="text">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td valign="top">
						</td>
						<td valign="top">
							<asp:RadioButtonList ID="rbList" runat="server" Width="190px" CssClass="text" RepeatColumns="2">
							</asp:RadioButtonList>
						</td>
					</tr>
					<tr>
						<td valign="top" height="28">
							&nbsp;
						</td>
						<td>
							<button id="btnAdd" runat="server" onclick="DisableButtons(this);" class="text" style="width: 90px;" type="button" onserverclick="btnAdd_Click">
							</button>
						</td>
					</tr>
				</table>
				<!-- End Groups & Users -->
			</td>
			<td valign="top" height="100%" class="ibn-navframe">
				<!-- Data GRID -->
				<div style="overflow-y: auto; height: 262px">
					<asp:DataGrid Width="95%" ID="dgMembers" runat="server" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="3" BorderWidth="0px">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="ID" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn>
								<ItemTemplate>
									<span class="IconAndText">
										<%# GetLink((int)DataBinder.Eval(Container.DataItem, "Weight"), DataBinder.Eval(Container.DataItem, "Name").ToString())%></span>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="45" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
								<ItemTemplate>
									<asp:Label ID="lblAllow" runat="server" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "Allow") %>'><img alt='' src='<%# ResolveClientUrl("~/layouts/Images/accept.gif") %>'/></asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="45" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
								<ItemTemplate>
									<asp:Label ID="lblDeny" runat="server" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem, "Allow") %>'><img alt='' src='<%# ResolveClientUrl("~/layouts/Images/accept.gif") %>'/></asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="30" Visible="True" ItemStyle-HorizontalAlign="Right">
								<ItemTemplate>
									<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" CommandName="Delete" CausesValidation="False" ImageUrl="~/layouts/images/DELETE.GIF"></asp:ImageButton>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
					<!-- End Data GRID -->
				</div>
			</td>
		</tr>
	</table>
	<asp:LinkButton ID="btnSave" runat="server" Visible="False"></asp:LinkButton>
	</form>
</body>
</html>
