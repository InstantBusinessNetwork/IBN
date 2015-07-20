<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Incidents.AddForumMessageUploadHandler" CodeBehind="AddForumMessageUploadHandler.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title>UploadHandler</title>
</head>
<body>
	<form id="uploadForm" method="post" runat="server" enctype="multipart/form-data">

	<script type="text/javascript">
		//<![CDATA[
		function SetVisibility(idObj) {
			var obj = document.getElementById(idObj);
			if (obj)
				obj.style.display = "";
		}
		//]]>
	</script>

	<table cellpadding="0" cellspacing="0" width="100%" border="0">
		<tr>
			<td>
				<table cellpadding="5" cellspacing="0" width="100%" border="0" class="text ibn-alternating ibn-navline">
					<tr align="left">
						<td width="150px" class="ibn-label">
							<%= LocRM2.GetString("IssueStatus")%>:
						</td>
						<td align="left">
							<asp:DropDownList runat="server" ID="ddlStatus" Width="130px">
							</asp:DropDownList>
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<asp:CheckBoxList RepeatDirection="Horizontal" ID="cbList" runat="server">
							</asp:CheckBoxList>
						</td>
						<td>
							&nbsp;
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<table cellpadding="5" cellspacing="0" width="100%" border="0" class="text">
					<tr>
						<td colspan="2" valign="top">
							<asp:TextBox runat="server" ID="txtMessage" TextMode="MultiLine" Rows="14" Width="100%"></asp:TextBox>
						</td>
					</tr>
					<tr id="trFirst">
						<td class="ibn-label">
							<%= LocRM.GetString("File")%>:
						</td>
						<td>
							<cc1:McHtmlInputFile onchange="SetVisibility('trSecond')" ID="McFileUp1" Width="100%" Size="40" runat="server"></cc1:McHtmlInputFile>
						</td>
					</tr>
					<tr id="trSecond" style="display: none">
						<td class="ibn-label">
							<%= LocRM.GetString("File")%>
							2:
						</td>
						<td>
							<cc1:McHtmlInputFile onchange="SetVisibility('trThird')" ID="McFileUp2" Width="100%" Size="40" runat="server"></cc1:McHtmlInputFile>
						</td>
					</tr>
					<tr id="trThird" style="display: none">
						<td class="ibn-label">
							<%= LocRM.GetString("File")%>
							3:
						</td>
						<td>
							<cc1:McHtmlInputFile ID="McFileUp3" Width="100%" Size="40" runat="server"></cc1:McHtmlInputFile>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
