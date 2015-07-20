<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Modules.EditControls.LongHTMLStringValue" Codebehind="LongHTMLStringValue.ascx.cs" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<FTB:FreeTextBox id="ftbValue" 
	ToolbarLayout="fontsizesmenu,undo,redo,bold,italic,underline, createlink" 
	runat="Server" Width="500px" Height="230px" EnableHtmlMode="true" DropDownListCssClass = "text" 
	GutterBackColor="#F5F5F5" StartMode="DesignMode"
	BreakMode = "LineBreak" BackColor="#F5F5F5" ToolbarBackgroundImage="false" 
	SupportFolder = "~/Scripts/FreeTextBox/"
	JavaScriptLocation="ExternalFile" 
	ButtonImagesLocation="ExternalFile"
	ToolBarImagesLocation="ExternalFile" />
<asp:RequiredFieldValidator id="txtValueRFValidator" runat="server" ErrorMessage="*" ControlToValidate="ftbValue"	Display="Dynamic"></asp:RequiredFieldValidator>
