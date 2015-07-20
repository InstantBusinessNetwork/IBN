<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryDataView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.EntityControls.HistoryDataView" %>
<%@ Reference Control="~/Apps/MetaUIEntity/Modules/EntityListLight.ascx" %>
<%@ Register TagPrefix="ibn" TagName="GridLight" Src="~/Apps/MetaUIEntity/Modules/EntityListLight.ascx" %>
<ibn:GridLight ID="gridHistory" runat="server" />