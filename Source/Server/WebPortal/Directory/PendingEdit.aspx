<%@ Reference Page="~/Directory/Directory.aspx" %>
<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Directory.PendingEdit" Codebehind="PendingEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="Pening User Edit" ControlName="../Directory/Modules/PendingEdit.ascx" enctype="multipart/form-data"></ibn:PageTemplate>
