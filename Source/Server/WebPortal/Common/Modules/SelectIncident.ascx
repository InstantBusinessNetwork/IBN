<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectIncident.ascx.cs" Inherits="Mediachase.UI.Web.Common.Modules.SelectIncident" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" Src="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<script type="text/javascript">
function resizeTable()
{
	var obj = document.getElementById('mainDiv');
	var toolbarRow = document.getElementById('trToolbar');
	var filterRow = document.getElementById('trFilter');

	var intHeight = 0;
	var intWidth = 0;
	if (typeof(window.innerWidth) == "number") 
	{
		intWidth = window.innerWidth;
		intHeight = window.innerHeight;
	} 
	else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) 
	{
		intWidth = document.documentElement.clientWidth;
		intHeight = document.documentElement.clientHeight;
	} 
	else if (document.body && (document.body.clientWidth || document.body.clientHeight)) 
	{
		intWidth = document.body.clientWidth;
		intHeight = document.body.clientHeight;
	}
	obj.style.height = (intHeight - toolbarRow.offsetHeight - filterRow.offsetHeight) + "px";
	obj.style.width = intWidth + "px";
}
window.onresize=resizeTable;
window.onload=resizeTable;
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-navline">
	<tr id="trToolbar">
		<td>
			<ibn:BlockHeader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr id="trFilter">
		<td class="ibn-light ibn-navline">
			<table cellspacing="0" cellpadding="0" width="100%" border="0" style="padding:5px"><tr><td>
				<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="0">
					<tr>
						<td class="ibn-value"><asp:CheckBox ID="cbShowActive" Runat="server" AutoPostBack="True"></asp:CheckBox> </td>
						<td class="ibn-value" align="right" >
							<asp:TextBox runat="server" ID="tbSearchString" Width="150px"></asp:TextBox>
							<asp:Button runat="server" ID="btnSearch" />
						</td>
					</tr>
				</table>
			</td></tr></table>
		</td>
	</tr>
	<tr>
		<td>
			<div id="mainDiv" style="height:420px;width:640px;overflow:auto;">
				<div style="height:100%">
					<dg:datagridextended runat="server" ID="grdMain" AutoGenerateColumns="false" Width="100%" 
						CellPadding="4" GridLines="None" AllowPaging="true" AllowSorting="true" PageSize="50" PagerStyle-CssClass="ibn-vb2">
						<Columns>
							<asp:boundcolumn visible="false" datafield="IncidentId"></asp:boundcolumn>
							<asp:TemplateColumn SortExpression="Title" >
								<ItemStyle CssClass="ibn-vb2" />
								<HeaderStyle CssClass="ibn-vh2" />
								<ItemTemplate>
									<%# Mediachase.UI.Web.Util.CommonHelper.GetIncidentTitleWL(Eval("Title").ToString(), (int)Eval("StateId"), false)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<ItemStyle CssClass="ibn-vb2" Width="25px"/>
								<HeaderStyle CssClass="ibn-vh2" Width="25px" />
								<ItemTemplate>
									<asp:ImageButton id="ibRelate" runat="server" imageurl="../../Layouts/Images/Select.gif" Width="16" Height="16"></asp:ImageButton>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</dg:datagridextended>
				</div>
			</div>
		</td>
  </tr>
</table>