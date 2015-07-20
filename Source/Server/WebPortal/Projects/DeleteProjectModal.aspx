<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.DeleteProjectModal" CodeBehind="DeleteProjectModal.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM.GetString("Delete")%></title>
</head>
<body style="padding-right: 7px; padding-left: 7px; padding-bottom: 7px; padding-top: 7px">
	<%=LocRM.GetString("DeleteTxt") %>
	<form id="PModal" method="post" runat="server">
	<p align="center">
		<input type="button" class="text" style="width: 100px" onclick="All();" value='<%#LocRM.GetString("DeleteAll") %>'/>&nbsp;&nbsp;
		<input type="button" class="text" value='<%#LocRM.GetString("DeleteProject") %>' style="width: 110px" onclick="Project();"/>&nbsp;&nbsp;
		<input type="button" class="text" value='<%#LocRM.GetString("Cancel") %>' style="width: 100px" onclick="cancel();"/>
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

		function Project() {
			returnValue = 'Project';
			window.close();
		}
		//]]>
	</script>

</body>
</html>
