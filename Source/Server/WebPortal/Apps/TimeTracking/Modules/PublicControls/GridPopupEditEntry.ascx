<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridPopupEditEntry.ascx.cs" Inherits="Mediachase.UI.Web.Apps.TimeTracking.Modules.PublicControls.GridPopupEditEntry" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="formView" Src="~/Apps/MetaUI/MetaForm/FormDocumentView.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>

<div runat="server" id="TTEntry">
	<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
		<tr>
			<td style="padding-left:10px;">&nbsp;</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day1Label"></asp:Label>
			</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day2Label"></asp:Label>
			</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day3Label"></asp:Label>
			</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day4Label"></asp:Label>
			</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day5Label" CssClass="smallCurrent"></asp:Label>
			</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day6Label"></asp:Label>
			</td>
			<td style="padding-top:10px;">
				<asp:Label runat="server" ID="Day7Label"></asp:Label>
			</td>
		</tr>
		<tr>
			<td style="padding-left:10px;">&nbsp;</td>
			<td valign="top">
				<ibn:Time id="Day1Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
			<td valign="top">
				<ibn:Time id="Day2Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
			<td valign="top">
				<ibn:Time id="Day3Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
			<td valign="top">
				<ibn:Time id="Day4Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
			<td valign="top">
				<ibn:Time id="Day5Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
			<td valign="top">
				<ibn:Time id="Day6Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
			<td valign="top">
				<ibn:Time id="Day7Time" ShowTime="HM" HourSpinMaxValue="24" runat="server" ShowSpin="false"/>
			</td>
		</tr>
		<tr>
			<td colspan="8" style="padding-top:8px;">
				<ibn:formView ID="frmView" runat="server" />
			</td>
		</tr> 
		<tr>
			<td colspan="8" style="text-align: center; padding-bottom:8px;" >
				<asp:Button runat="server" ID="btnSaveEntry" CausesValidation="true" CommandArgument="0" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSaveEntry_Click" />&nbsp;&nbsp;
				<asp:Button runat="server" ID="btnCancelEntry" CausesValidation="false" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" />
			</td>
		</tr> 
	</table>
</div>

<div runat="server" id="TTBlock">
	<table class="ibn-propertysheet" cellspacing="0" cellpadding="2" width="100%" border="0" style="height: 150px;">
		<tr>
			<td>
				<ibn:formView ID="frmViewBlock" runat="server"/>
			</td>
		</tr> 
		<tr>
			<td style="text-align: center;">
				<asp:Button runat="server" ID="btnSaveBlock" CausesValidation="true" CommandArgument="1" Text="<%$Resources : IbnFramework.Global, _mc_Save%>" OnClick="btnSaveBlock_Click" />&nbsp;&nbsp;
				<asp:Button runat="server" ID="btnCancelBlock" CausesValidation="false" Text="<%$Resources : IbnFramework.Global, _mc_Cancel%>" />
			</td>
		</tr>
	</table>
</div>	

<div id="ReadOnlyBlock" runat="server">
	<table class="ibn-propertysheet" cellspacing="0" cellpadding="2" width="100%" border="0" style="height: 100%;">
		<tr style="height: 130px;">
			<td style="text-align: center; padding: 5px 0px 15px 0px;"><asp:Literal runat="server" ID="ReadOnlyMsg" Text="<%$Resources: IbnFramework.TimeTracking, _mc_ReadOnly %>"></asp:Literal></td>
		</tr>
		<tr>
			<td style="text-align: center">
				<asp:Button runat="server" ID="btnCancelReadOnly" CausesValidation="false" Text="<%$Resources : IbnFramework.GlobalMetaInfo, Close%>"/>
			</td>
		</tr>
	</table>
</div>
