<%@ Reference Page="~/Directory/Directory.aspx" %>
<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Directory.UserEdit" Codebehind="UserEdit.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="../Modules/PageTemplateNew.ascx" %>
<ibn:PageTemplate runat="server" id="pT" Title="User Edit" ControlName="../Directory/Modules/UserEdit.ascx" enctype="multipart/form-data" ></ibn:PageTemplate>
