<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Public.Modules.forgot" Codebehind="Forgot.ascx.cs" %>
<script type="text/javascript">
	//<![CDATA[
	function ClickEnterKey()
	{
		try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					<%= Page.ClientScript.GetPostBackEventReference(btnSend, "")%>;
			}
		catch (e) {}
	}
	//]]>
</script>
<table width="100%" border="0" cellspacing="0" cellpadding="0" class="text">
	<tr>
		<td valign="top">
			<table width="250px" border="0" cellpadding="7" cellspacing="0" class="ibn-ToolPaneFrame">
				<tr class="text">
					<td>
						<asp:Label id="lBlank" runat="server"></asp:Label>
						<asp:Label id="lWrong" runat="server" Visible="False"></asp:Label>
						<asp:Label id="lComplete" runat="server" Visible="False"></asp:Label></td>
				</tr>
				<tr class="text">
					<td onkeypress="ClickEnterKey()">
						<div style="font-size: 7pt"><%=LocRM.GetString("tEx")%></div>
						<asp:textbox id="tbLogin" runat="server" cssclass="text" width="90%" MaxLength="255"></asp:textbox>
						<asp:requiredfieldvalidator id="rfl" runat="server" display="Dynamic" cssclass="text" controltovalidate="tbLogin" errormessage=" *" /></td>
				</tr>
				<tr>
					<td><asp:button id="btnSend" runat="server" cssclass="text" text="Send" width="80px" onclick="btnSend_Click"></asp:button></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
