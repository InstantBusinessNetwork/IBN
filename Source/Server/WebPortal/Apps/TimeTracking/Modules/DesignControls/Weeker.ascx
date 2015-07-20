<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Weeker.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Weeker" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DTCC" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<table cellpadding="0" border="0" class="ibn-propertysheet">
	<tr>
		<td id="cellPrev" runat="server"><div class="IbnCalendarButton" title='<%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Prev}") %>'><asp:ImageButton runat="server" ID="imgPrev" CausesValidation="false" /></div></td>
		<td id="cellNext" runat="server"><div class="IbnCalendarButton" title='<%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Next}") %>'><asp:ImageButton runat="server" ID="imgNext" CausesValidation="false" /></div></td>
		<td><asp:Button runat="server" id="btnCurrentWeek" Text='<%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:CurrentWeek}") %>' CausesValidation="false" /></td>
		<td id='CalendarCell' style="width: 100%; text-align: center;"><div id='CalendarDiv' align="center" style="margin-right: 90px;"><ibn:DTCC AutoPostBack="true" id="dtcWeek" runat="server" SelectedMode="Week" DateCssClass="IbnCalendarText" ShowImageButton="false" DateFormat="d MMM yyyy"></ibn:DTCC></div></td>
	</tr>
</table>