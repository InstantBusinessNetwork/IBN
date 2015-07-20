<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.ListApp.Primitives.Text_GridEntity_ListInfo" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem.Properties[FieldName].Value != null)
		{
			retval = GetLinkForTitleField(DataItem, FieldName, DataItem.Properties[FieldName].Value.ToString());
		}
		return retval;
	}

	private string GetLinkForTitleField(EntityObject mo, string fieldName, string text)
	{
		string retval = text;
		string url = ResolveClientUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_" + mo.PrimaryKeyId.Value.ToString());

		retval = String.Format(CultureInfo.InvariantCulture, "<a href='{0}'>{1}</a>", url, text);
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>