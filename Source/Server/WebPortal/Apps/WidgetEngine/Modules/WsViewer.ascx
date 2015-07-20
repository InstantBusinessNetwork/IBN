<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WsViewer.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Modules.WsViewer" %>
<%@ Register Assembly="Mediachase.UI.Web" Namespace="Mediachase.Ibn.Web.UI.Apps.WidgetEngine.Extender" TagPrefix="mc2" %>
<%@ Register Assembly="Mediachase.UI.Web" Namespace="Mediachase.Ibn.Web.UI.Apps.WidgetEngine" TagPrefix="mc" %>
<%@ Register Src="~/Apps/WidgetEngine/Modules/PropertyPageContainer.ascx" TagPrefix="ibn" TagName="PropertyPage" %>
<mc:IbnControlPlaceManager runat="server" ID="cpManager" />
<mc2:WsLayoutExtender runat="server" TargetControlID="cpManager" ID="cpManagerExtender" />
<div runat="server" id="divText" style="padding:25px;margin:25px;border:1px solid #ccc;text-align:center; display: none;">
	<asp:Label ID="lblEmpty" CssClass="text" ForeColor="Red" Runat="server"></asp:Label>
</div>
