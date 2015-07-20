<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTime.Edit.CalendarEvent.Start.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Primitives.DateTime_Edit_CalendarEvent_Start" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<mc:Picker ID="dtcValue" runat="server" DateCssClass="text" TimeCssClass="text" DateWidth="85px" TimeWidth="60px" ShowImageButton="false" ShowTime="true" />