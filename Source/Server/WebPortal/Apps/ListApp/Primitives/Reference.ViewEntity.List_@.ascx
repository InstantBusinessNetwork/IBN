<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.ListApp.Primitives.Reference_ViewEntity_ListAll" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			MetaField field = FormController.GetMetaField(DataItem.MetaClassName, FieldName);
			string sReferencedClass = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName);

			EntityObject obj = BusinessManager.Load(sReferencedClass, (PrimaryKeyId)DataItem[FieldName]);

			string url = ResolveClientUrl("~/Apps/MetaUIEntity/Pages/EntityViewPopup.aspx?formName=[MC_GeneralViewForm]&className=" + sReferencedClass + "&ObjectId=" + obj.PrimaryKeyId.Value);
			
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);
			retVal = String.Format("<span class='ibn-propertysheet'><a href='{0}'>{1}</a></span>", url, obj.Properties[mc.TitleFieldName].Value.ToString());
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>