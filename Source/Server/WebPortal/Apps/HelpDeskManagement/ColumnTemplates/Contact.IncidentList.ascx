<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.HelpDeskManagement.ColumnTemplates.Contact_IncidentList" %>
<%@ Import Namespace="Mediachase.Ibn.Data" %>
<%# GetClientLink(Eval("OrgUid"), Eval("ContactUid"), Eval("ClientName"))%>

<script type="text/C#" runat="server">
protected string GetClientLink(object OrgUid, object ContactUid, object ClientName)
{
	string retVal = "";
	if (OrgUid != DBNull.Value && PrimaryKeyId.Parse(OrgUid.ToString()) != PrimaryKeyId.Empty)
		retVal = Mediachase.UI.Web.Util.CommonHelper.GetOrganizationLink(this.Page, PrimaryKeyId.Parse(OrgUid.ToString()), ClientName.ToString());
	else if (ContactUid != DBNull.Value && PrimaryKeyId.Parse(ContactUid.ToString()) != PrimaryKeyId.Empty)
		retVal = Mediachase.UI.Web.Util.CommonHelper.GetContactLink(this.Page, PrimaryKeyId.Parse(ContactUid.ToString()), ClientName.ToString());
	return retVal;
}
</script>