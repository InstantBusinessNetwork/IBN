<%@ Page Language="c#" Inherits="Mediachase.UI.Web.FileStorage.FileInfoView" CodeBehind="FileInfoView.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	
	<title><%=LocRM.GetString("DocInfo")%></title>
</head>
<body class="UserBackground" id="pT_body">
	<form id="frmMain" method="post" runat="server" enctype="multipart/form-data">
	<table cellpadding="0" cellspacing="0" border="0" width="100%">
		<tr>
			<td valign="top">
				<table cellspacing="0" cellpadding="3" border="0" class="text" width="100%">
					<tr>
						<td width="100px">
							<b>
								<%= LocRM.GetString("tName")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblName" runat="server" CssClass="ibn-vb2"></asp:Label>
						</td>
					</tr>
					<tr>
						<td valign="top">
							<b>
								<%= LocRM.GetString("tDescription")%>:</b>
						</td>
						<td colspan="3">
							<div style="overflow: auto; height: 28px">
								<asp:Label ID="lblDescription" runat="server" CssClass="text"></asp:Label>
							</div>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%= LocRM.GetString("tLocation")%>:</b>
						</td>
						<td colspan="3">
							<asp:Label ID="lblPath" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%= LocRM.GetString("tCreatedBy")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblCreator" runat="server" CssClass="text"></asp:Label>
						</td>
						<td width="120px">
							<b>
								<%= LocRM.GetString("tCreated")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblCreated" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
							<b>
								<%= LocRM.GetString("tUpdatedBy")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblModifier" runat="server" CssClass="text"></asp:Label>
						</td>
						<td>
							<b>
								<%= LocRM.GetString("tLastUpdate")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblModified" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
						</td>
						<td>
							<asp:Label ID="lblKeepHistory" CssClass="text" runat="server"></asp:Label>
						</td>
						<td>
							<b>
								<%= LocRM.GetString("tDownCount")%>:</b>
						</td>
						<td>
							<asp:Label ID="lblViewCount" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr height="20px" id="trManage" runat="server">
			<td bgcolor="#eaeaea" style="border-top: #bababa 1px solid; border-bottom: #bababa 1px solid; padding-left: 5">
				<table cellspacing="0" cellpadding="0" border="0" width="100%" class="text">
					<tr>
						<td>
							<b>
								<%= LocRM.GetString("tVerHistory")%></b>
						</td>
						<td align="right" style="padding-right: 5px" class="ibn-vb2">
							<asp:Label ID="lblAddVersion" runat="server" CssClass="text"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td valign="top" style="padding: 1,5,5,5">
				<asp:Label ForeColor="Red" ID="lblNoHistory" runat="server" CssClass="text"></asp:Label>
				<div style="overflow: auto; height: 150px">
					<asp:DataGrid runat="server" ID="dgFiles" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="0" BorderWidth="0px" Width="100%">
						<Columns>
							<asp:BoundColumn DataField="Id" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Name">
								<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
								<ItemTemplate>
									<%# DataBinder.Eval(Container.DataItem, "Name")%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
								<ItemTemplate>
									<%# DataBinder.Eval(Container.DataItem, "Modifier")%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
								<ItemTemplate>
									<%# ((DateTime)DataBinder.Eval(Container.DataItem, "Modified")).ToShortDateString() + " " + ((DateTime)DataBinder.Eval(Container.DataItem, "Modified")).ToShortTimeString()%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Size">
								<ItemStyle CssClass="ibn-vb2" Width="70" HorizontalAlign="Right"></ItemStyle>
								<HeaderStyle CssClass="ibn-vh-right"></HeaderStyle>
								<ItemTemplate>
									<%# DataBinder.Eval(Container.DataItem, "Size")%>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</div>
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
