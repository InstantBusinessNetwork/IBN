<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutActivities.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.OutActivities" %>
<%@ Reference Control="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Register TagPrefix="mc" TagName="BlockHeader2" Src="~/Apps/Common/Design/BlockHeader2.ascx" %>
<%@ Reference Control="~/Apps/Shell/Modules/OutActivities.ascx" %>
<%@ Register TagPrefix="mc" TagName="OutActivities" Src="~/Apps/Shell/Modules/OutActivities.ascx" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" border="0" width="100%">
	<tr>
		<td>
			<mc:BlockHeader2 runat="server" ID="secHeader" />
		</td>
	</tr>
	<tr>
		<td>
			<mc:OutActivities ID="ctrlOut" runat="server" />
		</td>
	</tr>
</table>