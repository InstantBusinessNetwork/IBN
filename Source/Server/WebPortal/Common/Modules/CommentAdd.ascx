<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.CommentAdd" Codebehind="CommentAdd.ascx.cs" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<table cellspacing="3" cellpadding="3" border="0" class="ibn-propertysheet" width="100%" class="pNoMargin">
	<tr>
		<td valign="top">
			<FTB:FreeTextBox ID="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink" runat="Server" Width="100%" Height="120px" EnableHtmlMode="true" DropDownListCssClass="text" GutterBackColor="#F5F5F5" BreakMode="LineBreak" BackColor="#F5F5F5" ToolbarBackgroundImage="false" SupportFolder="~/Scripts/FreeTextBox/" JavaScriptLocation="ExternalFile" ButtonImagesLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" DesignModeBodyTagCssClass="pNoMargin" DesignModeCss="~/styles/IbnFramework/ibn.css" RemoveServerNameFromUrls="false" FormatHtmlTagsToXhtml="true" />
		</td>
	</tr>
	<tr id="trButtons">
		<td align="right" height="40" valign="middle">
			<btn:IMButton ID="btnSave" runat="server" Class="text" style="width: 110px;" OnServerClick="btnSave_ServerClick"></btn:IMButton>&nbsp;&nbsp;
			<btn:IMButton ID="btnCancel" CausesValidation="false" runat="server" Class="text" style="width: 110px;" IsDecline="true"></btn:IMButton>
		</td>
	</tr>
</table>
