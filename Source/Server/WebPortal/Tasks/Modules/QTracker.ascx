<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Tasks.Modules.QTracker" Codebehind="QTracker.ascx.cs" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table style="MARGIN-TOP: 5px" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="tbView" runat="server"></ibn:blockheader></td>
	</tr>
</table>
<table class="text ibn-stylebox-light" cellspacing="0" cellpadding="3" width="100%" border="0">
	<tr>
		<td align="center" width="80" rowspan="2"><img height="98" alt="" src="../Layouts/Images/check.gif" width="60" border="0" /></td>
		<td valign="top">
			<table cellspacing="0" cellpadding="7" border="0" class="text" width="100%">
				<tr id="trAD" runat="server">
					<td>
						<%=LocRM.GetString("Text7") %>
						<br />
						<br />
						<div style="WIDTH:100%" align="center">
							<btn:IMButton class=text  id="btnAccept" Text='<%# LocRM.GetString("Accept") %>' Runat="server" style="width:110px;" onserverclick="btnAccept_ServerClick"></btn:IMButton>&nbsp;&nbsp;
							<btn:IMButton class=text id="btnDecline" Text='<%# LocRM.GetString("Decline") %>' Runat="server" isdecline="true" style="width:110px;" onserverclick="btnDecline_ServerClick"></btn:IMButton>
						</div>
						<br />
					</td>
				</tr>
				<tr id="trPS" runat="server">
					<td>
						<%=LocRM.GetString("Text1") %>
						<br />
						<br />
						<table class="ibn-propertysheet" id="Table1" cellspacing="3" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="right" width="40%"><%=LocRM.GetString("PersonalStatus") %>:&nbsp;&nbsp;</td>
								<td><asp:dropdownlist id="ddPercentPS" Runat="server"></asp:dropdownlist>&nbsp;&nbsp;</td>
							</tr>
							<tr>
								<td align="right" valign="top"><%=LocRM.GetString("SpentHours") %>:&nbsp;&nbsp;</td>
								<td style="HEIGHT: 15px">
									<ibn:Time id="dtcTimesheetHoursPS" ShowTime="HM" HourSpinMaxValue="24" ViewStartDate="false" runat="server" />
									<asp:customvalidator id="CustomValidator1" runat="server" Display="Dynamic" ErrorMessage="Can't update" />
								</td>
							</tr>
							<tr>
								<td align="right"><%=LocRM.GetString("Date") %>:&nbsp;&nbsp;</td>
								<td>
									<mc:Picker ID="dtcPS" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
								</td>
							</tr>
							<tr>
								<td align="right"></td>
								<td>
									<btn:IMButton class="text" id="btnUpdatePS" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" onserverclick="btnUpdatePS_ServerClick"></btn:IMButton>
								</td>
							</tr>
						</table>
						<br />
					</td>
				</tr>
				<tr id="trPSOnly" runat="server">
					<td>
						<%=LocRM.GetString("Text1") %>
						<br />
						<br />
						<table class="ibn-propertysheet" cellspacing="3" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="right" width="40%"><%=LocRM.GetString("PersonalStatus") %>:&nbsp;&nbsp;</td>
								<td><asp:dropdownlist id="ddPercentPSOnly" Runat="server"></asp:dropdownlist>&nbsp;&nbsp;</td>
							</tr>
							<tr>
								<td align="right"></td>
								<td>
									<btn:IMButton class="text" id="btnUpdatePSOnly" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" onserverclick="btnUpdatePSOnly_ServerClick"></btn:IMButton>
								</td>
							</tr>
						</table>
						<br />
					</td>
				</tr>
				<tr id="trOS" runat="server">
					<td>
						<%=LocRM.GetString("Text2") %>
						<br />
						<br />
						<table class="ibn-propertysheet" id="Table2" cellspacing="3" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="right" width="40%">
									<%=LocRM.GetString("OverallStatus") %>:&nbsp;&nbsp;
								</td>
								<td>
									<asp:dropdownlist id="ddPercentOS" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;
								</td>
							</tr>
							<tr>
								<td align="right" valign="top">
									<%=LocRM.GetString("SpentHours") %>:&nbsp;&nbsp;
								</td>
								<td>
									<ibn:Time id="dtcTimesheetHoursOS" ShowTime="HM" HourSpinMaxValue="24" ViewStartDate="false" runat="server" />
									<asp:customvalidator id="CustomValidator2" runat="server" Display="Dynamic" ErrorMessage="Can't update" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<%=LocRM.GetString("Date") %>:&nbsp;&nbsp;
								</td>
								<td>
									<mc:Picker ID="dtcOS" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
								</td>
							</tr>
							<tr>
								<td align="right"></td>
								<td>
									<btn:IMButton class="text" id="btnUpdateOS" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" onserverclick="btnUpdateOS_ServerClick"></btn:IMButton>
								</td>
							</tr>
						</table>
						<br />
					</td>
				</tr>
				<tr id="trOSOnly" runat="server">
					<td>
						<%=LocRM.GetString("Text2") %>
						<br />
						<br />
						<table class="ibn-propertysheet" id="Table3" cellspacing="3" cellpadding="0" width="100%" border="0">
							<tr>
								<td align="right" width="40%">
									<%=LocRM.GetString("OverallStatus") %>:&nbsp;&nbsp;
								</td>
								<td>
									<asp:dropdownlist id="ddPercentOSOnly" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;
								</td>
							</tr>
							<tr>
								<td align="right"></td>
								<td>
									<btn:IMButton class="text" id="btnUpdateOSOnly" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" onserverclick="btnUpdateOSOnly_ServerClick"></btn:IMButton>
								</td>
							</tr>
						</table>
						<br />
					</td>
				</tr>
				<tr id="trTimeTracker" runat="server">
					<td>
						<%=LocRM.GetString("UpdateTimesheetText") %>
						<br />
						<br />
						<table width="100%" border="0" cellpadding="0" cellspacing="3" class="ibn-propertysheet">
							<tr>
								<td align="right" valign="top" width="40%">
									<%=LocRM.GetString("SpentHours") %>: &nbsp;&nbsp;
								</td>
								<td>
									<ibn:Time id="dtcTimesheetHours" ShowTime="HM" HourSpinMaxValue="24" ViewStartDate="false" runat="server" />
									<asp:customvalidator id="CustomValidator3" runat="server" Display="Dynamic" ErrorMessage="Can't update" />
								</td>
							</tr>
							<tr>
								<td align="right">
									<%=LocRM.GetString("Date") %>: &nbsp;&nbsp;
								</td>
								<td>
									<mc:Picker ID="dtc" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
								</td>
							</tr>
							<tr>
								<td align="right"></td>
								<td>
									<btn:IMButton class="text" id="btnUpdateTimeTracking" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" onserverclick="btnUpdateTimeTracking_ServerClick"></btn:IMButton>
								</td>
							</tr>
						</table>
						<br />
					</td>
				</tr>
				<tr id="trAT" runat="server">
					<td>
						<%=LocRM.GetString("Text8") %>
						<br />
						<br />
						<div style="WIDTH:100%" align="center">
							<btn:IMButton class=text id="btnActivateTD" Text='<%# LocRM.GetString("ActivateTask") %>' Runat="server" style="width:125px;" onserverclick="btnActivateTD_ServerClick"></btn:IMButton>
						</div>
						<br />
					</td>
				</tr>
				<tr id="trCT" runat="server">
					<td>
						<asp:Label Runat="server" ID="lblComplete"></asp:Label>
						<asp:Label Runat="server" ID="lblPhase"></asp:Label>
						<br />
						<br />
						<div style="WIDTH:100%" align="center">
							<btn:IMButton class=text id=btnCompleteTD Text='<%# LocRM.GetString("CompleteTask") %>' Runat="server" style="width:120px;" onserverclick="btnCompleteTD_ServerClick"></btn:IMButton>
						</div>
						<br />
					</td>
				</tr>
				<tr id="trST" runat="server">
					<td>
						<%=LocRM.GetString("Text4") %>
						<br />
						<br />
						<div style="WIDTH:100%" align="center">
							<btn:IMButton class=text id="btnSuspendTD" Text='<%# LocRM.GetString("SuspendTask") %>' Runat="server" style="width:150px;" onserverclick="btnSuspendTD_ServerClick"></btn:IMButton>
						</div>
						<br />
					</td>
				</tr>
				<tr id="trUT" runat="server">
					<td>
						<%=LocRM.GetString("Text5") %>
						<br />
						<br />
						<div style="WIDTH:100%" align="center">
							<btn:IMButton class=text id="btnUncompleteTD"  Text='<%# LocRM.GetString("UncompleteTask") %>' Runat="server" style="width:160px;" onserverclick="btnUncompleteTD_ServerClick"></btn:IMButton>
						</div>
						<br />
					</td>
				</tr>
				<tr id="trRT" runat="server">
					<td>
						<%=LocRM.GetString("Text6") %>
						<br />
						<br />
						<div style="WIDTH:100%" align="center">
							<btn:IMButton class=text id="btnResumeTask" Text='<%# LocRM.GetString("ResumeTask") %>' Runat="server" style="width:120px;" onserverclick="btnResumeTask_ServerClick"></btn:IMButton>
						</div>
						<br />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
