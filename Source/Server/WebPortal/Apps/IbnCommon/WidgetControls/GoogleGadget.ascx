<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleGadget.ascx.cs" Inherits="Mediachase.UI.Web.Apps.IbnCommon.WidgetControls.GoogleGadget" %>
<iframe runat="server" id="mainFrame" width="100%" border="0" style="border: solid 0px black;" />
<div style="padding: 7px;">
<asp:Literal runat="server" ID="ltEmpty" Text="<%$ Resources : IbnFramework.WidgetEngine, _mc_GoogleGadgetEmpty %>" />
</div>

