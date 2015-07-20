<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Integer_Grid_Incident_IncidentId_IncidentList" %>
<%# Eval("IncidentId") == DBNull.Value ? "" : "#" + Eval("IncidentId").ToString()%>
