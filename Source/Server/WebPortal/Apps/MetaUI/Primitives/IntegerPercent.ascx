<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.UI.Web.Apps.MetaUI.Primitives.IntegerPercent" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
		string retVal = "";

		if (DataItem != null && DataItem.Properties != null)
		{
			int value = 0;
			if (DataItem.Properties[FieldName].Value.ToString() != string.Empty)
				value = Convert.ToInt32(DataItem.Properties[FieldName].Value.ToString());
			retVal = String.Format("{0}%", value);
		}

		return String.Format("{0}", retVal);
    }
</script>
<div style="float: right;"><%# GetValue(DataItem, FieldName) %></div>