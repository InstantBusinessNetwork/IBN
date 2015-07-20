<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResourceEditor.ascx.cs" Inherits="Mediachase.UI.Web.Apps.Common.Modules.ResourceEditor" %>
<%@ Register Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" TagPrefix="WebControls" %>
<asp:TextBox runat="server" ID="tbValue" Width="200" />
<asp:RequiredFieldValidator runat="server" ID="fieldText" ControlToValidate="tbValue" ErrorMessage="*" />
<asp:Image runat="server" ID="imgHelp" CssClass="hiddenImage" />
<asp:Image runat="server" ID="imgSuccess" CssClass="hiddenImage"/>
<asp:Image runat="server" ID="imgFailed" CssClass="hiddenImage"/>
<asp:Image runat="server" ID="imgLoading" CssClass="hiddenImage"/>
<WebControls:ResourceEditorExtender runat="server" ID="tbValueExt" TargetControlID="tbValue" EmptyText="Click here" CssClassLabel="labelClass" />
