<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.MyWork.FinishDate" %>
<%# 
	(Eval("FinishDate") != System.DBNull.Value) 
	?
	((DateTime)Eval("FinishDate") < DateTime.UtcNow.AddYears(10))
	? ((DateTime)Eval("FinishDate") < Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow) ? "<span style='color:red'>" + ((DateTime)Eval("FinishDate")).ToShortDateString() + "</span>" : ((DateTime)Eval("FinishDate")).ToShortDateString())
	: ""
	: ""
%>