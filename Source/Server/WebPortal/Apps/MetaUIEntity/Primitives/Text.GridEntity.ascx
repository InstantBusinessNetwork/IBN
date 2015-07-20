<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Text_GridEntity" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			retval = GetLinkForTitleField(DataItem, FieldName, DataItem[FieldName].ToString());
		}
		return retval;
	}

	private string GetLinkForTitleField(EntityObject mo, string fieldName, string text)
	{
		string retval = text;
		MetaClass mc = MetaDataWrapper.GetMetaClassByName(mo.MetaClassName);
		if (mc.TitleFieldName == fieldName || (mc.CardOwner != null && mc.CardOwner.TitleFieldName == fieldName))
		{
			string url = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(mc.Name, mo.PrimaryKeyId.ToString()));

			if (CHelper.GetFromContext("MetaViewName") != null)
				url = String.Format(CultureInfo.InvariantCulture, "{0}&ViewName={1}", url, CHelper.GetFromContext("MetaViewName").ToString());

			retval = String.Format(CultureInfo.InvariantCulture, "<a href='{0}'>{1}</a>", url, text);
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>