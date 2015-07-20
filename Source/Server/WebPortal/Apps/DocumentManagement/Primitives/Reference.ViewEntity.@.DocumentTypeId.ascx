<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.DocumentManagement.Primitives.Reference_ViewEntity_Any_DocumentTypeId" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.IBN.Business.Documents" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = "";

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			string typeId = DataItem[FieldName].ToString();
			EntityObject eo = BusinessManager.Load(DocumentTypeEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(typeId));
			if (eo != null)
			{
				MetaClass mcType = MetaDataWrapper.GetMetaClassByName(eo["Name"].ToString());
				if(mcType != null)
					retVal = CHelper.GetResFileString(mcType.FriendlyName);
			}
		}
		return retVal;
	}
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
	 	<td valign='top'>
		  <%# GetValue(DataItem, FieldName) %>
		</td>
	</tr>
</table>