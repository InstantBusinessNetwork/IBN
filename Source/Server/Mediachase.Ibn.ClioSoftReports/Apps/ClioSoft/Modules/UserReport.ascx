<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserReport.ascx.cs" Inherits="Mediachase.Ibn.Apps.ClioSoft.Modules.UserReport" %>
<%@ Reference Control="~/Apps/ClioSoft/Modules/PickerControl.ascx" %>
<%@ Reference Control="~/Apps/ClioSoft/Modules/MultiSelectControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/ClioSoft/Modules/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="MultiSelect" Src="~/Apps/ClioSoft/Modules/MultiSelectControl.ascx" %>
<script type="text/javascript">
function collapse_expand(obj, key)
{
	var expand = true;
	var img = obj.cells[0].firstChild.firstChild;
	if (img && img.src && img.src.indexOf("minus.gif") > 0)
	{
		img.src = '<%= ResolveClientUrl("~/layouts/images/plus.gif")%>'
		expand = false;
	}
	else
	{
		img.src = '<%= ResolveClientUrl("~/layouts/images/minus.gif")%>'
	}

	var table = obj.parentNode.parentNode;
	var coll = table.rows;
	for (var i = 0; i < coll.length; i++)
	{
		var row = coll[i];
		var attrKey = row.getAttribute("key");
		if (attrKey)
		{
			if (expand)
			{
				if (attrKey == key)
				{
					row.style.display = "";
				}
			}
			else
			{
				if (attrKey.indexOf(key) >= 0)
				{
					row.style.display = "none";
					var imgInner = row.cells[0].firstChild.firstChild;
					if (imgInner && imgInner.src && imgInner.src.indexOf("minus.gif") > 0)
					{
						imgInner.src = '<%= ResolveClientUrl("~/layouts/images/plus.gif")%>'
					}
				}
			}
		}
	}
}
</script>
<div Printable="0">
	<table cellpadding="5" cellspacing="0" border="0" class="text">
		<tr>
			<td class="ibn-label" style="width:170px;">
				<%=LocRM.GetString("Period")%>:
			</td>
			<td>
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td><asp:DropDownList runat="server" ID="PeriodType" Width="100" AutoPostBack="true" OnSelectedIndexChanged="PeriodType_SelectedIndexChanged"></asp:DropDownList></td>
						<td><asp:DropDownList runat="server" ID="YearList" Width="70" AutoPostBack="true" OnSelectedIndexChanged="YearList_SelectedIndexChanged"></asp:DropDownList></td>
						<td><asp:DropDownList runat="server" ID="QuarterList" Width="100" AutoPostBack="true" OnSelectedIndexChanged="QuarterList_SelectedIndexChanged"></asp:DropDownList></td>
						<td><asp:DropDownList runat="server" ID="MonthList" Width="100" AutoPostBack="true" OnSelectedIndexChanged="MonthList_SelectedIndexChanged"></asp:DropDownList></td>
						<td><mc:Picker ID="FromDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" AutoPostBack="true" /></td>
						<td><asp:Label runat="server" ID="DateDelimiter" Text="- &nbsp;"></asp:Label></td>
						<td><mc:Picker ID="ToDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" AutoPostBack="true" /></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="ibn-label">
				<%=LocRM.GetString("User")%>:
			</td>
			<td>
				<mc:MultiSelect runat="server" ID="UserControl" Width="400" />
			</td>
		</tr>
		<tr>
			<td class="ibn-label">
				<%=LocRM.GetString("Client")%>:
			</td>
			<td>
				<mc:MultiSelect runat="server" ID="ClientControl" Width="400" />
			</td>
		</tr>
		<tr>
			<td class="ibn-label">
				<%=LocRM.GetString("ProjectGroup")%>:
			</td>
			<td>
				<mc:MultiSelect runat="server" ID="ProjectGroupControl" Width="400" />
			</td>
		</tr>
		<tr>
			<td class="ibn-label">
				<%=LocRM.GetString("Completion")%>:
			</td>
			<td>
				<asp:DropDownList runat="server" ID="CompletionList" Width="400" AutoPostBack="true" OnSelectedIndexChanged="CompletionList_SelectedIndexChanged"></asp:DropDownList>
			</td>
		</tr>
		<tr runat="server" id="CompletionPeriodRow">
			<td class="ibn-label">
				<%=LocRM.GetString("CompletionPeriod")%>:
			</td>
			<td>
				<table cellpadding="0" cellspacing="0">
					<tr>
						<td><asp:DropDownList runat="server" ID="CompletionPeriodType" Width="100" AutoPostBack="true" OnSelectedIndexChanged="CompletionPeriodType_SelectedIndexChanged"></asp:DropDownList></td>
						<td><asp:DropDownList runat="server" ID="CompletionYearList" Width="70" AutoPostBack="true" OnSelectedIndexChanged="CompletionYearList_SelectedIndexChanged"></asp:DropDownList></td>
						<td><asp:DropDownList runat="server" ID="CompletionQuarterList" Width="100" AutoPostBack="true" OnSelectedIndexChanged="CompletionQuarterList_SelectedIndexChanged"></asp:DropDownList></td>
						<td><asp:DropDownList runat="server" ID="CompletionMonthList" Width="100" AutoPostBack="true" OnSelectedIndexChanged="CompletionMonthList_SelectedIndexChanged"></asp:DropDownList></td>
						<td><mc:Picker ID="CompletionFromDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" AutoPostBack="true" /></td>
						<td><asp:Label runat="server" ID="CompletionDateDelimiter" Text="- &nbsp;"></asp:Label></td>
						<td><mc:Picker ID="CompletionToDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" AutoPostBack="true" /></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td class="ibn-label">
				<%=LocRM.GetString("Project")%>:
			</td>
			<td>
				<mc:MultiSelect runat="server" ID="ProjectControl" Width="400" />
			</td>
		</tr>
		<tr>
			<td>&nbsp;</td>
			<td>
				<asp:Button runat="server" ID="ShowButton" OnClick="ShowButton_Click" />
				&nbsp;&nbsp;
				<input type="button" class="text" style="WIDTH: 80px" onclick="javascript:window.print()" runat="server" id="PrintButton"/>
			</td>
		</tr>
	</table>
</div>
<asp:Table runat="server" ID="MainTable" CellPadding="5" CellSpacing="0" CssClass="text" Border="1" style="border-collapse:collapse;" Width="600"></asp:Table>
