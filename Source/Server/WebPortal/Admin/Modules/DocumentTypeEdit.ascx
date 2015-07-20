<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.DocumentTypesEdit" Codebehind="DocumentTypeEdit.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
	function UpdateImage(ImgID,path)
	{
		imgc = document.getElementById(ImgID);
		if (path != "")
			imgc.src = path;
		else
			imgc.src = '<%# ResolveUrl ("~/Layouts/Images/FileTypes/unknown16.gif") %>';
	}
	function UpdateIcon(path)
	{
		UpdateImage('<%=imgIcon.ClientID %>',path)
	}
	function UpdateBigIcon(path)
	{
		UpdateImage('<%=imgBigIcon.ClientID %>',path)
	}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<table cellspacing="0" cellpadding="5" width="100%" align="center" border="0" class="text">
				<tr>
					<td style="PADDING-RIGHT: 5px" align="left" width="100" class="text ibn-label"><%#LocRM.GetString("Icon") %>:</td>
					<td>
						<asp:Image id="imgIcon" runat="server" Width="16px" Height="16px" ImageUrl="~/Layouts/Images/FileTypes/unknown16.gif"></asp:Image>
						<cc1:McHtmlInputFile id="fIcon" runat="server" CssClass="text" Width="300px" onpropertychange="UpdateIcon(this.value)"></cc1:McHtmlInputFile>&nbsp;
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label" valign="baseline"><%#LocRM.GetString("BigIcon") %>:</td>
					<td valign="baseline">
						<asp:Image id="imgBigIcon" ImageAlign="AbsBottom" runat="server" Width="48px" Height="48px" ImageUrl="~/Layouts/Images/FileTypes/unknown48.gif"></asp:Image>
						<cc1:McHtmlInputFile id="fBigIcon" runat="server" CssClass="text" Width="300px" onpropertychange="UpdateBigIcon(this.value)"></cc1:McHtmlInputFile>&nbsp;
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label"><%#LocRM.GetString("Extension") %>:</td>
					<td>
						<asp:Label id="lblExtension" runat="server"></asp:Label>
						<asp:TextBox id="tbExtension" runat="server" Width="300px" CssClass="text"></asp:TextBox>&nbsp;
						<asp:RequiredFieldValidator id="rfExtension" runat="server" ErrorMessage="*" ControlToValidate="tbExtension" Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label"><%#LocRM.GetString("MimeType") %>:</td>
					<td>
						<asp:Label id="lblContentType" runat="server" CssClass="text"></asp:Label>
						<asp:TextBox id="tbMimeType" runat="server" Width="300px" CssClass="text"></asp:TextBox>&nbsp;
						<asp:RequiredFieldValidator id="rfMimeType" runat="server" ErrorMessage="*" ControlToValidate="tbMimeType" Display="Dynamic"></asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label">WebDav:</td>
					<td class="text">
					  <asp:CheckBox ID="cbAllowWebDav" runat="server" />
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label"><%#LocRM.GetString("InNewWindow") %>:</td>
					<td class="text">
					  <asp:CheckBox ID="cbInNewWindow" runat="server" />
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label"><%#LocRM.GetString("AllowForceDownload") %>:</td>
					<td class="text">
					  <asp:CheckBox ID="cbForceDownload" runat="server" />
					</td>
				</tr>
				<tr>
					<td style="PADDING-RIGHT: 5px" class="text ibn-label"><%#LocRM.GetString("FriendlyName") %>:</td>
					<td>
						<asp:TextBox id="tbFriendlyName" runat="server" Width="300px" CssClass="text"></asp:TextBox>&nbsp;
						<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="tbFriendlyName" ErrorMessage="*"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td></td>
					<td>
						<btn:imbutton class="text" id="btnSubmit" Runat="server" style="width:110px;" onserverclick="btnSave_Click"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" CausesValidation="false" style="width:110px;" id="btnCancel" Runat="server" IsDecline="true" onserverclick="btnCancel_Click"></btn:imbutton></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
