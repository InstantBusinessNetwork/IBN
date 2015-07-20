<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.ProjectManagement.ColumnTemplates.OverallStatus_ProjectList" %>
<table cellspacing='0' cellpadding='0' style='<%# Eval("PercentCompleted") == DBNull.Value ? "display:none;": ""%>'>
	<tr>
		<td>
			<div class='progress'>
				<img alt='' src='<%# Page.ResolveUrl("~/Layouts/Images/point.gif") %>' width='<%# Eval("PercentCompleted")%>%' />
			</div>
		</td>
		<td><%# Eval("PercentCompleted")%>%</td>
	</tr>
</table>
