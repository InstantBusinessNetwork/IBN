<%@ Page Language="c#" Inherits="Mediachase.UI.Web.ToDo.DeleteToDoModal" CodeBehind="DeleteToDoModal.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title><%=LocRM.GetString("Delete") %></title>
	
</head>
<body style="padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px">
	<%=LocRM.GetString("DeleteTxt") %>
	<form id="PModal" method="post" runat="server">
	<p align="center">
		<input type="button" class="text" style="width: 90px" onclick="All();" value='<%#LocRM.GetString("DeleteAll") %>' />
		&nbsp;&nbsp;
		<input type="button" class="text" value='<%#LocRM.GetString("DeleteToDo") %>' style="width: 90px" onclick="ToDo();" />
		&nbsp;&nbsp;
		<input type="button" class="text" value='<%#LocRM.GetString("Cancel") %>' style="width: 90px" onclick="cancel();" />
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

		function ToDo() {
			returnValue = 'ToDo';
			window.close();
		}
		//]]>
	</script>

</body>
</html>
