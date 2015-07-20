<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Url" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = String.Empty;
		
		if (DataItem != null)
		{
			MetaObjectProperty property = DataItem.Properties[FieldName];
			if (property != null && property.Value != null)
			{
				string txt = CHelper.ParseText(DataItem.Properties[FieldName].Value.ToString().Trim());
				string url = txt;
				if (!(url.IndexOf("://") >= 0 || url.IndexOf("\\\\") >= 0))
				{
					url = "http://" + url;
				}

				MetaField mf = property.GetMetaType();
				if (mf.IsReferencedField)
				{
					string refClassName = mf.Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
					string refFieldName = mf.Attributes[McDataTypeAttribute.ReferencedFieldMetaFieldName].ToString();
					mf = MetaDataWrapper.GetMetaFieldByName(refClassName, refFieldName);
				}
				
				retVal = String.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\" target=\"{1}\">{2}</a>", url, mf.Attributes[McDataTypeAttribute.StringUrlTarget].ToString(), txt);
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>

