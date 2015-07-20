<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Identifier_View" %>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
	  	<td>
		  <%# (DataItem == null || DataItem.Properties[FieldName].Value == null) ? "" : DataItem.Properties[FieldName].Value.ToString()%>
		</td>
	</tr>
</table>