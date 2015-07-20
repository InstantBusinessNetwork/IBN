<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.ListApp.Primitives.Text_GridEntity_List_All" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.Controls.Util" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retval = String.Empty;
		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem.Properties[FieldName].Value != null)
		{
			retval = GetLinkForTitleField(DataItem, FieldName, DataItem.Properties[FieldName].Value.ToString());
		}
		return retval;
	}

	private string GetLinkForTitleField(EntityObject mo, string fieldName, string text)
	{
		string retval = text;
		MetaClass mc = MetaDataWrapper.GetMetaClassByName(mo.MetaClassName);
		if (mc.TitleFieldName == fieldName || (mc.CardOwner != null && mc.CardOwner.TitleFieldName == fieldName))
		{
			try
			{
				if (this.Parent.Parent.Parent.Parent.Parent != null &&
					this.Parent.Parent.Parent.Parent.Parent is Mediachase.Ibn.Web.UI.EntityGrid &&
					((Mediachase.Ibn.Web.UI.EntityGrid)this.Parent.Parent.Parent.Parent.Parent).PlaceName == "EntitySelect")
					return retval;
			}
			catch //if this is rss
			{
				return retval;
			}
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_ListApp_ViewItem");
			if (cp.CommandArguments == null)
				cp.CommandArguments = new Dictionary<string, string>();

			if (mo.PrimaryKeyId.HasValue)
				cp.AddCommandArgument("primaryKeyId", mo.PrimaryKeyId.Value.ToString());
			else
				cp.AddCommandArgument("primaryKeyId", Mediachase.Ibn.Core.MetaViewGroupUtil.keyValueNotDefined);
			cp.AddCommandArgument("className", mc.Name);

			string command = cm.AddCommand(mc.Name, "", "EntityList", cp);
			command = command.Replace("\"", "&quot;");
			retval = String.Format(CultureInfo.InvariantCulture, "<a href=\"javascript:{{{0}}}\">{1}</a>", command, text);
		}
		return retval;
	}
</script>
<%# GetValue(DataItem, FieldName) %>