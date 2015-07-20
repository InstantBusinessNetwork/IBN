<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.Integer_Grid_Client_Type_SelectObject" %>
<%# Eval("Type") == DBNull.Value ? "&nbsp;" :
	((int)Eval("Type") == (int)Mediachase.IBN.Business.ObjectTypes.Organization ?
	GetGlobalResourceObject("IbnFramework.Common", "Organization").ToString() :
	GetGlobalResourceObject("IbnFramework.Common", "Contact").ToString()) %>