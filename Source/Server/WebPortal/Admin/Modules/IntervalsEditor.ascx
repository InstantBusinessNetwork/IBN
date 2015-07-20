<%@ Reference Page="~/Admin/IntervalsEditor.aspx" %>
<%@ Reference Control="~/Admin/Modules/TimeControl.ascx" %>
<%@ Reference Control="~/Modules/BlockHeader.ascx" %>
<%@ Register TagPrefix="ibn" TagName="DateTimeControl" src="~/Admin/Modules/TimeControl.ascx" %>
<%@ Register TagPrefix="ibn" TagName="BlockHeader" src="~/Modules/BlockHeader.ascx" %>
<%@ Control Language="c#" Inherits="Mediachase.UI.Web.Admin.Modules.IntervalsEditor" Codebehind="IntervalsEditor.ascx.cs" %>
<table class="ibn-stylebox2" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td><ibn:blockheader id="secHeader" runat="server" /></td>
	</tr>
	<tr>
		<td style="PADDING:7px">
			<asp:RadioButton GroupName="G1" ID="rbWorking" Runat="server" CssClass="text" Text='<%# LocRM.GetString("WorkingDay") %>' AutoPostBack="True" oncheckedchanged="rb_ChangeWorkDay">
			</asp:RadioButton><br>
			<asp:RadioButton GroupName="G1" ID="Radiobutton1" Runat="server" CssClass="text" Text='<%# LocRM.GetString("NoneWorkingDay") %>' AutoPostBack="True" oncheckedchanged="rb_ChangeWorkDay">
			</asp:RadioButton>
			<br>
			<asp:CustomValidator id="cvNoOneWorkingInterval" runat="server" ErrorMessage='<%# LocRM.GetString("EmptyCalendar") %>' CssClass="Text">
			</asp:CustomValidator>
			<br>
			<fieldset style="PADDING:8px" align="bottom"><legend class="text"><%=LocRM.GetString("WorkTimeIntervals") %></legend>
				<table border="0" cellpadding="5" cellspacing="0" class="text" align="left">
					<tr>
						<td>
							<ibn:datetimecontrol id="tc1" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc2" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc3" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc4" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
					<tr>
						<td>
							<ibn:datetimecontrol id="tc5" runat="server"></ibn:datetimecontrol>
						</td>
					</tr>
				</table>
				<asp:CustomValidator id="CustomValidator1" runat="server" ErrorMessage='<%# LocRM.GetString("IncorrectIntervals") %>' CssClass="text">
				</asp:CustomValidator>
			</fieldset>
		</td>
		</td>
	</tr>
</table>
<button id="btnSave" runat=server type=button style="display:none" onserverclick="Button1_Click"></button>
