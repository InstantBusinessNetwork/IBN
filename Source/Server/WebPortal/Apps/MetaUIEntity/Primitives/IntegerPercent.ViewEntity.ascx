<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.UI.Web.Apps.MetaUI.EntityPrimitives.IntegerPercent_ViewEntity" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			int value = 0;
			if (DataItem[FieldName].ToString() != string.Empty)
				value = Convert.ToInt32(DataItem[FieldName].ToString());
			retVal = String.Format("{0}%", value);
		}

		return String.Format("{0}", retVal);
	}
</script>
<div style="float: right;"><%# GetValue(DataItem, FieldName) %></div>