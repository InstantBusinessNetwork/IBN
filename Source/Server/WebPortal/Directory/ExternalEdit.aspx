<%@ Reference Page="~/Directory/Directory.aspx" %>
<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Directory.ExternalEdit" Codebehind="ExternalEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="External User Edit" ControlName="../Directory/Modules/ExternalEdit.ascx" enctype="multipart/form-data"></ibn:PageTemplate>
