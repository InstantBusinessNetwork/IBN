<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowList.aspx.cs" Inherits="Mediachase.Ibn.AssignmentsUI.Modules.Pages.WorkflowList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:LinkButton runat="server" ID="lblCreate" Text="New workflow" />
		<asp:GridView runat="server" ID="ctrlList" AutoGenerateColumns="false">
			<Columns>
				<asp:TemplateField>
					<HeaderTemplate>Workflow name</HeaderTemplate>
					<ItemTemplate><%# ((Mediachase.Ibn.Core.Business.EntityObject)Container.DataItem).Properties["Name"].Value%></ItemTemplate>
					<ItemStyle CssClass="padding5" />
				</asp:TemplateField>
				<asp:TemplateField>
					<HeaderTemplate>Actions</HeaderTemplate>
					<ItemTemplate>
						<asp:LinkButton runat="server" ID="lblRun" CommandName="Run" CommandArgument="<%# ((Mediachase.Ibn.Core.Business.EntityObject)Container.DataItem).PrimaryKeyId%>" Text="Run" />
						<asp:LinkButton runat="server" ID="lblEdit" CommandName="Edit" CommandArgument="<%# ((Mediachase.Ibn.Core.Business.EntityObject)Container.DataItem).PrimaryKeyId%>" Text="Edit" />
						<asp:LinkButton runat="server" ID="lblDelete" CommandName="Delete" CommandArgument="<%# ((Mediachase.Ibn.Core.Business.EntityObject)Container.DataItem).PrimaryKeyId%>" Text="Delete" OnClientClick="return confirm('Delete?');" />
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
    </div>
    </form>
</body>
</html>
