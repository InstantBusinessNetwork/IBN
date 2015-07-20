<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.Administration.Primitives.Text_GridEntity_CustomizationProfile_Name" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.UI.Web.Util" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			retVal = GetLinkForTitleField(DataItem, FieldName, DataItem[FieldName].ToString());
		}
		return retVal;
	}

	private string GetLinkForTitleField(EntityObject mo, string fieldName, string text)
	{
		string retval = CommonHelper.GetResFileString(text);
		MetaClass mc = MetaDataWrapper.GetMetaClassByName(mo.MetaClassName);
		if (mc.TitleFieldName == fieldName)
		{
			string url = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(mc.Name, mo.PrimaryKeyId.ToString()));

			if (CHelper.GetFromContext("MetaViewName") != null)
				url = String.Format(CultureInfo.InvariantCulture, "{0}&ViewName={1}", url, CHelper.GetFromContext("MetaViewName").ToString());

			retval = String.Format(CultureInfo.InvariantCulture, "<a href='{0}'>{1}</a>", url, retval);
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
