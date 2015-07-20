<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Text_Grid_Article_Question" %>
<%# (Eval("ArticleId") == DBNull.Value) ? "" :
	String.Format("<a href='{0}?ArticleId={1}'><img alt='' src='{2}' width='16px' height='16px' border='0' align='absmiddle' /> {3}</a>",
		this.Page.ResolveClientUrl("~/Incidents/ArticleView.aspx"), Eval("ArticleId").ToString(),
		this.Page.ResolveClientUrl("~/Layouts/images/kb.gif"), Eval("Question").ToString())%> 