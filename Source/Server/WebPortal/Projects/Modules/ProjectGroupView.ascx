<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Projects.Modules.ProjectGroupView" Codebehind="ProjectGroupView.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MetaDataViewControl" src="..\..\Modules\MetaDataViewControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="MdpCustomization" src="..\..\Admin\Modules\MdpCustomization.ascx" %>
<script language="javascript">
function OpenSnapshot(ProjectId)
{
	OpenWindow('../Reports/OverallProjectSnapshot.aspx?ProjectId=' + ProjectId,750,466,true);
}

function OpenWindow(query,w,h,scroll)
	{
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		
		winprops = 'resizable=1, height='+h+',width='+w+',top='+t+',left='+l;
		if (scroll) winprops+=',scrollbars=1';
		var f = window.open(query, "_blank", winprops);
	}
</script>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="MARGIN-TOP:0px">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<table class="ibn-propertysheet" width="100%" border="0" cellpadding="5" cellspacing="2">
				<tr>
					<td width="100" valign=top class="text">
						<b><%=LocRM.GetString("title")%>:</b>
					</td>
					<td width="30%" valign=top>
						<asp:label id="lblTitle" runat="server" CssClass="text"></asp:label>
					</td>
					<td></td>
				</tr>
				<tr>
					<td width="100" valign=top class="text">
						<b><%=LocRM.GetString("description")%>:</b>
					</td>
					<td width="30%" valign=top>
						<asp:label id="lblDescr" runat="server" CssClass="text"></asp:label>
					</td>
					<td></td>
				</tr>
				<tr>
					<td width="100" valign=top class="text">
						<b><%=LocRM.GetString("tCreationDate")%>:</b>
					</td>
					<td width="30%" valign=top>
						<asp:label id="lblCreationDate" runat="server" CssClass="text"></asp:label>
					</td>
					<td width="80" valign=top class="text">
						<b><%=LocRM.GetString("tCreator")%>:</b>
					</td>
					<td width="30%" valign=top>
						<asp:label id="lblCreator" runat="server" CssClass="text"></asp:label>
					</td>
					<td></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<ibn:MdpCustomization id="mdView" EnableCustomize="false" runat="server" ClassName="portfolio" />
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" style="MARGIN-TOP:10px" id="tcControl" runat=server>
	<tr>
		<td>
			<ibn:blockheader id="secHeaderPrj" runat="server" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:DataGrid Runat="server" ID="dgProjects" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" cellpadding="3" gridlines=none CellSpacing="0" borderwidth="0px" Width="100%" ShowHeader="True">
				<Columns>
					<asp:BoundColumn Visible="False" DataField="ProjectId" ReadOnly="True"></asp:BoundColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<%# 
							 Mediachase.UI.Web.Util.CommonHelper.GetProjectStatus((int)DataBinder.Eval(Container.DataItem, "ProjectId")) 
							%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle CssClass="ibn-vh2"></HeaderStyle>
						<ItemStyle CssClass="ibn-vb2" Width="170px"></ItemStyle>
						<ItemTemplate>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem, "ManagerId"))%>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Right" Width="25px" CssClass="ibn-vh-right"></HeaderStyle>
						<ItemStyle Width="25px" CssClass="ibn-vb2"></ItemStyle>
						<ItemTemplate>
							<asp:HyperLink Visible=false ImageUrl = "../../layouts/images/report.GIF" NavigateUrl='<%# "javascript:OpenSnapshot(" + DataBinder.Eval(Container.DataItem, "ProjectId").ToString() + ")" %>' Runat="server" ToolTip='<%#LocRM.GetString("Snapshot") %>' ID="hlSView">
								</asp:HyperLink>
							&nbsp;
							<asp:imagebutton id="ibDelete" title='<%#LocRM.GetString("tDelete")%>' runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid>
		</td>
	</tr>
</table>
