<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.EditActualTimeline" CodeBehind="EditActualTimeline.aspx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM2.GetString("timeline_info")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<asp:ScriptManager ID="sm" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true">
	</asp:ScriptManager>
	<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
		<tr>
			<td width="150" align="right">
				<b>
					<%= LocRM.GetString("actual_start_date")%>:</b>
			</td>
			<td class="ibn-value">
				<mc:Picker ID="dtcActualStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="false" />
			</td>
		</tr>
		<tr>
			<td width="150" align="right">
				<b>
					<%= LocRM.GetString("actual_finish_date")%>:</b>
			</td>
			<td class="ibn-value">
				<mc:Picker ID="dtcActualFinishDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="false" />
			</td>
		</tr>
		<tr>
			<td colspan="2" class="ibn-value" height="40" align="center">
				<asp:CustomValidator ID="cvDates" runat="server" Display="Static" ErrorMessage="*"></asp:CustomValidator>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="center">
				<btn:IMButton class="text" ID="btnSave" runat="server" style="width: 120">
				</btn:IMButton>
				<br/>
				<br/>
				<btn:IMButton class="text" ID="btnCancel" runat="server" IsDecline="true" CausesValidation="false" style="width: 120">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
