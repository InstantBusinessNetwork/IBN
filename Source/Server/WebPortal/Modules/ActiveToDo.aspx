<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Modules.ActiveToDo" CodeBehind="ActiveToDo.aspx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="dg" Namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title>Active ToDo</title>
</head>
<body class="UserBackground" id="pT_body" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0">
	<form id="frmMain" runat="server">
	<table cellpadding="0" cellspacing="0" width="100%" height="100%" style="margin-top: 0; background-color: #E1ECFC">
		<tr>
			<td height="100%" style="padding: 5px" valign="top">
				<dg:DataGridExtended ID="dgActivities" runat="server" AllowPaging="True" PageSize="10" AllowSorting="True" CellPadding="1" GridLines="None" CellSpacing="0" BorderWidth="0px" AutoGenerateColumns="False" Width="100%">
					<Columns>
						<asp:TemplateColumn SortExpression="Title">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%#
							Mediachase.UI.Web.Util.CommonHelper.GetTaskToDoLink 
							(
								(int)DataBinder.Eval(Container.DataItem, "ItemId"),
								(int)DataBinder.Eval(Container.DataItem, "IsToDo"),
								DataBinder.Eval(Container.DataItem, "Title").ToString(),
								(int)DataBinder.Eval(Container.DataItem, "StateId"),
								GetTarget()
							)
								%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="ProjectTitle">
							<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2"></ItemStyle>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem, "ProjectTitle")%>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="StartDate">
							<HeaderStyle CssClass="ibn-vh2" Width="95px"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2" Width="95px"></ItemStyle>
							<ItemTemplate>
								<span runat="server" visible='<%#(bool)DataBinder.Eval(Container.DataItem,"ShowStartDate")%>' id="Span3">
									<%# ((DateTime)DataBinder.Eval(Container.DataItem,"StartDate")).ToShortDateString()%></span>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn SortExpression="FinishDate">
							<HeaderStyle CssClass="ibn-vh2" Width="80px"></HeaderStyle>
							<ItemStyle CssClass="ibn-vb2" Width="80px"></ItemStyle>
							<ItemTemplate>
								<span runat="server" visible='<%#(bool)DataBinder.Eval(Container.DataItem,"ShowFinishDate")%>' id="Span4">
									<%# ((DateTime)DataBinder.Eval(Container.DataItem,"FinishDate")).ToShortDateString()%></span>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</dg:DataGridExtended>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
