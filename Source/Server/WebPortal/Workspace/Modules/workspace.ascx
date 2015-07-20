<%@ Reference Control="~/Modules/PageTemplateNew.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Workspace.Modules.workspace" Codebehind="workspace.ascx.cs" %>
<%@ Register Src="~/Apps/WidgetEngine/Modules/WsViewer.ascx" TagPrefix="mc" TagName="WsViewer2" %>
<%--<table align=center width=700 cellpadding=3 cellspacing=0 class="ibn-alerttext" id="tblWarning" style="display:none">
	<tr>
		<td width=20 align=center valign=center>
			<asp:Image ID="imgCaution" Runat=server ImageUrl="~/Layouts/Images/warning.gif" Width=16 Height=16 ImageAlign="AbsMiddle"></asp:Image>
		</td>
		<td><%=LocRM.GetString("WizardWarning") %>&nbsp;<a href="#" id="aWizardLink"></a>
		</td>
	</tr>
</table>
<asp:PlaceHolder ID="phItems" Runat="server" />
--%>
<mc:WsViewer2 runat="server" ID="ctrlViewer" PageUid="57399bd7-2588-4626-a85b-81ba58205246"/>

