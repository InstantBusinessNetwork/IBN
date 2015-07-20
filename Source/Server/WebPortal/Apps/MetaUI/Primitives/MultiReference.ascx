<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.MultiReference" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName].Value != null)
		{
			MultiReferenceObject mro = (MultiReferenceObject)DataItem.Properties[FieldName].Value;
			MetaClass mc = mro.ActiveReference;

			string sUrl = ResolveClientUrl(CHelper.GetLinkObjectView_Edit(mc.Name, mro.ObjectId.Value.ToString()));
			retVal = String.Format("<a href='{0}'>{1}</a>", sUrl, mro.ObjectTitle);
		}
		return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>