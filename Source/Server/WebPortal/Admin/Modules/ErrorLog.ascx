<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.UserLog" Codebehind="ErrorLog.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx"%>

<script language="javascript" type="text/javascript">
function ClearLog()
{
	if(confirm('<%=LocRM.GetString("tClearErrorLog")%>'+'?'))
	{
		<%= Page.ClientScript.GetPostBackEventReference(ClearLogButton, "")%>
	}
}
</script>

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server" id="tableErrors">
	<tr>
		<td>
			<ibn:BlockHeader id="Header2" title='<%#LocRM.GetString("QuickInformation")%>' runat="server" />
		</td>
	</tr>
	<tr>
		<td style="PADDING-RIGHT:5px;PADDING-BOTTOM:5px">
			<dg:DataGridExtended id="dgeErrors" runat="server" allowpaging="True" 
							pagesize="10" allowsorting="True" cellpadding="0" gridlines="None" 
							CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" LayoutFixed="True">
				<columns>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headertext="Error ID" datafield="ErrorID"></asp:boundcolumn>
					<asp:TemplateColumn headertext="Creation Time">
						<HeaderStyle cssclass="ibn-vh2" width="150" />
						<ItemStyle cssclass="ibn-vb2" width="150" />
						<ItemTemplate>
							<%# ((DateTime)Eval("CreationTime")).ToShortDateString() + " " + ((DateTime)Eval("CreationTime")).ToShortTimeString() %>
						</ItemTemplate>
					</asp:TemplateColumn>
				</columns>
			</dg:DataGridExtended>	
			<asp:LinkButton runat="server" ID="ClearLogButton" Visible="false" OnClick="ClearLogButton_Click"></asp:LinkButton>
		</td>
	</tr>
</table>
