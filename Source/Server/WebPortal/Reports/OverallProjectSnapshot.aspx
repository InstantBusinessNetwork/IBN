<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Reports.OverallProjectSnapshot" CodeBehind="OverallProjectSnapshot.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=sTitle %></title>

	<script type="text/javascript">
		//<![CDATA[
		function window.onbeforeprint() {
			var coll = document.all;
			if (coll != null) {
				for (var i = 0; i < coll.length; i++) {
					if (coll[i].Printable == "0")
						coll[i].style.display = "none";
					if (coll[i].Printable == "1")
						coll[i].style.display = "block";
				}
			}
		}
		function window.onafterprint() {
			var coll = document.all;
			if (coll != null) {
				for (var i = 0; i < coll.length; i++) {
					if (coll[i].Printable == "0")
						coll[i].style.display = "block";
					if (coll[i].Printable == "1")
						coll[i].style.display = "none";
				}
			}
		}
		//]]>
	</script>

</head>
<body>
	<form id="Form1" method="post" runat="server">
	<table width="100%" cellspacing="0" cellpadding="0" border="0">
		<tr>
			<td>
				<asp:PlaceHolder runat="server" ID="MainPlaceHolder"></asp:PlaceHolder>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
