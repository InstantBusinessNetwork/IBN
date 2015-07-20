<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.AddFinanceActual" Codebehind="AddFinanceActual.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="7" width="100%" style="table-layout:fixed;">
	<tr class="text">
		<td align="right" width="100px" class="ibn-label"><%= LocRM.GetString("Date") %>:</td>
		<td align="left"><mc:Picker ID="dtcDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
	</tr>
	<tr class="text">
		<td align="right" width="100px" class="ibn-label"><%= LocRM.GetString("tAccount") %>:</td>
		<td><asp:DropDownList Runat="server" ID="ddAccounts"></asp:DropDownList></td>
	</tr>
	<tr class="text">
		<td align="right" width="100px" class="ibn-label"><%= LocRM.GetString("tSum") %>:</td>
		<td>
			<asp:TextBox Runat="server" ID="tbValue" ></asp:TextBox>
			<asp:RegularExpressionValidator Runat="server" ErrorMessage="*" ControlToValidate="tbValue" ValidationExpression="[-+]?\b((?:[0-9]*\.)|(?:[0-9]*\,))?[0-9]+(?:[eE][-+]?[0-9]+)?\b"></asp:RegularExpressionValidator>
			<asp:RequiredFieldValidator runat="server" ErrorMessage="*" ControlToValidate="tbValue"></asp:RequiredFieldValidator>
		</td>
	</tr>	
	<tr>
		<td align="right" width="100px" valign="top" class="ibn-label"><%=LocRM.GetString("Description")%>:</td>
		<td align="left"><asp:TextBox ID="txtDescription" Runat="server" CssClass="text" TextMode="MultiLine" Width="95%" Rows="5"></asp:TextBox></td>
	</tr>	
	<tr><td colspan="2" style="padding-left:100px;">
		<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;"></btn:imbutton>&nbsp;&nbsp;
		<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" IsDecline="true" style="width:110px;"></btn:imbutton>
	</td></tr>		
</table>
