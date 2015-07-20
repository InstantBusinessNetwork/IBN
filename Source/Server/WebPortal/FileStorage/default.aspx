<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.FileStorage._default" Codebehind="default.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate enctype="multipart/form-data" runat="server" id="pT" ControlName="../FileStorage/Modules/FileLibrary.ascx"></ibn:PageTemplate>