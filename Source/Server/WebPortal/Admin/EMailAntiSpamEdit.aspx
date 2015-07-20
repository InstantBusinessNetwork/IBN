<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Admin.EMailAntiSpamEdit" CodeBehind="EMailAntiSpamEdit.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=sTitle%></title>

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
	<form id="frmMain" method="post" runat="server" onkeypress="disableEnterKey()" enctype="multipart/form-data">
	<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td>
				<ibn:BlockHeader ID="secHeader" runat="server" />
			</td>
		</tr>
		<tr>
			<td style="padding: 5px">
				<table cellpadding="7" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tAction")%>:</b>
						</td>
						<td>
							<asp:RadioButtonList CellPadding="3" ID="rbList" runat="server" CssClass="text" RepeatColumns="2">
							</asp:RadioButtonList>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tType")%>:</b>
						</td>
						<td>
							<asp:DropDownList AutoPostBack="True" ID="ddType" runat="server" Width="200px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tKey")%>:</b>
						</td>
						<td>
							<asp:DropDownList ID="ddKey" runat="server" Width="200px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr id="trFillList" runat="server">
						<td>
						</td>
						<td>
							<asp:CheckBox ID="cbFillList" runat="server" />
						</td>
					</tr>
					<tr id="trValue" runat="server">
						<td>
							<b>
								<%=LocRM.GetString("tValue")%>:</b>
						</td>
						<td>
							<asp:TextBox ID="txtValue" runat="server" CssClass="text" Width="200px"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%=LocRM.GetString("tWeight")%>:</b>
						</td>
						<td>
							<asp:DropDownList ID="ddWeight" runat="server" Width="200px">
							</asp:DropDownList>
						</td>
					</tr>
					<tr valign="bottom">
						<td align="right" colspan="2">
							<btn:IMButton runat="server" class='text' ID="imbSave" style="width: 110px">
							</btn:IMButton>
							&nbsp;
							<btn:IMButton runat="server" class='text' ID="imbCancel" onclick="javascript:window.close();" style="width: 110px" CausesValidation="false">
							</btn:IMButton>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
