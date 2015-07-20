<%@ Reference Control="~/Modules/PageTemplateExternal.ascx" %>
<%@ Page language="c#" Inherits="Mediachase.UI.Web.External.ExternalIncident" Codebehind="ExternalIncident.aspx.cs" %>
<%@ Register TagName="PageTemplate" TagPrefix="ibn" src="../Modules/PageTemplateExternal.ascx" %>
<ibn:pagetemplate id="pT" enctype="multipart/form-data" title="" runat="server" ControlName="../Incidents/Modules/IncidentView1.ascx"></ibn:pagetemplate>
