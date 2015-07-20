<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Image"%>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.IBN.Business.WebDAV.Common" %>
<script language="c#" runat="server">
	protected string GetValue(MetaObject DataItem, string FieldName)
	{
		string retVal = String.Empty;
		
		if (DataItem != null) 
		{ 
			MetaObjectProperty prop = DataItem.Properties[FieldName];
			if (prop != null && prop.Value != null)
			{
				FileInfo fi = (FileInfo)prop.Value;

				<%--retVal = String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}?FileUID={1}&mode=image' target='_blank'>{2}</a>",
					ResolveClientUrl("~/Apps/MetaUI/Pages/Public/DownloadFile.aspx"),
					fi.FileUID.ToString(),
					fi.Name);--%>
					retVal = String.Format(CultureInfo.InvariantCulture,"<a href='{0}'>{1}</a>", 
				                       WebDavUrlBuilder.GetMetaDataWebDavUrl(fi.FileUID, true), fi.Name);
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>