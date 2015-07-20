<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.UnchangebleRoles" Codebehind="UnchangebleRoles.ascx.cs" %>
<table height="100%" width="100%" border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td align="center" valign="middle">
			<table width="95%" border="0" cellpadding="0" class="text">
				<tr>
					<td width=10>&nbsp;
					</td>
					<td valign=center width=50>
						<img src="../layouts/images/DeleteUser.gif">
					</td>
					<td>
						<%=LocRM.GetString("UnchangebleRolesDialog") %><br><br>
						<table cellpadding="0" cellspacing="3" class="text">
							<tr>
								<td><%=LocRM.GetString("Replace") %>: </td>
								<td><asp:DropDownList ID="ddReplace" Runat="server" Width="180"></asp:DropDownList></td>
							</tr>
							<tr>
								<td></td>
								<td><asp:CheckBox ID="cbManager" Runat="server"></asp:CheckBox></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr style="height:50px;">
		<td align="center" valign="middle">
			<Button id="btnReplace" onclick="DisableButtons();" class="text" Runat="server" onserverclick="btnReplace_Click" style="margin-right:10px; width:140px;"></Button>
			<Button id="btnDelFromGroup" onclick="DisableButtons(this);" class="text" Runat="server" onserverclick="btnDelFromGroup_Click" style="margin-right:10px; width:130px;"></Button>
			<Button id=btnDeactivate onclick="DisableButtons();" class="text" Runat="server"  onserverclick="btnDeactivate_Click" style="margin-right:10px; width:130px;"></Button>
			<button onclick="window.close();return false;" class="text" type="button" style="width:130px;"><%=LocRM.GetString("Cancel")%></button>
		</td>
	</tr>
</table>
<script language=javascript>
	function ToChangeRoles() 
	{
		try { 
		  window.opener.location.href='../Directory/ChangeRoles.aspx?UserID=<%=Request["UserID"] %>'; 
		} 
		catch (e) {} 
		window.close(); 
	} 
</script>
