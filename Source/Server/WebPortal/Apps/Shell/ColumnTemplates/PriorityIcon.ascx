<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Shell.ColumnTemplates.PriorityIcon" %>
<%# (Eval("PriorityId") != System.DBNull.Value) 
	? Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)Eval("PriorityId"), this.Page) 
	: ""
%>