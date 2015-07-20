<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.ChangebleRoles" Codebehind="ChangebleRoles.ascx.cs" %>
<table height="100%" width="100%" border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td align="center" valign="middle">
			<table width="95%" border="0" cellpadding="0" cellpadding="5" class="text">
				<tr>
					<td style="width:10px;">&nbsp;</td>
					<td valign="middle" style="width:50px;">
						<img src="../layouts/images/DeleteUser.gif" alt="" />
					</td>
					<td>
						<%=LocRM.GetString("ChangableRolesDialog") %><br />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="50px">
		<td align="center" valign="middle">
			<button onclick="DisableButtons(this);ToChangeRoles()" class="text" type"button"><%=LocRM.GetString("Show")%></button>&nbsp;
			<button id="btnDelFromGroup" onclick="DisableButtons(this);" runat="server" class="text" onserverclick="btnDelFromGroup_Click"></button>&nbsp;&nbsp;
			<button runat="server" id="btnDeactivate" class="text" onserverclick="btnDeactivate_Click"></button>&nbsp;&nbsp;
			<button onclick="DisableButtons(this);window.close()" class=text type=button>
			<%=LocRM.GetString("Cancel")%></button>
		</td>
	</tr>
</table>
<script language="javascript" type="text/javascript">
	function ToChangeRoles() 
	{
		try { 
		  window.opener.location.href='../Directory/ChangeRoles.aspx?UserID=<%=Request["UserID"] %>'; 
		} 
		catch (e) {} 
		window.close(); 
	} 
</script>