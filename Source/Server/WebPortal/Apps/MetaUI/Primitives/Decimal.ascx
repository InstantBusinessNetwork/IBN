<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Decimal" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = String.Empty;
		
		if (DataItem != null)
		{
			MetaObjectProperty property = DataItem.Properties[FieldName];
			if (property != null && property.Value != null)
			{
				retVal = ((System.Decimal)property.Value).ToString();
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>