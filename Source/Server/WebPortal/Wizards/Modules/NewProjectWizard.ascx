<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Wizards.Modules.NewProjectWizard" Codebehind="NewProjectWizard.ascx.cs" %>
<%@ Reference Control="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ Register TagPrefix="mc" TagName="Picker" Src="~/Apps/Common/DateTimePickerAjax/PickerControl.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<asp:panel id="step1" Runat="server">
	<div class=text><%=LocRM.GetString("s1TopText") %></div>
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=top width="50%">
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("s1ProjectTitle") %></div>
				<asp:TextBox id=tbTitle Runat="server" Width="260"></asp:TextBox>
				<asp:RequiredFieldValidator id=s1rfTitle Runat="server" Display="Dynamic" ErrorMessage="*" ControlToValidate="tbTitle"></asp:RequiredFieldValidator>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 7px; PADDING-TOP: 13px"><%=LocRM.GetString("s1ProjectType") %></div>
				<asp:DropDownList id=ddType Runat="server" Width="260"></asp:DropDownList>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 7px; PADDING-TOP: 8px"><%=LocRM.GetString("s1Priority") %></div>
				<asp:DropDownList id="ddPriority" Runat="server" Width="260"></asp:DropDownList>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 7px; PADDING-TOP: 13px"><%=LocRM.GetString("s1Phase") %></div>
				<asp:DropDownList id="ddPhase" Runat="server" Width="260"></asp:DropDownList>
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 7px; PADDING-TOP: 13px"><%=LocRM2.GetString("calendar") %>:</div>
				<asp:dropdownlist id="ddlCalendar" runat="server" Width="260px"></asp:dropdownlist>
			</td>
			<td vAlign=top align=middle width=60><IMG alt="" src="../layouts/images/quicktip.gif" border=0>
			</td>
			<td class=text style="PADDING-RIGHT: 15px" vAlign=top><%=LocRM.GetString("s1QuickTip") %></td>
		</tr>
	</table>
</asp:panel><asp:panel id="step2" Runat="server">
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=top width="50%">
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("s2StartDate") %></div>
				<mc:Picker ID="dtcStartDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px"><%=LocRM.GetString("s2EndDate") %></div>
				<mc:Picker ID="dtcEndDate" runat="server" DateCssClass="text" DateWidth="85px" ShowImageButton="false" DateIsRequired="true" />
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-TOP: 10px; PADDING-BOTTOM: 10px"><%=LocRM.GetString("ProjectCurrency") %>:</div>
				<asp:DropDownList id=ddCurrency Runat="server" Width="260"></asp:DropDownList>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 5px; PADDING-TOP: 10px"><%=LocRM.GetString("s2Executive") %></div>
				<asp:DropDownList id=ddExecutive Runat="server" Width="260"></asp:DropDownList>
				<div class=subHeader><%=LocRM.GetString("s2ExecutiveComment") %></div>
				<div class="subHeader" style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 5px; PADDING-TOP: 13px"><%=GetGlobalResourceObject("IbnFramework.TimeTracking", "TimeTrackingBlockType").ToString()%>:</div>
				<asp:DropDownList ID="ddlBlockType" runat="server" Width="260px"></asp:DropDownList>
			</td>
			<td vAlign=top align=middle width=60><IMG alt="" src="../layouts/images/quicktip.gif" border=0>
			</td>
			<td class=text style="PADDING-RIGHT: 15px" vAlign=top><%=LocRM.GetString("s2QuickTip") %></td>
		</tr>
	</table>
</asp:panel><asp:panel id="step3" Runat="server">
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=top width="50%">
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><%=LocRM.GetString("s3Category") %></div>
				<asp:ListBox id=lbCategories Width="260px" runat="server" Rows="6" SelectionMode="Multiple"></asp:ListBox>
				<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 2px; PADDING-BOTTOM: 5px; PADDING-TOP: 10px"><%=LocRM.GetString("s3Portfolio") %></div>
				<asp:ListBox ID="lbPortfolios" Width="260px" Runat=server Rows="6" SelectionMode="Multiple"></asp:ListBox>
			</td>
			<td vAlign=top align=middle width=60><IMG alt="" src="../layouts/images/quicktip.gif" border=0>
			</td>
			<td class=text style="PADDING-RIGHT: 15px" vAlign=top><%=LocRM.GetString("s3QuickTip") %></td>
		</tr>
	</table>
