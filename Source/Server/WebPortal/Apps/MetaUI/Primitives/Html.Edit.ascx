<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Html.Edit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Html_Edit" %>
<%@ Register TagPrefix="ftb" Assembly="FreeTextBox" Namespace="FreeTextBoxControls" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-right:3px;">
			<ftb:FreeTextBox ID="txtValue" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" 
				   runat="Server" Height="150px" EnableHtmlMode="true"
				   DropDownListCssClass = "text"  StartMode="DesignMode"
				   GutterBackColor="#F5F5F5" BreakMode = "LineBreak" BackColor="#F5F5F5"
				   SupportFolder = "~/Scripts/FreeTextBox/"
				   JavaScriptLocation="ExternalFile" 
				   ButtonImagesLocation="ExternalFile"
				   ToolBarImagesLocation="ExternalFile"></ftb:FreeTextBox>
		</td>
		<td style="width:20px;">
			<asp:RequiredFieldValidator id="vldValue_Required" runat="server" ErrorMessage="*" ControlToValidate="txtValue"	Display="Dynamic"></asp:RequiredFieldValidator>
		</td>
	</tr>
</table>