<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Ibn.Web.UI.Controls.Util.BaseType" ClassName="Mediachase.Ibn.Web.UI.MetaUI.Primitives.Image_View" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Mediachase.Ibn.Core" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta" %>
<%@ Import Namespace="Mediachase.Ibn.Data.Meta.Management" %>
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
				MetaField mf = prop.GetMetaType();

				string border = String.Empty;
				if ((bool)mf.Attributes[McDataTypeAttribute.ImageShowBorder])
					border = " border=1";

				<%--retVal = String.Format(CultureInfo.InvariantCulture,
					"<img src='{0}?FileUID={1}&mode=image'{2}/>",
					ResolveClientUrl("~/Apps/MetaUI/Pages/Public/DownloadFile.aspx"),
					fi.FileUID.ToString(), border);--%>
					retVal = String.Format(CultureInfo.InvariantCulture,"<img src='{0}'{1}/>", 
				                       WebDavUrlBuilder.GetMetaDataWebDavUrl(fi.FileUID, true), border);
			}
		}
		return retVal;
	}
</script>
<%# GetValue(DataItem, FieldName) %>
