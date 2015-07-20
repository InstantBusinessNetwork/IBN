<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Common.Comments" Codebehind="Comments.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" Src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<script type="text/javascript">
//<![CDATA[
function OpenWindow(query,w,h,scroll)
{
	var l = (screen.width - w) / 2;
	var t = (screen.height - h) / 2;
	
	winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
	if (scroll) winprops+=',scrollbars=1';
	var f = window.open(query, "_blank", winprops);
}
//]]>
</script>
<table cellpadding="0" cellspacing="7" width="100%">
	<tr>
		<td>
			<ibn:BlockHeaderLight ID="secHeader" runat="server"></ibn:BlockHeaderLight>
			<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="5" width="100%" border="0">
				<tr>
					<td><dg:DataGridExtended id="dgComments" runat="server" width="100%" autogeneratecolumns="False" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="1" allowsorting="True" pagesize="10" allowpaging="True" enableviewstate="false" LayoutFixed="false" CssClass="pNoMargin">
							<Columns>
								<asp:BoundColumn Visible="False" DataField="DiscussionId">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Text">
									<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<%# Mediachase.UI.Web.Util.CommonHelper.parsetext(Eval("Text").ToString(),true) %>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Created By">
									<HeaderStyle Width="170px" CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle Width="170px" CssClass="ibn-vb2"></ItemStyle>
									<ItemTemplate>
										<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)Eval("CreatorId"))%>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="CreationDate" SortExpression="CreationDate" HeaderText="Creation Date" DataFormatString="{0:g}">
									<HeaderStyle Width="150px" CssClass="ibn-vh2"></HeaderStyle>
									<ItemStyle Width="150px" CssClass="ibn-vb2"></ItemStyle>
								</asp:BoundColumn>
								<asp:templatecolumn Visible="True">
								<headerstyle horizontalalign="Right" cssclass="ibn-vh-right" width="60px"></headerstyle>
									<itemstyle horizontalalign="Right" cssclass="ibn-vb2" width="60px"></itemstyle>
									<itemtemplate>
										<asp:HyperLink 
											Visible='<%# (int)Eval("CreatorId") == Mediachase.IBN.Business.Security.CurrentUser.UserID%>' 
											ImageUrl = "../../layouts/images/Edit.GIF" NavigateUrl=<%# GetEditLink((int)Eval("DiscussionId")) %> Runat="server" ToolTip='<%#LocRM.GetString("Edit") %>' ID="Hyperlink1" NAME="Hyperlink1">
										</asp:HyperLink>&nbsp;
										<asp:imagebutton
											Visible='<%# (int)Eval("CreatorId") == Mediachase.IBN.Business.Security.CurrentUser.UserID%>' 
											id="ibDelete" title='<%# LocRM.GetString("Delete")%>' runat="server" borderwidth="0" imageurl="../../layouts/images/delete.gif" commandname="Delete" causesvalidation="False">
										</asp:imagebutton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Right" Mode="NumericPages"></PagerStyle>
						</dg:DataGridExtended>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>