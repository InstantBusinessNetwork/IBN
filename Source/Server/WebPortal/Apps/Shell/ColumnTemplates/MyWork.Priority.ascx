<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.Shell.ColumnTemplates.MyWork_Priority" %>
<script language="c#" runat="server">
	protected string GetValue(object priorityObject, object typeObject, object dataItem)
	{
		System.Data.DataRow row = ((System.Data.DataRowView)dataItem).Row;
			
		string retval = string.Empty;
		if (priorityObject != DBNull.Value && typeObject != DBNull.Value)
		{
			if ((int)typeObject == (int)Mediachase.IBN.Business.Calendar.TimeType.WorkPinnedUp)
			{
				retval = String.Format(System.Globalization.CultureInfo.InvariantCulture,
					"<span style='font-size:8px;'>{0}</span>{1}",
					row.Table.Rows.IndexOf(row) + 1,
					Mediachase.Ibn.Web.UI.CHelper.GetIcon(Page.ResolveUrl("~/Images/IbnFramework/Pushpin-Yellow.png"), GetGlobalResourceObject("IbnFramework.Calendar", "PinnedUp").ToString()));
			}
			else
			{
				retval = Mediachase.UI.Web.Util.CommonHelper.GetPriorityIcon((int)priorityObject, this.Page);
			}
		}
		return retval;
	}
</script>
<%# GetValue(Eval("PriorityId"), Eval("Type"), DataBinder.GetDataItem(Container))%>