<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType"
	ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.EnumMultiValue_GridEntity" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null)
		{
			MetaField field = MetaDataWrapper.GetMetaFieldByName(DataItem.MetaClassName, FieldName);

			MetaFieldType type = field.GetMetaType();

			int[] idList = (int[])DataItem[FieldName];
			foreach (int id in idList)
			{
				if (retVal != String.Empty)
					retVal += "<br />";
				retVal += CHelper.GetResFileString(MetaEnum.GetFriendlyName(type, id));
			}
		}
		return retVal;
	}
</script>

<%# GetValue(DataItem, FieldName) %>
