<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Projects.AddRelFromClip" CodeBehind="AddRelFromClip.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	
	<title><%=LocRM.GetString("Clipboard")%></title>
</head>
<body>

	<script type="text/javascript">
		//<![CDATA[
		function AddProject(ProjectId)
		{
			document.forms[0].<%=hdnProjectId.ClientID %>.value = ProjectId;
			<%=Page.ClientScript.GetPostBackEventReference(lbAddProject,"") %>
		}
		//]]>
	</script>

	<form id="Form1" method="post" runat="server">
	<div style="overflow: auto; height: 170px">
		<asp:DataGrid runat="server" ID="dgPrjs" ShowHeader="False" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="0" BorderWidth="0px" Width="100%">
			<Columns>
				<asp:BoundColumn DataField="ProjectId" Visible="False"></asp:BoundColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					<ItemTemplate>
						<a title='<%=LocRM.GetString("tAddFromClip")%>' href='<%# "javascript:AddProject(" + DataBinder.Eval(Container.DataItem, "ProjectId").ToString() + ")" %>'>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetProjectStatusWL((int)DataBinder.Eval(Container.DataItem, "ProjectId"))%></a>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
		<asp:Label ID="lblAlert" CssClass="ibn-alerttext" runat="server"></asp:Label>
	</div>
	<input id="hdnProjectId" type="hidden" runat="server" />
	<asp:LinkButton ID="lbAddProject" runat="server" Visible="False" OnClick="Add_Click"></asp:LinkButton>
	</form>
</body>
</html>
