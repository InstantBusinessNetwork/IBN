<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Directory.SharingEditor" CodeBehind="SharingEditor.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("tTitle")%></title>

	<script type="text/javascript">
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		} 
	</script>

</head>
<body class="UserBackground" id="pT_body" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
	<form id="frmMain" runat="server">
	<ibn:BlockHeader ID="secHeader" Title="ToolBar" runat="server"></ibn:BlockHeader>
	<table class="ibn-propertysheet" style="height: 100%" height="100%" cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr height="22">
			<td width="280" height="22" class="boldtext">
				<%=LocRM.GetString("Available") %>
				:
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td class="ibn-navframe boldtext" height="22">
				<%=LocRM.GetString("Selected") %>
				:
			</td>
		</tr>
		<tr style="height: 100%">
			<td valign="top" width="280">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Group") %>
							:
						</td>
						<td width="91%">
							<lst:IndentedDropDownList ID="ddGroups" runat="server" CssClass="text" Width="190px" AutoPostBack="True" OnSelectedIndexChanged="ddGroups_ChangeGroup">
							</lst:IndentedDropDownList>
						</td>
					</tr>
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Search") %>:
						</td>
						<td width="91%">
							<asp:TextBox ID="tbSearch" runat="server" CssClass="text" Width="125px"></asp:TextBox>
							<asp:Button ID="btnSearch" runat="server" Width="60px" CssClass="text" CausesValidation="False" OnClick="btnSearch_Click"></asp:Button>
						</td>
					</tr>
					<tr>
						<td valign="top">
							<%=LocRM.GetString("User") %>
							:
						</td>
						<td valign="top">
							<asp:ListBox ID="lbUsers" runat="server" CssClass="text" Width="190px" SelectionMode="Multiple" Rows="6"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td valign="top">
						</td>
						<td valign="top">
							<asp:CheckBox ID="cbCanManage" runat="server"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td valign="top" height="28">
							&nbsp;
						</td>
						<td>
							<asp:Button ID="btnAdd" runat="server" CssClass="text" Width="90px" CausesValidation="False" OnClick="btnAdd_Click"></asp:Button>
						</td>
					</tr>
				</table>
				<button id="btnSave" runat="server" text="Button" style="display: none" onserverclick="btnSave_Click">
				</button>
				<!-- End Groups & Users -->
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td valign="top" class="ibn-navframe" height="100%">
				<!-- Data GRID -->
				<div style="overflow-y: auto; height: 300px">
					<asp:DataGrid runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="3" BorderWidth="0px" Width="100%">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText='Name'>
								<ItemTemplate>
									<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "UserId"),false)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText='Name' ItemStyle-Width="50">
								<ItemTemplate>
									<%# GetLevel( (int)DataBinder.Eval(Container.DataItem, "Level"))%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="30" Visible="True">
								<ItemTemplate>
									<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" title="delete" ImageUrl="../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False"></asp:ImageButton>
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
				function FuncSave()
				{
					DisableButtons(document.forms[0].<%=btnSave.ClientID%>);
					<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
				}
	</script>

	</form>
</body>
</html>
