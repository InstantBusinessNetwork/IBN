<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserCalendar.ascx.cs" Inherits="Mediachase.UI.Web.Directory.Modules.UserCalendar" %>
<%@ register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ register TagPrefix="btn" namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<table width="100%" cellpadding="0" cellspacing="0" class="ibn-propertysheet">
	<tr>
		<td id="firstEnter" runat="server" colspan="2">
			<div style="padding:15px; background-color:#ffffe1;">
			<blockquote style="border-left:solid 2px #CE3431; padding-left:10px; margin:5px; margin-left:15px; padding-top:3px; padding-bottom:3px">
			<%=LocRM.GetString("CreateUserCalendarMessage")%><br />
			<div style="white-space:nowrap; text-align:center; padding-top:5px;">
				<asp:LinkButton ID="lbNewCalendar" Font-Bold="true" Font-Underline="true" ForeColor="Red" runat="server" OnClick="lbNewCalendar_Click"></asp:LinkButton>
			</div>
			</blockquote>
			</div>
		</td>
		<td id="tdCalendar" runat="server" colspan="2" style="padding:5px 5px 0px 5px;">
			<mc:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secCalendar" runat="server" />
			<table id="newCalendar" runat="server" class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="7" width="100%" border="0">
				<colgroup>
					<col width="140px" />
					<col />
					<col />
				</colgroup>
				<tr>
					<td style="width:140px;"><b><%=LocRM.GetString("BaseCalendar")%>:</b></td>
					<td>
						<asp:DropDownList ID="ddCalendar" runat="server" Width="250px"></asp:DropDownList>
						&nbsp;
						<asp:ImageButton ID="ibSave" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/saveitem.gif" Width="16" Height="16" OnClick="ibSave_Click" />
						&nbsp;
						<asp:ImageButton ID="ibCancel" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/layouts/images/cancel.gif" Width="16" Height="16" OnClick="ibCancel_Click" />
					</td>
					<td>&nbsp;</td>
				</tr>
			</table>
			<table id="currentCalendar" runat="server" class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="7" width="100%" border="0">
				<colgroup>
					<col width="140px" />
					<col />
					<col />
				</colgroup>
				<tr>
					<td style="width:140px;font-weight:bold"><%=LocRM.GetString("BaseCalendar")%>:</td>
					<td class="IconAndText">
						<asp:Label ID="lblCalendar" runat="server"></asp:Label>&nbsp;
						<asp:ImageButton ID="ibEdit" runat="server" ImageUrl="~/Layouts/Images/edit.gif" OnClick="ibEdit_Click" />
					</td>
					<td>&nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="trInformation" runat="server">
		<td style="width:50%;padding:5px 5px 5px 5px;" valign="top">
			<mc:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secCalendarDays" runat="server" />
			<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<asp:datagrid id="dgCalendar" ShowHeader="True" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" Runat="server">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="DayOfWeek" Visible="False" />
								<asp:BoundColumn DataField="DayTitle">
									<ItemStyle Width="120px" CssClass="ibn-vb2 ibn-label"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Intervals">
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
		<td align="center" style="padding:5px 5px 5px 5px;" valign="top">
			<mc:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secCalendarExcep" runat="server" />
			<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<asp:datagrid id="dgCalendarEx" ShowHeader="True" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False" Runat="server">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ExceptionId" Visible="False" />
								<asp:BoundColumn DataField="ExceptionInterval">
									<ItemStyle Width="240px" CssClass="ibn-vb2 ibn-label"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Intervals">
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
			<br />
			<mc:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secUserExcep" runat="server" />
			<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr>
					<td>
						<asp:datagrid id="dgUserCalendarEx" ShowHeader="True" Width="100%" borderwidth="0px" 
								CellSpacing="0" gridlines="None" cellpadding="3" AllowSorting="False" AllowPaging="False" 
								AutoGenerateColumns="False" Runat="server" OnDeleteCommand="dgUserCalendarEx_DeleteCommand">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ExceptionId" Visible="False" />
								<asp:BoundColumn DataField="ExceptionInterval">
									<ItemStyle Width="240px" CssClass="ibn-vb2 ibn-label"></ItemStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Intervals">
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn>
									<ItemStyle CssClass="ibn-vb2" Width="70"></ItemStyle>
									<ItemTemplate>
										<a href='<%#Page.ResolveUrl("~/Directory/ExceptionsEditor.aspx")+"?UserCalendarId=" + UserCalendarId + "&amp;ExceptionID=" + DataBinder.Eval(Container.DataItem, "ExceptionId")%>'><img alt="" title="<%=LocRM.GetString("Edit")%>" src="<%=Page.ResolveUrl("~/Layouts/Images/Edit.GIF")%>" /></a>&nbsp;&nbsp;
										<asp:ImageButton id="ibDelete" runat="server" style="vertical-align: middle" ToolTip='<%#LocRM.GetString("Delete")%>' borderwidth="0" width="16" height="16" imageurl="~/layouts/images/DELETE.GIF" causesvalidation="False" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ExceptionId")%>'></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:datagrid>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>