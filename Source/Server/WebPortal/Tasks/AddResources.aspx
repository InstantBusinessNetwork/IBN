<%@ Page language="c#" Inherits="Mediachase.UI.Web.Tasks.AddResources" Codebehind="AddResources.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%=LocRM.GetString("tTitle") %></title>
	

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
			<ibn:blockheader id="secHeader" title="ToolBar" runat="server"></ibn:blockheader>
			<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
				<tr height="22">
					<td class="boldtext" width="305" height="22"><%=LocRM.GetString("Available") %>:</td>
					<td width="4">&nbsp;</td>
					<td class="ibn-navframe boldtext"><%=LocRM.GetString("Selected") %>:</td>
				</tr>
				<tr>
					<td valign="top" width="305">
						<!-- Groups & Users -->
						<table class="text" style="MARGIN-TOP: 5px;table-layout:fixed" border="0" cellspacing="0" cellpadding="3" width="100%">
							<tr>
								<td width="100px"><%=LocRM.GetString("Group") %>:</td>
								<td width="200px"><lst:indenteddropdownlist id="ddGroups" runat="server" CssClass="text" Width="190px" AutoPostBack="True" onselectedindexchanged="ddGroups_ChangeGroup"></lst:indenteddropdownlist></td>
							</tr>
							<tr>
								<td><%=LocRM.GetString("Search") %>:</td>
								<td>
									<asp:TextBox id="tbSearch" runat="server" CssClass="text" Width="125px"></asp:TextBox>
									<asp:button id="btnSearch" runat="server" Width="60px" CssClass="text" CausesValidation="False" onclick="btnSearch_Click"></asp:button>
									<asp:Label ID="lblError" CssClass="ibn-error" Runat="server"></asp:Label>
								</td>
							</tr>
							<tr>
								<td valign="top" height="96"><%=LocRM.GetString("User") %>:</td>
								<td valign="top"><asp:listbox id="lbUsers" runat="server" CssClass="text" Width="190px" SelectionMode="Multiple" Rows="6"></asp:listbox></td>
							</tr>
							<tr>
								<td valign="top"></td>
								<td valign="top">
									<asp:CheckBox id="cbCanManage" runat="server"></asp:CheckBox>
								</td>
							</tr>	
							<tr>
								<td valign="top"></td>
								<td valign="top">
									<asp:CheckBox id="cbConfirmed" runat="server"></asp:CheckBox>
								</td>
							</tr>
							<tr>
								<td valign="top">&nbsp;</td>
								<td>
									<Button id="btnSave" runat="server" Text="Button" style="DISPLAY:none" onserverclick="btnSave_Click"></Button><button id="btnAdd" onclick="DisableButtons(this);" runat="server" Class="text" style="Width:160px" onserverclick="btnAdd_Click"></button>
									<button id="btnAddGroup" runat="server" style="Width:160px;visibility:hidden;" class="text" onserverclick="btnAddGroup_Click"></button></td>
							</tr>
						</table>
						<!-- End Groups & Users -->
					</td>
					<td width="4">&nbsp;</td>
					<td valign="top" class="ibn-navframe">
						<!-- Data GRID -->
						<div style='OVERFLOW-Y:auto;HEIGHT:<%=Request.Browser.Browser.IndexOf("IE")>=0 ? "295" : "305" %>px'>
							<asp:DataGrid Runat="server" ID="dgMembers" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="1" gridlines="None" CellSpacing="1" borderwidth="0px">
								<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
								<HeaderStyle CssClass="text"></HeaderStyle>
								<Columns>
									<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
									<asp:TemplateColumn HeaderText='Name' ItemStyle-Width="220" HeaderStyle-Width="220">
										<ItemTemplate>
											<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "UserId"),false )%>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText='Can Manage' ItemStyle-HorizontalAlign=Center ItemStyle-Width="60" HeaderStyle-Width="60">
										<ItemTemplate>
											<input type="checkbox" checked='<%# (bool)DataBinder.Eval(Container.DataItem, "CanManage") %>' class="text" runat="server" id="icCanManage" NAME="icCanManage"/>
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
									<asp:templatecolumn itemstyle-width="30" HeaderStyle-Width="30" Visible="True" ItemStyle-HorizontalAlign="Center">
										<itemtemplate>
											<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
										</itemtemplate>
									</asp:templatecolumn>
								</Columns>
							</asp:DataGrid>
							<!-- End Data GRID --></div>
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
