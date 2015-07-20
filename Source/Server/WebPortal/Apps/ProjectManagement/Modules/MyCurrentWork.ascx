<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyCurrentWork.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ProjectManagement.Modules.MyCurrentWork" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/ProjectManagement/Modules/ResourceWorkControl.ascx" %>
<%@ Reference Control="~/Projects/Modules/ResourceUtilGraphControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="ResourceWorkControl" Src="~/Apps/ProjectManagement/Modules/ResourceWorkControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="GraphControl" Src="~/Projects/Modules/ResourceUtilGraphControl.ascx" %>
<%@ Register TagPrefix="mc2" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>

<script type="text/javascript">
function LayoutResizeHandler(sender, eventArgs)
{
	
}
</script>
<mc2:McDock ID="DockTop" runat="server" Anchor="top" EnableSplitter="False" DefaultSize="73">
	<DockItems>
		<table class="text" style="margin-top: 0px;" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
				<td>
					<ibn:BlockHeader ID="secHeader" runat="server" Title="<%$ Resources: IbnFramework.Project, CurrentWork %>"></ibn:BlockHeader>
				</td>
			</tr>
			<tr>
				<td>
					<ibn:GraphControl runat="server" id="GraphControlMain" ShowUsers="false"></ibn:GraphControl>
				</td>
			</tr>
		</table>
	</DockItems>
</mc2:McDock>

<table style="margin-top:0px; padding-top: 0px; table-layout: fixed;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-propertysheet">
	<tr>
		<td style="padding-left: 5px;" class="filter">
			<ibn:ResourceWorkControl runat="server" ID="ResourceWorkCtrl" />
		</td>
	</tr>
</table>
