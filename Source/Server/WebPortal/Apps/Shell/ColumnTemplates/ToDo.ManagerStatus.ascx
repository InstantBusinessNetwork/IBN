<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.ToDo.ManagerStatus" %>
<%# (Eval("ItemId") != System.DBNull.Value) 
	? Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)Eval("ManagerId"))
	: ""
%>