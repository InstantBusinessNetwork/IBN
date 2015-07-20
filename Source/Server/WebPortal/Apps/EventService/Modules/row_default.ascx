<%@ Control Language="C#" AutoEventWireup="true" ClassName="Mediachase.Ibn.Web.UI.EventService.row_default" %>
<%@ Register TagPrefix="mc" Namespace="Mediachase.Ibn.Web.UI.EventService" Assembly="Mediachase.UI.Web" %>
<div class="nodeItem">
	<table class="tbl nodeHead" cellpadding="0" cellspacing="0">
		<tr>
			<td class="user">
				<div class="ibn-propertysheet"><%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(int.Parse(Eval("CreatorId").ToString())) %></div>
			</td>
			<td>
				<div class="ibn-propertysheet"><mc:EventsList runat="server" ID="ucHeadEvents" DataItem='<%# ((Mediachase.Ibn.Data.Services.EventGroup)DataBinder.GetDataItem(Container)).HeaderEvents %>'></mc:EventsList></div>
			</td>
			<td class="date right">
				<%# (Mediachase.IBN.Business.User.GetLocalDate((DateTime)Eval("Created"))).ToShortDateString() %>
				<%# (Mediachase.IBN.Business.User.GetLocalDate((DateTime)Eval("Created"))).ToShortTimeString()%>
			</td>
		</tr>
	</table>

	<div style='<%# ((Mediachase.Ibn.Data.Services.EventGroup)DataBinder.GetDataItem(Container)).BodyEvents != null ? "display:block" : "display:none" %>'>
		<table class="tbl" cellpadding="0" cellspacing="0">
			<tr>
				<td class="nodeIcon" align="center"></td>
				<td>
					<div class="nodeBody ibn-propertysheet">
						<mc:EventsList runat="server" ID="ucBodyEvents" DataItem='<%# ((Mediachase.Ibn.Data.Services.EventGroup)DataBinder.GetDataItem(Container)).BodyEvents %>'></mc:EventsList>
					</div>
					<div class="nodeFiles">
					</div>
				</td>
			</tr>
		</table>
	</div>
	
	<div class="nodeFoot">
		<mc:EventsList runat="server" ID="ucFooterEvents" DataItem='<%# ((Mediachase.Ibn.Data.Services.EventGroup)DataBinder.GetDataItem(Container)).FooterEvents %>'></mc:EventsList>
	</div>
</div>