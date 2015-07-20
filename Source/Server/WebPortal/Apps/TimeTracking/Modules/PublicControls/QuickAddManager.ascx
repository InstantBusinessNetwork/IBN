<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickAddManager.ascx.cs" Inherits="Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls.QuickAddManager" %>
<%@ Reference Control="~/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="Time" src="~/Modules/TimeControl.ascx" %>
<%@ register TagPrefix="lst" namespace="Mediachase.Ibn.Web.UI.WebControls" Assembly="Mediachase.Ibn.Web.UI.WebControls" %>
<table cellpadding="0" cellspacing="5" width="100%" border="0">
	<tr>
		<td>
			<table cellspacing="5" cellpadding="0" width="100%" class="ibn-propertysheet" border="0" style="table-layout:fixed;">
				<tr>
					<td style="width: 185px;" valign="top">
						<asp:Literal ID="Literal2" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, _mc_SelectProject%>" />:
					</td>
					<td>
						<asp:Label runat="server" ID="labelNoItems" CssClass="ibn-alerttext" Text="<%$Resources : IbnFramework.TimeTracking, ProjectsNotFound%>"></asp:Label>
						<lst:IndentedDropDownList ID="ProjectList" Width="270px" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ProjectList_SelectedIndexChanged"></lst:IndentedDropDownList>
						<asp:Label ID="lblError2" runat="server" CssClass="text" ForeColor="Red" Text="*"></asp:Label>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<asp:Literal ID="Literal11" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, _mc_SelectUser%>" />:
					</td>
					<td>
						<asp:Label runat="server" ID="labelNoUsers" CssClass="ibn-alerttext" Text="<%$Resources : IbnFramework.TimeTracking, UsersNotFound%>"></asp:Label>
						<asp:DropDownList runat="server" ID="UserList" Width="270px"></asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td valign="top">
						<asp:Literal runat="server" ID="Literal1" Text="<%$Resources : IbnFramework.TimeTracking, _mc_WorkDescription%>"></asp:Literal>:
					</td>
					<td valign="top">
						<span class="ffBugFix">
						<asp:TextBox ID="txtEntry" runat="server" Width="270px"></asp:TextBox>
						<asp:RequiredFieldValidator runat="server" ID="EntryValidator" ErrorMessage="*" ControlToValidate="txtEntry" Display="Dynamic"></asp:RequiredFieldValidator>
						</span>
					</td>
				</tr>
				<tr>
					<td colspan="2" style="padding:0;padding-top:10px;" height="60px">
						<table id="tblDayWeek" runat="server" class="ibn-propertysheet" cellspacing="0" cellpadding="0" border="0" style="height: 40px; width: 490px;">
							<tr>
								<th style="width:70px;">
									<asp:Label runat="server" ID="Day1Label"></asp:Label>
								</th>
								<th style="width:70px;">
									<asp:Label runat="server" ID="Day2Label"></asp:Label>
								</th>
								<th style="width:70px;">
									<asp:Label runat="server" ID="Day3Label"></asp:Label>
								</th>
								<th style="width:70px;">
									<asp:Label runat="server" ID="Day4Label"></asp:Label>
								</th>
								<th style="width:70px;">
									<asp:Label runat="server" ID="Day5Label" CssClass="smallCurrent"></asp:Label>
								</th>
								<th style="width:70px;">
									<asp:Label runat="server" ID="Day6Label"></asp:Label>
								</th>
								<th>
									<asp:Label runat="server" ID="Day7Label"></asp:Label>
								</th>
							</tr>
							<tr>
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
						</table>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="center" style="padding-top:10px;">
						<asp:Button ID="AddButton" runat="server" OnClick="AddButton_Click" Text="<%$Resources : IbnFramework.TimeTracking, _mc_Add%>" Width="100px" />&nbsp;&nbsp;
						<asp:Button ID="AddCloseButton" runat="server" OnClick="AddCloseButton_Click" Text="<%$Resources : IbnFramework.TimeTracking, _mc_AddClose%>"  Width="130px" />&nbsp;&nbsp;
						<asp:Button ID="CancelButton" runat="server" Text="<%$Resources : IbnFramework.TimeTracking, _mc_Close%>" Width="100px" />	
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>