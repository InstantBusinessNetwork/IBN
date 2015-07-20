<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddFramePopup.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Apps.Common.Layout.Modules.AddFramePopup" %>
<%@ Register TagPrefix="ibn" TagName="CheckControl" Src="~/Apps/Common/Design/CheckControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeaderLight" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="mc" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<asp:UpdatePanel runat="server" ID="panelAddWorkspace" ChildrenAsTriggers="true">
	<ContentTemplate>
		<!-- control set tab -->
		<div style="display: block; margin-bottom: 10px;" runat="server" id="divControlSet">
			<!-- DVS: !width: 100%; - css hack apply for IE ONLY -->
			<div style="margin: 10px 20px 0px 20px; "> 
			<ibn:BlockHeaderLight ID="secControls" runat="server" />
			</div>
			<div style="width: 130px; float: left; margin-left: 20px; border-left: solid 1px #ADCBEF; border-bottom: solid 1px #ADCBEF; height: 260px;">
				<asp:Repeater runat="server" ID="repCategories">
					<ItemTemplate>
						<div style="padding: 5px 3px 5px 3px;" runat="server" id="divContainer">
						<asp:LinkButton runat="server" ID="lblGroup" Text='<%# Mediachase.Ibn.Web.UI.CHelper.GetResFileString(Eval("FriendlyName").ToString()) %>' CommandName="Edit" CommandArgument='<%# Eval("Uid") %>' />
						</div>
					</ItemTemplate>
				</asp:Repeater>
			</div>
			<div style="margin: 0px 20px 0px 0px; height: 260px; overflow: scroll; position: relative;" class="ibn-WPBorder">
				<table cellpadding="0" cellspacing="0" border="0" class="text" style="min-height: 260px; height: 100%; table-layout: fixed;">
					<tr>

						<td class="text" style="width: 100%; vertical-align: top; overflow: hidden;">
							<asp:DataGrid runat="server" ID="grdMain" PageSize="50" AutoGenerateColumns="false" CssClass="text" CellPadding="5" CellSpacing="0" ShowHeader="false" Width="100%" GridLines="none" BorderWidth="0">
								<Columns>
									<asp:BoundColumn DataField="Id" Visible="false" />
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:CheckBox runat="server" ID="cbId" Checked="false" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderTemplate>
											<%= Mediachase.Ibn.Web.UI.CHelper.GetResFileString("{IbnFramework.Global:_mc_Title}")%>
										</HeaderTemplate>
										<ItemTemplate>
											<table style="width: 100%; border-bottom: dotted 1px #bbb; padding-bottom: 10px;">
												<tr>
													<td style="padding-right: 5px;"><img src='<%# Eval("IconPath") %>' style="float: left;" /></td>
													<td style="width: 100%;">
														<p style="padding: 0px 0px 3px 0px; margin: 0px;font-size: 14px; font-weight: bold;"><%# Eval("Title") %></p>
														<p style="padding: 0px; margin: 0px;"><%# Eval("Description") %></p>
													</td>
												</tr>
												<tr>
													<td colspan="2"><asp:LinkButton runat="server" ID="lblAddControl" Text="<%$ Resources:IbnFramework.WidgetEngine, _mc_AddSingleControl %>" CommandName="Add" CommandArgument='<%# Eval("Id") %>' /></td>
												</tr>
											</table>

										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
						</td>
					</tr>
				</table>
			</div>	
		</div>
		<div style="clear: both;"></div>
		<div runat="server" id="divTemplateUpdate" style="position: absolute; display: none; width: 60%; left: 20%; height: 18px; top: 1px; border: none;" >
			<span class="infoBlock" style="padding: padding: 3px 1px 3px 1px; border: none; font-weight: bold;"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources : IbnFramework.WidgetEngine, _mc_ControlAdded%>" /> </span>
		</div>
		
		<div style="margin: 5px 20px 0px 20px" align="right">
			<asp:LinkButton CssClass="floatLeft" runat="server" ID="lblEditTemplate" Text="<%$ Resources : IbnFramework.WidgetEngine, _mc_EditPageTemplate %>" />
			<mc:ImButton runat="server" ID="btnCancel" />
			<mc:ImButton runat="server" ID="btnAddAndClose"></mc:ImButton>&nbsp;
		</div>
	</ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
function hidedivTemplateUpdate()
{
	var obj_divTemplateUpdate = document.getElementById('<%= divTemplateUpdate.ClientID %>');
	if (obj_divTemplateUpdate)
	{
		obj_divTemplateUpdate.style.display = 'none';
	}
}

$addHandler(document.body, 'click', hidedivTemplateUpdate);
</script>
