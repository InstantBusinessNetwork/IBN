<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Modules.ControlPlace.MetaDataEditPage" CodeBehind="MetaDataEditPage.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="MDBEControl" Src="~/Modules/MetaDataBlockEditControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("tMDEPage")%></title>

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
<body>
	<form id="Form1" method="post" runat="server">
	<asp:ScriptManager ID="sm" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true">
	</asp:ScriptManager>
	<table cellpadding="0" cellspacing="0" border="0" width="100%" height="100%" bgcolor="#ECE9D8">
		<tr>
			<td align="center" id="tdMain" runat="server" valign="top">
				<ibn:MDBEControl ID="MDBEControl1" Path_Img="../../" runat="server" />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