</asp:panel><asp:panel id="step4" Runat="server">
	<div class=text><IMG height=20 src="../layouts/images/help.gif" width=20 align=left>
		<%=LocRM.GetString("s4TopText") %>
	</div>
	<BR>
	<table class="text" style="HEIGHT: 100%" height="100%" cellspacing=0 cellpadding=3 width="100%" border=0>
		<tr height=22>
			<td class="boldtext" width=350 height=22><%=LocRM.GetString("s4Selected") %>:</td>
			<td width=4>&nbsp;</td>
			<td class=boldtext><%=LocRM.GetString("s4Available") %>:</td>
		</tr>
		<tr style="HEIGHT: 100%">
			<td vAlign=top width=350 height="100%"><!-- Data GRID -->
				<div style="OVERFLOW-Y: auto; HEIGHT: 200px">
					<asp:DataGrid id=dgMembers Runat="server" Width="100%" borderwidth="0px" CellSpacing="0" gridlines="None" cellpadding="0" AllowSorting="False" AllowPaging="False" AutoGenerateColumns="False">
						<ItemStyle CssClass="text"></ItemStyle>
						<HeaderStyle CssClass="text"></HeaderStyle>
						<Columns>
							<asp:BoundColumn DataField="UserId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText='Name'>
								<ItemTemplate>
									<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText='Code' itemstyle-width="50">
								<ItemTemplate>
									<input type="text" class="text" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "Code") %>' id="tCode"  maxlength="2" style="Width:30px" NAME="tCode"/>
									<asp:RequiredFieldValidator ID="vCode" Runat="server" ControlToValidate="tCode" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText='Rate' itemstyle-width="70">
								<ItemTemplate>
									<input type="text" class="text" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "Rate") %>' id="tRate"  style="Width:50px" NAME="tRate"/>
									<asp:RequiredFieldValidator ID="Requiredfieldvalidator1" Runat="server" ControlToValidate="tRate" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
									<asp:RangeValidator Type="Integer" MinimumValue="0" MaximumValue="100000" ID="Requiredfieldvalidator2" Runat="server" ControlToValidate="tRate" ErrorMessage="*" Display="Dynamic" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:templatecolumn itemstyle-width="30" Visible="True">
								<itemtemplate>
									<asp:imagebutton id="ibDelete" runat="server" borderwidth="0" width="16" height="16" imageurl="../../layouts/images/DELETE.GIF" commandname="Delete" causesvalidation="False"></asp:imagebutton>
								</itemtemplate>
							</asp:templatecolumn>
						</Columns>
					</asp:DataGrid><!-- End Data GRID --></div>
			</td>
			<td width=4>&nbsp;</td>
			<td vAlign=top><!-- Groups & Users -->
				<table class=text style="MARGIN-TOP: 5px" cellspacing=0 cellpadding=3 width="100%">
					<tr>
						<td style="HEIGHT: 18px" width="9%"><%=LocRM.GetString("s4Group") %>:</td>
						<td style="HEIGHT: 18px" width="91%">
							<LST:INDENTEDDROPDOWNLIST id=ddGroups Width="190px" runat="server" AutoPostBack="True" CssClass="text" onselectedindexchanged="ddGroups_ChangeGroup"></LST:INDENTEDDROPDOWNLIST></td>
					</tr>
					<tr>
						<td width="9%"><%=LocRM.GetString("s4Search") %>:</td>
						<td width="91%">
							<asp:TextBox id=tbSearch Width="125px" runat="server" CssClass="text"></asp:TextBox>
							<asp:button id=btnSearch Width="60px" runat="server" CssClass="text" CausesValidation="False" onclick="btnSearch_Click"></asp:button></td>
					</tr>
					<tr>
						<td vAlign=top height=96><%=LocRM.GetString("s4User") %>:</td>
						<td vAlign=top>
							<asp:listbox id=lbUsers Width="190px" runat="server" Rows="6" SelectionMode="Multiple" CssClass="text"></asp:listbox></td>
					</tr>
					<tr>
						<td vAlign=top height=28>&nbsp;</td>
						<td><BUTTON class=text id=btnAdd style="WIDTH: 90px" type=button runat="server" onserverclick="btnAdd_Click"></BUTTON>
							<asp:Button id=btnSave style="DISPLAY: none" runat="server" Text="Button"></asp:Button></td>
					</tr>
				</table> <!-- End Groups & Users --></td>
		</tr>
	</table>
