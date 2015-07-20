<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Date.Edit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Date_Edit" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<mc:Picker ID="dtcValue" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" />