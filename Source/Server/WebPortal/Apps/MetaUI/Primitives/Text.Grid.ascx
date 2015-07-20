<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Text_Grid" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem.Properties[FieldName].Value != null)
		{
			retval = GetLinkForTitleField(DataItem, FieldName, DataItem.Properties[FieldName].Value.ToString());
		}
		return retval;
	}

	private string GetLinkForTitleField(MetaObject mo, string fieldName, string text)
	{
		string retval = text;
		if (mo.GetMetaType().TitleFieldName == fieldName)
		{
			string metaClassName = mo.GetMetaType().Name;
			if (mo.GetCardMetaType() != null)
				metaClassName = mo.GetCardMetaType().Name;

			string url = ResolveClientUrl(CHelper.GetLinkObjectView_Edit(metaClassName, mo.PrimaryKeyId.ToString()));

			if (CHelper.GetFromContext("MetaViewName") != null)
				url = String.Format(CultureInfo.InvariantCulture, "{0}&ViewName={1}", url, CHelper.GetFromContext("MetaViewName").ToString());

			retval = String.Format(CultureInfo.InvariantCulture, "<a href='{0}'>{1}</a>", url, text);
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>