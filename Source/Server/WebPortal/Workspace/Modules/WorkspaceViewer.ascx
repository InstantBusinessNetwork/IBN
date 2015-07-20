<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkspaceViewer.ascx.cs" Inherits="Mediachase.UI.Web.Workspace.Modules.WorkspaceViewer" %>
<%@ Register Assembly="Mediachase.UI.Web" Namespace="Mediachase.Ibn.Web.UI.Layout.Extender" TagPrefix="mc2" %>
<%@ Register Assembly="Mediachase.UI.Web" Namespace="Mediachase.Ibn.Web.UI.Layout" TagPrefix="mc" %>
<%@ Register Src="~/Apps/WidgetEngine/Modules/PropertyPageContainer.ascx" TagPrefix="ibn" TagName="PropertyPage" %>

<%--<link href='<%= this.Page.ResolveClientUrl("~/layouts/styles/ext/ext-all2-workspace.css") %>' type="text/css" rel="stylesheet" />--%>
<link type="text/css" rel="stylesheet" href='<%=ResolveUrl("~/styles/IbnFramework/ibn.css")%>' />

<mc:IbnControlPlaceManager runat="server" ID="cpManager" />
<mc2:DDLayoutExtender runat="server" TargetControlID="cpManager" ID="cpManagerExtender" />

<div runat="server" id="divText" style="padding:25px;margin:25px;border:1px solid #ccc;text-align:center;">
<asp:Label ID="lblEmpty" CssClass="text" ForeColor="Red" Runat="server"></asp:Label>
</div>