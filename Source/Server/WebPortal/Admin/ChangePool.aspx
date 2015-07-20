<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.ChangePool" CodeBehind="ChangePool.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tPoolTitle") %></title>

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		}
		//]]>
	</script>

</head>
<body class="UserBackground">
	<form id="frmMain" runat="server">
	<ibn:BlockHeader ID="secHeader" Title="&nbsp;" runat="server"></ibn:BlockHeader>
	<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr>
			<td class="boldtext" width="305" height="22">
				<%=LocRM.GetString("Available") %>:
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td class="ibn-navframe boldtext">
				<%=LocRM.GetString("Selected") %>:
			</td>
		</tr>
		<tr>
			<td valign="top" width="305">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px; table-layout: fixed" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td width="100px">
							<%=LocRM.GetString("Group") %>:
						</td>
						<td width="200px">
							<lst:IndentedDropDownList ID="ddGroups" runat="server" CssClass="text" Width="190px" AutoPostBack="True" DataTextField="GroupName" DataValueField="GroupId" OnSelectedIndexChanged="ddGroups_ChangeGroup">
							</lst:IndentedDropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<%=LocRM.GetString("Search") %>:
						</td>
						<td>
							<asp:TextBox ID="tbSearch" runat="server" CssClass="text" Width="125px"></asp:TextBox>
							<asp:Button ID="btnSearch" runat="server" Width="60px" CssClass="text" CausesValidation="False" OnClick="btnSearch_Click"></asp:Button>
							<asp:Label ID="lblError" CssClass="ibn-error" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td valign="top" height="96">
							<%=LocRM.GetString("tUser") %>:
						</td>
						<td valign="top">
							<asp:ListBox ID="lbUsers" runat="server" CssClass="text" Width="190px" SelectionMode="Multiple" Rows="6"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td valign="top">
							&nbsp;
						</td>
						<td>
							<button id="btnSave" runat="server" style="display: none" onserverclick="btnSave_Click">
							</button>
							<button id="btnAdd" onclick="DisableButtons(this);" runat="server" class="text" style="width: 160px;" onserverclick="btnAdd_Click">
							</button>
						</td>
					</tr>
				</table>
				<!-- End Groups & Users -->
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td valign="top" class="ibn-navframe">
				<!-- Data GRID -->
				<div style='overflow-y: auto; height: <%=Request.Browser.Browser.IndexOf("IE")>=0 ? "215" : "225" %>px'>
					<asp:DataGrid runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="1" GridLines="None" CellSpacing="1" BorderWidth="0px">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn ItemStyle-Width="220" HeaderStyle-Width="220">
								<ItemTemplate>
									<%# GetLink((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="30" HeaderStyle-Width="30" Visible="True" ItemStyle-HorizontalAlign="Center">
								<ItemTemplate>
									<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" ImageUrl="~/layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False"></asp:ImageButton>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
					<!-- End Data GRID -->
				</div>
			</td>
		</tr>
	</table>

	<script type="text/javascript">
		//<![CDATA[
		function FuncSave() {
			DisableButtons(document.forms[0].<%=btnSave.ClientID%>);
			<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
		}
		//]]>
	</script>

	</form>
</body>
</html>
