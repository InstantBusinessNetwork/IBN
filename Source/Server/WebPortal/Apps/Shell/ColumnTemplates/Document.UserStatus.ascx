<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Apps.Shell.ColumnTemplates.Document.UserStatus" %>
<%#
	(Eval("CreatorId") != DBNull.Value)
	?
	Mediachase.UI.Web.Util.CommonHelper.GetUserStatus
	(
		(int)Eval("CreatorId")
	)
	:
	""
%>