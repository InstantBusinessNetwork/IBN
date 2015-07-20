<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.File"%>
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

				// ============ Hard-coded for 4.6
				// ToDo: Unify for common functionality
				int contentTypeId = Mediachase.IBN.Business.DSFile.GetContentTypeByFileName(fi.Name);
				string text = String.Format(CultureInfo.InvariantCulture,
					"<img src='{0}?IconID={1}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{2}",
					ResolveClientUrl("~/Common/ContentIcon.aspx"), contentTypeId, fi.Name);
				//================
				<%--retVal = String.Format(CultureInfo.InvariantCulture,
					"<a href='{0}?FileUID={1}'>{2}</a>",
					ResolveClientUrl("~/Apps/MetaUI/Pages/Public/DownloadFile.aspx"),
					fi.FileUID.ToString(),
					text);--%>
				string sLink = WebDavUrlBuilder.GetMetaDataWebDavUrl(fi.FileUID, true);
				string sNameLocked = Mediachase.UI.Web.Util.CommonHelper.GetLockerText(sLink);
					
				retVal = String.Format(CultureInfo.InvariantCulture,"<a href='{0}'{3}>{1}</a> {2}", 
						sLink, text, sNameLocked,
						Mediachase.IBN.Business.Common.OpenFileInNewWindow(fi.Name) ? " target='_blank'" : "");
					
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>