<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.AddPredecessor" Codebehind="AddPredecessor.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="text" cellspacing="0" cellpadding="0" border="0">
	<tr>
		<td width="110px" style="padding: 5px;"><b><%=LocRM.GetString("SelectGroup") %>:</b></td>
		<td style="padding: 5px;"><asp:dropdownlist id="ddGroups" Width="350px" CssClass="text" Runat="server"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td width="110px" style="padding: 5px;"><b><%=LocRM.GetString("Lag") %>:</b></td>
		<td style="padding: 5px;">
			<table class="text" cellspacing="0" cellpadding="0" border="0">
				<tr>
					<td width="20">
						<%= LocRM.GetString("Hours") %>:</td>
					<td width="50"><asp:textbox id="tbH" Width="35px" CssClass="text" Runat="server" Font-Size="10px" MaxLength="4"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator2" CssClass="text" Runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbH"></asp:requiredfieldvalidator><asp:rangevalidator id="Rangevalidator2" CssClass="text" Runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbH" Type="Integer" MaximumValue="9999" MinimumValue="-9999"></asp:rangevalidator></td>
					<td width="20">
						<%= LocRM.GetString("Minutes") %>:</td>
					<td width="30"><asp:textbox id="tbMin" Width="35px" CssClass="text" Runat="server" Font-Size="10px" MaxLength="2"></asp:textbox><asp:requiredfieldvalidator id="Requiredfieldvalidator1" CssClass="text" Runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbMin"></asp:requiredfieldvalidator><asp:rangevalidator id="Rangevalidator1" CssClass="text" Runat="server" ErrorMessage="*" Display="Dynamic" ControlToValidate="tbMin" Type="Integer" MaximumValue="59" MinimumValue="0"></asp:rangevalidator></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td valign="bottom" align="right" colspan="2" height="40" style="padding: 5px;">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true" CausesValidation="false"></btn:imbutton></td>
	</tr>
</table>