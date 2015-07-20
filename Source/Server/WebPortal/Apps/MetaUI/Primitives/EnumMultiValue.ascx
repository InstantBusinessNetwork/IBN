<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.EnumMultiValue" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
        string retVal = "";
        
        if (DataItem != null)
        {

          MetaFieldType type = DataItem.Properties[FieldName].GetMetaType().GetMetaType();

          int[] idList = (int[])DataItem.Properties[FieldName].Value;
          foreach (int id in idList)
          {
            if (retVal != String.Empty)
              retVal += "<br />";
            retVal += MetaEnum.GetFriendlyName(type, id);
          }
        }
        return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>