<%@ Page Language="c#" Inherits="Mediachase.UI.Web.Incidents.AddRelFromClip" CodeBehind="AddRelFromClip.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
	
	<title><%=LocRM.GetString("Clipboard")%></title>
</head>
<body>
	<form id="Form1" method="post" runat="server">

	<script type="text/javascript">
		//<![CDATA[
		function AddIssue(IssueId)
		{
			document.forms[0].<%=hdnIssueId.ClientID %>.value = IssueId;
			<%=Page.ClientScript.GetPostBackEventReference(lbAddIssue,"") %>
		}
		//]]>
	</script>

	<div style="overflow: auto; height: 170px">
		<asp:DataGrid runat="server" ID="dgIssues" ShowHeader="False" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" CellPadding="3" GridLines="None" CellSpacing="0" BorderWidth="0px" Width="100%">
			<Columns>
				<asp:BoundColumn DataField="IncidentId" Visible="False"></asp:BoundColumn>
				<asp:TemplateColumn>
					<ItemStyle CssClass="ibn-vb2"></ItemStyle>
					<ItemTemplate>
						<a title='<%=LocRM2.GetString("tPasteFromClipboard")%>' href='<%# "javascript:AddIssue(" + DataBinder.Eval(Container.DataItem, "IncidentId").ToString() + ")" %>'>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitleWL((int)DataBinder.Eval(Container.DataItem, "IncidentId"))%></a>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
		<asp:Label ID="lblAlert" CssClass="ibn-alerttext" runat="server"></asp:Label>
	</div>
	<input id="hdnIssueId" type="hidden" runat="server" name="hdnIssueId">
	<asp:LinkButton ID="lbAddIssue" runat="server" Visible="False"></asp:LinkButton>
	</form>
</body>
</html>
