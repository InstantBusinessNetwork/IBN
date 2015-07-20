<%@ Page Language="C#" AutoEventWireup="true" Inherits="Mediachase.UI.Web.Incidents.AddEMailMessage" CodeBehind="AddEMailMessage.aspx.cs" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("tbAddEMail")%></title>
	<link rel="shortcut icon" id="iconIBN" runat="server" type='image/x-icon' />
</head>
<body onload='javascript:window.setTimeout("ResizeForm()",100);SetInvisible();'>
	<form id="form1" runat="server">
	<table id="topTable" width="100%" border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td colspan="4">
				<div id="divMessage" runat="server" class="text" style="vertical-align: middle; padding-top: 5px; padding-bottom: 5px; background-color: #ffffe1; border: 1px solid #bbb;">
					<blockquote style="padding-left: 10px; margin: 5px; margin-left: 15px; padding-bottom: 3px">
						<div style="float: left; padding-right: 10px;">
							<img src="../layouts/images/deleteuser.gif" align="absmiddle" /></div>
						<div style="float: left;">
							<asp:Label ID="lblException" runat="server" CssClass="text"></asp:Label></div>
						<div style="clear: both; padding-left: 45px;">
							<asp:Label ID="lblSMTP" runat="server" CssClass="text"></asp:Label></div>
					</blockquote>
				</div>
			</td>
		</tr>
		<tr>
			<td class="leftButtonCell" rowspan="3" valign="top">
				<mc:IMButton ImageWidth="32px" ImageHeight="16px" runat="server" class="text" ID="imbSave" style="width: 95px; height: 40px;">
				</mc:IMButton>
				<asp:LinkButton ID="lbSend" runat="server" Style="display: none;" OnClick="lbSend_Click"></asp:LinkButton>
			</td>
			<td class="centerCaptionCell">
				<b>
					<%=LocRM.GetString("tFrom")%>:</b>
			</td>
			<td class="rightSendCell">
				<div style="padding: 2px;">
					<asp:Label ID="txtFrom" CssClass="txtFromStyle" runat="server"></asp:Label></div>
			</td>
			<td class="rightActionsCell">
				&nbsp;
			</td>
		</tr>
		<tr>
			<td class="centerCaptionCell" valign="top">
				<b>
					<asp:Label ID="lblCCTitle" runat="server"></asp:Label></b>
			</td>
			<td class="rightSendCell" valign="top">
				<asp:Label ID="lblCC" runat="server"></asp:Label>
			</td>
			<td class="rightActionsCell">
				&nbsp;
			</td>
		</tr>
		<tr>
			<td class="centerCaptionCell" valign="top">
				<b>
					<asp:Label ID="lblToTitle" runat="server"></asp:Label></b>
			</td>
			<td class="rightSendCell" valign="top">
				<asp:TextBox ID="txtTo" runat="server" Width="99%" TextMode="MultiLine" Rows="3"></asp:TextBox>
			</td>
			<td class="rightActionsCell">
				<a href="javascript:openAddEMails('<%=txtTo.ClientID %>');">
					<img align="absmiddle" title='<%=LocRM.GetString("tAddEMails") %>' border="0" width="16" alt="" src="../layouts/images/addressbook.gif" /></a>
			</td>
		</tr>
		<tr>
			<td>
			</td>
			<td class="centerCaptionCell">
				<b>
					<%=LocRM.GetString("Subject")%>:</b>
			</td>
			<td class="rightSendCell">
				<asp:TextBox ID="txtSubject" runat="server" Width="99%"></asp:TextBox>
			</td>
			<td>
				&nbsp;
			</td>
		</tr>
		<tr>
			<td>
			</td>
			<td>
				<span id="percValue" style="float: left;"></span>
			</td>
			<td class="rightSendCell" colspan="2">
				<table cellpadding="0" cellspacing="0" width="100%" border="0" style="margin-bottom: 5px;">
					<tr>
						<td style="padding-right: 1px;">
							<div id="divAttachs">
							</div>
							<div id="divWait" style="display: none;">
								<i>
									<%=LocRM3.GetString("tWaitForUploading") %></i></div>
							<div id="divProgress" style="display: none;">
								<div id="progressContainer" style="float: left; border: 1px solid #008000; height: 17px; width: 250px;">
									<div id="progressBar" style="background-color: #00aa00; margin: 1px; height: 15px; display: block;">
									</div>
								</div>
							</div>
						</td>
						<td class="uploadActionsCell">
							<div id='divUploadButton' style="position: absolute; left: 200px; top: 200px; z-index: 900; text-align: right;">
								<img align="absmiddle" title='<%=LocRM.GetString("tUploadFiles") %>' border="0" width="16" alt="" src="../layouts/images/upload.gif" />&nbsp;<a href="javascript:StartUpload();" title='<%=LocRM.GetString("tUploadFiles") %>'><%=LocRM.GetString("tUpload") %></a>&nbsp;
							</div>
						</td>
						<td class="rightActionsCell">
							<a href="javascript:{openAddArticle('<%=_guid %>');}" title='<%=LocRM.GetString("tAddIBNArticle") %>'>
								<img align="absmiddle" title='<%=LocRM.GetString("tAddIBNArticle") %>' border="0" width="16" alt="" src="../layouts/images/attachfromkb.gif" /></a>
						</td>
						<td class="rightActionsCell">
							<a href="javascript:{openAddPortalFiles('<%=_guid %>');}" title='<%=LocRM.GetString("tAddIBNfiles") %>'>
								<img align="absmiddle" id="imgAttachInner" runat="server" border="0" width="16" alt="" /></a>
						</td>
						<td class="rightActionsCell">
							<div id="divAddAttach">
								<a href="javascript:SetInvisible();">
									<img align="absmiddle" title='<%=LocRM.GetString("tAddAttachments") %>' border="0" width="16" alt="" src="../layouts/images/attachtomail.gif" /></a>
							</div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr id="trUploadFrame">
			<td colspan="4" class="iframecell">
				<iframe name="uploadFrame" id="uploadFrame" frameborder="0" marginwidth="0" style="height: 110px;" marginheight="0" width="100%" src='AddEMailMessageUploadHandler.aspx?guid=<%=_guid%>'></iframe>
			</td>
		</tr>
	</table>
	<div id="divHTML">
		<FTB:FreeTextBox ID="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" runat="Server" Width="98%" Height="300px" DropDownListCssClass="text" GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5" StartMode="DesignMode" SupportFolder="~/Scripts/FreeTextBox/" JavaScriptLocation="ExternalFile" ButtonImagesLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" RemoveServerNameFromUrls="false" />
	</div>
	<asp:HiddenField ID="hidGuid" runat="server" />
	</form>
	<script type="text/javascript">window.onresize = ResizeForm;</script>
</body>
</html>
