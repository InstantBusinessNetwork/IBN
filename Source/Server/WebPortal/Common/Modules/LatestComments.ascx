<%@ Reference Control="~/Modules/TopTabs.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.Modules.LatestComments" Codebehind="LatestComments.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<script type="text/javascript">
	//<![CDATA[
	function OpenWindow(query, w, h, scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
	//]]>
</script>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:10px;"><tr><td>
<ibn:blockheader id="tbComments" runat="server" />
</td></tr></table>
<table cellspacing="0" cellpadding="1" width="100%" border="0" class="ibn-stylebox-light ibn-propertysheet">
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgComments" AutoGenerateColumns="False"  AllowSorting="False" cellpadding="3" gridlines="None" CellSpacing="3" borderwidth="0px" Width="100%" ShowHeader="False" PageSize="3" AllowPaging="True" EnableViewState="False" CssClass="pNoMargin">
				<ItemStyle CssClass="ibn-propertysheet"></ItemStyle>
				<HeaderStyle CssClass="text"></HeaderStyle>
				<PagerStyle Visible="False"></PagerStyle>
				<Columns>
					<asp:BoundColumn DataField="ObjectId" Visible="False"></asp:BoundColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)Eval("CreatorId")) %>
							&nbsp;&nbsp;<span class="usernotification">(<%# ((DateTime)Eval("CreationDate")).ToShortDateString() %>)</span>
							<div><%# Mediachase.UI.Web.Util.CommonHelper.parsetext(Eval("Text").ToString(), true, true)%></div>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
