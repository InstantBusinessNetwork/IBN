<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskDates.ascx.cs" Inherits="Mediachase.UI.Web.UserReports.Modules.TaskDates" %>
<%@ Reference Control="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Reference Control="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/UserReports/GlobalModules/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="up" Src="~/UserReports/GlobalModules/ReportHeader.ascx" %>
<div id="filter" runat="server" style="border-bottom: #cccccc 1px solid" printable="0">

	<script type="text/javascript">
		//<![CDATA[
		function ChangeModify(obj) {
			objTbl = document.getElementById('<%=DateTable.ClientID %>');
			id = obj.value;
			if (id == "3") {
				objTbl.style.display = 'block';
			}
			else {
				objTbl.style.display = 'none';
			}
		}
		//]]>
	</script>

	<table cellpadding="5" cellspacing="0" border="0" class="text">
		<tr height="45px">
			<td style="width: 80px;" class="ibn-label">
				<%=LocRM.GetString("Period")%>:&nbsp;
			</td>
			<td valign="middle">
				<select class="text" id="PeriodType" style="width: 150px" onchange="ChangeModify(this);" runat="server">
				</select>
			</td>
			<td>
				<table id="DateTable" cellspacing="2" cellpadding="0" runat="server" class="text">
					<tr>
						<td class="ibn-label">
							&nbsp;<%=LocRM.GetString("PeriodFrom")%>:&nbsp;
						</td>
						<td>
							<mc:Picker ID="PeriodFrom" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
						<td class="ibn-label" style="padding-left: 10px;">
							<%=LocRM.GetString("PeriodTo")%>:&nbsp;
						</td>
						<td>
							<mc:Picker ID="PeriodTo" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td style="width: 80px;" class="ibn-label">
				<%=LocRM.GetString("Project")%>:&nbsp;
			</td>
			<td colspan="2">
				<asp:DropDownList runat="server" ID="ProjectList">
				</asp:DropDownList>
			</td>
		</tr>
	</table>
	<table>
		<tr class="ibn-descriptiontext">
			<td style="width: 80px;">
			</td>
			<td>
				<asp:Button ID="ApplyButton" runat="server" CssClass="text" Width="80px" OnClick="ApplyButton_Click"></asp:Button>
			</td>
			<td>
				<input type="button" class="text" value='<%=LocRM.GetString("Print")%>' style="width: 80px" onclick="javascript:window.print()" />
			</td>
		</tr>
	</table>
	<br />
</div>
<div style="display: none; margin-bottom: 20px" printable="1">
	<ibn:up ID="HeaderControl" runat="server"></ibn:up>
</div>
<asp:DataGrid ID="MainGrid" runat="server" EnableViewState="False" AutoGenerateColumns="False" BorderStyle="None" GridLines="Horizontal" BorderWidth="0px" CellPadding="5" Width="100%">
	<Columns>
		<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="ProjectName"></asp:BoundColumn>
		<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="TaskName"></asp:BoundColumn>
		<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="UserName"></asp:BoundColumn>
		<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="Created" DataFormatString="{0:g}" HeaderStyle-Width="150"></asp:BoundColumn>
		<asp:BoundColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" DataField="OldDate" DataFormatString="{0:g}" HeaderStyle-Width="150"></asp:BoundColumn>
		<asp:TemplateColumn HeaderStyle-CssClass="ibn-vh2" ItemStyle-CssClass="ibn-vb2" HeaderStyle-Width="150">
			<ItemTemplate>
				<%# ((DateTime)Eval("NewDate") > (DateTime)Eval("OldDate")) 
					? "<span style='color:red'>" + ((DateTime)Eval("NewDate")).ToString("g") + "</span>"
					: ((DateTime)Eval("NewDate")).ToString("g") %>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>