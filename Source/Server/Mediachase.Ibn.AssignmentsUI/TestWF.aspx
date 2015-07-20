<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWF.aspx.cs" Inherits="Mediachase.Ibn.AssignmentsUI.TestWF" Trace="true" %>
<%@ Register Namespace="Mediachase.Ibn.AssignmentsUI.Modules" Assembly="Mediachase.Ibn.AssignmentsUI" TagPrefix="Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href='~/Styles/assigment.css' rel="Stylesheet" type="text/css" />
    	<script type="text/javascript">
    		function openPopupWindow(query) {
    			var w = 2 * screen.width / 3;
    			var h = 2 * screen.height / 3;
    			var l = (screen.width) / 5;
    			var t = (screen.height) / 5;

    			winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
    			if (scroll) winprops += ',scrollbars=1';
    			var f = window.open(query, "_blank", winprops);
    		}
		</script>
</head>
<body>
  <form id="form1" runat="server">
  <div>
	<asp:ScriptManager runat="server" ID="ctrlManager1" EnablePageMethods="true" AsyncPostBackTimeout="1000" />
	<a href='<%= this.ResolveUrl("~/Modules/Pages/WorkflowList.aspx") %>'>Return to workflow list</a>
	<Controls:WorkflowBuilder runat="server" ID="ctrlWF" />
  </div>
  </form>
</body>
</html>
