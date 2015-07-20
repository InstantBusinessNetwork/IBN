<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.BroadcastAlerts" Codebehind="BroadcastAlerts.ascx.cs" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~\Modules\BlockHeader.ascx" %>
<table cellspacing="0" cellpadding="0" border="0" width="100%" class="ibn-stylebox2">
	<tr>
		<td>
			<ibn:blockheader id="secHeader" runat="server" title="" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:repeater id="MessRep" Runat="server" EnableViewState="false">
				<HeaderTemplate>
					<table class="text" border="0" cellspacing="0" cellpadding="5" width="100%">
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td>
							<%# (DateTime)DataBinder.Eval(Container.DataItem,"CreationDate")%>
						</td>
						<td>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem,"CreatorId")) %>
						</td>
						<td>
							<%# DataBinder.Eval(Container.DataItem,"text")%>
						</td>
					</tr>
				</ItemTemplate>
				<AlternatingItemTemplate>
					<tr class="ibn-alternating">
						<td>
							<%# (DateTime)DataBinder.Eval(Container.DataItem,"CreationDate")%>
						</td>
						<td>
							<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus((int)DataBinder.Eval(Container.DataItem,"CreatorId")) %>
						</td>
						<td>
							<%# DataBinder.Eval(Container.DataItem,"text")%>
						</td>
					</tr>
				</AlternatingItemTemplate>
				<FooterTemplate>
					</table>
				</FooterTemplate>
			</asp:repeater>
			<div style="padding:10px;">
			<asp:Label ID="lblNoItems" Runat=server CssClass="ibn-alerttext"></asp:Label>
			</div>
		</td>
	</tr>
</table>