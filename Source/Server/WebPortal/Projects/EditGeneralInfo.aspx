<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.EditGeneralInfo" CodeBehind="EditGeneralInfo.aspx.cs" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM2.GetString("general_info")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<table class="text" cellspacing="0" cellpadding="5" width="100%" border="0" style="margin-top: 5">
		<tr>
			<td valign="middle" width="130" align="right">
				<b>
					<%= LocRM.GetString("title")%>:</b>
			</td>
			<td class="ibn-value">
				<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="350px"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rfTitle" runat="server" Display="Dynamic" ErrorMessage="*" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td align="right" valign="top">
				<b>
					<%= LocRM.GetString("description")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtDescription" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Height="80"></asp:TextBox>
			</td>
		</tr>
		<tr runat="server" id="GoalsRow">
			<td align="right" valign="top">
				<b>
					<%= LocRM.GetString("goals")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtGoals" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Height="80"></asp:TextBox>
			</td>
		</tr>
		<tr runat="server" id="ScopeRow">
			<td align="right" valign="top">
				<b>
					<%= LocRM.GetString("scope")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtScope" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Height="80"></asp:TextBox>
			</td>
		</tr>
		<tr runat="server" id="DeliverablesRow">
			<td align="right" valign="top">
				<b>
					<%= LocRM.GetString("deliverables")%>:</b>
			</td>
			<td>
				<asp:TextBox ID="txtDeliverables" runat="server" CssClass="text" Width="350px" TextMode="MultiLine" Height="80"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120">
				</btn:IMButton>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 120; margin-left: 20">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
