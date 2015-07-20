<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryControl.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.EventService.HistoryControl" %>
<%@ Register TagPrefix="ibn" Namespace="Mediachase.Ibn.Web.UI.EventService" Assembly="Mediachase.UI.Web" %>
<div style="padding: 5px;padding-top:10px;" class="text">
	<ibn:HistoryGrid runat="server" ID="grdMain" ShowHeader="false" AllowPaging="true" 
		OnPageIndexChanged="grdMain_PageIndexChanged" OnPageIndexChanging="grdMain_PageIndexChanging" 
		OnRowCommand="grdMain_RowCommand" PagerStyle-HorizontalAlign="Right"></ibn:HistoryGrid>
</div>