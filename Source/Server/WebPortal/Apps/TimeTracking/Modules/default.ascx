<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="default.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules._default" %>

<table cellpadding="0" cellspacing="0" border="0" width="100%" class="text">
	<tr>
		<td valign="top" style="width:40px;"><img alt="" src='<%=Page.ResolveUrl("~/layouts/images/listset.gif")%>' /></td>
		<td valign="top">
			<table cellspacing="0" cellpadding="4" border="0" class="text">
				<tr>
					<td valign="top">
						<asp:Literal runat="server" ID="HeaderText" Text="<%$Resources : IbnFramework.TimeTracking, TimeTrackingMenuHeader%>"></asp:Literal>
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /> 
						<a href='<%=Page.ResolveUrl("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class=TimeTrackingBlock") %>'><asp:Literal runat="server" ID="Literal1" Text="<%$Resources : IbnFramework.TimeTracking, TTBlock%>"></asp:Literal></a>
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /> 
						<a href='<%=Page.ResolveUrl("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class=TimeTrackingEntry") %>'><asp:Literal runat="server" ID="Literal2" Text="<%$Resources : IbnFramework.TimeTracking, TTEntry%>"></asp:Literal></a>
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /> 
						<a href='<%=Page.ResolveUrl("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class=TimeTrackingBlockTypeInstance") %>'><asp:Literal runat="server" ID="Literal3" Text="<%$Resources : IbnFramework.TimeTracking, TTBlockTypeInstance%>"></asp:Literal></a>
					</td>
				</tr>
				<tr>
					<td>
						<img alt="" src='<%=Page.ResolveUrl("~/layouts/images/rect.gif")%>' /> 
						<a href='<%=Page.ResolveUrl("~/Apps/TimeTracking/Pages/Admin/TimeTrackingBlockTypeList.aspx") %>'><asp:Literal runat="server" ID="Literal4" Text="<%$Resources : IbnFramework.TimeTracking, BlockTypes%>"></asp:Literal></a>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>