<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Incidents.Modules.Forum" Codebehind="Forum.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="..\..\Modules\BlockHeaderLightWithMenu.ascx" %>
<%@ register TagPrefix="dg" namespace="Mediachase.UI.Web.Modules.DGExtension" Assembly="Mediachase.UI.Web" %>
<%@ Register TagPrefix="ibn" TagName="Responsibility" src="..\..\Incidents\Modules\Responsibility.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Tracking" src="..\..\Incidents\Modules\QuickTracking.ascx" %>
<script type="text/javascript">
	//<![CDATA[
	function OpenMessage(nodeId, eMailId, fl)
	{
		var w = 750;
		var h = 550;
		var l = (screen.width - w) / 2;
		var t = (screen.height - h) / 2;
		winprops = 'resizable=1,,scrollbars=0, height='+h+',width='+w+',top='+t+',left='+l;
		if(fl)
			window.open('../Incidents/EMailView.aspx?NodeId='+nodeId, "NodeView", winprops);
		else
			window.open('../Incidents/EMailView.aspx?NodeId='+nodeId+'&EMailId='+eMailId, "EMailView", winprops);
	}
	//]]>
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
	<tr>
		<td valign="top"><ibn:Tracking id="ucTracking" runat="server"/></td>
		<td valign="top"><ibn:Responsibility id="ucResponsibility" runat="server"/></td>
	</tr>
</table>
<table cellpadding="0" cellspacing="0" width="100%" style="margin-top:5px">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<div style="padding:10" runat="server" id="divNoMess"></div>
			<dg:datagridextended id="dgForum" runat="server" allowpaging="True" allowsorting="True" cellpadding="0" gridlines="None" CellSpacing="0" borderwidth="0px" autogeneratecolumns="False" width="100%" enableviewstate="true" ShowHeader="False" PageSize="10">
				<columns>
					<asp:TemplateColumn>
						<ItemTemplate>
							<table cellpadding="4" cellspacing="0" border="0" width="100%" class="ibn-propertysheet" style='table-layout:fixed;<%# ((int)DataBinder.Eval(Container.DataItem, "Weight")<3)? "BACKGROUND-COLOR: #dedede;" : ""%>'>
								<tr class="ForumNode">
									<td style="padding:0px; width:24px;" class="ibn-navline-top" align="center"><%# DataBinder.Eval(Container.DataItem, "Index")%></td>
									<td style="width:28px;" class="ibn-navline-top"><%# DataBinder.Eval(Container.DataItem, "NodeType")%></td>
									<td style="width:200px; white-space:nowrap" class="ibn-navline-top ibn-value"><%# DataBinder.Eval(Container.DataItem, "Sender")%></td>
									<td style="width:170px;" class="ibn-navline-top ibn-value"><%# DataBinder.Eval(Container.DataItem, "Created")%></td>
									<td class="ibn-navline-top"><%# DataBinder.Eval(Container.DataItem, "SystemMessage")%></td>
									<td style="width:125px;" class="ibn-navline-top-right">
										&nbsp;
										<asp:ImageButton ID="ibReply" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanReply")%>' runat="server" CausesValidation="False" CommandName="Reply" ImageUrl='<%# GetReplyUrl() %>' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EMailMessageId")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibReSend" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanReSend")%>' runat="server" CausesValidation="False" CommandName="ReSend" ImageUrl="../../layouts/images/forward.gif" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EMailMessageId")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibReSendOut" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanReSendOut")%>' runat="server" CausesValidation="False" CommandName="ReSendOut" ImageUrl="../../layouts/images/forward.gif" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "EMailMessageId")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibResolution" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanMakeResolution")%>' runat="server" CausesValidation="False" CommandName="Resolution" ImageUrl="../../layouts/images/icons/resol.gif" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibUnResolution" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanUnMakeResolution")%>' runat="server" CausesValidation="False" CommandName="UnResolution" ImageUrl="../../layouts/images/icons/unresol.gif" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibWA" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanMakeWorkaround")%>' runat="server" CausesValidation="False" CommandName="Workaround" ImageUrl="../../layouts/images/icons/wa.GIF" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibUnWA" Visible='<%# (bool)DataBinder.Eval(Container.DataItem, "CanUnMakeWorkaround")%>' runat="server" CausesValidation="False" CommandName="UnWorkaround" ImageUrl="../../layouts/images/icons/unwa.GIF" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
										<asp:ImageButton ID="ibDelete" runat="server" CausesValidation="False" CommandName="Delete" ImageUrl="../../layouts/images/DELETE.GIF" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id")%>'></asp:ImageButton>
									</td>
								</tr>
								<tr class="ForumNode" runat="server" visible='<%# (DataBinder.Eval(Container.DataItem, "Attachments").ToString() == "")?false:true%>'>
									<td colspan="2"></td>
									<td valign="top" colspan="3"><%# DataBinder.Eval(Container.DataItem, "Attachments")%></td>
									<td>&nbsp;</td>
								</tr>
								<tr style='<%#!CanVisible(DataBinder.Eval(Container.DataItem, "Message"))?"display:none;":""%>'>
									<td class='ibn-navline-top' colspan="2">&nbsp;</td>
									<td class='ibn-navline-top' colspan='4' style='padding-top:5px;padding-bottom:5px;'>
										 <%#DataBinder.Eval(Container.DataItem, "Message").ToString()%>
									</td>
								</tr>
							</table>
						</ItemTemplate>
					</asp:TemplateColumn>
				</columns>
			</dg:datagridextended>
		</td>
	</tr>
</table>
<asp:LinkButton ID="lbSort" Runat="server" Visible="False"></asp:LinkButton>