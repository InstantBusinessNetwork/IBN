<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSecurity.aspx.cs" Inherits="Mediachase.UI.Web.Projects.EditSecurity" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM2.GetString("SecuritySettings")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server">
	<table cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td style="width: 33%;">
				<table class="text" cellspacing="0" cellpadding="4" width="100%" border="0" style="margin: 5px;">
					<tr>
						<td class="ibn-label">
							<%= LocRM2.GetString("ToDoCanCreate")%>:
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="ToDoManagerCheckbox" Checked="true" Enabled="false" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="ToDoExecutiveCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="ToDoTeamCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="ToDoSponsorCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="ToDoStakeholderCheckbox" />
						</td>
					</tr>
				</table>
			</td>
			<td style="width: 34%;">
				<table class="text" cellspacing="0" cellpadding="4" width="100%" border="0" style="margin: 5px;">
					<tr>
						<td class="ibn-label">
							<%= LocRM2.GetString("DocumentCanCreate")%>:
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="DocumentManagerCheckbox" Checked="true" Enabled="false" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="DocumentExecutiveCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="DocumentTeamCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="DocumentSponsorCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="DocumentStakeholderCheckbox" />
						</td>
					</tr>
				</table>
			</td>
			<td style="width: 33%;">
				<table class="text" cellspacing="0" cellpadding="4" width="100%" border="0" style="margin: 5px;">
					<tr>
						<td class="ibn-label">
							<%= LocRM2.GetString("TaskCanCreate")%>:
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="TaskManagerCheckbox" Checked="true" Enabled="false" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="TaskExecutiveCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="TaskTeamCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="TaskSponsorCheckbox" />
						</td>
					</tr>
					<tr>
						<td style="padding-left: 20px;">
							<asp:CheckBox runat="server" ID="TaskStakeholderCheckbox" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="3" align="center" class="text" style="padding-top:25px; padding-bottom:25px;">
				<asp:CheckBox runat="server" ID="HideManagementCheckbox"></asp:CheckBox>
			</td>
		</tr>
		<tr>
			<td align="center" colspan="3">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width:120px;" OnServerClick="btnSave_ServerClick">
				</btn:IMButton>
				&nbsp;
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width:120px;">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
