<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.aspx.cs" Inherits="Mediachase.UI.Web.External.FileUpload" %>
<%@ Reference Control="~/Modules/DialogTemplate.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/DialogTemplate.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Enctype="multipart/form-data" ControlName="../FileStorage/Modules/FileUpload.ascx"></ibn:PageTemplate>