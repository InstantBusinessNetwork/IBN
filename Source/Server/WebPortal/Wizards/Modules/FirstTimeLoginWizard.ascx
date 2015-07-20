<%@ Reference Control="~/Directory/Modules/PrefsEdit2.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Mediachase.FileUploader.Web.UI" Assembly="Mediachase.FileUploader" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.FirstTimeLoginWizard" Codebehind="FirstTimeLoginWizard.ascx.cs" %>
<%@ Register TagPrefix="usr" TagName="Prefs" src="..\..\Directory\Modules\PrefsEdit2.ascx" %>
<script language="javascript">
function fPhotochange()
{
	var strFile = document.forms[0].<%=fPhoto.ClientID%>.value;
	if (strFile != "")
	{
		//document.forms[0].imgPhoto.src = "file:///" + strFile;
	}
}
</script>
<asp:panel id="step1" Runat="server">
	<div class="text"><br>
	</div>
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td width="50%">
				<FIELDSET style="HEIGHT: 180px;"><LEGEND class="text" id="lgdContactInf" runat="server"></LEGEND>
					<table width="100%">
						<tr>
							<td class="text" style="PADDING-TOP:10px" width="100"><B><%=LocRM.GetString("tFirstName")%>:</B></td>
							<td style="PADDING-TOP:10px">
								<asp:TextBox id="txtFirstName" Runat="server" Width="150px" CssClass="text"></asp:TextBox>
								<asp:RequiredFieldValidator id="rfFirstName" Runat="server" CssClass="text" ControlToValidate="txtFirstName"
									Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator></td>
						</tr>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tLastName")%>:</B></td>
							<td>
								<asp:TextBox id="txtLastName" Runat="server" Width="150px" CssClass="text"></asp:TextBox>
								<asp:RequiredFieldValidator id="rfLastName" Runat="server" CssClass="text" ControlToValidate="txtLastName" Display="Dynamic"
									ErrorMessage="*"></asp:RequiredFieldValidator></td>
						</tr>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tEMail")%>:</B></td>
							<td>
								<asp:TextBox id="txtEMail" Runat="server" Width="150px" CssClass="text"></asp:TextBox>
								<asp:RequiredFieldValidator id="rfEMail" Runat="server" CssClass="text" ControlToValidate="txtEMail" Display="Dynamic"
									ErrorMessage="*"></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator id="revEMail" runat="server" ControlToValidate="txtEMail" Display="Dynamic" ErrorMessage="*"
									ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator></td>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tWorkPhone")%>:</B></td>
							<td>
								<asp:TextBox id="txtWorkPhone" Runat="server" Width="150px" CssClass="text"></asp:TextBox></td>
						</tr>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tMobilePhone")%>:</B></td>
							<td>
								<asp:TextBox id="txtMobilePhone" Runat="server" Width="150px" CssClass="text"></asp:TextBox></td>
						</tr>
					</table>
				</FIELDSET>
			</td>
			<td vAlign="top" align="middle" width="60"><IMG alt="" src="../layouts/images/quicktip.gif" border="0">
			</td>
			<td class="text" style="PADDING-RIGHT: 15px" vAlign="top"><%=LocRM.GetString("s1Comments") %></td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step2" Runat="server">
	<table cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td width="50%">
				<FIELDSET style="HEIGHT: 200px"><LEGEND class="text" id="lgdDetails" runat="server"></LEGEND>
					<table width="100%">
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tCompany")%>:</B></td>
							<td>
								<asp:TextBox id="txtCompany" Runat="server" Width="150px" CssClass="text"></asp:TextBox></td>
						</tr>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tDepartment")%>:</B></td>
							<td>
								<asp:TextBox id="txtDepartment" Runat="server" Width="150px" CssClass="text"></asp:TextBox></td>
						</tr>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tPosition")%>:</B></td>
							<td>
								<asp:TextBox id="txtPosition" Runat="server" Width="150px" CssClass="text"></asp:TextBox></td>
						</tr>
						<tr>
							<td class="text" width="100"><B><%=LocRM.GetString("tLocation")%>:</B></td>
							<td>
								<asp:TextBox id="txtLocation" Runat="server" Width="150px" CssClass="text"></asp:TextBox></td>
						</tr>
					</table>
				</FIELDSET>
			</td>
			<td>
				<table width="100%">
					<tr>
						<td width="15"></td>
						<td>
							<table>
								<tr>
									<td class="text" width="45"><B><%=LocRM.GetString("tPhoto")%>:</B></td>
									<td width="200">
										<cc1:McHtmlInputFile onkeypress="fPhotochange();" id="fPhoto" onpropertychange="fPhotochange();" onclick="fPhotochange();"
											runat="server" CssClass="text"></cc1:McHtmlInputFile><NOBR>
											<asp:RegularExpressionValidator id="revfPhoto" runat="server" ControlToValidate="fPhoto" ErrorMessage="*" ValidationExpression="^.+((\.[jJ][pP][gG])|(\.[jJ][pP][eE][gG])|(\.[gG][iI][fF])|(\.[pP][nN][gG])|(\.[tT][iI][fF][fF])|(\.[bB][mM][pP]))$"></asp:RegularExpressionValidator></NOBR></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td width="15"></td>
						<td style="PADDING-RIGHT: 17px" align="left" rowSpan="6">
							<div id="Picture" align="left" runat="server" visible="false">
								<table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
									<tr>
										<td vAlign="top" align="right" width="10"><IMG height="10" src="../layouts/images/photo-cornertleft.gif" width="10"></td>
										<td><IMG height="10" src="../layouts/images/photo-bgtop.gif" width="100%"></td>
										<td vAlign="top" align="left"><IMG height="10" src="../layouts/images/photo-cornertright.gif" width="17"></td>
									</tr>
									<tr>
										<td vAlign="top" align="left" width="10" background="../layouts/images/photo-bgleft.gif"><IMG height="1" src="../layouts/images/photo-bgleft.gif" width="10"></td>
										<td vAlign="center" align="middle" bgColor="#ffffff">
											<div align="center"><IMG id="imgPhoto" src="~/layouts/images/transparentpoint.gif" border="1" name="imgPhoto"
													runat="server"></div>
										</td>
										<td vAlign="top" align="left"><IMG height="126" src="../layouts/images/photo-bgright.gif" width="17"></td>
									</tr>
									<tr>
										<td vAlign="top" align="left" width="10"><IMG height="33" src="../layouts/images/photo-cornerbleft.gif" width="10"></td>
										<td vAlign="top" align="left"><IMG height="33" src="../layouts/images/photo-bgbottom.gif" width="100%"></td>
										<td vAlign="top" align="left"><IMG height="33" src="../layouts/images/photo-cornerbright.gif" width="17"></td>
									</tr>
								</table>
							</div>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step3" Runat="server">
	<usr:prefs id="ctlEditPrefs" runat="server"></usr:prefs>
