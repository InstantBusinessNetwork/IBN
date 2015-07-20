<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.EditStateInfo" CodeBehind="EditStateInfo.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM2.GetString("state_info")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<br/>
	<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0">
		<tr>
			<td width="110" align="right">
				<b>
					<%= LocRM.GetString("status")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlStatus" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right">
				<b>
					<%= LocRM.GetString("tPrjPhase")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlPhase" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right">
				<b>
					<%= LocRM.GetString("risk_level")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlRiskLevel" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right">
				<b>
					<%= LocRM.GetString("tOverallStatus")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlOverallStatus" runat="server" Width="104px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr id="trPriority" runat="server">
			<td align="right">
				<b>
					<%= LocRM.GetString("Priority")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlPriority" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120">
				</btn:IMButton>
				<br/>
				<br/>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 120">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
