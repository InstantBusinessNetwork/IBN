<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.UI.Web.ProjectManagement.ColumnTemplates.ResourceWork_Period" %>
<%@ Import Namespace="Mediachase.IBN.Business" %>
<script language="c#" runat="server">
	protected string GetValue(object start)
	{
		string retval = string.Empty;
		if (start != DBNull.Value)
		{
			DateTime dt = ((DateTime)start).Date;
			DateTime userDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;

			if (dt == userDate)
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "Today").ToString();
			else if (dt == userDate.AddDays(1))
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "Tomorrow").ToString();
			else if (dt == userDate.AddDays(2))
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "AfterTomorrow").ToString();
			else if (Iso8601WeekNumber.GetWeekNumber(dt) == Iso8601WeekNumber.GetWeekNumber(userDate))
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "ThisWeek").ToString();
			else if (Iso8601WeekNumber.GetWeekNumber(dt) == Iso8601WeekNumber.GetWeekNumber(userDate.AddDays(7)))
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "NextWeek").ToString();
			else if (dt.Month == userDate.Month)
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "ThisMonth").ToString();
			else if (dt.Month == userDate.AddMonths(1).Month)
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "NextMonth").ToString();
			else if (dt.Year == userDate.Year)
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "ThisYear").ToString();
			else
				retval = GetGlobalResourceObject("IbnFramework.Calendar", "NextYear").ToString();
		}
		return retval;
	}
</script>
<%# GetValue(Eval("Start"))%>
