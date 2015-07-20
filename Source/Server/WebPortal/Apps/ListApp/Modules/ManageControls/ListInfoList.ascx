<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListInfoList.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.ListApp.Modules.ManageControls.ListInfoList" %>
<%@ Reference Control="~/Modules/PageViewMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/PageViewMenu.ascx" %>
<script type="text/javascript">
function DeleteFolder(FolderId)
{
	if(confirm('<%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.ListInfo:WarningF}")%>'))
	{
		document.forms[0].<%= deletedId.ClientID%>.value = FolderId;
		<%= Page.ClientScript.GetPostBackEventReference(lbDelFolder, "")%>
	}
}

function DeleteList(ListId)
{
	if(confirm('<%=Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.ListInfo:WarningL}")%>'))
	{
		document.forms[0].<%= deletedId.ClientID%>.value = ListId;
		<%= Page.ClientScript.GetPostBackEventReference(lbDelList, "")%>
	}
}
</script>
<table style="margin-top:0px;" cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox" id="mainTable" runat="server">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" title="ToolBar" runat="server"></ibn:blockheader>
		</td>
	</tr>
	<tr>
		<td>
			<dg:datagridextended id="grdMain" runat="server" allowsorting="True" allowpaging="True" width="100%" 
				autogeneratecolumns="False" PageSize="10" borderwidth="0" gridlines="None" cellpadding="3">
				<columns>
					<asp:boundcolumn datafield="ObjectId" visible="False"></asp:boundcolumn>
					<asp:boundcolumn datafield="Type" visible="False"></asp:boundcolumn>
					<asp:templatecolumn>
						<headerstyle cssclass="ibn-vh2" width="21"></headerstyle>
						<ItemStyle cssclass="ibn-vb2" width="21"></ItemStyle>
						<itemtemplate>
							<img alt="" src='<%# DataBinder.Eval(Container.DataItem, "Icon")%>' width="16" height="16" border="0" />
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" datafield="Name" sortexpression="sortName"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="100px" datafield="TypeName" sortexpression="StatusName"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="100px" datafield="StatusName" sortexpression="StatusName"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" HeaderStyle-Width="170px" datafield="CreatorName" sortexpression="sortCreator"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" HeaderStyle-Width="120px" datafield="CreationDate" sortexpression="CreationDate" dataformatstring="{0:d}"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="23" datafield="ActionMove"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="23" datafield="ActionSecurity"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="23" datafield="ActionEdit"></asp:boundcolumn>
					<asp:boundcolumn headerstyle-cssclass="ibn-vh2" itemstyle-cssclass="ibn-vb2" headerstyle-width="23" datafield="ActionDelete"></asp:boundcolumn>
				</columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>
<input type="hidden" id="deletedId" runat="server" />
<asp:LinkButton ID="lbDelFolder" Runat="server" Visible="False"></asp:LinkButton>
<asp:LinkButton ID="lbDelList" Runat="server" Visible="False"></asp:LinkButton>
<asp:Button runat="server" ID="btnRefresh" OnClick="btnRefresh_Click" Text="Refresh" style="display:none" />