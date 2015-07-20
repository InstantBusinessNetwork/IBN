<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.Security.Primitives.RoleMultiValue.Grid" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Services" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = String.Empty;

		if (DataItem != null && DataItem.Properties[FieldName].Value != null && DataItem.Properties[FieldName].Value is Mediachase.Ibn.Data.Services.RolePrincipalCollection)
		{
			Mediachase.Ibn.Data.Services.RolePrincipalCollection coll = (Mediachase.Ibn.Data.Services.RolePrincipalCollection)DataItem.Properties[FieldName].Value;
			foreach (Mediachase.Ibn.Data.Services.RolePrincipal srp in coll)
			{
				if (!String.IsNullOrEmpty(retVal))
					retVal += "<br />";

				retVal += srp.Principal;
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
