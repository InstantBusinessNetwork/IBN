<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowInstanceDesign.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.BusinessProcess.Modules.WorkFlowInstanceDesign" %>
<%@ Register Assembly="Mediachase.Ibn.Assignments.UI" Namespace="Mediachase.Ibn.Assignments.UI" TagPrefix="McUi" %>
<%@ Register Src="~/Apps/MetaUI/Toolbar/MetaToolbar.ascx" TagPrefix="mc" TagName="Toolbar" %>
<script type="text/javascript">
	function LayoutResizeHandler(sender, args)
	{
	}
</script>
<div style="margin: 5px;" class="ibn-stylebox2 ibn-propertysheet">
<mc:Toolbar runat="server" ID="ctrlToolbar" ClassName="WorkflowInstance" ToolbarMode="ListViewUI" />
<div class="ibn-light ibn-underline">
<table style="width: 100%; padding: 10px;">
	<tr>
		<td style="width: 66%;"><b><asp:Literal ID="ltName" runat="server" Text='<%$Resources : IbnFramework.BusinessProcess, Name%>' />:</b> <%= this.wfName %></td>
		<td style="width: 33%;"><b><asp:Literal ID="ltState" runat="server" Text='<%$Resources : IbnFramework.BusinessProcess, State%>' />:</b> <%= this.wfState %></td>
	</tr>
</table>
</div>
<asp:UpdatePanel runat="server" ID="wfPanel" UpdateMode="Always">
	<ContentTemplate>
		<McUi:WorkflowBuilder runat="server" ID="ctrlWFBuilder" />
	</ContentTemplate>
</asp:UpdatePanel>
</div>
