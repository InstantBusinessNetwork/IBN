<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.SponsorsEditor" CodeBehind="SponsorsEditor.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tTitle")%></title>

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
<body class="UserBackground" id="pT_body">
	<form id="frmMain" runat="server">
	<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr height="22">
			<td class="boldtext" width="280px" height="22">
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
			<td valign="top" width="280px">
				<!-- Groups & Users -->
				<table class="text" style="margin-top: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td>
							<%=LocRM.GetString("Group") %>:
						</td>
						<td>
							<btn:IndentedDropDownList ID="ddGroups" runat="server" CssClass="text" Width="190px" AutoPostBack="True" OnSelectedIndexChanged="ddGroups_ChangeGroup">
							</btn:IndentedDropDownList>
						</td>
					</tr>
					<tr>
						<td valign="top">
							<%=LocRM.GetString("Search") %>:
						</td>
						<td valign="top">
							<asp:TextBox ID="tbSearch" runat="server" CssClass="text" Width="125px"></asp:TextBox><asp:Button ID="btnSearch" runat="server" Width="60px" CssClass="text" CausesValidation="False" OnClick="btnSearch_Click"></asp:Button><asp:Label ID="lblError" CssClass="ibn-error" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td valign="top" height="96">
							<%=LocRM.GetString("User") %>:
						</td>
						<td valign="top">
							<asp:ListBox ID="lbUsers" runat="server" CssClass="text" Width="190px" SelectionMode="Multiple" Rows="10"></asp:ListBox>
						</td>
					</tr>
					<tr>
						<td valign="top" height="28">
							&nbsp;
						</td>
						<td>
							<button id="btnSave" runat="server" text="Button" style="display: none" onserverclick="btnSave_Click">
							</button>
							<button id="btnAdd" runat="server" style="width: 160px" class="text" onserverclick="btnAdd_Click">
							</button>
							<br />
							<br />
							<button id="btnAddGroup" style="width: 160px" runat="server" class="text" onserverclick="btnAddGroup_Click">
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
				<div style='overflow-y: auto; height: <%=Request.Browser.Browser.IndexOf("IE")>=0 ? "284" : "296" %>px'>
					<asp:DataGrid runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="3" BorderWidth="0px" Width="100%">
						<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="PrincipalId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText='Name'>
								<ItemTemplate>
									<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "PrincipalId"),(bool)DataBinder.Eval(Container.DataItem, "IsGroup") )%>
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
		<tr>
			<td style="height: 30px;" colspan="2">
			</td>
			<td style="height: 30px;" align="center" class="ibn-navframe">
				<btn:IMButton class="text" ID="SaveButton" runat="server" style="width: 115px;">
				</btn:IMButton>
				&nbsp;&nbsp;
				<btn:IMButton class="text" CausesValidation="false" ID="CancelButton" runat="server" Text="" IsDecline="true" style="width: 115px;">
				</btn:IMButton>
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
		function FuncCancel(ProjectId)
		{
			var obj = window.parent.document.getElementById('<%=BtnID%>');
			if(obj!=null)
				obj.style.display = "none";
			else if(parent == window)
				window.close();
			else
				window.top.frames['right'].location.href='ProjectView.aspx?ProjectId='+ProjectId;
		}
		//]]>
	</script>

	</form>
</body>
</html>
