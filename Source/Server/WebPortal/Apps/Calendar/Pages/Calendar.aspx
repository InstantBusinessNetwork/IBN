<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Pages.Calendar" %>
<%@ Reference Control="~/Modules/PageTemplateCalendarNext.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateCalendarNext.ascx" %>
<ibn:PageTemplate runat="server" id="pT" ControlName="~/Apps/Calendar/Modules/CalendarControl.ascx" Title="<%$Resources : IbnFramework.Calendar, Calendar_%>" />