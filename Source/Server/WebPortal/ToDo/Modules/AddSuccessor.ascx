<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddSuccessor.ascx.cs" Inherits="Mediachase.UI.Web.ToDo.Modules.AddSuccessor" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="0" border="0" class="text">
	<tr>
		<td width="110px" style="padding: 5px;"><b><%=LocRM.GetString("SelectGroup") %>:</b></td>
		<td style="padding: 5px;">
			<asp:DropDownList ID="ddGroups" Runat="server" CssClass="text" Width="350px"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td valign="bottom" align="right" height="40" colspan="2" style="padding: 5px;">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true"></btn:imbutton></td>
	</tr>
</table>