<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.AddSuccessor" Codebehind="AddSuccessor.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="0" border="0" class="text">
	<tr>
		<td width="110px" style="padding: 5px;"><b><%=LocRM.GetString("SelectGroup") %>:</b></td>
		<td style="padding: 5px;">
			<asp:DropDownList ID="ddGroups" Runat="server" CssClass="text" Width="350px"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td width="110px" style="padding: 5px;"><b><%=LocRM.GetString("Lag") %>:</b></td>
		<td style="padding: 5px;">
			<table border="0" cellpadding="0" cellspacing="0" class="text">
				<tr>
					<td width="20"><%= LocRM.GetString("Hours") %>:</td>
					<td width="50">
						<asp:TextBox Runat="server" CssClass="text" Width="35" ID="tbH" MaxLength="4" Font-Size="10px"></asp:TextBox><asp:RequiredFieldValidator Runat="server" CssClass="text" ControlToValidate="tbH" Display="Dynamic" ErrorMessage="*" ID="Requiredfieldvalidator2"></asp:RequiredFieldValidator>
						<asp:RangeValidator Runat="server" CssClass="text" ControlToValidate="tbH" Display="Dynamic" ErrorMessage="*" ID="Rangevalidator2" MinimumValue="-9999" MaximumValue="9999" Type="Integer" />
					</td>
					<td width="20"><%= LocRM.GetString("Minutes") %>:</td>
					<td width="30"><asp:TextBox Runat="server" CssClass="text" Width="20" ID="tbMin" MaxLength="2" Font-Size="10px"></asp:TextBox><asp:RequiredFieldValidator Runat="server" CssClass="text" ControlToValidate="tbMin" Display="Dynamic" ErrorMessage="*" ID="Requiredfieldvalidator1"></asp:RequiredFieldValidator><asp:RangeValidator Runat="server" CssClass="text" ControlToValidate="tbMin" Display="Dynamic" ErrorMessage="*" ID="Rangevalidator1" MinimumValue="0" MaximumValue="59" Type="Integer" /></td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td valign="bottom" align="right" height="40" colspan="2" style="padding: 5px;">
			<btn:imbutton class="text" id="btnSave" Runat="server" style="width:110px;" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
			<btn:imbutton class="text" CausesValidation="false" id="btnCancel" Runat="server" style="width:110px;" IsDecline="true"></btn:imbutton></td>
	</tr>
</table>