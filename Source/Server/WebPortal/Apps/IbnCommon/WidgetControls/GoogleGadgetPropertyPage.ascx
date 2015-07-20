<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleGadgetPropertyPage.ascx.cs" Inherits="Mediachase.UI.Web.Apps.IbnCommon.WidgetControls.GoogleGadgetPropertyPage" %>
<%@ Register Src="~/Apps/MetaUIEntity/Grid/EntityGrid.ascx" TagName="Grid" TagPrefix="mc" %>
<%@ Register Src="~/Apps/MetaUI/Grid/MetaGridServerEventAction.ascx" TagName="GridEventAction" TagPrefix="mc" %>
<%@ Register TagPrefix="mc2" Assembly="Mediachase.Ibn.Web.UI.WebControls" Namespace="Mediachase.Ibn.Web.UI.WebControls" %>
<mc2:McDock ID="DockTop" runat="server" Anchor="Top" EnableSplitter="false" DefaultSize="35">
	<DockItems>
		<div style="marign: 10px 10px 10px 0px;">
			<table style="width: 100%;">
				<tr>
					<td style="padding: 10px 10px 0px 10px;width: 50%;" align="left">
						<asp:Literal ID="Literal1" runat="server" Text="<%$ Resources : IbnFramework.WidgetEngine, _mc_CurrentGadget%> " /> <%= this.CurrentGadget() %> 
					</td>
					<td style="padding: 10px 10px 0px 10px; width: 50%;" align="right">
						<asp:TextBox runat="server" ID="tbSearch" />
						<asp:ImageButton Runat="server" id="btnSearch" Width="16" Height="16" ImageUrl="~/layouts/images/search.gif" ImageAlign="AbsMiddle" />
						<asp:ImageButton runat="server" ID="btnClear" Width="19" Height="17" ImageUrl="~/Layouts/Images/reset17.gif" ImageAlign="AbsMiddle" />
					</td>
				</tr>
			</table>
			
		</div>
	</DockItems>
</mc2:McDock>
<mc2:McDock ID="McLeft" runat="server" Anchor="Left" EnableSplitter="false" DefaultSize="9">
	<DockItems>
	</DockItems>
</mc2:McDock>
<mc:Grid runat="server" ID="ctrlGrid" ClassName="GoogleGadget" ShowCheckboxes="false">
</mc:Grid>
<mc:GridEventAction runat="server" ID="ctrlEventAction" ClassName="GoogleGadget" GridActionMode="ListViewUI">
</mc:GridEventAction>