<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegisterFinances.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.RegisterFinances" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<table cellpadding="5" width="100%" height="100%" border="0">
	<tr>
		<td>
			<table cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td class="ibn-label">
						<%= LocRM.GetString("Date") %>: &nbsp;
					</td>
					<td>
						<mc:Picker ID="dtcDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td class="ibn-label" align="left" style="padding-top:10px;">
			<%=LocRM.GetString("tSelAccount")%>:
		</td>
	</tr>
	<tr valign="top">
		<td align="center">
			<asp:DropDownList ID="ddAccounts" Runat="server" Width="98%"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td align="center">
			<br /><br />
			<btn:ImButton class="text" id="btnApprove" Runat="server" style="width:110px;" onserverclick="btnApprove_click" />
			<br /><br />
			<btn:ImButton class="text" id="btnCancel" Runat="server" style="width:110px;" onclick="javascript:window.close();" />
		</td>
	</tr>
</table>
