<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentEditorPropertyPage.ascx.cs" Inherits="Mediachase.UI.Web.Apps.Shell.Modules.ContentEditorPropertyPage" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<div id="divHTML" class="text" style="padding: 3px;">
	<div style="padding-bottom: 15px">
		<asp:LinkButton runat="server" ID="lblHtml" Visible="false"/><asp:LinkButton runat="server" ID="lblIframe" Visible="true"/>
	</div>
	<div runat="server" id="divIframe" visible="false" style="padding-top: 15px;">
	<table style="width: 100%; table-layout: fixed;" cellpadding="5">
		<tr>
			<td style="width: 150px;"><asp:Literal runat="server" ID="Literal1" Text="<%$Resources: IbnFramework.WidgetEngine, _mc_ExternalLink %>" />: </td>
			<td>http://<asp:TextBox runat="server" ID="tbSource" Width="165px" /></td>
		</tr>
			<tr>
			<td style="width: 150px;"><asp:Literal runat="server" ID="ltHeight" Text="<%$Resources: IbnFramework.Global, _ce_height %>" /></td>
			<td><asp:TextBox runat="server" ID="tbHeight" Width="200px" /></td>
		</tr>
	</table>
	</div>
	<table style="width: 100%; table-layout: fixed;" cellpadding="5">
		<tr>
			<td style="width: 150px;"><asp:Literal runat="server" ID="ltTitle" Text="<%$Resources: IbnFramework.Global, _ce_title %>" /></td>
			<td><asp:TextBox runat="server" ID="tbTitle" Width="200px"/></td>
		</tr>
	</table>
	<div runat="server" id="divHtml" visible="true" >
	<FTB:FreeTextBox id="fckEditor" ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink,fontforecolorsmenu,fontbackcolorsmenu" 
		runat="Server" Width="98%" Height="245px" DropDownListCssClass = "text" 
		GutterBackColor="#F5F5F5"  BreakMode = "LineBreak" BackColor="#F5F5F5"
		StartMode="DesignMode"
		SupportFolder = "~/Scripts/FreeTextBox/"
		JavaScriptLocation="ExternalFile" 
		ButtonImagesLocation="ExternalFile"
		ToolBarImagesLocation="ExternalFile" />
	</div>
</div>