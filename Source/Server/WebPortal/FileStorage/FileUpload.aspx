<%@ Page language="c#" Inherits="Mediachase.UI.Web.FileStorage.FileUpload" Codebehind="FileUpload.aspx.cs" %>
<%@ Reference Control="~/Modules/DialogTemplate.ascx" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/DialogTemplate.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Enctype="multipart/form-data" ControlName="../FileStorage/Modules/FileUpload.ascx"></ibn:PageTemplate>