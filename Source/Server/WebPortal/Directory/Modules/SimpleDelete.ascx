<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Directory.Modules.SimpleDelete" Codebehind="SimpleDelete.ascx.cs" %>
<table height="100%" width="100%" border="0" cellpadding="0" cellspacing="0">
	<tr>
		<td align="middle" valign="center">
			<table width="95%" border="0" cellpadding="0" class="text">
				<tr>
					<td width="10">&nbsp;
					</td>
					<td valign="center" width="50">
						<img src="../layouts/images/QuickTip.gif">
					</td>
					<td>
						<%=LocRM.GetString("SimpleDeleteDialog") %>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="50">
		<td align="middle" valign="center">
			<Button id="btnDelete" onclick="DisableButtons(this);" Runat="server" Class="text" onserverclick="btnDelete_Click"></Button>&nbsp;&nbsp; 
			<Button id="btnDelFromGroup" onclick="DisableButtons(this);" Runat="server" Class="text" onserverclick="btnDelFromGroup_Click"></Button>&nbsp;&nbsp;
			<Button id="btnDeactivate" onclick="DisableButtons(this);" Runat="server" Class="text" onserverclick="btnDeactivate_Click"></Button>&nbsp;&nbsp;
			<button onclick="window.close()" class="text" type="button">
				<%=LocRM.GetString("Cancel")%>
			</button>
		</td>
	</tr>
</table>
<script language="javascript">
	function ToChangeRoles() 
	{
		try { 
		  window.opener.location.href='../Directory/ChangeRoles.aspx?UserID=<%=Request["UserID"] %>'; 
		} 
		catch (e) {} 
		window.close(); 
	} 
</script>
