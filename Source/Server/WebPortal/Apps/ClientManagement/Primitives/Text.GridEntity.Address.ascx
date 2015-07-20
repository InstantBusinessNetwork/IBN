<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ClientManagement.EntityPrimitives.Text_GridEntity_Address" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null)
		{
			retval = GetLinkForTitleField(DataItem, FieldName, DataItem[FieldName].ToString());
		}
		return retval;
	}

	private string GetLinkForTitleField(EntityObject mo, string fieldName, string text)
	{
		string retval = text;
		MetaClass mc = MetaDataWrapper.GetMetaClassByName(mo.MetaClassName);
		if (mc.TitleFieldName == fieldName)
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_Address_ViewItem");
			
			if (cp.CommandArguments == null)
				cp.CommandArguments = new Dictionary<string, string>();

			if (mo.PrimaryKeyId.HasValue)
				cp.AddCommandArgument("primaryKeyId", mo.PrimaryKeyId.Value.ToString());
			else
				cp.AddCommandArgument("primaryKeyId", Mediachase.Ibn.Core.MetaViewGroupUtil.keyValueNotDefined);

			string command = cm.AddCommand("Address", "AdditionalAddresses", "", cp);
			command = command.Replace("\"", "&quot;");
			retval = String.Format(CultureInfo.InvariantCulture, "<a href=\"javascript:{{{0}}}\">{1}</a>", command, text);
 		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>