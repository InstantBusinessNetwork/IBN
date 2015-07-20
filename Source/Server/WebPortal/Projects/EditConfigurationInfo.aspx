<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.EditConfigurationInfo" CodeBehind="EditConfigurationInfo.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM2.GetString("configuration_info")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0" style="margin-top: 10">
		<tr>
			<td valign="middle" align="right">
				<b>
					<%= LocRM.GetString("calendar")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlCalendar" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right" valign="middle">
				<b>
					<%= LocRM.GetString("ProjectCurrency")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlCurrency" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right" valign="middle">
				<b>
					<%= LocRM.GetString("type")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlType" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right" valign="middle">
				<b>
					<%= LocRM.GetString("tInitialPhase")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddInitialPhase" runat="server" CssClass="text" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right" valign="middle">
				<b>
					<asp:Literal runat="server" ID="BlockTypeLiteral" Text="<%$ Resources: IbnFramework.TimeTracking, TimeTrackingBlockType %>"></asp:Literal>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlBlockType" runat="server" Width="99%">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120">
				</btn:IMButton>
				<br/>
				<br/>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 120;">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
