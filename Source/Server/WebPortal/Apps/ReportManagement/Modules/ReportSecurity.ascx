<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportSecurity.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ReportManagement.Modules.ReportSecurity" %>
<%@ Register TagPrefix="lst" namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="ibn-propertysheet" cellspacing="0" cellpadding="2" width="100%" border="0">
	<tr>
		<td class="boldtext" width="290px" height="22"><%=LocRM.GetString("Available") %> :</td>
		<td class="ibn-navframe boldtext"><%=LocRM.GetString("Selected") %> :</td>
	</tr>
	<tr>
		<td valign="top" width="290px">
			<!-- Groups & Users -->
			<table class="text" style="MARGIN-TOP: 5px" cellspacing="0" cellpadding="2" width="100%">
				<tr>
					<td width="9%"><%=LocRM.GetString("Group") %>:</td>
					<td width="91%"><asp:dropdownlist id="ddGroups" runat="server" Width="190px" CssClass="text" AutoPostBack="True"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td vAlign="top"><%=LocRM.GetString("User") %>:</td>
					<td vAlign="top"><asp:DropDownList id="ddUsers" runat="server" Width="190px" CssClass="text"></asp:DropDownList></td>
				</tr>
				<tr>
					<td vAlign="top"></td>
					<td vAlign="top">
						<asp:radiobuttonlist id="rbList" runat="server" Width="190px" CssClass="text" RepeatColumns="2"></asp:radiobuttonlist>
					</td>
				</tr>
				<tr>
					<td vAlign="top" height="28">&nbsp;</td>
					<td>
						<Button id="btnAdd" runat="server" onclick="DisableButtons(this);" class="text" style="width:90px;" type="button" onserverclick="btnAdd_Click">
						</Button>
					</td>
				</tr>
			</table>
			<!-- End Groups & Users -->
		</td>
		<td valign="top" class="ibn-navframe">
			<!-- Data GRID -->
			<div style="OVERFLOW-Y: auto; HEIGHT:<%=Request.Browser.Browser.IndexOf("IE")>=0 ? "217" : "222" %>px;">
				<asp:datagrid Width="95%" id="dgMembers" Runat="server" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px">
					<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
					<HeaderStyle CssClass="text"></HeaderStyle>
					<Columns>
						<asp:BoundColumn DataField="ID" Visible="False"></asp:BoundColumn>
						<asp:TemplateColumn>
							<ItemTemplate>
								<%# GetLink((int)DataBinder.Eval(Container.DataItem, "Weight"), DataBinder.Eval(Container.DataItem, "Name").ToString())%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:templatecolumn itemstyle-width="45" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
								<asp:Label ID="lblAllow" Runat="server" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "Allow") %>'><img src='<%# ResolveClientUrl("~/layouts/Images/accept.gif") %>' border='0' width='16' height='16' align='absmiddle' alt='' /></asp:Label>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:templatecolumn itemstyle-width="45" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
								<asp:Label ID="lblDeny" Runat="server" Visible='<%# !(bool)DataBinder.Eval(Container.DataItem, "Allow") %>'><img src='<%# ResolveClientUrl("~/layouts/Images/accept.gif") %>' border='0' width='16' height='16' align='absmiddle' alt='' /></asp:Label>
							</itemtemplate>
						</asp:templatecolumn>
						<asp:templatecolumn itemstyle-width="30" Visible="True" ItemStyle-HorizontalAlign="Right">
							<itemtemplate>
								<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" commandname="Delete" causesvalidation="False" imageurl="~/layouts/images/delete.gif">
								</asp:imagebutton>
							</itemtemplate>
						</asp:templatecolumn>
					</Columns>
				</asp:datagrid>
				<!-- End Data GRID --></div>
				<div style="padding-top:5px;padding-left:70px;height:30px;">
					<btn:IMButton class="text" id="SaveButton" Runat="server" style="width:115px;"></btn:IMButton>&nbsp;&nbsp;
					<btn:IMButton class="text" id="CancelButton" Runat="server" style="width:115px;"></btn:IMButton>
				</div>
		</td>
	</tr>
</table>