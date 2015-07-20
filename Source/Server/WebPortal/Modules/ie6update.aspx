<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ie6update.aspx.cs" Inherits="Mediachase.UI.Web.Modules.ie6update" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Internet Explorer 6.0 update</title>
		<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/styles/IbnFramework/ibn.css")%>' />
		<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/styles/IbnFramework/theme.css")%>' />
		<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/styles/IbnFramework/tabStyle.css")%>' />
		<link type="text/css" rel="stylesheet" href='<%=ResolveClientUrl("~/styles/IbnFramework/mcCalendClient.css")%>' />    
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_ie6update}")%>
    </div>
    </form>
</body>
</html>
