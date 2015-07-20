<%@ Page Language="c#" Inherits="Mediachase.UI.Web.ToDo.ClientToDoCreate" CodeBehind="ClientToDoCreate.aspx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title>Create ToDo</title>
</head>
<body style="border-top-style: none; border-right-style: none; border-left-style: none; border-bottom-style: none">
	<form enctype="multipart/form-data" method="post" runat="server">
	<asp:ScriptManager ID="sm" runat="server" EnableScriptGlobalization="true" EnableScriptLocalization="true">
	</asp:ScriptManager>
	<table class="text" style="padding-left: 15px" cellspacing="0" cellpadding="5" border="0">
		<tr>
			<td>
				<asp:Label ID="lblTitleTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<asp:TextBox ID="txtTitle" runat="server" CssClass="text" Width="360"></asp:TextBox>
				<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
					ControlToValidate="txtTitle" Display="Dynamic"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr id="trProject" runat="server">
			<td>
				<asp:Label ID="lblProjectTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<asp:DropDownList ID="ddlProject" runat="server" CssClass="text" Width="200" AutoPostBack="True"
					OnSelectedIndexChanged="ddlProject_SelectedIndexChanged">
					<asp:ListItem Value="-1">not set</asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Label ID="lblManagerTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<asp:DropDownList ID="ddlManager" runat="server" CssClass="text" Width="200">
				</asp:DropDownList>
				<asp:Label ID="lblManager" runat="server" CssClass="text"></asp:Label>
			</td>
		</tr>
		<tr>
			<td>
				<asp:Label ID="lblStartDateTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" TimeCssClass="text"
					DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />
			</td>
		</tr>
		<tr>
			<td>
				<asp:Label ID="lblEndDateTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" TimeCssClass="text"
					DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />
				<asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Wrong Date"
					Display="Dynamic"></asp:CustomValidator>
			</td>
		</tr>
		<tr id="trPriority" runat="server">
			<td>
				<asp:Label ID="lblPriorityTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<asp:DropDownList ID="ddlPriority" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr id="trCompletion" runat="server">
			<td>
				<asp:Label ID="lblCompletionTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td width="300">
				<asp:DropDownList ID="ddlCompletionType" runat="server" Width="200px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr id="trMustConfirm" runat="server">
			<td>
			</td>
			<td width="300">
				<asp:CheckBox ID="chbMustBeConfirmed" runat="server" Text="Must be confirmed by Manager">
				</asp:CheckBox>
				<asp:Label ID="lblConfirm" runat="server" CssClass="text" Visible="False"></asp:Label>
			</td>
		</tr>
		<tr id="trAttach" runat="server">
			<td>
				<asp:Label ID="lblFileLoad" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td>
				<cc1:McHtmlInputFile ID="fAssetFile" runat="server" CssClass="text"></cc1:McHtmlInputFile>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<asp:Label ID="lblDescriptionTitle" CssClass="text" runat="server"></asp:Label>:
			</td>
			<td valign="top" width="300">
				<asp:TextBox ID="txtDescription" runat="server" CssClass="text" Width="360" TextMode="MultiLine"
					Height="100px" Rows="5"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td valign="center" align="right" colspan="2" height="40">
				<input id="txtManagerId" style="visibility: hidden" name="iGroups" runat="server">
				<btn:IMButton class="text" ID="btnSave" style="width: 110px" runat="server" Text=""
					OnServerClick="btnSave_ServerClick">
				</btn:IMButton>
				&nbsp;&nbsp;
				<btn:IMButton class="text" ID="btnCancel" runat="server" style="width: 110px;" IsDecline="true"
					CausesValidation="false">
				</btn:IMButton>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
