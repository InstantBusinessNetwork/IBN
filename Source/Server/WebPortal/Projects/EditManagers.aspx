<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.EditManagers" CodeBehind="EditManagers.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM.GetString("managers")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server">
	<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
		<colgroup>
			<col width="120px" />
			<col />
		</colgroup>
		<tr>
			<td width="120px" align="right">
				<b>
					<%= LocRM.GetString("manager")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlManager" runat="server" CssClass="text" Width="200px">
				</asp:DropDownList>
				<asp:Label runat="server" ID="lblManager" CssClass="text"></asp:Label>
			</td>
		</tr>
		<tr>
			<td align="right">
				<b>
					<%= LocRM.GetString("exec_manager")%>:</b>
			</td>
			<td>
				<asp:DropDownList ID="ddlExecManager" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120">
				</btn:IMButton>
				<br />
				<br />
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 120">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
