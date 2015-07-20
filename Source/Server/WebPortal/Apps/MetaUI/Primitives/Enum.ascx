<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Enum" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%# (DataItem == null || DataItem.Properties[FieldName].Value == null) ? "" : MetaEnum.GetFriendlyName(DataItem.Properties[FieldName].GetMetaType().GetMetaType(), (int)DataItem.Properties[FieldName].Value)%>
