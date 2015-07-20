<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GanttView2.ascx.cs" Inherits="Mediachase.UI.Web.Projects.Modules.GanttView2" %>
<%@ Reference Control="~/Projects/Modules/GanttChart2.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Gantt" src="~/Projects/Modules/GanttChart2.ascx" %>
<script type="text/javascript">
function closeLegendMenu()
{
	document.getElementById('divLegendMenu').style.display = "none";
}

function GetTotalOffset(eSrc)
{
	this.Top = 0;
	this.Left = 0;
	while (eSrc)
	{
		this.Top += eSrc.offsetTop;
		this.Left += eSrc.offsetLeft;
		eSrc = eSrc.offsetParent;
	}
	return this;
}

function ShowLegend(objId)
{
	menu = document.getElementById('divLegendMenu');
	var curObj = document.getElementById(objId);
	var off = GetTotalOffset(curObj);
	menu.style.left = (off.Left - 340).toString() + "px";
	menu.style.top = (off.Top + 20).toString() + "px";
	menu.style.display = "";
}
</script>
<table class="ibn-navline ibn-alternating" cellpadding="4" cellspacing="2" width="100%" border="0">
	<tr>
		<td width="80" id="tdProject1" runat="server" class="text">
			<%=LocRM.GetString("Project") %>:
		</td>
		<td align="left" id="tdProject2" runat="server">
			<asp:DropDownList id="ddProject" runat="server" CssClass="text" Width="120px" AutoPostBack="True" onselectedindexchanged="ddProjects_IndexChange" />
		</td>
	</tr>
</table>
<ibn:Gantt runat="server" id="ctrlGanttChart"></ibn:Gantt>
<div id="divLegendMenu" style="position:absolute; width:330px; border: solid 1px #333333; display:none;background-color:#ffffff;" class="ibn-rtetoolbarmenu ibn-propertysheet ibn-selectedtitle">
<img alt="" src="../../Layouts/images/deny_black.gif" width="15" height="15" border="0" align="right" hspace="0" vspace="0" style="cursor:pointer;" class="borderWhite" onclick="closeLegendMenu()" runat="server" id="imgClose" />
<asp:Table ID="tblLegend" runat="server" CellPadding="4" CellSpacing="0" CssClass="text">
</asp:Table>
</div>
<asp:LinkButton ID="lbChangeView" runat="server" Visible="false" OnClick="lbChangeView_Click"></asp:LinkButton>