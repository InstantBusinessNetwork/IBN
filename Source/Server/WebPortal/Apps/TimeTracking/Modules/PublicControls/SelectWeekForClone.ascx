<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectWeekForClone.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.SelectWeekForClone" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Picker" src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<table width="100%" border="0" style="padding:5px;">
	<tr>
		<td style="padding:5px;"><asp:Label ID="lblComment" runat="server"></asp:Label></td>
	</tr>
	<tr>
		<td align="center" style="padding:5px">
			<table>
				<tr>
					<td><%=GetGlobalResourceObject("IbnFramework.Global", "SelectWeek").ToString()%>:</td>
					<td><ibn:Picker id="DTCWeek" runat="server" SelectedMode="Week" DateCssClass="IbnCalendarText" ShowImageButton="false" DateFormat="d MMM yyyy"></ibn:Picker></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr id="ButtonsRow">
		<td align="center" style="height:80px;padding:5px;vertical-align:bottom;">
			<asp:Button runat="server" ID="AddButton" Text="<%$Resources: IbnFramework.Global, _mc_Add %>" OnClick="AddButton_Click" />
			<asp:Button runat="server" ID="CloseButton" Text="<%$Resources: IbnFramework.GlobalMetaInfo, Close %>" style="margin-left:20px;" />
		</td>
	</tr>
</table>