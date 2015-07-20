<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Reference_Grid_All_All_Export" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName].Value!=null)
		{
			string sReferencedClass = DataItem.Properties[FieldName].GetMetaType().Attributes[McDataTypeAttribute.ReferenceMetaClassName].ToString();
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);

			MetaObject obj = new MetaObject(mc, (PrimaryKeyId)DataItem.Properties[FieldName].Value);

			//DV: fix bug with no reference in excel report (09-07-2008)
			retVal = obj.Properties[mc.TitleFieldName].Value.ToString();
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
