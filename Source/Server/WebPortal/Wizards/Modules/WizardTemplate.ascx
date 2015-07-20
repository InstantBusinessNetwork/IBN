<%@ Register TagPrefix="mc" Namespace="Mediachase.UI.Web.Modules" Assembly="Mediachase.UI.Web" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.WizardTemplate" CodeBehind="WizardTemplate.ascx.cs" %>
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("WindowTitle")%></title>
	<link id="iconIBN" runat="server" type='image/x-icon' rel="shortcut icon" />

	<script type="text/javascript">
		//<![CDATA[
		function disableEnterKey() {
			try {
				if (window.event.keyCode == 13 && window.event.srcElement.type != "textarea")
					window.event.keyCode = 0;
			}
			catch (e) { }
		}
		//]]>
	</script>

</head>
<body style="margin: 0px; background-color: #ece9d8">
	<form id="frmMain" method="post" runat="server" onkeypress="disableEnterKey()" bgcolor="#ece9d8" enctype="multipart/form-data">
	<asp:ScriptManager ID="sm" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true" EnableHistory="false"></asp:ScriptManager>
	<table cellspacing="0" cellpadding="0" width="100%" border="0" style="background-color: #ffffff; height: 100%">
		<tr>
			<td class="topHeader" style="padding-right: 0px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px" valign="top">
				<asp:Label ID="topHeader" runat="server"></asp:Label>
			</td>
		</tr>
		<tr>
			<td style="padding-right: 10px; padding-left: 10px; padding-bottom: 2px; padding-top: 0px; border-bottom: #aca199 2px solid" valign="top">
				<table cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td class="topSubHeader">
							<asp:Label ID="topSubHeader" runat="server"></asp:Label>
						</td>
						<td class="step" align="right">
							<asp:Label ID="lblSteps" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td style="padding-right: 7px; border-top: #ffffff 1px solid; padding-left: 7px; padding-bottom: 7px; padding-top: 7px; border-bottom: #aca199 2px solid; height: 100%" valign="top" bgcolor="#ece9d8" height="100%">
				<asp:PlaceHolder ID="phControl" runat="server"></asp:PlaceHolder>
			</td>
		</tr>
		<tr>
			<td style="padding-right: 7px; border-top: #ffffff 1px solid" align="right" bgcolor="#ece9d8" height="42">
				<table cellspacing="3" cellpadding="0" border="0">
					<tr>
						<td>
							<button id="Migrated_btnBack" type="button" runat="server" causesvalidation="False" onserverclick="btnBack_Click"></button>
						</td>
						<td>
							<button id="Migrated_btnNext" runat="server" onserverclick="btnNext_Click"></button>
						</td>
						<td width="5">
							&nbsp;
						</td>
						<td>
							<button id="btnCancel" type="button" runat="server" causesvalidation="False" onserverclick="btnCancel_Click"></button>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
	<iframe id="__historyFrame" style="display: none" src='<%=this.ResolveClientUrl("~/Apps/Shell/Pages/Empty.html")%>' />
</body>
</html>
