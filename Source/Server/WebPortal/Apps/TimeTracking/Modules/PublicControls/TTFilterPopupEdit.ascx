<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TTFilterPopupEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.TTFilterPopupEdit" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DTCC" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Weeker" src="~/Apps/TimeTracking/Modules/DesignControls/Weeker.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%--<asp:UpdatePanel runat="server" ID="FilterPopupPanel" ChildrenAsTriggers="false" UpdateMode="Conditional">
<ContentTemplate>--%>
<div class="bottomBorder" runat="server" id="FiltersDiv">
	<table cellpadding="0" cellspacing="0" border="0" width="100%" class="tablePadding3">
		<tr runat="server" id="trWeekPeriod">
			<td id="tdWeek" runat="server">
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td style="width:110px;">
							<b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_TimePeriod}")%>:</b>
						</td>
						<td>
							<ibn:DTCC id="DTCWeek" runat="server" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" AutoPostBack="true"></ibn:DTCC>
						</td>
					</tr>
				</table>
			</td>
			<td id="tdPeriod" runat="server">
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td style="width:110px;">
							<b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_TimePeriod}")%>:</b>
						</td>
						<td>
							<asp:DropDownList ID="ddPeriod" runat="server" Width="170px"></asp:DropDownList>
						</td>
						<td style="padding-left:25px;" id="divBeg" runat="server"><ibn:DTCC id="DTCBeg" runat="server" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" AutoPostBack="true"></ibn:DTCC></td>
						<td style="padding-left:15px;" id="divEnd" runat="server"><div style="float:left;padding-right:15px;">-</div><ibn:DTCC id="DTCEnd" runat="server" SelectedMode="Week" ShowImageButton="false" DateCssClass="IbnCalendarText" ReadOnly="true" DateFormat="dd MMM yyyy" AutoPostBack="true"></ibn:DTCC></td>
					</tr>
				</table>
			</td>
			<td id="tdEmpty" runat="server"></td>
		</tr>
		<tr style="height: 25px;">
			<td id="tdUser" runat="server" style="padding-top:3px;" >
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td style="padding-right:5px;">
							<%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_User}")%>:
						</td>
						<td>
							<asp:DropDownList ID="ddUser" runat="server"></asp:DropDownList>
						</td>
					</tr>
				</table>
			</td>
			<td id="tdProject" runat="server" style="padding-top:3px;">
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td style="padding-right:5px;" nowrap="nowrap">
							<%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:Project}")%>:
						</td>
						<td>
							<lst:IndentedDropDownList ID="ttBlock" runat="server" Width="400"></lst:IndentedDropDownList>
						</td>
					</tr>
				</table>
			</td>
			<td id="tdProjectTitle" runat="server" style="padding-top:3px;">
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td style="padding-right:5px;" nowrap="nowrap">
							<b><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:Project}")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblProjTitle" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
			<td id="tdState" runat="server" style="padding-top:3px;">
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td style="padding-right:5px;"><%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_State}")%>:</td>
						<td>
							<asp:DropDownList runat="server" ID="ddState"></asp:DropDownList>
						</td>
					</tr>
				</table>
			</td>
			<td align="right" runat="server" id="tdButtons" style="padding-top:3px;">
				<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Width="80px" />&nbsp;&nbsp;
				<asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" Width="80px" />	
			</td>
		</tr>
	</table>
</div>
<div style="padding-left:5px; padding-right:5px;" runat="server" id="WeekerDiv">
	<table cellpadding="0" cellspacing="0" border="0" width="100%">	
		<tr style="height: 25px;">
			<td style="width: 100%;" colspan="10">
				<ibn:Weeker runat="server" ID="DTCWeeker" ShowWeekNumber="True" />
			</td>
		</tr>
	</table>
</div>
<%--</ContentTemplate>
</asp:UpdatePanel>--%>