<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.DecimalPercent" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null)
		{
			System.Decimal value = Convert.ToDecimal("0");
			if (!String.IsNullOrEmpty(DataItem.Properties[FieldName].Value.ToString()))
				value = Convert.ToDecimal(DataItem.Properties[FieldName].Value.ToString());
			
			retVal = value.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
			retVal = String.Format("{0}%", retVal);
		}

		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>