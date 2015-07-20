<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.ProjectManagement.ColumnTemplates.ResourceWork_GeneralCategories" %>
<%@ Import Namespace="System.Data" %>
<script language="c#" runat="server">
	protected string GetValue(object objectTypeId, object objectId)
	{
		string retval = string.Empty;
		if (objectTypeId != DBNull.Value && objectId != DBNull.Value)
		{
			using (IDataReader reader = Mediachase.IBN.Business.Common.GetListCategoriesByObject((int)objectTypeId, (int)objectId))
			{
				while (reader.Read())
				{
					if (!String.IsNullOrEmpty(retval))
						retval += "<br/>";
					retval += reader["CategoryName"].ToString();
				}
			}
		}
		return retval;
	}
</script>
<%# GetValue(Eval("ObjectTypeId"), Eval("ObjectId"))%>