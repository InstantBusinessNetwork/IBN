<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Text_Grid_All_All_PrimaryKey" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<script language="c#" runat="server">
    protected string GetValue(MetaObject DataItem, string FieldName)
    {
		string retVal = "";

		if (DataItem == null)
		{
			retVal = "null";
		}
		else
		{
			if (DataItem.PrimaryKeyId == null)
			{
				if (DataItem.Properties["ParentBlockId"] == null || DataItem.Properties["ParentBlockId"].OriginalValue == null)
					retVal = "0";
				else
					retVal = "-" + DataItem.Properties["ParentBlockId"].OriginalValue.ToString();
			}
			else
			{
				retVal = DataItem.PrimaryKeyId.ToString();
			}
		}

        return retVal;
    }
</script>
<%# GetValue(DataItem, FieldName) %>