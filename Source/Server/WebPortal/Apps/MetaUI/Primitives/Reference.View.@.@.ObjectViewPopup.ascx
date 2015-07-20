<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Reference_View_All_All_ObjectViewPopup"%>
<%@ Import Namespace="System.Globalization" %>
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
			MetaObjectProperty property = DataItem.Properties[FieldName];
			if (property != null && property.Value != null)
			{
				string referencedClass = property.GetMetaType().Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
				MetaClass mc = MetaDataWrapper.GetMetaClassByName(referencedClass);

				MetaObject obj = new MetaObject(mc, (PrimaryKeyId)property.Value);

				string url = String.Format(CultureInfo.InvariantCulture, "~/Apps/ListApp/Pages/ObjectViewPopup.aspx?ClassName={0}&ObjectId={1}&mode=popup", referencedClass, property.Value.ToString());
				url = ResolveClientUrl(url);
				retVal = String.Format("<a href='{0}'>{1}</a>", url, CHelper.GetResFileString(obj.Properties[mc.TitleFieldName].Value.ToString()));
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
