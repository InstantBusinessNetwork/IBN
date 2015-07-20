<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Incidents.EMailView" CodeBehind="EMailView.aspx.cs" %>
<html>
<head runat="server">
	
	<title><%=LocRM2.GetString("MessageView")%></title>
	<link rel="shortcut icon" id="iconIBN" runat="server" type='image/x-icon' />

	<style type="text/css">
		p
		{
			padding: 0;
			margin: 0;
		}
	</style>
</head>
<body onload='javascript:window.setTimeout("ResizeForm()", 100);' style="background-color: #ffffff">
	<form id="frmMain" method="post" runat="server" bgcolor="#ece9d8" enctype="multipart/form-data">

	<script type="text/javascript">
		//<![CDATA[
		window.onresize = ResizeForm;

		function ResizeForm() {
			var obj = document.getElementById('mainDiv');
			var objTBL = document.getElementById('<%=trEmail.ClientID%>');
			if (obj) {
				var intWidth = 0;
				var intHeight = 0;
				if (typeof (window.innerWidth) == "number") {
					intWidth = window.innerWidth;
					intHeight = window.innerHeight;
				}
				else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
					intWidth = document.documentElement.clientWidth;
					intHeight = document.documentElement.clientHeight;
				}
				else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
					intWidth = document.body.clientWidth;
					intHeight = document.body.clientHeight;
				}
				var trHeight = 5;
				if (objTBL)
					trHeight = 5 + objTBL.offsetHeight;
				obj.style.height = (intHeight - trHeight) + "px";
				obj.style.width = (intWidth - 5) + "px";
			}
		}
		//]]>
	</script>

	<table cellpadding="0" cellspacing="0" border="0" width="100%">
		<tr bgcolor="#ECE9D8" id="trEmail" runat="server">
			<td class="ibn-navline">
				<table cellpadding="5" class="ibn-propertysheet">
					<tr>
						<td class="ibn-label" style="width: 100px" align="right">
							<%= LocRM2.GetString("Subject")%>:
						</td>
						<td>
							<asp:Label ID="lblSubj" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td class="ibn-label" align="right">
							<%= LocRM2.GetString("tFrom")%>:
						</td>
						<td>
							<asp:Label ID="lblFrom" runat="server"></asp:Label>
						</td>
					</tr>
					<tr runat="server" id="ToRow">
						<td valign="top" class="ibn-label" align="right">
							<%= LocRM2.GetString("tTo")%>:
						</td>
						<td>
							<asp:Label ID="lblTo" runat="server"></asp:Label>
						</td>
					</tr>
					<tr runat="server" id="AttachRow">
						<td valign="top" class="ibn-label" align="right">
							<%= LocRM2.GetString("Attachments")%>:
						</td>
						<td valign="top">
							<asp:Label ID="lblAttach" runat="server"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="ibn-navline" style="padding: 5 0 0 5;">
				<div id="mainDiv" style="height: 430px; width: 713px; overflow-y: auto; overflow-x: auto;">
					<asp:Label ID="lblBody" runat="server"></asp:Label>
				</div>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
