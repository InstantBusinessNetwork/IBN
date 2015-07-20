<%@ Control Language="c#" Inherits="Mediachase.UI.Web.ToDo.Modules.QTracker" Codebehind="QTracker.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeaderLightWithMenu.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<table cellspacing="0" cellpadding="0" width="100%" border="0" style="margin-top:5px;"><tr><td>
<ibn:blockheader id="tbView" runat="server"></ibn:blockheader>
</td></tr></table>
<table class="ibn-stylebox-light ibn-propertysheet" cellspacing="0" cellpadding="1" width="100%" border="0">
	<tr>
		<td>
			<table class="text" cellspacing="0" cellpadding="3" width="100%" border="0">
				<tr>
					<td align="center" width="80" rowspan="2"><img height="98" alt="" src="../Layouts/Images/check.gif" width="60" border="0" /></td>
					<td valign="top">
						<table class="text" cellspacing="0" cellpadding="7" width="100%" border="0">
							<tr id="trAD" runat="server">
								<td>
									<%=LocRM.GetString("Text7") %>
									<br/>
									<br/>
									<div style="WIDTH: 100%" align="center">
										<btn:IMButton class=text id=btnAccept Runat="server" Text='<%# LocRM.GetString("Accept") %>' onserverclick="btnAccept_ServerClick" style="width:120px;"></btn:IMButton>&nbsp;&nbsp;
										<btn:IMButton class=text id=btnDecline Runat="server" Text='<%# LocRM.GetString("Decline") %>' isdecline="true" onserverclick="btnDecline_ServerClick" style="width:120px;"></btn:IMButton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trPS" runat="server">
								<td>
									<%=LocRM.GetString("Text1") %>
									<br/>
									<br/>
									<table class="ibn-propertysheet" id="Table1" cellspacing="3" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="right" width="40%"><%=LocRM.GetString("PersonalStatus") %>:&nbsp;&nbsp;</td>
											<td><asp:dropdownlist id="ddPercentPS" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;</td>
										</tr>
										<tr>
											<td align="right" valign="top">
												<%=LocRM.GetString("SpentHours") %>:&nbsp;&nbsp;
											</td>
											<td style="HEIGHT: 15px">
												<ibn:Time id="dtcTimesheetHoursPS" ShowTime="HM" HourSpinMaxValue="24" ViewStartDate="false" runat="server" />
												<asp:customvalidator id="CustomValidator1" runat="server" Display="Dynamic" ErrorMessage="Can't update" />
											</td>
										</tr>
										<tr>
											<td align="right">
												<%=LocRM.GetString("Date") %>:&nbsp;&nbsp;
											</td>
											<td>
												<mc:Picker ID="dtcPS" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
											</td>
										</tr>
										<tr>
											<td align="right"></td>
											<td><btn:IMButton class=text id=btnUpdatePS Runat="server" Text='<%# LocRM.GetString("Update") %>' onserverclick="btnUpdatePS_ServerClick"></btn:IMButton></td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trPSOnly" runat="server">
								<td>
									<%=LocRM.GetString("Text1") %>
									<br/>
									<br/>
									<table class="ibn-propertysheet" cellspacing="3" cellpadding="0" width="100%" border="0">
										<tr>
											<td align="right" width="40%"><%=LocRM.GetString("PersonalStatus") %>:&nbsp;&nbsp;</td>
											<td><asp:dropdownlist id="ddPercentPSOnly" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;</td>
										</tr>
										<tr>
											<td align="right"></td>
											<td><btn:IMButton class="text" id="btnUpdatePSOnly" Runat="server" Text='<%# LocRM.GetString("Update") %>' onserverclick="btnUpdatePSOnly_ServerClick"></btn:IMButton></td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trOS" runat="server">
								<td>
									<%=LocRM.GetString("Text2") %>
									<br/>
									<br/>
									<table class="ibn-propertysheet" id="Table2" cellspacing="3" cellpadding="0" width="100%"
										border="0">
										<tr>
											<td align="right" width="40%">
												<%=LocRM.GetString("OverallStatus") %>:&nbsp;&nbsp;
											</td>
											<td><asp:dropdownlist id="ddPercentOS" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;</td>
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
											<td><btn:IMButton class=text id=btnUpdateOS Runat="server" Text='<%# LocRM.GetString("Update") %>' onserverclick="btnUpdateOS_ServerClick"></btn:IMButton></td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trOSOnly" runat="server">
								<td>
									<%=LocRM.GetString("Text2") %>
									<br/>
									<br/>
									<table class="ibn-propertysheet" cellspacing="3" cellpadding="0" width="100%"
										border="0">
										<tr>
											<td align="right" width="40%">
												<%=LocRM.GetString("OverallStatus") %>:&nbsp;&nbsp;
											</td>
											<td><asp:dropdownlist id="ddPercentOSOnly" Runat="server" CssClass="text"></asp:dropdownlist>&nbsp;&nbsp;</td>
										</tr>
										<tr>
											<td align="right"></td>
											<td><btn:IMButton class="text" id="btnUpdateOSOnly" Runat="server" Text='<%# LocRM.GetString("Update") %>' onserverclick="btnUpdateOSOnly_ServerClick"></btn:IMButton></td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trTimeTracker" runat="server">
								<td>
									<%=LocRM.GetString("UpdateTimesheetText") %>
									<br/>
									<br/>
									<table class="ibn-propertysheet" cellspacing="3" cellpadding="0" width="100%" border="0">
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
												<%=LocRM.GetString("Date") %>
												: &nbsp;&nbsp;
											</td>
											<td>
												<mc:Picker ID="dtc" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
											</td>
										</tr>
										<tr>
											<td align="right"></td>
											<td><btn:IMButton class=text id=btnUpdateTT Runat="server" Text='<%# LocRM.GetString("Update") %>' onserverclick="btnUpdateTT_Click"></btn:IMButton></td>
										</tr>
									</table>
									<br />
								</td>
							</tr>
							<tr id="trAT" runat="server">
								<td>
									<%=LocRM.GetString("Text8") %>
									<br/>
									<br/>
									<div style="WIDTH:100%" align="center">
										<btn:IMButton class=text id="btnActivateTD" style="width:125px;" Text='<%# LocRM.GetString("ActivateTodo") %>' Runat="server" onserverclick="btnActivateTD_ServerClick"></btn:IMButton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trCT" runat="server">
								<td>
									<%=LocRM.GetString("Text3") %>
									<br/>
									<br/>
									<div style="WIDTH: 100%" align="center">
										<btn:IMButton class=text id=btnCompleteTD Text='<%# LocRM.GetString("CompleteToDo") %>' Runat="server" onserverclick="btnCompleteTD_ServerClick"></btn:IMButton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trST" runat="server">
								<td>
									<%=LocRM.GetString("Text4") %>
									<br/>
									<br/>
									<div style="WIDTH: 100%" align="center">
										<btn:IMButton class=text id=btnSuspendTD Runat="server" Text='<%# LocRM.GetString("SuspendToDo") %>' onserverclick="btnSuspendTD_ServerClick"></btn:IMButton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trUT" runat="server">
								<td>
									<%=LocRM.GetString("Text5") %>
									<br/>
									<br/>
									<div style="WIDTH: 100%" align="center">
										<btn:IMButton class=text id=btnUncompleteTD Runat="server" Text='<%# LocRM.GetString("UncompleteToDo") %>' onserverclick="btnUncompleteTD_ServerClick"></btn:IMButton>
									</div>
									<br />
								</td>
							</tr>
							<tr id="trRT" runat="server">
								<td>
									<%=LocRM.GetString("Text6") %>
									<br/>
									<br/>
									<div style="WIDTH:100%" align="center">
										<btn:IMButton class=text id="btnResumeToDo"  Text='<%# LocRM.GetString("ResumeToDo") %>' Runat="server" onserverclick="btnResumeToDo_ServerClick"></btn:IMButton>
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
