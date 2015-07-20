<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseEntityType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.EntityPrimitives.Image_GridEntity"%>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Core.Business" %>
<%@ Import Namespace="Mediachase.IBN.Business.WebDAV.Common" %>
<script language="c#" runat="server">
	protected string GetValue(EntityObject DataItem, string FieldName)
	{
		string retVal = String.Empty;

		if (DataItem != null && DataItem.Properties[FieldName] != null && DataItem[FieldName] != null) 
		{
			FileInfo fi = (FileInfo)DataItem[FieldName];

			<%--retVal = String.Format(CultureInfo.InvariantCulture,
				"<a href='{0}?FileUID={1}&mode=image' target='_blank'>{2}</a>",
				ResolveClientUrl("~/Apps/MetaUI/Pages/Public/DownloadFile.aspx"),
				fi.FileUID.ToString(),
				fi.Name);--%>
				retVal = String.Format(CultureInfo.InvariantCulture,"<a href='{0}'{2}>{1}</a>", 
				                       WebDavUrlBuilder.GetMetaDataWebDavUrl(fi.FileUID, true), fi.Name,
				                       Mediachase.IBN.Business.Common.OpenFileInNewWindow(fi.Name) ? " target='_blank'" : "");
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>