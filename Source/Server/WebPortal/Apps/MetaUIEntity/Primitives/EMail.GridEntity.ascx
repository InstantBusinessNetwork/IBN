<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.EMail_GridEntity" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = String.Empty;

		if (DataItem != null && DataItem.Properties[FieldName] != null && 
			DataItem[FieldName] != null)
		{
			string str = CHelper.ParseText(DataItem[FieldName].ToString());
			retVal = String.Format("<a href='mailto:{0}'>{0}</a>", str);
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>