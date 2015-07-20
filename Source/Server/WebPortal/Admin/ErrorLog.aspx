<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.Admin.ErrorLog" Codebehind="ErrorLog.aspx.cs" %>
<%@ Register TagPrefix="ibn" TagName="PageTemplate" src="~/Modules/PageTemplateNew.ascx" %>
<ibn:pagetemplate runat="server" id="pT" title="Error Log" controlname="~/Admin/Modules/ErrorLog.ascx" enctype="multipart/form-data"></ibn:pagetemplate>
