<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Documents.Modules.QuickTracking" Codebehind="QuickTracking.ascx.cs" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;"><tr><td>
<ibn:blockheader id="tbView" runat="server"></ibn:blockheader>
</td></tr></table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="1" width="100%" border="0">
	<tr>
		<td>
			<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
				<tr>
					<td align="middle" width="80" rowSpan="2"><IMG height="98" alt="" src="../Layouts/Images/check.gif" width="60" border="0">
					</td>
					<td vAlign="top">
						<table cellspacing="0" cellpadding="7" border="0" class="text" width="100%">
							<tr id="trNewVersion" runat="server">
								<td>
									<%=LocRM.GetString("AddVersionText") %>
									<br />
									<br />
									<div style="WIDTH:100%" align="center">
										<btn:imbutton class=text id="btnNewVersion" Text='<%# LocRM.GetString("AddVersion") %>' Runat="server"></btn:imbutton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trAD" runat="server">
								<td>
									<%=LocRM.GetString("Text7") %>
									<br />
									<br />
									<div style="WIDTH:100%" align="center">
										<btn:imbutton class=text id="btnAccept"  Text='<%# LocRM.GetString("Accept") %>' Runat="server" style="width:110px;" CausesValidation="false"></btn:imbutton>&nbsp;&nbsp;
										<btn:imbutton class=text id="btnDecline"  Text='<%# LocRM.GetString("Decline") %>' Runat="server" isdecline="true" style="width:110px;" CausesValidation="false"></btn:imbutton>
									</div>
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
												<asp:customvalidator id="cvHours" runat="server" Display="Dynamic" ErrorMessage="Can't update" ValidationGroup="TimeTracking" />
											</td>
										</tr>
										<tr>
											<td align="right">
												<%=LocRM.GetString("Date") %>: &nbsp;&nbsp;
											</td>
											<td><mc:Picker ID="dtc" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
										</tr>
										<tr>
											<td align="right"></td>
											<td>
												<btn:imbutton class="text" id="ibUpdate" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" ValidationGroup="TimeTracking"></btn:imbutton>
											</td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trStatus" runat="server">
								<td>
									<%=LocRM.GetString("Text2") %>
									<br />
									<br />
									<table class="ibn-propertysheet" cellspacing="3" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="right" width="40%">
												<%=LocRM.GetString("Status") %>:&nbsp;&nbsp;
											</td>
											<td>
												<asp:dropdownlist id="ddStatus" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;
											</td>
										</tr>
										<tr>
											<td align="right"></td>
											<td><btn:imbutton class=text id=btnUpdateStatus Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" CausesValidation="false"></btn:imbutton></td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trTTStatus" runat="server">
								<td>
									<%=LocRM.GetString("Text1") %>
									<br />
									<br />
									<table width="100%" border="0" cellpadding="0" cellspacing="3" class="ibn-propertysheet">
										<tr>
											<td align="right" width="40%">
												<%=LocRM.GetString("Status") %>:&nbsp;&nbsp;
											</td>
											<td>
												<asp:dropdownlist id="ddStatus2" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;
											</td>
										</tr>
										<tr>
											<td align="right" valign="top">
												<%=LocRM.GetString("SpentHours") %>: &nbsp;&nbsp;
											</td>
											<td>
												<ibn:Time id="dtcTimesheetHours2" ShowTime="HM" HourSpinMaxValue="24" ViewStartDate="false" runat="server" />
												<asp:customvalidator id="cvHours2" runat="server" Display="Dynamic" ErrorMessage="Can't update" ValidationGroup="TimeTracking" />
											</td>
										</tr>
										<tr>
											<td align="right">
												<%=LocRM.GetString("Date") %>: &nbsp;&nbsp;
											</td>
											<td><mc:Picker ID="dtc2" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
										</tr>
										<tr>
											<td align="right"></td>
											<td>
												<btn:imbutton class="text" id="btnUpdateTTStatus" Runat="server" Text='<%# LocRM.GetString("Update") %>' style="width:110px;" ValidationGroup="TimeTracking"></btn:imbutton>
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
										<btn:imbutton class=text id="btnActivate" Text='<%# LocRM.GetString("ActivateDocument") %>' Runat="server" style="width:125px;" CausesValidation="false"></btn:imbutton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trCD" runat="server">
								<td>
									<%=LocRM.GetString("Text3") %>
									<br />
									<br />
									<div style="WIDTH:100%" align="center">
										<btn:imbutton class=text id=btnComplete Text='<%# LocRM.GetString("Complete") %>' Runat="server" style="width:110px;" CausesValidation="false" ValidationGroup="TimeTracking"></btn:imbutton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trSD" runat="server">
								<td>
									<%=LocRM.GetString("Text4") %>
									<br />
									<br />
									<div style="WIDTH:100%" align="center">
										<btn:imbutton class=text id="btnSuspend" Text='<%# LocRM.GetString("Suspend") %>' Runat="server" CausesValidation="false" ValidationGroup="TimeTracking"></btn:imbutton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trUD" runat="server">
								<td>
									<%=LocRM.GetString("Text5") %>
									<br />
									<br />
									<div style="WIDTH:100%" align="center">
										<btn:imbutton class=text  id="btnUncomplete" Text='<%# LocRM.GetString("Uncomplete") %>' Runat="server" CausesValidation="false" ValidationGroup="TimeTracking"></btn:imbutton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trRD" runat="server">
								<td>
									<%=LocRM.GetString("Text6") %>
									<br />
									<br />
									<div style="WIDTH:100%" align="center">
										<btn:imbutton class=text id="btnResume" Text='<%# LocRM.GetString("Resume") %>' Runat="server" style="width:110px;" CausesValidation="false" ValidationGroup="TimeTracking"></btn:imbutton>
									</div>
									<br />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
