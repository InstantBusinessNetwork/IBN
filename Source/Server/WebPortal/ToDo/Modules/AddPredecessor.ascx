<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddPredecessor.ascx.cs" Inherits="Mediachase.UI.Web.ToDo.Modules.AddPredecessor" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="text" cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td width="110px" style="padding: 5px;"><b><%=LocRM.GetString("SelectGroup") %>:</b></td>
		<td style="padding: 5px;"><asp:dropdownlist id="ddGroups" Width="350px" CssClass="text" Runat="server"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td valign="bottom" align="right" colspan="2" height="40" style="padding: 5px;">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" CausesValidation="false"></btn:imbutton></td>
	</tr>
</table>