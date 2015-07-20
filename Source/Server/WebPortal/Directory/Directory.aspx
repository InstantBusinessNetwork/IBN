<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Directory.Directory" Codebehind="Directory.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Directory" ControlName="../Directory/Modules/DirectoryBrowser.ascx"></ibn:PageTemplate>
