<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReccurenceEdit.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.Calendar.Modules.ReccurenceEdit" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc2" Namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls"%>
<script type="text/javascript">
	function SelRecType(block) {
		document.getElementById('tblDaily').style.display = "none";
		document.getElementById('tblWeekly').style.display = "none";
		document.getElementById('tblMonthly').style.display = "none";
		document.getElementById('tblYearly').style.display = "none";

		switch (block) {
			case 0:
				document.getElementById('tblDaily').style.display = "";
				break;
			case 1:
				document.getElementById('tblWeekly').style.display = "";
				break;
			case 2:
			case 3:
				document.getElementById('tblMonthly').style.display = "";
				break;
			case 4:
			case 5:
				document.getElementById('tblYearly').style.display = "";
				break;
		}
	}
</script>
<table class="ibn-propertysheet" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td>
			<table class="text" id="tbl" cellspacing="3" cellpadding="3" border="0">
				<tr>
					<td>
						<mc2:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secHeader" runat="server" />
						<table class="ibn-stylebox-light text" cellspacing="5" cellpadding="0" width="100%" border="0">
							<tr>
								<td valign="top" width="105px">
									<p>
										<input id="rbRecType1" onclick="SelRecType(0)" type="radio" checked="true" value="1" name="rbRT" runat="server" /><label for='<%=rbRecType1.ClientID %>'>
											<%=LocRM.GetString("Daily")%>
										</label>
										<br />
										<input id="rbRecType2" onclick="SelRecType(1)" type="radio" value="2" name="rbRT" runat="server" /><label for='<%=rbRecType2.ClientID %>'>
											<%=LocRM.GetString("Weekly")%>
										</label>
										<br />
										<input id="rbRecType3" onclick="SelRecType(2)" type="radio" value="3" name="rbRT" runat="server" /><label for='<%=rbRecType3.ClientID %>'>
											<%=LocRM.GetString("Monthly")%>
										</label>
										<br />
										<input id="rbRecType5" onclick="SelRecType(4)" type="radio" value="5" name="rbRT" runat="server" /><label for='<%=rbRecType5.ClientID %>'>
											<%=LocRM.GetString("Yearly")%>
										</label>
									</p>
								</td>
								<td width="1px"></td>
								<td valign="top">
									<table class="text" id="tblDaily" cellspacing="5" cellpadding="0" border="0">
										<tr>
											<td>&nbsp;<%=LocRM.GetString("Every")%>&nbsp;&nbsp;
												<asp:textbox id="tbFreq1" runat="server" width="30px" cssclass="text">1</asp:textbox><asp:requiredfieldvalidator id="rfvDay" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq1"></asp:requiredfieldvalidator><asp:rangevalidator id="rvDay" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq1" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;
											</td>
											<td>
												<input id="rbRecType11" type="radio" checked="true" value="rbDRecType1" name="rbDR" runat="server" /><label for='<%=rbRecType11.ClientID %>'>
													<%=LocRM.GetString("Day").ToLower()%>
												</label>
											</td>
										</tr>
										<tr>
											<td>&nbsp;<%=LocRM.GetString("Every")%>&nbsp;&nbsp;</td>
											<td><input id="rbRecType12" type="radio" value="rbDRecType2" name="rbDR" runat="server" />&nbsp;<label for='<%=rbRecType12.ClientID %>'><%=LocRM.GetString("weekday")%></label></td>
										</tr>
									</table>
									<div id="Weekday2" runat="server">
										<table class="text" id="tblWeekly" style="display: none" cellspacing="5" width="100%" border="0">
											<tr>
												<td colspan="4"><%=LocRM.GetString("Recurevery")%>&nbsp;&nbsp;&nbsp;
													<asp:textbox id="tbFreq2" runat="server" width="30" cssclass="text"></asp:textbox><asp:requiredfieldvalidator id="rfvWeek" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq2"></asp:requiredfieldvalidator><asp:rangevalidator id="rvWeek" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq2" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>
													&nbsp;&nbsp;&nbsp;<%=LocRM.GetString("week(s)on")%>:
												</td>
											</tr>
											<tr>
												<td width="25%"><input id="day1" type="checkbox" value="1" name="cbWSelDay1" runat="server" /><label for='<%=day1.ClientID %>'><%=LocRM.GetString("Monday").ToLower()%></label></td>
												<td width="25%"><input id="day2" type="checkbox" value="2" name="cbWSelDay2" runat="server" /><label for='<%=day2.ClientID %>'><%=LocRM.GetString("Tuesday").ToLower()%></label></td>
												<td width="25%"><input id="day4" type="checkbox" value="4" name="cbWSelDay3" runat="server" /><label for='<%=day4.ClientID %>'><%=LocRM.GetString("Wednesday").ToLower()%></label></td>
												<td width="25%"><input id="day8" type="checkbox" value="8" name="cbWSelDay4" runat="server" /><label for='<%=day8.ClientID %>'><%=LocRM.GetString("Thursday").ToLower()%></label></td>
											</tr>
											<tr>
												<td width="25%"><input id="day16" type="checkbox" value="16" name="cbWSelDay5" runat="server" /><label for='<%=day16.ClientID %>'><%=LocRM.GetString("Friday").ToLower()%></label></td>
												<td width="25%"><input id="day32" type="checkbox" value="32" name="cbWSelDay6" runat="server" /><label for='<%=day32.ClientID %>'><%=LocRM.GetString("Saturday").ToLower()%></label></td>
												<td width="25%"><input id="day64" type="checkbox" value="64" name="cbWSelDay7" runat="server" /><label for='<%=day64.ClientID %>'><%=LocRM.GetString("Sunday").ToLower()%></label></td>
												<td width="25%">&nbsp;<asp:customvalidator id="cvWeek" runat="server" ErrorMessage="*"></asp:customvalidator></td>
											</tr>
										</table>
									</div>
									<table class="text" id="tblMonthly" style="display: none" cellspacing="5" width="100%" border="0">
										<tr>
											<td width="65px"><input id="rbRecType31" type="radio" value="rbMRecType1" name="rbMR" runat="server" /><label for='<%=rbRecType31.ClientID %>'>
													<%=LocRM.GetString("Day")%>&nbsp;</label>
											</td>
											<td>
												<asp:textbox id="tbMonthDay3" runat="server" CssClass="text" width="30"></asp:textbox>&nbsp;
												<asp:requiredfieldvalidator id="rfvMonth1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay3"></asp:requiredfieldvalidator>
												<asp:rangevalidator id="rvMonth1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay3" MaximumValue="31" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;
												<%=LocRM.GetString("ofevery")%>&nbsp;&nbsp;&nbsp;
												<asp:textbox id="tbFreq3" runat="server" CssClass="text" width="30"></asp:textbox>&nbsp;
												<asp:requiredfieldvalidator id="rfvMonth2" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq3"></asp:requiredfieldvalidator>
												<asp:rangevalidator id="rvMonth2" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq3" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;
												<%=LocRM.GetString("month(s)")%>
											</td>
										</tr>
										<tr>
											<td width="65px"><input id="rbRecType41" type="radio" value="rbMRecType2" name="rbMR" runat="server" /><label for='<%=rbRecType41.ClientID %>'>
													<%=LocRM.GetString("The")%>&nbsp;</label>
											</td>
											<td valign="top">
												<asp:dropdownlist id="WeekNumber4" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
												<asp:dropdownlist id="Weekday4" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
												<%=LocRM.GetString("ofevery")%>&nbsp;&nbsp;&nbsp;
												<asp:textbox id="tbFreq4" runat="server" CssClass="text" width="30px" Height="18px"></asp:textbox>
												<asp:requiredfieldvalidator id="rfvMonth3" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq4"></asp:requiredfieldvalidator>
												<asp:rangevalidator id="rvMonth3" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq4" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;&nbsp;
												<%=LocRM.GetString("month(s)")%>
											</td>
										</tr>
									</table>
									<table class="text" id="tblYearly" style="display: none" cellspacing="5" width="100%" border="0">
										<tr>
											<td width="75px"><input id="rbRecType51" type="radio" value="rbYRecType1" name="rbYR" runat="server" /><label for='<%=rbRecType51.ClientID %>'>
													<%=LocRM.GetString("Every")%></label>
											</td>
											<td>
												<asp:dropdownlist id="MonthNumber5" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
												<asp:textbox id="tbMonthDay5" runat="server" width="30px" cssclass="text"></asp:textbox>
												<asp:requiredfieldvalidator id="rfvYear" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay5"></asp:requiredfieldvalidator>
												<asp:rangevalidator id="rvYear" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay5" MaximumValue="31" MinimumValue="1" Type="Integer"></asp:rangevalidator>
											</td>
										</tr>
										<tr>
											<td width="75px"><input id="rbRecType61" type="radio" value="rbYRecType2" name="rbYR" runat="server" /><label for='<%=rbRecType61.ClientID %>'>
													<%=LocRM.GetString("The")%>
												</label>
											</td>
											<td valign="top">
												<asp:dropdownlist id="WeekNumber6" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
												<asp:dropdownlist id="Weekday6" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
												<%=LocRM.GetString("of")%>&nbsp;&nbsp;&nbsp;
												<asp:dropdownlist id="MonthNumber6" runat="server" cssclass="text"></asp:dropdownlist>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<br />
						<mc2:BlockHeaderLight2 HeaderCssClass="ibn-toolbar-light" ID="secHeader2" runat="server" />
						<table class="ibn-stylebox-light text" id="table1" cellspacing="5" width="100%" border="0">
							<tr>
								<td width="260px"><%=LocRM.GetString("Start")%>:<mc:Picker ID="dtcDateStart" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
								<td>
									<input id="rbEndBy" type="radio" checked="true" value="chbEndBy" name="rbR" runat="server" /><label for='<%=rbEndBy.ClientID %>'>
										<%=LocRM.GetString("Endby")%></label>:
								</td>
								<td>
									<table cellpadding="0" cellspacing="0" border="0">
										<tr>
											<td><mc:Picker ID="dtcDateEnd" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
											<td>&nbsp;<asp:CustomValidator id="cvDate2" runat="server" ErrorMessage="*" CssClass="text"></asp:CustomValidator></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td width="260px">&nbsp;</td>
								<td width="135px"><input id="rbEndAfter" type="radio" value="chbEndAfter" name="rbR" runat="server" /><label for='<%=rbEndAfter.ClientID %>'>
										<%=LocRM.GetString("Endafter")%></label>:
								</td>
								<td><asp:textbox id="tbEndAfter" runat="server" width="30" cssclass="text"></asp:textbox>
									<asp:requiredfieldvalidator id="rfvDate1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbEndAfter"></asp:requiredfieldvalidator>
									<asp:rangevalidator id="rvDate1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbEndAfter" MaximumValue="65536" MinimumValue="1" Type="Integer"></asp:rangevalidator>
									&nbsp;<%=LocRM.GetString("occurrences")%>
								</td>
							</tr>
						</table>
						<asp:button id="addRec" style="display: none" runat="server" text="OK"></asp:button>
						<asp:button id="btnRemove" style="display: none" runat="server"></asp:button>
					</td>
				</tr>
				<tr>
					<td valign="middle" align="right" height="60px">
						<mc2:IMButton class="text" id="btnSave" style="width:110px;" Runat="server" onserverclick="btnSave_ServerClick"></mc2:IMButton>&nbsp;&nbsp;
						<mc2:IMButton class="text" id="btnCancel" style="width:110px;" Runat="server" CausesValidation="false"></mc2:IMButton>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<div><input id="txtManagerId" style="visibility: hidden" name="iGroups" runat="server" /></div>
<script type="text/javascript">
	SelRecType(<%=Pattern - 1 %>); 
</script>