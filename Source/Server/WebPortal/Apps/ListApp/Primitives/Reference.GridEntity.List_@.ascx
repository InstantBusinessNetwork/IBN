<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.ListApp.Primitives.Reference_GridEntity_ListAll"%>
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
			MetaClass mc = MetaDataWrapper.GetMetaClassByName(sReferencedClass);
			if (Mediachase.Ibn.Lists.ListManager.MetaClassIsList(sReferencedClass))
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_ListApp_ViewReference");
				if (cp.CommandArguments == null)
					cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("className", sReferencedClass);
				if (obj.PrimaryKeyId.HasValue)
					cp.AddCommandArgument("primaryKeyId", obj.PrimaryKeyId.Value.ToString());
				else
					cp.AddCommandArgument("primaryKeyId", Mediachase.Ibn.Core.MetaViewGroupUtil.keyValueNotDefined);

				string command = cm.AddCommand("List_@", "", "EntityList", cp);
				command = command.Replace("\"", "&quot;");
				
				retVal = String.Format(System.Globalization.CultureInfo.InvariantCulture, "<a href=\"javascript:{{{0}}}\">{1}</a>", command, CHelper.GetResFileString(obj.Properties[mc.TitleFieldName].Value.ToString()));
			}
			else
			{
				string sUrl = ResolveClientUrl(CHelper.GetLinkEntityView_Edit(obj.MetaClassName, obj.PrimaryKeyId.Value.ToString()));

				retVal = String.Format("<a href='{0}'>{1}</a>", sUrl, obj.Properties[mc.TitleFieldName].Value.ToString());
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
