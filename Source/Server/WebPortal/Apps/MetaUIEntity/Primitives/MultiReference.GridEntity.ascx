<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.MultiReference_GridEntity" %>
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
			MultiReferenceObject mro = (MultiReferenceObject)DataItem[FieldName];
			MetaClass mc = mro.ActiveReference;

			string sUrl = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(mc.Name, mro.ObjectId.Value.ToString()));
			retVal = String.Format("<a href='{0}'>{1}</a>", sUrl, mro.ObjectTitle);
		}
		return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>