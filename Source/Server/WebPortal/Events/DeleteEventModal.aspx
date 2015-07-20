<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Events.DeleteEventModal" CodeBehind="DeleteEventModal.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("Delete")%></title>
</head>
<body style="padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px">
	<%=LocRM.GetString("DeleteTxt") %>
	<form id="PModal" method="post" runat="server">
	<p align="center">
		<input type="button" class="text" style="width: 90px" onclick="All();" value='<%#LocRM.GetString("DeleteAll") %>'/>&nbsp;&nbsp;
		<input type="button" class="text" value='<%#LocRM.GetString("DeleteEvent") %>' style="width: 90px" onclick="Event();"/>&nbsp;&nbsp;
		<input type="button" class="text" value='<%#LocRM.GetString("Cancel") %>' style="width: 90px" onclick="cancel();"/>
	</p>
	</form>

	<script type="text/javascript">
		//<![CDATA[
		function cancel() {
			returnValue = 'Cancel';
			window.close();
		}

		function All() {
			returnValue = 'All';
			window.close();
		}

		function Event() {
			returnValue = 'Event';
			window.close();
		}
		//]]>
	</script>

</body>
</html>
