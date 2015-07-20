<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.TimeTracking.ColumnTemplates.Text_Grid_WeekPeriod" %>
<%@ Import Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>
<script language="c#" runat="server">
	protected string GetValue(object dtStart, object wNum)
	{
		string retVal = "";
		if (dtStart != DBNull.Value)
			retVal = BindTitle(DateTime.Parse(dtStart.ToString(), CultureInfo.InvariantCulture), wNum.ToString());
		return retVal;
	}

	#region BindTitle
	/// <summary>
	/// Binds the action.
	/// </summary>
	string BindTitle(DateTime dtStart, string wNum)
	{
		string retVal = "";
		string commandName = "MC_TT_Week_GoTo";
		CommandParameters cp = new CommandParameters(commandName);
		cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
		cp.AddCommandArgument("primaryKeyId", dtStart.ToString(CultureInfo.InvariantCulture));
		bool isEnable = true;
		string clientScript = "";
		CommandManager cm = CommandManager.GetCurrent(this.Page);
		if (!String.IsNullOrEmpty(commandName) && cm != null)
			clientScript = cm.AddCommand("TimeTrackingWeek", "", "", cp, out isEnable);

		LinkButton lb = new LinkButton();
		lb.Style.Add("cursor", "pointer");
		lb.Text = String.Format("<b>#{0}</b>&nbsp;&nbsp;&nbsp;{1} - {2}", wNum, dtStart.ToString("dd MMM yyyy"), dtStart.AddDays(6).ToString("dd MMM yyyy"));
		lb.OnClientClick = string.Format("{0} return false;", clientScript);

		//LinkButton control -> toString()
		StringBuilder sb = new StringBuilder();
		StringWriter tw = new StringWriter(sb);
		HtmlTextWriter hw = new HtmlTextWriter(tw);
		lb.RenderControl(hw);
		if (isEnable)
			retVal += sb.ToString();

		return retVal;
	}
	#endregion
</script>
<%# GetValue(Eval("uid"), Eval("WeekNumber"))%>