</asp:panel>
<asp:panel id="step4" Runat="server">
	<BR>
	<table cellspacing="0" cellpadding="3" width="100%" border="0">
		<tr height="22">
			<td class="text" width="220" height="22"><B><%=LocRM.GetString("Selected") %>:</B></td>
			<td width="4">&nbsp;</td>
			<td><SPAN class="text"><B><%=LocRM.GetString("Available") %>:</B></SPAN></td>
		</tr>
		<tr style="HEIGHT: 220px">
			<td vAlign="top" width="300" height="100%"><!-- Data GRID -->
				<div style="OVERFLOW-Y: auto; HEIGHT: 220px">
					<asp:DataGrid id="dgMembers" Runat="server" Width="100%" borderwidth="0px" CellSpacing="3" gridlines="None"
						cellpadding="3" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False">
						<ItemStyle CssClass="ibn-propertysheet" Font-Size="8"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText='Name'>
								<ItemTemplate>
									<%# GetLink( (int)DataBinder.Eval(Container.DataItem, "UserId"),false)%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText='Name' itemstyle-width="50">
								<ItemTemplate>
									<%# GetLevel( (int)DataBinder.Eval(Container.DataItem, "Level"))%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:templatecolumn itemstyle-width="30" Visible="True">
								<itemtemplate>
									<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16"
										imageurl="../../layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
								</itemtemplate>
							</asp:templatecolumn>
						</Columns>
					</asp:DataGrid><!-- End Data GRID --></div>
			</td>
			<td width="4">&nbsp;</td>
			<td vAlign="top"><!-- Groups & Users -->
				<table class="text" style="MARGIN-TOP: 5px" cellspacing="0" cellpadding="3" width="100%">
					<tr>
						<td width="9%"><%=LocRM.GetString("Group") %>:</td>
						<td width="91%">
							<lst:indenteddropdownlist id="ddGroups" runat="server" Width="190px" CssClass="text" AutoPostBack="True" onselectedindexchanged="ddGroups_ChangeGroup"></lst:indenteddropdownlist></td>
					</tr>
					<tr>
						<td width="9%"><%=LocRM.GetString("Search") %>:</td>
						<td width="91%">
							<asp:TextBox id="tbSearch" runat="server" Width="125px" CssClass="text"></asp:TextBox>
							<asp:button id="btnSearch" runat="server" Width="60px" CssClass="text" CausesValidation="False" onclick="btnSearch_Click"></asp:button></td>
					</tr>
					<tr>
						<td vAlign="top"><%=LocRM.GetString("User") %>:</td>
						<td vAlign="top">
							<asp:listbox id="lbUsers" runat="server" Width="190px" CssClass="text" Rows="6" SelectionMode="Multiple"></asp:listbox></td>
					</tr>
					<tr>
						<td vAlign="top"></td>
						<td vAlign="top">
							<asp:CheckBox id="cbCanManage" runat="server"></asp:CheckBox></td>
					</tr>
					<tr>
						<td vAlign="top" height="28">&nbsp;</td>
						<td>
							<asp:button id="btnAdd" runat="server" Width="90px" CssClass="text" CausesValidation="False" onclick="btnAdd_Click"></asp:button></td>
					</tr>
				</table> <!-- End Groups & Users --></td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step5" Runat="server" Visible="false">
	<div class="text"><%=LocRM.GetString("s5TopDiv") %></div>
	<BR>
	<table width="100%">
		<tr>
			<td></td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="step6" Runat="server" Visible="false">
	<table height="100%" width="100%">
		<tr>
			<td width="100%">
				<table width="100%">
					<tr>
						<td width="40px"></td>
						<td valign="top" width="80px"><IMG src="../layouts/images/check.gif" align="absMiddle"></td>
						<td class="text" valign="top"><%=LocRM.GetString("s6TopDiv") %></td>
						<td width="40px"></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr vAlign="bottom">
			<td vAlign="bottom">
				<asp:CheckBox id="cbAllow" Runat="server" CssClass="text"></asp:CheckBox></td>
		</tr>
	</table>
</asp:panel>
<input type="hidden" runat="server" id="pastStep" value="0"> <INPUT id="hdnOffset" type="hidden" runat="server" value="111" NAME="hdnOffset">
<INPUT id="hdnBatch" type="hidden" runat="server" value="0" NAME="hdnBatch">