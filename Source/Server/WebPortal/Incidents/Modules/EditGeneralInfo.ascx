<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.EditGeneralInfo"
	CodeBehind="EditGeneralInfo.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0" style="margin-top: 5">
	<tr>
		<td width="130" align="right" valign="top">
			<b>
				<%=LocRM.GetString("Title")%>:</b>
		</td>
		<td class="ibn-value">
			<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="300px"></asp:TextBox><asp:RequiredFieldValidator
				ID="rfText" runat="server" ControlToValidate="txtTitle" ErrorMessage="*"></asp:RequiredFieldValidator>
		</td>
	</tr>
	<tr id="trPriority" runat="server">
		<td align="right" valign="top">
			<b>
				<%=LocRM.GetString("Priority")%>:</b>
		</td>
		<td>
			<asp:DropDownList ID="ddlPriority" runat="server" Width="200px" DataTextField="PriorityName"
				DataValueField="PriorityId">
			</asp:DropDownList>
		</td>
	</tr>
	<tr id="trType" runat="server">
		<td align="right" valign="top">
			<b>
				<%=LocRM.GetString("Type")%>:</b>
		</td>
		<td>
			<asp:DropDownList ID="ddlType" runat="server" Width="200px" DataTextField="TypeName"
				DataValueField="TypeId">
			</asp:DropDownList>
		</td>
	</tr>
	<tr id="trSeverity" runat="server">
		<td valign="top" align="right">
			<b>
				<%=LocRM.GetString("Severity")%>:</b>
		</td>
		<td>
			<asp:DropDownList ID="ddlSeverity" runat="server" Width="200px" DataTextField="SeverityName"
				DataValueField="SeverityId">
			</asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td valign="top" align="right">
			<b>
				<%=LocRM.GetString("Description")%>:</b>
		</td>
		<td>
			<asp:TextBox runat="server" ID="txtDescription" Width="300" Height="200" CssClass="text"
				TextMode="MultiLine"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center">
			<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120" OnServerClick="btnSave_ServerClick">
			</btn:IMButton>
			<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false"
				style="width: 120; margin-left: 20">
			</btn:IMButton>
		</td>
	</tr>
</table>