</asp:panel><asp:panel id="step5" Runat="server">
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=top>
				<table class=text cellspacing=3 cellpadding=3 width="100%" border=0>
					<tr>
						<td><%=LocRM.GetString("s5Description") %></td>
					</tr>
					<tr>
						<td>
							<asp:textbox class=text id=txtDescription Width="260px" runat="server" CssClass="text" TextMode="MultiLine" Height="105px"></asp:textbox></td>
					</tr>
					<tr>
						<td><%=LocRM.GetString("s5Scope") %></td>
					</tr>
					<tr>
						<td>
							<asp:textbox id=txtScope Width="260px" runat="server" CssClass="text" TextMode="MultiLine" Height="105px" DESIGNTIMEDRAGDROP="1295"></asp:textbox></td>
					</tr>
				</table>
			</td>
			<td vAlign=top>
				<table class=text cellspacing=3 cellpadding=3 width="100%" border=0>
					<tr>
						<td><%=LocRM.GetString("s5Goals") %></td>
					</tr>
					<tr>
						<td>
							<asp:textbox id=txtGoals Width="260px" runat="server" CssClass="text" TextMode="MultiLine" Height="105px"></asp:textbox></td>
					</tr>
					<tr>
						<td><%=LocRM.GetString("s5Deliverables") %></td>
					</tr>
					<tr>
						<td>
							<asp:textbox id=txtDeliverables Width="260px" runat="server" CssClass="text" TextMode="MultiLine" Height="105px"></asp:textbox></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel><asp:panel id="step6" Runat="server" Visible="False">
	<div class=text><IMG height=20 src="../layouts/images/help.gif" width=20 align=absMiddle>
		&nbsp;<%=LocRM.GetString("s6TopText") %>
	</div>
	<BR>
	<table cellspacing="0" cellpadding="0" width="100%" border=0>
		<tr>
			<td vAlign=top width="50%">
				<table cellspacing="0" cellpadding="0" width="100%" border=0>
					<tr>
						<td vAlign=top>
							<table class=subHeader cellspacing=0 cellpadding=5 width="100%" border=0>
								<tr>
									<td width=100><B><%=LocRM.GetString("s6Project") %>:</B>
									</td>
									<td>
										<asp:Label id=lblProjectTitle Runat="server"></asp:Label></td>
								</tr>
								<tr>
									<td><B><%=LocRM.GetString("s6StartDate") %>:</B>
									</td>
									<td>
										<asp:Label id=lblStartDate Runat="server"></asp:Label></td>
								</tr>
								<tr>
									<td><B><%=LocRM.GetString("s6EndDate") %>:</B>
									</td>
									<td>
										<asp:Label id=lblEndDate Runat="server"></asp:Label></td>
								</tr>
								<tr>
									<td><B><%=LocRM.GetString("s6ProjectType") %>:</B>
									</td>
									<td>
										<asp:Label id=lblType Runat="server"></asp:Label></td>
								</tr>
								<tr>
									<td><B><%=LocRM.GetString("s6Executive") %>:</B>
									</td>
									<td>
										<asp:Label id=lblExecutive Runat="server"></asp:Label></td>
								</tr>
							</table>
						</td>
						<td vAlign=top width=200>
							<div class=subHeader style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 10px; PADDING-TOP: 3px"><B><%=LocRM.GetString("s6Team") %>:</B></div>
							<div style="OVERFLOW-Y:auto;height:120px;">
							<asp:DataList id=dlTeam Runat="server" CssClass="subTitle">
								<ItemTemplate>
									<%# Mediachase.UI.Web.Util.CommonHelper.GetUserStatusUL((int)DataBinder.Eval(Container.DataItem, "UserId"))%>
								</ItemTemplate>
							</asp:DataList>
							</div>
							<asp:CheckBox id=cbNotify Runat="server" CssClass="cb"></asp:CheckBox></td>
					</tr>
					<tr>
						<td colSpan=2>
							<table class=subHeader cellspacing=0 cellpadding=5 width="100%" border=0>
								<tr>
									<td width=100><B><%=LocRM.GetString("s6Description") %>:</B></td>
									<td><asp:Label id=lblDescription Runat="server"></asp:Label></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:panel><asp:panel id="step7" Runat="server" Visible="False">
	<div class=text><IMG height=20 src="../layouts/images/help.gif" width=20 align=absMiddle>&nbsp;<%=LocRM.GetString("s7TopText") %></div>
	<BR>
	<div class=SubHeader><B></B><BR>
		<BR>
	</div>
	<div class=SubHeader style="PADDING-LEFT: 20px"><%=LocRM.GetString("s7Text") %></div>
</asp:panel>
