<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.DocumentGeneral2" Codebehind="DocumentGeneral2.ascx.cs" %>
<%@ Reference Control="~/Documents/Modules/DocumentsToDo.ascx" %>
<%@ Reference Control="~/Documents/Modules/QuickTracking.ascx" %>
<%@ Reference Control="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Reference Control="~/Apps/BusinessProcess/Modules/MyAssignmentsByObject.ascx" %>
<%@ Reference Control="~/Apps/BusinessProcess/Modules/ActiveAssignmentsByObject.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ToDo" src="DocumentsToDo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Comments" src="~/Common/Modules/LatestComments.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Docs" src="~/Common/Modules/LatestFiles.ascx" %>
<%@ Register TagPrefix="ibn" TagName="QuickTracking" src="QuickTracking.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DocResources" src="DocumentResources.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DocumentVersions" src="DocumentVersions.ascx" %>
<%@ Register TagPrefix="ibn" TagName="TimeTracking" src="~/Apps/TimeTracking/Modules/PublicControls/TimeTrackingInfo.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MyAssignments" src="~/Apps/BusinessProcess/Modules/MyAssignmentsByObject.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ActiveAssignments" src="~/Apps/BusinessProcess/Modules/ActiveAssignmentsByObject.ascx" %>

<table border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td valign="top">
			<ibn:MyAssignments id="MyAssignments" runat="server"></ibn:MyAssignments>
			<ibn:quicktracking id="QT" runat="server"></ibn:quicktracking>
		</td>
		<td style="width: 10px;">&nbsp;</td>
		<td valign="top" style="width: 50%;">
			<ibn:ActiveAssignments id="ActiveAssignments" runat="server"></ibn:ActiveAssignments>
			<ibn:DocResources id="ctrlDR" runat="server" />
			<ibn:todo id="ToDoList" runat="server"></ibn:todo>
			<ibn:DocumentVersions id="DocumentVersionList" runat="server" />
			<ibn:comments id="LatestCommentsTemplate" runat="server"></ibn:comments>
			<ibn:docs id="DocsList" runat="server"></ibn:docs>
			<ibn:TimeTracking runat="server" ID="ibnTimeTracking" />
		</td>
	</tr>
</table>