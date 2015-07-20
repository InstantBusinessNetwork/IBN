<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailPop3BoxStatistics.aspx.cs" Inherits="Mediachase.UI.Web.Admin.EmailPop3BoxStatistics" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=sTitle%></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
				<td>
					<ibn:BlockHeader ID="secHeader" runat="server" />
				</td>
			</tr>
			<tr>
				<td>
					<table border="0" cellpadding="5" cellspacing="0" class="text">
						<tr>
							<td align="right">
								<b>
									<%=LocRM.GetString("tEmailBox")%>: </b>
							</td>
							<td align="left">
								<asp:Label runat="server" ID="lbName" CssClass="text"></asp:Label>
							</td>
						</tr>
						<tr>
							<td align="right">
								<b>
									<%=LocRM.GetString("tIsActive")%>: </b>
							</td>
							<td align="left">
								<asp:Label runat="server" ID="lbIsActive" CssClass="text"></asp:Label>
							</td>
						</tr>
						<tr>
							<td align="right">
								<b>
									<%=LocRM.GetString("tMessageCount")%>: </b>
							</td>
							<td align="left">
								<asp:Label runat="server" ID="lbMessageCount" CssClass="text"></asp:Label>
							</td>
						</tr>
						<tr>
							<td align="right">
								<b>
									<%=LocRM.GetString("tLastReq")%>: </b>
							</td>
							<td align="left">
								<asp:Label runat="server" ID="lbLastReq" CssClass="text"></asp:Label>
							</td>
						</tr>
						<tr>
							<td align="right">
								<b>
									<%=LocRM.GetString("tLastSuccReq")%>: </b>
							</td>
							<td align="left">
								<asp:Label runat="server" ID="lbLastSuccReq" CssClass="text"></asp:Label>
							</td>
						</tr>
						<tr runat="server" id="trLastErrText" visible="false">
							<td align="right">
								<font color="red"><b>
									<%=LocRM.GetString("tLastErrText")%>: </b></font>
							</td>
							<td align="left">
								<asp:Label runat="server" ID="lbLastErrText" CssClass="text"></asp:Label>
							</td>
						</tr>
						<tr>
							<td colspan="2" align="right">
								<mc:IMButton runat="server" class='text' ID="imbCancel" onclick="javascript:window.close();" style="width: 110px" CausesValidation="false">
								</mc:IMButton>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	</form>
</body>
</html>
