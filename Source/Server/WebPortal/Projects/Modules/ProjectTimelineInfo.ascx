<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectTimelineInfo" Codebehind="ProjectTimelineInfo.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<ibn:blockheader id="secHeader" runat="server"></ibn:blockheader>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="4" width="100%">
	<tr>
		<td width="130px" align="right"><b><%= LocRM.GetString("target")%>:</b></td>
		<td class="ibn-value">
			<asp:Label Runat="server" ID="lblTargetStartDate" /> 
				<a href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.Project%> &ObjectId=<% =ProjectId%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderStart" ImageAlign="AbsMiddle"></asp:Image></a> - 
			<asp:Label Runat="server" ID="lblTargetFinishDate" /> 
				<a href="javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId=<% =(int)Mediachase.IBN.Business.ObjectTypes.Project%> &ObjectId=<% =ProjectId%>', 420, 150)"><asp:Image ImageUrl="~/Layouts/Images/reminder.gif" Width="16" Height="16" Runat="server" ID="imgReminderFinish" ImageAlign="AbsMiddle"></asp:Image></a>
		</td>
	</tr>
	<tr>
		<td align="right"><b><%= LocRM.GetString("actual")%>:</b></td>
		<td class="ibn-value">
			<asp:Label Runat="server" ID="lblActualStartDate" /> - <asp:Label Runat="server" ID="lblActualFinishDate" />
		</td>
	</tr>
	<tr style="padding-bottom:10">
		<td align="right"><b><%= LocRM.GetString("calculated")%>:</b></td>
		<td class="ibn-value">
			<asp:Label Runat="server" ID="lblCalculatedStartDate" /> - <asp:Label Runat="server" ID="lblCalculatedFinishDate" />
		</td>
	</tr>
</table>

