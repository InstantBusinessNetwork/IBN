<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Events.ParticipantsEditor" CodeBehind="ParticipantsEditor.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="lst" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tTitle") %></title>

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey()
		{
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) {}
		}
		//]]>
	</script>

</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" runat="server">

	<script type="text/javascript">
		//<![CDATA[
		function ChangeButtonState()
		{
			if (document.forms[0].<%=btnAddGroup.ClientID %> != null)
			{
				if (document.forms[0].<%=cbConfirmed.ClientID %>.checked)
					document.forms[0].<%=btnAddGroup.ClientID %>.style.display = 'none';
				else
					document.forms[0].<%=btnAddGroup.ClientID %>.style.display = '';
			}
		}
		//]]>
	</script>

	<ibn:BlockHeader ID="secHeader" Title="ToolBar" runat="server"></ibn:BlockHeader>
	<table class="ibn-propertysheet" cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr height="22">
			<td class="boldtext" width="290" height="22">
				<%=LocRM.GetString("Available") %>:
			</td>
			<td width="4">
				&nbsp;
			</td>
			<td class="ibn-navframe boldtext">
				<%=LocRM.GetString("Selected") %>
				:
			</td>
		</tr>
		<tr>
			<td valign="top" width="290">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td width="9%">
							<%=LocRM.GetString("Group") %>:
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
							<asp:Button ID="btnSearch" runat="server" Width="55px" CssClass="text" CausesValidation="False" OnClick="btnSearch_Click"></asp:Button>
							<asp:Label ID="lblError" CssClass="ibn-error" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td valign="top" height="96">
							<%=LocRM.GetString("User") %>:
						</td>
						<td valign="top">
							<asp:ListBox ID="lbUsers" runat="server" CssClass="text" Width="190px" SelectionMode="Multiple" Rows="6"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td valign="top">
						</td>
						<td valign="top">
							<asp:CheckBox ID="cbConfirmed" runat="server" onclick="ChangeButtonState();"></asp:CheckBox>
						</td>
					</tr>
					<tr>
						<td valign="top">
							&nbsp;
						</td>
						<td>
							<button id="btnSave" runat="server" style="display: none" onserverclick="btnSave_Click">
							</button>
							<button id="btnAdd" runat="server" class="text" style="width: 160px" onserverclick="btnAdd_Click">
							</button>
							<br />
							<br />
							<button id="btnAddGroup" runat="server" style="width: 160px" class="text" onserverclick="btnAddGroup_Click">
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
				<div style='overflow-y: auto; height: <%=Request.Browser.Browser.IndexOf("IE")>=0 ? "295" : "305" %>px'>
					<asp:DataGrid runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="1" GridLines="None" CellSpacing="1" BorderWidth="0px" Width="100%">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText='Name'>
								<ItemTemplate>
									<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "PrincipalId"),(bool)DataBinder.Eval(Container.DataItem, "IsGroup") )%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText='Status' ItemStyle-Width="50" HeaderStyle-Width="50">
								<ItemTemplate>
									<%# GetStatus
										(
										DataBinder.Eval(Container.DataItem, "MustBeConfirmed"),
										DataBinder.Eval(Container.DataItem, "ResponsePending"),
										DataBinder.Eval(Container.DataItem, "IsConfirmed")
										)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-Width="30" Visible="True">
								<ItemTemplate>
									<asp:ImageButton ID="ibDelete" runat="server" BorderWidth="0" Width="16" Height="16" ImageUrl="../layouts/images/DELETE.GIF" CommandName="Delete" CausesValidation="False"></asp:ImageButton>
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
			function FuncSave()
			{
				DisableButtons(document.forms[0].<%=btnSave.ClientID%>);
				<%=Page.ClientScript.GetPostBackEventReference(btnSave,"") %>;
			}
			//]]>
	</script>

	</form>
</body>
</html>
