<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IssueForumPostView.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.IssueForumPostView" %>
<style type="text/css">
	.ibn-propertysheet2 {
	font-family: verdana;
	font-size: 8pt;
	}
	.ibn-propertysheet2 td {
	padding:5px;
	}
	.ibn-propertysheet2 th {
		font-family: verdana; 
		font-size: 8pt; 
		color: gray;
		font-weight: normal; 
	}
	.ibn-propertysheet2 a {
		text-decoration: none; 
		color: #003399;
	}
	.ibn-propertysheet2 a:hover {
		text-decoration: underline; 
		color: #ff3300;
	}
</style>
<script type="text/javascript">
function resizeTable()
{
	var obj = document.getElementById('bodyDiv');
	
	var toolbarRow = document.getElementById('Tr1');
	var toolbarRow2 = document.getElementById('Tr2');
	var toolbarRow3 = document.getElementById('Tr3');
	var topHeight = 0; 
	if(toolbarRow)
		topHeight = toolbarRow.offsetHeight;
	if(toolbarRow2)
		topHeight = topHeight + toolbarRow2.offsetHeight;
	if(toolbarRow3)
		topHeight = topHeight + toolbarRow3.offsetHeight;
	
	var obj2 = document.getElementById('<%=dgForum.ClientID %>');
	if(obj2)
	{
		if(obj2.rows.length > 0 && obj2.rows[0])
			topHeight = topHeight + obj2.rows[0].offsetHeight;
	}
	
	var intHeight = 0;
	if (typeof(window.innerWidth) == "number")
	{
	  intHeight = window.innerHeight;
	}
	else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
	{
	  intHeight = document.documentElement.clientHeight;
	}
	else if (document.body && (document.body.clientWidth || document.body.clientHeight))
	{
	  intHeight = document.body.clientHeight;
	}

	if(obj)
	{
		obj.style.height = (intHeight - topHeight - 13) + "px";
	}
} 
window.onresize=resizeTable; 
window.onload=resizeTable; 
</script>
<div style="padding:10px;text-align:center;" runat="server" id="divNoMess"></div>
<asp:DataGrid id="dgForum" runat="server" allowpaging="True" cellpadding="0" gridlines="None" 
	CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" 
	enableviewstate="true" ShowHeader="False" PageSize="1">
	<columns>
		<asp:TemplateColumn>
			<ItemTemplate>
				<table cellpadding="0" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style='table-layout:fixed;'>
					<tr class="ForumNode" id="Tr1">
						<td style="width:24px;" class="ibn-navline-top" align="center" valign="top"><%# DataBinder.Eval(Container.DataItem, "Index")%></td>
						<td style="width:28px;padding:4px;" class="ibn-navline-top" valign="top"><%# DataBinder.Eval(Container.DataItem, "NodeType")%></td>
						<td style="padding:4px;" class="ibn-navline-top ibn-value" nowrap="nowrap" valign="top"><%# DataBinder.Eval(Container.DataItem, "Sender")%></td>
						<td style="width:170px;padding:4px;" class="ibn-navline-top ibn-value" valign="top"><%# DataBinder.Eval(Container.DataItem, "Created")%></td>
						<td style="width:125px;padding:4px;" class="ibn-navline-top-right" valign="top">
							&nbsp;
							<asp:ImageButton ID="ibReply" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanReply")%>' Width="16" BorderWidth="0" Runat="server" CausesValidation="False" CommandName="Reply" ImageUrl='<%# GetReplyUrl() %>' Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EMailMessageId")%>'></asp:ImageButton>
							<asp:ImageButton ID="ibReSend" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanReSend")%>' Width="16" BorderWidth="0" Runat="server" CausesValidation="False" CommandName="ReSend" ImageUrl="~/layouts/images/forward.gif" Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EMailMessageId")%>'></asp:ImageButton>
							<asp:ImageButton ID="ibReSendOut" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanReSendOut")%>' Width="16" BorderWidth="0" Runat="server" CausesValidation="False" CommandName="ReSendOut" ImageUrl="~/layouts/images/forward.gif" Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EMailMessageId")%>'></asp:ImageButton>
							<asp:ImageButton ID="ibResolution" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanMakeResolution")%>' Width="16" BorderWidth="0" Runat="server" CausesValidation="False" CommandName="Resolution" ImageUrl="~/layouts/images/icons/resol.gif" Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
							<asp:ImageButton ID="ibUnResolution" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanUnMakeResolution")%>' Width="16" BorderWidth="0" Runat="server" CausesValidation="False" CommandName="UnResolution" ImageUrl="~/layouts/images/icons/unresol.gif" Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
							<asp:ImageButton ID="ibWA" Width="16" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanMakeWorkaround")%>' BorderWidth="0" Runat="server" CausesValidation="False" CommandName="Workaround" ImageUrl="~/layouts/images/icons/wa.GIF" Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
							<asp:ImageButton ID="ibUnWA" Width="16" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanUnMakeWorkaround")%>' BorderWidth="0" Runat="server" CausesValidation="False" CommandName="UnWorkaround" ImageUrl="~/layouts/images/icons/unwa.GIF" Height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
							<asp:ImageButton id="ibDelete" width="16" borderwidth="0" runat="server" causesvalidation="False" commandname="Delete" imageurl="~/layouts/images/delete.gif"  height="16" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
						</td>
					</tr>
					<tr id="Tr2" class="ForumNode" style='<%#!CanVisible(DataBinder.Eval(Container.DataItem, "SystemMessage"))?"display:none;":""%>'>
						<td colspan="2">&nbsp;</td>
						<td style="padding:4px;" valign="top" colspan="2"><%# DataBinder.Eval(Container.DataItem, "SystemMessage")%></td>
						<td>&nbsp;</td>
					</tr>
					<tr id="Tr3" class="ForumNode" style='<%#!CanVisible(DataBinder.Eval(Container.DataItem, "Attachments"))?"display:none;":""%>'>
						<td colspan="2">&nbsp;</td>
						<td valign="top" colspan="2" style="padding:4px;"><%# DataBinder.Eval(Container.DataItem, "Attachments")%></td>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td class='ibn-navline-top' colspan="2">&nbsp;</td>
						<td class='ibn-navline-top' colspan="3" style='padding:4px;padding-top:5px;padding-bottom:5px;'>
							<div id="bodyDiv" style="overflow:auto;">
								<%# DataBinder.Eval(Container.DataItem, "Message").ToString()%>
							</div>
						</td>
					</tr>
				</table>
			</ItemTemplate>
		</asp:TemplateColumn>
	</columns>
	<PagerStyle Position="Top" CssClass="ibn-propertysheet2" HorizontalAlign="Right" />
</asp:DataGrid>