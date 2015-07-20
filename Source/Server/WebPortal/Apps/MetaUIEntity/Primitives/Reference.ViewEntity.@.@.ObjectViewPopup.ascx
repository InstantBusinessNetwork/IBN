<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Reference_ViewEntity_All_All_ObjectViewPopup"%>
<%@ Import Namespace="System.Globalization" %>
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
			MetaField field = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);
			string referencedClass = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName);

			EntityObject obj = BusinessManager.Load(referencedClass, (PrimaryKeyId)DataItem[FieldName]);

			string url = String.Format(CultureInfo.InvariantCulture, "~/Apps/ListApp/Pages/ObjectViewPopup.aspx?ClassName={0}&ObjectId={1}&mode=popup", referencedClass, DataItem[FieldName].ToString());
			url = ResolveClientUrl(url);

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(referencedClass);
			retVal = String.Format("<a href='{0}'>{1}</a>", url, CHelper.GetResFileString(obj.Properties[mc.TitleFieldName].Value.ToString()));
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
