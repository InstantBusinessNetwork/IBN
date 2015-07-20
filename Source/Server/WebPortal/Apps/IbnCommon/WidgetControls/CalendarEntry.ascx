<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Calendar.Modules.CalendarEntry" Codebehind="CalendarEntry.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/Separator1.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="sep" src="~/Modules/Separator1.ascx"%>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2" style="border: none">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" Visible="false" /></td>
	</tr>
	<tr>
		<td width="100%" align="center">
			<div style="margin:10px">
			<table cellpadding="0" cellspacing="0" style="border:1px solid black;">
				<tr>
					<td style="padding:1px; background-color:#DDECFE" align="center">
						<asp:Calendar ID="msCal" runat="server"
							BackColor="#DDECFE" BorderColor="#FFFFFF"
							SelectionMode="Day" ShowDayHeader="true"
							ShowTitle="true" TitleFormat="MonthYear">
							<DayHeaderStyle CssClass="headMSCal" />
							<WeekendDayStyle ForeColor="#CC3300" CssClass="dayMSCal ibn-propertysheet" />
							<TitleStyle BackColor="#DDECFE" CssClass="titleMSCal" />
							<DayStyle ForeColor="#0066CC" CssClass="dayMSCal ibn-propertysheet" />
							<SelectedDayStyle ForeColor="#555555" BackColor="#B8D7F5" BorderColor="#0066CC" BorderWidth="1px" BorderStyle="Solid" Font-Bold="true" CssClass="dayMSCal ibn-propertysheet" />
							<OtherMonthDayStyle ForeColor="#696969" CssClass="dayMSCal ibn-propertysheet" />
						</asp:Calendar>
					</td>
				</tr>
			</table>
			</div>
		</td>
	</tr>
	<tr>
		<td class="ibn-propertysheet">
			<asp:DataList ID="dlWeek" Runat="server" Width="100%" CssClass="text" EnableViewState="False">
				<ItemTemplate>
					<ibn:sep id="sep1" runat="server" title='<%# DataBinder.Eval(Container.DataItem, "DayTitle")%>' IsClickable="false" />
					<div style="padding:3px 0px;">
					<asp:Panel ID="panDay" Runat="server">
						<%# DataBinder.Eval(Container.DataItem, "EventList")%>
					</asp:Panel>
					</div>
				</ItemTemplate>
			</asp:DataList>
		</td>
	</tr>
</table>
<asp:HiddenField runat="server" ID="hdnDate" />
<asp:LinkButton id="lbHide" Runat="server" Visible="False"></asp:LinkButton>
