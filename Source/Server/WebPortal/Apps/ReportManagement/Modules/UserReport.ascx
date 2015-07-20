<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserReport.ascx.cs" Inherits="Mediachase.UI.Web.Apps.ReportManagement.Modules.UserReport" %>
<%@ Register Src="~/Apps/ReportManagement/Modules/UserReportPersonal.ascx" TagName="Personal" TagPrefix="mc" %>
<%@ Register Src="~/Apps/ReportManagement/Modules/UserReportGeneral.ascx" TagName="General" TagPrefix="mc" %>
<%@ Register Src="~/Apps/ReportManagement/Modules/UserReportCustom.ascx" TagName="Custom" TagPrefix="mc" %>
<script type="text/javascript">
function OpenWindow(query, w, h, scroll) {
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;

	winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l;
	if (scroll) winprops += ',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
</script>
<div style="width: 48%; float: left;">
	<mc:Personal runat="server" ID="ctrlUserPersonal" />
</div>
<div style="width: 48%; float: left;">
	<mc:General runat="server" ID="ctrlUserGeneral" />
	<mc:Custom runat="server" ID="ctrlUserCustom" />
</div>
