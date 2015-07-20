<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LegendControl.ascx.cs" Inherits="Mediachase.UI.Web.Common.Modules.LegendControl" %>
<table class="text" cellpadding="0" cellspacing="0" align="center">
	<tr>
		<td style="padding-right:5px;">
			<asp:Literal runat="server" ID="Literal1" Text="<%$Resources: IbnFramework.Calendar, tLegend %>"></asp:Literal>:
		</td>
		<td>
			<table cellpadding="4" cellspacing="0" style='<%= (ShowLeftBorder ? "border-left: solid 1px #999999;" : "") + (ShowRightBorder ? "border-right: solid 1px #999999;" : "") + (ShowTopBorder ? "border-top: solid 1px #999999;" : "") + (ShowBottomBorder ? "border-bottom: solid 1px #999999;" : "")%>' class="text">
				<tr>
					<td style="border-right: solid 1px #999999; color:#003399; background-color:#E9FEEC;"><asp:Literal runat="server" ID="Literal2" Text="<%$Resources: IbnFramework.Calendar, StateUpcoming %>"></asp:Literal></td>
					<td style="border-right: solid 1px #999999; color:#003399;"><asp:Literal runat="server" ID="Literal3" Text="<%$Resources: IbnFramework.Calendar, StateActive %>"></asp:Literal></td>
					<td style="border-right: solid 1px #999999; color:red;"><asp:Literal runat="server" ID="Literal4" Text="<%$Resources: IbnFramework.Calendar, StateOverdue %>"></asp:Literal></td>
					<td style="border-right: solid 1px #999999; color:#003399; background-color:#F2F2F2;"><asp:Literal runat="server" ID="Literal5" Text="<%$Resources: IbnFramework.Calendar, StateSuspended %>"></asp:Literal></td>
					<td style="color:#999999;text-decoration:line-through; background-color:#F2F2F2;"><span style="color:#003399;"><asp:Literal runat="server" ID="Literal6" Text="<%$Resources: IbnFramework.Calendar, StateCompleted %>"></asp:Literal></span></td>
				</tr>
			</table>
		</td>
	</tr>
</table>

