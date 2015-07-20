<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Events.Modules.RecEditor" Codebehind="RecEditor.ascx.cs" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="../../Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="btn" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<script type="text/javascript">
function SelRecType(block)
{
	document.getElementById('tblDaily').style.display="none";
	document.getElementById('tblWeekly').style.display="none";
	document.getElementById('tblMonthly').style.display="none";
	document.getElementById('tblYearly').style.display="none";
	
	switch (block)
	{
		case 0:
			document.getElementById('tblDaily').style.display="";
			break;
		case 1:
			document.getElementById('tblWeekly').style.display="";
			break;
		case 2:
			document.getElementById('tblMonthly').style.display="";
			break;
		case 3:
			document.getElementById('tblYearly').style.display="";
	}
	
}
</script>

<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="Migrated_secHeader" title="Edit Recurring Pattern" runat="server"></ibn:blockheader></td>
	</tr>
	<tr>
		<td>
			<table class="text" id="tbl" cellspacing="3" cellpadding="3" border="0">
				<tr>
					<td class="text">
						<%=LocRM.GetString("TimeZone") %>
						:
						<asp:dropdownlist id="lstTimeZone" runat="server" CssClass="text"></asp:dropdownlist><br><br>
						<fieldset><LEGEND class="text ibn-label" id="Legend1" runat="server" NAME="Legend1"></LEGEND>
							<table class="text" id="tblRangeRec" cellspacing="5" border="0">
								<tr>
									<td style="width:80px;"><%=LocRM.GetString("Start")%>:</td>
									<td style="width:100px">
										<mc:Picker ID="dtcTimeStart" runat="server" TimeCssClass="text" TimeOnly="true" TimeWidth="65px" />
									</td>
									<td style="width:80px"><%=LocRM.GetString("End")%>:</td>
									<td style="width:100px">
										<mc:Picker ID="dtcTimeEnd" runat="server" TimeCssClass="text" TimeOnly="true" TimeWidth="65px" />
									</td>
									<td>
										<asp:customvalidator id="cvDates" runat="server" ErrorMessage="*"></asp:customvalidator>
									</td>
								</tr>
							</table>
						</FIELDSET>
						<BR>
						<FIELDSET><LEGEND class="text ibn-label" id="Legend2" runat="server"></LEGEND>
							<table class="text" height="100" cellspacing="5" cellpadding="0" width="100%" border="0">
								<tr>
									<td vAlign="top" width="105">
										<P>
											<INPUT id="rbRecType1" onclick="SelRecType(0)" type="radio" CHECKED value="1" name="rbRT" runat="server"><label for='<%=rbRecType1.ClientID %>'>
												<%=LocRM.GetString("Daily")%>
											</label>
											<BR>
											<INPUT id="rbRecType2" onclick="SelRecType(1)" type="radio" value="2" name="rbRT" runat="server"><label for='<%=rbRecType2.ClientID %>'>
												<%=LocRM.GetString("Weekly")%>
											</label>
											<BR>
											<INPUT id="rbRecType3" onclick="SelRecType(2)" type="radio" value="3" name="rbRT" runat="server"><label for='<%=rbRecType3.ClientID %>'>
												<%=LocRM.GetString("Monthly")%>
											</label>
											<BR>
											<INPUT id="rbRecType4" onclick="SelRecType(3)" type="radio" value="4" name="rbRT" runat="server"><label for='<%=rbRecType4.ClientID %>'>
												<%=LocRM.GetString("Yearly")%>
											</label>
										</P>
									</td>
									<td width="1" bgColor="lightblue"></td>
									<td vAlign="top">
										<table class="text" id="tblDaily" cellspacing="5" cellpadding="0" width="100%" border="0" name="tblDaily">
											<tr>
												<td>&nbsp;<%=LocRM.GetString("Every")%>&nbsp;&nbsp;
													<asp:textbox id="tbFreq1" runat="server" width="30" cssclass="text">1</asp:textbox><asp:requiredfieldvalidator id="rfvDay" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq1"></asp:requiredfieldvalidator><asp:rangevalidator id="rvDay" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq1" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;
													<INPUT id="rbRecType11" type="radio" CHECKED value="rbDRecType1" name="rbDR" runat="server"><label for='<%=rbRecType11.ClientID %>'>
														<%=LocRM.GetString("Day").ToLower()%>
													</label><INPUT id="rbRecType12" type="radio" value="rbDRecType2" name="rbDR" runat="server">&nbsp;<label for='<%=rbRecType12.ClientID %>'><%=LocRM.GetString("weekday")%></label></td>
											</tr>
										</table>
										<div id="Weekday2" runat="server">
											<table class="text" id="tblWeekly" style="DISPLAY: none" cellspacing="5" width="90%" border="0">
												<tr>
													<td colSpan="4"><%=LocRM.GetString("Recurevery")%>&nbsp;&nbsp;&nbsp;
														<asp:textbox id="tbFreq2" runat="server" width="30" cssclass="text"></asp:textbox><asp:requiredfieldvalidator id="rfvWeek" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq2"></asp:requiredfieldvalidator><asp:rangevalidator id="rvWeek" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq2" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;&nbsp;<%=LocRM.GetString("week(s)on")%>
														:</td>
												</tr>
												<tr>
													<td width="25%"><INPUT id="day1" type="checkbox" value="1" name="cbWSelDay1" runat="server"><label for='<%=day1.ClientID %>'><%=LocRM.GetString("Monday").ToLower()%></label></td>
													<td width="25%"><INPUT id="day2" type="checkbox" value="2" name="cbWSelDay2" runat="server"><label for='<%=day2.ClientID %>'><%=LocRM.GetString("Tuesday").ToLower()%></label></td>
													<td width="25%"><INPUT id="day4" type="checkbox" value="4" name="cbWSelDay3" runat="server"><label for='<%=day4.ClientID %>'><%=LocRM.GetString("Wednesday").ToLower()%></label></td>
													<td width="25%"><INPUT id="day8" type="checkbox" value="8" name="cbWSelDay4" runat="server"><label for='<%=day8.ClientID %>'><%=LocRM.GetString("Thursday").ToLower()%></label></td>
												</tr>
												<tr>
													<td width="25%"><INPUT id="day16" type="checkbox" value="16" name="cbWSelDay5" runat="server"><label for='<%=day16.ClientID %>'><%=LocRM.GetString("Friday").ToLower()%></label></td>
													<td width="25%"><INPUT id="day32" type="checkbox" value="32" name="cbWSelDay6" runat="server"><label for='<%=day32.ClientID %>'><%=LocRM.GetString("Saturday").ToLower()%></label></td>
													<td width="25%"><INPUT id="day64" type="checkbox" value="64" name="cbWSelDay7" runat="server"><label for='<%=day64.ClientID %>'><%=LocRM.GetString("Sunday").ToLower()%></label></td>
													<td width="25%">&nbsp;
														<asp:customvalidator id="cvWeek" runat="server" ErrorMessage="*"></asp:customvalidator></td>
												</tr>
											</table>
										</div>
										<table class="text" id="tblMonthly" style="DISPLAY: none" cellspacing="5" width="100%" border="0">
											<tr>
												<td width="65"><INPUT id="rbRecType31" type="radio" value="rbMRecType1" name="rbMR" runat="server"><label for='<%=rbRecType31.ClientID %>'>
														<%=LocRM.GetString("Day")%>
														&nbsp;</label></td>
												<td>
													<asp:textbox id="tbMonthDay31" runat="server" CssClass="text" width="30"></asp:textbox>&nbsp;
													<asp:requiredfieldvalidator id="rfvMonth1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay31"></asp:requiredfieldvalidator>
													<asp:rangevalidator id="rvMonth1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay31" MaximumValue="31" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;
													<%=LocRM.GetString("ofevery")%>
													&nbsp;&nbsp;&nbsp;
													<asp:textbox id="tbFreq31" runat="server" CssClass="text" width="30"></asp:textbox>&nbsp;
													<asp:requiredfieldvalidator id="rfvMonth2" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq31"></asp:requiredfieldvalidator>
													<asp:rangevalidator id="rvMonth2" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq31" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;
													<%=LocRM.GetString("month(s)")%>
												</td>
											</tr>
											<tr>
												<td width="65"><INPUT id="rbRecType32" type="radio" value="rbMRecType2" name="rbMR" runat="server"><label for='<%=rbRecType32.ClientID %>'>
														<%=LocRM.GetString("The")%>
														&nbsp;</label></td>
												<td vAlign="top">
													<asp:dropdownlist id="WeekNumber32" runat="server" name="select4" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
													<asp:dropdownlist id="Weekday32" runat="server" name="select5" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
													<%=LocRM.GetString("ofevery")%>
													&nbsp;&nbsp;&nbsp;
													<asp:textbox id="tbFreq32" runat="server" CssClass="text" width="30" Height="18"></asp:textbox>
													<asp:requiredfieldvalidator id="rfvMonth3" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq32"></asp:requiredfieldvalidator>
													<asp:rangevalidator id="rvMonth3" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbFreq32" MaximumValue="365" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;&nbsp;&nbsp;<%=LocRM.GetString("month(s)")%>
												</td>
											</tr>
										</table>
										<table class="text" id="tblYearly" style="DISPLAY: none" cellspacing="5" width="100%" border="0">
											<tr>
												<td width="75"><INPUT id="rbRecType41" type="radio" value="rbYRecType1" name="rbYR" runat="server"><label for='<%=rbRecType41.ClientID %>'>
														<%=LocRM.GetString("Every")%>
													</label>
												</td>
												<td><asp:dropdownlist id="MonthNumber41" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
													<asp:textbox id="tbMonthDay41" runat="server" width="30" cssclass="text"></asp:textbox>
													<asp:requiredfieldvalidator id="rfvYear" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay41"></asp:requiredfieldvalidator>
													<asp:rangevalidator id="rvYear" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbMonthDay41" MaximumValue="31" MinimumValue="1" Type="Integer"></asp:rangevalidator></td>
											</tr>
											<tr>
												<td width="75"><INPUT id="rbRecType42" type="radio" value="rbYRecType2" name="rbYR" runat="server"><label for='<%=rbRecType42.ClientID %>'>
														<%=LocRM.GetString("The")%>
													</label>
												</td>
												<td vAlign="top"><asp:dropdownlist id="WeekNumber42" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
													<asp:dropdownlist id="Weekday42" runat="server" cssclass="text"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;
													<%=LocRM.GetString("of")%>
													&nbsp;&nbsp;&nbsp;
													<asp:dropdownlist id="MonthNumber42" runat="server" cssclass="text"></asp:dropdownlist></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</FIELDSET>
						<BR>
						<FIELDSET><LEGEND class="text ibn-label" id="Legend3" runat="server" NAME="Legend1"></LEGEND>
							<table class="text" id="tblRangeRec" cellspacing="5" width="100%" border="0">
								<tr>
									<td width="260"><%=LocRM.GetString("Start")%>:<mc:Picker ID="dtcDateStart" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
									<td width="135"><INPUT id="rbEndAfter" type="radio" value="chbEndAfter" name="rbR" runat="server"><label for='<%=rbEndAfter.ClientID %>'>
											<%=LocRM.GetString("Endafter")%>
										</label>:</td>
									<td><asp:textbox id="tbEndAfter" runat="server" width="30" cssclass="text"></asp:textbox>
										<asp:requiredfieldvalidator id="rfvDate1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbEndAfter"></asp:requiredfieldvalidator>
										<asp:rangevalidator id="rvDate1" runat="server" ErrorMessage="*" EnableClientScript="False" Display="Dynamic" ControlToValidate="tbEndAfter" MaximumValue="65536" MinimumValue="1" Type="Integer"></asp:rangevalidator>&nbsp;<%=LocRM.GetString("occurrences")%></td>
								</tr>
								<tr>
									<td width="240">&nbsp;</td>
									<td><INPUT id="rbEndBy" type="radio" CHECKED value="chbEndBy" name="rbR" runat="server"><label for='<%=rbEndBy.ClientID %>'>
											<%=LocRM.GetString("Endby")%>
										</label>:</td>
									<td><table cellpadding="0" cellspacing="0" border="0">
											<tr>
												<td><mc:Picker ID="dtcDateEnd" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" /></td>
												<td>&nbsp;<asp:CustomValidator id="cvDate2" runat="server" ErrorMessage="*" CssClass=text></asp:CustomValidator></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</FIELDSET>
						<asp:button id="addRec" style="DISPLAY: none" runat="server" text="OK"></asp:button><asp:button id="btnRemove" style="DISPLAY: none" runat="server"></asp:button></td>
				</tr>
				<tr>
					<td valign="center" align="right" height="60"><btn:imbutton class="text" id="btnSave" style="width:110px;" Runat="server" onserverclick="btnSave_ServerClick"></btn:imbutton>&nbsp;&nbsp;
						<btn:imbutton class="text" id="btnCancel" style="width:110px;" Runat="server" IsDecline="true" CausesValidation="false" onserverclick="btnCancel_ServerClick"></btn:imbutton></td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<div><input id="txtManagerId" style="VISIBILITY: hidden" name="iGroups" runat="server" /></div>
<script type="text/javascript">
SelRecType(<%=Pattern - 1 %>); 
</script>
